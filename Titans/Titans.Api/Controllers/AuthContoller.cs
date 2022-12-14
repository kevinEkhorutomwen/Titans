using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using Titans.Contract.Queries;

namespace Titans.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthContoller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserCommand command)
        {
            await _mediator.Publish(command);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserCommand command)
        {
            var jwtToken = await _mediator.Send(command);
            var refreshToken = await _mediator.Send(new RefreshTokenCommand(command.Username, string.Empty));

            SetRefreshToken(refreshToken);

            return Ok(jwtToken);
        }

        [HttpGet("Users")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _mediator.Send(new GetUsersQuery());
            return Ok(users);
        }

        [HttpPost("RefreshToken"), Authorize]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var username = await _mediator.Send(new GetCurrentUserNameQuery());
            var cookieRefreshToken = await _mediator.Send(new GetCurrentRefreshToken());
            var refreshToken = await _mediator.Send(new RefreshTokenCommand(username, cookieRefreshToken));

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
