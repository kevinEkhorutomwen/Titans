using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titans.Application.Commands;
using Titans.Application.Query;
using Titans.Application.Repositories;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;

namespace Titans.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRegisterUserApplicationService _registerUserApplicationService;
        private readonly ILoginUserApplicationService _loginUserApplicationService;
        private readonly IGetUsersApplicationService _getUsersApplicationService;

        public AuthContoller(
            IUserRepository userRepository,
            IRegisterUserApplicationService registerUserApplicationService,
            ILoginUserApplicationService loginUserApplicationService,
            IGetUsersApplicationService getUsersApplicationService)
        {
            _userRepository = userRepository;
            _registerUserApplicationService = registerUserApplicationService;
            _loginUserApplicationService = loginUserApplicationService;
            _getUsersApplicationService = getUsersApplicationService;
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
            var token = await _loginUserApplicationService.RunAsync(command);
            return Ok(token);
        }

        [HttpGet("Users"), Authorize]
        public async Task<ActionResult<List<User>>> GetUsers()
        {            
             var users = await _getUsersApplicationService.RunAsync();
            return Ok(users);
        }
    }
}
