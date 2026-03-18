using CrusadeTracker.API.Extensions;
using CrusadeTracker.Application.Forces;
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
                u.BattlefieldRole,
                u.Points.Value,
                u.ExperiencePoints.Value,
                u.Equipment.ToList(),
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

    [HttpPost("import")]
    public async Task<ActionResult<ForceDetailResponse>> ImportForce(
        ImportForceRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();

        BattleForgeForce parsed;
        try
        {
            parsed = BattleForgeParser.Parse(request.BattleForgeExport);
        }
        catch (FormatException ex)
        {
            return BadRequest(ex.Message);
        }

        var force = CrusadeForce.Create(
            userId,
            parsed.Name,
            parsed.Faction,
            new SupplyLimit(parsed.PointsLimit));

        // Track name occurrences to disambiguate duplicates
        var nameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var unitNames = new List<(BattleForgeUnit Unit, string ResolvedName)>();

        // First pass: count occurrences
        foreach (var u in parsed.Units)
        {
            nameCounts.TryGetValue(u.Name, out int count);
            nameCounts[u.Name] = count + 1;
        }

        // Second pass: assign numbered names for duplicates
        var nameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var u in parsed.Units)
        {
            string resolvedName;
            if (nameCounts[u.Name] > 1)
            {
                nameIndex.TryGetValue(u.Name, out int idx);
                idx++;
                nameIndex[u.Name] = idx;
                resolvedName = $"{u.Name} #{idx}";
            }
            else
            {
                resolvedName = u.Name;
            }

            var unit = new CrusadeUnit(
                new UnitId(Guid.NewGuid()),
                resolvedName,
                u.Name,
                new Points(u.Points),
                u.BattlefieldRole,
                u.Equipment);

            force.AddUnit(unit);
        }

        await _forceRepository.AddAsync(force, ct);
        await _forceRepository.SaveChangesAsync(ct);

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
                u.BattlefieldRole,
                u.Points.Value,
                u.ExperiencePoints.Value,
                u.Equipment.ToList(),
                u.BattleHonours.ToList(),
                u.BattleScars.ToList(),
                u.CreatedAt)).ToList(),
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
