using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Repositories
{
    public class FriendshipRepository
    {
        private readonly AppDbContext _context;

        public FriendshipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByUserAndFriendAsync(User sender, User receiver)
        {
            return await _context.Friendship.AnyAsync(f => f.User == sender && f.Friend == receiver);
        }
        public async Task<Friendship?> FindByUserAndFriendAsync(User sender, User receiver)
        {
            return await _context.Friendship.FirstOrDefaultAsync(f => (f.User == sender && f.Friend == receiver) 
                                                                   || (f.User == receiver && f.Friend == sender));
        }

        public async Task<List<Friendship>> FindFriendRequestByUser(User user)
        {
            return await _context.Friendship
                .Include(f => f.User)
                .Include(f => f.Friend)
                .Where(f => f.Friend == user && f.Status == "PENDING")
                .ToListAsync();
        }

        public async Task<Friendship> SaveAsync(Friendship friendship)
        {
            _context.Friendship.Add(friendship);
            await _context.SaveChangesAsync();
            return friendship;
        }

        public async Task<Friendship> UpdateStatus(Friendship friendship, string newStatus)
        {
            friendship.Status = "FRIEND";
            await _context.SaveChangesAsync();
            return friendship;
        }

        public async Task DeleteByUserAndFriendAsync(User user, User friend)
        {
            Friendship? friendship = await _context.Friendship.FirstOrDefaultAsync(f => f.User == user && f.Friend == friend);
            if(friendship != null)
            {
                _context.Friendship.Remove(friendship);
                await _context.SaveChangesAsync();
            }
        }
    }
}
