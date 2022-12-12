using Titans.Application.Mapping.v1;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;

namespace Titans.Application.Query
{
    public class GetUsersApplicationService
    {
        readonly IUserRepository _userRepository;
        public GetUsersApplicationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> RunAsync()
        {
            var users = await _userRepository.FindAsync();
            return users.ToContract();
        }
    }
}
