using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace AA18_MultiConverter.Views.Tests;

[TestClass()]
public class MainWindowTests
{
    MainWindow? testView;

    [TestMethod()]
    public void MainWindowTest()
    {
        testView = null;
        var t = new Thread(() => testView = new(null));
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(MainWindow));
    }
}