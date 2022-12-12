using Microsoft.EntityFrameworkCore;
using Titans.Application.Repositories;
using Titans.Domain.User;

namespace Titans.SqlDb
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context) 
        {
            _context = context;
        }
        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> FindAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> FindAsyncByUsername(string username)
        {
            return await _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
        }
    }
}
