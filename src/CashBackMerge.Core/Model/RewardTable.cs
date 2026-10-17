namespace CashBackMerge.Core;

public sealed class RewardTable
{
    private readonly IReadOnlyList<RewardTier> _tiers;
    private readonly Cashback _maxRewardCashback;

    public RewardTable(CampaignConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        _tiers = config.RewardTiers
            .OrderByDescending(tier => tier.Cashback.Percent)
            .ToArray();

        _maxRewardCashback = config.MaxRewardCashback;
    }

    public decimal GetReward(Cashback maxTile)
    {
        foreach (var tier in _tiers)
        {
            if (tier.Cashback.Percent <= maxTile.Percent)
            {
                return tier.Reward;
            }
        }

        return 0;
    }

    public bool IsMaxReward(Cashback maxTile)
    {
        return maxTile.Percent >= _maxRewardCashback.Percent;
    }
}
