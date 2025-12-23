using CrusadeTracker.Domain.Common;

namespace CrusadeTracker.Domain.Battles;

public enum BattleResult
{
    Victory,
    Defeat,
    Draw
}
public sealed class Battle : Entity<BattleId>, IAggregateRoot
{
    public DateTimeOffset Date { get; private set; }
    public string Mission { get; private set; } = default!;
    public bool IsFinalized { get; private set; }

    public void FinalizeBattle()
    {
        IsFinalized = true;
    }

    private readonly List<BattleParticipant> _particpants = new();
}
