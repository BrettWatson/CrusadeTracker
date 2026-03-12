namespace CrusadeTracker.Application.Units.DTOs;

public sealed record UnitResponse(
    Guid Id,
    string Name,
    string DataSheet,
    int Points,
    int ExperiencePoints,
    IReadOnlyCollection<string> BattleHonours,
    IReadOnlyCollection<string> BattleScars,
    DateTimeOffset CreatedAt);
