using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;
using NSubstitute;
using BaseLib.Helper;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ApplicationTests : TestBase
{
    private class FakeExt : IExtendedConsole
    {
        public event EventHandler<IMouseEvent>? MouseEvent;
        public event EventHandler<IKeyEvent>? KeyEvent;
        public event EventHandler<Point>? WindowBufferSizeEvent;
        public void RaiseMouse(IMouseEvent e)=>MouseEvent?.Invoke(this,e);
        public void RaiseKey(IKeyEvent e)=>KeyEvent?.Invoke(this,e);
        public void RaiseResize(Point p)=>WindowBufferSizeEvent?.Invoke(this,p);
        public void Stop() { }
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

    private class KeyEvt : IKeyEvent
    {
        public bool bKeyDown { get; set; }
        public char KeyChar { get; set; }
        public ushort usKeyCode { get; set; }
        public ushort usScanCode { get; set; }
        public uint dwControlKeyState { get; set; }
        public bool Handled { get; set; }
    }

    [TestMethod]
    public void Dispatch_Executes_Action_Via_MessageQueue()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext));
        bool ran=false;
        app.Dispatch(()=>ran=true);

        // per Reflection private HandleMessages abrufen
        app.ProcessPendingMessages();
        Assert.IsTrue(ran);
    }

    [TestMethod]
    [DataRow(2,1,'X')]
    [DataRow(3,1,'Y')]
    public void Mouse_Event_And_Key_Event_Routed(int mx,int my,char accel)
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext)) { Dimension=new Rectangle(0,0,40,10)};
        var btn = new Button{ Dimension=new Rectangle(1,1,8,1), Parent=app};
        bool clicked=false; btn.OnClick += (_,_)=>clicked=true;
        ext.RaiseMouse(new MouseEvt{ ButtonEvent=true, MousePos=new Point(mx,my), MouseButtonLeft=true});
        Assert.IsTrue(clicked);
        btn.Accelerator=accel;
        bool kClicked=false; btn.OnClick += (_,_)=>kClicked=true;
        ext.RaiseKey(new KeyEvt{ bKeyDown=true, KeyChar=accel});
        Assert.IsTrue(kClicked);
    }

    [TestMethod]
    public void Resize_Event_Invalidates_And_Raises_OnCanvasResize()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext));
        int cnt=0;
        app.OnCanvasResize += (_,_)=>cnt++;
        ext.RaiseResize(new Point(120,30));
        Assert.AreEqual(1,cnt);
    }

    [TestMethod]
    public void RaiseMouseEvent_Routes_Click_Without_ExtendedConsole_Wiring_In_Control()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext)) { Dimension = new Rectangle(0, 0, 20, 5) };
        var btn = new Button { Dimension = new Rectangle(1, 1, 5, 1), Parent = app };
        bool clicked = false;
        btn.OnClick += (_, _) => clicked = true;

        app.RaiseMouseEvent(new MouseEvt { ButtonEvent = true, MousePos = new Point(2, 1), MouseButtonLeft = true });

        Assert.IsTrue(clicked);
    }
}