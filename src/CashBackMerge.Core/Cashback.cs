namespace CashBackMerge.Core;

public readonly record struct Cashback
{
    public int Percent { get; }

    public Cashback(int percent)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(percent);

        if ((percent & (percent - 1)) != 0)
        {
            throw new ArgumentException("Percent: значение не является степенью двойки", nameof(percent));
        }

        Percent = percent;
    }

    public Cashback Double()
    {
        return new Cashback(Percent * 2);
    }
}