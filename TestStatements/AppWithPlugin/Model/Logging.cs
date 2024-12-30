using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace AppWithPlugin.Model
{
    public class Logging : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Debug.WriteLine($"[{logLevel}] {eventId.Id} {formatter(state, exception)}");
        }
    }
}