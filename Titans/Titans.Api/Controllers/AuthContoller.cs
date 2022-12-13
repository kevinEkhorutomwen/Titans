using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titans.Application.Commands;
using Titans.Application.Query;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Titans.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : ControllerBase
    {
        private readonly IRegisterUserApplicationService _registerUserApplicationService;
        private readonly ILoginUserApplicationService _loginUserApplicationService;
        private readonly IGetUsersApplicationService _getUsersApplicationService;
        private readonly IRefreshTokenApplicationService _refreshTokenApplicationService;
        private readonly IGetUserInformationApplicationService _getUserInformationApplicationService;

        public AuthContoller(
            IRegisterUserApplicationService registerUserApplicationService,
            ILoginUserApplicationService loginUserApplicationService,
            IGetUsersApplicationService getUsersApplicationService,
            IRefreshTokenApplicationService refreshTokenApplicationService,
            IGetUserInformationApplicationService getUserInformationApplicationService)
        {
            _registerUserApplicationService = registerUserApplicationService;
            _loginUserApplicationService = loginUserApplicationService;
            _getUsersApplicationService = getUsersApplicationService;
            _refreshTokenApplicationService = refreshTokenApplicationService;
            _getUserInformationApplicationService = getUserInformationApplicationService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserCommand command)
        {
            await _registerUserApplicationService.RunAsync(command);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserCommand command)
        {
            var jwtToken = await _loginUserApplicationService.RunAsync(command);
            var refreshToken = await _refreshTokenApplicationService.RunAsync(new RefreshTokenCommand
            {
                Username = command.Username
            });

            SetRefreshToken(refreshToken);

            return Ok(jwtToken);
        }

        [HttpGet("Users")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {            
             var users = await _getUsersApplicationService.RunAsync();
            return Ok(users);
        }

        [HttpPost("RefreshToken"), Authorize]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var usernamer = _getUserInformationApplicationService.GetCurrentUserName();
            var cookieRefreshToken = _getUserInformationApplicationService.GetCurrentRefreshToken();
            var refreshToken = await _refreshTokenApplicationService.RunAsync(new RefreshTokenCommand
            {
                Username = usernamer,
                CurrentToken = cookieRefreshToken
            });

            SetRefreshToken(refreshToken);

            return Ok(refreshToken);
        }

        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
