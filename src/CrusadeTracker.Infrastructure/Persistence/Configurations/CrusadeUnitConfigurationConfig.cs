using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using CrusadeTracker.Domain.Forces.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrusadeTracker.Infrastructure.Persistence.Configurations;

public sealed class CrusadeUnitConfigurationConfig: IEntityTypeConfiguration<CrusadeUnit>
{
    public void Configure(EntityTypeBuilder<CrusadeUnit> builder)
    {
        builder.ToTable("CrusadeUnits");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, v => new UnitId(v));

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DataSheet).HasMaxLength(200).IsRequired();

        builder.Property(x => x.Points)
            .HasConversion(
                p => p.Value,
                v => new Points(v))
            .HasColumnName("Points")
            .IsRequired();
        builder.Property(x => x.ExperiencePoints)
            .HasConversion(
                xp => xp.Value,
                v => new ExperiencePoints(v))
            .HasColumnName("ExperiencePoints")
            .IsRequired();

        // Honours/scars string lists -> separate tables
        builder.OwnsMany<string>("_battleHonours", bh =>
        {
            bh.ToTable("CrusadeUnitBattleHonours");
            bh.WithOwner().HasForeignKey("UnitId");
            bh.Property<Guid>("Id");
            bh.HasKey("Id");
            bh.Property(b => b)
              .HasColumnName("BattleHonour")
              .HasMaxLength(200)
              .IsRequired();
        });
        

        builder.OwnsMany<string>("_scars", s =>
        {
            s.ToTable("CrusadeUnitScars");
            s.WithOwner().HasForeignKey("UnitId");
            s.Property<Guid>("Id");
            s.HasKey("Id");
            s.Property(sc => sc)
              .HasColumnName("Scar")
              .HasMaxLength(200)
              .IsRequired();
        });

    }
}
