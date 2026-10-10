using System.Text.Json;
using System.Globalization;

namespace CashBackMerge.Core;

public static class CampaignConfigParser
{
    private const int MaxBoardSize = 10;
    private const int MinBoardSize = 2;
    private const int BoardSizeDefaultValue = 4;
    private const int MaxProbabilityDecimalPlaces = 9;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static CampaignConfig Parse(string json)
    {
        CampaignConfigDto dto;
        try
        {
            dto = JsonSerializer.Deserialize<CampaignConfigDto>(json, JsonOptions);
        }
        catch (JsonException exception)
        {
            throw new IncorrectConfigException(
                [new ConfigError(ConfigErrorCodes.JsonInvalid, $"Некорректный JSON: {exception.Message}")],
                exception);
        }

        var errors = new List<ConfigError>();

        int movesLimit = dto.MovesLimit ?? 0;
        if (movesLimit <= 0)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.MovesLimitNotPositive, "movesLimit: должно быть положительным значением"));
        }

        int boardSize = dto.BoardSize ?? BoardSizeDefaultValue;
        if (boardSize > MaxBoardSize || boardSize < MinBoardSize)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.BoardSizeOutOfRange, $"boardSize: значение должно быть в пределах ({MinBoardSize}, {MaxBoardSize})"));
        }

        var spawnRules = ParseSpawnRules(dto.SpawnRules, errors);

        if (errors.Count != 0)
        {
            throw new IncorrectConfigException(errors);
        }

        return new CampaignConfig(boardSize, movesLimit, spawnRules);
    }

    private static List<SpawnRule> ParseSpawnRules(List<SpawnRuleDto>? spawnRules, List<ConfigError> errors)
    {
        List<SpawnRule> spawnRulesParsed = [];

        if (spawnRules is null || spawnRules.Count == 0)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.SpawnRulesEmpty, "spawnRules: нужно хотя бы одно правило"));
            return spawnRulesParsed;
        }

        for (int i = 0; i < spawnRules.Count; i++)
        {
            SpawnRuleDto rule = spawnRules[i];

            bool cashbackOk = int.TryParse(rule.Cashback, NumberStyles.None, CultureInfo.InvariantCulture, out int percent)
                && Cashback.IsValid(percent);
            if (!cashbackOk)
            {
                errors.Add(new ConfigError(ConfigErrorCodes.SpawnRuleCashbackInvalid, $"spawnRules[{i}].cashback: нужна положительная степень двойки"));
            }

            bool probabilityOk = false;
            if (!decimal.TryParse(rule.Probability, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal probability)
                || probability.Scale > MaxProbabilityDecimalPlaces)
            {
                errors.Add(new ConfigError(ConfigErrorCodes.SpawnRuleProbabilityInvalid, $"spawnRules[{i}].probability: нужно число с точкой и не более {MaxProbabilityDecimalPlaces} знаков после неё"));
            }
            else if (probability <= 0)
            {
                errors.Add(new ConfigError(ConfigErrorCodes.SpawnRuleProbabilityNotPositive, $"spawnRules[{i}].probability: вероятность должна быть больше нуля"));
            }
            else
            {
                probabilityOk = true;
            }

            if (cashbackOk && probabilityOk)
            {
                spawnRulesParsed.Add(new SpawnRule(new Cashback(percent), probability));
            }
        }

        if (spawnRulesParsed.Count == spawnRules.Count
            && spawnRulesParsed.Sum(rule => rule.Probability) != 1m)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.SpawnRulesProbabilitySumNotOne, "spawnRules: сумма вероятностей должна быть равна 1"));
        }

        return spawnRulesParsed;
    }
}