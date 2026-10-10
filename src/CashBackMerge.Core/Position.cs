namespace CashBackMerge.Core;

public readonly record struct Position
{
    public int Row { get; }
    public int Column { get; }

    public Position(int row, int column)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfNegative(column);

        Row = row;
        Column = column;
    }
}