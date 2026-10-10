namespace CashBackMerge.Core;

public sealed class SpawnTable
{
    private readonly IReadOnlyList<int> _ruleTickets;
    private readonly IReadOnlyList<Cashback> _ruleTiles;
    private readonly int _totalTickets;

    public SpawnTable(CampaignConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        int scale = 0;
        foreach (var rule in config.SpawnRules)
        {
            if (scale < rule.Probability.Scale)
            {
                scale = rule.Probability.Scale;
            }
        }

        int totalTickets = 1;
        for (int i = 0; i < scale; i++)
        {
            totalTickets *= 10;
        }
        _totalTickets = totalTickets;

        _ruleTickets = config.SpawnRules
            .Select(rule => (int)(rule.Probability * totalTickets))
            .ToArray();

        _ruleTiles = config.SpawnRules
            .Select(rule => rule.Cashback)
            .ToArray();

        if (_ruleTickets.Sum() != totalTickets)
        {
            throw new ArgumentException("RuleTickets: количество билетов не равно 10^k");
        }
    }

    public Cashback SelectTile(IRandomSource randomSource)
    {
        ArgumentNullException.ThrowIfNull(randomSource);

        int generatedNumber = randomSource.Next(_totalTickets);
        int accumulatedTickets = 0;

        for (int i = 0; i < _ruleTickets.Count; i++)
        {
            accumulatedTickets += _ruleTickets[i];
            if (accumulatedTickets > generatedNumber)
            {
                return _ruleTiles[i];
            }
        }

        throw new InvalidOperationException("Плитка не была выбрана");
    }
}