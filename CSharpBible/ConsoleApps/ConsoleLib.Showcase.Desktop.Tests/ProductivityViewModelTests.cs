using System;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using ConsoleLib.Showcase.Desktop.ViewModels;
using ConsoleLib.Showcase.Desktop.Capabilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class ProductivityViewModelTests
{
    [TestMethod]
    public void CalendarCommandsNavigateMonths()
    {
        var calendar = new CalendarViewModel(new DateTime(2026, 1, 15));

        calendar.NextCommand.Execute(null);
        Assert.AreEqual(new DateTime(2026, 2, 1).ToString("MMMM yyyy"), calendar.MonthTitle);

        calendar.PreviousCommand.Execute(null);
        Assert.AreEqual(new DateTime(2026, 1, 1).ToString("MMMM yyyy"), calendar.MonthTitle);
    }

    [TestMethod]
    public void CalendarDaysTextContainsEveryWeekOnItsOwnLine()
    {
        var calendar = new CalendarViewModel(new DateTime(2026, 8, 15));

        var lines = calendar.DaysText.Split(Environment.NewLine);

        Assert.AreEqual(6, lines.Length);
        Assert.IsTrue(lines.All(line => line.Length == 20));
        StringAssert.Contains(lines[^1], "31");
    }

    [TestMethod]
    public void CalculatorCommandsUpdateDisplay()
    {
        var calculator = new CalculatorViewModel();

        calculator.SevenCommand.Execute(null);
        calculator.AddCommand.Execute(null);
        calculator.NineCommand.Execute(null);
        calculator.EqualsCommand.Execute(null);

        Assert.AreEqual("16", calculator.Display);
    }

    [TestMethod]
    public void CalculatorSupportsCompleteKeypadOperations()
    {
        var calculator = new CalculatorViewModel();

        calculator.OneCommand.Execute(null);
        calculator.DecimalCommand.Execute(null);
        calculator.FiveCommand.Execute(null);
        calculator.MultiplyCommand.Execute(null);
        calculator.TwoCommand.Execute(null);
        calculator.EqualsCommand.Execute(null);

        Assert.AreEqual("3", calculator.Display);
    }

    [TestMethod]
    public void NotepadReportsClipboardAvailability()
    {
        Assert.AreEqual("Clipboard unavailable on this host.", new NotepadViewModel().ClipboardStatus);
        Assert.AreEqual("Clipboard ready.", new NotepadViewModel(new FakeClipboard()).ClipboardStatus);
    }

    [TestMethod]
    public async Task NotepadUsesInjectedClipboard()
    {
        var clipboard = new FakeClipboard { PasteValue = " pasted" };
        var notepad = new NotepadViewModel(clipboard) { Document = "text" };

        await notepad.CopyCommand.ExecuteAsync(null);
        await notepad.PasteCommand.ExecuteAsync(null);

        Assert.AreEqual("text pasted", notepad.Document);
        Assert.AreEqual("text", clipboard.CopiedValue);
        Assert.AreEqual("11 characters, 1 lines", notepad.Statistics);
    }

    [TestMethod]
    public async Task CharacterTableNavigatesAndCopiesSelection()
    {
        var clipboard = new FakeClipboard();
        var characters = new CharactersViewModel(clipboard);

        characters.NextCommand.Execute(null);
        Assert.AreEqual("Selected: ! U+0021", characters.Preview);

        await characters.CopyCommand.ExecuteAsync(null);
        Assert.AreEqual("!", clipboard.CopiedValue);
    }

    [TestMethod]
    public void CharacterTableProvidesEightRows()
    {
        var characters = new CharactersViewModel();

        Assert.AreEqual(8, characters.TableText.Split(Environment.NewLine).Length);
        Assert.IsTrue(characters.TableText.Contains(" !\"#$%&'()*+,-./0123456789"));
    }

    [TestMethod]
    public void ClockCommandsToggleAlarm()
    {
        var clock = new ClockViewModel(new DateTime(2026, 9, 1, 10, 15, 0));

        Assert.AreEqual("10:15:00", clock.TimeText);
        var faceLines = clock.Face.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        Assert.AreEqual(9, faceLines.Length);
        Assert.IsTrue(faceLines.All(line => line.Length == 32));
        StringAssert.Contains(faceLines[2], "10:15:00");
        StringAssert.Contains(faceLines[8], "H:");
        Assert.AreEqual("Alarm off", clock.AlarmText);
        clock.SetAlarmCommand.Execute(null);
        Assert.AreEqual("Alarm armed", clock.AlarmText);
        clock.StopAlarmCommand.Execute(null);
        Assert.AreEqual("Alarm off", clock.AlarmText);
    }

    [TestMethod]
    public async Task TerminalReflectsHostCapability()
    {
        var terminal = new TerminalViewModel(new FakeTerminalCapability());

        Assert.AreEqual("Terminal capability available", terminal.Status);
        await terminal.StartCommand.ExecuteAsync(null);
        Assert.AreEqual("Terminal session running.", terminal.Status);
    }

    [TestMethod]
    public void TerminalReportsHostBoundary()
    {
        var terminal = new TerminalViewModel();

        terminal.StartCommand.Execute(null);

        StringAssert.Contains(terminal.Status, "host-provided");
    }

    private sealed class FakeClipboard : IClipboardService
    {
        public string? CopiedValue { get; private set; }
        public string? PasteValue { get; init; }

        public Task<bool> CopyAsync(string text, CancellationToken cancellationToken = default)
        {
            CopiedValue = text;
            return Task.FromResult(true);
        }

        public Task<string?> PasteAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(PasteValue);
    }

    private sealed class FakeTerminalCapability : IShowcaseTerminalCapability
    {
        public bool IsAvailable => true;
        public bool IsRunning { get; private set; }
        public event EventHandler<string>? OutputChanged;

        public Task StartAsync(Size size, CancellationToken cancellationToken = default)
        {
            IsRunning = true;
            OutputChanged?.Invoke(this, "terminal started");
            return Task.CompletedTask;
        }

        public Task SendInputAsync(string input, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task ResizeAsync(Size size, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            IsRunning = false;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
