using Microsoft.VisualStudio.TestTools.UnitTesting;

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
