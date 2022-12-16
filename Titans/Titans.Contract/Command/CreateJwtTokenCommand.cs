namespace Titans.Contract.Command;
using MediatR;
using Titans.Contract.Models.v1;

public record CreateJwtTokenCommand(Claims claims) : IRequest<Result<string>>;