namespace CashBackMerge.Core;

public sealed record ReplayResult(GameState InitialState, IReadOnlyList<MoveResult> Steps, bool IsGameOver)
{
    public GameState FinalState => Steps.Count == 0 ? InitialState : Steps[^1].GameState;
}
