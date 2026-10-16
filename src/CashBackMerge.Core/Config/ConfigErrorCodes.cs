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
    public const string RewardTableEmpty = "config.rewardTable.empty";
    public const string RewardTierCashbackInvalid = "config.rewardTier.cashback.invalid";
    public const string RewardTierCashbackDuplicate = "config.rewardTier.cashback.duplicate";
    public const string RewardTierRewardInvalid = "config.rewardTier.reward.invalid";
    public const string MaxRewardCashbackInvalid = "config.maxRewardCashback.invalid";
    public const string MaxRewardCashbackNotInTable = "config.maxRewardCashback.notInTable";
    public const string MaxRewardAllowedRateInvalid = "config.maxRewardAllowedRate.invalid";
}