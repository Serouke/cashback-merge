namespace CashBackMerge.Core;

public readonly record struct Cashback
{
    public int Percent { get; }

    public Cashback(int percent)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(percent);

        if (!IsValid(percent))
        {
            throw new ArgumentException("Percent: значение не является степенью двойки", nameof(percent));
        }

        Percent = percent;
    }

    public static bool IsValid(int percent)
    {
        return percent > 0 && (percent & (percent - 1)) == 0;
    }

    public Cashback Double()
    {
        return new Cashback(Percent * 2);
    }
}