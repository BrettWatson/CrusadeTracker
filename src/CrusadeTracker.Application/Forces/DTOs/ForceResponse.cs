namespace CrusadeTracker.Application.Forces.DTOs;

public sealed record ForceResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Faction,
    int PointsLimit,
    int TotalPoints,
    int UnitCount,
    DateTimeOffset CreatedAt);
