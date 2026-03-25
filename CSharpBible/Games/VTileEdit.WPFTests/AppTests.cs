using System;
using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTileEdit.WPF;

namespace VTileEdit.WPF.Tests;

/// <summary>
/// Provides tests that verify the dependency injection setup of the WPF application.
/// </summary>
[TestClass]
public sealed class AppTests
{
    /// <summary>
    /// Resets the IoC delegates to a known state before each test.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        IoC.GetReqSrv = _ => throw new InvalidOperationException("IoC is not configured for the test.");
        IoC.GetSrv = _ => null;
        IoC.GetScope = () => throw new InvalidOperationException("IoC is not configured for the test.");
    }

    /// <summary>
    /// Ensures that <see cref="App.ConfigureServices(App)"/> registers the provided <see cref="App"/> instance.
    /// </summary>
    /// <remarks>
    /// The test verifies that the IoC helper resolves the same instance that was passed into the configuration routine.
    /// </remarks>
    [TestMethod]
    public void ConfigureServices_ShouldRegisterCurrentAppInstance()
    {
        var app = new App();

        var provider = App.ConfigureServices(app);

        Assert.IsNotNull(provider);
        var resolvedApp = IoC.GetRequiredService<App>();
        Assert.AreSame(app, resolvedApp);
    }
}