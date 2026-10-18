using System.Text;
using CashBackMerge.App;
using CashBackMerge.Core;

try
{
    Console.OutputEncoding = Encoding.UTF8;
    var options = CommandLine.ParseArgs(args);

    var text = File.ReadAllText(options.ConfigPath);
    var config = CampaignConfigParser.Parse(text);

    return options switch
    {
        ShowOptions show => Show(config, show),
        SimulateOptions simulate => Simulate(config, simulate),
        _ => throw new InvalidOperationException($"Нет обработчика для команды {options.GetType().Name}")
    };
}
catch (CommandLineException commandLineException)
{
    Console.Error.WriteLine(commandLineException.Message);
    Console.Error.WriteLine(CommandLine.Usage);
    return 1;
}
catch (IncorrectConfigException configException)
{
    foreach (var configError in configException.Errors)
    {
        Console.Error.WriteLine($"{configError.Code} {configError.Message}");
    }
    return 1;
}
catch (IOException ioException)
{
    Console.Error.WriteLine("Ошибка при работе с файлом. " + ioException.Message);
    return 1;
}
catch (UnauthorizedAccessException unauthorizedAccessException)
{
    Console.Error.WriteLine("Ошибка при работе с файлом. " + unauthorizedAccessException.Message);
    return 1;
}

static int Show(CampaignConfig config, ShowOptions options)
{
    var engine = new GameEngine(config, new SeededRandomSource(options.Seed));
    var state = engine.CreateInitialState();

    Console.WriteLine(BoardProcessing.BoardToString(state.Board));
    Console.WriteLine($"Оставшиеся ходы {state.MovesLeft}");
    Console.WriteLine($"Счёт {state.Score}");

    return 0;
}

static int Simulate(CampaignConfig config, SimulateOptions options)
{
    var strategy = CreateStrategy(options.Strategy);
    var report = CampaignSimulator.Run(config, strategy, options.Players, options.Seed);
    var markdown = MarkdownReportFormatter.Format(report);

    if (options.OutputPath is null)
    {
        Console.Write(markdown);
    }
    else
    {
        File.WriteAllText(options.OutputPath, markdown);
        Console.WriteLine($"Отчёт записан в {options.OutputPath}");
    }

    Console.WriteLine($"Статус: {MarkdownReportFormatter.FormatStatus(report.Status)}");

    return report.Status == SimulationStatus.Ok ? 0 : 2;
}

static IMoveStrategy CreateStrategy(string name)
{
    return name switch
    {
        "random" => new RandomStrategy(),
        _ => throw new CommandLineException($"Неизвестная стратегия: {name}. Доступна: random")
    };
}
