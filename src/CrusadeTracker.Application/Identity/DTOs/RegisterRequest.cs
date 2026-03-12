namespace CrusadeTracker.Application.Identity.DTOs;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password);
