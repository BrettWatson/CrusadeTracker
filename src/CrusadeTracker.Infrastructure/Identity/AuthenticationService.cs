using CrusadeTracker.Application.Identity;
using CrusadeTracker.Application.Identity.DTOs;
using CrusadeTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CrusadeTracker.Infrastructure.Identity;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _dbContext;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        IJwtTokenGenerator tokenGenerator,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail is not null)
            throw new InvalidOperationException("Email already registered.");

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername is not null)
            throw new InvalidOperationException("Username already taken.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Username
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        return await GenerateAuthResponseAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await GenerateAuthResponseAsync(user, ct);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var storedToken = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked, ct);

        if (storedToken is null || storedToken.ExpiresAt < DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        storedToken.IsRevoked = true;

        return await GenerateAuthResponseAsync(storedToken.User, ct);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, ct);

        if (storedToken is not null)
        {
            storedToken.IsRevoked = true;
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user, CancellationToken ct)
    {
        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync(ct);

        return new AuthResponse(
            user.Id,
            user.Email!,
            user.UserName!,
            accessToken,
            refreshToken,
            expiresAt);
    }
}
