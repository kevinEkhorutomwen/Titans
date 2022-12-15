namespace Titans.Domain.User;
using FluentValidation;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public byte[] PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }
    public RefreshToken? RefreshToken { get; private set; }

    private User(string username, byte[] passwordHash, byte[] passwordSalt)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public static User Create(string username, byte[] passwordHash, byte[] passwordSalt)
    {
        var user = new User(username, passwordHash, passwordSalt);
        user.EnsureValidState();
        return user;
    }

    public void UpdateRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken = refreshToken;
    }

    private void EnsureValidState()
    {
        new Validator().ValidateAndThrow(this);
    }

    class Validator : AbstractValidator<User>
    {
        public Validator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(ErrorMessages.CanNotBeEmpty(nameof(Username)));
            RuleFor(x => x.PasswordHash).NotEmpty().WithMessage(ErrorMessages.CanNotBeEmpty(nameof(PasswordHash)));
            RuleFor(x => x.PasswordSalt).NotEmpty().WithMessage(ErrorMessages.CanNotBeEmpty(nameof(PasswordSalt)));
        }
    }
}