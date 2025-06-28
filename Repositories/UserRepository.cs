using ChatAppApi.Dtos;
using ChatAppApi.Models;
using ChatAppApi.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ChatAppApi.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Boolean> ExistsByUsernameAsync(string username)
        {
            return await _context.User.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.User.AnyAsync(u => u.Email == email);
        }

        public async Task<Page<User>> FindAllAsync(Pageable pageable)
        {
            IQueryable<User> query = _context.User.AsQueryable();
            Page<User> result = await Pagination<User>.Execute(query, pageable);
            return result;
        }


        public async Task<User?> FindByIdAsync(string id)
        {
            User? user = await _context.User.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            return user;
        }

        public async Task<User?> FindByIdentifierAsync(string identifer)
        {
            return await _context.User
                .Include(u => u.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Username == identifer || u.Email == identifer);
        }

        public async Task<User> SaveAsync(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
