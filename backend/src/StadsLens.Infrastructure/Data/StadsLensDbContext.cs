using Microsoft.EntityFrameworkCore;
using StadsLens.Domain.Entities;

namespace StadsLens.Infrastructure.Data;

public class StadsLensDbContext : DbContext
{
    public StadsLensDbContext(DbContextOptions<StadsLensDbContext> options) : base(options)
    {
    }

    public DbSet<Route> Routes => Set<Route>();
    public DbSet<Landmark> Landmarks => Set<Landmark>();
    public DbSet<RouteLandmark> RouteLandmarks => Set<RouteLandmark>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Visit> Visits => Set<Visit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StadsLensDbContext).Assembly);
    }
}
