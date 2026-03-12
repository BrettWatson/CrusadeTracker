namespace CrusadeTracker.Application.Battles.DTOs;

public sealed record AddParticipantRequest(
    Guid ForceId,
    string? ForceNameSnapshot = null);
