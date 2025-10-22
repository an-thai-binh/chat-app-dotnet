using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatAppApi.Models.Configurations
{
    public class MessageSeenConfiguration : IEntityTypeConfiguration<MessageSeen>
    {
        public void Configure(EntityTypeBuilder<MessageSeen> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(s => s.Message)
                .WithMany(m => m.MessageSeens)
                .HasForeignKey("MessageId");

            builder.HasOne(s => s.User)
               .WithMany(u => u.MessageSeens)
               .HasForeignKey("UserId");
        }
    }
}
