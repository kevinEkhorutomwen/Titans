using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Titans.Domain.User;
using Xunit;

namespace Titans.Domain.Tests.User
{
    public class UserTests
    {
        readonly IFixture _fixture = new Fixture().RegisterDomainCreatorFunctions();

        [Fact]
        public void Create_WithoutUsername_ThrowValidationException()
        {
            // Arrang
            var passwordSalt = _fixture.Create<byte[]>();
            var passwordHash = _fixture.Create<byte[]>();

            // Act
            Func<Domain.User.User> act = () => Domain.User.User.Create(string.Empty, passwordHash, passwordSalt);

            // Assert
            var missingProp = nameof(Domain.User.User.Username);
            act.Should().Throw<ValidationException>()
                .WithMessage(ValidationExceptionExtensions.GetDomainValidationErrorText(missingProp, ErrorMessages.CanNotBeEmpty(missingProp)));
        }

        [Fact]
        public void Create_WithoutHash_ThrowValidationException()
        {
            // Arrang
            var username = _fixture.Create<string>();
            var passwordSalt = _fixture.Create<byte[]>();

            // Act
            Func<Domain.User.User> act = () => Domain.User.User.Create(username, Array.Empty<byte>(), passwordSalt);

            // Assert
            var missingProp = nameof(Domain.User.User.PasswordHash);
            act.Should().Throw<ValidationException>()
                .WithMessage(ValidationExceptionExtensions.GetDomainValidationErrorText(missingProp, ErrorMessages.CanNotBeEmpty(missingProp)));
        }

        [Fact]
        public void Create_WithoutSalt_ThrowValidationException()
        {
            // Arrang
            var username = _fixture.Create<string>();
            var passwordHash = _fixture.Create<byte[]>();

            // Act
            Func<Domain.User.User> act = () => Domain.User.User.Create(username, passwordHash, Array.Empty<byte>());

            // Assert
            var missingProp = nameof(Domain.User.User.PasswordSalt);
            act.Should().Throw<ValidationException>()
                .WithMessage(ValidationExceptionExtensions.GetDomainValidationErrorText(missingProp, ErrorMessages.CanNotBeEmpty(missingProp)));
        }

        [Fact]
        public void Create_PersonGetCreated_PropertiesAreSetCorrectly()
        {
            // Arrang
            var username = _fixture.Create<string>();
            var passwordHash = _fixture.Create<byte[]>();
            var passwordSalt = _fixture.Create<byte[]>();

            // Act
            Func<Domain.User.User> act = () => Domain.User.User.Create(username, passwordHash, passwordSalt);
            var user = act();

            // Assert
            act.Should().NotThrow();
            user.Username.Should().Be(username);
            user.PasswordHash.Should().BeEquivalentTo(passwordHash);
        }

        [Fact]
        public void UpdateRefreshToken_RefreshTokenGetsUpdated_PropertiesAreSetCorrectly()
        {
            // Arrang
            var refreshToken = _fixture.Create<RefreshToken>();
            var user = _fixture.Create<Domain.User.User>();

            // Act
            user.UpdateRefreshToken(refreshToken);

            // Assert
            user.RefreshToken.Should().Be(refreshToken);
        }
    }
}
