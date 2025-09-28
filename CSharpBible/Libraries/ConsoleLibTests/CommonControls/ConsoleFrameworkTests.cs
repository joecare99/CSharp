using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib;
using ConsoleLib.Interfaces;
using System.Drawing;
using System;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ConsoleFrameworkTests: TestBase
{
    private class FakeExt : IExtendedConsole
    {
        public event EventHandler<IMouseEvent>? MouseEvent;
        public event EventHandler<IKeyEvent>? KeyEvent;
        public event EventHandler<Point>? WindowBufferSizeEvent;
        public void Stop() { }
        public void RaiseMouse(IMouseEvent e)=>MouseEvent?.Invoke(this,e);
        public void RaiseResize(Point p)=>WindowBufferSizeEvent?.Invoke(this,p);
    }
    private class MouseEvt : IMouseEvent
    {
        public bool MouseMoved { get; set; }
        public bool ButtonEvent { get; set; }
        public Point MousePos { get; set; }
        public bool MouseButtonLeft { get; set; }
        public bool MouseButtonRight { get; set; }
        public bool MouseButtonMiddle { get; set; }
        public int MouseWheel { get; set; }
        public bool Handled { get; set; }
    }

    [TestMethod]
    public void ExtendedConsole_Hooks_Events_And_Resizes_Canvas()
    {
        var ext = new FakeExt();
        ConsoleFramework.ExtendedConsole = ext;
        ext.RaiseMouse(new MouseEvt{ MousePos=new Point(7,3)});
        Assert.AreEqual(new Point(7,3), ConsoleFramework.MousePos);
        var canvas = ConsoleFramework.Canvas;
        var field = canvas.GetType().GetField("_dimension", System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
        ext.RaiseResize(new Point(90,25));
        var rect = (Rectangle)field!.GetValue(canvas)!;
        Assert.AreEqual(90, rect.Width);
        Assert.AreEqual(25, rect.Height);
    }
}
