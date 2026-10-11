using System.Globalization;

namespace CashBackMerge.App;

public static class CommandLine
{
    public const string CommandShow = "show";
    public const string Usage = "Запуск: show --config <путь к конфигу> --seed <целое число>";

    private const string KeyWordSeed = "--seed";
    private const string KeyWordConfig = "--config";

    public static CommandLineOptions ParseArgs(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length == 0)
        {
            throw new CommandLineException("Не указана команда");
        }

        string command = args[0];
        if (command != CommandShow)
        {
            throw new CommandLineException($"Неизвестная команда: {command}");
        }

        int? seed = null;
        string? configPath = null;

        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case KeyWordSeed:
                    seed = ParseInt(ValueOfParam(args, i), args[i]);
                    i++;
                    break;
                case KeyWordConfig:
                    configPath = ValueOfParam(args, i);
                    i++;
                    break;
                default:
                    throw new CommandLineException($"Неизвестный параметр: {args[i]}");
            }
        }

        if (configPath is null)
        {
            throw new CommandLineException($"{KeyWordConfig}: параметр обязателен");
        }

        if (seed is null)
        {
            throw new CommandLineException($"{KeyWordSeed}: параметр обязателен");
        }

        return new CommandLineOptions(command, configPath, seed.Value);
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