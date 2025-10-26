using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Repositories
{
    public class ConversationRepository
    {
        private readonly AppDbContext _dbContext;

        public ConversationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conversation?> FindByUserAndFriendAsync(User user, User friend)
        {
            var conversationIds = await _dbContext.ConversationParticipant
                .Join(_dbContext.ConversationParticipant,
                p1 => p1.Conversation, p2 => p2.Conversation,
                (p1, p2) => new { p1, p2 }
                )
                .Where(x => (x.p1.User == user && x.p2.User == friend) || (x.p1.User == friend && x.p2.User == user))
                .Select(x => x.p1.Conversation.Id)
                .ToListAsync();
            return await _dbContext.Conversation.FirstOrDefaultAsync(c => conversationIds.Contains(c.Id) && c.IsGroup == false);
        }

        public async Task<Conversation> SaveAsync(Conversation conversation)
        {
            _dbContext.Conversation.Add(conversation);
            await _dbContext.SaveChangesAsync();
            return conversation;
        }
    }
}
