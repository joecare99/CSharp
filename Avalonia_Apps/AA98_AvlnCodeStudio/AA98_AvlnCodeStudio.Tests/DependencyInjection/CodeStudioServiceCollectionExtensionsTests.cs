using AA98_AvlnCodeStudio.Base.AI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.AI.Services;
using AA98_AvlnCodeStudio.Base.Building.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Debugging.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AA98_AvlnCodeStudio.Base.OS.DependencyInjection;
using AA98_AvlnCodeStudio.Base.OS.Models;
using AA98_AvlnCodeStudio.Base.OS.Services;
using AA98_AvlnCodeStudio.Base.Testing.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Base.Versioning.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Diagnostics.Debug.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
#if NET10_0
using AA98_AvlnCodeStudio.Versioning.Git.DependencyInjection;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
#endif
using AA98_AvlnCodeStudio.UI.DependencyInjection;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Planning.Core.Services;

namespace AA98_AvlnCodeStudio.Tests.DependencyInjection;

/// <summary>
/// Verifies the shared DI registration baseline for Code Studio foundation services.
/// </summary>
[TestClass]
public class CodeStudioServiceCollectionExtensionsTests
{
    /// <summary>
    /// Verifies that the default AI registration adds the fallback AI services.
    /// </summary>
    [TestMethod]
    public void AddCodeStudioAI_RegistersFallbackServices()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioAI();

