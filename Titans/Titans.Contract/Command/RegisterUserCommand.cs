namespace Titans.Contract.Models.v1;
using MediatR;
using System.ComponentModel.DataAnnotations;

public record RegisterUserCommand
    (
        [property: Required] string Username,
        [property: Required, MinLength(6)] string Password,
        [property: Required, Compare("Password")] string ConfirmPassword
    ) : IRequest<Result>;