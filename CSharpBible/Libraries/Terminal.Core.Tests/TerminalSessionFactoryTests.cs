using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Terminal.Core;

namespace Terminal.Core.Tests;

[TestClass]
public class TerminalSessionFactoryTests
{
    [TestMethod]
    public void CreateSession_ShouldReturnFirstSupportedBackendSession()
    {
        var expectedSession = new FakeTerminalSession();
        var factory = new TerminalSessionFactory(
        [
            new FakeBackendFactory(false, new FakeTerminalSession()),
            new FakeBackendFactory(true, expectedSession)
        ]);

        var session = factory.CreateSession();

        Assert.AreSame(expectedSession, session);
    }

    [TestMethod]
    public void CreateSession_WithoutSupportedBackend_ShouldThrow()
    {
        var factory = new TerminalSessionFactory(
        [
            new FakeBackendFactory(false, new FakeTerminalSession())
        ]);

        Assert.ThrowsExactly<PlatformNotSupportedException>(() => factory.CreateSession());
    }

    private sealed class FakeBackendFactory(bool isSupported, ITerminalSession session) : ITerminalSessionBackendFactory
    {
        public string Name => nameof(FakeBackendFactory);

        public bool IsSupported => isSupported;

        public ITerminalSession CreateSession() => session;
    }

    private sealed class FakeTerminalSession : ITerminalSession
    {
#pragma warning disable CS0067 // The event is never used
        public event EventHandler<string>? OutputReceived;
#pragma warning restore CS0067

        public bool IsRunning => false;

        public TerminalSize Size => new(80, 25);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public Task ResizeAsync(TerminalSize size, System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task StartAsync(TerminalSessionOptions options, System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task StopAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task WriteAsync(string input, System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
