using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        // Points limit (SupplyLimit VO)
        builder.OwnsOne(x => x.PointsLimit, pl =>
        {
            pl.Property(p => p.Value).HasColumnName("PointsLimit").IsRequired();
        });

        // Units: map backing field "_units"
        builder.Metadata.FindNavigation(nameof(CrusadeForce.Units))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<CrusadeUnit>("_units")
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        // Applied battle ids: backing field "_battles"
        builder.OwnsMany<BattleId>("_battles", b =>
        {
            b.ToTable("CrusadeForceBattles");
            b.WithOwner().HasForeignKey("ForceId");

            b.Property<Guid>("Id");
            b.HasKey("Id");

            b.Property(x => x.Value)
              .HasColumnName("BattleId")
              .IsRequired();
        });
    }
}
