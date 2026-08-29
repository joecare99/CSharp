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
