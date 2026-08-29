using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ConsoleLib.Tests;

[TestClass]
public class V2InputContractTests
{
    [TestMethod]
    public void KeyInput_PreservesLogicalKeyAndModifiers()
    {
        var input = new KeyInput(ConsoleKey.LeftArrow, '\0', KeyModifiers.Control | KeyModifiers.Shift, true, true);

        Assert.AreEqual(ConsoleKey.LeftArrow, input.Key);
        Assert.AreEqual(KeyModifiers.Control | KeyModifiers.Shift, input.Modifiers);
        Assert.IsTrue(input.IsKeyDown);
        Assert.IsTrue(input.IsRepeat);
    }

    [TestMethod]
    public void DispatchContext_CanHandleWithoutMutatingPayload()
    {
        var input = new KeyInput(ConsoleKey.Enter, '\r', KeyModifiers.None, true);
        var context = new InputDispatchContext();

        context.MarkHandled(stopPropagation: false);

        Assert.IsTrue(context.Handled);
        Assert.IsFalse(context.StopPropagation);
        Assert.AreEqual('\r', input.KeyChar);
    }

    [TestMethod]
    public void PointerInput_PreservesPositionButtonsAndWheel()
    {
        var input = new PointerInput(new Point(4, 7), PointerInputKind.Wheel, PointerButtons.Left, -1);

        Assert.AreEqual(new Point(4, 7), input.Position);
        Assert.AreEqual(PointerInputKind.Wheel, input.Kind);
        Assert.AreEqual(PointerButtons.Left, input.Buttons);
        Assert.AreEqual(-1, input.WheelDelta);
    }
}
