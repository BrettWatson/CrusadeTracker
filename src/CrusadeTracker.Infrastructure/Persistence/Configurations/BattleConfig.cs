using CrusadeTracker.Domain.Battles;
using CrusadeTracker.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrusadeTracker.Infrastructure.Persistence.Configurations;

public sealed class BattleConfig : IEntityTypeConfiguration<Battle>
{
    public void Configure(EntityTypeBuilder<Battle> builder)
    {
        builder.ToTable("Battles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, v => new BattleId(v));

        builder.Property(x => x.Mission).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.IsFinalized).IsRequired();

        builder.OwnsMany(x => x.Participants, pb =>
        {
            pb.ToTable("BattleParticipants");
            pb.WithOwner().HasForeignKey("BattleId");

            pb.Property<Guid>("Id");
            pb.HasKey("Id");

            pb.Property(x => x.PlayerId)
                .HasConversion(id => id.Value, v => new UserId(v))
                .IsRequired();

            pb.Property(x => x.ForceId)
                .HasConversion(id => id.Value, v => new ForceId(v))
                .IsRequired();

            pb.Property(x => x.ForceNameSnapshot).HasMaxLength(200);
            pb.Property(x => x.Result).IsRequired();
        });
    }
}
