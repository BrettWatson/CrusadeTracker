using CrusadeTracker.Domain.Common;

namespace CrusadeTracker.Domain.Battles.Repositories;

public interface IBattleRepository
{
    Task<Battle?> GetAsync(BattleId id, CancellationToken ct);
    Task AddAsync(Battle battle, CancellationToken ct);
}
