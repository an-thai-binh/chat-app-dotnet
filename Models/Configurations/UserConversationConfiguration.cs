using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatAppApi.Models.Configurations
{
    public class UserConversationConfiguration : IEntityTypeConfiguration<UserConversation>
    {
        public void Configure(EntityTypeBuilder<UserConversation> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(uc => uc.User)
                .WithMany(u => u.UserConversations)
                .HasForeignKey("UserId");

            builder.HasOne(uc => uc.Conversation)
                .WithMany(c => c.UserConversations)
                .HasForeignKey("ConversationId");

            builder.HasOne(uc => uc.LatestMessage)
                .WithMany(m => m.UserConversations)
                .HasForeignKey("LatestMessageId");
        }
    }
}
