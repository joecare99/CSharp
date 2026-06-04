using System;
using System.Threading.Tasks;
using Avln_TestConsole;
using Avln_TestConsole.Interfaces;
using Avalonia.Headless.MSTest;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avln_TestConsoleTests;

[TestClass]
public class AvaloniaTestConsoleTests
{
    [TestMethod]
    public void Constructor_ShouldWriteInitialBanner()
    {
        var sut = new AvaloniaTestConsole();

        sut.Content.Should().Contain("Testconsole Ver. 1.0");
    }

    [TestMethod]
    public void Write_ShouldExportColorMarkersLikeTestConsole()
    {
        var sut = new AvaloniaTestConsole
        {
            WindowWidth = 20,
            WindowHeight = 3,
        };

        sut.Clear();
        sut.ForegroundColor = ConsoleColor.Red;
        sut.BackgroundColor = ConsoleColor.Blue;
        sut.Write('A');

        sut.Content.Should().StartWith("\\c94A");
    }

    [TestMethod]
    public void Write_ShouldEscapeBackslashesAndTabs()
    {
        var sut = new AvaloniaTestConsole
        {
            WindowWidth = 20,
            WindowHeight = 3,
        };

        sut.Clear();
        sut.Write("\\\t");

        sut.Content.Should().StartWith("\\\\t");
    }

    [TestMethod]
    public void ReadLine_ShouldReturnScriptedLineAndEchoIt()
    {
        var sut = new AvaloniaTestConsole();
        sut.Clear();
        sut.EnqueueLine("hello");

        var result = sut.ReadLine();

        result.Should().Be("hello");
        sut.Content.Should().Contain("hello");
    }

    [TestMethod]
    public void ReadLine_ShouldConsumeQueuedKeys()
    {
        var sut = new AvaloniaTestConsole();
        sut.Clear();
        sut.EnqueueKey(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false));
        sut.EnqueueKey(new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false));
        sut.EnqueueKey(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

        var result = sut.ReadLine();

        result.Should().Be("ab");
        sut.Content.Should().Contain("ab");
    }

    [TestMethod]
    public void ReadLine_ShouldRespectTimeoutWhenNoInputIsAvailable()
    {
        var sut = new AvaloniaTestConsole
        {
            ReadLineTimeout = TimeSpan.FromMilliseconds(20),
        };

        Action action = () => sut.ReadLine();

        action.Should().Throw<TimeoutException>();
    }

    [TestMethod]
    public void Clear_ShouldResetContentToEmptyVisibleState()
    {
        var sut = new AvaloniaTestConsole();
        sut.Write("abc");

        sut.Clear();

        sut.Content.Should().BeEmpty();
    }

    [TestMethod]
    public void WriteLine_WithExplicitColors_ShouldRestorePreviousColorsAfterWrite()
    {
        var sut = new AvaloniaTestConsole
        {
            WindowWidth = 30,
            WindowHeight = 4,
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.Black,
        };

        sut.Clear();

        sut.WriteLine("err", ConsoleColor.White, ConsoleColor.DarkRed);
        sut.Write('x');

        sut.Content.Should().Contain("\\c4Ferr");
        sut.Content.Should().Contain("\\c0Ax");
    }

    [AvaloniaTestMethod]
    public async Task Control_ShouldBeAvailableForAvaloniaHosts()
    {
        IAvaloniaConsole sut = new AvaloniaTestConsole();

        await Task.Yield();

        sut.Control.Should().NotBeNull();
        sut.Control.Buffer.Should().NotBeNull();
    }
}
