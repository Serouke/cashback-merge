using CashBackMerge.Core;
using CashBackMerge.Tests.Helpers;

namespace CashBackMerge.Tests.Bonus;

/// <summary>Бонусные тесты: стратегия greedy и её сравнение с random.</summary>
public class GreedyStrategyTest
{
    [Theory(DisplayName = "Greedy выбирает ход с наибольшим счётом")]
    [InlineData(0, Direction.Left)]
    [InlineData(1, Direction.Right)]
    public void GreedyChoosesMoveWithBestScore(int tieBreak, Direction expected)
    {
        // Arrange
        var state = new GameState(Notation.ParseBoard("4,4,.,.", "2,.,.,.", ".,.,.,.", ".,.,.,."), 10, 0);

        // Act
        var direction = new GreedyStrategy().ChooseMove(state, new FakeRandomSource(tieBreak));

        // Assert
        Assert.Equal(expected, direction);
    }

    [Theory(DisplayName = "Greedy не выбирает ход, который не меняет поле")]
    [InlineData(0, Direction.Right)]
    [InlineData(1, Direction.Down)]
    public void GreedySkipsMovesThatChangeNothing(int tieBreak, Direction expected)
    {
        // Arrange
        var state = new GameState(Notation.ParseBoard("2,4,.,.", ".,.,.,.", ".,.,.,.", ".,.,.,."), 10, 0);

        // Act
        var direction = new GreedyStrategy().ChooseMove(state, new FakeRandomSource(tieBreak));

        // Assert
        Assert.Equal(expected, direction);
    }

    [Fact(DisplayName = "На одинаковых seed greedy играет лучше random")]
    public void GreedyBeatsRandomOnSameSeeds()
    {
        // Arrange
        var campaign = new NamedCampaign("40 ходов", CampaignConfigParser.Parse(ConfigJson));
        IMoveStrategy[] strategies = [new RandomStrategy(), new GreedyStrategy()];

        // Act
        var comparison = CampaignComparison.RunStrategies(campaign, strategies, 200, 42);
        var randomReport = comparison[0].Report;
        var greedyReport = comparison[1].Report;

        // Assert
        Assert.Equal("random", comparison[0].Name);
        Assert.Equal("greedy", comparison[1].Name);
        Assert.True(greedyReport.AverageScore > randomReport.AverageScore);
        Assert.True(greedyReport.MaxRewardPlayers > randomReport.MaxRewardPlayers);
    }

    private const string ConfigJson = """
        {
          "movesLimit": 40,
          "spawnRules": [
            { "cashback": "1", "probability": "0.85" },
            { "cashback": "2", "probability": "0.15" }
          ],
          "rewardTable": [
            { "cashback": "2", "reward": "0" },
            { "cashback": "32", "reward": "2000" }
          ],
          "maxRewardCashback": "32",
          "maxRewardAllowedRate": "0.20"
        }
        """;
}
