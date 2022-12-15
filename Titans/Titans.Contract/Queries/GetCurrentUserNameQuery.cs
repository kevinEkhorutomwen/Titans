namespace Titans.Contract.Queries;
using MediatR;

public record GetCurrentUserNameQuery : IRequest<string> { }