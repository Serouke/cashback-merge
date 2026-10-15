namespace CashBackMerge.Core;

public sealed record MoveResult
{
    public GameState GameState { get; }
    public bool IsFieldChanged => SpawnedTile is not null;
    public int ScoreGained { get; }
    public SpawnedTile? SpawnedTile { get; }

    private MoveResult(GameState gameState, int scoreGained, SpawnedTile? spawnedTile)
    {
        ArgumentNullException.ThrowIfNull(gameState);

        GameState = gameState;
        ScoreGained = scoreGained;
        SpawnedTile = spawnedTile;
    }

    public static MoveResult Unchanged(GameState gameState)
    {
        return new(gameState, 0, null);
    }

    public static MoveResult Changed(GameState gameState, int scoreGained, SpawnedTile spawnedTile)
    {
        return new(gameState, scoreGained, spawnedTile);
    }
}