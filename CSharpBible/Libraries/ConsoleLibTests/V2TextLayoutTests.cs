using ConsoleLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class V2TextLayoutTests
{
    [TestMethod]
    [DataRow("abc", 3)]
    [DataRow("\u0301", 0)]
    [DataRow("\u4e2d", 2)]
    [DataRow("\r\n", 0)]
    public void UnicodeWidth_MeasuresTerminalCells(string text, int expected)
    {
        Assert.AreEqual(expected, new UnicodeTextLayoutService().GetCellWidth(text));
    }
}
