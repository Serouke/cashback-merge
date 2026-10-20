namespace CashBackMerge.Core;

public static class CampaignComparison
{
    public static IReadOnlyList<ComparedCampaign> Run(IReadOnlyList<NamedCampaign> campaigns, IMoveStrategy strategy, int players, int seed)
    {
        ArgumentNullException.ThrowIfNull(campaigns);
        ArgumentNullException.ThrowIfNull(strategy);

        if (campaigns.Count < 2)
        {
            throw new ArgumentException("Кампаний не может быть меньше двух", nameof(campaigns));
        }

        var comparedCampaigns = new List<ComparedCampaign>(campaigns.Count);

        foreach (var campaign in campaigns)
        {
            var report = CampaignSimulator.Run(campaign.Config, strategy, players, seed);
            comparedCampaigns.Add(new ComparedCampaign(campaign.Name, report));
        }

        return comparedCampaigns;
    }

    public static IReadOnlyList<ComparedCampaign> RunStrategies(NamedCampaign campaign, IReadOnlyList<IMoveStrategy> strategies, int players, int seed)
    {
        ArgumentNullException.ThrowIfNull(campaign);
        ArgumentNullException.ThrowIfNull(strategies);

        if (strategies.Count < 2)
        {
            throw new ArgumentException("Стратегий не может быть меньше двух", nameof(strategies));
        }

        var comparedStrategies = new List<ComparedCampaign>(strategies.Count);

        foreach (var strategy in strategies)
        {
            var report = CampaignSimulator.Run(campaign.Config, strategy, players, seed);
            comparedStrategies.Add(new ComparedCampaign(strategy.Name, report));
        }

        return comparedStrategies;
    }
}