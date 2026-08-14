using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MVVM_38_CTDependencyInjection.Models.Tests;

[TestClass]
public class CSysTimeTests
{
    CSysTime testClass = new();

    [TestInitialize]
    public void Init()
    {
        testClass = new();
        Assert.IsNotNull(CSysTime.GetNow);
        CSysTime.GetNow = () => new DateTime(2021, 1, 1);
    }

    [TestMethod]
    public void GetTimeTest()
    {
        Assert.AreEqual(new DateTime(2021, 1, 1),testClass.Now);
    }
}
