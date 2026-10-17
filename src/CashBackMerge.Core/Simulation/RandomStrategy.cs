namespace CashBackMerge.Core;

public sealed class RandomStrategy : IMoveStrategy
{
    private static readonly Direction[] Directions = Enum.GetValues<Direction>();

    public string Name => "random";

    public Direction ChooseMove(GameState state, IRandomSource random)
    {
        ArgumentNullException.ThrowIfNull(random);
        return Directions[random.Next(Directions.Length)];
    }
}