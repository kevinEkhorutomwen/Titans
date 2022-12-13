using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Titans.Application.Repositories;
using Titans.Domain.User;

namespace Titans.SqlDb
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context,
            IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateAsync(User user)
        {
            var sqlUser = _mapper.Map<Models.User>(user);
            _context.Users.Add(sqlUser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> FindAsync()
        {
            return await _mapper.ProjectTo<User>(_context.Users.Include(x => x.RefreshToken)).AsNoTracking().ToListAsync();
        }

        public async Task<User?> FindAsyncByUsername(string username)
        {
            var sqlUser = await _context.Users.Include(x => x.RefreshToken).AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);
            return _mapper.Map<User>(sqlUser);
        }

        public async Task UpdateAsync(User user)
        {
            var sqlUser = _mapper.Map<Models.User>(user);
            _context.Update(sqlUser);
            await _context.SaveChangesAsync();
        }
    }
}
