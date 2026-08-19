using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptedSvgWpf.Dsl;
using ScriptedSvgWpf.Rendering;
using ScriptedSvgWpf.Samples;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class CheckerboardTests
{
    [TestMethod]
    public void CheckerboardUsesNestedLoopsParityScalingAndRotation()
    {
        var document = new ScriptInterpreter().Execute(CheckerboardScript.Source);
        var rectangles = document.Commands.OfType<RectangleCommand>().ToArray();

        Assert.AreEqual(640, document.Width);
        Assert.AreEqual(640, document.Height);
        Assert.AreEqual(72, rectangles.Length);
        Assert.IsTrue(rectangles.Any(rectangle => rectangle.Scale < 0.1));
        Assert.IsTrue(rectangles.Any(rectangle => rectangle.Scale > 0.9));
        Assert.IsTrue(rectangles.Any(rectangle => rectangle.Rotation > 0));
        Assert.IsTrue(rectangles.All(rectangle => rectangle.Width == 48 && rectangle.Height == 48));
    }
}
