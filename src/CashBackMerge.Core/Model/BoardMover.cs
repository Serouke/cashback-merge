namespace CashBackMerge.Core;

public static class BoardMover
{
    public static (Board Board, int TotalScore) Move(Board board, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(board);

        var n = board.Size;
        var newBoard = new Board(n);
        var totalScore = 0;

        for (int k = 0; k < n; k++)
        {
            var line = new Cashback?[n];
            for (int j = 0; j < n; j++)
            {
                line[j] = board.GetCell(GetPosition(direction, k, j, n));
            }
            var (mergedLine, score) = LineMerger.MoveLeft(line);
            totalScore += score;
            for (int j = 0; j < n; j++)
            {
                var tile = mergedLine[j];
                if (tile is not null)
                {
                    newBoard = newBoard.SetTile(GetPosition(direction, k, j, n), tile.Value);
                }
            }
        }

        return (newBoard, totalScore);
    }

    private static Position GetPosition(Direction direction, int k, int j, int n) => direction switch
    {
        Direction.Left => new Position(k, j),
        Direction.Right => new Position(k, n - 1 - j),
        Direction.Up => new Position(j, k),
        Direction.Down => new Position(n - 1 - j, k),
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}