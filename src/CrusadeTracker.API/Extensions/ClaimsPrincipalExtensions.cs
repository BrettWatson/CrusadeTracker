using System.Security.Claims;
using CrusadeTracker.Domain.Common;

namespace CrusadeTracker.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserId GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User ID not found in claims.");

        return new UserId(userId);
    }

    public static UserId? TryGetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        return new UserId(userId);
    }
}
