using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces.ValueObjects;

namespace CrusadeTracker.Domain.Forces;

public sealed class CrusadeForce : Entity<ForceId>, IAggregateRoot
{
    public UserId OwnerId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Faction { get; private set; } = default!;
    public SupplyLimit PointsLimit { get; private set; }

    private readonly List<CrusadeUnit> _units = new();
    public IReadOnlyCollection<CrusadeUnit> Units => _units.AsReadOnly();

    private readonly HashSet<BattleId> _battles = new();
    public IReadOnlyCollection<BattleId> Battles => _battles.ToList().AsReadOnly();

    private CrusadeForce() { }

    private CrusadeForce(
        ForceId id,
        UserId ownerId,
        string name,
        string faction,
        SupplyLimit pointsLimit)
    {
        Id = id;
        OwnerId = ownerId;
        Rename(name);
        ChangeFaction(faction);
        PointsLimit = pointsLimit;
    }

    public static CrusadeForce Create(UserId ownerId, string name, string faction, SupplyLimit pointsLimit)
     => new(new ForceId(Guid.NewGuid()), ownerId, name, faction, pointsLimit);
    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        Name = name.Trim();
    }
    public void ChangeFaction(string faction)
    {
        if (string.IsNullOrWhiteSpace(faction))
            throw new ArgumentException("Faction cannot be null or empty.", nameof(faction));
        Faction = faction.Trim();
    }

    public Points TotalPoints() => new(_units.Sum(u => u.Points.Value));

    public void AddUnit(CrusadeUnit unit)
    {
        if (unit is null)
            throw new ArgumentNullException(nameof(unit), "CrusadeUnit cannot be null.");
        if (_units.Any(u => u.Name == unit.Name))
            throw new InvalidOperationException("Unit with the same name already exists in the force.");

        if (_units.Sum(u => u.Points.Value) + unit.Points.Value > PointsLimit.Value)
            throw new InvalidOperationException("Adding this unit exceeds the points limit of the force.");

        _units.Add(unit);
    }

    public void RemoveUnit(UnitId unitId)
    {
        var unit = _units.FirstOrDefault(u => u.Id == unitId);
        if (unit is null)
            throw new InvalidOperationException("Unit not found in the force.");
        _units.Remove(unit);
    }

    public void RemaneUnit(UnitId unitId, string newName)
    {
        var unit = _units.FirstOrDefault(u => u.Id == unitId);
        if (unit is null)
            throw new InvalidOperationException("Unit not found in the force.");
        if (_units.Any(u => u.Name == newName))
            throw new InvalidOperationException("Another unit with the same name already exists in the force.");
        unit.Rename(newName);
    }

    public void RecordBattle(BattleId battleId)
    {
        if (!_battles.Add(battleId))
            throw new InvalidOperationException("Battle already recorded for this force.");

        // TODO: Apply battle outcomes to units, e.g., adding experience points, battle honours, scars, etc.



        _battles.Add(battleId);
    }
}