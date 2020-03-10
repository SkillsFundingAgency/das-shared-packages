using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Testing.Fakes
{
    public class FakeLogger<T> : ILogger<T>
    {
        private readonly List<(LogLevel logLevel, Exception exception, string message)> _logMessages = new List<(LogLevel logLevel, Exception exception, string message)>();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logMessages.Add((logLevel, exception, formatter(state, exception)));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool HasErrors => _logMessages.Any(l => l.logLevel == LogLevel.Error);
        public bool HasInfo => _logMessages.Any(l => l.logLevel == LogLevel.Information);
    }
}