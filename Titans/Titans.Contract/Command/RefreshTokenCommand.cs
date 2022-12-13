namespace Titans.Contract.Command
{
    public class RefreshTokenCommand
    {
        public string Username { get; set; } = string.Empty;
        public string CurrentToken { get; set; } = string.Empty;
    }
}
