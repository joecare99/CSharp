using AA98_AvlnCodeStudio.Base.AI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
using AA98_AvlnCodeStudio.Base.Building.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Debugging.Services;
#if NET10_0
using AA98_AvlnCodeStudio.Building.Workbench.DependencyInjection;
using AA98_AvlnCodeStudio.Versioning.Git.DependencyInjection;
#endif
using AA98_AvlnCodeStudio.Base.Debugging.DependencyInjection;
using AA98_AvlnCodeStudio.Base.OS.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Testing.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Base.Versioning.DependencyInjection;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Versioning.Svn.DependencyInjection;
using System;
#if NET10_0
using AA98_AvlnCodeStudio.Base.Versioning.Services;
#endif

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

        services.AddSingleton<OpenAICompatibleAISettings>();
        services.AddCodeStudioAI<OpenAICompatibleAIClientFactory>();
        services.AddComponentMcpCommand<Commands.InspectWorkspaceChatCommand>();
        services.AddComponentMcpCommand<Commands.InspectVersionControlWorkspaceChatCommand>();
        services.AddSvnCodeStudioVersioning();
        services.AddComponentMcpCommand<Commands.UpdateSvnWorkspaceChatCommand>();
#if NET10_0
        services.AddComponentMcpCommand<Commands.BuildWorkspaceChatCommand>();
#endif
#if NET10_0
        services.AddWorkbenchCodeStudioBuilding();
        services.AddGitCodeStudioVersioning();
        services.AddSingleton<NullVersionControlService>();
        services.AddSingleton<IVersionControlService, WorkspaceVersionControlService>();
#else
        services.AddCodeStudioBuilding();
        services.AddCodeStudioVersioning();
#endif
        services.AddCodeStudioTesting<DotNetTestExecutionService>();
        services.AddComponentMcpCommand<Commands.TestWorkspaceChatCommand>();
        services.AddSingleton<AA98_AvlnCodeStudio.Base.Debugging.Services.IDebugSessionService>(
            _ => new NetCoreDbgDebugSessionService());
        services.AddCodeStudioOS<FileSystemTextDocumentStorageService>();
        services.AddCodeStudioUI<AvaloniaEditorFileDialogService>();
        services.AddWorkbenchCommandCatalog();
        services.AddCSharpWorkspace();
        services.AddSingleton<IPlanningProvider, MarkdownPlanningReader>();
        services.AddSingleton<PlanningExplorerViewModel>();
        return services;
    }
}
