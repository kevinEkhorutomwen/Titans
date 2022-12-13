using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Titans.Application.Query
{
    public class GetUserInformationApplicationService : IGetUserInformationApplicationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserInformationApplicationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserName()
        {
            var result = string.Empty;
            if(_httpContextAccessor.HttpContext != null)
            {
               result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result!;
        }

        public string GetCurrentRefreshToken()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            }
            return result!;
        }
    }
}
