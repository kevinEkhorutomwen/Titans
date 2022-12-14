using MediatR;

namespace Titans.Contract.Models.v1
{
    public record RegisterUserCommand : INotification
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
    }
}
