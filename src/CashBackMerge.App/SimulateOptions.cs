namespace CashBackMerge.App;

public sealed record SimulateOptions(string ConfigPath, int Seed, int Players, string Strategy, string? OutputPath)
    : CommandLineOptions(ConfigPath, Seed);
