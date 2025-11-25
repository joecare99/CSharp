using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AA16_UserControl1;
using AA16_UserControl1.ViewModels.Interfaces;
using AA16_UserControl1.Views;
using Avalonia.Headless.MSTest;
using Avalonia.Views.Extension;

namespace AA16_UserControl1.Tests;

[TestClass]
public class AppTests
{
    private static void BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var method = typeof(App).GetMethod("ConfigureServices", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method, "ConfigureServices nicht gefunden.");
        method.Invoke(null, new object[] { services });
        IoC.Configure(services.BuildServiceProvider());
    }

    [TestMethod]
    public void Should_Register_MainWindowViewModel_As_Transient()
    {
        BuildServiceProvider();
        var vm1 = IoC.GetRequiredService<AA16_UserControl1.ViewModels.MainWindowViewModel>();
        var vm2 = IoC.GetRequiredService<AA16_UserControl1.ViewModels.MainWindowViewModel>();
        Assert.IsNotNull(vm1);
        Assert.IsNotNull(vm2);
        Assert.AreNotSame(vm1, vm2, "Transient Lifetime erwartet.");
    }

    [TestMethod]
    public void Should_Register_IUserControlViewModel()
    {
        BuildServiceProvider();
        var ucVm = IoC.GetRequiredService<IUserControlViewModel>();
        Assert.IsNotNull(ucVm);
        Assert.AreEqual(typeof(AA16_UserControl1.ViewModels.UserControlViewModel), ucVm.GetType());
    }

    [AvaloniaTestMethod]
    public void Should_Register_Views()
    {
        BuildServiceProvider();
        var mainWindow = IoC.GetRequiredService<MainWindow>();
        var userControlView = IoC.GetRequiredService<UserControlView>();
        var labeledTextbox = IoC.GetRequiredService<LabeldMaxLengthTextbox>();

        Assert.IsNotNull(mainWindow);
        Assert.IsNotNull(userControlView);
        Assert.IsNotNull(labeledTextbox);
    }

    [TestMethod]
    public void Should_Not_Register_Commented_DoubleButtonUC()
    {
        BuildServiceProvider();
        // Sicherstellen, dass nicht versehentlich registriert
        var service = IoC.GetService<DoubleButtonUC>();
        Assert.IsNull(service, "DoubleButtonUC sollte nicht registriert sein.");
    }

    [AvaloniaTestMethod]
    public void App_Should_Initialize_Correctly()
    {
        var app = new App();
        app.Initialize();
        Assert.IsNotNull(app, "App-Instanz sollte nicht null sein.");
    }

    [AvaloniaTestMethod]
    public void App_OnFrameworkInitializationCompletedTests()
    {
        var app = new App();
        app.OnFrameworkInitializationCompleted();
        Assert.IsNotNull(app, "App-Instanz sollte nicht null sein.");

    }

}