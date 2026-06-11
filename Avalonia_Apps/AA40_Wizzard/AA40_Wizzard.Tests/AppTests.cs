using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using AA40_Wizzard.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace AA40_Wizzard.Tests;

[TestClass]
public class AppTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
        => TestAppBuilder.EnsureInitialized();

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

    [TestMethod]
    public void OnFrameworkInitializationCompletedConfiguresServicesTest()
    {
        var app = new App();
        app.Initialize();
        app.ApplicationLifetime = new ClassicDesktopStyleApplicationLifetime();

        app.OnFrameworkInitializationCompleted();

        Assert.IsNotNull(app.Services);
        Assert.IsNotNull(app.Services.GetService(typeof(IWizzardModel)));
        Assert.IsNotNull(app.Services.GetService(typeof(MainWindowViewModel)));
        Assert.IsInstanceOfType(((ClassicDesktopStyleApplicationLifetime)app.ApplicationLifetime).MainWindow, typeof(MainWindow));
    }
}
