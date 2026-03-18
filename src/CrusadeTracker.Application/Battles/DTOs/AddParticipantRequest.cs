namespace CrusadeTracker.Application.Battles.DTOs;

public sealed record AddParticipantRequest(
    Guid ForceId,
    List<Guid> UnitIds,
    string? ForceNameSnapshot = null);
