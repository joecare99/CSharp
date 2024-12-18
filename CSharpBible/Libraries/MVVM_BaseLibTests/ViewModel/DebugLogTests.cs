using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM.ViewModel.Tests;

[TestClass]
public class DebugLogTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    DebugLog testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init() {
        testClass = new();
    }

    [TestMethod]
    public void SetupTest()
    {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass,typeof(DebugLog));
        Assert.IsInstanceOfType(testClass,typeof(IDebugLog));
        Assert.AreEqual("", (testClass as IDebugLog).DebugLog);
    }

    [TestMethod]
    public void DebugLogTest()
    {
        Assert.AreEqual("", (testClass as IDebugLog).DebugLog);
        testClass.DoLog("123");
        Assert.AreEqual("123\r\n", (testClass as IDebugLog).DebugLog);
    }

    [TestMethod]
    public void ClearLogTest()
    {
        testClass.ClearLog();
        Assert.AreEqual("", (testClass as IDebugLog).DebugLog);
        testClass.DoLog("123");
        Assert.AreEqual("123\r\n", (testClass as IDebugLog).DebugLog);
        testClass.ClearLog();
        Assert.AreEqual("", (testClass as IDebugLog).DebugLog);
    }

    [DataTestMethod]
    [DataRow(new[] { "" }, new[] { "\r\n" })]
    [DataRow(new[] { "","" }, new[] { "\r\n\r\n" })]
    [DataRow(new[] { "Peter" }, new[] { "Peter\r\n" })]
    [DataRow(new[] { "Peter","Haase" }, new[] { "Peter\r\nHaase\r\n" })]
    public void DoLogTest(string[] asVal, string[] asExp)
    {
        foreach (var line in asVal)
            testClass.DoLog(line);
        Assert.AreEqual(asExp[0], (testClass as IDebugLog).DebugLog);
    }
}
