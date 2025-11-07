using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RenderImage.BaseTests.Mathlib;

[TestClass]
public class TFTripleTests
{
    private TFTriple _t;

    [TestInitialize]
    public void SetUp()
    {
        _t.Init(0, 0, 0);
    }

    private static void AreEqual(TFTriple exp, TFTriple act, double eps, string msg)
    {
        Assert.AreEqual(exp.X, act.X, eps, msg + "[X]");
        Assert.AreEqual(exp.Y, act.Y, eps, msg + "[Y]");
        Assert.AreEqual(exp.Z, act.Z, eps, msg + "[Z]");
    }

    [TestMethod]
    public void TestSetUp()
    {
        Assert.AreEqual(0.0, _t.X, 1e-20);
        Assert.AreEqual(0.0, _t.Y, 1e-20);
        Assert.AreEqual(0.0, _t.Z, 1e-20);
    }

    [TestMethod]
    public void TestInit()
    {
        AreEqual(new TFTriple { X = 0, Y = 0, Z = 0 }, _t, 1e-20, "ZeroTrp");
        _t.Init(0, 0, 0);
        AreEqual(new TFTriple { X = 0, Y = 0, Z = 0 }, _t, 1e-20, "init(0,0,0)");
        _t.Init(1.0, -1.0, 0.5);
        AreEqual(new TFTriple { X = 1.0, Y = -1.0, Z = 0.5 }, _t, 1e-20, "init(1.0,-1.0,0.5)");
        Assert.AreEqual(1.0, _t[0], 1e-20);
        Assert.AreEqual(-1.0, _t[1], 1e-20);
        _t.Init(23.0, 17.0, 13.0);
        AreEqual(new TFTriple { X = 23.0, Y = 17.0, Z = 13.0 }, _t, 1e-20, "init(23,17,13)");
        Assert.AreEqual(23.0, _t[0], 1e-20);
        Assert.AreEqual(17.0, _t[1], 1e-20);
    }

    [TestMethod]
    public void TestAddAndSubt()
    {
        _t.Init(1.0, -1.0, 0.5);
        var r = _t.Add(new TFTriple { X = 2.0, Y = -2.0, Z = 2.0 });
        AreEqual(new TFTriple { X = 3.0, Y = -3.0, Z = 2.5 }, r, 1e-12, "add");
        _t.Init(23.0, 17.0, 13.0);
        AreEqual(new TFTriple { X = 26.0, Y = 14.0, Z = 14.5 }, _t.Add(new TFTriple { X = 3.0, Y = -3.0, Z = 1.5 }), 1e-12, "add2");
        _t.Init(1.0, -1.0, 0.5);
        AreEqual(new TFTriple { X = -1.0, Y = 1.0, Z = -1.5 }, _t.Subt(new TFTriple { X = 2.0, Y = -2.0, Z = 2.0 }), 1e-12, "subt");
    }

    [TestMethod]
    public void TestDotCross()
    {
        _t.Init(1.0, -1.0, 0.5);
        Assert.AreEqual(5.0, _t.Mul(new TFTriple { X = 2.0, Y = -2.0, Z = 2.0 }), 1e-12);
        _t.Init(23.0, 17.0, 13.0);
        AreEqual(new TFTriple { X = 64.5, Y = 4.5, Z = -120.0 }, _t.XMul(new TFTriple { X = 3.0, Y = -3.0, Z = 1.5 }), 1e-12, "cross");
    }

    [TestMethod]
    public void TestScaleDivide()
    {
        _t.Init(1.0, -1.0, 0.5);
        AreEqual(new TFTriple { X = 2.0, Y = -2.0, Z = 1.0 }, _t.Mul(2.0), 1e-12, "mul");
        _t.Init(23.0, 17.0, 13.0);
        AreEqual(new TFTriple { X = -69.0, Y = -51.0, Z = -39.0 }, _t.Mul(-3.0), 1e-12, "mul2");
        _t.Init(1.0, -1.0, 0.5);
        AreEqual(new TFTriple { X = 0.5, Y = -0.5, Z = 0.25 }, _t.Divide(2.0), 1e-12, "div");
        _t.Init(-69.0, -51.0, -39.0);
        AreEqual(new TFTriple { X = 23.0, Y = 17.0, Z = 13.0 }, _t.Divide(-3.0), 1e-12, "div2");
    }

    [TestMethod]
    public void TestLenAndMax()
    {
        Assert.AreEqual(0.0, TFTriple.Abs(new TFTriple { X = 0, Y = 0, Z = 0 }), 1e-20);
        _t.Init(1.0, -1.0, 0.5);
        Assert.AreEqual(1.5, _t.GLen(), 1e-12);
        _t.Init(23.0, 17.0, 13.0);
        Assert.AreEqual(Math.Sqrt(818.0 + 169.0), _t.GLen(), 1e-12);
        _t.Init(13.0, 17.0, 11.0);
        Assert.AreEqual(Math.Sqrt(458.0 + 121.0), _t.GLen(), 1e-12);
        _t.Init(11.0, 13.0, 17.0);
        Assert.AreEqual(17.0, _t.MLen(), 1e-12);
        _t.Init(-13.0, -11.0, -17.0);
        Assert.AreEqual(17.0, _t.MLen(), 1e-12);
    }
}
