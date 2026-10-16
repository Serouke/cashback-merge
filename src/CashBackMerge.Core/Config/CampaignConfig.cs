namespace CashBackMerge.Core;

public sealed class CampaignConfig
{
    public int BoardSize { get; }
    public int MovesLimit { get; }
    public IReadOnlyList<SpawnRule> SpawnRules { get; }
    public IReadOnlyList<RewardTier> RewardTiers { get; }
    public Cashback MaxRewardCashback { get; }
    public decimal MaxRewardAllowedRate { get; }

    internal CampaignConfig(int boardSize, int movesLimit, IReadOnlyList<SpawnRule> spawnRules,
                                IReadOnlyList<RewardTier> rewardTiers, Cashback maxRewardCashback, decimal maxRewardAllowedRate)
    {
        BoardSize = boardSize;
        MovesLimit = movesLimit;
        SpawnRules = spawnRules;
        RewardTiers = rewardTiers;
        MaxRewardCashback = maxRewardCashback;
        MaxRewardAllowedRate = maxRewardAllowedRate;
    }
}