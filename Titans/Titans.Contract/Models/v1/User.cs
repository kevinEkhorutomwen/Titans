namespace Titans.Contract.Models.v1
{
    public record User
    {
        public string Username { get; init; } = string.Empty;
        public byte[] PasswordHash { get; init; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; init; } = Array.Empty<byte>();
        public RefreshToken? RefreshToken { get; init; }
    }
}
