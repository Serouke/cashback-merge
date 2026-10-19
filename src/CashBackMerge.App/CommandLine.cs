using System.Globalization;
using CashBackMerge.Core;

namespace CashBackMerge.App;

public static class CommandLine
{
    public const string CommandShow = "show";
    public const string CommandSimulate = "simulate";
    public const string CommandPlay = "play";
    public const string CommandReplay = "replay";
    public const string CommandCompare = "compare";
    public const string Usage = """
        Запуск:
          show --config <путь к конфигу> --seed <целое число>
          simulate --config <путь к конфигу> --players <число игроков> --strategy random --seed <целое число> [--output <путь к отчёту>]
          play --config <путь к конфигу> --seed <целое число>
          replay --config <путь к конфигу> --seed <целое число> --moves <ходы буквами L, R, U, D>
          compare --config <путь к конфигу> --config <путь к конфигу> [--config ...] --players <число игроков> --strategy random --seed <целое число> [--output <путь к отчёту>]
        """;

    private const string KeyWordSeed = "--seed";
    private const string KeyWordConfig = "--config";
    private const string KeyWordPlayers = "--players";
    private const string KeyWordStrategy = "--strategy";
    private const string KeyWordOutput = "--output";
    private const string KeyWordMoves = "--moves";

    public static CommandLineOptions ParseArgs(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length == 0)
        {
            throw new CommandLineException("Не указана команда");
        }

        string command = args[0];
        if (command is not (CommandShow or CommandSimulate or CommandPlay or CommandReplay or CommandCompare))
        {
            throw new CommandLineException($"Неизвестная команда: {command}");
        }

        int? seed = null;
        var configPaths = new List<string>();
        int? players = null;
        string? strategy = null;
        string? outputPath = null;
        string? moves = null;

        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case KeyWordSeed:
                    seed = ParseInt(ValueOfParam(args, i), args[i]);
                    i++;
                    break;
                case KeyWordConfig:
                    configPaths.Add(ValueOfParam(args, i));
                    i++;
                    break;
                case KeyWordPlayers:
                    players = ParseInt(ValueOfParam(args, i), args[i]);
                    i++;
                    break;
                case KeyWordStrategy:
                    strategy = ValueOfParam(args, i);
                    i++;
                    break;
                case KeyWordOutput:
                    outputPath = ValueOfParam(args, i);
                    i++;
                    break;
                case KeyWordMoves:
                    moves = ValueOfParam(args, i);
                    i++;
                    break;
                default:
                    throw new CommandLineException($"Неизвестный параметр: {args[i]}");
            }
        }

        if (configPaths.Count == 0)
        {
            throw new CommandLineException($"{KeyWordConfig}: параметр обязателен");
        }

        if (seed is null)
        {
            throw new CommandLineException($"{KeyWordSeed}: параметр обязателен");
        }

        if (command == CommandCompare)
        {
            return CreateCompareOptions(configPaths, seed.Value, players, strategy, outputPath);
        }

        if (configPaths.Count > 1)
        {
            throw new CommandLineException($"{KeyWordConfig}: команде {command} нужен один конфиг");
        }

        var configPath = configPaths[0];

        return command switch
        {
            CommandShow => new ShowOptions(configPath, seed.Value),
            CommandPlay => new PlayOptions(configPath, seed.Value),
            CommandReplay => CreateReplayOptions(configPath, seed.Value, moves),
            CommandSimulate => new SimulateOptions(configPath, seed.Value, RequirePlayers(players), RequireStrategy(strategy), outputPath),
            _ => throw new CommandLineException($"Неизвестная команда: {command}")
        };
    }

    private static CompareOptions CreateCompareOptions(List<string> configPaths, int seed, int? players, string? strategy, string? outputPath)
    {
        if (configPaths.Count < 2)
        {
            throw new CommandLineException($"{KeyWordConfig}: для сравнения нужно хотя бы два конфига");
        }

        return new CompareOptions(configPaths, seed, RequirePlayers(players), RequireStrategy(strategy), outputPath);
    }

    private static ReplayOptions CreateReplayOptions(string configPath, int seed, string? moves)
    {
        if (moves is null)
        {
            throw new CommandLineException($"{KeyWordMoves}: параметр обязателен");
        }

        if (!MoveNotation.TryParse(moves, out var directions))
        {
            throw new CommandLineException($"{KeyWordMoves}: ходы записываются буквами L, R, U, D, например LLUR");
        }

        return new ReplayOptions(configPath, seed, directions);
    }

    private static int RequirePlayers(int? players)
    {
        if (players is null)
        {
            throw new CommandLineException($"{KeyWordPlayers}: параметр обязателен");
        }

        if (players <= 0)
        {
            throw new CommandLineException($"{KeyWordPlayers}: число игроков должно быть больше нуля");
        }

        return players.Value;
    }

    private static string RequireStrategy(string? strategy)
    {
        if (strategy is null)
        {
            throw new CommandLineException($"{KeyWordStrategy}: параметр обязателен");
        }

        return strategy;
    }

    private static int ParseInt(string str, string flagName)
    {
        if (!int.TryParse(str, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int parsed))
        {
            throw new CommandLineException($"{flagName}: ожидалось целое число, получено {str}");
        }

        return parsed;
    }

    private static string ValueOfParam(string[] args, int index)
    {
        var flagName = args[index];

        if (index + 1 >= args.Length || string.IsNullOrWhiteSpace(args[index + 1]) || args[index + 1].StartsWith("--"))
        {
            throw new CommandLineException($"{flagName}: отсутствует значение");
        }

        return args[index + 1];
    }
}
