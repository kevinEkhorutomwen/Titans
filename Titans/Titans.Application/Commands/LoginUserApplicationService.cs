namespace Titans.Application.Commands;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using Titans.Domain;

public class LoginUserApplicationService : IRequestHandler<LoginUserCommand, Result<string>>
{
    readonly IUserRepository _userRepository;
    private readonly IOptions<AppSettingsOptions> _options;

    public LoginUserApplicationService(
        IUserRepository userRepository,
        IOptions<AppSettingsOptions> options)
    {
        _userRepository = userRepository;
        _options = options;
    }
    public async Task<Result<string>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsyncByUsername(command.Username);
        if (user == null)
        {
            return Result<string>.SetError(new Error(ErrorMessages.UserNotFound(command.Username)));
        }

        if (!VerifyPasswordHash(command.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Result<string>.SetError(new Error(ErrorMessages.WrongPassword));
        }

        return Result<string>.SetOk(CreateToken(user));
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

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.Value.Token));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}