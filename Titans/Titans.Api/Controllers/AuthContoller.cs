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
        private readonly IConfiguration _configuration;

        public AuthContoller(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserRegistration command)
        {
            if (command.Password != command.ConfirmPassword)
            {
                return BadRequest("Die Passwörter müssen identisch sein");
            }
            var service = new RegisterUserApplicationService(_userRepository);
            await service.RunAsync(command);

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLogin command)
        {
            var setting = _configuration.GetSection("AppSettings:Token").Value;
            if (setting == null)
            {
                return BadRequest("Token nicht gefunden");
            }
            var service = new LoginUserApplicationService(_userRepository, setting);
            var token = await service.RunAsync(command);
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
