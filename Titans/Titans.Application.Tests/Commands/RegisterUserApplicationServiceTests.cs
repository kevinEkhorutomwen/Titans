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

namespace Titans.Application.Tests.Commands
{
    public class RegisterUserApplicationServiceTests
    {
        readonly IFixture _fixture = new Fixture();
        readonly IServiceProvider _serviceProvider;
        public RegisterUserApplicationServiceTests()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped(x => Substitute.For<IUserRepository>())
                .AddScoped<RegisterUserApplicationService>()
                .BuildServiceProvider();
        }

        [Fact]
        public async void RunAsync_PasswordsAreNotIdentical_ThrowException()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RegisterUserApplicationService>();
            var command = _fixture.Create<RegisterUserCommand>();

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.PasswordMustBeIdentical);
            await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<Domain.User.User>());
            await userRepository.DidNotReceiveWithAnyArgs().FindAsyncByUsername(Arg.Any<string>());
        }

        [Fact]
        public async void RunAsync_UsernameAlreadyExist_ThrowException()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RegisterUserApplicationService>();
            var command = CreateCommand();
            var user = UserFixtures.Create();
            userRepository.FindAsyncByUsername(Arg.Any<string>()).ReturnsForAnyArgs(user);

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage(ErrorMessages.UserAlreadyExist);
            await userRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<Domain.User.User>());
            await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
        }

        [Fact]
        public async void RunAsync_UsernameDosntExist_DontThrowError()
        {
            // Arrang
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var service = _serviceProvider.GetRequiredService<RegisterUserApplicationService>();
            var command = CreateCommand();

            // Act
            Func<Task> act = async () => await service.RunAsync(command);

            // Assert
            await act.Should().NotThrowAsync();
            await userRepository.ReceivedWithAnyArgs(1).CreateAsync(Arg.Any<Domain.User.User>());
            await userRepository.ReceivedWithAnyArgs(1).FindAsyncByUsername(Arg.Any<string>());
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
}
