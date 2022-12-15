namespace Titans.Contract.Models.v1;
using MediatR;

public record RegisterUserCommand(string Username, string Password, string ConfirmPassword) : IRequest<Result>;