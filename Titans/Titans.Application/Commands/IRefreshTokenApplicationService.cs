using Titans.Contract.Command;
using Titans.Contract.Models.v1;

namespace Titans.Application.Commands
{
    public interface IRefreshTokenApplicationService
    {
        Task<RefreshToken> RunAsync(RefreshTokenCommand command);
    }
}
