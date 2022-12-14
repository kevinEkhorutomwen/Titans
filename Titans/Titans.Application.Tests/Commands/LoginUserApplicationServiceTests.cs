namespace Titans.Application.Tests.Commands;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Cryptography;
using Titans.Application.Commands;
using Titans.Application.Repositories;
using Titans.Contract.Command;
using Titans.Contract.Interfaces;
using Titans.Domain;
using Xunit;

public class LoginUserApplicationServiceTests
{
    readonly IFixture _fixture = new Fixture();
    readonly IServiceProvider _serviceProvider;
    readonly CancellationToken _cancellationToken = CancellationToken.None;
    public LoginUserApplicationServiceTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddScoped(x => Substitute.For<IUserRepository>())
            .AddScoped(x => Substitute.For<ISettings>())
            .AddScoped<LoginUserApplicationService>()
            .BuildServiceProvider();
    }

    [Fact]
    public async void Handle_DidntFindAnyUser_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<LoginUserApplicationService>();
        var command = _fixture.Create<LoginUserCommand>();

        // Act
        Func<Task> act = async () => await service.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.UserNotFound(command.Username));
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_PasswordDosntMatch_ThrowException()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<LoginUserApplicationService>();
        var command = _fixture.Create<LoginUserCommand>();
        var user = UserFixtures.Create();

        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        Func<Task> act = async () => await service.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.WrongPassword);
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_PasswordAndUserMatch_DontThrowError()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<LoginUserApplicationService>();
        var settings = _serviceProvider.GetRequiredService<ISettings>();
        var command = _fixture.Create<LoginUserCommand>();
        CreatePasswordHash(command.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = UserFixtures.Create(passwordHash: passwordHash, passwordSalt: passwordSalt);
        settings.Token.ReturnsForAnyArgs(_fixture.Create<string>());

        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        Func<Task> act = async () => await service.Handle(command, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_PasswordAndUserMatch_ReturnToken()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<LoginUserApplicationService>();
        var settings = _serviceProvider.GetRequiredService<ISettings>();
        var command = _fixture.Create<LoginUserCommand>();
        CreatePasswordHash(command.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = UserFixtures.Create(passwordHash: passwordHash, passwordSalt: passwordSalt);
        settings.Token.ReturnsForAnyArgs(_fixture.Create<string>());

        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        var token = await service.Handle(command, _cancellationToken);

        // Assert
        token.Should().NotBeEmpty();
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}