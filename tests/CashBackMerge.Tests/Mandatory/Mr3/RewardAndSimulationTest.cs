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

    [Fact(DisplayName = "Отчёт содержит распределение максимальных плиток и наград",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void ReportContainsDistributions()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Отчёт содержит долю игроков с максимальной наградой",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void ReportContainsMaxRewardRate()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Статус OK, если actualMaxRewardRate <= maxRewardAllowedRate, иначе FAILED",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void ReportStatusReflectsBusinessConstraint()
    {
        Assert.Fail("Тест не реализован");
    }
}