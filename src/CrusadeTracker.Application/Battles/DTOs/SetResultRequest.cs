namespace CrusadeTracker.Application.Battles.DTOs;

public sealed record SetResultRequest(
    Guid ForceId,
    string Result); // "Victory", "Defeat", "Draw"
