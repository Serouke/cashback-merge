using CashBackMerge.Core;

namespace CashBackMerge.Tests.ModelTests;

/// <summary>Тесты модели: координата клетки.</summary>
public class PositionTest
{
    [Fact(DisplayName = "Строка и столбец сохраняются")]
    public void RowAndColumnAreKept()
    {
        var position = new Position(2, 3);

        Assert.Equal(2, position.Row);
        Assert.Equal(3, position.Column);
    }

    [Theory(DisplayName = "Отрицательная координата отклоняется")]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void NegativeCoordinateIsRejected(int row, int column)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Position(row, column));
    }
}