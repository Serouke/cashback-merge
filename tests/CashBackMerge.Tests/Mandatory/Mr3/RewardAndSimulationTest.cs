namespace CashBackMerge.Tests.Mandatory.Mr3;

/// <summary>Обязательные тесты: награды, симуляция и отчёт.</summary>
public class RewardAndSimulationTest
{
    [Fact(DisplayName = "Максимальная плитка 16% даёт награду соответствующего tier",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void RewardMatchesHighestTile()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Weighted random учитывает вероятности из конфига",
          Skip = "MR3: реализуй тест и удали эту строку")]
    public void SpawnRespectsConfiguredProbabilities()
    {
        Assert.Fail("Тест не реализован");
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