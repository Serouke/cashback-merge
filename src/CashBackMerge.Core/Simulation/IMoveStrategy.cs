namespace CashBackMerge.Core;

public interface IMoveStrategy
{
    string Name { get; }
    Direction ChooseMove(GameState state, IRandomSource random);
}