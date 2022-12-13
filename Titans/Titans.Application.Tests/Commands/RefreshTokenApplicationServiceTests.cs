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

namespace Titans.Application.Tests.Commands
{
    public class RefreshTokenApplicationServiceTests
    {
        readonly IFixture _fixture = new Fixture();
        readonly IServiceProvider _serviceProvider;
        public RefreshTokenApplicationServiceTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped(x => Substitute.For<IUserRepository>())
                .AddScoped(x => Substitute.For<IMapper>())
                .AddScoped<RefreshTokenApplicationService>()
                .BuildServiceProvider();
        }
        [Fact]
        public async void RunAsync_UserNotFound_ThrowException()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
            var command = _fixture.Create<RefreshTokenCommand>();

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.UserNotFound(command.Username));
            await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
            await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
        }

        [Fact]
        public async void RunAsync_TokenInvalid_ThrowException()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
            var command = _fixture.Create<RefreshTokenCommand>();
            var user = UserFixtures.Create();
            userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.TokenInvalid);
            await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
            await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
        }

        [Fact]
        public async void RunAsync_TokenAbgelaufen_ThrowException()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
            var command = _fixture.Create<RefreshTokenCommand>();
            var user = UserFixtures.Create();
            user.UpdateRefreshToken(RefreshToken.Create(command.CurrentToken, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)));
            userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.TokenExpired);
            await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<User>());
            await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
        }

        [Fact]
        public async void RunAsync_UserFound_DontThrowError()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RefreshTokenApplicationService>();
            var command = _fixture.Create<RefreshTokenCommand>();
            var user = UserFixtures.Create();
            user.UpdateRefreshToken(RefreshToken.Create(command.CurrentToken, DateTime.UtcNow, DateTime.UtcNow.AddDays(7)));
            userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().NotThrowAsync();
            await userRepository.ReceivedWithAnyArgs(1).UpdateAsync(Arg.Any<User>());
            await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
        }
    }
}