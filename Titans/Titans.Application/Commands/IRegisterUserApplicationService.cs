using Titans.Contract.Models.v1;

namespace Titans.Application.Commands
{
    public interface IRegisterUserApplicationService
    {
        Task RunAsync(RegisterUserCommand command);
    }
}
