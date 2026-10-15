namespace CashBackMerge.Core;

public sealed class GameEngine
{
    private readonly CampaignConfig _config;
    private readonly TileSpawner _tileSpawner;

    private const int StartTilesQuantity = 2;

    public GameEngine(CampaignConfig config, IRandomSource source)
    {
        ArgumentNullException.ThrowIfNull(config);

        _config = config;

        var spawnTable = new SpawnTable(_config);
        _tileSpawner = new TileSpawner(spawnTable, source);
    }

    public GameState CreateInitialState()
    {
        Board board = new(_config.BoardSize);

        for (var i = 0; i < StartTilesQuantity; i++)
        {
            board = _tileSpawner.Spawn(board).Board;
        }

        return new GameState(board, _config.MovesLimit, 0);
    }

    public MoveResult ApplyMove(GameState gameState, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(gameState);

        if (gameState.IsFinished)
        {
            throw new InvalidOperationException("Игра была завершена");
        }

        var (shiftedBoard, moveScore) = BoardMover.Move(gameState.Board, direction);

        if (shiftedBoard == gameState.Board)
        {
            return MoveResult.Unchanged(gameState);
        }

        var (boardWithTile, spawnedTile) = _tileSpawner.Spawn(shiftedBoard);

        var gameStateChanged = new GameState(boardWithTile, gameState.MovesLeft - 1, gameState.Score + moveScore);
        return MoveResult.Changed(gameStateChanged, moveScore, spawnedTile);
    }
}