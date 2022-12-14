using MediatR;
using Titans.Contract.Models.v1;

namespace Titans.Contract.Queries
{
    public class GetUsersQuery : IRequest<List<User>> { }
}
