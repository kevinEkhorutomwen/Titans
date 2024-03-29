﻿namespace Titans.Application.Tests.Commands;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Cryptography;
using Titans.Application.Commands;
using Titans.Application.Repositories;
using Titans.Contract;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
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
            .AddScoped(x => Substitute.For<IOptions<AppSettingsOptions>>())
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
        var tokenResponse = await service.Handle(command, _cancellationToken);

        // Assert
        tokenResponse.Error.Should().BeEquivalentTo(new Error(ErrorMessages.UserNotFound(command.Username)));
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
        var tokenResponse = await service.Handle(command, _cancellationToken);

        // Assert
        tokenResponse.Error.Should().BeEquivalentTo(new Error(ErrorMessages.WrongPassword));
        await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
    }

    [Fact]
    public async void Handle_PasswordAndUserMatch_ReturnToken()
    {
        // Arrang
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var service = _serviceProvider.GetRequiredService<LoginUserApplicationService>();
        var settings = _serviceProvider.GetRequiredService<IOptions<AppSettingsOptions>>();
        var command = _fixture.Create<LoginUserCommand>();
        CreatePasswordHash(command.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = UserFixtures.Create(passwordHash: passwordHash, passwordSalt: passwordSalt);
        settings.Value.ReturnsForAnyArgs(_fixture.Create<AppSettingsOptions>());

        userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

        // Act
        var token = await service.Handle(command, _cancellationToken);

        // Assert
        token.Data.Should().NotBeEmpty();
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