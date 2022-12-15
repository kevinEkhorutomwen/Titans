namespace Titans.Contract.Models.v1;

public record User(string Username, byte[] PasswordHash, byte[] PasswordSalt, RefreshToken? RefreshToken);