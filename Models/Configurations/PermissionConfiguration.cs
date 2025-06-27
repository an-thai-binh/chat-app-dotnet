using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatAppApi.Models.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<RevokatedToken>
    {
        public void Configure(EntityTypeBuilder<RevokatedToken> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd();

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
