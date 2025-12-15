using Serilog;

namespace TrainingDataGenerator.Utilities;

public static class Logger
{
    public static Serilog.ILogger Instance => Log.Logger;

    static Logger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                "Generator/logs/app.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 100 * 1024 * 1024,
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }
}
