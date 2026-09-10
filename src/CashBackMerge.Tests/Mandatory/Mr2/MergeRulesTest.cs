namespace CashBackMerge.Tests.Mandatory.Mr2;

/// <summary>Обязательные тесты: правила слияния плиток.</summary>
public class MergeRulesTest
{
    [Theory(DisplayName = "Сдвиг влево: строка до -> после",
            Skip = "MR2: реализуй тест и удали эту строку")]
    [InlineData("1,1,.,.", "2,.,.,.")]
    [InlineData("1,1,1,.", "2,1,.,.")]
    [InlineData("1,1,1,1", "2,2,.,.")]
    [InlineData("2,2,4,4", "4,8,.,.")]
    public void RowShiftsLeftAndMerges(string before, string expected)
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Одна плитка не сливается дважды за ход: [1,1,2,.] -> [2,2,.,.]",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void TileDoesNotMergeTwiceInOneMove()
    {
        Assert.Fail("Тест не реализован");
    }

    [Fact(DisplayName = "Слияние работает во всех четырёх направлениях",
          Skip = "MR2: реализуй тест и удали эту строку")]
    public void MergeWorksInAllDirections()
    {
        Assert.Fail("Тест не реализован");
    }
}