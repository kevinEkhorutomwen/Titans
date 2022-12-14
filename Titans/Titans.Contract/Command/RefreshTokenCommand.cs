using MediatR;
using Titans.Contract.Models.v1;

namespace Titans.Contract.Command
{
    public record RefreshTokenCommand : IRequest<RefreshToken>
    {
        public string Username { get; init; } = string.Empty;
        public string CurrentToken { get; init; } = string.Empty;
    }
}
