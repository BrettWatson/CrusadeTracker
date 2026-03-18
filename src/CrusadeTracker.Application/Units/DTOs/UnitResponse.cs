namespace CrusadeTracker.Application.Units.DTOs;

public sealed record UnitResponse(
    Guid Id,
    string Name,
    string DataSheet,
    string BattlefieldRole,
    int Points,
    int ExperiencePoints,
    IReadOnlyCollection<string> Equipment,
    IReadOnlyCollection<string> BattleHonours,
    IReadOnlyCollection<string> BattleScars,
    DateTimeOffset CreatedAt);
