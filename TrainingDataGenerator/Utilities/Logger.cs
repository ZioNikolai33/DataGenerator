using Serilog;
using Serilog.Core;
using Serilog.Events;
using ILogger = TrainingDataGenerator.Interfaces.ILogger;

namespace TrainingDataGenerator.Utilities;

public sealed class Logger : ILogger
{
    private readonly Serilog.ILogger _logger;
    private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());

    public Serilog.ILogger SerilogLogger => _logger;   
    public static Logger Instance => _instance.Value;

    private Logger()
    {
        _logger = CreateLogger();
    }

    private static Serilog.ILogger CreateLogger()
    {
        var config = GetConfig();
        var levelSwitch = CreateLoggingLevelSwitch(config);

        return new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.File(
                "../../../Generator/logs/app.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 100 * 1024 * 1024,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    private static Config GetConfig()
    {
        try
        {
            var configText = File.ReadAllText("appsettings.json");
            var config = System.Text.Json.JsonSerializer.Deserialize<Config>(configText);
            return config ?? new Config();
        }
        catch (FileNotFoundException)
        {
            // Return default config if appsettings.json doesn't exist
            return new Config();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading configuration: {ex.Message}");
            return new Config();
        }
    }

    private static LoggingLevelSwitch CreateLoggingLevelSwitch(Config config)
    {
        var level = ParseLogEventLevel(config.Logging?.Default);
        return new LoggingLevelSwitch(level);
    }

    private static LogEventLevel ParseLogEventLevel(string? level)
    {
        if (string.IsNullOrWhiteSpace(level))
            return LogEventLevel.Information;

        if (Enum.TryParse<LogEventLevel>(level, true, out var parsedLevel))
            return parsedLevel;

        return LogEventLevel.Information;
    }

    #region ILogger Implementation

    public void Verbose(string message)
    {
        _logger.Verbose(message);
    }

    public void Verbose(string messageTemplate, params object[] propertyValues)
    {
        _logger.Verbose(messageTemplate, propertyValues);
    }

    public void Debug(string message)
    {
        _logger.Debug(message);
    }

    public void Debug(string messageTemplate, params object[] propertyValues)
    {
        _logger.Debug(messageTemplate, propertyValues);
    }

    public void Information(string message)
    {
        _logger.Information(message);
    }

    public void Information(string messageTemplate, params object[] propertyValues)
    {
        _logger.Information(messageTemplate, propertyValues);
    }

    public void Warning(string message)
    {
        _logger.Warning(message);
    }

    public void Warning(string messageTemplate, params object[] propertyValues)
    {
        _logger.Warning(messageTemplate, propertyValues);
    }

    public void Error(string message)
    {
        _logger.Error(message);
    }

    public void Error(string messageTemplate, params object[] propertyValues)
    {
        _logger.Error(messageTemplate, propertyValues);
    }

    public void Error(Exception exception, string message)
    {
        _logger.Error(exception, message);
    }

    public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _logger.Error(exception, messageTemplate, propertyValues);
    }

    public void Fatal(string message)
    {
        _logger.Fatal(message);
    }

    public void Fatal(string messageTemplate, params object[] propertyValues)
    {
        _logger.Fatal(messageTemplate, propertyValues);
    }

    public void Fatal(Exception exception, string message)
    {
        _logger.Fatal(exception, message);
    }

    public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _logger.Fatal(exception, messageTemplate, propertyValues);
    }

    #endregion

    public void Dispose()
    {
        Log.CloseAndFlush();
    }
}
