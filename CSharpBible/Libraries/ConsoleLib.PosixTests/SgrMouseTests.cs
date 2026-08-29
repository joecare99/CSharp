using System.Drawing;
using System.Linq;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if NET5_0_OR_GREATER
using ConsoleLib.Posix;
#endif

namespace ConsoleLibTests;

[TestClass]
public sealed class SgrMouseTests
{
#if NET5_0_OR_GREATER

    [TestMethod]
    public void ParsesPressReleaseMotionAndWheel()
    {
        var parser = new SgrMouseParser();
        var events = parser.Decode("\u001b[<0;4;3M\u001b[<2;4;3m\u001b[<35;5;6M\u001b[<64;5;6M").ToArray();

        Assert.AreEqual(4, events.Length);
        Assert.AreEqual(new Point(3, 2), events[0].Position);
        Assert.AreEqual(PointerInputKind.Press, events[0].Kind);
        Assert.AreEqual(PointerButtons.Left, events[0].Buttons);
        Assert.AreEqual(PointerInputKind.Release, events[1].Kind);
        Assert.AreEqual(PointerButtons.Right, events[1].Buttons);
        Assert.AreEqual(PointerInputKind.Move, events[2].Kind);
        Assert.AreEqual(PointerInputKind.Wheel, events[3].Kind);
        Assert.AreEqual(120, events[3].WheelDelta);
    }

    [TestMethod]
    public void PreservesModifiersAndHandlesSplitSequence()
    {
        var parser = new SgrMouseParser();
        Assert.AreEqual(0, parser.Decode("\u001b[<").Count);

        var pointer = parser.Decode("20;8;9M").Single();
        Assert.AreEqual(new Point(7, 8), pointer.Position);
        Assert.AreEqual(KeyModifiers.Shift | KeyModifiers.Control, pointer.Modifiers);
        Assert.AreEqual(PointerButtons.Left, pointer.Buttons);
    }

    [TestMethod]
    public void EncodesPointerAndTrackingControls()
    {
        var pointer = new PointerInput(new Point(3, 2), PointerInputKind.Release, PointerButtons.Right);

        Assert.AreEqual("\u001b[<2;4;3m", SgrMouseEncoder.Encode(pointer));
        Assert.AreEqual("\u001b[?1000h\u001b[?1006h", SgrMouseEncoder.EnableTracking);
        Assert.AreEqual("\u001b[?1006l\u001b[?1000l", SgrMouseEncoder.DisableTracking);
    }
#endif
}
