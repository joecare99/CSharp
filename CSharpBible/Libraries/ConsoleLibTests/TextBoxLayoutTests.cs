using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class TextBoxLayoutTests
{
    [TestMethod]
    public void TextBox_ReportsCaretInTerminalCells()
    {
        var textBox = new TextBox { Text = "A\u4e2dB", Caret = (3, 0) };

        Assert.AreEqual(4, textBox.GetCaretCellColumn());
    }
}
