using Microsoft.AspNetCore.Identity;

namespace CrusadeTracker.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
