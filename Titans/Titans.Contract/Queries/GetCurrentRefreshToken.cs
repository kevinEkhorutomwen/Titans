using MediatR;

namespace Titans.Contract.Queries
{
    public class GetCurrentRefreshToken : IRequest<string> { }
}
