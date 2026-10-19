namespace CashBackMerge.Core;

public static class GameReplay
{
    public static ReplayResult Replay(CampaignConfig config, int seed, IReadOnlyList<Direction> moves)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(moves);

        var engine = new GameEngine(config, new SeededRandomSource(seed));
        var initialState = engine.CreateInitialState();
        var state = initialState;
        var steps = new List<MoveResult>();

        foreach (var direction in moves)
        {
            if (engine.IsGameOver(state))
            {
                break;
            }

            var moveResult = engine.ApplyMove(state, direction);
            steps.Add(moveResult);
            state = moveResult.GameState;
        }

        return new ReplayResult(initialState, steps, engine.IsGameOver(state));
    }
}
