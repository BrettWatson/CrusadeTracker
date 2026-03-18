using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces.ValueObjects;

namespace CrusadeTracker.Domain.Battles;

public sealed class BattleParticipant
{
    public UserId PlayerId { get; private set; }
    public ForceId ForceId { get; private set; }
    public BattleResult Result { get; private set; }
    public string? ForceNameSnapshot { get; private set; }

    private readonly List<ParticipantUnit> _units = new();
    public IReadOnlyCollection<ParticipantUnit> Units => _units.AsReadOnly();

    private BattleParticipant() { }

    public BattleParticipant(
        UserId playerId,
        ForceId forceId,
        IReadOnlyCollection<ParticipantUnit> units,
        string? forceNameSnapshot = null)
    {
        if (units is null || units.Count == 0)
            throw new ArgumentException("At least one unit must participate in the battle.", nameof(units));

        PlayerId = playerId;
        ForceId = forceId;
        ForceNameSnapshot = forceNameSnapshot;
        _units.AddRange(units);
    }

    public Points TotalPoints() => new(_units.Sum(u => u.Points.Value));

    public void SetResult(BattleResult result)
    {
        Result = result;
    }
}
