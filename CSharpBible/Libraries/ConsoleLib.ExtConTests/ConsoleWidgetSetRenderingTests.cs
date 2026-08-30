using BaseLib.Interfaces;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Drawing;
using System.Reflection;

namespace ConsoleLibTests;

[TestClass]
public class ConsoleWidgetSetRenderingTests
{
    [TestInitialize]
    public void ResetCanvasBeforeTest()
    {
        ResetCanvas();
    }

    [TestCleanup]
    public void ResetCanvasAfterTest()
    {
        ResetCanvas();
    }

    [TestMethod]
    public void DrawControl_WithInsufficientWidth_DoesNotThrow()
    {
        foreach (int width in new[] { 0, 1, 2 })
        {
            var console = CreateConsole();
            var widgetSet = new ConsoleWidgetSet(console, Substitute.For<IExtendedConsole>());
            var button = new Button
            {
                size = new Size(width, 1),
                Text = "button"
            };

            widgetSet.DrawControl(button);

            Assert.AreEqual(width, button.Dimension.Width);
        }
    }

    [TestMethod]
    public void DrawControl_WithNormalWidth_CompletesSuccessfully()
    {
        var console = CreateConsole();
        var widgetSet = new ConsoleWidgetSet(console, Substitute.For<IExtendedConsole>());
        var button = new Button
        {
            Position = new Point(2, 3),
            size = new Size(8, 1),
            Text = "OK"
        };

        widgetSet.DrawControl(button);

        Assert.AreEqual(new Size(8, 1), button.Dimension.Size);
    }

    [TestMethod]
    public void HostOperations_DelegateToConsoleAndExtendedConsole()
    {
        var console = CreateConsole();
        console.IsOutputRedirected.Returns(true);
        var extendedConsole = Substitute.For<IExtendedConsole>();
        var widgetSet = new ConsoleWidgetSet(console, extendedConsole);

        Assert.AreEqual(80, widgetSet.WindowWidth);
        Assert.IsTrue(widgetSet.IsOutputRedirected);
        Assert.AreEqual(ConsoleFramework.VK_ENTER, widgetSet.KeyEnter);
        Assert.AreEqual(ConsoleFramework.VK_ESC, widgetSet.KeyEsc);
        Assert.AreEqual(ConsoleFramework.VK_TAB, widgetSet.KeyTab);
        Assert.AreEqual(ConsoleFramework.VK_LEFT, widgetSet.KeyLeft);
        Assert.AreEqual(ConsoleFramework.VK_UP, widgetSet.KeyUp);
        Assert.AreEqual(ConsoleFramework.VK_RIGHT, widgetSet.KeyRight);
        Assert.AreEqual(ConsoleFramework.VK_DOWN, widgetSet.KeyDown);
        Assert.AreEqual(ConsoleFramework.VK_HOME, widgetSet.KeyHome);
        Assert.AreEqual(ConsoleFramework.VK_END, widgetSet.KeyEnd);
        Assert.AreEqual(ConsoleFramework.VK_DELETE, widgetSet.KeyDelete);
        Assert.AreEqual(ConsoleFramework.VK_PRIOR, widgetSet.KeyPageUp);
        Assert.AreEqual(ConsoleFramework.VK_NEXT, widgetSet.KeyPageDown);

        widgetSet.ClearHost();
        widgetSet.StopHost();
        widgetSet.Beep(440, 100);
        widgetSet.SetCursorPosition(4, 5);

        console.Received().Clear();
        extendedConsole.Received().Stop();
        console.Received().Beep(440, 100);
        console.Received().SetCursorPosition(4, 5);
    }

    private static IConsole CreateConsole()
    {
        var console = Substitute.For<IConsole>();
        console.WindowWidth.Returns(80);
        console.LargestWindowHeight.Returns(25);
        console.BufferHeight.Returns(25);
        return console;
    }

    private static void ResetCanvas()
    {
        FieldInfo? canvasField = typeof(ConsoleFramework).GetField("_canvas", BindingFlags.Static | BindingFlags.NonPublic);
        if (canvasField == null)
        {
            throw new InvalidOperationException("The ConsoleFramework canvas field was not found.");
        }

        canvasField.SetValue(null, null);
    }
}
