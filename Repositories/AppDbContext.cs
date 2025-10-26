using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Friendship> Friendship { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Conversation> Conversation { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipant { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MessageSeen> MessageSeen { get; set; }
        public DbSet<UserConversation> UserConversation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
