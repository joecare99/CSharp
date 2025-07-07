using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AppWithPlugin.Model
{
    public class Logging : ILogger
    {     
        public static Action<string> _logAction { get; set; } = (s) => Debug.WriteLine(s);

        List<IDisposable> _scopes = new();

        private class Scope : IDisposable
        {
            private readonly Logging _logger;

            public WeakReference<object> State => state;

            WeakReference<object> state;
            public Scope(Logging logger,object state)
            {
                _logger = logger;
                this.state = new(state);
                _logger._scopes.Add(this);
            }
            public void Dispose()
            {
                _logger._scopes.Remove(this);
            }
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            if (state is IDisposable disposable)
            {
                if (!_scopes.Contains(disposable))
                {
                    _scopes.Add(disposable);    
                    return disposable;
                }
                return null;
            }
            else
            {
                return new Scope(this, state);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                _logAction($"[{logLevel}] {eventId.Id} {formatter(state, exception)}");                
            }
        }
    }
}