using CrusadeTracker.Domain.Common;

namespace CrusadeTracker.Domain.Forces.Repositories;

public interface ICrusadeForceRespository
{
    Task<CrusadeForce?> GetAsync(ForceId id, CancellationToken ct);
    Task<IReadOnlyList<CrusadeForce>> GetByOwnerAsync(UserId ownerId, CancellationToken ct);
    Task AddAsync(CrusadeForce force, CancellationToken ct);
    void Remove(CrusadeForce force);
    Task SaveChangesAsync(CancellationToken ct);
}
