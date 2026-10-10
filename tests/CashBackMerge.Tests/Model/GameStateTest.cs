using CashBackMerge.Core;

namespace CashBackMerge.Tests.Model;

/// <summary>Тесты модели: состояние игры.</summary>
public class GameStateTest
{
    [Fact(DisplayName = "Начальное состояние: лимит ходов, нулевой score, игра не завершена")]
    public void InitialStateIsCreated()
    {
        var board = new Board(4);

        var state = new GameState(board, 100, 0);

        Assert.Equal(board, state.Board);
        Assert.Equal(100, state.MovesLeft);
        Assert.Equal(0, state.Score);
        Assert.False(state.IsFinished);
    }

    [Fact(DisplayName = "Когда ходов не осталось, игра завершена")]
    public void StateWithoutMovesIsFinished()
    {
        var state = new GameState(new Board(4), 0, 12);

        Assert.True(state.IsFinished);
    }
}