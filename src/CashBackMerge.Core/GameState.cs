namespace CashBackMerge.Core;

public sealed record GameState
{
    public Board Board { get; }
    public int MovesLeft { get; }
    public int Score { get; }

    public bool IsFinished
    {
        get
        {
            return MovesLeft == 0;
        }
    }

    public GameState(Board board, int movesLeft, int score)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(movesLeft);
        ArgumentOutOfRangeException.ThrowIfNegative(score);

        Board = board ?? throw new ArgumentNullException(nameof(board));
        MovesLeft = movesLeft;
        Score = score;
    }
}