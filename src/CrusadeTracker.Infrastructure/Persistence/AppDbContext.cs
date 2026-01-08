using CrusadeTracker.Domain.Battles;
using CrusadeTracker.Domain.Forces;
using Microsoft.EntityFrameworkCore;

namespace CrusadeTracker.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<CrusadeForce> Forces => Set<CrusadeForce>();
    public DbSet<Battle> Battles => Set<Battle>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
