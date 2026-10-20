using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Description).HasMaxLength(1000);
        builder.Property(r => r.City).IsRequired().HasMaxLength(100);

        // Route 1 -> n Landmark
        builder.HasMany(r => r.Landmarks)
            .WithOne(l => l.Route)
            .HasForeignKey(l => l.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
