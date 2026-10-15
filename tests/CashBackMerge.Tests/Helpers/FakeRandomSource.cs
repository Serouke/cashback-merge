using CashBackMerge.Core;

namespace CashBackMerge.Tests.Helpers;

public sealed class FakeRandomSource : IRandomSource
{
    private readonly int[] _values;
    private int _nextIndex;

    public FakeRandomSource(params int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = values;
    }

    public int Next(int exclusiveUpperBound)
    {
        if (_nextIndex >= _values.Length)
        {
            throw new InvalidOperationException($"Закончились числа из списка рандомных значений (в сценарии было {_values.Length})");
        }
        var value = _values[_nextIndex];

        if (value < 0 || value >= exclusiveUpperBound)
        {
            throw new InvalidOperationException($"Число {value} вне диапазона от 0 до {exclusiveUpperBound - 1}");
        }
        _nextIndex++;

        return value;
    }
}