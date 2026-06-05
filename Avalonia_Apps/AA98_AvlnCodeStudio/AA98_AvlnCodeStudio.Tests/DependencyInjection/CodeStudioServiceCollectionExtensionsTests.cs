using AA98_AvlnCodeStudio.Base.AI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.AI.Services;
using AA98_AvlnCodeStudio.Base.Debugging.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
using AA98_AvlnCodeStudio.Base.OS.DependencyInjection;
using AA98_AvlnCodeStudio.Base.OS.Services;
using AA98_AvlnCodeStudio.Base.Testing.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Base.Versioning.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.UI.DependencyInjection;
using AA98_AvlnCodeStudio.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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

        services.AddCodeStudioVersioning();
        services.AddCodeStudioTesting();
        services.AddCodeStudioDebugging();

        using var serviceProvider = services.BuildServiceProvider();
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
        Assert.IsInstanceOfType<DesignEditorFileDialogService>(serviceProvider.GetRequiredService<IEditorFileDialogService>());
    }

    /// <summary>
    /// Verifies that the shared foundation registration composes all base scopes for the application startup.
    /// </summary>
    [TestMethod]
    public void AddCodeStudioFoundation_RegistersSharedBaseScopes()
    {
        var services = new ServiceCollection();

        services.AddCodeStudioFoundation();

        using var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<IAIClientFactory>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IVersionControlService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ITestExecutionService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IDebugSessionService>());

        var storageDescriptor = services.Last(static descriptor => descriptor.ServiceType == typeof(ITextDocumentStorageService));
        var dialogDescriptor = services.Last(static descriptor => descriptor.ServiceType == typeof(IEditorFileDialogService));

        Assert.AreEqual(typeof(FileSystemTextDocumentStorageService), storageDescriptor.ImplementationType);
        Assert.AreEqual(typeof(AvaloniaEditorFileDialogService), dialogDescriptor.ImplementationType);
    }
}
