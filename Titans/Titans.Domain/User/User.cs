using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Titans.Domain.User
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public byte[] PasswordHash { get; private set; }
        public byte[] PasswordSalt { get; private set; }

        private User(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
        }

        public static User Create(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            return new(username, passwordHash, passwordSalt);
        }
    }
}
