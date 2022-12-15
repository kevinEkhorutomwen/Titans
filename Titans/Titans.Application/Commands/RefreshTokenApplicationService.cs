namespace Titans.Application.Commands;
using AutoMapper;
using MediatR;
using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using Titans.Domain;

public class RefreshTokenApplicationService : IRequestHandler<RefreshTokenCommand, Result<RefreshToken>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RefreshTokenApplicationService(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<RefreshToken>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsyncByUsername(command.Username);
        if (user == null)
        {
            return Result<RefreshToken>.SetError(new Error(ErrorMessages.UserNotFound(command.Username)));
        }

        if (command.CurrentToken != string.Empty)
        {
            if (user.RefreshToken?.Token != command.CurrentToken)
            {
                return Result<RefreshToken>.SetError(new Error(ErrorMessages.TokenInvalid));
            }

            if (user.RefreshToken?.Expires <= DateTime.UtcNow)
            {
                return Result<RefreshToken>.SetError(new Error(ErrorMessages.TokenExpired));
            }
        }

        var refreshToken = CreateToken();
        await UpdateUser();

        return Result<RefreshToken>.SetOk(refreshToken);

        RefreshToken CreateToken()
        {
            return new RefreshToken(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        }

        async Task UpdateUser()
        {
            user.UpdateRefreshToken(_mapper.Map<Domain.User.RefreshToken>(refreshToken));
            await _userRepository.UpdateAsync(user);
        }
    }
}