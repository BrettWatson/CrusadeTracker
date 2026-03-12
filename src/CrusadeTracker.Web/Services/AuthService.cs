using System.Net.Http.Json;
using CrusadeTracker.Application.Identity.DTOs;

namespace CrusadeTracker.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;
    private readonly ApiAuthenticationStateProvider _authStateProvider;

    public AuthService(
        HttpClient httpClient,
        TokenService tokenService,
        ApiAuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var request = new LoginRequest(email, password);
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (authResponse is not null)
        {
            await _tokenService.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
            _authStateProvider.NotifyUserAuthentication(authResponse.AccessToken);
        }

        return authResponse;
    }

    public async Task<AuthResponse?> RegisterAsync(string email, string username, string password)
    {
        var request = new RegisterRequest(email, username, password);
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

        if (!response.IsSuccessStatusCode)
            return null;

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (authResponse is not null)
        {
            await _tokenService.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
            _authStateProvider.NotifyUserAuthentication(authResponse.AccessToken);
        }

        return authResponse;
    }

    public async Task<AuthResponse?> RefreshTokenAsync()
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
            return null;

        var request = new RefreshTokenRequest(refreshToken);
        var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);

        if (!response.IsSuccessStatusCode)
        {
            await LogoutAsync();
            return null;
        }

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (authResponse is not null)
        {
            await _tokenService.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
            _authStateProvider.NotifyUserAuthentication(authResponse.AccessToken);
        }

        return authResponse;
    }

    public async Task LogoutAsync()
    {
        await _tokenService.ClearTokensAsync();
        _authStateProvider.NotifyUserLogout();
    }
}
