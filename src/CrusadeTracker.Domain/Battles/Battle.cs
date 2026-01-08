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

    private readonly List<BattleParticipant> _participants = new();
    public IReadOnlyCollection<BattleParticipant> Participants => _participants.AsReadOnly();

    private Battle() { }
    private Battle(BattleId id, DateTimeOffset occuredDate, string mission)
    {
        Id = id;
        Date = occuredDate;
        Mission = string.IsNullOrWhiteSpace(mission) ? throw new ArgumentException("Mission cannot be empty.", nameof(mission)) : mission;
    }

    public static Battle Record(DateTimeOffset occuredDate, string mission)
        => new(new BattleId(Guid.NewGuid()), occuredDate, mission);

    public void AddParticipant(UserId playerId, ForceId forceId, string? forceNameSnapshot = null)
    {
        EnsureNotFinalized();

        // Allowing for the same player to have multiple forces in a battle.
        //if (_participants.Any(p => p.PlayerId == playerId))
        //{
        //    throw new InvalidOperationException("Participant with the same player ID already exists in the battle.");
        //}
        if (_participants.Any(p => p.ForceId == forceId))
        {
            throw new InvalidOperationException("Participant with the same force ID already exists in the battle.");
        }

        _participants.Add(new BattleParticipant(playerId, forceId, forceNameSnapshot));
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
