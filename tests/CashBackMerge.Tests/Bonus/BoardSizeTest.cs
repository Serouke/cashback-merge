using CashBackMerge.Core;
using CashBackMerge.Tests.Helpers;

namespace CashBackMerge.Tests.Bonus;

/// <summary>Бонусные тесты: размер поля задаётся конфигом.</summary>
public class BoardSizeTest
{
    [Theory(DisplayName = "Начальное поле получает размер из конфига")]
    [InlineData(3)]
    [InlineData(5)]
    public void InitialBoardHasConfiguredSize(int size)
    {
        // Arrange
        var config = CampaignConfigParser.Parse(CreateConfigJson(size));
        var engine = new GameEngine(config, new SeededRandomSource(42));

        // Act
        var state = engine.CreateInitialState();

        // Assert
        Assert.Equal(size, state.Board.Size);
        Assert.Equal(size * size - 2, state.Board.GetEmptyCells().Count);
    }

    [Fact(DisplayName = "Ход на поле 5×5 сдвигает и объединяет плитки")]
    public void MoveWorksOnFiveByFiveBoard()
    {
        // Arrange
        var board = Notation.ParseBoard(
            "1,1,.,.,2",
            ".,.,.,.,.",
            ".,.,.,.,.",
            ".,.,.,.,.",
            "4,.,.,.,4");
        var expectedBoard = Notation.ParseBoard(
            "2,2,.,.,.",
            ".,.,.,.,.",
            ".,.,.,.,.",
            ".,.,.,.,.",
            "8,.,.,.,.");

        // Act
        var (movedBoard, score) = BoardMover.Move(board, Direction.Left);

        // Assert
        Assert.Equal(BoardProcessing.BoardToString(expectedBoard), BoardProcessing.BoardToString(movedBoard));
        Assert.Equal(10, score);
    }

    private static string CreateConfigJson(int size)
    {
        return $$"""
            {
              "boardSize": {{size}},
              "movesLimit": 100,
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
