using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces.ValueObjects;

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
    public Points PointsLimit { get; private set; }
    public bool IsFinalized { get; private set; }

    private readonly List<BattleParticipant> _participants = new();
    public IReadOnlyCollection<BattleParticipant> Participants => _participants.AsReadOnly();

    private Battle() { }
    private Battle(BattleId id, DateTimeOffset occuredDate, string mission, Points pointsLimit)
    {
        Id = id;
        Date = occuredDate;
        Mission = string.IsNullOrWhiteSpace(mission) ? throw new ArgumentException("Mission cannot be empty.", nameof(mission)) : mission;
        PointsLimit = pointsLimit;
    }

    public static Battle Record(DateTimeOffset occuredDate, string mission, Points pointsLimit)
        => new(new BattleId(Guid.NewGuid()), occuredDate, mission, pointsLimit);

    public void AddParticipant(UserId playerId, ForceId forceId, IReadOnlyCollection<ParticipantUnit> units, string? forceNameSnapshot = null)
    {
        EnsureNotFinalized();

        if (_participants.Any(p => p.ForceId == forceId))
        {
            throw new InvalidOperationException("Participant with the same force ID already exists in the battle.");
        }

        var participant = new BattleParticipant(playerId, forceId, units, forceNameSnapshot);

        if (participant.TotalPoints().Value > PointsLimit.Value)
        {
            throw new InvalidOperationException(
                $"Selected units total {participant.TotalPoints().Value} points, which exceeds the battle's {PointsLimit.Value} point limit.");
        }

        _participants.Add(participant);
    }

    public void SetResult(ForceId forceId, BattleResult result)
    {
        EnsureNotFinalized();

        var participant = _participants.FirstOrDefault(p => p.ForceId == forceId);

        if (participant == null)
        {
            throw new InvalidOperationException("No participant found with the given force ID in the battle.");
        }

        participant.SetResult(result);
    }

    public void FinalizeBattle()
    {
        IsFinalized = true;
    }

    private void EnsureNotFinalized()
    {
        if (IsFinalized)
            throw new InvalidOperationException("Cannot modify a finalized battle.");
    }
}
