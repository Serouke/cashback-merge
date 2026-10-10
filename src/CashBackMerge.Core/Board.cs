namespace CashBackMerge.Core;

public sealed class Board : IEquatable<Board>
{
    private readonly Cashback?[,] _cells;
    public int Size { get; }

    public Cashback? MaxTile
    {
        get
        {
            Cashback? max = null;
            foreach (var cell in _cells)
            {
                if (cell != null)
                {
                    if (max == null || cell.Value.Percent > max.Value.Percent)
                    {
                        max = cell;
                    }
                }
            }
            return max;
        }
    }

    public Board(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        Size = size;
        _cells = new Cashback?[size, size];
    }

    private Board(int size, Cashback?[,] cells)
    {
        Size = size;
        _cells = cells;
    }

    public Cashback? GetCell(Position position)
    {
        ValidateBounds(position);
        return _cells[position.Row, position.Column];
    }

    public Board SetTile(Position position, Cashback tile)
    {
        ValidateBounds(position);

        if (_cells[position.Row, position.Column] != null)
        {
            throw new InvalidOperationException("Клетка занята");
        }

        Cashback?[,] newCells = (Cashback?[,])_cells.Clone();
        newCells[position.Row, position.Column] = tile;

        return new Board(Size, newCells);
    }

    public IReadOnlyList<Position> GetEmptyCells()
    {
        var emptyCells = new List<Position>();
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                if (_cells[r, c] == null)
                {
                    emptyCells.Add(new Position(r, c));
                }
            }
        }
        return emptyCells;
    }

    private void ValidateBounds(Position position)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position.Row, Size);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(position.Column, Size);
    }

    public bool Equals(Board? other)
    {
        if (other is null)
        {
            return false;
        }
        if (Size != other.Size)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                if (!EqualityComparer<Cashback?>.Default.Equals(_cells[r, c], other._cells[r, c]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Board);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Size);

        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                hash.Add(_cells[r, c]);
            }
        }

        return hash.ToHashCode();
    }

    public static bool operator ==(Board? left, Board? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Board? left, Board? right)
    {
        return !Equals(left, right);
    }
}