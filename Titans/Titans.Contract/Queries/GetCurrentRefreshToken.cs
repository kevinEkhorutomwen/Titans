using MediatR;

namespace Titans.Contract.Queries
{
    public record GetCurrentRefreshToken : IRequest<string> { }
}
