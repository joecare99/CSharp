using System;
using AA37_TreeView.Model;
using AA37_TreeView.ViewModels;
using AA37_TreeView.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless.MSTest;
using Microsoft.Extensions.DependencyInjection;

namespace AA37_TreeView.Tests;

[TestClass]
public class AppTests
{
    [TestMethod]
    public void BuildAvaloniaAppReturnsAppBuilderTest()
    {
        var result = AppBuilderFactory.BuildAvaloniaApp();

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(AppBuilder));
    }

    [TestMethod]
    public void BuildAvaloniaAppUsesConfigurableFactoryTest()
    {
        var originalFactory = AppBuilderFactory.GetAppBuilder;

        try
        {
            AppBuilderFactory.GetAppBuilder = static () => AppBuilder.Configure<App>();

            var result = AppBuilderFactory.BuildAvaloniaApp();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AppBuilder));
        }
        finally
        {
            AppBuilderFactory.GetAppBuilder = originalFactory;
        }
    }

    [AvaloniaTestMethod]
    public void OnFrameworkInitializationCompletedConfiguresServicesTest()
    {
        var app = new App();
        app.Initialize();
        app.ApplicationLifetime = new ClassicDesktopStyleApplicationLifetime();

        app.OnFrameworkInitializationCompleted();

        Assert.IsNotNull(app.Services);
        Assert.IsNotNull(app.Services.GetService(typeof(IBooksService)));
        Assert.IsNotNull(app.Services.GetService(typeof(MainWindowViewModel)));
        Assert.IsInstanceOfType(((ClassicDesktopStyleApplicationLifetime)app.ApplicationLifetime).MainWindow, typeof(MainWindow));
    }
}
