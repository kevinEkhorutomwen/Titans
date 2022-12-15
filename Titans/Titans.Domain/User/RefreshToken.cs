namespace Titans.Domain.User;
using FluentValidation;

public class RefreshToken
{
    public string Token { get; private set; } = string.Empty;
    public DateTime Created { get; private set; }
    public DateTime Expires { get; private set; }

    private RefreshToken(string token, DateTime created, DateTime expires)
    {
        Token = token;
        Created = created;
        Expires = expires;
    }

    public static RefreshToken Create(string token, DateTime created, DateTime expires)
    {
        var refreshToken = new RefreshToken(token, created, expires);
        refreshToken.EnsureValidState();
        return refreshToken;
    }

    private void EnsureValidState()
    {
        new Validator().ValidateAndThrow(this);
    }

    class Validator : AbstractValidator<RefreshToken>
    {
        public Validator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage(ErrorMessages.CanNotBeEmpty(nameof(Token)));
            RuleFor(x => x.Created)
                .LessThanOrEqualTo(x => x.Expires)
                .WithMessage(ErrorMessages.CreationDateCantBefAfterExpiredDate);
        }
    }
}