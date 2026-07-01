using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia.Controls.ApplicationLifetimes;
using Avln_CommonDialogs.Base.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace AA98_AvlnCodeStudio.Tests.Startup;

/// <summary>
/// Verifies the AA98 shell startup composition behavior.
/// </summary>
[TestClass]
public class AppStartupCompositionTests
{
    /// <summary>
    /// Verifies that the application service provider can be created without a realized desktop window.
    /// </summary>
    [TestMethod]
    public void CreateServiceProvider_RegistersCoreStartupServices()
    {
        using var serviceProvider = App.CreateServiceProvider(static () => null);

        var viewModelFactory = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.UI.ViewModels.IEditorViewModelFactory>();
        var workflowFactory = serviceProvider.GetRequiredService<AA98_AvlnCodeStudio.Editor.Services.IEditorWorkflowFactory>();
        var dialogService = serviceProvider.GetRequiredService<IOpenFileDialog>();
        var planningExplorer = serviceProvider.GetRequiredService<PlanningExplorerViewModel>();

        Assert.IsNotNull(viewModelFactory);
        Assert.IsNotNull(workflowFactory);
        Assert.IsNotNull(dialogService);
        Assert.IsNotNull(planningExplorer);
    }

    /// <summary>
    /// Verifies that service-provider creation rejects a missing top-level provider delegate.
    /// </summary>
    [TestMethod]
    public void CreateServiceProvider_RejectsMissingTopLevelProvider()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => App.CreateServiceProvider(null!));
    }

    /// <summary>
    /// Verifies that desktop initialization wraps main-window creation failures with actionable diagnostics.
    /// </summary>
    [TestMethod]
    public void InitializeDesktop_WhenUiCreationFails_ThrowsStartupDiagnostic()
    {
        var app = new App();
        var desktop = Substitute.For<IClassicDesktopStyleApplicationLifetime>();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => app.InitializeDesktop(desktop));

        Assert.IsNotNull(app.ServiceProvider);
        StringAssert.Contains(exception.Message, "AA98 shell startup failed during desktop initialization");
        Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
    }

    /// <summary>
    /// Verifies that desktop initialization rejects a missing desktop lifetime argument.
    /// </summary>
    [TestMethod]
    public void InitializeDesktop_RejectsMissingDesktopLifetime()
    {
        var app = new App();

        Assert.ThrowsExactly<ArgumentNullException>(() => app.InitializeDesktop(null!));
    }

    /// <summary>
    /// Verifies that main window creation reports a focused startup error when required registrations are missing.
    /// </summary>
    [TestMethod]
    public void CreateMainWindow_WhenRegistrationMissing_ThrowsStartupDiagnostic()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => App.CreateMainWindow(serviceProvider));

        StringAssert.Contains(exception.Message, "AA98 main window creation failed");
        Assert.IsNotNull(exception.InnerException);
    }

    /// <summary>
    /// Verifies that main-window creation rejects a missing service provider argument.
    /// </summary>
    [TestMethod]
    public void CreateMainWindow_RejectsMissingServiceProvider()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => App.CreateMainWindow(null!));
    }
}