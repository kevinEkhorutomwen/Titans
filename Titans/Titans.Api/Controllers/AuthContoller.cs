using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titans.Application.Commands;
using Titans.Application.Query;
using Titans.Application.Repositories;
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

        public AuthContoller(
            IUserRepository userRepository,
            IRegisterUserApplicationService registerUserApplicationService,
            ILoginUserApplicationService loginUserApplicationService)
        {
            _userRepository = userRepository;
            _registerUserApplicationService = registerUserApplicationService;
            _loginUserApplicationService = loginUserApplicationService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserRegistration command)
        {
            await _registerUserApplicationService.RunAsync(command);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLogin command)
        {
            var token = await _loginUserApplicationService.RunAsync(command);
            return Ok(token);
        }

        [HttpGet("Users"), Authorize]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var service = new GetUsersApplicationService(_userRepository);
            var users = await service.RunAsync();

            return Ok(users);
        }
    }
}
