namespace Titans.Contract.Queries;
using MediatR;

public record GetCurrentRefreshToken : IRequest<string> { }