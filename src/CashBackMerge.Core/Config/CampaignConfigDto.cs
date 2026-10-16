namespace CashBackMerge.Core;

internal readonly record struct CampaignConfigDto(int? BoardSize, int? MovesLimit, List<SpawnRuleDto>? SpawnRules,
                                List<RewardTierDto>? RewardTable, string? MaxRewardCashback, string? MaxRewardAllowedRate);