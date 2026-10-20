using System.Globalization;
using CashBackMerge.Core;

namespace CashBackMerge.Tests.Bonus;

/// <summary>Бонусные генеративные тесты: правила, которые выполняются на любом случайном поле.</summary>
public class MergePropertyTest
{
    private const int Iterations = 1000;
    private const int GeneratorSeed = 2026;

    [Fact(DisplayName = "Сдвиг в любую сторону не меняет сумму плиток")]
    public void MoveKeepsTileSum()
    {
        var random = new Random(GeneratorSeed);

        for (int i = 0; i < Iterations; i++)
        {
            var board = CreateRandomBoard(random, random.Next(2, 7));

            foreach (var direction in Enum.GetValues<Direction>())
            {
                var (movedBoard, _) = BoardMover.Move(board, direction);

                Assert.True(
                    TileSum(board) == TileSum(movedBoard),
                    $"Ход {direction} изменил сумму плиток на поле:\n{BoardProcessing.BoardToString(board)}");
            }
        }
    }

    [Fact(DisplayName = "Плитка не объединяется дважды за ход")]
    public void TileMergesAtMostOncePerMove()
    {
        var random = new Random(GeneratorSeed);

        for (int i = 0; i < Iterations; i++)
        {
            var line = CreateRandomLine(random, random.Next(2, 9));
            var (mergedLine, score) = LineMerger.MoveLeft(line);

            Assert.True(
                IsExplainedBySingleMerges(line, mergedLine, score),
                $"Строка {FormatLine(line)} превратилась в {FormatLine(mergedLine)}, очки {score}");
        }
    }

    [Fact(DisplayName = "Один и тот же seed даёт одну и ту же партию")]
    public void SameSeedGivesSameGame()
    {
        var config = CampaignConfigParser.Parse(ConfigJson);
        var seeds = new Random(GeneratorSeed);
        IMoveStrategy[] strategies = [new RandomStrategy(), new GreedyStrategy()];

        foreach (var strategy in strategies)
        {
            var runner = new GameRunner(config, strategy);

            for (int i = 0; i < 50; i++)
            {
                var seed = seeds.Next();

                Assert.Equal(runner.Play(seed), runner.Play(seed));
            }
        }
    }

    private static Board CreateRandomBoard(Random random, int size)
    {
        var board = new Board(size);

        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (random.Next(2) == 0)
                {
                    board = board.SetTile(new Position(row, column), CreateRandomTile(random));
                }
            }
        }

        return board;
    }

    private static Cashback?[] CreateRandomLine(Random random, int length)
    {
        var line = new Cashback?[length];

        for (int i = 0; i < length; i++)
        {
            line[i] = random.Next(3) == 0 ? null : CreateRandomTile(random);
        }

        return line;
    }

    private static Cashback CreateRandomTile(Random random)
    {
        return new Cashback(1 << random.Next(4));
    }

    private static int TileSum(Board board)
    {
        var sum = 0;

        for (int row = 0; row < board.Size; row++)
        {
            for (int column = 0; column < board.Size; column++)
            {
                sum += board.GetCell(new Position(row, column))?.Percent ?? 0;
            }
        }

        return sum;
    }

    private static bool IsExplainedBySingleMerges(IReadOnlyList<Cashback?> line, IReadOnlyList<Cashback?> mergedLine, int score)
    {
        var tiles = line.OfType<Cashback>().ToList();
        var mergedTiles = mergedLine.OfType<Cashback>().ToList();
        var next = 0;
        var expectedScore = 0;

        foreach (var tile in mergedTiles)
        {
            if (next + 1 < tiles.Count && tiles[next] == tiles[next + 1] && tile.Percent == tiles[next].Percent * 2)
            {
                expectedScore += tile.Percent;
                next += 2;
            }
            else if (next < tiles.Count && tile == tiles[next])
            {
                next++;
            }
            else
            {
                return false;
            }
        }

        var tilesAreAtStart = mergedLine.Take(mergedTiles.Count).All(tile => tile is not null);

        return next == tiles.Count
            && expectedScore == score
            && mergedLine.Count == line.Count
            && tilesAreAtStart;
    }

    private static string FormatLine(IReadOnlyList<Cashback?> line)
    {
        return string.Join(" ", line.Select(tile => tile?.Percent.ToString(CultureInfo.InvariantCulture) ?? "."));
    }

    private const string ConfigJson = """
        {
          "movesLimit": 40,
          "spawnRules": [
            { "cashback": "1", "probability": "0.85" },
            { "cashback": "2", "probability": "0.15" }
          ],
          "rewardTable": [
            { "cashback": "2", "reward": "0" },
            { "cashback": "32", "reward": "2000" }
          ],
          "maxRewardCashback": "32",
          "maxRewardAllowedRate": "0.20"
        }
        """;
}
