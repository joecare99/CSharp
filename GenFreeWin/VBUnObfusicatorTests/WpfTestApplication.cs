using System;

namespace VBUnObfusicator.Tests
{
    internal static class WpfTestApplication
    {
        private static readonly object Sync = new();
        private static App? _application;

        public static App? Instance => _application;

        public static void EnsureCreated()
        {
            lock (Sync)
                _application ??= new App();
        }
    }
}
