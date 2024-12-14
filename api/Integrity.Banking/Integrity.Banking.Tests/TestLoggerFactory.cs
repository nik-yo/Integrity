using Integrity.Banking.Application;
using Microsoft.Extensions.Logging;

namespace Integrity.Banking.Tests
{
    internal class TestLoggerFactory
    {
        public static ILogger<BankingService> CreateLogger()
        {
            using var loggerFactory = LoggerFactory.Create(configure => configure
                .SetMinimumLevel(LogLevel.Debug));
            return loggerFactory.CreateLogger<BankingService>();
        }
    }
}
