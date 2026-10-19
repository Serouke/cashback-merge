namespace CashBackMerge.Core;

public static class MoveNotation
{
    public static bool TryParse(string? text, out IReadOnlyList<Direction> moves)
    {
        moves = [];

        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        var parsed = new List<Direction>();
        foreach (var letter in text)
        {
            Direction? direction = char.ToUpperInvariant(letter) switch
            {
                'L' => Direction.Left,
                'R' => Direction.Right,
                'U' => Direction.Up,
                'D' => Direction.Down,
                _ => null
            };

            if (direction is null)
            {
                return false;
            }

            parsed.Add(direction.Value);
        }

        moves = parsed;
        return true;
    }

    public static string Format(IEnumerable<Direction> moves)
    {
        ArgumentNullException.ThrowIfNull(moves);

        return string.Concat(moves.Select(ToLetter));
    }

    private static char ToLetter(Direction direction)
    {
        return direction switch
        {
            Direction.Left => 'L',
            Direction.Right => 'R',
            Direction.Up => 'U',
            Direction.Down => 'D',
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
}
