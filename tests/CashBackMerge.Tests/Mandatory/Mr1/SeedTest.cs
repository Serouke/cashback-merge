using CashBackMerge.Core;

namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: воспроизводимость по seed.</summary>
public class SeedTest
{
    private const string ConfigJson = """
        {
          "_note": "Проценты и вероятности записаны строками",
          "boardSize": 4,
          "movesLimit": 100,
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

    [Fact(DisplayName = "Одинаковый конфиг и seed дают одинаковое начальное поле")]
    public void SameConfigAndSeedProduceSameInitialBoard()
    {
        // Arrange
        SeededRandomSource source1 = new(42);
        SeededRandomSource source2 = new(42);
        GameEngine engine1 = new(CampaignConfigParser.Parse(ConfigJson), source1);
        GameEngine engine2 = new(CampaignConfigParser.Parse(ConfigJson), source2);

        // Act
        GameState gameState1 = engine1.CreateInitialState();
        GameState gameState2 = engine2.CreateInitialState();

        // Assert
        Assert.Equal(14, gameState1.Board.GetEmptyCells().Count);
        Assert.Equal(gameState1.Board, gameState2.Board);
    }

    [Fact(DisplayName = "Одинаковый конфиг, seed и стратегия дают одинаковый отчёт симуляции")]
    public void SameInputsProduceSameSimulationReport()
    {
        // Arrange
        var config = CampaignConfigParser.Parse(ConfigJson);
        var strategy = new RandomStrategy();

        // Act
        var report = CampaignSimulator.Run(config, strategy, 200, 42);
        var repeatedReport = CampaignSimulator.Run(config, strategy, 200, 42);
        var otherSeedReport = CampaignSimulator.Run(config, strategy, 200, 43);

        // Assert
        Assert.Equal(MarkdownReportFormatter.Format(report), MarkdownReportFormatter.Format(repeatedReport));
        Assert.NotEqual(report.AverageScore, otherSeedReport.AverageScore);
    }
}