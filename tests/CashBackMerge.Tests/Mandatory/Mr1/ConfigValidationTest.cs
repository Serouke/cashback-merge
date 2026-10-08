namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: конфиг кампании.</summary>
public class ConfigValidationTest
{
    [Fact(DisplayName = "Валидный конфиг принимается",
          Skip = "MR1: реализуй тест и удали эту строку")]
    public void ValidConfigIsAccepted()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Сумма вероятностей spawn-правил не равна 1 — конфиг отклоняется",
          Skip = "MR1: реализуй тест и удали эту строку")]
    public void ProbabilitiesNotSummingToOneAreRejected()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "movesLimit <= 0 — конфиг отклоняется",
          Skip = "MR1: реализуй тест и удали эту строку")]
    public void NonPositiveMovesLimitIsRejected()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Пустой список spawn-правил — конфиг отклоняется",
          Skip = "MR1: реализуй тест и удали эту строку")]
    public void EmptySpawnRulesAreRejected()
    {
        Assert.Fail("Тест не реализован");
    }
}