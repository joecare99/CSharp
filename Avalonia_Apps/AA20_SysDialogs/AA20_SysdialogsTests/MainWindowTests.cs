using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA20_SysDialogs.Tests;

[TestClass()]
public class MainWindowTests
{
    [TestMethod()]
    public void MainWindowTest()
    {
        var mw = new MainWindow();
        Assert.IsNotNull(mw);
        Assert.IsInstanceOfType(mw, typeof(MainWindow));
    }
}