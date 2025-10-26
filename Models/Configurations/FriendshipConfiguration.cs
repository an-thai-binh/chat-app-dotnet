using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatAppApi.Models.Configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(o => o.User)
                .WithMany(u => u.FriendSent)
                .HasForeignKey(o => o.UserId);

            builder.HasOne(o => o.Friend)
                .WithMany(u => u.FriendReceived)
                .HasForeignKey(o => o.FriendId);

            builder.Property(o => o.Status)
                .HasMaxLength(20);

            builder.HasOne(o => o.PrivateConversation)
                .WithOne(c => c.Friendship)
                .HasForeignKey<Friendship>(o => o.PrivateConversationId);
        }
    }
}
