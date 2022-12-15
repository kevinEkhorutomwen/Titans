namespace Titans.Application.Repositories;
using Titans.Domain.User;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> FindAsync();
    Task<User?> FindAsyncByUsername(string username);
}