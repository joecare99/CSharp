using System;
using System.Collections.Generic;
using ScreenX.Base;

namespace ScreenX.BaseTests;

[TestClass]
public class RendererServiceTests
{
    [TestMethod]
    public void IdentityFunction_ProducesExpectedGradient()
    {
        var opts = new RenderOptions(
        Width: 4,
        Height: 4,
        Source: new ExRect(0, 0, 1, 1),
        Functions: new List<DFunction> { (ExPoint p, ExPoint p0, ref bool brk) => p },
        Colorizer: p => Color32.FromRgb((byte)(p.X * 255), (byte)(p.Y * 255), 0));

        var sut = new RendererService();
        var res = sut.Render(opts);
        Assert.HasCount(16, res.Pixels);
        Assert.AreEqual(4, res.Width);
        Assert.AreEqual(4, res.Height);
        // spot-check a few pixels
        uint c00 = res.Pixels[0]; // x=0,y=0
        uint c33 = res.Pixels[3 + 3 * 4];
        Assert.AreNotEqual(c00, c33);
    }

    [TestMethod]
    public void FunctionChain_ComposesCorrectly()
    {
        DFunction f1 = (ExPoint p, ExPoint p0, ref bool brk) => new ExPoint(p.X + 1, p.Y);
        DFunction f2 = (ExPoint p, ExPoint p0, ref bool brk) => new ExPoint(p.X, p.Y + 2);
        var opts = new RenderOptions(
       2, 2,
        new ExRect(0, 0, 0, 0),
        new List<DFunction> { f1, f2 },
        p => Color32.FromRgb((byte)p.X, (byte)p.Y, 0));
        var sut = new RendererService();
        var res = sut.Render(opts);
        foreach (var c in res.Pixels)
        {
            Assert.AreNotEqual(0u, c);
        }
    }
}
