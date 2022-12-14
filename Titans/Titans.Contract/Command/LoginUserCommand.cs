using MediatR;

namespace Titans.Contract.Command
{
    public record LoginUserCommand : IRequest<string>
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
