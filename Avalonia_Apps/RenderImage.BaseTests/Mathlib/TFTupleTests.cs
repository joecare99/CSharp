using MathLibrary.RenderImage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace RenderImage.BaseTests.Mathlib;

[TestClass]
public class TFTupleTests
{
    private TFTuple _t;
    private static readonly CultureInfo Invar = CultureInfo.InvariantCulture;

    [TestInitialize]
    public void SetUp()
    {
        _t.Init(0, 0);
    }

    private static void AreEqual(TFTuple exp, TFTuple act, double eps, string msg)
    {
        Assert.AreEqual(exp.X, act.X, eps, msg + "[X]");
        Assert.AreEqual(exp.Y, act.Y, eps, msg + "[Y]");
    }

    [TestMethod]
    public void TestSetUp()
    {
        Assert.AreEqual(0.0, _t.X, 1e-20, "_t.x =0");
        Assert.AreEqual(0.0, _t.Y, 1e-20, "_t.y =0");
    }

    [TestMethod]
    public void TestToString()
    {
        Assert.AreEqual("<0.00;0.00>", _t.ToString());
        Assert.AreEqual("<0.00;0.00>", TFTuple.Zero.ToString());
        _t.Init(1.0, -1.0);
        Assert.AreEqual("<1.00; -1.00>", _t.ToString());
        Assert.AreEqual(1.0, _t[0], 1e-20);
        Assert.AreEqual(-1.0, _t[1], 1e-20);
        _t.Init(23.0, 17.0);
        Assert.AreEqual("<23.00;17.00>", _t.ToString());
        var vfs = (NumberFormatInfo)Invar.NumberFormat.Clone();
        vfs.NumberDecimalSeparator = ".";
        var rnd = new Random(1234);
        for (int i = 0; i < 2000; i++)
        {
            var x1 = (rnd.NextDouble() - 0.5) * int.MaxValue;
            var y1 = (rnd.NextDouble() - 0.5) * int.MaxValue;
            _t.Init(x1, y1);
            var exp = $"<{x1.ToString("F2", Invar)}; {y1.ToString("F2", Invar)}>";
        Assert.AreEqual(exp, _t.ToString(), $"init({x1},{y1})");
        Assert.AreEqual(x1, _t.X, 1e-12);
        Assert.AreEqual(y1, _t.Y, 1e-12);
    }
 }

 [TestMethod]
    public void TestInit()
    {
        AreEqual(new TFTuple { X = 0, Y = 0 }, TFTuple.Zero, 1e-20, "ZeroTup");
        _t.Init(0, 0);
        AreEqual(new TFTuple { X = 0, Y = 0 }, _t, 1e-20, "init(0,0)");
        _t.Init(1.0, -1.0);
        AreEqual(new TFTuple { X = 1.0, Y = -1.0 }, _t, 1e-20, "init(1.0,-1.0)");
        Assert.AreEqual(1.0, _t[0], 1e-20);
        Assert.AreEqual(-1.0, _t[1], 1e-20);
        _t.Init(23.0, 17.0);
        AreEqual(new TFTuple { X = 23.0, Y = 17.0 }, _t, 1e-20, "init(23.0,17.0)");
        Assert.AreEqual(23.0, _t[0], 1e-20);
        Assert.AreEqual(17.0, _t[1], 1e-20);
        var rnd = new Random(1234);
        for (int i = 0; i < 2000; i++)
        {
            var x1 = (rnd.NextDouble() - 0.5) * int.MaxValue;
            var y1 = (rnd.NextDouble() - 0.5) * int.MaxValue;
            _t.Init(x1, y1);
            AreEqual(new TFTuple { X = x1, Y = y1 }, _t, 1e-12, $"init({x1},{y1})");
            Assert.AreEqual(x1, _t[0], 1e-12);
            Assert.AreEqual(y1, _t[1], 1e-12);
        }
    }

