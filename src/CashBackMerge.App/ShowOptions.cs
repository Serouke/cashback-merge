namespace CashBackMerge.App;

public sealed record ShowOptions(string ConfigPath, int Seed) : CommandLineOptions(ConfigPath, Seed);
