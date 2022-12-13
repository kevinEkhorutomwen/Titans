namespace Titans.Contract.Models.v1
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public RefreshToken? RefreshToken { get; set; }
    }
}
