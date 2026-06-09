using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FroniusMonitor.Avalonia.Infrastructure;

namespace FroniusMonitor.Avalonia.Tests.Infrastructure;

[TestClass]
public sealed class AvaloniaUiDispatcherTests
{
    [TestMethod]
    public async Task InvokeAsync_ExecutesAction()
    {
        AvaloniaUiDispatcher dispatcher = new();
        bool xExecuted = false;

        await dispatcher.InvokeAsync(() => xExecuted = true);

        Assert.IsTrue(xExecuted);
    }
}
