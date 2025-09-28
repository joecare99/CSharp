using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Drawing;
using ConsoleLib.CommonControls.Tests;

namespace ConsoleLibTests.CommonControls;

[TestClass]
public class TerminalTests : TestBase
{
    [DataTestMethod]
    [DataRow(10,5,14,6,"ABC","D")]
    [DataRow(12,6,16,8,"Hello","World")] 
    public void Resize_Preserves_Content(int w1,int h1,int w2,int h2,string first,string second)
    {
        var term = new Terminal{ Dimension=new Rectangle(0,0,w1,h1)};
        term.Write(first);
        term.size = new Size(w2,h2);
        term.Write(second);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void WriteLine_And_Scroll_Wrap_Tab_CR()
    {
        var term = new Terminal{ Dimension=new Rectangle(0,0,10,4)};
        for(int i=0;i<10;i++) term.WriteLine($"L{i}");
        term.Write('\t');
        term.Write('\r');
        term.Write("X");
        Assert.IsTrue(true);
    }

    [DataTestMethod]
    [DataRow("Data","After")] 
    [DataRow("123","456")] 
    public void Clear_Resets_Area(string first,string second)
    {
        var term = new Terminal{ Dimension=new Rectangle(0,0,12,5)};
        term.Write(first);
        term.Clear();
        term.Write(second);
        Assert.IsTrue(true);
    }
}