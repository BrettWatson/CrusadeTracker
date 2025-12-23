
namespace CrusadeTracker.Domain.Forces.ValueObjects;

public readonly struct SupplyLimit
{
    public int Value { get; }

    public SupplyLimit(int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "SupplyLimit must be greater than zero.");

        Value = value;
    }
}