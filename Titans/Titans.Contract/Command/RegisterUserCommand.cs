using MediatR;

namespace Titans.Contract.Models.v1
{
    public class RegisterUserCommand : INotification
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
