using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using Avalonia.Controls.ApplicationLifetimes;
using Avln_CommonDialogs.Base.Interfaces;
using Code.Navigation;
using Config.Service;
using Config.UI;
using Config.UI.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Project.Explorer;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        var documentHost = serviceProvider.GetRequiredService<IEditorDocumentHost>();
        var caretService = serviceProvider.GetRequiredService<IEditorCaretService>();
        var navigator = serviceProvider.GetRequiredService<ICodeNavigator>();
        var diagnostics = serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>();
        var projectExplorer = serviceProvider.GetRequiredService<ProjectExplorerViewModel>();

        Assert.IsNotNull(viewModelFactory);
        Assert.IsNotNull(workflowFactory);
        Assert.IsNotNull(dialogService);
        Assert.IsNotNull(planningExplorer);
        Assert.IsNotNull(documentHost);
        Assert.IsNotNull(caretService);
        Assert.IsNotNull(navigator);
        Assert.IsNotNull(projectExplorer);
        diagnostics.SelectedDiagnostic = new AppKomponentBaseLib.Diagnostics.Diagnostic
        {
            SourcePath = Path.GetTempFileName(),
            LineNumber = 1,
            ColumnNumber = 1,
        };

        Assert.IsNotNull(diagnostics);
        Assert.IsTrue(diagnostics.IsSelectedDiagnosticNavigable);
    }

    [TestMethod]
    public async Task CreateServiceProvider_ProjectExplorerUsesCodeNavigationForFiles()
    {
        ICodeNavigator navigator = Substitute.For<ICodeNavigator>();
        ServiceCollection services = new();
        App.ConfigureServices(services, static () => null);
        services.AddSingleton(navigator);
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        IProjectExplorerItemOpener opener = serviceProvider.GetRequiredService<IProjectExplorerItemOpener>();
        ProjectExplorerItem file = new(
            "file:sample",
            "sample.cs",
            Path.GetTempFileName(),
            ProjectExplorerItemKind.File);

        await opener.OpenAsync(file);

        await navigator.Received(1).NavigateAsync(
            Arg.Is<CodeLocation>(location => location.Path == file.Path),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that CodeStudio registers only its host-owned section and a
    /// storage identity distinct from RnzTrauer.
    /// </summary>
    [TestMethod]
    public void CreateServiceProvider_RegistersIsolatedConfigUi()
    {
        using var serviceProvider = App.CreateServiceProvider(static () => null);

        ConfigService configService = serviceProvider.GetRequiredService<ConfigService>();
        IConfigUiSectionRegistry registry = serviceProvider.GetRequiredService<IConfigUiSectionRegistry>();

        Assert.AreEqual("JC-Soft", configService.VendorName);
        Assert.AreEqual("AA98-CodeStudio", configService.ApplicationName);
        Assert.AreEqual(1, registry.Sections.Count);
        Assert.AreEqual("Workbench", registry.Sections[0].Section.Name);
        Assert.IsNotNull(serviceProvider.GetRequiredService<ConfigUiViewModel>());
    }

    /// <summary>
    /// Verifies that the workbench composition injects the navigator into the
    /// diagnostics consumer and forwards activation to its editor services.
    /// </summary>
    [TestMethod]
    public async Task CreateServiceProvider_DiagnosticActivationUsesRegisteredEditorNavigator()
    {
        var documentHost = Substitute.For<IEditorDocumentHost>();
        var caretService = Substitute.For<IEditorCaretService>();
        var document = Substitute.For<IEditorDocument>();
        var location = new CodeLocation(Path.GetTempFileName(), 2, 3);
        documentHost.OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>()).Returns(document);

        ServiceCollection services = new();
        App.ConfigureServices(services, static () => null);
        services.AddSingleton(documentHost);
        services.AddSingleton(caretService);
        using var serviceProvider = services.BuildServiceProvider();

        var diagnostics = serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>();

        await diagnostics.ActivateAsync(new AppKomponentBaseLib.Diagnostics.Diagnostic
        {
            SourcePath = location.Path,
            LineNumber = location.Line,
            ColumnNumber = location.Column,
        });

        await documentHost.Received(1).OpenOrActivateAsync(location.Path, Arg.Any<CancellationToken>());
        await caretService.Received(1).FocusAndMoveCaretAsync(
            document,
            Arg.Is<CodeLocation>(candidate => candidate == location),
            Arg.Any<CancellationToken>());
        Assert.AreEqual(string.Empty, diagnostics.ActivationErrorText);
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