using AA98_AvlnCodeStudio.Base.AI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Building.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Debugging.DependencyInjection;
using AA98_AvlnCodeStudio.Base.OS.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Testing.DependencyInjection;
using AA98_AvlnCodeStudio.Base.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Versioning.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Planning.Services;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.UI.DependencyInjection;

/// <summary>
/// Provides the application-level composition baseline for Code Studio services.
/// </summary>
public static class CodeStudioServiceCollectionExtensions
{
    /// <summary>
    /// Adds the shared Code Studio base-scope registrations for the current application startup.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddCodeStudioFoundation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddCodeStudioAI();
        services.AddCodeStudioBuilding();
        services.AddCodeStudioVersioning();
        services.AddCodeStudioTesting();
        services.AddCodeStudioDebugging();
        services.AddCodeStudioOS<FileSystemTextDocumentStorageService>();
        services.AddCodeStudioUI<AvaloniaEditorFileDialogService>();
        services.AddSingleton<IPlanningReader, MarkdownPlanningReader>();
        services.AddSingleton<PlanningExplorerViewModel>();
        return services;
    }
}
