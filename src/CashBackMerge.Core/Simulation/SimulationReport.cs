using System.IO.Pipelines;

namespace CashBackMerge.Core;

public sealed class SimulationReport
{
    public int Players { get; }

    public int MovesLimit { get; }

    public int BoardSize { get; }

    public int Seed { get; }

    public string StrategyName { get; }

    public Cashback MaxRewardCashback { get; }

    public decimal MaxRewardAllowedRate { get; }

    public IReadOnlyDictionary<int, int> MaxTileDistribution { get; }

    public IReadOnlyDictionary<decimal, int> RewardDistribution { get; }

    public decimal AverageScore { get; }

    public int MaxScore { get; }

    public int MaxRewardPlayers { get; }

    public decimal MaxRewardRate { get; }

    public SimulationStatus Status { get; }

    private SimulationReport(
        int players,
        int movesLimit,
        int boardSize,
        int seed,
        string strategyName,
        Cashback maxRewardCashback,
        decimal maxRewardAllowedRate,
        IReadOnlyDictionary<int, int> maxTileDistribution,
        IReadOnlyDictionary<decimal, int> rewardDistribution,
        decimal averageScore,
        int maxScore,
        int maxRewardPlayers,
        decimal maxRewardRate,
        SimulationStatus status)
    {
        Players = players;
        MovesLimit = movesLimit;
        BoardSize = boardSize;
        Seed = seed;
        StrategyName = strategyName;
        MaxRewardCashback = maxRewardCashback;
        MaxRewardAllowedRate = maxRewardAllowedRate;
        MaxTileDistribution = maxTileDistribution;
        RewardDistribution = rewardDistribution;
        AverageScore = averageScore;
        MaxScore = maxScore;
        MaxRewardPlayers = maxRewardPlayers;
        MaxRewardRate = maxRewardRate;
        Status = status;
    }

    public static SimulationReport FromResults(CampaignConfig config, string strategyName, int seed, IReadOnlyList<GameResult> results)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(strategyName);
        ArgumentNullException.ThrowIfNull(results);

        if (results.Count == 0)
        {
            throw new ArgumentException("results: нужен хотя бы один итог партии");
        }

        var rewardTable = new RewardTable(config);
        var tileDistribution = new SortedDictionary<int, int>();
        var rewardDistribution = new SortedDictionary<decimal, int>();
        int playersWithMaxReward = 0;
        long totalScore = 0;
        int maxScore = 0;

        foreach (var result in results)
        {
            tileDistribution[result.MaxTile.Percent] = tileDistribution.GetValueOrDefault(result.MaxTile.Percent) + 1;

            var reward = rewardTable.GetReward(result.MaxTile);
            rewardDistribution[reward] = rewardDistribution.GetValueOrDefault(reward) + 1;

            if (rewardTable.IsMaxReward(result.MaxTile))
            {
                playersWithMaxReward++;
            }

            totalScore += result.Score;
            maxScore = Math.Max(maxScore, result.Score);
        }

        var averageScore = (decimal)totalScore / results.Count;
        var maxRewardRate = (decimal)playersWithMaxReward / results.Count;
        var status = playersWithMaxReward <= config.MaxRewardAllowedRate * results.Count
            ? SimulationStatus.Ok
            : SimulationStatus.Failed;

        return new SimulationReport(
            results.Count,
            config.MovesLimit,
            config.BoardSize,
            seed,
            strategyName,
            config.MaxRewardCashback,
            config.MaxRewardAllowedRate,
            tileDistribution.AsReadOnly(),
            rewardDistribution.AsReadOnly(),
            averageScore,
            maxScore,
            playersWithMaxReward,
            maxRewardRate,
            status);
    }
}