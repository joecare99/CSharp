using Microsoft.VisualStudio.TestTools.UnitTesting;
using TranspilerLib.Models.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Models.Interpreter.Tests;

[TestClass()]
public class IECInterpreterTests
{
    [TestMethod()]
    public void InterpretTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    [DataRow(IECResWords.rw_ABS, 1, true, 1)]
    [DataRow(IECResWords.rw_ABS, -1, true, 1)]
    [DataRow(IECResWords.rw_ABS, 0, true, 0)]
    [DataRow(IECResWords.rw_ABS, -1.1, true, 1.1)]
    [DataRow(IECResWords.rw_ACOS, 0.0, true, Math.PI/2)]
    [DataRow(IECResWords.rw_ACOS, 1, true, 1.1)]
    [DataRow(IECResWords.rw_ASIN, 0, true, 1.1)]
    [DataRow(IECResWords.rw_ASIN, 0.5, true, Math.PI/6)]
    [DataRow(IECResWords.rw_ATAN, 0f, true, 0.0)]
    [DataRow(IECResWords.rw_ATAN, 0.5, true,  0.463647609)]
    [DataRow(IECResWords.rw_ATAN2, new object[] { 0.0, 1.0 }, true, 1.1)]
    [DataRow(IECResWords.rw_CONCAT, new object[] { "a", "b" }, true, "ab")]
    [DataRow(IECResWords.rw_CONCAT, new object[] { "a", "b", "c" }, true, "abc")]
    [DataRow(IECResWords.rw_COS, 0f, true, 1.0)]
    [DataRow(IECResWords.rw_COS, Math.PI, true, -1.0)]
    [DataRow(IECResWords.rw_DIV, new object[] { 5, 2 }, true, 2)]
    [DataRow(IECResWords.rw_DIV, new object[] { 5, 3 }, true, 1)]
    [DataRow(IECResWords.rw_EXP, 0f, true, 1.0)]
    [DataRow(IECResWords.rw_EXP, 1f, true, Math.E)]
    [DataRow(IECResWords.rw_INT, 1.1, true, 1d)]
    [DataRow(IECResWords.rw_INT, -1.1, true, -2d)]
    [DataRow(IECResWords.rw_LEN, "abc", true, 3)]
    [DataRow(IECResWords.rw_LN, 1f, true, 0.0)]
    [DataRow(IECResWords.rw_LN, Math.E, true, 1.0)]
    [DataRow(IECResWords.rw_LOG, 1f, true, 0.0)]
    [DataRow(IECResWords.rw_LOG, 10f, true, 1.0)]
    [DataRow(IECResWords.rw_MOD, new object[] { 5, 2 }, true, 1)]
    [DataRow(IECResWords.rw_MOD, new object[] { 5, 3 }, true, 2)]
    [DataRow(IECResWords.rw_SIN, 0f, true, 0.0)]
    [DataRow(IECResWords.rw_SIN, Math.PI, true, 0.0)]
    [DataRow(IECResWords.rw_SQRT, 0f, true, 0.0)]
    [DataRow(IECResWords.rw_SQRT, 4f, true, 2.0)]
    [DataRow(IECResWords.rw_TO_STRING, 1, true, "1")]
    [DataRow(IECResWords.rw_TO_STRING, 1.1, true, "1,1")]
    [DataRow(IECResWords.rw_TAN, 0f, true, 0.0)]
    [DataRow(IECResWords.rw_TAN, Math.PI, true, 0.0)]
    [DataRow(IECResWords.rw_TRUNC, 1.1, true, 1.0)]
    [DataRow(IECResWords.rw_TRUNC, -1.1, true, -1.0)]
    public void SystemfunctionsTest(Enum eAct, object value,bool xExp,object exp)
    {
        Assert.AreEqual(xExp, IECInterpreter.systemfunctions.TryGetValue(eAct,out var methods));
        if (value is object[] values)
        {
            var m = methods.First(m => (values.Count() == m?.GetParameters().Count()) && m.GetParameters().First().ParameterType.IsAssignableFrom(values[0].GetType()));
            if (m.ReturnType.IsAssignableTo(typeof(double)))
            Assert.AreEqual((double)exp, (double)m?.Invoke(null, values),1e-7d);
            else
            Assert.AreEqual(exp, m?.Invoke(null, values));
        }
        else
        {
            var m = methods.First(m => m?.GetParameters().First().ParameterType.IsAssignableFrom(value.GetType())??false);
            if (m.ReturnType.IsAssignableTo(typeof(double)))
                Assert.AreEqual((double)exp, (double)m?.Invoke(null, [value]),1e-7);
            else
                Assert.AreEqual(exp, m?.Invoke(null, [value]));
        }
    }

}