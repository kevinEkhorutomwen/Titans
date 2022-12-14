using MediatR;
using Titans.Contract.Models.v1;

namespace Titans.Contract.Queries
{
    public record GetUsersQuery : IRequest<List<User>> { }
}
