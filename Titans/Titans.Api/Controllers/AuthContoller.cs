namespace Titans.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using Titans.Contract.Queries;

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
        var result = await _mediator.Send(command);
        if (result.Error != null)
        {
            return new BadRequestObjectResult(result.Error.Message);
        }
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(LoginUserCommand command)
    {
        var jwtTokenResponse = await _mediator.Send(command);
        if(jwtTokenResponse.Error != null)
        {
            return new BadRequestObjectResult(jwtTokenResponse.Error.Message);
        }

        var refreshTokenResponse = await _mediator.Send(new RefreshTokenCommand(command.Username, string.Empty));
        if (refreshTokenResponse.Error != null)
        {
            return new BadRequestObjectResult(refreshTokenResponse.Error.Message);
        }

        SetRefreshToken(refreshTokenResponse.Data!);

        return Ok(jwtTokenResponse.Data!);
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
        var refreshTokenResponse = await _mediator.Send(new RefreshTokenCommand(username, cookieRefreshToken));

        if (refreshTokenResponse.Error != null)
        {
            return new BadRequestObjectResult(refreshTokenResponse.Error.Message);
        }

        SetRefreshToken(refreshTokenResponse.Data!);

        return Ok(refreshTokenResponse.Data!);
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
