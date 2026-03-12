using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using CrusadeTracker.Domain.Forces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrusadeTracker.Infrastructure.Persistence.Repositories;

public sealed class CrusadeForceRepository : ICrusadeForceRespository
{
    private readonly AppDbContext _context;

    public CrusadeForceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CrusadeForce?> GetAsync(ForceId id, CancellationToken ct)
    {
        return await _context.Forces
            .Include(f => f.Units)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IReadOnlyList<CrusadeForce>> GetByOwnerAsync(UserId ownerId, CancellationToken ct)
    {
        return await _context.Forces
            .Include(f => f.Units)
            .Where(f => f.OwnerId == ownerId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(CrusadeForce force, CancellationToken ct)
    {
        await _context.Forces.AddAsync(force, ct);
    }

    public void Remove(CrusadeForce force)
    {
        _context.Forces.Remove(force);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
