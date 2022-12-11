using Microsoft.AspNetCore.Mvc;
using Titans.Application.Commands;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;

namespace Titans.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContoller : ControllerBase
    {       
        public static User user = new User();
        private readonly IUserRepository _userRepository;

        public AuthContoller(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegistration command)
        {
            var service = new RegisterUserApplicationService(_userRepository);
            user = await service.RunAsync(command);

            return Ok(user);
        }
    }
}
