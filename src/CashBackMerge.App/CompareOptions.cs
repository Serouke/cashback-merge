namespace CashBackMerge.App;

public sealed record CompareOptions(IReadOnlyList<string> ConfigPaths, int Seed, int Players, IReadOnlyList<string> Strategies, string? OutputPath)
    : CommandLineOptions(Seed);
