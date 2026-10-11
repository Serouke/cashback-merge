using System.Text;
using CashBackMerge.App;
using CashBackMerge.Core;

try
{
    Console.OutputEncoding = Encoding.UTF8;
    var parsed = CommandLine.ParseArgs(args);

    var configPath = parsed.ConfigPath;
    var seed = parsed.Seed;

    var text = File.ReadAllText(configPath);

    var source = new SeededRandomSource(seed);
    var engine = new GameEngine(CampaignConfigParser.Parse(text), source);
    var state = engine.CreateInitialState();

    Console.WriteLine(BoardProcessing.BoardToString(state.Board));
    Console.WriteLine($"Оставшиеся ходы {state.MovesLeft}");
    Console.WriteLine($"Счёт {state.Score}");

    return 0;
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
    Console.Error.WriteLine($"Ошибка при чтении файла. " + ioException.Message);
    return 1;
}
catch (UnauthorizedAccessException unauthorizedAccessException)
{
    Console.Error.WriteLine($"Ошибка при чтении файла. " + unauthorizedAccessException.Message);
    return 1;
}


