using System.Globalization;
using System.Text.RegularExpressions;
using CashBackMerge.Core;

namespace CashBackMerge.Tests.Bonus;

/// <summary>Бонусные тесты: сравнение кампаний на одинаковых seed.</summary>
public class CampaignComparisonTest
{
    [Fact(DisplayName = "Кампании сравниваются на одинаковых seed")]
    public void CampaignsAreComparedOnSameSeeds()
    {
        // Arrange
        var campaign = new NamedCampaign("campaign", CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 100)));

        // Act
        var comparison = CampaignComparison.Run([campaign, campaign], new RandomStrategy(), 200, 42);

        // Assert
        Assert.Equal(MarkdownReportFormatter.Format(comparison[0].Report), MarkdownReportFormatter.Format(comparison[1].Report));
    }

    [Fact(DisplayName = "На одинаковых seed больше ходов даёт не меньше максимальных наград")]
    public void MoreMovesNeverGiveFewerMaxRewardsOnSameSeeds()
    {
        // Arrange
        var shortCampaign = new NamedCampaign("40 ходов", CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 40)));
        var longCampaign = new NamedCampaign("100 ходов", CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 100)));

        // Act
        var comparison = CampaignComparison.Run([shortCampaign, longCampaign], new RandomStrategy(), 200, 42);
        var shortReport = comparison[0].Report;
        var longReport = comparison[1].Report;

        // Assert
        Assert.True(longReport.MaxRewardPlayers >= shortReport.MaxRewardPlayers);
        Assert.True(longReport.AverageScore >= shortReport.AverageScore);
        Assert.Equal(SimulationStatus.Ok, shortReport.Status);
        Assert.Equal(SimulationStatus.Failed, longReport.Status);
    }

    [Fact(DisplayName = "В таблице сравнения столбец на каждую кампанию, плитки по возрастанию")]
    public void ComparisonTableHasColumnPerCampaign()
    {
        // Arrange
        var longCampaign = new NamedCampaign("100 ходов", CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 100)));
        var shortCampaign = new NamedCampaign("20 ходов", CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 20)));
        var comparison = CampaignComparison.Run([longCampaign, shortCampaign], new RandomStrategy(), 200, 42);

        // Act
        var markdown = MarkdownComparisonFormatter.Format(comparison);
        var tiles = Regex.Matches(markdown, @"^\| (\d+)% \|", RegexOptions.Multiline)
            .Select(match => int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture))
            .ToList();

        // Assert
        Assert.Contains("| Показатель | 100 ходов | 20 ходов |\n|---|---|---|\n", markdown);
        Assert.Contains("| Лимит ходов | 100 | 20 |", markdown);
        Assert.Contains("| Статус | FAILED | OK |", markdown);
        Assert.Matches(@"\| 64% \| [\d.]+% \| 0% \|", markdown);
        Assert.Contains(4, tiles);
        Assert.Equal(tiles.Order(), tiles);
    }

    private static string CreateConfigJson(int movesLimit)
    {
        return $$"""
            {
              "movesLimit": {{movesLimit}},
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
}
