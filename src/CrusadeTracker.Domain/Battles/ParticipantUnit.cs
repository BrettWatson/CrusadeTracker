using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces.ValueObjects;

namespace CrusadeTracker.Domain.Battles;

public sealed class ParticipantUnit
{
    public UnitId UnitId { get; private set; }
    public string UnitNameSnapshot { get; private set; } = default!;
    public Points Points { get; private set; }

    private ParticipantUnit() { }

    public ParticipantUnit(UnitId unitId, string unitNameSnapshot, Points points)
    {
        UnitId = unitId;
        UnitNameSnapshot = string.IsNullOrWhiteSpace(unitNameSnapshot)
            ? throw new ArgumentException("Unit name snapshot cannot be empty.", nameof(unitNameSnapshot))
            : unitNameSnapshot;
        Points = points;
    }
}
