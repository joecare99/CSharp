using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GenFree.Sys.Tests;

[TestClass]
public class CSysTimeTests
{

    [TestMethod]
    public void NowTest()
    {
        Assert.IsTrue(new CSysTime().Now > DateTime.MinValue);
    }

    [TestMethod]
    public void DefaultTest()
    {
        Assert.AreEqual(default, new CSysTime().Default);
    }
}
