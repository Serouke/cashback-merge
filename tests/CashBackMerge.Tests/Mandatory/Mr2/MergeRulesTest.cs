using CashBackMerge.Core;
using CashBackMerge.Tests.Helpers;

namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: правила слияния плиток.</summary>
public class MergeRulesTest
{
    [Theory(DisplayName = "Сдвиг влево: строка до -> после")]
    [InlineData("1,1,.,.", "2,.,.,.")]
    [InlineData("1,1,1,.", "2,1,.,.")]
    [InlineData("1,1,1,1", "2,2,.,.")]
    [InlineData("2,2,4,4", "4,8,.,.")]
    public void RowShiftsLeftAndMerges(string before, string expected)
    {
        // Arrange
        var beforeSplitted = Notation.ParseLine(before);
        var expectedSplitted = Notation.ParseLine(expected);

        // Act & Assert
        Assert.Equal(expectedSplitted, LineMerger.MoveLeft(beforeSplitted).Line);
    }

    [Fact(DisplayName = "Одна плитка не сливается дважды за ход: [1,1,2,.] -> [2,2,.,.]")]
    public void TileDoesNotMergeTwiceInOneMove()
    {
        // Arrange
        var beforeSplitted = Notation.ParseLine("1,1,2,.");
        var expectedSplitted = Notation.ParseLine("2,2,.,.");

        // Act
        var result = LineMerger.MoveLeft(beforeSplitted);

        // Assert
        Assert.Equal(expectedSplitted, result.Line);
        Assert.Equal(2, result.Score);
    }

    [Theory(DisplayName = "Слияние работает во всех четырёх направлениях")]
    [InlineData(Direction.Left, "4,.,.,.", ".,.,.,.", ".,.,.,.", "4,.,.,.")]
    [InlineData(Direction.Right, ".,.,.,4", ".,.,.,.", ".,.,.,.", ".,.,.,4")]
    [InlineData(Direction.Up, "4,.,.,4", ".,.,.,.", ".,.,.,.", ".,.,.,.")]
    [InlineData(Direction.Down, ".,.,.,.", ".,.,.,.", ".,.,.,.", "4,.,.,4")]
    public void MergeWorksInAllDirections(Direction direction, string row0, string row1, string row2, string row3)
    {
        // Arrange
        var board = Notation.ParseBoard("2,.,.,2", ".,.,.,.", ".,.,.,.", "2,.,.,2");
        var expected = Notation.ParseBoard(row0, row1, row2, row3);

        // Act
        var (movedBoard, score) = BoardMover.Move(board, direction);

        // Assert
        Assert.Equal(BoardProcessing.BoardToString(expected), BoardProcessing.BoardToString(movedBoard));
        Assert.Equal(8, score);
    }
}