using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA98_AvlnCodeStudio.UI.DependencyInjection;

/// <summary>
/// Registers C# workspace services for a workbench host.
/// </summary>
public static class CSharpWorkspaceServiceCollectionExtensions
{
    /// <summary>
    /// Registers the UI-neutral C# workspace loader.
    /// </summary>
    public static IServiceCollection AddCSharpWorkspace(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<CSharpWorkspaceLoader>();
        return services;
    }
}
