using Titans.Domain;

namespace Titans.Application.Repositories
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
    }
}
