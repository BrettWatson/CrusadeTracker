using CrusadeTracker.Application.Identity.DTOs;

namespace CrusadeTracker.Web.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(string email, string password);
    Task<AuthResponse?> RegisterAsync(string email, string username, string password);
    Task<AuthResponse?> RefreshTokenAsync();
    Task LogoutAsync();
}
