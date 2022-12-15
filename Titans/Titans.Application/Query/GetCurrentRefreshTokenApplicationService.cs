namespace Titans.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Titans.Contract.Queries;

public class GetCurrentRefreshTokenApplicationService : RequestHandler<GetCurrentRefreshToken, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentRefreshTokenApplicationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override string Handle(GetCurrentRefreshToken request)
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
        }
        return result!;
    }
}