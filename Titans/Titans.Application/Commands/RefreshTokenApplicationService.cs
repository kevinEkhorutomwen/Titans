namespace Titans.Application.Commands;
using AutoMapper;
using MediatR;
using System.Security.Cryptography;
using Titans.Application.Repositories;
using Titans.Contract.Command;
using Titans.Contract.Models.v1;
using Titans.Domain;

public class RefreshTokenApplicationService : IRequestHandler<RefreshTokenCommand, RefreshToken>
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

    public async Task<RefreshToken> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await GetUser();

        if (command.CurrentToken != string.Empty)
        {
            if (user.RefreshToken?.Token != command.CurrentToken)
            {
                throw new Exception(ErrorMessages.TokenInvalid);
            }

            if (user.RefreshToken?.Expires <= DateTime.UtcNow)
            {
                throw new Exception(ErrorMessages.TokenExpired);
            }
        }

        var refreshToken = CreateToken();
        await UpdateUser();

        return refreshToken;

        RefreshToken CreateToken()
        {
            return new RefreshToken(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        }

        async Task<Domain.User.User> GetUser()
        {
            var user = await _userRepository.FindAsyncByUsername(command.Username);
            if (user == null)
            {
                throw new Exception(ErrorMessages.UserNotFound(command.Username));
            }

            return user;
        }

        async Task UpdateUser()
        {
            user.UpdateRefreshToken(_mapper.Map<Domain.User.RefreshToken>(refreshToken));
            await _userRepository.UpdateAsync(user);
        }
    }
}