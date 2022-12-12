using Titans.Contract.Command;

namespace Titans.Application.Commands
{
    public interface ILoginUserApplicationService
    {
        Task<string> RunAsync(LoginUserCommand command);
    }
}
