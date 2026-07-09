using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terminal.Core;

namespace Terminal.Core.Tests;

[TestClass]
public class TerminalBufferTests
{
    [TestMethod]
    public void Write_ShouldStoreCharacterAtCursorPosition()
    {
        var buffer = new TerminalBuffer(new TerminalSize(4, 2));

        buffer.Write('A', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual(1, snapshot.Cursor.Column);
        Assert.AreEqual(0, snapshot.Cursor.Row);
    }

    [TestMethod]
    public void LineFeedAtViewportBottom_ShouldScrollBuffer()
    {
        var buffer = new TerminalBuffer(new TerminalSize(3, 2));

        buffer.Write('A', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.CarriageReturn();
        buffer.LineFeed();
        buffer.Write('B', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.CarriageReturn();
        buffer.LineFeed();
        buffer.Write('C', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('B', snapshot.Lines[0][0].Character);
        Assert.AreEqual('C', snapshot.Lines[1][0].Character);
    }

    [TestMethod]
    public void Resize_ShouldPreserveExistingViewportContent()
    {
        var buffer = new TerminalBuffer(new TerminalSize(2, 2));
        buffer.Write('X', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.Write('Y', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);

        buffer.Resize(new TerminalSize(4, 3));

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('X', snapshot.Lines[0][0].Character);
        Assert.AreEqual('Y', snapshot.Lines[0][1].Character);
        Assert.AreEqual(4, snapshot.Size.Columns);
        Assert.AreEqual(3, snapshot.Size.Rows);
    }

    [TestMethod]
    public void MoveCursorForward_ShouldAdvanceCursorWithoutChangingContent()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));

        buffer.Write('A', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.MoveCursorForward(2);
        buffer.Write('B', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual('B', snapshot.Lines[0][3].Character);
    }

    [TestMethod]
    public void EraseCharacters_ShouldBlankCharactersAtCursorPosition()
    {
        var buffer = new TerminalBuffer(new TerminalSize(5, 2));

        buffer.Write('A', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.Write('B', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.Write('C', TerminalColor.DefaultForeground, TerminalColor.DefaultBackground);
        buffer.SetCursorPosition(1, 0);
        buffer.EraseCharacters(2);

        var snapshot = buffer.CreateSnapshot();
        Assert.AreEqual('A', snapshot.Lines[0][0].Character);
        Assert.AreEqual(' ', snapshot.Lines[0][1].Character);
        Assert.AreEqual(' ', snapshot.Lines[0][2].Character);
        Assert.AreEqual(1, snapshot.Cursor.Column);
    }
}
