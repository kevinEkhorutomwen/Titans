using AutoMapper;
using MediatR;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;
using Titans.Contract.Queries;

namespace Titans.Application.Query
{
    public class GetUsersApplicationService : IRequestHandler<GetUsersQuery, List<User>>
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

        public async Task<List<User>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var users = await _userRepository.FindAsync();
            return users.Select(_mapper.Map<User>).ToList();
        }
    }
}
