using ConsoleLib; // for Control base
using ConsoleLib.CommonControls;
using ConsoleLib.CommonControls.Tests; // for ConsoleColor
using ConsoleLib.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Linq;

namespace ConsoleLibTests.CommonControls;

[TestClass]
public class PanelTests : TestBase
{
    private class TestChild : Control
    {
        public int DrawCount; public int ReDrawCount;
        public override void Draw() { DrawCount++; base.Draw(); }
        public override void ReDraw(Rectangle dimension) { ReDrawCount++; base.ReDraw(dimension); }
    }

    [TestMethod]
    [TestCategory("Panel")]
    [DataRow(BorderStyle.Single, '┌')]
    [DataRow(BorderStyle.None, '░')]
    [DataRow(BorderStyle.Raised, '▛')]
    public void Draw_Renders_Border_And_Children_With_Shadow(BorderStyle borderStyle, char expectedCorner)
    {
        var p = new Panel { Dimension = new Rectangle(0, 0, 20, 5) };
        p.Parent = _TestApp;
        p.BorderColor = ConsoleColor.Yellow;
        p.BorderStyle = borderStyle;
        var c1 = new TestChild { Dimension = new Rectangle(1, 1, 5, 1), Parent = p };
        var c2 = new TestChild { Dimension = new Rectangle(2, 2, 6, 2), Parent = p, Shadow = true };
        p.Draw();
        Assert.IsTrue(p.Valid);
        Assert.AreEqual(1, c1.DrawCount);
        Assert.AreEqual(1, c2.DrawCount);
        Assert.AreEqual(expectedCorner, __tstCon?.Content[4]);

    }


    [TestMethod]
    public void BringToFront_Reorders_Children()
    {
        var p = new Panel();
        var a = new TestChild { Parent = p };
        var b = new TestChild { Parent = p };
        var c = new TestChild { Parent = p };
        // initial order (added -> inserted at end due to SetParent logic): a,b,c
        CollectionAssert.AreEqual(new[] { a, b, c }, p.Children.Cast<Control>().ToArray());
        p.BringToFront(b);
        CollectionAssert.AreEqual(new[] { b, a, c }, p.Children.Cast<Control>().ToArray());
        // calling with control not in list should do nothing
        var foreign = new TestChild();
        p.BringToFront(foreign);
        CollectionAssert.AreEqual(new[] { b, a, c }, p.Children.Cast<Control>().ToArray());
        // calling with null should do nothing
        p.BringToFront(null!);
        CollectionAssert.AreEqual(new[] { b, a, c }, p.Children.Cast<Control>().ToArray());
    }

    [TestMethod]
    public void ReDraw_FullRegion_Draws_Border()
    {
        var p = new Panel { Dimension = new Rectangle(0, 0, 15, 4) };
        p.Parent = _TestApp;
        p.BorderStyle = BorderStyle.Single;
        var c1 = new TestChild { Dimension = new Rectangle(1, 1, 5, 1), Parent = p };
        var c2 = new TestChild { Dimension = new Rectangle(2, 2, 6, 2), Parent = p, Shadow = true };
        p.ReDraw(p.Dimension); // region intersects and includes border
        Assert.IsTrue(p.Valid); // becomes valid after redraw
    }

    [TestMethod]
    public void ReDraw_InnerRegion_No_Border()
    {
        var p = new Panel { Dimension = new Rectangle(0, 0, 15, 5) };
        p.Parent = _TestApp;
        p.Border = new[] { '─', '│', '┌', '┐', '└', '┘' };
        // inner region completely inside (border skip path)
        var inner = new Rectangle(3, 2, 5, 1);
        p.ReDraw(inner);
        Assert.IsTrue(p.Valid); // still valid
    }

    [TestMethod]
    public void ReDraw_Empty_Returns_Immediately()
    {
        var p = new Panel { Dimension = new Rectangle(0, 0, 10, 3) };
        p.Valid = false;
        p.ReDraw(Rectangle.Empty); // should early return and not set Valid
        Assert.IsFalse(p.Valid);
    }
}
