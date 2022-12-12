using Titans.Contract.Models.v1;

namespace Titans.Application.Mapping.v1
{
    public static class UserMapping
    {
        public static User ToContract(this Domain.User.User self)
        {
            return new User
            {
                Username = self.Username,
                PasswordHash = self.PasswordHash,
                PasswordSalt = self.PasswordSalt,
            };
        }

        public static List<User> ToContract(this List<Domain.User.User> self)
        {
            return new List<User>(self.Select(x => x.ToContract()).ToList());
        }
    }
}
