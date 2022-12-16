namespace Titans.Application.Commands;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using Titans.Domain;

public class LoginUserApplicationService : IRequestHandler<LoginUserCommand, Result>
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
    public async Task<Result> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsyncByUsername(command.Username);
        if (user == null)
        {
            return Result.SetError(new Error(ErrorMessages.UserNotFound(command.Username)));
        }

        if (!VerifyPasswordHash(command.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Result.SetError(new Error(ErrorMessages.WrongPassword));
        }

        return new Result();
    }

    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}