using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;
using System;

namespace RenderImage.Base.Model.Tests;

[TestClass]
public class RenderMathTests
{
    [TestMethod]
    [DataRow(0.0, 0.0, 0.0, 1.0, -1.0)]
    [DataRow(1.0, 0.0, -1.0, -1.0, 1.0)]
    [DataRow(2.0, 0.0, -2.0, -1.0, 1.0)]
    [DataRow(1.0, -2.0, 0.0, 0.0, 2.0)]
    public void SolveQuadratic_SimpleCases(double a, double b, double c, double e1, double e2)
    {
        var ok = RenderMath.SolveQuadratic(a, b, c, out var l1, out var l2);
        Assert.IsTrue(ok);
        Assert.AreEqual(e1, l1, 1e-12);
        Assert.AreEqual(e2, l2, 1e-12);
    }

    [TestMethod]
    public void SolveQuadratic_ParametricRoots()
    {
        var rnd = new Random(1234);
        for (int i = 0; i < 500; i++)
        {
            var x1 = (rnd.NextDouble() - 0.5) * 20;
            var x2 = (rnd.NextDouble() - 0.5) * 20;
            var a = 1.0;
            var b = -x1 - x2;
            var c = x1 * x2;
            var ok = RenderMath.SolveQuadratic(a, b, c, out var l1, out var l2);
            Assert.IsTrue(ok, $"solvable[{i}]");
            // order may be swapped; sort
            if (l1 > l2) (l1, l2) = (l2, l1);
            if (x1 > x2) (x1, x2) = (x2, x1);
            Assert.AreEqual(x1, l1, 1e-6, $"root1[{i}]");
            Assert.AreEqual(x2, l2, 1e-6, $"root2[{i}]");
        }
    }

    [TestMethod]
    [DataRow(0.0, 0.0, 1.0)]
    [DataRow(1.0, 0.0, 1.0)]
    [DataRow(2.0, 0.0, 2.0)]
    [DataRow(1.0, -2.0, 2.0)]
    public void SolveQuadratic_Unsolvable(double a, double b, double c)
    {
        var ok = RenderMath.SolveQuadratic(a, b, c, out var l1, out var l2);
        Assert.IsFalse(ok);
    }
}
