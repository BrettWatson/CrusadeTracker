
namespace CrusadeTracker.Domain.Forces.ValueObjects;

public readonly struct Points
{
    public int Value { get; }

    public Points(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Points cannot be negative.");

        Value = value;
    }
}
