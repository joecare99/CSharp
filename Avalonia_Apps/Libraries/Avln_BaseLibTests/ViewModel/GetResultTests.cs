using Avalonia.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avalonia.ViewModels.Tests;

[TestClass]
public class GetResultTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    GetResult testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init() { 
        testClass = new();
    }

    [TestMethod]
    public void SetupTest() {
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass,typeof(GetResult));
        Assert.IsInstanceOfType(testClass,typeof(IGetResult));
        Assert.AreEqual(0,testClass.Count);
    }

    [TestMethod]
    public void RegisterTest()
    {
        testClass.Register("Test1",(o)=>$"Test1({string.Join(", ",o)})");
        Assert.AreEqual(1, testClass.Count);
        testClass.Register("Test2", (o) => $"Test2({string.Join(", ", o)})");
        Assert.AreEqual(2, testClass.Count);
        testClass.Register("Test3", (o) => null);
        Assert.AreEqual(3, testClass.Count);
        testClass.Register("Test3", (o) => $"Test3({string.Join(", ", o)})");
        Assert.AreEqual(3, testClass.Count);
    }

    [DataTestMethod]
    [DataRow("Test1", new[] {"Hello" }, "Test1(Hello)")]
    [DataRow("Test1", new[] { "Hello", "World" }, "Test1(Hello, World)")]
    [DataRow("Test1", new[] { "Hello", "World","!" }, "Test1(Hello, World, !)")]
    public void Test1(string _, object[] param, string? sExp) {
        Assert.IsNull(testClass.Get(param));
        RegisterTest();
        Assert.AreEqual(sExp,testClass.Get(param));
        Assert.AreEqual(3, testClass.Count);
    }

    [DataTestMethod]
    [DataRow("Test3", new[] { "Hello" }, "Test3(Hello)")]
    [DataRow("Test2", new[] { "Hello", "World" }, "Test2(Hello, World)")]
    [DataRow("Test1", new[] { "Hello", "new", "World" }, "Test1(Hello, new, World)")]
    public void GetTest(string name, object[] param, string? sExp)
    {
        Assert.IsNull(testClass.Get(param,name));
        RegisterTest();
        Assert.AreEqual(sExp, testClass.Get(param,name));
        Assert.AreEqual(3, testClass.Count);
    }

    [DataTestMethod]
    [DataRow(null, new[] { "Hello" }, null)]
    [DataRow("Test0", new[] { "Hello", "World" }, null)]
    [DataRow("", new[] { "Hello", "new", "World" }, null)]
    public void GetTest2(string name, object[] param, string? sExp)
    {
        Assert.IsNull(testClass.Get(param, name));
        RegisterTest();
        Assert.AreEqual(sExp, testClass.Get(param,name));
        Assert.AreEqual(3, testClass.Count);
    }
}
