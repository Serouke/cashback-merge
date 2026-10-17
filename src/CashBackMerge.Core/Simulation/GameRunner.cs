namespace CashBackMerge.Core;

public sealed class GameRunner
{
    private readonly CampaignConfig _config;
    private readonly IMoveStrategy _strategy;

    public GameRunner(CampaignConfig config, IMoveStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(strategy);

        _config = config;
        _strategy = strategy;
    }

    public GameResult Play(int seed)
    {
        var random = new SeededRandomSource(seed);

        var engine = new GameEngine(_config, random);

        var state = engine.CreateInitialState();

        while (!engine.IsGameOver(state))
        {
            var direction = _strategy.ChooseMove(state, random);
            var moveResult = engine.ApplyMove(state, direction);
            state = moveResult.GameState;
        }

        var maxTile = state.Board.MaxTile ?? throw new InvalidOperationException("Поле не бывает пустым");

        return new GameResult(maxTile, state.Score);
    }
}