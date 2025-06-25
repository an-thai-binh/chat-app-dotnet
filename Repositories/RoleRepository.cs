using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Repositories
{
    public class RoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> FindByNameAsync(string roleName)
        {
            return await _context.Role.FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
