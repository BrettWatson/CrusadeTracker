using CrusadeTracker.API.Extensions;
using CrusadeTracker.Application.Battles.DTOs;
using CrusadeTracker.Domain.Battles;
using CrusadeTracker.Domain.Battles.Repositories;
using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrusadeTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BattlesController : ControllerBase
{
    private readonly IBattleRepository _battleRepository;
    private readonly ICrusadeForceRespository _forceRepository;

    public BattlesController(
        IBattleRepository battleRepository,
        ICrusadeForceRespository forceRepository)
    {
        _battleRepository = battleRepository;
        _forceRepository = forceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BattleResponse>>> GetMyBattles(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var battles = await _battleRepository.GetByParticipantAsync(userId, ct);

        var response = battles.Select(MapToBattleResponse).ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BattleResponse>> GetBattle(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var battle = await _battleRepository.GetAsync(new BattleId(id), ct);

        if (battle is null)
            return NotFound();

        // Only participants can view the battle
        if (!battle.Participants.Any(p => p.PlayerId == userId))
            return Forbid();

        return Ok(MapToBattleResponse(battle));
    }

    [HttpPost]
    public async Task<ActionResult<BattleResponse>> CreateBattle(
        CreateBattleRequest request,
        CancellationToken ct)
    {
        var battle = Battle.Record(request.Date, request.Mission);

        await _battleRepository.AddAsync(battle, ct);
        await _battleRepository.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetBattle), new { id = battle.Id.Value }, MapToBattleResponse(battle));
    }

    [HttpPost("{id:guid}/participants")]
    public async Task<ActionResult<BattleResponse>> AddParticipant(
        Guid id,
        AddParticipantRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var battle = await _battleRepository.GetAsync(new BattleId(id), ct);

        if (battle is null)
            return NotFound();

        // Verify the user owns the force they're adding
        var force = await _forceRepository.GetAsync(new ForceId(request.ForceId), ct);
        if (force is null)
            return BadRequest(new { error = "Force not found." });

        if (force.OwnerId != userId)
            return Forbid();

        try
        {
            var forceNameSnapshot = request.ForceNameSnapshot ?? force.Name;
            battle.AddParticipant(userId, new ForceId(request.ForceId), forceNameSnapshot);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _battleRepository.SaveChangesAsync(ct);

        return Ok(MapToBattleResponse(battle));
    }

    [HttpPost("{id:guid}/results")]
    public async Task<ActionResult<BattleResponse>> SetResult(
        Guid id,
        SetResultRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var battle = await _battleRepository.GetAsync(new BattleId(id), ct);

        if (battle is null)
            return NotFound();

        // Verify the user owns the force they're setting result for
        var force = await _forceRepository.GetAsync(new ForceId(request.ForceId), ct);
        if (force is null)
            return BadRequest(new { error = "Force not found." });

        if (force.OwnerId != userId)
            return Forbid();

        if (!Enum.TryParse<BattleResult>(request.Result, true, out var result))
            return BadRequest(new { error = "Invalid result. Must be 'Victory', 'Defeat', or 'Draw'." });

        try
        {
            battle.SetResult(new ForceId(request.ForceId), result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _battleRepository.SaveChangesAsync(ct);

        return Ok(MapToBattleResponse(battle));
    }

    [HttpPost("{id:guid}/finalize")]
    public async Task<ActionResult<BattleResponse>> FinalizeBattle(
        Guid id,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var battle = await _battleRepository.GetAsync(new BattleId(id), ct);

        if (battle is null)
            return NotFound();

        // Only participants can finalize
        if (!battle.Participants.Any(p => p.PlayerId == userId))
            return Forbid();

        battle.FinalizeBattle();
        await _battleRepository.SaveChangesAsync(ct);

        return Ok(MapToBattleResponse(battle));
    }

    private static BattleResponse MapToBattleResponse(Battle battle)
    {
        return new BattleResponse(
            battle.Id.Value,
            battle.Date,
            battle.Mission,
            battle.IsFinalized,
            battle.Participants.Select(p => new ParticipantResponse(
                p.PlayerId.Value,
                p.ForceId.Value,
                p.ForceNameSnapshot,
                p.Result.ToString())).ToList(),
            battle.CreatedAt);
    }
}
