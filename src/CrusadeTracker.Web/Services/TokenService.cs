using Blazored.LocalStorage;

namespace CrusadeTracker.Web.Services;

public class TokenService
{
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";

    private readonly ILocalStorageService _localStorage;

    public TokenService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<string?> GetAccessTokenAsync()
        => await _localStorage.GetItemAsync<string>(AccessTokenKey);

    public async Task<string?> GetRefreshTokenAsync()
        => await _localStorage.GetItemAsync<string>(RefreshTokenKey);

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await _localStorage.SetItemAsync(AccessTokenKey, accessToken);
        await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        await _localStorage.RemoveItemAsync(AccessTokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
    }
}
