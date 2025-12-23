using CrusadeTracker.Domain.Common;
using CrusadeTracker.Domain.Forces.ValueObjects;

namespace CrusadeTracker.Domain.Forces;

public sealed class CrusadeUnit : Entity<UnitId>
{
    public string Name { get; private set; } = default!;
    public string DataSheet { get; private set; } = default!;
    public Points Points { get; private set; }
    public ExperiencePoints ExperiencePoints { get; private set; }

    // TODO: Updgrade to a proper value object.
    private readonly List<string> _battleHonours = new();
    public IReadOnlyCollection<string> BattleHonours => _battleHonours.AsReadOnly();

    private readonly List<string> _battleScars = new();
    public IReadOnlyCollection<string> BattleScars => _battleScars.AsReadOnly();

    private CrusadeUnit() { }

    public CrusadeUnit(
        UnitId id,
        string name,
        string dataSheet, 
        Points points)
    {
        Id = id;
        Rename(name);
        DataSheet = string.IsNullOrWhiteSpace(dataSheet) ? throw new ArgumentException(nameof(dataSheet), "DataSheet cannot be blank or null.") : dataSheet.Trim();
        Points = points;
        ExperiencePoints = new ExperiencePoints();
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        Name = name.Trim();
    }

    public void ChangePoints(Points points) => Points = points;
    public void AddExperience(ExperiencePoints xp) => ExperiencePoints += xp;

    // TODO: Ensure limit the number of battle honours a unit can have. unless there is a specific rule that states otherwise.
    // Also, make it so battle honours can be switched out or removed as well as unique per unit.
    public void AddBattleHonour(string honour)
    {
        if (string.IsNullOrWhiteSpace(honour))
            throw new ArgumentException(nameof(honour), "Battle honour cannot be null or empty.");
        if (_battleHonours.Count >= 3)
            throw new InvalidOperationException("Cannot add more than 3 battle honours.");
        _battleHonours.Add(honour.Trim());
            }

    public void RemoveBattleHonour(string honour)
    {
        if (string.IsNullOrWhiteSpace(honour))
            throw new ArgumentException(nameof(honour), "Battle honour cannot be null or empty.");
        if (!_battleHonours.Remove(honour.Trim()))
            throw new InvalidOperationException("Battle honour not found.");
    }

    public void AddBattleScar(string scar)
    {
        if (string.IsNullOrWhiteSpace(scar))
            throw new ArgumentException(nameof(scar), "Battle scar cannot be null or empty.");
        _battleScars.Add(scar.Trim());
    }
    public void RemoveBattleScar(string scar)
    {
        if (string.IsNullOrWhiteSpace(scar))
            throw new ArgumentException(nameof(scar), "Battle scar cannot be null or empty.");
        if (!_battleScars.Remove(scar.Trim()))
            throw new InvalidOperationException("Battle scar not found.");
    }
}
