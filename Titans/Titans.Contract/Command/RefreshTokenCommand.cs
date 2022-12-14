using MediatR;
using Titans.Contract.Models.v1;

namespace Titans.Contract.Command
{
    public class RefreshTokenCommand : IRequest<RefreshToken>
    {
        public string Username { get; set; } = string.Empty;
        public string CurrentToken { get; set; } = string.Empty;
    }
}
