using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Models.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Password)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(o => o.Email)
                .IsRequired();
            builder.HasIndex(o => o.Email)
                .IsUnique();

            builder.Property(o => o.DisplayName)
                .HasMaxLength(100);

            builder.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRole"));
        }
    }
}