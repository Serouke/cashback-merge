namespace CashBackMerge.Core;

public interface IRandomSource
{
    int Next(int exclusiveUpperBound);
}