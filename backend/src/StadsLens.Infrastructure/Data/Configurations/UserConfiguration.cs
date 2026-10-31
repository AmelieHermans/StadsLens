using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);

        // Elke naam maar één keer, zodat inloggen met naam + wachtwoord eenduidig is.
        builder.HasIndex(u => u.Name).IsUnique();
        builder.Property(u => u.PreferredLanguage).IsRequired().HasMaxLength(5);

        // User 1 -> n Visit
        builder.HasMany(u => u.Visits)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
