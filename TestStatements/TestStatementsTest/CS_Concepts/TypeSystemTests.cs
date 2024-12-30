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
    private readonly string cExpListMain = @"";
    private readonly string cExpDelareVariables = @"";
    private readonly string cExpValueTypes1 = @"";

    [TestMethod()]
    public void UseOtfTypesTest()
    {
        AssertConsoleOutput(cExpListMain, TypeSystem.UseOfTypes);
    }

    [TestMethod()]
    public void DelareVariablesTest()
    {
        AssertConsoleOutput(cExpDelareVariables, TypeSystem.DelareVariables);
    }

    [TestMethod()]
    public void ValueTypes1Test()
    {
        AssertConsoleOutput(cExpValueTypes1, TypeSystem.ValueTypes1);
    }
}
