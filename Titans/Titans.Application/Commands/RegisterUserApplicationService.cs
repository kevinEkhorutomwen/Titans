using System.Security.Cryptography;
using Titans.Application.Mapping.v1;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;
using Titans.Domain.User;

namespace Titans.Application.Commands
{
    public class RegisterUserApplicationService
    {
        readonly IUserRepository _userRepository;
        public RegisterUserApplicationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RunAsync(UserRegistration command)
        {
            CreatePasswordHash(command.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = Domain.User.User.Create(command.Username, passwordHash, passwordSalt);
            await _userRepository.CreateAsync(user);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
