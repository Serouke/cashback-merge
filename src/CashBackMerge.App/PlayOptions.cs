namespace CashBackMerge.App;

public sealed record PlayOptions(string ConfigPath, int Seed) : CommandLineOptions(Seed);
