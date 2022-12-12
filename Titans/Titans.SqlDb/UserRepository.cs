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
            var sqlUsers = await _context.Users.ToListAsync();
            return sqlUsers.Select(x => _mapper.Map<User>(x)).ToList();
        }

        public async Task<User?> FindAsyncByUsername(string username)
        {
            var sqlUser = await _context.Users.Where(x => x.Username == username).FirstOrDefaultAsync();
            return _mapper.Map<User>(sqlUser);
        }
    }
}
