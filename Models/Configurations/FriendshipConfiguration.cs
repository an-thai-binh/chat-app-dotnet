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
                .HasForeignKey("UserId");

            builder.HasOne(o => o.Friend)
                .WithMany(u => u.FriendReceived)
                .HasForeignKey("FriendId");

            builder.Property(o => o.Status)
                .HasMaxLength(20);
        }
    }
}
