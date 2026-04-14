using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA09_DialogBoxes.Tests;

internal class TestApp : App
{
    public void DoFrameworkInitialization()
    {
        OnFrameworkInitializationCompleted();
    }
}
[TestClass()]
public class AppTests 
{
    static TestApp app = new();

    [TestMethod]
    public void AppTest()
    {
        Assert.IsNotNull(app);
    }

    [TestMethod]
    public void AppTest2()
    {
        app.DoFrameworkInitialization();
        Assert.IsNotNull(app);
    }
}
