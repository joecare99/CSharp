using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_38_CTDependencyInjection.Models.Tests;

[TestClass]
public class CLogTests
{
    CLog testLog = new();

    [TestMethod()]
    public void LogTest()
    {
        testLog.Log("Test");
    }
}
