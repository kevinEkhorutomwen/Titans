using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract.Models.v1;

namespace Titans.Application.Commands
{
    public class LoginUserApplicationService
    {
        readonly IUserRepository _userRepository;
        readonly string _token;
        public LoginUserApplicationService(IUserRepository userRepository, string token)
        {
            _userRepository = userRepository;
            _token = token;
        }

        public async Task<string> RunAsync(UserLogin command)
        {
            var user = await _userRepository.FindAsyncByUsername(command.Username);
            if(user == null)
            {
                throw new Exception("Kein User gefunden");
            }

            if (!VerifyPasswordHash(command.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Passwort falsch");
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

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_token));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
