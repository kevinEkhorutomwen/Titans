using Titans.Domain.User;

namespace Titans.Application.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<List<User>> FindAsync();
        Task<User?> FindAsyncByUsername(string username);
    }
}
