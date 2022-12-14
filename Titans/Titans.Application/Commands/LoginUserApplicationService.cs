using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract.Command;
using Titans.Contract.Interfaces;
using Titans.Domain;

namespace Titans.Application.Commands
{
    public class LoginUserApplicationService : IRequestHandler<LoginUserCommand, string>
    {
        readonly IUserRepository _userRepository;
        readonly ISettings _settings;
        public LoginUserApplicationService(
            IUserRepository userRepository,
            ISettings settings)
        {
            _userRepository = userRepository;
            _settings = settings;
        }
        public async Task<string> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindAsyncByUsername(command.Username);
            if (user == null)
            {
                throw new Exception(ErrorMessages.UserNotFound(command.Username));
            }

            if (!VerifyPasswordHash(command.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception(ErrorMessages.WrongPassword);
            }

            return CreateToken(user);
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(Domain.User.User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_settings.Token));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
