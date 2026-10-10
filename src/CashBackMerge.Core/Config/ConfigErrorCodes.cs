namespace CashBackMerge.Core;

public static class ConfigErrorCodes
{
    public const string JsonInvalid = "config.json.invalid";
    public const string BoardSizeOutOfRange = "config.boardSize.outOfRange";
    public const string MovesLimitNotPositive = "config.movesLimit.notPositive";
    public const string SpawnRulesEmpty = "config.spawnRules.empty";
    public const string SpawnRuleCashbackInvalid = "config.spawnRule.cashback.invalid";
    public const string SpawnRuleProbabilityInvalid = "config.spawnRule.probability.invalid";
    public const string SpawnRuleProbabilityNotPositive = "config.spawnRule.probability.notPositive";
    public const string SpawnRulesProbabilitySumNotOne = "config.spawnRules.probabilitySumNotOne";
}