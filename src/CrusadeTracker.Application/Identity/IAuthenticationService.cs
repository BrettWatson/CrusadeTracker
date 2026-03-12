using CrusadeTracker.Application.Identity.DTOs;

namespace CrusadeTracker.Application.Identity;

public interface IAuthenticationService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct);
    Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct);
}
