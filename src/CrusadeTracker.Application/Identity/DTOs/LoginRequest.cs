namespace CrusadeTracker.Application.Identity.DTOs;

public sealed record LoginRequest(
    string Email,
    string Password);
