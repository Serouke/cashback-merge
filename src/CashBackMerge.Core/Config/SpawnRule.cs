namespace CashBackMerge.Core;

public sealed class SpawnRule
{
    public Cashback Cashback { get; }
    public decimal Probability { get; }
    public SpawnRule(Cashback cashback, decimal probability)
    {
        Cashback = cashback;
        Probability = probability;
    }
}