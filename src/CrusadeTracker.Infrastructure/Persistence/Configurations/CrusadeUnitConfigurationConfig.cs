using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using CrusadeTracker.Domain.Forces.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CrusadeTracker.Infrastructure.Persistence.Configurations;

public sealed class CrusadeUnitConfigurationConfig : IEntityTypeConfiguration<CrusadeUnit>
{
    public void Configure(EntityTypeBuilder<CrusadeUnit> builder)
    {
        builder.ToTable("CrusadeUnits");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, v => new UnitId(v));

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DataSheet).HasMaxLength(200).IsRequired();
        builder.Property(x => x.BattlefieldRole).HasMaxLength(100).HasDefaultValue("");

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

        // Ignore public read-only collection properties
        builder.Ignore(x => x.Equipment);
        builder.Ignore(x => x.BattleHonours);
        builder.Ignore(x => x.BattleScars);

        // Battle honours - store as JSON array via primitive collection
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => string.Join("|||", v),
            v => string.IsNullOrEmpty(v) ? new List<string>() : v.Split("|||", StringSplitOptions.RemoveEmptyEntries).ToList());

        var stringListComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property<List<string>>("_equipment")
            .HasField("_equipment")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Equipment")
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property<List<string>>("_battleHonours")
            .HasField("_battleHonours")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("BattleHonours")
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property<List<string>>("_battleScars")
            .HasField("_battleScars")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("BattleScars")
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);
    }
}
