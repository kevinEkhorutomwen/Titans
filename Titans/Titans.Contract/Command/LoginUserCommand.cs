namespace Titans.Contract.Command;
using MediatR;

public record LoginUserCommand(string Username, string Password) : IRequest<Result>;