        using var serviceProvider = services.BuildServiceProvider();
        Assert.IsInstanceOfType<NullAIClientFactory>(serviceProvider.GetRequiredService<IAIClientFactory>());
        Assert.IsInstanceOfType<NullAIClient>(serviceProvider.GetRequiredService<IAIClient>());
    }

    /// <summary>
    /// Verifies that the default engineering registrations add fallback services.
    /// </summary>
    [TestMethod]
    public void EngineeringRegistrations_RegisterFallbackServices()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioBuilding();
        services.AddCodeStudioVersioning();
        services.AddCodeStudioTesting();
        services.AddCodeStudioDebugging();

        using var serviceProvider = services.BuildServiceProvider();
        Assert.IsInstanceOfType<NullCodeStudioBuilderService>(serviceProvider.GetRequiredService<ICodeStudioBuilderService>());
        Assert.IsInstanceOfType<NullVersionControlService>(serviceProvider.GetRequiredService<IVersionControlService>());
        Assert.IsInstanceOfType<NullTestExecutionService>(serviceProvider.GetRequiredService<ITestExecutionService>());
        Assert.IsInstanceOfType<NullDebugSessionService>(serviceProvider.GetRequiredService<IDebugSessionService>());
    }

    /// <summary>
    /// Verifies that environment-bound OS and UI registrations use the configured implementation types.
    /// </summary>
    [TestMethod]
    public void EnvironmentRegistrations_RegisterConfiguredImplementationTypes()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioOS<DesignTextDocumentStorageService>();
        services.AddCodeStudioUI<DesignEditorFileDialogService>();

        using var serviceProvider = services.BuildServiceProvider();
        Assert.IsInstanceOfType<DesignTextDocumentStorageService>(serviceProvider.GetRequiredService<ITextDocumentStorageService>());
        Assert.IsInstanceOfType<NullTerminalShellResolver>(serviceProvider.GetRequiredService<ITerminalShellResolver>());
        Assert.IsInstanceOfType<NullTerminalSessionService>(serviceProvider.GetRequiredService<ITerminalSessionService>());
        Assert.IsInstanceOfType<DesignEditorFileDialogService>(serviceProvider.GetRequiredService<IEditorFileDialogService>());
    }

    /// <summary>
    /// Verifies that the default terminal registrations add fallback services.
    /// </summary>
    [TestMethod]
    public async Task AddCodeStudioTerminal_RegistersFallbackServices()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioTerminal();

        using var serviceProvider = services.BuildServiceProvider();
        var shellResolver = serviceProvider.GetRequiredService<ITerminalShellResolver>();
        var sessionService = serviceProvider.GetRequiredService<ITerminalSessionService>();
        Assert.IsInstanceOfType<NullTerminalShellResolver>(shellResolver);
        Assert.IsInstanceOfType<NullTerminalSessionService>(sessionService);

        var session = await sessionService.StartSessionAsync(new TerminalSessionStartRequest());
        Assert.AreEqual(TerminalSessionState.Running, session.State);
        Assert.IsTrue(session.Shell.IsFallback);
        await session.DisposeAsync();
    }

    /// <summary>
    /// Verifies that the fallback shell resolver preserves explicit request configuration.
    /// </summary>
    [TestMethod]
    public async Task AddCodeStudioTerminal_FallbackShellPreservesRequestedConfiguration()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioTerminal();

        using var serviceProvider = services.BuildServiceProvider();
        var shellResolver = serviceProvider.GetRequiredService<ITerminalShellResolver>();
        var request = new TerminalSessionStartRequest
        {
            ShellPath = "/bin/bash",
            ShellDisplayName = "bash",
        };
        request.Arguments.Add("-l");
        request.Arguments.Add("-i");

        var shell = await shellResolver.ResolveShellAsync(request);

        Assert.AreEqual("bash", shell.DisplayName);
        Assert.AreEqual("/bin/bash", shell.ExecutablePath);
        CollectionAssert.AreEqual(new[] { "-l", "-i" }, shell.Arguments.ToArray());
        Assert.IsTrue(shell.IsFallback);
    }

    /// <summary>
    /// Verifies that the fallback terminal session can be stopped deterministically.
    /// </summary>
    [TestMethod]
    public async Task AddCodeStudioTerminal_FallbackSessionStopRaisesExitAndStopsSession()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioTerminal();

        using var serviceProvider = services.BuildServiceProvider();
        var sessionService = serviceProvider.GetRequiredService<ITerminalSessionService>();
        var session = await sessionService.StartSessionAsync(new TerminalSessionStartRequest());
        int? exitCode = null;
        session.Exited += (_, code) => exitCode = code;

        await session.StopAsync();

        Assert.AreEqual(TerminalSessionState.Stopped, session.State);
        Assert.AreEqual(0, exitCode);
        await session.DisposeAsync();
    }

    /// <summary>
    /// Verifies that the shared foundation registration composes all base scopes for the application startup.
    /// </summary>
    [TestMethod]
    public void AddCodeStudioFoundation_RegistersSharedBaseScopes()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioFoundation();
        services.AddDiagnosticsUi();
        services.AddDebugDiagnostics();

        using var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<IAIClientFactory>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ICodeStudioBuilderService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IVersionControlService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ITestExecutionService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IDebugSessionService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ITerminalShellResolver>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ITerminalSessionService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IPlanningProvider>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<PlanningExplorerViewModel>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>());
        Assert.IsTrue(serviceProvider.GetServices<IDiagnosticConsumer>().Any());

        var storageDescriptor = services.Last(static descriptor => descriptor.ServiceType == typeof(ITextDocumentStorageService));
        var dialogDescriptor = services.Last(static descriptor => descriptor.ServiceType == typeof(IEditorFileDialogService));

        Assert.AreEqual(typeof(FileSystemTextDocumentStorageService), storageDescriptor.ImplementationType);
        Assert.AreEqual(typeof(AvaloniaEditorFileDialogService), dialogDescriptor.ImplementationType);
    }

#if NET10_0
    /// <summary>
    /// Verifies that the Git-backed versioning registration replaces the fallback service with the Git adapter.
    /// </summary>
    [TestMethod]
    public void AddGitCodeStudioVersioning_RegistersGitBackedServices()
    {
        var services = new ServiceCollection();

        services.AddGitCodeStudioVersioning();

        using var serviceProvider = services.BuildServiceProvider();
        Assert.IsInstanceOfType<GitCommandRunner>(serviceProvider.GetRequiredService<IGitCommandRunner>());
        Assert.IsInstanceOfType<GitVersionControlService>(serviceProvider.GetRequiredService<IVersionControlService>());
    }
#endif
}
