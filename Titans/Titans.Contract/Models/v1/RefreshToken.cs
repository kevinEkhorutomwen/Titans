namespace Titans.Contract.Models.v1;

public record RefreshToken(string Token, DateTime Created, DateTime Expires);