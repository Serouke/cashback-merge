namespace CashBackMerge.Core;

public sealed class IncorrectConfigException : Exception
{
    public IReadOnlyList<ConfigError> Errors { get; }

    public IncorrectConfigException(IReadOnlyList<ConfigError> errors, Exception? innerException = null)
        : base(
            "Конфиг не прошел проверку: " + string.Join("; ", (errors ?? Array.Empty<ConfigError>()).Select(error => error.Message)),
            innerException)
    {
        Errors = errors ?? Array.Empty<ConfigError>();
    }
}