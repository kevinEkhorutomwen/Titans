namespace Titans.Application.Commands;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Titans.Contract;
using Titans.Contract.Command;

public class CreateJwtTokenApplicationService : IRequestHandler<CreateJwtTokenCommand, Result<string>>
{
    private readonly IOptions<AppSettingsOptions> _options;

    public CreateJwtTokenApplicationService(IOptions<AppSettingsOptions> options)
    {
        _options = options;
    }

    public Task<Result<string>> Handle(CreateJwtTokenCommand command, CancellationToken cancellationToken)
    {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, command.claims.Username)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_options.Value.Token));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(Result<string>.SetOk(jwt));
    }
}