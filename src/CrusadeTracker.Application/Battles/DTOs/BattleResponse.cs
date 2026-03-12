namespace CrusadeTracker.Application.Battles.DTOs;

public sealed record BattleResponse(
    Guid Id,
    DateTimeOffset Date,
    string Mission,
    bool IsFinalized,
    IReadOnlyCollection<ParticipantResponse> Participants,
    DateTimeOffset CreatedAt);

public sealed record ParticipantResponse(
    Guid PlayerId,
    Guid ForceId,
    string? ForceNameSnapshot,
    string Result);
