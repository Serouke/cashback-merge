namespace CashBackMerge.App;

public sealed record CompareOptions(IReadOnlyList<string> ConfigPaths, int Seed, int Players, string Strategy, string? OutputPath)
    : CommandLineOptions(Seed);
