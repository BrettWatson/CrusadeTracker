using CrusadeTracker.Application.Units.DTOs;

namespace CrusadeTracker.Application.Forces.DTOs;

public sealed record ForceDetailResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Faction,
    int PointsLimit,
    int TotalPoints,
    IReadOnlyCollection<UnitResponse> Units,
    DateTimeOffset CreatedAt);
