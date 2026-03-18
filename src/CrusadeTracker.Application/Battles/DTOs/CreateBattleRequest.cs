namespace CrusadeTracker.Application.Battles.DTOs;

public sealed record CreateBattleRequest(
    DateTimeOffset Date,
    string Mission,
    int PointsLimit);
