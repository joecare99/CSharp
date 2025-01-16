using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static MathLibrary.TwoDim.Math2d;

namespace MathLibrary.TwoDim.Tests;

/// <summary>
	/// Defines test class VectorTests.
	/// </summary>
	[TestClass()]
public class VectorTests
{

    [TestMethod()]
    public void VectorInitTest1()
    {
        var _v = new Vector();
        Assert.IsNotNull(_v);
        Assert.AreEqual(0, _v.x);
        Assert.AreEqual(0, _v.y);
    }

    [DataTestMethod()]
    [DataRow(0, 0, "( 0, 0)")]
    [DataRow(0, 1, "( 0, 1)")]
    [DataRow(1, 0, "( 1, 0)")]
    [DataRow(-1, 0, "( -1, 0)")]
    [DataRow(0, -1, "( 0, -1)")]
    [DataRow(1, -1, "( 1, -1)")]
    public void VectorInitTest2(double x,double y,string _)
    {
        var _v = new Vector(x,y);
        Assert.IsNotNull(_v);
        Assert.AreEqual(x, _v.x);
        Assert.AreEqual(y, _v.y);
    }

    /// <summary>
    /// Converts to stringtest2.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="exp">The exp.</param>
    [DataTestMethod()]
    [DataRow(0, 0, "( 0, 0)")]
    [DataRow(0, 1, "( 0, 1)")]
    [DataRow(1, 0, "( 1, 0)")]
    [DataRow(-1, 0, "( -1, 0)")]
    [DataRow(0, -1, "( 0, -1)")]
    [DataRow(1, -1, "( 1, -1)")]
    public void ToStringTest2(Double x, Double y, string exp)
    {
        var v1 = new Vector(x, y);
        Assert.AreEqual(exp, v1.ToString(), $"({x},{y}).ToString()");
    }

#if NET472_OR_GREATER
    /// <summary>
    /// Gets or sets the vector as a tuple.
    /// </summary>
    /// <value>The vector as a tuple.</value>
    [DataTestMethod()]
    [DataRow(0, 0, "( 0, 0)")]
    [DataRow(0, 1, "( 0, 1)")]
    [DataRow(1, 0, "( 1, 0)")]
    [DataRow(-1, 0, "( -1, 0)")]
    [DataRow(0, -1, "( 0, -1)")]
    [DataRow(1, -1, "( 1, -1)")]
    public void ValueTupleGetTest(Double x, Double y, string _)
    {
        var v1 = new Vector(x, y);
        Assert.AreEqual((x,y), v1.vTuple, $"({x},{y}).vTuple");
    }

    /// <summary>
    /// Gets or sets the vector as a tuple.
    /// </summary>
    /// <value>The vector as a tuple.</value>
    [DataTestMethod()]
    [DataRow(0, 0, "( 0, 0)")]
    [DataRow(0, 1, "( 0, 1)")]
    [DataRow(1, 0, "( 1, 0)")]
    [DataRow(-1, 0, "( -1, 0)")]
    [DataRow(0, -1, "( 0, -1)")]
    [DataRow(1, -1, "( 1, -1)")]
    public void ValueTupleSetTest(Double x, Double y, string _)
    {
        var v1 = new Vector();
        v1.vTuple = (x, y);
        Assert.AreEqual(x, v1.x, $"({x},{y}).x");
        Assert.AreEqual(y, v1.y, $"({x},{y}).y");
    }

#endif
    /// <summary>
    /// Gets or sets the vector as a tuple.
    /// </summary>
    /// <value>The vector as a tuple.</value>
    [DataTestMethod()]
    [DataRow(0, 0, "( 0, 0)")]
    [DataRow(0, 1, "( 0, 1)")]
    [DataRow(1, 0, "( 1, 0)")]
    [DataRow(-1, 0, "( -1, 0)")]
    [DataRow(0, -1, "( 0, -1)")]
    [DataRow(1, -1, "( 1, -1)")]
    public void ComplexGetTest(Double x, Double y, string _)
    {
        var v1 = new Vector(x, y);
        Assert.AreEqual(new System.Numerics.Complex(x,y), v1.AsComplex, $"({x},{y}).AsComplex");
    }

    /// <summary>
    /// Gets or sets the vector as a tuple.
    /// </summary>
    /// <value>The vector as a tuple.</value>
    [DataTestMethod()]
    [DataRow(0, 0, "( 0, 0)")]
    [DataRow(0, 1, "( 0, 1)")]
    [DataRow(1, 0, "( 1, 0)")]
    [DataRow(-1, 0, "( -1, 0)")]
    [DataRow(0, -1, "( 0, -1)")]
    [DataRow(1, -1, "( 1, -1)")]
    public void ComplexSetTest(Double x, Double y, string _)
    {
        var v1 = new Vector();
        v1.AsComplex = new System.Numerics.Complex(x, y);
        Assert.AreEqual(x, v1.x, $"({x},{y}).x");
        Assert.AreEqual(y, v1.y, $"({x},{y}).y");
    }

    /// <summary>
    /// Gets or sets the vector as a tuple.
    /// </summary>
    /// <value>The vector as a tuple.</value>
    [DataTestMethod()]
    [DataRow(0, 0, 0, 0, true)]
    [DataRow(0, 1, 0, 0, false)]
    [DataRow(1, 0, 0, 0, false)]
    [DataRow(-1, 0, 0, 0, false)]
    [DataRow(0, -1, 0, 0, false)]
    [DataRow(1, -1, 0, 0, false)]
    [DataRow(0, 0, 0, 1, false)]
    [DataRow(0, 1, 0, 1, true)]
    [DataRow(1, 0, 0, 1, false)]
    [DataRow(-1, 0, 0, 1, false)]
    [DataRow(0, -1, 0, 1, false)]
    [DataRow(1, -1, 0, 1, false)]
    [DataRow(0, 0, 1, 0, false)]
    [DataRow(0, 1, 1, 0, false)]
    [DataRow(1, 0, 1, 0, true)]
    [DataRow(-1, 0, 1, 0, false)]
    [DataRow(0, -1, 1, 0, false)]
    [DataRow(1, -1, 1, 0, false)]
    public void EqualsTest(Double x, Double y, Double x2, Double y2, bool xExp)
    {
        var v1 = new Vector(x,y);
        var v2 = new Vector(x2, y2);
        Assert.AreEqual(xExp, v1.Equals(v2), $"({x},{y}).Equals(({x2},{y2}))");
        Assert.AreEqual(xExp, v2.Equals(v1), $"({x2},{y2}).Equals(({x},{y}))");
    }

    [DataTestMethod()]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -4194304)]
    [DataRow(1, 0, 1072693248)]
    [DataRow(-1, 0, -1074790400)]
    [DataRow(0, -1, 4194302)]
    [DataRow(1, -1, 1070596094)]
    public void GetHashCodeTest(Double x, Double y, int iExp)
    {
        var v1 = new Vector(x, y);
        Assert.AreEqual(iExp, v1.GetHashCode(), $"({x},{y}).GetHashCode()");
    }

}
