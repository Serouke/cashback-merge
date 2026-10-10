using CashBackMerge.Core;

namespace CashBackMerge.Tests.Mandatory.Mr1;

/// <summary>Обязательные тесты: конфиг кампании.</summary>
public class ConfigValidationTest
{
    [Fact(DisplayName = "Валидный конфиг принимается")]
    public void ValidConfigIsAccepted()
    {
        // Arrange
        const string json = """
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

        // Act
        var config = CampaignConfigParser.Parse(json);

        // Assert
        Assert.Equal(4, config.BoardSize);
        Assert.Equal(100, config.MovesLimit);
        Assert.Equal(2, config.SpawnRules.Count);
        Assert.Equal(new Cashback(1), config.SpawnRules[0].Cashback);
        Assert.Equal(0.85m, config.SpawnRules[0].Probability);
        Assert.Equal(new Cashback(2), config.SpawnRules[1].Cashback);
        Assert.Equal(0.15m, config.SpawnRules[1].Probability);
    }

    [Fact(DisplayName = "Сумма вероятностей spawn-правил не равна 1 — конфиг отклоняется")]
    public void ProbabilitiesNotSummingToOneAreRejected()
    {
        // Arrange
        const string json = """
            {
              "boardSize": 4,
              "movesLimit": 100,
              "spawnRules": [
                { "cashback": "1", "probability": "0.6" },
                { "cashback": "2", "probability": "0.3" }
              ]
            }
            """;

        // Act & Assert
        AssertRejected(json, ConfigErrorCodes.SpawnRulesProbabilitySumNotOne);
    }

    [Fact(DisplayName = "movesLimit <= 0 — конфиг отклоняется")]
    public void NonPositiveMovesLimitIsRejected()
    {
        // Arrange
        const string zeroLimit = """
            {
              "boardSize": 4,
              "movesLimit": 0,
              "spawnRules": [
                { "cashback": "1", "probability": "1" }
              ]
            }
            """;
        const string negativeLimit = """
            {
              "boardSize": 4,
              "movesLimit": -5,
              "spawnRules": [
                { "cashback": "1", "probability": "1" }
              ]
            }
            """;

        // Act & Assert
        AssertRejected(zeroLimit, ConfigErrorCodes.MovesLimitNotPositive);
        AssertRejected(negativeLimit, ConfigErrorCodes.MovesLimitNotPositive);
    }

    [Fact(DisplayName = "Пустой список spawn-правил — конфиг отклоняется")]
    public void EmptySpawnRulesAreRejected()
    {
        // Arrange
        const string json = """
            {
              "boardSize": 4,
              "movesLimit": 100,
              "spawnRules": []
            }
            """;

        // Act & Assert
        AssertRejected(json, ConfigErrorCodes.SpawnRulesEmpty);
    }

    private static void AssertRejected(string json, string expectedCode)
    {
        var exception = Assert.Throws<IncorrectConfigException>(() => CampaignConfigParser.Parse(json));

        var error = Assert.Single(exception.Errors);
        Assert.Equal(expectedCode, error.Code);
    }
}