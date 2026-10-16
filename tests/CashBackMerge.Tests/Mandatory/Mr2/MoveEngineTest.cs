using CashBackMerge.Core;
using CashBackMerge.Tests.Helpers;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: движок ходов — что считается ходом и что после него происходит.</summary>
public class MoveEngineTest
{
    private const string ConfigJson = """
        {
          "movesLimit": 10,
          "spawnRules": [
            { "cashback": "1", "probability": "0.85" },
            { "cashback": "2", "probability": "0.15" }
          ],
          "rewardTable": [
            { "cashback": "2", "reward": "0" },
            { "cashback": "32", "reward": "2000" }
          ],
          "maxRewardCashback": "32",
          "maxRewardAllowedRate": "0.20"
        }
        """;

    [Fact(DisplayName = "Невалидный ход не меняет поле, не тратит ход и не создаёт плитку")]
    public void InvalidMoveChangesNothing()
    {
        // Arrange
        var engine = CreateEngine();
        var state = CreateState("2,4,.,.", movesLeft: 10, score: 7);

        // Act
        var result = engine.ApplyMove(state, Direction.Left);

        // Assert
        Assert.False(result.IsFieldChanged);
        Assert.Null(result.SpawnedTile);
        Assert.Equal(0, result.ScoreGained);
        Assert.Equal(state, result.GameState);
        Assert.Equal(10, result.GameState.MovesLeft);
        Assert.Equal(7, result.GameState.Score);
    }

    [Fact(DisplayName = "После валидного хода появляется ровно одна новая плитка")]
    public void ValidMoveSpawnsExactlyOneTile()
    {
        // Arrange
        var engine = CreateEngine(0, 90);
        var state = CreateState("2,2,.,.", movesLeft: 10, score: 0);
        var expectedBoard = Notation.ParseBoard("4,2,.,.", ".,.,.,.", ".,.,.,.", ".,.,.,.");

        // Act
        var result = engine.ApplyMove(state, Direction.Left);

        // Assert
        Assert.True(result.IsFieldChanged);
        Assert.Equal(new SpawnedTile(new Position(0, 1), new Cashback(2)), result.SpawnedTile);
        Assert.Equal(BoardProcessing.BoardToString(expectedBoard), BoardProcessing.BoardToString(result.GameState.Board));
        Assert.Equal(14, result.GameState.Board.GetEmptyCells().Count);
        Assert.Equal(9, result.GameState.MovesLeft);
    }

    [Fact(DisplayName = "После N валидных ходов игра завершена")]
    public void GameEndsAfterMovesLimit()
    {
        // Arrange
        var engine = CreateEngine(0, 0, 0, 0, 0, 0);
        var state = CreateState("2,.,.,.", movesLeft: 3, score: 0);

        // Act
        var firstResult = engine.ApplyMove(state, Direction.Right);
        var secondResult = engine.ApplyMove(firstResult.GameState, Direction.Left);
        var thirdResult = engine.ApplyMove(secondResult.GameState, Direction.Right);

        // Assert
        Assert.False(engine.IsGameOver(state));
        Assert.False(engine.IsGameOver(firstResult.GameState));
        Assert.False(engine.IsGameOver(secondResult.GameState));
        Assert.Equal(0, thirdResult.GameState.MovesLeft);
        Assert.True(engine.IsGameOver(thirdResult.GameState));
    }

    [Fact(DisplayName = "Достижение плитки 32% не заканчивает игру — доигрываем до лимита ходов")]
    public void ReachingMaxTileDoesNotEndGame()
    {
        // Arrange
        var engine = CreateEngine(0, 0, 0, 0, 0, 0);
        var state = CreateState(".,16,16,.", movesLeft: 3, score: 0);

        // Act
        var firstResult = engine.ApplyMove(state, Direction.Left);
        var secondResult = engine.ApplyMove(firstResult.GameState, Direction.Right);
        var thirdResult = engine.ApplyMove(secondResult.GameState, Direction.Left);

        // Assert
        Assert.False(engine.IsGameOver(state));
        Assert.True(firstResult.IsFieldChanged);
        Assert.Equal(new Cashback(32), firstResult.GameState.Board.MaxTile);
        Assert.Equal(2, firstResult.GameState.MovesLeft);
        Assert.False(engine.IsGameOver(firstResult.GameState));
        Assert.Equal(1, secondResult.GameState.MovesLeft);
        Assert.False(engine.IsGameOver(secondResult.GameState));
        Assert.Equal(0, thirdResult.GameState.MovesLeft);
        Assert.True(engine.IsGameOver(thirdResult.GameState));
    }

    [Fact(DisplayName = "Score считается по правилам и растёт только при слияниях")]
    public void ScoreIsCalculatedOnMerges()
    {
        // Arrange
        var mergeState = CreateState("2,2,4,4", movesLeft: 10, score: 5);
        var slideState = CreateState(".,.,.,2", movesLeft: 10, score: 5);

        // Act
        var mergeResult = CreateEngine(0, 0).ApplyMove(mergeState, Direction.Left);
        var slideResult = CreateEngine(0, 0).ApplyMove(slideState, Direction.Left);

        // Assert
        Assert.Equal(12, mergeResult.ScoreGained);
        Assert.Equal(17, mergeResult.GameState.Score);
        Assert.True(slideResult.IsFieldChanged);
        Assert.Equal(0, slideResult.ScoreGained);
        Assert.Equal(5, slideResult.GameState.Score);
    }

    private static GameEngine CreateEngine(params int[] randomValues)
    {
        return new GameEngine(CampaignConfigParser.Parse(ConfigJson), new FakeRandomSource(randomValues));
    }

    private static GameState CreateState(string firstRow, int movesLeft, int score)
    {
        return new GameState(Notation.ParseBoard(firstRow, ".,.,.,.", ".,.,.,.", ".,.,.,."), movesLeft, score);
    }
}