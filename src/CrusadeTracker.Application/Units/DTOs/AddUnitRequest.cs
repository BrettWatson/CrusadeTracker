namespace CrusadeTracker.Application.Units.DTOs;

public sealed record AddUnitRequest(
    string Name,
    string DataSheet,
    int Points);
