using CrusadeTracker.Domain.Battles;
using CrusadeTracker.Domain.Battles.Repositories;
using CrusadeTracker.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CrusadeTracker.Infrastructure.Persistence.Repositories;

public sealed class BattleRepository : IBattleRepository
{
    private readonly AppDbContext _context;

    public BattleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Battle?> GetAsync(BattleId id, CancellationToken ct)
    {
        return await _context.Battles
            .Include(b => b.Participants)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<IReadOnlyList<Battle>> GetByParticipantAsync(UserId playerId, CancellationToken ct)
    {
        return await _context.Battles
            .Include(b => b.Participants)
            .Where(b => b.Participants.Any(p => p.PlayerId == playerId))
            .OrderByDescending(b => b.Date)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Battle battle, CancellationToken ct)
    {
        await _context.Battles.AddAsync(battle, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
