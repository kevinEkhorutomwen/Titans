using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Titans.Contract.Queries;

namespace Titans.Application.Query
{
    public class GetCurrentUserNameApplicationService : RequestHandler<GetCurrentUserNameQuery, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserNameApplicationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override string Handle(GetCurrentUserNameQuery request)
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            return result!;
        }
    }
}
