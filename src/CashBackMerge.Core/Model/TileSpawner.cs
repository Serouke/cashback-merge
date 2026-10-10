namespace CashBackMerge.Core;

public sealed class TileSpawner
{
    private readonly SpawnTable _spawnTable;
    private readonly IRandomSource _randomSource;

    public TileSpawner(SpawnTable spawnTable, IRandomSource randomSource)
    {
        ArgumentNullException.ThrowIfNull(spawnTable);
        ArgumentNullException.ThrowIfNull(randomSource);

        _spawnTable = spawnTable;
        _randomSource = randomSource;
    }

    public (Board Board, SpawnedTile SpawnedTile) Spawn(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        IReadOnlyList<Position> emptyCells = board.GetEmptyCells();
        if (emptyCells.Count == 0)
        {
            throw new InvalidOperationException("На поле нет пустых клеток");
        }

        Position position = emptyCells[_randomSource.Next(emptyCells.Count)];
        Cashback tile = _spawnTable.SelectTile(_randomSource);
        Board newBoard = board.SetTile(position, tile);

        return (newBoard, new SpawnedTile(position, tile));
    }
}