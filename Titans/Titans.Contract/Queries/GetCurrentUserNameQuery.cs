using MediatR;

namespace Titans.Contract.Queries
{
    public class GetCurrentUserNameQuery : IRequest<string> { }
}
