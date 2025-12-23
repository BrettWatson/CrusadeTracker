
namespace CrusadeTracker.Domain.Forces.ValueObjects;

public readonly struct ExperiencePoints
{
    public int Value { get; }
    public ExperiencePoints(int value = 0)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "ExperiencePoints cannot be negative.");
        Value = value;
    }

    public static ExperiencePoints operator +(ExperiencePoints a, ExperiencePoints b)
        => new (a.Value + b.Value);
}
