using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System;
using System.ComponentModel;
using System.Drawing;
using NSubstitute;
using ConsoleLib.Interfaces;
using ConsoleLib.CommonControls.Tests; // for TestBase

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class ScrollBarTests : TestBase
{
    private sealed class ValueModel : INotifyPropertyChanged
    {
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed class MouseEventStub : IMouseEvent
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
    public void Ctor_Defaults()
    {
        var sb = new ScrollBar();
        Assert.IsTrue(sb.Vertical);
        Assert.AreEqual(0, sb.Minimum);
        Assert.AreEqual(100, sb.Maximum);
        Assert.AreEqual(0, sb.Value);
    }

    [TestMethod]
    public void Set_Minimum_Adjusts_Max_And_Value()
    {
        var sb = new ScrollBar();
        sb.Value = 50;
        sb.Minimum = 60;
        Assert.AreEqual(60, sb.Minimum);
        Assert.AreEqual(100, sb.Maximum);
        Assert.AreEqual(60, sb.Value);
    }

    [TestMethod]
    public void Set_Maximum_Clamps_Value()
    {
        var sb = new ScrollBar();
        sb.Value = 90;
        sb.Maximum = 80;
        Assert.AreEqual(80, sb.Maximum);
        Assert.AreEqual(80, sb.Value);
    }

    [TestMethod]
    public void LargeChange_Rejects_Zero()
    {
        var sb = new ScrollBar();
        sb.LargeChange = 0;
        Assert.AreEqual(1, sb.LargeChange);
    }

    [TestMethod]
    public void Value_Raises_Event()
    {
        var sb = new ScrollBar();
        int cnt=0; sb.OnValueChanged += (_,_)=>cnt++;
        sb.Value = 10; sb.Value = 10; sb.Value = 15;
        Assert.AreEqual(2,cnt); // initial->10, 10->15
    }

    [TestMethod]
    public void Draw_Validates()
    {
        var sb = new ScrollBar{ Dimension=new Rectangle(0,0,1,8)};
        sb.Value = 30;
        sb.Draw();
        Assert.IsTrue(sb.Valid);
    }

    [TestMethod]
    public void Thumb_Computes_For_Horizontal()
    {
        var sb = new ScrollBar{ Vertical=false, Dimension=new Rectangle(0,0,10,1)};
        sb.Maximum = 200; sb.Minimum = 0; sb.Value = 100; sb.LargeChange=50;
        sb.Draw();
        Assert.IsTrue(sb.Valid);
    }

    [TestMethod]
    public void Keyboard_Small_And_Large_Steps()
    {
        var sb = new ScrollBar();
        var key = Substitute.For<IKeyEvent>();
        key.bKeyDown.Returns(true);
        key.KeyChar.Returns('+'); sb.HandlePressKeyEvents(key); Assert.AreEqual(1,sb.Value);
        key.KeyChar.Returns('-'); sb.HandlePressKeyEvents(key); Assert.AreEqual(0,sb.Value);
        key.KeyChar.Returns('P'); sb.HandlePressKeyEvents(key); Assert.AreEqual(sb.LargeChange,sb.Value);
        key.KeyChar.Returns('O'); sb.HandlePressKeyEvents(key); Assert.AreEqual(0,sb.Value);
    }

    [TestMethod]
    public void Value_Clamped_On_Set()
    {
        var sb = new ScrollBar();
        sb.Value = 500;
        Assert.AreEqual(100, sb.Value);
        sb.Value = -10;
        Assert.AreEqual(0, sb.Value);
    }

    [TestMethod]
    public void MouseClick_ArrowsAndTrack_ChangeValue()
    {
        var sb = new ScrollBar { Dimension = new Rectangle(2, 3, 1, 10), Maximum = 100, LargeChange = 10 };

        sb.MouseClick(new MouseEventStub { MousePos = new Point(2, 3) });
        Assert.AreEqual(0, sb.Value);

        sb.MouseClick(new MouseEventStub { MousePos = new Point(2, 12) });
        Assert.AreEqual(1, sb.Value);

        sb.Value = 50;
        sb.MouseClick(new MouseEventStub { MousePos = new Point(2, 4) });
        Assert.AreEqual(40, sb.Value);

        sb.MouseClick(new MouseEventStub { MousePos = new Point(2, 11) });
        Assert.AreEqual(50, sb.Value);
    }

    [TestMethod]
    public void MouseClick_HorizontalTrack_UsesXCoordinate()
    {
        var sb = new ScrollBar { Vertical = false, Dimension = new Rectangle(4, 5, 10, 1), Maximum = 100 };

        sb.MouseClick(new MouseEventStub { MousePos = new Point(4, 5) });
        Assert.AreEqual(0, sb.Value);
        sb.MouseClick(new MouseEventStub { MousePos = new Point(13, 5) });
        Assert.AreEqual(1, sb.Value);
    }

    [TestMethod]
    public void MouseMove_TracksHoverAndDragging()
    {
        var sb = new ScrollBar { Dimension = new Rectangle(0, 0, 1, 12), Maximum = 100, LargeChange = 10, Value = 50 };
        var mouse = new MouseEventStub { MouseMoved = true, MousePos = new Point(0, 0) };

        sb.MouseMove(mouse, Point.Empty);
        Assert.IsTrue(sb.IsHoveringDecreaseArrow());
        mouse.MousePos = new Point(0, 5);
        sb.MouseMove(mouse, new Point(0, 0));
        Assert.IsTrue(sb.IsHoveringThumb());

        sb.MouseClick(new MouseEventStub { MousePos = new Point(0, 5) });
        Assert.IsTrue(sb.IsDraggingThumb());
        mouse.MousePos = new Point(0, 10);
        sb.MouseMove(mouse, new Point(0, 5));
        Assert.IsTrue(sb.Value > 0);

        sb.MouseLeave(Point.Empty);
        Assert.IsFalse(sb.IsDraggingThumb());
        Assert.IsFalse(sb.IsHoveringThumb());
    }

    [TestMethod]
    public void BindValue_SynchronizesBothDirections()
    {
        var model = new ValueModel { Value = 25 };
        var sb = new ScrollBar();

        sb.BindValue(model, nameof(ValueModel.Value));
        Assert.AreEqual(25, sb.Value);

        sb.Value = 40;
        Assert.AreEqual(40, model.Value);
        model.Value = 60;
        Assert.AreEqual(60, sb.Value);
    }

    [TestMethod]
    public void BindValue_RejectsMissingArguments()
    {
        var sb = new ScrollBar();
        var model = new ValueModel();

        Assert.Throws<ArgumentNullException>(() => sb.BindValue(null!, nameof(ValueModel.Value)));
        Assert.Throws<ArgumentException>(() => sb.BindValue(model, " "));
    }
}
