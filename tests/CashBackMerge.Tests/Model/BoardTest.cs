using CashBackMerge.Core;

namespace CashBackMerge.Tests.Model;

/// <summary>Тесты модели: игровое поле.</summary>
public class BoardTest
{
    [Theory(DisplayName = "Новое поле пустое и имеет заданный размер")]
    [InlineData(4)]
    [InlineData(5)]
    public void NewBoardIsEmpty(int size)
    {
        var board = new Board(size);

        Assert.Equal(size, board.Size);
        Assert.Equal(size * size, board.GetEmptyCells().Count);
        Assert.Null(board.MaxTile);
    }

    [Fact(DisplayName = "Постановка плитки возвращает новое поле и не меняет исходное")]
    public void SetTileDoesNotChangeOriginalBoard()
    {
        var position = new Position(1, 2);
        var original = new Board(4);

        var updated = original.SetTile(position, new Cashback(2));

        Assert.Null(original.GetCell(position));
        Assert.Equal(new Cashback(2), updated.GetCell(position));
    }

    [Fact(DisplayName = "В занятую клетку нельзя поставить вторую плитку")]
    public void SetTileOnOccupiedCellIsRejected()
    {
        var position = new Position(0, 0);
        var board = new Board(4).SetTile(position, new Cashback(1));

        Assert.Throws<InvalidOperationException>(() => board.SetTile(position, new Cashback(2)));
    }

    [Fact(DisplayName = "Пустые клетки перечисляются по строкам слева направо")]
    public void EmptyCellsAreListedRowByRow()
    {
        var board = new Board(2).SetTile(new Position(0, 1), new Cashback(1));

        var expected = new[] { new Position(0, 0), new Position(1, 0), new Position(1, 1) };

        Assert.Equal(expected, board.GetEmptyCells());
    }

    [Fact(DisplayName = "Поля с одинаковыми плитками равны")]
    public void BoardsWithSameTilesAreEqual()
    {
        var first = new Board(4)
            .SetTile(new Position(0, 0), new Cashback(1))
            .SetTile(new Position(3, 2), new Cashback(8));
        var second = new Board(4)
            .SetTile(new Position(3, 2), new Cashback(8))
            .SetTile(new Position(0, 0), new Cashback(1));

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact(DisplayName = "Поля с разными плитками или размерами не равны")]
    public void DifferentBoardsAreNotEqual()
    {
        var board = new Board(4).SetTile(new Position(0, 0), new Cashback(2));

        Assert.NotEqual(board, new Board(4).SetTile(new Position(0, 0), new Cashback(4)));
        Assert.NotEqual(board, new Board(4).SetTile(new Position(0, 1), new Cashback(2)));
        Assert.NotEqual(new Board(4), new Board(5));
    }
}