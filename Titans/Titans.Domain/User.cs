namespace Titans.Domain
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public byte[] PasswordHash { get; private set; }
        public byte[] PasswordSalt { get; private set; }

        private User(int id, string username, byte[] passwordHash, byte[] passwordSalt)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }

        public static User Create(int id, string username, byte[] passwordHash, byte[] passwordSalt)
        {
            return new(id, username, passwordHash, passwordSalt);
        }
    }
}
