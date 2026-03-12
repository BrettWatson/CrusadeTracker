namespace CrusadeTracker.Application.Forces.DTOs;

public sealed record UpdateForceRequest(
    string Name,
    string Faction);
