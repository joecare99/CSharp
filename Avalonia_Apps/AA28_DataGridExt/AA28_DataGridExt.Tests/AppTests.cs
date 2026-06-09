using AA28_DataGridExt.Model;
using Avalonia;
using BaseLib.Helper;
using System;

namespace AA28_DataGridExt.Tests;

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
            AppBuilderFactory.GetAppBuilder = () => AppBuilder.Configure<App>();

            var result = AppBuilderFactory.BuildAvaloniaApp();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(AppBuilder));
        }
        finally
        {
            AppBuilderFactory.GetAppBuilder = originalFactory;
        }
    }

    [Avalonia.Headless.MSTest.AvaloniaTestMethod]
    public void OnFrameworkInitializationCompletedConfiguresPersonServiceTest()
    {
        var originalRequiredService = IoC.GetReqSrv;
        var originalService = IoC.GetSrv;

        try
        {
            var app = new App();
            app.Initialize();
            app.ApplicationLifetime = new Avalonia.Controls.ApplicationLifetimes.ClassicDesktopStyleApplicationLifetime();

            app.OnFrameworkInitializationCompleted();

            Assert.IsNotNull(app.Services);
            Assert.IsNotNull(IoC.GetReqSrv(typeof(IPersonService)));
        }
        finally
        {
            IoC.GetReqSrv = originalRequiredService;
            IoC.GetSrv = originalService;
        }
    }
}
