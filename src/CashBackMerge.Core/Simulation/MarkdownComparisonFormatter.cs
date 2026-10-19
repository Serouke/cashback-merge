using static System.FormattableString;
using System.Text;

namespace CashBackMerge.Core;

public static class MarkdownComparisonFormatter
{
    public static string Format(IReadOnlyList<ComparedCampaign> campaigns)
    {
        ArgumentNullException.ThrowIfNull(campaigns);

        if (campaigns.Count == 0)
        {
            throw new ArgumentException("Подан пустой список", nameof(campaigns));
        }

        var builder = new StringBuilder();
        var report = campaigns[0].Report;

        MarkdownReportFormatter.AppendLine(builder, "# Сравнение кампаний Cashback Merge");
        MarkdownReportFormatter.AppendLine(builder, "");
        MarkdownReportFormatter.AppendLine(builder, Invariant($"Условия у всех кампаний одинаковые: {report.Players} игроков, seed {report.Seed}, стратегия {report.StrategyName}."));
        MarkdownReportFormatter.AppendLine(builder, "");

        AppendRow(builder, "Показатель", campaigns.Select(campaign => campaign.Name));
        AppendSeparator(builder, campaigns.Count);
        AppendRow(builder, "Поле", campaigns.Select(campaign => Invariant($"{campaign.Report.BoardSize}×{campaign.Report.BoardSize}")));
        AppendRow(builder, "Лимит ходов", campaigns.Select(campaign => Invariant($"{campaign.Report.MovesLimit}")));
        AppendRow(builder, "Максимальная награда", campaigns.Select(campaign => Invariant($"плитка {campaign.Report.MaxRewardCashback.Percent}% и выше")));
        AppendRow(builder, "Допустимая доля", campaigns.Select(campaign => Invariant($"{campaign.Report.MaxRewardAllowedRate * 100:0.##}%")));
        AppendRow(builder, "Средний score", campaigns.Select(campaign => Invariant($"{campaign.Report.AverageScore:0.##}")));
        AppendRow(builder, "Максимальный score", campaigns.Select(campaign => Invariant($"{campaign.Report.MaxScore}")));
        AppendRow(builder, "Максимальную награду получили", campaigns.Select(campaign => Invariant($"{campaign.Report.MaxRewardRate * 100:0.##}%")));
        AppendRow(builder, "Статус", campaigns.Select(campaign => MarkdownReportFormatter.FormatStatus(campaign.Report.Status)));
        MarkdownReportFormatter.AppendLine(builder, "");

        MarkdownReportFormatter.AppendLine(builder, "## Самая большая плитка");
        MarkdownReportFormatter.AppendLine(builder, "");
        AppendRow(builder, "Плитка", campaigns.Select(campaign => campaign.Name));
        AppendSeparator(builder, campaigns.Count);

        var tiles = campaigns
            .SelectMany(campaign => campaign.Report.MaxTileDistribution.Keys)
            .Distinct()
            .Order();

        foreach (var tile in tiles)
        {
            AppendRow(builder, Invariant($"{tile}%"), campaigns.Select(campaign => Invariant($"{TileShare(campaign.Report, tile):0.##}%")));
        }

        return builder.ToString();
    }

    private static decimal TileShare(SimulationReport report, int tile)
    {
        return MarkdownReportFormatter.Share(report.MaxTileDistribution.GetValueOrDefault(tile), report.Players);
    }

    private static void AppendRow(StringBuilder builder, string label, IEnumerable<string> cells)
    {
        MarkdownReportFormatter.AppendLine(builder, $"| {label} | {string.Join(" | ", cells)} |");
    }

    private static void AppendSeparator(StringBuilder builder, int campaignCount)
    {
        MarkdownReportFormatter.AppendLine(builder, "|" + string.Concat(Enumerable.Repeat("---|", campaignCount + 1)));
    }
}