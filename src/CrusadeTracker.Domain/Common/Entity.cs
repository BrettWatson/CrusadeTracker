
namespace CrusadeTracker.Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; protected init; } = default!;
}
