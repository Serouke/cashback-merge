using System.Text;
using static System.FormattableString;

namespace CashBackMerge.Core;

public static class MarkdownReportFormatter
{
    public static string Format(SimulationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();

        AppendLine(builder, "# Отчёт симуляции Cashback Merge");
        AppendLine(builder, "");
        AppendLine(builder, "## Параметры");
        AppendLine(builder, "");
        AppendLine(builder, "| Параметр | Значение |");
        AppendLine(builder, "|---|---|");
        AppendLine(builder, Invariant($"| Игроков | {report.Players} |"));
        AppendLine(builder, Invariant($"| Лимит ходов | {report.MovesLimit} |"));
        AppendLine(builder, Invariant($"| Поле | {report.BoardSize}×{report.BoardSize} |"));
        AppendLine(builder, Invariant($"| Seed | {report.Seed} |"));
        AppendLine(builder, $"| Стратегия | {report.StrategyName} |");
        AppendLine(builder, Invariant($"| Максимальная награда | плитка {report.MaxRewardCashback.Percent}% и выше |"));
        AppendLine(builder, Invariant($"| Допустимая доля | {report.MaxRewardAllowedRate * 100:0.##}% |"));
        AppendLine(builder, "");
        AppendLine(builder, "## Максимальные плитки");
        AppendLine(builder, "");
        AppendLine(builder, "| Плитка | Игроков | Доля |");
        AppendLine(builder, "|---|---|---|");
        foreach (var (percent, players) in report.MaxTileDistribution)
        {
            AppendLine(builder, Invariant($"| {percent}% | {players} | {Share(players, report.Players):0.##}% |"));
        }

        AppendLine(builder, "");
        AppendLine(builder, "## Награды");
        AppendLine(builder, "");
        AppendLine(builder, "| Награда | Игроков | Доля |");
        AppendLine(builder, "|---|---|---|");
        foreach (var (reward, players) in report.RewardDistribution)
        {
            AppendLine(builder, Invariant($"| {reward:0.##} | {players} | {Share(players, report.Players):0.##}% |"));
        }

        AppendLine(builder, "");
        AppendLine(builder, "## Score");
        AppendLine(builder, "");
        AppendLine(builder, "| Средний | Максимальный |");
        AppendLine(builder, "|---|---|");
        AppendLine(builder, Invariant($"| {report.AverageScore:0.##} | {report.MaxScore} |"));
        AppendLine(builder, "");
        AppendLine(builder, "## Итог");
        AppendLine(builder, "");
        AppendLine(builder, Invariant($"Максимальную награду получили {report.MaxRewardPlayers} из {report.Players} игроков — {report.MaxRewardRate * 100:0.##}%, допустимо {report.MaxRewardAllowedRate * 100:0.##}%."));
        AppendLine(builder, "");
        AppendLine(builder, $"**Статус: {FormatStatus(report.Status)}**");

        return builder.ToString();
    }

    public static string FormatStatus(SimulationStatus status)
    {
        return status switch
        {
            SimulationStatus.Ok => "OK",
            SimulationStatus.Failed => "FAILED",
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    internal static decimal Share(int players, int totalPlayers)
    {
        return players * 100m / totalPlayers;
    }

    internal static void AppendLine(StringBuilder builder, string line)
    {
        builder.Append(line).Append('\n');
    }
}
