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

    [Fact(DisplayName = "Слияние работает во всех четырёх направлениях",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void MergeWorksInAllDirections()
    {
        Assert.Fail("Тест не реализован");
    }
}