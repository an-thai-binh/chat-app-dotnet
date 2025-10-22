using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatAppApi.Models.Configurations
{
    public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(p => p.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey("ConversationId");

            builder.HasOne(p => p.User)
                .WithMany(u => u.Participations)
                .HasForeignKey("UserId");
        }
    }
}
