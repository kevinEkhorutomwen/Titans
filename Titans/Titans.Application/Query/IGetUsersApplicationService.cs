using Titans.Contract.Models.v1;

namespace Titans.Application.Query
{
    public interface IGetUsersApplicationService
    {
        Task<List<User>> RunAsync();
    }
}
