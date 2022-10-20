using Microsoft.Extensions.Logging;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;

public class FakeLogger : ILogger
{
    public long LogInformationCallCount { get; set; } = 0;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if(logLevel == LogLevel.Information)
            LogInformationCallCount++;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}