namespace CrusadeTracker.Application.Forces.DTOs;

public sealed record CreateForceRequest(
    string Name,
    string Faction,
    int PointsLimit);
