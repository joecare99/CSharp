using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Desktop;
using ConsoleLib.Showcase.Desktop.Capabilities;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.Calendar;
using CommonDialogs.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleButton = ConsoleLib.CommonControls.Button;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class DesktopShellTests
{
    [TestMethod]
    public void CalendarModuleRegistersAndLoadsAsAnIndependentPage()
    {
        var context = new ShowcaseAppRegistrationContext(new EmptyServiceProvider());
        new CalendarAppModule().Register(context);

        var descriptor = context.Apps["Calendar"];
        var model = descriptor.CreateViewModel(new EmptyServiceProvider());
        var page = descriptor.LoadPage(new EmptyServiceProvider(), model);

        Assert.AreEqual(ShowcaseAppCategory.Productivity, descriptor.Category);
        Assert.IsInstanceOfType(page.Root, typeof(Page));
        Assert.IsTrue(page.NamedControls.ContainsKey("Next"));
    }

    [TestMethod]
    public async Task UnavailableFileDialogCapabilityNeverReportsSuccessfulSelection()
    {
        var service = new UnavailableShowcaseFileDialogService("Unavailable in test.");

        var result = await service.OpenFileAsync(new FileDialogRequest("Open file"));

        Assert.IsFalse(result.IsAvailable);
        Assert.IsFalse(result.IsAccepted);
        Assert.AreEqual("Unavailable in test.", result.Message);
        Assert.AreEqual(FileDialogStatus.Unavailable, result.Status);
    }

    [TestMethod]
    public async Task WindowsFileDialogAdapterReportsAcceptedSelection()
    {
        var dialog = new FakeFileDialog { Accepted = true, FileName = @"C:\picked.txt" };
        var service = new WindowsShowcaseFileDialogService(
            () => new FakeOpenFileDialog(dialog),
            () => dialog,
            () => dialog);

        var result = await service.OpenFileAsync(new FileDialogRequest("Open", filter: "Text|*.txt"));

        Assert.AreEqual(FileDialogStatus.Accepted, result.Status);
        Assert.AreEqual(@"C:\picked.txt", result.Path);
        Assert.AreEqual("Open", dialog.Title);
        Assert.AreEqual("Text|*.txt", dialog.Filter);
    }

    [TestMethod]
    public async Task WindowsFileDialogAdapterReportsCancellation()
    {
        var dialog = new FakeFileDialog();
        var service = new WindowsShowcaseFileDialogService(
            () => new FakeOpenFileDialog(dialog),
            () => dialog,
            () => dialog);

        var result = await service.SaveFileAsync(new FileDialogRequest("Save"));

        Assert.AreEqual(FileDialogStatus.Cancelled, result.Status);
        Assert.IsFalse(result.IsAccepted);
        Assert.IsNull(result.Path);
    }

    [TestMethod]
    public async Task WindowsFileDialogAdapterReportsDialogFailure()
    {
        var service = new WindowsShowcaseFileDialogService(
            () => throw new InvalidOperationException("native dialog failed"),
            () => new FakeFileDialog(),
            () => new FakeFileDialog());

        var result = await service.OpenFileAsync(new FileDialogRequest("Open"));

        Assert.AreEqual(FileDialogStatus.Failed, result.Status);
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("native dialog failed", result.Message);
    }

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
    public void AboutButtonOpensVisibleModalDialogAndCloseRemovesIt()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());
        var about = FindControls(shell).OfType<ConsoleButton>().Single(button => button.Text == "About");

        for (var iteration = 0; iteration < 3; iteration++)
        {
            about.Click();

            var session = shell.DialogManager.Sessions.Single();
            Assert.IsTrue(session.IsModal);
            Assert.IsTrue(session.Dialog.Visible);
            Assert.AreSame(shell, session.Dialog.Parent);

            var close = session.Dialog.Children.OfType<ConsoleButton>().Single(button => button.Text == "Close");
            close.Click();

            Assert.AreEqual(0, shell.DialogManager.Sessions.Count);
            Assert.IsNull(session.Dialog.Parent);
            Assert.IsFalse(session.Dialog.Visible);
        }
    }

    [TestMethod]
    public void DefaultCompositionIncludesIndependentGalleryAndGameModules()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());

        foreach (var appId in new[] { "Gallery", "TicTacToe", "Memory" })
        {
            Assert.IsTrue(shell.WindowManager.Open(appId), $"Expected default module '{appId}' to be registered.");
            Assert.AreEqual(appId, shell.WindowManager.Active!.Id);
            Assert.IsTrue(shell.Children.OfType<DesktopWindowView>().Any(view => view.Visible));
            Assert.IsTrue(shell.WindowManager.Close(appId));
        }
    }

    [TestMethod]
    public void DefaultCompositionRegistersAllExtractedApplications()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());

        foreach (var appId in new[] { "Calendar", "Calculator", "Notepad", "Characters", "Clock", "Terminal" })
        {
            Assert.IsTrue(shell.WindowManager.Open(appId), $"Expected extracted module '{appId}' to be registered.");
            Assert.AreEqual(appId, shell.WindowManager.Active!.Id);
            Assert.IsTrue(shell.WindowManager.Close(appId));
        }
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
        Assert.AreEqual(new Point(1, 24), shell.Children.OfType<ConsoleLib.CommonControls.Label>().Single(label => label.Text.Contains("Ready")).Position);
    }

    [TestMethod]
    public void ResizingBelowMinimumShowsResizeMessage()
    {
        using var shell = new DesktopShell(new WidgetSetStub(), new DesktopViewModel());

        shell.RaiseResizeEvent(new Point(40, 12));

        StringAssert.Contains(shell.Children.OfType<ConsoleLib.CommonControls.Label>().Single(label => label.Position.Y == 11).Text, "Resize console");
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

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private static IEnumerable<IControl> FindControls(IControl control)
    {
        yield return control;
        foreach (var child in control.Children)
        foreach (var nested in FindControls(child))
            yield return nested;
    }

    private sealed class FakeOpenFileDialog : IOpenFileDialog
    {
        private readonly FakeFileDialog _inner;

        public FakeOpenFileDialog(FakeFileDialog inner) => _inner = inner;
        public string FileName { get => _inner.FileName; set => _inner.FileName = value; }
        public string Filter { get => _inner.Filter; set => _inner.Filter = value; }
        public int FilterIndex { get => _inner.FilterIndex; set => _inner.FilterIndex = value; }
        public string InitialDirectory { get => _inner.InitialDirectory; set => _inner.InitialDirectory = value; }
        public bool RestoreDirectory { get => _inner.RestoreDirectory; set => _inner.RestoreDirectory = value; }
        public bool AddExtension { get => _inner.AddExtension; set => _inner.AddExtension = value; }
        public bool CheckFileExists { get => _inner.CheckFileExists; set => _inner.CheckFileExists = value; }
        public string Title { get => _inner.Title; set => _inner.Title = value; }
        public string DefaultExt { get => _inner.DefaultExt; set => _inner.DefaultExt = value; }
        public bool Multiselect { get; set; }
        public string[] FileNames => new[] { FileName };
        public string SafeFileName => FileName;
        public string[] SafeFileNames => FileNames;
        public string FileNameExtension => DefaultExt;
        public bool? ShowDialog() => _inner.ShowDialog();
        public bool? ShowDialog(object owner) => _inner.ShowDialog(owner);
    }

    private sealed class FakeFileDialog : IFileDialog
    {
        public bool Accepted { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
        public int FilterIndex { get; set; }
        public string InitialDirectory { get; set; } = string.Empty;
        public bool RestoreDirectory { get; set; }
        public bool AddExtension { get; set; }
        public bool CheckFileExists { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DefaultExt { get; set; } = string.Empty;
        public bool? ShowDialog() => Accepted;
        public bool? ShowDialog(object owner) => Accepted;
    }

    private sealed class FakeHostCapabilities : IShowcaseHostCapabilities
    {
        public FakeHostCapabilities(ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseTerminalCapability? terminal) => Terminal = terminal;

        public IClipboardService? Clipboard => null;
        public bool SupportsMouse => true;
        public bool SupportsTerminal => true;
        public ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseTerminalCapability? Terminal { get; }
        public ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseAlertService Alert => new FakeAlertService();
        public IShowcaseFileDialogService FileDialogs => new UnavailableShowcaseFileDialogService();
    }

    private sealed class FakeAlertService : ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseAlertService
    {
        public bool IsAvailable => true;

        public void Alert() { }
    }

    private sealed class FakeTerminalCapability : ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseTerminalCapability
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
