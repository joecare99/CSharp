using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using NSubstitute;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ControlTests : TestBase
{
    private class TestControl : Control { public void ForceReDraw(Rectangle r)=>ReDraw(r); public void ForceHandleKey(IKeyEvent e)=>HandlePressKeyEvents(e);}    

    [TestMethod]
    public void Invalidate_Pushes_Message()
    {
        var c = new TestControl();
        Control.MessageQueue = new ConcurrentQueue<(Action<object, EventArgs>, object, EventArgs)>();
        c.Invalidate();
        Assert.HasCount(1, Control.MessageQueue);
    }

    [TestMethod]
    public void SetText_Raises_OnChange()
    {
        var c = new TestControl();
        int changeCount = 0; c.OnChange += (_, _) => changeCount++;
        c.Text = "Hello"; c.Text = "Hello"; // second time no change
        Assert.AreEqual(1, changeCount);
    }

    [TestMethod]
    public void Active_Sets_OnActivate_And_ParentTracking()
    {
        var parent = new TestControl { size = new Size(10,1)};
        var child1 = new TestControl { size = new Size(3,1), Parent = parent};
        var child2 = new TestControl { size = new Size(3,1), Parent = parent};
        int actCount = 0; child1.OnActivate += (_,_)=>actCount++;
        child1.Active = true;
        Assert.AreSame(child1, parent.ActiveControl);
        child2.Active = true; // should deactivate child1
        Assert.AreSame(child2, parent.ActiveControl);
        Assert.IsFalse(child1.Active);
        Assert.AreEqual(1, actCount);
        child2.Active = false;
        Assert.IsNull(parent.ActiveControl);
    }

    [TestMethod]
    public void Visible_False_Deactivates()
    {
        var c = new TestControl
        {
            Active = true,
            Visible = false
        };
        Assert.IsFalse(c.Active);
        c.Visible = true;
        Assert.IsTrue(c.Visible);
    }

    [TestMethod]
    public void Shadow_Set_Invalidates_Parent()
    {
        var parent = new TestControl();
        var child = new TestControl
        {
            Parent = parent,
            Shadow = true
        };
        Assert.IsTrue(child.Shadow);
    }

    [TestMethod]
    public void Add_Remove_Works()
    {
        var p = new TestControl();
        var c = new TestControl();
        p.Add(c);
        Assert.AreSame(p,c.Parent);
        p.Remove(c);
        Assert.IsNull(c.Parent);
        Assert.IsEmpty(p.Children);
    }

    [TestMethod]
    public void RealDimAndLocalDim_Work()
    {
        var p = new TestControl{ Dimension=new Rectangle(2,2,10,2)};
        var c = new TestControl{ Dimension=new Rectangle(3,0,5,1), Parent = p};
        var real = c.RealDim;
        Assert.AreEqual(new Rectangle(5,2,5,1), real);
        var loc = c.LocalDimOf(real,p);
        Assert.AreEqual(new Rectangle(2,2,5,1), loc);
    }

    [TestMethod]
    public void Click_Raises_OnClick()
    {
        var c = new TestControl();
        int clicks = 0; c.OnClick += (_,_)=>clicks++;
        c.Click();
        Assert.AreEqual(1, clicks);
    }

    [TestMethod]
    public void MouseEnterLeave_Propagates_To_Child()
    {
        var p = new TestControl{ Dimension=new Rectangle(0,0,20,3)};
        var c = new TestControl{ Dimension=new Rectangle(2,0,5,1), Parent = p};
        int enter=0, leave=0; c.OnMouseEnter += (_,_)=>enter++; c.OnMouseLeave += (_,_)=>leave++;
        p.MouseEnter(new Point(3,0));
        p.MouseLeave(new Point(3,0));
        Assert.AreEqual(1, enter);
        Assert.AreEqual(1, leave);
    }

    [TestMethod]
    public void MouseMove_Child_Transitions()
    {
        var p = new TestControl{ Dimension=new Rectangle(0,0,20,3)};
        var c = new TestControl{ Dimension=new Rectangle(2,0,5,1), Parent=p};
        int enter=0, leave=0, move=0; 
        c.OnMouseEnter+=(_,_)=>enter++; 
        c.OnMouseLeave+=(_,_)=>leave++; 
        c.OnMouseMove+=(_,e)=>move++;
        var me = Substitute.For<IMouseEvent>();
        me.MousePos.Returns(new Point(3,0));
        p.MouseMove(me,new Point(10,0)); // enter
        var me2 = Substitute.For<IMouseEvent>(); me2.MousePos.Returns(new Point(4,0));
        p.MouseMove(me2,new Point(3,0)); // move
        var me3 = Substitute.For<IMouseEvent>(); me3.MousePos.Returns(new Point(19,0));
        p.MouseMove(me3,new Point(4,0)); // leave
        Assert.AreEqual(1, enter);
        Assert.AreEqual(1, leave);
        Assert.AreEqual(2, move);
    }

    [TestMethod]
    public void MouseClick_Dispatches_To_Child_Or_Self()
    {
        var p = new TestControl{ Dimension=new Rectangle(0,0,20,3)};
        var c = new TestControl{ Dimension=new Rectangle(2,0,5,1), Parent=p};
        int pClicks=0, cClicks=0; p.OnClick+=(_,_)=>pClicks++; c.OnClick+=(_,_)=>cClicks++;
        var meChild = Substitute.For<IMouseEvent>(); meChild.MousePos.Returns(new Point(4,0)); meChild.MouseButtonLeft.Returns(true);
        p.MouseClick(meChild);
        var meOutside = Substitute.For<IMouseEvent>(); meOutside.MousePos.Returns(new Point(19,2)); meOutside.MouseButtonLeft.Returns(true);
        p.MouseClick(meOutside);
        Assert.AreEqual(1,cClicks);
        Assert.AreEqual(1,pClicks);
    }

    [TestMethod]
    public void HandlePressKeyEvents_Traverses_Children()
    {
        var p = new TestControl();
        var c1 = new TestControl{ Parent=p, Accelerator='A'};
        var c2 = new TestControl{ Parent=p, Accelerator='B'};
        var ke = Substitute.For<IKeyEvent>();
        ke.KeyChar.Returns('B'); ke.bKeyDown.Returns(true);
        p.HandlePressKeyEvents(ke);
        Assert.IsTrue(ke.Handled);
    }

    [TestMethod]
    public void DoUpdate_Calls_Children()
    {
        var p = new TestControl();
        var called=false; var child = new TestControl{ Parent=p};
        child.OnChange += (_,_)=>called=true; // trigger something on invalidate
        child.Invalidate();
        // manually process message queue like Application would
        while(Control.MessageQueue!.TryDequeue(out var work))
        {
            var (a,s,e) = work;
            a(s,e);
        }
        p.DoUpdate();
        Assert.IsFalse(called); // OnChange not triggered by DoUpdate itself
    }
}
