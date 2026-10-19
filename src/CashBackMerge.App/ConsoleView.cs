using CashBackMerge.Core;
using static System.FormattableString;

namespace CashBackMerge.App;

public static class ConsoleView
{
    public static void PrintState(GameState state, RewardTable rewardTable)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(rewardTable);

        Console.WriteLine(BoardProcessing.BoardToString(state.Board));
        Console.WriteLine(Invariant($"Осталось ходов {state.MovesLeft}, счёт {state.Score}, награда {CurrentReward(state, rewardTable):0.##}"));
    }

    public static void PrintResult(GameState state, RewardTable rewardTable)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(rewardTable);

        var maxTile = state.Board.MaxTile?.Percent ?? 0;
        Console.WriteLine(Invariant($"Итог: самая большая плитка {maxTile}%, счёт {state.Score}, награда {CurrentReward(state, rewardTable):0.##}"));
    }

    public static string DescribeMove(Direction direction, MoveResult moveResult)
    {
        ArgumentNullException.ThrowIfNull(moveResult);

        var name = DirectionName(direction);
        if (!moveResult.IsFieldChanged)
        {
            return $"Ход {name}: поле не изменилось, ход не потрачен";
        }

        return moveResult.ScoreGained > 0
            ? Invariant($"Ход {name}: +{moveResult.ScoreGained} к счёту")
            : $"Ход {name}";
    }

    public static string DirectionName(Direction direction)
    {
        return direction switch
        {
            Direction.Left => "влево",
            Direction.Right => "вправо",
            Direction.Up => "вверх",
            Direction.Down => "вниз",
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }

    private static decimal CurrentReward(GameState state, RewardTable rewardTable)
    {
        return state.Board.MaxTile is Cashback maxTile ? rewardTable.GetReward(maxTile) : 0;
    }
}
