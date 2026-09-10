namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: движок ходов — что считается ходом и что после него происходит.</summary>
public class MoveEngineTest
{
    [Fact(DisplayName = "Невалидный ход не меняет поле, не тратит ход и не создаёт плитку",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void InvalidMoveChangesNothing()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "После валидного хода появляется ровно одна новая плитка",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void ValidMoveSpawnsExactlyOneTile()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "После N валидных ходов игра завершена",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void GameEndsAfterMovesLimit()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Достижение плитки 32% не заканчивает игру — доигрываем до лимита ходов",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void ReachingMaxTileDoesNotEndGame()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Score считается по правилам и растёт только при слияниях",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void ScoreIsCalculatedOnMerges()
    {
        Assert.Fail("Тест не реализован");
    }
}