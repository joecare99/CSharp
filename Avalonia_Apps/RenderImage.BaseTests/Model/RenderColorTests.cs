using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenderImage.Base.Model;
using System;

namespace RenderImage.Base.Model.Tests;

[TestClass]
public class RenderColorTests
{
    [TestMethod]
    public void Init_SetsComponents()
    {
        var c = new RenderColor();
        c.Init(0.1, 0.2, 0.3);
        Assert.AreEqual(0.1, c.Red, 1e-12);
        Assert.AreEqual(0.2, c.Green, 1e-12);
        Assert.AreEqual(0.3, c.Blue, 1e-12);
    }

    [TestMethod]
    [DataRow(1.0, 0.0, 0.0)]
    [DataRow(0.0, 1.0, 0.0)]
    [DataRow(0.0, 0.0, 1.0)]
    public void Init_Primaries(double r, double g, double b)
    {
        var c = new RenderColor();
        c.Init(r, g, b);
        Assert.AreEqual(r, c.Red, 1e-12);
        Assert.AreEqual(g, c.Green, 1e-12);
        Assert.AreEqual(b, c.Blue, 1e-12);
    }

    [TestMethod]
    public void Plus_ReturnsSum()
    {
        var a = new RenderColor(); a.Init(0.1, 0.2, 0.3);
        var b = new RenderColor(); b.Init(0.4, 0.5, 0.6);
        var s = a.Plus(b);
        Assert.AreEqual(0.5, s.Red, 1e-12);
        Assert.AreEqual(0.7, s.Green, 1e-12);
        Assert.AreEqual(0.9, s.Blue, 1e-12);
    }

    [TestMethod]
    public void Minus_ClampsAtZero()
    {
        var a = new RenderColor(); a.Init(0.1, 0.2, 0.3);
        var b = new RenderColor(); b.Init(0.2, 0.3, 0.1);
        var d = a.Minus(b);
        Assert.AreEqual(0.0, d.Red, 1e-12);
        Assert.AreEqual(0.0, d.Green, 1e-12);
        Assert.AreEqual(0.2, d.Blue, 1e-12);
    }

    [TestMethod]
    public void Mult_ScalesComponents()
    {
        var a = new RenderColor(); a.Init(0.2, 0.4, 0.6);
        var m = a.Mult(2.5);
        Assert.AreEqual(0.5, m.Red, 1e-12);
        Assert.AreEqual(1.0, m.Green, 1e-12);
        Assert.AreEqual(1.5, m.Blue, 1e-12);
    }

    [TestMethod]
    public void Mix_Interpolates()
    {
        var a = new RenderColor(); a.Init(0.0, 0.0, 0.0);
        var b = new RenderColor(); b.Init(1.0, 1.0, 1.0);
        var mid = a.Mix(b, 0.25);
        Assert.AreEqual(0.25, mid.Red, 1e-12);
        Assert.AreEqual(0.25, mid.Green, 1e-12);
        Assert.AreEqual(0.25, mid.Blue, 1e-12);
    }

    [TestMethod]
    public void Filter_ComponentWiseMultiply()
    {
        var a = new RenderColor(); a.Init(0.2, 0.4, 0.6);
        var b = new RenderColor(); b.Init(0.5, 0.25, 0.1);
        var f = a.Filter(b);
        Assert.AreEqual(0.1, f.Red, 1e-12);
        Assert.AreEqual(0.1, f.Green, 1e-12);
        Assert.AreEqual(0.06, f.Blue, 1e-12);
    }

    [TestMethod]
    public void Equals_WithTolerance()
    {
        var a = new RenderColor(); a.Init(0.1, 0.2, 0.3);
        var b = new RenderColor(); b.Init(0.10005, 0.20005, 0.30005);
        Assert.IsTrue(a.Equals(b));
        var c = new RenderColor(); c.Init(0.101, 0.2, 0.3);
        Assert.IsFalse(a.Equals(c));
    }

    [TestMethod]
    public void Operators_BehaveLikeMethods()
    {
        var a = new RenderColor(); a.Init(0.1, 0.2, 0.3);
        var b = new RenderColor(); b.Init(0.4, 0.5, 0.6);
        var s1 = a + b;
        var s2 = a.Plus(b);
        Assert.AreEqual(s2.Red, s1.Red, 1e-12);
        Assert.AreEqual(s2.Green, s1.Green, 1e-12);
        Assert.AreEqual(s2.Blue, s1.Blue, 1e-12);

        var d1 = a - b;
        var d2 = a.Minus(b);
        Assert.AreEqual(d2.Red, d1.Red, 1e-12);
        Assert.AreEqual(d2.Green, d1.Green, 1e-12);
        Assert.AreEqual(d2.Blue, d1.Blue, 1e-12);

        var f1 = a * b;
        var f2 = a.Filter(b);
        Assert.AreEqual(f2.Red, f1.Red, 1e-12);
        Assert.AreEqual(f2.Green, f1.Green, 1e-12);
        Assert.AreEqual(f2.Blue, f1.Blue, 1e-12);

        var m1 = a * 2.0;
        var m2 = a.Mult(2.0);
        Assert.AreEqual(m2.Red, m1.Red, 1e-12);
        Assert.AreEqual(m2.Green, m1.Green, 1e-12);
        Assert.AreEqual(m2.Blue, m1.Blue, 1e-12);

        var m3 = 2.0 * a;
        Assert.AreEqual(m2.Red, m3.Red, 1e-12);
        Assert.AreEqual(m2.Green, m3.Green, 1e-12);
        Assert.AreEqual(m2.Blue, m3.Blue, 1e-12);
    }

    [TestMethod]
    public void ToString_FormatsInvariant()
    {
        var a = new RenderColor(); a.Init(Math.PI, Math.E, 1.0 / 3.0);
        var s = a.ToString();
        StringAssert.StartsWith(s, "RGB(");
        StringAssert.Contains(s, ",");
        StringAssert.EndsWith(s, ")");
        Assert.IsTrue(s.Contains("3.142"));
        Assert.IsTrue(s.Contains("2.718"));
        Assert.IsTrue(s.Contains("0.333"));
    }

    [TestMethod]
    [DataRow(0.0, 1.0, 0.5, 1.0, 0.25, 0.25)]
    [DataRow(1.0 / 3.0, 1.0, 0.5, 0.25, 1.0, 0.25)]
    [DataRow(2.0 / 3.0, 1.0, 0.5, 0.25, 0.25, 1.0)]
    public void InitHSB_KnownHues(double hue, double satur, double bright, double er, double eg, double eb)
    {
        var c = new RenderColor();
        c.InitHSB(hue, satur, bright);
        Assert.AreEqual(er, c.Red, 1e-12);
        Assert.AreEqual(eg, c.Green, 1e-12);
        Assert.AreEqual(eb, c.Blue, 1e-12);
    }

    [TestMethod]
    public void InitHSB_ProducesExpectedRanges()
    {
        var c = new RenderColor();
        for (double h = 0; h <= 1.0; h += 0.2)
        {
            c.InitHSB(h, 1.0, 0.5);
            Assert.IsTrue(c.Red >= 0 && c.Red <= 1.0);
            Assert.IsTrue(c.Green >= 0 && c.Green <= 1.0);
            Assert.IsTrue(c.Blue >= 0 && c.Blue <= 1.0);
        }
    }
}
