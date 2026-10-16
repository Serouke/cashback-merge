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

        int errorsCountBeforeRewardTable = errors.Count;
        var rewardTiers = ParseRewardTiers(dto.RewardTable, errors);
        bool isTableValid = errors.Count == errorsCountBeforeRewardTable;

        Cashback? maxRewardCashback = ParseCashback(dto.MaxRewardCashback);
        if (maxRewardCashback is null)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.MaxRewardCashbackInvalid, "maxRewardCashback: нужна положительная степень двойки"));
        }

        if (!decimal.TryParse(dto.MaxRewardAllowedRate, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal maxRewardAllowedRate) || maxRewardAllowedRate > 1)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.MaxRewardAllowedRateInvalid, "maxRewardAllowedRate: нужно число от 0 до 1"));
        }

        if (isTableValid && maxRewardCashback is not null && !rewardTiers.Any(tier => tier.Cashback == maxRewardCashback.Value))
        {
            errors.Add(new ConfigError(ConfigErrorCodes.MaxRewardCashbackNotInTable, $"maxRewardCashback: процента {maxRewardCashback.Value.Percent} нет в rewardTable"));
        }

        if (errors.Count != 0 || maxRewardCashback is not Cashback maxCashback)
        {
            throw new IncorrectConfigException(errors);
        }

        return new CampaignConfig(
            boardSize,
            movesLimit,
            spawnRules,
            rewardTiers,
            maxCashback,
            maxRewardAllowedRate);
    }

    private static List<RewardTier> ParseRewardTiers(List<RewardTierDto>? rewardTiers, List<ConfigError> errors)
    {
        List<RewardTier> rewardTiersParsed = [];

        if (rewardTiers is null || rewardTiers.Count == 0)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.RewardTableEmpty, "rewardTable: нужна хотя бы одна строка"));
            return rewardTiersParsed;
        }

        HashSet<Cashback> seen = [];

        for (int i = 0; i < rewardTiers.Count; i++)
        {
            int errorsCountBefore = errors.Count;
            RewardTierDto tier = rewardTiers[i];

            Cashback? cashback = ParseCashback(tier.Cashback);
            if (cashback is null)
            {
                errors.Add(new ConfigError(ConfigErrorCodes.RewardTierCashbackInvalid, $"rewardTable[{i}].cashback: нужна положительная степень двойки"));
            }
            else if (!seen.Add(cashback.Value))
            {
                errors.Add(new ConfigError(ConfigErrorCodes.RewardTierCashbackDuplicate, $"rewardTable[{i}].cashback: процент {cashback.Value.Percent} уже есть в таблице"));
            }

            if (!decimal.TryParse(tier.Reward, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal reward))
            {
                errors.Add(new ConfigError(ConfigErrorCodes.RewardTierRewardInvalid, $"rewardTable[{i}].reward: нужно неотрицательное число"));
            }

            if (errors.Count == errorsCountBefore && cashback is Cashback tile)
            {
                rewardTiersParsed.Add(new RewardTier(tile, reward));
            }
        }

        return rewardTiersParsed;
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

            Cashback? cashback = ParseCashback(rule.Cashback);
            if (cashback is null)
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

            if (cashback is Cashback tile && probabilityOk)
            {
                spawnRulesParsed.Add(new SpawnRule(tile, probability));
            }
        }

        if (spawnRulesParsed.Count == spawnRules.Count
            && spawnRulesParsed.Sum(rule => rule.Probability) != 1m)
        {
            errors.Add(new ConfigError(ConfigErrorCodes.SpawnRulesProbabilitySumNotOne, "spawnRules: сумма вероятностей должна быть равна 1"));
        }

        return spawnRulesParsed;
    }

    private static Cashback? ParseCashback(string? text)
    {
        if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out int percent)
            || !Cashback.IsValid(percent))
        {
            return null;
        }

        return new Cashback(percent);
    }
}