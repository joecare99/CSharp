using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary.Common.Tests;

[TestClass]
public class CommonMathTests
{
    [TestMethod]
    [DataRow(float.MinValue,float.MaxValue,false)]
    [DataRow(float.MinValue,float.MinValue,false)]
    [DataRow(float.MaxValue,float.MinValue,true)]
    [DataRow(float.Epsilon,float.MinValue,true)]
    [DataRow(float.Epsilon,float.NegativeInfinity,true)]
    [DataRow(default(float),-float.Epsilon,true)]
    [DataRow(default(float),float.NaN,false)]
    [DataRow(float.NaN,float.NaN,false)]
    [DataRow(float.NaN,float.NegativeInfinity,false)]
    public void TestFloatNumbersGt(float fAct1,float fAct,bool xExp)
    {
        Assert.AreEqual(xExp, fAct1 > fAct);
    }

    [TestMethod]
    [DataRow(float.MinValue, float.MaxValue, false)]
    [DataRow(float.MinValue, float.MinValue, true)]
    [DataRow(float.MaxValue, float.MinValue, true)]
    [DataRow(float.Epsilon, float.MinValue, true)]
    [DataRow(float.Epsilon, float.NegativeInfinity, true)]
    [DataRow(default(float), -float.Epsilon, true)]
    [DataRow(default(float), float.NaN, false)]
    [DataRow(float.NaN, float.NaN, false)]
    [DataRow(float.NaN, float.NegativeInfinity, false)]
    public void TestFloatNumbersGtE(float fAct1, float fAct, bool xExp)
    {
        Assert.AreEqual(xExp, fAct1 >= fAct);
    }

    [TestMethod]
    [DataRow(float.MinValue, float.MaxValue, false)]
    [DataRow(float.MinValue, float.MinValue, true)]
    [DataRow(float.MaxValue, float.MinValue, false)]
    [DataRow(float.Epsilon, float.MinValue, false)]
    [DataRow(float.Epsilon, float.NegativeInfinity, false)]
    [DataRow(default(float), -float.Epsilon, false)]
    [DataRow(default(float), float.NaN, false)]
    [DataRow(float.NaN, float.NaN, false)]
    [DataRow(float.NaN, float.NegativeInfinity, false)]
    public void TestFloatNumbersEq(float fAct1, float fAct, bool xExp)
    {
        Assert.AreEqual(xExp, fAct1 == fAct);
    }
}
