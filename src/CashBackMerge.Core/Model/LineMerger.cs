namespace CashBackMerge.Core;

public static class LineMerger
{
    public static (IReadOnlyList<Cashback?> Line, int Score) MoveLeft(IReadOnlyList<Cashback?> line)
    {
        ArgumentNullException.ThrowIfNull(line);

        var compressedLine = line.OfType<Cashback>().ToArray();
        var mergedLine = new List<Cashback?>();
        var totalScore = 0;

        for (int i = 0; i < compressedLine.Length; i++)
        {
            var current = compressedLine[i];
            if (i + 1 < compressedLine.Length && Equals(compressedLine[i], compressedLine[i + 1]))
            {
                var currentMerged = current.Double();
                mergedLine.Add(currentMerged);
                totalScore += currentMerged.Percent;
                i++;
            }
            else
            {
                mergedLine.Add(current);
            }
        }

        while (mergedLine.Count < line.Count)
        {
            mergedLine.Add(null);
        }

        return (mergedLine, totalScore);
    }
}