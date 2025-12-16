using AA18_MultiConverter.ViewModels.Interfaces;
using Avalonia.Headless.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading;

namespace AA18_MultiConverter.Views.Tests;

[TestClass()]
public class MainWindowTests
{
    MainWindow? testView;

    [TestInitialize]
    public void TestInitialize()
    {
        var services = typeof(App).GetProperty(nameof(App.Services));
        services.SetValue(null, Substitute.For<IServiceProvider>());
        App.Services.GetService(typeof(IDateDifViewModel)).Returns(Substitute.For<IDateDifViewModel>());
    }

    [AvaloniaTestMethod()]
    public void MainWindowTest()
    {
        testView = new(null);
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(MainWindow));
    }
}