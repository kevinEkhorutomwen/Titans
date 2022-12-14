namespace Titans.Contract.Models.v1
{
    public record RefreshToken
    {
        public string Token { get; init; } = string.Empty;
        public DateTime Created { get; init; } = DateTime.UtcNow;
        public DateTime Expires { get; init; }
    }
}
