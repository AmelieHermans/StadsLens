using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data.Configurations;

public class LandmarkConfiguration : IEntityTypeConfiguration<Landmark>
{
    public void Configure(EntityTypeBuilder<Landmark> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(150);
        builder.Property(l => l.CustomVisionTag).IsRequired().HasMaxLength(100);
        builder.HasIndex(l => l.CustomVisionTag).IsUnique();

        // Landmark 1 -> n HistoricalImage
        builder.HasMany(l => l.HistoricalImages)
            .WithOne(i => i.Landmark)
            .HasForeignKey(i => i.LandmarkId)
            .OnDelete(DeleteBehavior.Cascade);

        // Landmark 1 -> n Visit
        builder.HasMany(l => l.Visits)
            .WithOne(v => v.Landmark)
            .HasForeignKey(v => v.LandmarkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
