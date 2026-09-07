using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia.Headless.MSTest;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA20_SysDialogs.Tests;

[TestClass()]
public class MainWindowTests
{
    [AvaloniaTestMethod]
    public void MainWindowTest()
    {
        var services = new ServiceCollection();
        services.AddSingleton<AA20_SysDialogs.ViewModels.SysDialogsViewModel>();
        typeof(AA20_SysDialogs.App).GetProperty(nameof(AA20_SysDialogs.App.Services))!.SetValue(null, services.BuildServiceProvider());

        var mw = new MainWindow();
        Assert.IsNotNull(mw);
        Assert.IsInstanceOfType(mw, typeof(MainWindow));
    }
}