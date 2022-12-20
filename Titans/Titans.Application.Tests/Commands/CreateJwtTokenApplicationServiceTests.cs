namespace Titans.Application.Tests.Commands;
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

public class CreateJwtTokenApplicationServiceTests
{
    readonly IFixture _fixture = new Fixture();
    readonly IServiceProvider _serviceProvider;
    readonly CancellationToken _cancellationToken = CancellationToken.None;
    public CreateJwtTokenApplicationServiceTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddScoped(x => Substitute.For<IUserRepository>())
            .AddScoped(x => Substitute.For<IOptions<AppSettingsOptions>>())
            .AddScoped<CreateJwtTokenApplicationService>()
            .BuildServiceProvider();
    }

    [Fact]
    public async void Handle_PasswordAndUserMatch_ReturnToken()
    {
        // Arrang
        var service = _serviceProvider.GetRequiredService<CreateJwtTokenApplicationService>();
        var settings = _serviceProvider.GetRequiredService<IOptions<AppSettingsOptions>>();
        var command = _fixture.Create<CreateJwtTokenCommand>();
        settings.Value.ReturnsForAnyArgs(_fixture.Create<AppSettingsOptions>());

        // Act
        var token = await service.Handle(command, _cancellationToken);

        // Assert
        token.Error.Should().BeNull();
    }   
}