namespace Titans.Contract.Command;
using MediatR;
using Titans.Contract.Models.v1;

public record RefreshTokenCommand(string Username, string CurrentToken) : IRequest<Result<RefreshToken>>;