    [TestMethod]
    public void TestInitLenDir()
    {
        _t.InitLenDir(0, 0);
        AreEqual(TFTuple.Zero, _t, 1e-20, "InitLenDir(0,0)");
        _t.InitLenDir(1.0, 0.0);
        AreEqual(new TFTuple { X = 1.0, Y = 0.0 }, _t, 1e-12, "InitLenDir(1,0)");
        _t.InitLenDir(1.0, Math.PI / 6);
        AreEqual(new TFTuple { X = Math.Sqrt(3.0 / 4.0), Y = 0.5 }, _t, 1e-12, "30°");
        _t.InitLenDir(Math.Sqrt(2), Math.PI / 4);
        AreEqual(new TFTuple { X = 1.0, Y = 1.0 }, _t, 1e-12, "45°");
        _t.InitLenDir(1.0, Math.PI / 3);
        AreEqual(new TFTuple { X = 0.5, Y = Math.Sqrt(3.0 / 4.0) }, _t, 1e-12, "60°");
        _t.InitLenDir(1.0, Math.PI / 2);
        AreEqual(new TFTuple { X = 0.0, Y = 1.0 }, _t, 1e-12, "90°");
        _t.InitLenDir(1.0, 2 * Math.PI / 3);
        AreEqual(new TFTuple { X = -0.5, Y = Math.Sqrt(3.0 / 4.0) }, _t, 1e-12, "120°");
        _t.InitLenDir(Math.Sqrt(2), 3 * Math.PI / 4);
        AreEqual(new TFTuple { X = -1.0, Y = 1.0 }, _t, 1e-12, "135°");
        _t.InitLenDir(1.0, 5 * Math.PI / 6);
        AreEqual(new TFTuple { X = -Math.Sqrt(3.0 / 4.0), Y = 0.5 }, _t, 1e-12, "150°");
        _t.InitLenDir(1.0, Math.PI);
        AreEqual(new TFTuple { X = -1.0, Y = 0.0 }, _t, 1e-12, "180°");
        _t.InitLenDir(1.0, -5 * Math.PI / 6);
        AreEqual(new TFTuple { X = -Math.Sqrt(3.0 / 4.0), Y = -0.5 }, _t, 1e-12, "-150°");
        _t.InitLenDir(Math.Sqrt(2.0), -3 * Math.PI / 4);
        AreEqual(new TFTuple { X = -1.0, Y = -1.0 }, _t, 1e-12, "-135°");
        _t.InitLenDir(1.0, -2 * Math.PI / 3);
        AreEqual(new TFTuple { X = -0.5, Y = -Math.Sqrt(3.0 / 4.0) }, _t, 1e-12, "-120°");
        _t.InitLenDir(1.0, -Math.PI / 2);
        AreEqual(new TFTuple { X = 0.0, Y = -1.0 }, _t, 1e-12, "-90°");
        _t.InitLenDir(1.0, -Math.PI / 3);
        AreEqual(new TFTuple { X = 0.5, Y = -Math.Sqrt(3.0 / 4.0) }, _t, 1e-12, "-60°");
        _t.InitLenDir(Math.Sqrt(2.0), -Math.PI / 4);
        AreEqual(new TFTuple { X = 1.0, Y = -1.0 }, _t, 1e-12, "-45°");
        _t.InitLenDir(1.0, -Math.PI / 6);
        AreEqual(new TFTuple { X = Math.Sqrt(3.0 / 4.0), Y = -0.5 }, _t, 1e-12, "-30°");

        _t.InitLenDir(Math.Sqrt(818.0), 0.636508215787951);
        AreEqual(new TFTuple { X = 23.0, Y = 17.0 }, _t, 1e-12, "(23,17)");
        _t.InitLenDir(Math.Sqrt(818.0), Math.PI - 0.636508215787951);
        AreEqual(new TFTuple { X = -23.0, Y = 17.0 }, _t, 1e-12, "(-23,17)");
        _t.InitLenDir(Math.Sqrt(458.0), 0.917949695694122);
        AreEqual(new TFTuple { X = 13.0, Y = 17.0 }, _t, 1e-12, "(13,17)");
        _t.InitLenDir(Math.Sqrt(458.0), -Math.PI + 0.917949695694122);
        AreEqual(new TFTuple { X = -13.0, Y = -17.0 }, _t, 1e-12, "(-13,-17)");
    }
}
