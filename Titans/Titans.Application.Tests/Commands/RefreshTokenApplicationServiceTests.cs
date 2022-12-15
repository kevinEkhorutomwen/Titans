namespace Titans.Application.Tests.Commands;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Titans.Application.Commands;
using Titans.Application.Repositories;
using Titans.Contract.Command;
using Titans.Domain;
using Titans.Domain.User;
using Xunit;

public class RefreshTokenApplicationServiceTests
{
    readonly IFixture _fixture = new Fixture();
    readonly IServiceProvider _serviceProvider;
    readonly CancellationToken _cancellationToken = CancellationToken.None;
    public RefreshTokenApplicationServiceTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddScoped(x => Substitute.For<IUserRepository>())
            .AddScoped(x => Substitute.For<IMapper>())
            .AddScoped<RefreshTokenApplicationService>()
            .BuildServiceProvider();
    }
    [Fact]
    public async void Handle_UserNotFound_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
        var command = _fixture.Create<RefreshTokenCommand>();

        // Act
        var tokenResponse = await service.Handle(command, _cancellationToken);

        // Assert
        tokenResponse.Error.Should().BeEquivalentTo(new Contract.Models.v1.Error(ErrorMessages.UserNotFound(command.Username)));
        await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_TokenInvalid_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
        var command = _fixture.Create<RefreshTokenCommand>();
        var user = UserFixtures.Create();
        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        var tokenResponse = await service.Handle(command, _cancellationToken);

        // Assert
        tokenResponse.Error.Should().BeEquivalentTo(new Contract.Models.v1.Error(ErrorMessages.TokenInvalid));
        await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_TokenAbgelaufen_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
        var command = _fixture.Create<RefreshTokenCommand>();
        var user = UserFixtures.Create();
        user.UpdateRefreshToken(RefreshToken.Create(command.CurrentToken, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)));
        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        var tokenResponse = await service.Handle(command, _cancellationToken);

        // Assert
        tokenResponse.Error.Should().BeEquivalentTo(new Contract.Models.v1.Error(ErrorMessages.TokenExpired));
        await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_UserFound_DataShouldNotBeNull()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
        var command = _fixture.Create<RefreshTokenCommand>();
        var user = UserFixtures.Create();
        user.UpdateRefreshToken(RefreshToken.Create(command.CurrentToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(7)));
        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        var tokenResponse = await service.Handle(command, _cancellationToken);

        // Assert
        tokenResponse.Data.Should().NotBeNull();
        await userRepository.ReceivedWithAnyArgs(1).UpdateAsync(Arg.Any<User>());
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }
}