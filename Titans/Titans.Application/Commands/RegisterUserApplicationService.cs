using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;
using Titans.Domain;

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
            if (command.Password != command.ConfirmPassword)
            {
                throw new Exception(ErrorMessages.PasswordMustBeIdentical);
            }

            var user = await _userRepository.FindAsyncByUsername(command.Username);
            {
                if(user != null)
                {
                    throw new Exception(ErrorMessages.UserAlreadyExist);
                }
            }

            CreatePasswordHash(command.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user = Domain.User.User.Create(command.Username, passwordHash, passwordSalt);
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
