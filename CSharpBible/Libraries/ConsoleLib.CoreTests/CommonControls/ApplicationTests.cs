using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;
using NSubstitute;
using BaseLib.Helper;
using ConsoleLib.ExtCon;

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
        app.ProcessPendingMessages();
        Assert.IsTrue(clicked);
        btn.Accelerator=accel;
        bool kClicked=false; btn.OnClick += (_,_)=>kClicked=true;
        ext.RaiseKey(new KeyEvt{ bKeyDown=true, KeyChar=accel});
        app.ProcessPendingMessages();
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
        app.ProcessPendingMessages();
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

    [TestMethod]
    public void NativeInput_IsDispatchedBeforeRedraw()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext))
        {
            Dimension = new Rectangle(0, 0, 30, 8)
        };
        var label = new Label
        {
            Parent = app,
            Position = new Point(1, 1),
            size = new Size(12, 1),
            Text = "initial"
        };
        var button = new Button
        {
            Parent = app,
            Position = new Point(1, 3),
            size = new Size(8, 1),
            Text = "Update"
        };
        button.OnClick += (_, _) => label.Text = "updated";

        app.Visible = true;
        app.Draw();
        ext.RaiseMouse(new MouseEvt
        {
            ButtonEvent = true,
            MousePos = new Point(2, 3),
            MouseButtonLeft = true
        });
        app.ProcessPendingMessages();

        StringAssert.Contains(__tstCon!.Content, "updated");
    }

    [TestMethod]
    public void Tab_Key_Moves_Focus_To_Next_Control()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext));
        var first = new Button { Parent = app, Position = new Point(1, 1), size = new Size(8, 1) };
        var second = new Button { Parent = app, Position = new Point(1, 2), size = new Size(8, 1) };

        ext.RaiseKey(new KeyEvt { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.Tab });
        app.ProcessPendingMessages();
        Assert.IsTrue(first.Active);

        ext.RaiseKey(new KeyEvt { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.Tab });
        app.ProcessPendingMessages();
        Assert.IsTrue(second.Active);
    }

    [TestMethod]
    public void F10_TogglesMenuKeyboardMode_AndAltTogglesAcceleratorVisibility()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext));
        var menu = new MenuBar { Parent = app };
        var popup = new MenuPopup();
        menu.AddRootItem(new MenuItem { Text = "&File" }, popup);

        app.RaiseKeyEvent(new KeyEvt { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.F10 });
        Assert.IsTrue(menu.IsKeyboardActive);
        Assert.IsTrue(menu.ActiveControl!.Active);
        Assert.IsTrue(popup.Visible);

        app.RaiseKeyEvent(new KeyEvt { bKeyDown = true, usKeyCode = 0x12 });
        Assert.IsTrue(menu.ShowAccelerators);
        app.RaiseKeyEvent(new KeyEvt { bKeyDown = false, usKeyCode = 0x12 });
        Assert.IsFalse(menu.ShowAccelerators);

        app.RaiseKeyEvent(new KeyEvt { bKeyDown = true, usKeyCode = (ushort)ConsoleKey.F10 });
        Assert.IsFalse(menu.IsKeyboardActive);
        Assert.IsNull(menu.ActiveControl);
        Assert.IsFalse(popup.Visible);
    }

    [TestMethod]
    public void Resize_ClearsHost_And_RedrawsApplication()
    {
        FakeExt ext = new FakeExt();
        var app = new Application(new ConsoleWidgetSet(_tstCon, ext))
        {
            Dimension = new Rectangle(0, 0, 30, 8)
        };
        var label = new Label
        {
            Parent = app,
            Position = new Point(1, 1),
            size = new Size(20, 1),
            Text = "after resize"
        };

        app.Draw();
        StringAssert.Contains(__tstCon!.Content, "after resize");

        ext.RaiseResize(new Point(120, 30));
        app.ProcessPendingMessages();
        app.ProcessPendingMessages();

        StringAssert.Contains(__tstCon.Content, "after resize");
    }
}