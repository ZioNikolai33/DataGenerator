using Serilog;
using Serilog.Events;
using Serilog.Core;

namespace TrainingDataGenerator.Utilities;

public static class Logger
{
    public static Serilog.ILogger Instance => Log.Logger;

    public static Config GetConfig()
    {
        var configText = File.ReadAllText("appsettings.json");
        var config = System.Text.Json.JsonSerializer.Deserialize<Config>(configText);

        return config ?? new Config();
    }

    private static LoggingLevelSwitch CreateLoggingLevelSwitch()
    {
        var config = GetConfig();
        var level = ParseLogEventLevel(config.Logging?.Default);

        return new LoggingLevelSwitch(level);
    }

    private static LogEventLevel ParseLogEventLevel(string? level)
    {
        if (Enum.TryParse<LogEventLevel>(level, true, out var parsedLevel))
            return parsedLevel;
        
        return LogEventLevel.Information;
    }

    static Logger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(CreateLoggingLevelSwitch())
            .WriteTo.File(
                "../../../Generator/logs/app.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 100 * 1024 * 1024,
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }
}
