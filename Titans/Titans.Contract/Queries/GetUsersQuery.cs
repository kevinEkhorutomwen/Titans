namespace Titans.Contract.Queries;
using MediatR;
using Titans.Contract.Models.v1;

public record GetUsersQuery : IRequest<List<User>> { }
