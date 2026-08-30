using ConsoleLib.Showcase.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class VisualEffectsTests
{
    [TestMethod]
    public void CreateWaveFrame_ReturnsRequestedWidthAndDeterministicFrames()
    {
        var effects = new VisualEffects();

        var first = effects.CreateWaveFrame(0, 12);
        var second = effects.CreateWaveFrame(1, 12);

        Assert.AreEqual(12, first.Length);
        Assert.AreEqual(12, second.Length);
        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void CreateProgressFrame_ClampsFraction()
    {
        var effects = new VisualEffects();

        Assert.AreEqual("█████", effects.CreateProgressFrame(2, 5));
        Assert.AreEqual("░░░░░", effects.CreateProgressFrame(-1, 5));
    }
}
