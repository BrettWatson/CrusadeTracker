using CrusadeTracker.API.Extensions;
using CrusadeTracker.Application.Forces.DTOs;
using CrusadeTracker.Application.Units.DTOs;
using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using CrusadeTracker.Domain.Forces.Repositories;
using CrusadeTracker.Domain.Forces.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrusadeTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ForcesController : ControllerBase
{
    private readonly ICrusadeForceRespository _forceRepository;

    public ForcesController(ICrusadeForceRespository forceRepository)
    {
        _forceRepository = forceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ForceResponse>>> GetMyForces(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var forces = await _forceRepository.GetByOwnerAsync(userId, ct);

        var response = forces.Select(f => new ForceResponse(
            f.Id.Value,
            f.OwnerId.Value,
            f.Name,
            f.Faction,
            f.PointsLimit.Value,
            f.TotalPoints().Value,
            f.Units.Count,
            f.CreatedAt)).ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ForceDetailResponse>> GetForce(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(id), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var response = new ForceDetailResponse(
            force.Id.Value,
            force.OwnerId.Value,
            force.Name,
            force.Faction,
            force.PointsLimit.Value,
            force.TotalPoints().Value,
            force.Units.Select(u => new UnitResponse(
                u.Id.Value,
                u.Name,
                u.DataSheet,
                u.Points.Value,
                u.ExperiencePoints.Value,
                u.BattleHonours.ToList(),
                u.BattleScars.ToList(),
                u.CreatedAt)).ToList(),
            force.CreatedAt);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ForceResponse>> CreateForce(
        CreateForceRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();

        var force = CrusadeForce.Create(
            userId,
            request.Name,
            request.Faction,
            new SupplyLimit(request.PointsLimit));

        await _forceRepository.AddAsync(force, ct);
        await _forceRepository.SaveChangesAsync(ct);

        var response = new ForceResponse(
            force.Id.Value,
            force.OwnerId.Value,
            force.Name,
            force.Faction,
            force.PointsLimit.Value,
            force.TotalPoints().Value,
            force.Units.Count,
            force.CreatedAt);

        return CreatedAtAction(nameof(GetForce), new { id = force.Id.Value }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ForceResponse>> UpdateForce(
        Guid id,
        UpdateForceRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(id), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        force.Rename(request.Name);
        force.ChangeFaction(request.Faction);

        await _forceRepository.SaveChangesAsync(ct);

        var response = new ForceResponse(
            force.Id.Value,
            force.OwnerId.Value,
            force.Name,
            force.Faction,
            force.PointsLimit.Value,
            force.TotalPoints().Value,
            force.Units.Count,
            force.CreatedAt);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteForce(Guid id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(id), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        _forceRepository.Remove(force);
        await _forceRepository.SaveChangesAsync(ct);

        return NoContent();
    }
}
