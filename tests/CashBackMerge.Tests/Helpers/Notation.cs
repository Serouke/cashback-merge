using System.Globalization;
using CashBackMerge.Core;

namespace CashBackMerge.Tests.Helpers;

/// <summary>Текстовая запись линий и полей в тестах: "1,1,.,." — точка означает пустую клетку.</summary>
public static class Notation
{
    public static Cashback?[] ParseLine(string str)
    {
        var parsedString = str.Split(new[] { ',' }, StringSplitOptions.TrimEntries)
            .Select(x => x == "." ? (Cashback?)null : new Cashback(int.Parse(x, CultureInfo.InvariantCulture)))
            .ToArray();

        return parsedString;
    }

    public static Board ParseBoard(params string[] rows)
    {
        var board = new Board(rows.Length);

        for (int row = 0; row < rows.Length; row++)
        {
            var cells = ParseLine(rows[row]);
            if (cells.Length != rows.Length)
            {
                throw new ArgumentException($"Строка {row}: клеток {cells.Length}, а строк в поле {rows.Length}", nameof(rows));
            }

            for (int column = 0; column < cells.Length; column++)
            {
                if (cells[column] is Cashback tile)
                {
                    board = board.SetTile(new Position(row, column), tile);
                }
            }
        }

        return board;
    }
}