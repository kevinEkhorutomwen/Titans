namespace Titans.Application.Tests.Commands;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Titans.Application.Commands;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;
using Titans.Domain;
using Xunit;

public class RegisterUserApplicationServiceTests
{
    readonly IFixture _fixture = new Fixture();
    readonly IServiceProvider _serviceProvider;
    readonly CancellationToken _cancellationToken = CancellationToken.None;
    public RegisterUserApplicationServiceTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddScoped(x => Substitute.For<IUserRepository>())
            .AddScoped<RegisterUserApplicationService>()
            .BuildServiceProvider();
    }

    [Fact]
    public async void Handle_PasswordsAreNotIdentical_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RegisterUserApplicationService>();
        var command = _fixture.Create<RegisterUserCommand>();

        // Act
        var userResponse = await service.Handle(command, _cancellationToken);

        // Assert
        userResponse.Error.Should().BeEquivalentTo(new Error(ErrorMessages.PasswordMustBeIdentical));
        await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<Domain.User.User>());
        await userRepository.DidNotReceiveWithAnyArgs().FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_UsernameAlreadyExist_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RegisterUserApplicationService>();
        var command = CreateCommand();
        userRepository.UserAlreadyExist(Arg.Any<string>()).ReturnsForAnyArgs(true);

        // Act
        var userResponse = await service.Handle(command, _cancellationToken);

        // Assert
        userResponse.Error.Should().BeEquivalentTo(new Error(ErrorMessages.UserAlreadyExist));
        await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<Domain.User.User>());
    }

    [Fact]
    public async void Handle_UsernameDosntExist_DontThrowError()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RegisterUserApplicationService>();
        var command = CreateCommand();
        userRepository.UserAlreadyExist(Arg.Any<string>()).ReturnsForAnyArgs(false);

        // Act
        var userResponse = await service.Handle(command, _cancellationToken);

        // Assert
        userResponse.Error.Should().BeNull();
        await userRepository.ReceivedWithAnyArgs(1).CreateAsync(Arg.Any<Domain.User.User>());
    }

    private RegisterUserCommand CreateCommand()
    {
        var password = _fixture.Create<string>();

        return _fixture.Build<RegisterUserCommand>()
            .With(x => x.Password, password)
            .With(x => x.ConfirmPassword, password)
            .Create();
    }
}