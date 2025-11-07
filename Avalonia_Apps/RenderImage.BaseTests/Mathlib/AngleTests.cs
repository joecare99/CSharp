using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RenderImage.Base.Mathlib.Tests;

[TestClass]
public class AngleTests
{
    private static void AreEqual(Angle exp, Angle act, double eps, string msg)
    {
        Assert.AreEqual(exp.Value, act.Value, eps, msg);
    }

    [TestMethod]
    [DataRow(0.0, 0.0)]
    [DataRow(Math.PI * 0.5, Math.PI * 0.5)]
    [DataRow(-Math.PI * 0.5, -Math.PI * 0.5)]
    [DataRow(Math.PI * 0.9, Math.PI * 0.9)]
    [DataRow(-Math.PI * 0.9, -Math.PI * 0.9)]
    [DataRow(Math.PI, -Math.PI)]
    [DataRow(-Math.PI, -Math.PI)]
    [DataRow(1.5 * Math.PI, -0.5 * Math.PI)]
    [DataRow(-1.5 * Math.PI, 0.5 * Math.PI)]
    [DataRow(2.0 * Math.PI, 0.0)]
    [DataRow(-2.0 * Math.PI, 0.0)]
    [DataRow(2.5 * Math.PI, 0.5 * Math.PI)]
    [DataRow(-2.5 * Math.PI, -0.5 * Math.PI)]
    public void Normalize_Static(double input, double expected)
    {
        var res = Angle.Normalize(new Angle { Value = input });
        AreEqual(new Angle { Value = expected }, res, 1e-15, "Normalize");
    }

    [TestMethod]
    [DataRow(0.0, 0.0)]
    [DataRow(Math.PI * 0.5, Math.PI * 0.5)]
    [DataRow(-Math.PI * 0.5, -Math.PI * 0.5)]
    [DataRow(Math.PI * 0.9, Math.PI * 0.9)]
    [DataRow(-Math.PI * 0.9, -Math.PI * 0.9)]
    [DataRow(Math.PI, -Math.PI)]
    [DataRow(-Math.PI, -Math.PI)]
    [DataRow(1.5 * Math.PI, -0.5 * Math.PI)]
    [DataRow(-1.5 * Math.PI, 0.5 * Math.PI)]
    [DataRow(2.0 * Math.PI, 0.0)]
    [DataRow(-2.0 * Math.PI, 0.0)]
    [DataRow(2.5 * Math.PI, 0.5 * Math.PI)]
    [DataRow(-2.5 * Math.PI, -0.5 * Math.PI)]
    public void Normalize_Instance(double input, double expected)
    {
        var a = new Angle { Value = input };
        AreEqual(new Angle { Value = expected }, a.Normalize(), 1e-15, "Normalize");
        AreEqual(new Angle { Value = input }, a, 1e-15, "Instance not mutated");
    }

    [TestMethod]
    [DataRow(0.0, 0.0, 0.0)]
    [DataRow(0.1, 0.0, 0.1)]
    [DataRow(0.1, 0.1, 0.2)]
    [DataRow(0.1, -0.1, 0.0)]
    [DataRow(2.0 * Math.PI, 0.0, 0.0)]
    [DataRow(-2.0 * Math.PI + 0.1, 0.0, 0.1)]
    [DataRow(2.0 * Math.PI + 0.1, 0.1, 0.2)]
    [DataRow(-2.0 * Math.PI + 0.1, -0.1, 0.0)]
    [DataRow(0.1, 2.0 * Math.PI + 0.1, 0.2)]
    [DataRow(0.1, 2.0 * Math.PI - 0.1, 0.0)]
    public void Sum_ReturnsNormalized(double a, double b, double expected)
    {
        var res = new Angle { Value = a }.Sum(new Angle { Value = b });
        AreEqual(new Angle { Value = expected }, res, 5e-13, "Sum");
    }

    [TestMethod]
    [DataRow(0.0, 0.0, 0.0)]
    [DataRow(0.1, 0.0, 0.1)]
    [DataRow(0.1, 0.1, 0.2)]
    [DataRow(0.1, -0.1, 0.0)]
    public void Add_MutatesToSum(double a, double b, double expected)
    {
        var x = new Angle { Value = a };
        var res = x.Add(new Angle { Value = b });
        AreEqual(new Angle { Value = expected }, res, 5e-13, "Add result");
        AreEqual(new Angle { Value = expected }, x, 5e-13, "Add mutated");
    }

    [TestMethod]
    [DataRow(0.0, 0.0, 0.0)]
    [DataRow(0.1, 0.0, 0.1)]
    [DataRow(0.1, -0.1, 0.2)]
    [DataRow(0.1, 0.1, 0.0)]
    [DataRow(2.0 * Math.PI, 0.0, 0.0)]
    [DataRow(-2.0 * Math.PI + 0.1, 0.0, 0.1)]
    [DataRow(2.0 * Math.PI + 0.1, -0.1, 0.2)]
    [DataRow(-2.0 * Math.PI + 0.1, 0.1, 0.0)]
    [DataRow(0.1, 2.0 * Math.PI - 0.1, 0.2)]
    [DataRow(0.1, 2.0 * Math.PI + 0.1, 0.0)]
    public void Diff_ReturnsNormalized(double a, double b, double expected)
    {
        var res = new Angle { Value = a }.Diff(new Angle { Value = b });
        AreEqual(new Angle { Value = expected }, res, 5e-13, "Diff");
    }

    [TestMethod]
    [DataRow(0.0, 0.0, 0.0)]
    [DataRow(0.1, 0.0, 0.1)]
    [DataRow(0.1, -0.1, 0.2)]
    [DataRow(0.1, 0.1, 0.0)]
    public void Subt_MutatesToDiff(double a, double b, double expected)
    {
        var x = new Angle { Value = a };
        var res = x.Subt(new Angle { Value = b });
        AreEqual(new Angle { Value = expected }, res, 5e-13, "Subt result");
        AreEqual(new Angle { Value = expected }, x, 5e-13, "Subt mutated");
    }
}
