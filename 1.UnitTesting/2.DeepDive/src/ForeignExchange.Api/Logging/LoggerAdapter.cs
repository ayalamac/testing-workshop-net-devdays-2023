namespace ForeignExchange.Api.Logging;

public class LoggerAdapter<T> : ILoggerAdapter<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string? message, params object?[] args)
    {
        _logger.LogInformation(message, args);
    }
}

public interface ILoggerAdapter<T>
{
    public void LogInformation(string? message, params object?[] args);
}
