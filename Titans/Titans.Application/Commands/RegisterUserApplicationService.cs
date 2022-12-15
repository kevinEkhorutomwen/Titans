namespace Titans.Application.Commands;
using MediatR;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Titans.Application.Repositories;
using Titans.Contract;
using Titans.Contract.Models.v1;
using Titans.Domain;

public class RegisterUserApplicationService : IRequestHandler<RegisterUserCommand, Result>
{
    readonly IUserRepository _userRepository;
    public RegisterUserApplicationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (command.Password != command.ConfirmPassword)
        {
            return Result.SetError(new Error(ErrorMessages.PasswordMustBeIdentical));
        }

        var user = await _userRepository.FindAsyncByUsername(command.Username);
        {
            if (user != null)
            {
                return Result.SetError(new Error(ErrorMessages.UserAlreadyExist));
            }
        }

        CreatePasswordHash(command.Password, out byte[] passwordHash, out byte[] passwordSalt);

        user = Domain.User.User.Create(command.Username, passwordHash, passwordSalt);
        await _userRepository.CreateAsync(user);

        return new Result();
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