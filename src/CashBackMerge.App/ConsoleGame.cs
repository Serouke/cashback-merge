using CashBackMerge.Core;
using static System.FormattableString;

namespace CashBackMerge.App;

public static class ConsoleGame
{
    private const string KeysHint = "W/A/S/D или стрелки — ход, Q — выход";

    public static int Play(CampaignConfig config, PlayOptions options)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(options);

        var engine = new GameEngine(config, new SeededRandomSource(options.Seed));
        var rewardTable = new RewardTable(config);
        var state = engine.CreateInitialState();
        var moves = new List<Direction>();
        var message = "Новая партия";
        var isInterrupted = false;

        while (!engine.IsGameOver(state))
        {
            Redraw(state, rewardTable, message);
            Console.WriteLine(KeysHint);

            var key = ReadKey();
            if (key is null || key == ConsoleKey.Q || key == ConsoleKey.Escape)
            {
                isInterrupted = true;
                break;
            }

            if (ToDirection(key.Value) is not Direction direction)
            {
                message = "Неизвестная клавиша";
                continue;
            }

            var moveResult = engine.ApplyMove(state, direction);
            moves.Add(direction);
            state = moveResult.GameState;
            message = ConsoleView.DescribeMove(direction, moveResult);
        }

        Redraw(state, rewardTable, isInterrupted ? "Игра прервана" : FinishReason(state));
        ConsoleView.PrintResult(state, rewardTable);

        if (moves.Count > 0)
        {
            Console.WriteLine(Invariant($"Повторить партию: replay --config {options.ConfigPath} --seed {options.Seed} --moves {MoveNotation.Format(moves)}"));
        }

        return 0;
    }

    private static void Redraw(GameState state, RewardTable rewardTable, string message)
    {
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }

        ConsoleView.PrintState(state, rewardTable);
        Console.WriteLine(message);
    }

    private static ConsoleKey? ReadKey()
    {
        if (!Console.IsInputRedirected)
        {
            return Console.ReadKey(intercept: true).Key;
        }

        var line = Console.ReadLine();
        if (line is null)
        {
            return null;
        }

        var text = line.Replace("﻿", string.Empty).Trim();
        if (text.Length == 0)
        {
            return ConsoleKey.NoName;
        }

        var letter = char.ToUpperInvariant(text[0]);
        return letter is >= 'A' and <= 'Z' ? (ConsoleKey)letter : ConsoleKey.NoName;
    }

    private static Direction? ToDirection(ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.W or ConsoleKey.UpArrow => Direction.Up,
            ConsoleKey.A or ConsoleKey.LeftArrow => Direction.Left,
            ConsoleKey.S or ConsoleKey.DownArrow => Direction.Down,
            ConsoleKey.D or ConsoleKey.RightArrow => Direction.Right,
            _ => null
        };
    }

    private static string FinishReason(GameState state)
    {
        return state.IsFinished
            ? "Партия окончена: ходы закончились"
            : "Партия окончена: ни один ход не меняет поле";
    }
}
