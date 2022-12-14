using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Titans.Domain.User;
using Xunit;

namespace Titans.Domain.Tests.User
{
    public class RefreshTokenTests
    {
        readonly IFixture _fixture = new Fixture().RegisterDomainCreatorFunctions();

        [Fact]
        public void Create_WithoutToken_ThrowValidationException()
        {
            // Arrang
            var created = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddDays(7);

            // Act
            Func<RefreshToken> act = () => RefreshToken.Create(string.Empty, created, expires);

            // Assert
            var missingProp = nameof(RefreshToken.Token);
            act.Should().Throw<ValidationException>()
                .WithMessage(ValidationExceptionExtensions.GetDomainValidationErrorText(missingProp, ErrorMessages.CanNotBeEmpty(missingProp)));
        }

        [Fact]
        public void Create_WithInvalidDate_ThrowValidationException()
        {
            // Arrang
            var token = _fixture.Create<string>();
            var created = DateTime.UtcNow.AddDays(1);
            var expires = DateTime.UtcNow;

            // Act
            Func<RefreshToken> act = () => RefreshToken.Create(token, created, expires);

            // Assert
            var missingProp = nameof(RefreshToken.Created);
            act.Should().Throw<ValidationException>()
                .WithMessage(ValidationExceptionExtensions.GetDomainValidationErrorText(missingProp, ErrorMessages.CreationDateCantBefAfterExpiredDate));
        }

        [Fact]
        public void Create_RefreshTokenGetsCreated_PropertiesAreSetCorrectly()
        {
            var token = _fixture.Create<string>();
            var created = DateTime.UtcNow;
            var expires = DateTime.UtcNow.AddDays(1);

            // Act
            Func<RefreshToken> act = () => RefreshToken.Create(token, created, expires);
            var refreshToken = act();

            // Assert
            act.Should().NotThrow();
            refreshToken.Token.Should().Be(token);
            refreshToken.Created.Should().Be(created);
            refreshToken.Expires.Should().Be(expires);
        }
    }
}
