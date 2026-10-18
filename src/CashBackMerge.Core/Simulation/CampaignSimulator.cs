namespace CashBackMerge.Core;

public static class CampaignSimulator
{
    public static SimulationReport Run(CampaignConfig config, IMoveStrategy strategy, int players, int seed)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(strategy);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(players);

        var seededRandomSource = new SeededRandomSource(seed);
        var gameRunner = new GameRunner(config, strategy);
        var gameResults = new List<GameResult>(players);

        for (var i = 0; i < players; i++)
        {
            var playerSeed = seededRandomSource.Next(int.MaxValue);
            gameResults.Add(gameRunner.Play(playerSeed));
        }

        return SimulationReport.FromResults(config, strategy.Name, seed, gameResults);
    }
}