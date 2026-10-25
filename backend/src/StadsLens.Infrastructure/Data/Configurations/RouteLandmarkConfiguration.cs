using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data.Configurations;

public class RouteLandmarkConfiguration : IEntityTypeConfiguration<RouteLandmark>
{
    public void Configure(EntityTypeBuilder<RouteLandmark> builder)
    {
        // Een landmark zit maximaal één keer in dezelfde route.
        builder.HasKey(rl => new { rl.RouteId, rl.LandmarkId });

        // Twee landmarks in dezelfde route kunnen niet op dezelfde plaats staan.
        builder.HasIndex(rl => new { rl.RouteId, rl.OrderInRoute }).IsUnique();

        // Route n <-> n Landmark (via RouteLandmark)
        builder.HasOne(rl => rl.Route)
            .WithMany(r => r.RouteLandmarks)
            .HasForeignKey(rl => rl.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rl => rl.Landmark)
            .WithMany(l => l.RouteLandmarks)
            .HasForeignKey(rl => rl.LandmarkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
