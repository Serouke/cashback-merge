namespace CashBackMerge.Core;

public sealed class CampaignConfig
{
    public int BoardSize { get; }
    public int MovesLimit { get; }
    public IReadOnlyList<SpawnRule> SpawnRules { get; }

    internal CampaignConfig(int boardSize, int movesLimit, IReadOnlyList<SpawnRule> spawnRules)
    {
        BoardSize = boardSize;
        MovesLimit = movesLimit;
        SpawnRules = spawnRules;
    }
}