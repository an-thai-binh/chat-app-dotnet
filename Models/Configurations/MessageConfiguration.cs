using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatAppApi.Models.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Content)
                .HasColumnType("TEXT");

            builder.Property(m => m.Type)
                .HasMaxLength(50);

            builder.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey("ConversationId");

            builder.HasOne(m => m.User)
               .WithMany(u => u.Messages)
               .HasForeignKey("UserId");
        }
    }
}
