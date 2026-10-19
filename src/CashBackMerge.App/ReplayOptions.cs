using CashBackMerge.Core;

namespace CashBackMerge.App;

public sealed record ReplayOptions(string ConfigPath, int Seed, IReadOnlyList<Direction> Moves)
    : CommandLineOptions(ConfigPath, Seed);
