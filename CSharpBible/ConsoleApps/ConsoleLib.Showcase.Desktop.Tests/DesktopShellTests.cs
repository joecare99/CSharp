using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Desktop;
using ConsoleLib.Showcase.Desktop.Capabilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class DesktopShellTests
{
    [TestMethod]
    public void OpeningApplicationCreatesVisibleWindowView()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());

        Assert.IsTrue(shell.WindowManager.Open("Calendar"));

        Assert.AreEqual("Calendar", shell.WindowManager.Active!.Id);
        Assert.IsTrue(shell.Children.OfType<DesktopWindowView>().Any());
        Assert.IsTrue(shell.Children.OfType<DesktopWindowView>().Single().Visible);
    }

    [TestMethod]
    public void MinimizingApplicationHidesWindowViewAndRestoringShowsIt()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());
        shell.WindowManager.Open("Calculator");
        var view = shell.Children.OfType<DesktopWindowView>().Single();

        shell.WindowManager.Minimize("Calculator");
        Assert.IsFalse(view.Visible);

        shell.WindowManager.Restore("Calculator");
        Assert.IsTrue(view.Visible);
    }

    [TestMethod]
    public void ResizingShellRelayoutsTaskbarAndClampsOpenWindows()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());
        shell.WindowManager.Open("Notepad");
        var view = shell.Children.OfType<DesktopWindowView>().Single();
        view.Position = new Point(90, 20);

        shell.RaiseResizeEvent(new Point(80, 25));

        Assert.AreEqual(new Point(20, 1), view.Position);
        Assert.AreEqual(new Size(60, 22), view.size);
        Assert.AreEqual(new Point(1, 24), shell.Children.OfType<Label>().Single(label => label.Text.Contains("Ready")).Position);
    }

    [TestMethod]
    public void ResizingBelowMinimumShowsResizeMessage()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());

        shell.RaiseResizeEvent(new Point(40, 12));

        StringAssert.Contains(shell.Children.OfType<Label>().Single(label => label.Position.Y == 11).Text, "Resize console");
    }

    [TestMethod]
    public void ClosingTerminalWindowDisposesPageResources()
    {
        var terminal = new FakeTerminalCapability();
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel(), capabilities: new FakeHostCapabilities(terminal));

        shell.WindowManager.Open("Terminal");
        Assert.IsTrue(shell.WindowManager.Close("Terminal"));

        Assert.IsTrue(terminal.Disposed);
    }

    [TestMethod]
    public void ClosingAndReopeningApplicationCreatesItsViewAgain()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());

        shell.WindowManager.Open("Calendar");
        Assert.AreEqual(1, shell.Children.OfType<DesktopWindowView>().Count());

        shell.WindowManager.Close("Calendar");
        Assert.AreEqual(0, shell.Children.OfType<DesktopWindowView>().Count());

        shell.WindowManager.Open("Calendar");
        Assert.AreEqual(1, shell.Children.OfType<DesktopWindowView>().Count());
        Assert.IsTrue(shell.Children.OfType<DesktopWindowView>().Single().Visible);
    }

    private sealed class FakeHostCapabilities : IShowcaseHostCapabilities
    {
        public FakeHostCapabilities(IShowcaseTerminalCapability? terminal) => Terminal = terminal;

        public IClipboardService? Clipboard => null;
        public bool SupportsMouse => true;
        public bool SupportsTerminal => true;
        public IShowcaseTerminalCapability? Terminal { get; }
        public IShowcaseAlertService Alert => new FakeAlertService();
    }

    private sealed class FakeAlertService : IShowcaseAlertService
    {
        public bool IsAvailable => true;

        public void Alert() { }
    }

    private sealed class FakeTerminalCapability : IShowcaseTerminalCapability
    {
        public bool IsAvailable => true;
        public bool IsRunning { get; private set; }
        public bool Disposed { get; private set; }
        public event EventHandler<string>? OutputChanged;

        public Task StartAsync(Size size, CancellationToken cancellationToken = default)
        {
            IsRunning = true;
            OutputChanged?.Invoke(this, "started");
            return Task.CompletedTask;
        }

        public Task SendInputAsync(string input, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ResizeAsync(Size size, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            IsRunning = false;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            IsRunning = false;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class WidgetSetStub : IWidgetSet
    {
        public Rectangle ClipRect => new(0, 0, 100, 30);
        public void InitializeApplication(IApplication application) { }
        public void RunApplication(IApplication application) { }
        public void StopApplication(IApplication application) { }
        public void AttachControl(IControl control) { }
        public void DetachControl(IControl control) { }
        public void SynchronizeControl(IControl control) { }
        public void DrawControl(IControl control) { }
        public void DrawLabel(IControl label) { }
        public void DrawPixel(IControl pixel) { }
        public void DrawPanel(IGroupControl panel) { }
        public void RedrawPanel(IGroupControl panel, Rectangle dimension) { }
        public void DrawMenuItem(IControl menuItem) { }
        public void DrawMenuBar(IGroupControl menuBar) { }
        public void DrawListBox(IControl listBox) { }
        public void DrawScrollBar(IControl scrollBar) { }
        public void DrawTextBox(IControl textBox) { }
        public void DrawTerminal(IControl terminal) { }
        public void RedrawTerminal(IControl terminal, Rectangle dimension) { }
        public void SetTitle(string value) { }
    }
}
