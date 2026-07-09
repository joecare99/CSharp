using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terminal.Core;

namespace Terminal.Core.Tests;

[TestClass]
public class TerminalAnsiParserTests
{
    [TestMethod]
    public void Parse_ShouldApplyPlainTextAndLineFeed()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 3));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("A\nB");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual('B', snapshot.Lines[1][1].Character);
    }

    [TestMethod]
    public void Parse_ShouldApplyCursorPositionSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 4));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("\u001b[2;3HX");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('X', snapshot.Lines[1][2].Character);
    }

    [TestMethod]
    public void Parse_ShouldApplyHorizontalCursorPositionSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 4));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("A\n\u001b[5GZ");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual('Z', snapshot.Lines[1][4].Character);
    }

    [TestMethod]
    public void Parse_ShouldApplyCursorForwardSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 4));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("A\u001b[2CB");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual('B', snapshot.Lines[0][3].Character);
    }

    [TestMethod]
    public void Parse_ShouldApplyEraseCharacterSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 4));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("ABCDE\u001b[2G\u001b[2X");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual(' ', snapshot.Lines[0][1].Character);
        Assert.AreEqual(' ', snapshot.Lines[0][2].Character);
        Assert.AreEqual('D', snapshot.Lines[0][3].Character);
    }

    [TestMethod]
    public void Parse_ShouldApplyColorSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 2));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("\u001b[31mR");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('R', snapshot.Lines[0][0].Character);
        Assert.AreEqual(TerminalPalette.GetColor(31), snapshot.Lines[0][0].Foreground);
    }

    [TestMethod]
    public void Parse_ShouldApplyExtendedForegroundColorSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 2));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("\u001b[38;5;214mX");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('X', snapshot.Lines[0][0].Character);
        Assert.AreEqual(TerminalPalette.GetExtendedColor(214), snapshot.Lines[0][0].Foreground);
    }

    [TestMethod]
    public void Parse_ShouldApplyRgbForegroundAndBackgroundColorSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(10, 2));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("\u001b[38;2;1;2;3;48;2;4;5;6mX");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('X', snapshot.Lines[0][0].Character);
        Assert.AreEqual(new TerminalColor(1, 2, 3), snapshot.Lines[0][0].Foreground);
        Assert.AreEqual(new TerminalColor(4, 5, 6), snapshot.Lines[0][0].Background);
    }

    [TestMethod]
    public void Parse_ShouldToggleCursorVisibility()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));
        var parser = new TerminalAnsiParser(buffer);

        parser.Parse("\u001b[?25l");

        Assert.IsFalse(buffer.Cursor.IsVisible);
    }

    [TestMethod]
    public void Parse_ShouldReportMouseTrackingModeAndProtocolChanges()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));
        var reportedMode = TerminalMouseTrackingMode.None;
        var reportedProtocol = TerminalMouseProtocol.None;
        var parser = new TerminalAnsiParser(buffer, null, (mode, protocol) =>
        {
            reportedMode = mode;
            reportedProtocol = protocol;
        });

        parser.Parse("\u001b[?1002h\u001b[?1006h");

        Assert.AreEqual(TerminalMouseTrackingMode.Drag, reportedMode);
        Assert.AreEqual(TerminalMouseProtocol.Sgr, reportedProtocol);
    }

    [TestMethod]
    public void Parse_ShouldReportMouseTrackingModeAndProtocolChangesFromCombinedPrivateModeSequence()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));
        var reportedMode = TerminalMouseTrackingMode.None;
        var reportedProtocol = TerminalMouseProtocol.None;
        var parser = new TerminalAnsiParser(buffer, null, (mode, protocol) =>
        {
            reportedMode = mode;
            reportedProtocol = protocol;
        });

        parser.Parse("\u001b[?1002;1006h");

        Assert.AreEqual(TerminalMouseTrackingMode.Drag, reportedMode);
        Assert.AreEqual(TerminalMouseProtocol.Sgr, reportedProtocol);
    }

    [TestMethod]
    public void Parse_ShouldDisableMouseTrackingWhenModeIsReset()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));
        var reportedMode = TerminalMouseTrackingMode.None;
        var reportedProtocol = TerminalMouseProtocol.None;
        var parser = new TerminalAnsiParser(buffer, null, (mode, protocol) =>
        {
            reportedMode = mode;
            reportedProtocol = protocol;
        });

        parser.Parse("\u001b[?1000h\u001b[?1006h\u001b[?1000l");

        Assert.AreEqual(TerminalMouseTrackingMode.None, reportedMode);
        Assert.AreEqual(TerminalMouseProtocol.None, reportedProtocol);
    }

    [TestMethod]
    public void Parse_ShouldDisableMouseTrackingWhenCombinedModeIsReset()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));
        var reportedMode = TerminalMouseTrackingMode.None;
        var reportedProtocol = TerminalMouseProtocol.None;
        var parser = new TerminalAnsiParser(buffer, null, (mode, protocol) =>
        {
            reportedMode = mode;
            reportedProtocol = protocol;
        });

        parser.Parse("\u001b[?1000;1006h\u001b[?1000;1006l");

        Assert.AreEqual(TerminalMouseTrackingMode.None, reportedMode);
        Assert.AreEqual(TerminalMouseProtocol.None, reportedProtocol);
    }

    [TestMethod]
    public void Parse_ShouldApplyWindowTitleSequenceTerminatedByBell()
    {
        var buffer = new TerminalBuffer(new TerminalSize(20, 2));
        string? capturedTitle = null;
        var parser = new TerminalAnsiParser(buffer, title => capturedTitle = title);

        parser.Parse("\u001b]0;Window Title\aX");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual("Window Title", capturedTitle);
        Assert.AreEqual('X', snapshot.Lines[0][0].Character);
    }

    [TestMethod]
    public void Parse_ShouldApplyWindowTitleSequenceTerminatedByStringTerminator()
    {
        var buffer = new TerminalBuffer(new TerminalSize(20, 2));
        string? capturedTitle = null;
        var parser = new TerminalAnsiParser(buffer, title => capturedTitle = title);

        parser.Parse("\u001b]2;Window Title\u001b\\X");

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual("Window Title", capturedTitle);
        Assert.AreEqual('X', snapshot.Lines[0][0].Character);
    }
}
