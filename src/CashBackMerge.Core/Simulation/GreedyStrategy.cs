namespace CashBackMerge.Core;

public sealed class GreedyStrategy : IMoveStrategy
{
    private static readonly Direction[] Directions = Enum.GetValues<Direction>();

    public string Name => "greedy";

    public Direction ChooseMove(GameState state, IRandomSource random)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(random);

        var bestDirections = new List<Direction>();
        var bestScore = -1;

        foreach (var direction in Directions)
        {
            var (movedBoard, score) = BoardMover.Move(state.Board, direction);
            if (movedBoard == state.Board)
            {
                continue;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestDirections.Clear();
            }

            if (score == bestScore)
            {
                bestDirections.Add(direction);
            }
        }

        if (bestDirections.Count == 0)
        {
            return Directions[0];
        }

        return bestDirections.Count == 1
            ? bestDirections[0]
            : bestDirections[random.Next(bestDirections.Count)];
    }
}
