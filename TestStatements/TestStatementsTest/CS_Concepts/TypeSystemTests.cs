using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace TestStatements.CS_Concepts.Tests;

[TestClass]
public class TypeSystemTests : ConsoleTestsBase
{
    [TestMethod()]
    public void UseOtfTypesTest()
    {
        var result = GetConsoleOutput(TypeSystem.UseOfTypes);
        Assert.IsTrue(result.Contains("System.Int32 a = 5"));
        Assert.IsTrue(result.Contains("System.Int32 b = 7"));
        Assert.IsTrue(result.Contains("System.Boolean test = True"));
    }

    [TestMethod()]
    public void DelareVariablesTest()
    {
        var result = GetConsoleOutput(TypeSystem.DelareVariables);
        Assert.IsTrue(result.Contains("System.Char firstLetter = C"));
        Assert.IsTrue(result.Contains("query ="));
        Assert.IsTrue(result.Contains("item = 100"));
    }

    [TestMethod()]
    public void ValueTypes1Test()
    {
        var result = GetConsoleOutput(TypeSystem.ValueTypes1);
        Assert.IsTrue(result.Contains("System.Byte b = 255"));
    }
}
