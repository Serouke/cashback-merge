namespace CashBackMerge.Core;

public sealed class RewardTier
{
    public Cashback Cashback { get; }
    public decimal Reward { get; }
    public RewardTier(Cashback cashback, decimal reward)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(reward);

        Cashback = cashback;
        Reward = reward;
    }
}