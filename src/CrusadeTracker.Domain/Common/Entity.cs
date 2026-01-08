
namespace CrusadeTracker.Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; protected init; } = default!;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
}
