using System.Globalization;
using CashBackMerge.Core;

namespace CashBackMerge.Tests.Helpers;

/// <summary>Текстовая запись линий в тестах: "1,1,.,." — точка означает пустую клетку.</summary>
public static class Notation
{
    public static Cashback?[] ParseLine(string str)
    {
        var parsedString = str.Split(new[] { ',' }, StringSplitOptions.TrimEntries)
            .Select(x => x == "." ? (Cashback?)null : new Cashback(int.Parse(x, CultureInfo.InvariantCulture)))
            .ToArray();

        return parsedString;
    }
}