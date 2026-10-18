using CashBackMerge.Core;
using CashBackMerge.Tests.Helpers;

namespace CashBackMerge.Tests.Mandatory.Mr3;

/// <summary>Обязательные тесты: награды, симуляция и отчёт.</summary>
public class RewardAndSimulationTest
{
    private const string ConfigJson = """
        {
          "movesLimit": 100,
          "spawnRules": [
            { "cashback": "1", "probability": "0.85" },
            { "cashback": "2", "probability": "0.15" }
          ],
          "rewardTable": [
            { "cashback": "2", "reward": "0" },
            { "cashback": "4", "reward": "50" },
            { "cashback": "8", "reward": "150" },
            { "cashback": "16", "reward": "500" },
            { "cashback": "32", "reward": "2000" }
          ],
          "maxRewardCashback": "32",
          "maxRewardAllowedRate": "0.20"
        }
        """;

    [Fact(DisplayName = "Максимальная плитка 16% даёт награду соответствующего tier")]
    public void RewardMatchesHighestTile()
    {
        // Arrange
        var rewardTable = new RewardTable(CampaignConfigParser.Parse(ConfigJson));
        var board = Notation.ParseBoard("16,8,4,2", ".,.,.,.", ".,.,.,.", ".,.,.,.");

        // Act
        var maxTile = Assert.NotNull(board.MaxTile);
        var reward = rewardTable.GetReward(maxTile);

        // Assert
        Assert.Equal(new Cashback(16), maxTile);
        Assert.Equal(500m, reward);
        Assert.False(rewardTable.IsMaxReward(maxTile));
    }

    [Fact(DisplayName = "Weighted random учитывает вероятности из конфига")]
    public void SpawnRespectsConfiguredProbabilities()
    {
        // Arrange
        var spawnTable = new SpawnTable(CampaignConfigParser.Parse(ConfigJson));
        var everyTicketOnce = new FakeRandomSource(Enumerable.Range(0, 100).ToArray());
        var tiles = new List<Cashback>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            tiles.Add(spawnTable.SelectTile(everyTicketOnce));
        }

        // Assert
        Assert.Equal(85, tiles.Count(tile => tile == new Cashback(1)));
        Assert.Equal(15, tiles.Count(tile => tile == new Cashback(2)));
    }

    [Fact(DisplayName = "Отчёт содержит распределение максимальных плиток и наград")]
    public void ReportContainsDistributions()
    {
        // Arrange
        var results = CreateResults(64, 8, 32, 64, 16);

        // Act
        var report = CreateReport(results);

        // Assert
        Assert.Equal(new[] { 8, 16, 32, 64 }, report.MaxTileDistribution.Keys);
        Assert.Equal(new[] { 1, 1, 1, 2 }, report.MaxTileDistribution.Values);
        Assert.Equal(new[] { 150m, 500m, 2000m }, report.RewardDistribution.Keys);
        Assert.Equal(new[] { 1, 1, 3 }, report.RewardDistribution.Values);
    }

    [Fact(DisplayName = "Отчёт содержит долю игроков с максимальной наградой")]
    public void ReportContainsMaxRewardRate()
    {
        // Arrange
        var results = CreateResults(32, 64, 32, 16, 16, 8, 8, 4, 4, 2);

        // Act
        var report = CreateReport(results);

        // Assert
        Assert.Equal(10, report.Players);
        Assert.Equal(3, report.MaxRewardPlayers);
        Assert.Equal(0.3m, report.MaxRewardRate);
    }

    [Fact(DisplayName = "Статус OK, если actualMaxRewardRate <= maxRewardAllowedRate, иначе FAILED")]
    public void ReportStatusReflectsBusinessConstraint()
    {
        // Arrange
        var atLimit = CreateResults(32, 64, 16, 16, 8, 8, 4, 4, 2, 2);
        var overLimit = CreateResults(32, 64, 32, 16, 8, 8, 4, 4, 2, 2);

        // Act
        var atLimitReport = CreateReport(atLimit);
        var overLimitReport = CreateReport(overLimit);

        // Assert
        Assert.Equal(0.2m, atLimitReport.MaxRewardRate);
        Assert.Equal(SimulationStatus.Ok, atLimitReport.Status);
        Assert.Equal(0.3m, overLimitReport.MaxRewardRate);
        Assert.Equal(SimulationStatus.Failed, overLimitReport.Status);
    }

    private static List<GameResult> CreateResults(params int[] maxTiles)
    {
        return maxTiles
            .Select(percent => new GameResult(new Cashback(percent), 0))
            .ToList();
    }

    private static SimulationReport CreateReport(List<GameResult> results)
    {
        return SimulationReport.FromResults(CampaignConfigParser.Parse(ConfigJson), "random", 42, results);
    }
}