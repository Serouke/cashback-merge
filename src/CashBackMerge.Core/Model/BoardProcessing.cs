using System.Globalization;

namespace CashBackMerge.Core;

public static class BoardProcessing
{
    public static string BoardToString(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        const string emptyCell = ".";
        List<string> boardRows = [];
        var cellWidth = board.MaxTile?.Percent.ToString(CultureInfo.InvariantCulture).Length ?? 1;

        for (int row = 0; row < board.Size; row++)
        {
            List<string> rowCells = [];
            for (int column = 0; column < board.Size; column++)
            {
                var cell = board.GetCell(new Position(row, column));
                var cellValue = cell?.Percent.ToString(CultureInfo.InvariantCulture) ?? emptyCell;
                rowCells.Add(cellValue.PadLeft(cellWidth));
            }

            boardRows.Add(string.Join(" ", rowCells));
        }

        return string.Join("\n", boardRows);
    }
}