using CashBackMerge.Core;

namespace CashBackMerge.Tests.ModelTests;

/// <summary>Тесты модели: значение плитки.</summary>
public class CashbackTest
{
    [Theory(DisplayName = "Ноль, отрицательное значение и не степень двойки отклоняются")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(3)]
    public void InvalidPercentIsRejected(int percent)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Cashback(percent));
    }

    [Fact(DisplayName = "Удвоение даёт следующую плитку: 2% -> 4%")]
    public void DoubleReturnsNextTile()
    {
        Assert.Equal(new Cashback(4), new Cashback(2).Double());
    }
}