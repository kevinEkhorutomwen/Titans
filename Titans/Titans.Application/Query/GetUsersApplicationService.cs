using AutoMapper;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;

namespace Titans.Application.Query
{
    public class GetUsersApplicationService : IGetUsersApplicationService
    {
        readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersApplicationService(
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<User>> RunAsync()
        {
            var users = await _userRepository.FindAsync();
            return users.Select(_mapper.Map<User>).ToList();
        }
    }
}
