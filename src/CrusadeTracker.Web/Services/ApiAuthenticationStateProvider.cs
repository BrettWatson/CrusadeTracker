using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace CrusadeTracker.Web.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public ApiAuthenticationStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(_anonymous);

        var claims = ParseClaimsFromJwt(token);
        if (claims is null || IsTokenExpired(claims))
            return new AuthenticationState(_anonymous);

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private static IEnumerable<Claim>? ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsTokenExpired(IEnumerable<Claim> claims)
    {
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
        if (expClaim is null || !long.TryParse(expClaim.Value, out var exp))
            return true;

        var expDate = DateTimeOffset.FromUnixTimeSeconds(exp);
        return expDate <= DateTimeOffset.UtcNow;
    }
}
