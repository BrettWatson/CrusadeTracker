using CrusadeTracker.API.Extensions;
using CrusadeTracker.Application.Units.DTOs;
using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces;
using CrusadeTracker.Domain.Forces.Repositories;
using CrusadeTracker.Domain.Forces.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrusadeTracker.API.Controllers;

[ApiController]
[Route("api/forces/{forceId:guid}/units")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly ICrusadeForceRespository _forceRepository;

    public UnitsController(ICrusadeForceRespository forceRepository)
    {
        _forceRepository = forceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UnitResponse>>> GetUnits(
        Guid forceId,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var response = force.Units.Select(u => new UnitResponse(
            u.Id.Value,
            u.Name,
            u.DataSheet,
            u.Points.Value,
            u.ExperiencePoints.Value,
            u.BattleHonours.ToList(),
            u.BattleScars.ToList(),
            u.CreatedAt)).ToList();

        return Ok(response);
    }

    [HttpGet("{unitId:guid}")]
    public async Task<ActionResult<UnitResponse>> GetUnit(
        Guid forceId,
        Guid unitId,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var unit = force.Units.FirstOrDefault(u => u.Id == new UnitId(unitId));
        if (unit is null)
            return NotFound();

        var response = new UnitResponse(
            unit.Id.Value,
            unit.Name,
            unit.DataSheet,
            unit.Points.Value,
            unit.ExperiencePoints.Value,
            unit.BattleHonours.ToList(),
            unit.BattleScars.ToList(),
            unit.CreatedAt);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UnitResponse>> AddUnit(
        Guid forceId,
        AddUnitRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var unit = new CrusadeUnit(
            new UnitId(Guid.NewGuid()),
            request.Name,
            request.DataSheet,
            new Points(request.Points));

        try
        {
            force.AddUnit(unit);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _forceRepository.SaveChangesAsync(ct);

        var response = new UnitResponse(
            unit.Id.Value,
            unit.Name,
            unit.DataSheet,
            unit.Points.Value,
            unit.ExperiencePoints.Value,
            unit.BattleHonours.ToList(),
            unit.BattleScars.ToList(),
            unit.CreatedAt);

        return CreatedAtAction(nameof(GetUnit), new { forceId, unitId = unit.Id.Value }, response);
    }

    [HttpPut("{unitId:guid}")]
    public async Task<ActionResult<UnitResponse>> RenameUnit(
        Guid forceId,
        Guid unitId,
        UpdateUnitRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        try
        {
            force.RemaneUnit(new UnitId(unitId), request.Name);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _forceRepository.SaveChangesAsync(ct);

        var unit = force.Units.First(u => u.Id == new UnitId(unitId));
        var response = new UnitResponse(
            unit.Id.Value,
            unit.Name,
            unit.DataSheet,
            unit.Points.Value,
            unit.ExperiencePoints.Value,
            unit.BattleHonours.ToList(),
            unit.BattleScars.ToList(),
            unit.CreatedAt);

        return Ok(response);
    }

    [HttpDelete("{unitId:guid}")]
    public async Task<IActionResult> RemoveUnit(
        Guid forceId,
        Guid unitId,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        try
        {
            force.RemoveUnit(new UnitId(unitId));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _forceRepository.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpPost("{unitId:guid}/honours")]
    public async Task<ActionResult<UnitResponse>> AddBattleHonour(
        Guid forceId,
        Guid unitId,
        AddBattleHonourRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var unit = force.Units.FirstOrDefault(u => u.Id == new UnitId(unitId));
        if (unit is null)
            return NotFound();

        try
        {
            unit.AddBattleHonour(request.Honour);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _forceRepository.SaveChangesAsync(ct);

        var response = new UnitResponse(
            unit.Id.Value,
            unit.Name,
            unit.DataSheet,
            unit.Points.Value,
            unit.ExperiencePoints.Value,
            unit.BattleHonours.ToList(),
            unit.BattleScars.ToList(),
            unit.CreatedAt);

        return Ok(response);
    }

    [HttpDelete("{unitId:guid}/honours/{honour}")]
    public async Task<IActionResult> RemoveBattleHonour(
        Guid forceId,
        Guid unitId,
        string honour,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var unit = force.Units.FirstOrDefault(u => u.Id == new UnitId(unitId));
        if (unit is null)
            return NotFound();

        try
        {
            unit.RemoveBattleHonour(honour);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _forceRepository.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpPost("{unitId:guid}/scars")]
    public async Task<ActionResult<UnitResponse>> AddBattleScar(
        Guid forceId,
        Guid unitId,
        AddBattleScarRequest request,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var unit = force.Units.FirstOrDefault(u => u.Id == new UnitId(unitId));
        if (unit is null)
            return NotFound();

        unit.AddBattleScar(request.Scar);
        await _forceRepository.SaveChangesAsync(ct);

        var response = new UnitResponse(
            unit.Id.Value,
            unit.Name,
            unit.DataSheet,
            unit.Points.Value,
            unit.ExperiencePoints.Value,
            unit.BattleHonours.ToList(),
            unit.BattleScars.ToList(),
            unit.CreatedAt);

        return Ok(response);
    }

    [HttpDelete("{unitId:guid}/scars/{scar}")]
    public async Task<IActionResult> RemoveBattleScar(
        Guid forceId,
        Guid unitId,
        string scar,
        CancellationToken ct)
    {
        var userId = User.GetUserId();
        var force = await _forceRepository.GetAsync(new ForceId(forceId), ct);

        if (force is null)
            return NotFound();

        if (force.OwnerId != userId)
            return Forbid();

        var unit = force.Units.FirstOrDefault(u => u.Id == new UnitId(unitId));
        if (unit is null)
            return NotFound();

        try
        {
            unit.RemoveBattleScar(scar);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        await _forceRepository.SaveChangesAsync(ct);

        return NoContent();
    }
}
