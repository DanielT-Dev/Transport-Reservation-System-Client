using Microsoft.Extensions.Logging;

namespace MPP_Client
{
    public static class LoggerManager
    {
        public static readonly ILoggerFactory LoggerFactory = 
            Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information);
            });

        public static ILogger<T> CreateLogger<T>() =>
            LoggerFactory.CreateLogger<T>();
    }
}