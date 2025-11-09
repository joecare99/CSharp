using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using Avalonia.ViewModels;

namespace AA16_UserControl1.ViewModels.Tests;

[TestClass()]
public class MainWindowViewModelTests
{
    MainWindowViewModel testModel;

    [TestInitialize]
    public void Init()
    {
        testModel = new(null);
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}