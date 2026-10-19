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
}