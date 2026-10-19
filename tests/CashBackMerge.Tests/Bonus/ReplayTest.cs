using CashBackMerge.Core;

namespace CashBackMerge.Tests.Bonus;

/// <summary>Бонусные тесты: повтор партии по seed и списку ходов.</summary>
public class ReplayTest
{
    [Fact(DisplayName = "Повтор по seed и списку ходов воспроизводит сыгранную партию")]
    public void ReplayReproducesPlayedGame()
    {
        // Arrange
        var config = CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 100));
        Direction[] moves = [Direction.Left, Direction.Up, Direction.Right, Direction.Down, Direction.Left, Direction.Up];
        var engine = new GameEngine(config, new SeededRandomSource(42));
        var playedState = engine.CreateInitialState();
        foreach (var direction in moves)
        {
            playedState = engine.ApplyMove(playedState, direction).GameState;
        }

        // Act
        var replay = GameReplay.Replay(config, 42, moves);

        // Assert
        Assert.Equal(moves.Length, replay.Steps.Count);
        Assert.Equal(BoardProcessing.BoardToString(playedState.Board), BoardProcessing.BoardToString(replay.FinalState.Board));
        Assert.Equal(playedState.Score, replay.FinalState.Score);
        Assert.Equal(playedState.MovesLeft, replay.FinalState.MovesLeft);
    }

    [Fact(DisplayName = "Повтор останавливается, когда партия окончена")]
    public void ReplayStopsWhenGameIsOver()
    {
        // Arrange
        var config = CampaignConfigParser.Parse(CreateConfigJson(movesLimit: 2));
        Direction[] moves = [Direction.Left, Direction.Right, Direction.Up, Direction.Down, Direction.Left, Direction.Right];

        // Act
        var replay = GameReplay.Replay(config, 42, moves);

        // Assert
        Assert.True(replay.IsGameOver);
        Assert.Equal(0, replay.FinalState.MovesLeft);
        Assert.True(replay.Steps.Count < moves.Length);
        Assert.True(replay.Steps[^1].IsFieldChanged);
    }

    [Fact(DisplayName = "Ходы записываются буквами L, R, U, D")]
    public void MovesAreWrittenAsLetters()
    {
        // Act
        var isParsed = MoveNotation.TryParse("lurD", out var moves);
        var isWrongParsed = MoveNotation.TryParse("LXR", out _);

        // Assert
        Assert.True(isParsed);
        Assert.Equal([Direction.Left, Direction.Up, Direction.Right, Direction.Down], moves);
        Assert.Equal("LURD", MoveNotation.Format(moves));
        Assert.False(isWrongParsed);
        Assert.False(MoveNotation.TryParse("", out _));
    }

    private static string CreateConfigJson(int movesLimit)
    {
        return $$"""
            {
              "movesLimit": {{movesLimit}},
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
    }
}
