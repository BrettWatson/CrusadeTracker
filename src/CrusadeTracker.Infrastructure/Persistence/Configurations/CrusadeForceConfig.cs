using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using CrusadeTracker.Domain.Forces.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CrusadeTracker.Infrastructure.Persistence.Configurations;

public sealed class CrusadeForceConfig : IEntityTypeConfiguration<CrusadeForce>
{
    public void Configure(EntityTypeBuilder<CrusadeForce> builder)
    {
        builder.ToTable("CrusadeForces");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, v => new ForceId(v));

        builder.Property(x => x.OwnerId)
            .HasConversion(id => id.Value, v => new UserId(v));

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Faction).HasMaxLength(200).IsRequired();

        // Points limit (SupplyLimit VO) - use value conversion for structs
        builder.Property(x => x.PointsLimit)
            .HasConversion(
                sl => sl.Value,
                v => new SupplyLimit(v))
            .HasColumnName("PointsLimit")
            .IsRequired();

        // Units: map backing field "_units"
        builder.HasMany(x => x.Units)
            .WithOne()
            .HasForeignKey("ForceId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(CrusadeForce.Units))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Battles collection - ignore the public property, configure backing field
        builder.Ignore(x => x.Battles);

        var battleIdsConverter = new ValueConverter<HashSet<BattleId>, string>(
            v => string.Join(',', v.Select(b => b.Value)),
            v => string.IsNullOrEmpty(v)
                ? new HashSet<BattleId>()
                : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => new BattleId(Guid.Parse(g)))
                    .ToHashSet());

        var battleIdsComparer = new ValueComparer<HashSet<BattleId>>(
            (c1, c2) => c1!.SetEquals(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToHashSet());

        builder.Property<HashSet<BattleId>>("_battles")
            .HasField("_battles")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("BattleIds")
            .HasConversion(battleIdsConverter)
            .Metadata.SetValueComparer(battleIdsComparer);
    }
}
