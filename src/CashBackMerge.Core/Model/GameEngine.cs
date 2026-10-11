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

        var _spawnTable = new SpawnTable(_config);
        _tileSpawner = new TileSpawner(_spawnTable, source);
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
}