namespace CrusadeTracker.Application.Battles.DTOs;

public sealed record BattleResponse(
    Guid Id,
    DateTimeOffset Date,
    string Mission,
    int PointsLimit,
    bool IsFinalized,
    IReadOnlyCollection<ParticipantResponse> Participants,
    DateTimeOffset CreatedAt);

public sealed record ParticipantResponse(
    Guid PlayerId,
    Guid ForceId,
    string? ForceNameSnapshot,
    string Result,
    IReadOnlyCollection<ParticipantUnitResponse> Units);

public sealed record ParticipantUnitResponse(
    Guid UnitId,
    string UnitNameSnapshot,
    int Points);
