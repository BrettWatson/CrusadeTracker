namespace CrusadeTracker.Application.Identity.DTOs;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string Username,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt);
