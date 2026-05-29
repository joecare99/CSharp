using Microsoft.VisualStudio.TestTools.UnitTesting;
using AA09_DialogBoxes.ViewModels;
using AA09_DialogBoxes.Views;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NSubstitute;
using System;

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
        Assert.IsNotNull(app.Services);
    }

    [TestMethod]
    public void AppTest3()
    {
        var browserApp = new TestApp
        {
            ApplicationLifetime = Substitute.For<ISingleViewApplicationLifetime>()
        };

        try
        {
            browserApp.DoFrameworkInitialization();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("ICursorFactory"))
        {
            Assert.Inconclusive($"Avalonia browser test environment unavailable: {ex.Message}");
        }

        Assert.IsNotNull(browserApp.Services);
        Assert.IsNotNull(browserApp.Services.GetService(typeof(MainWindowViewModel)));
        Assert.IsInstanceOfType(browserApp.ApplicationLifetime, typeof(ISingleViewApplicationLifetime));
        Assert.IsInstanceOfType(((ISingleViewApplicationLifetime)browserApp.ApplicationLifetime).MainView, typeof(BrowserMainView));
    }
}
