using Microsoft.Extensions.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using System;

namespace Project.Explorer.Avalonia.DependencyInjection;

/// <summary>Registers reusable Avalonia project explorer presentation services.</summary>
public static class ProjectExplorerAvaloniaServiceCollectionExtensions
{
    /// <summary>Registers the shared project explorer ViewModel.</summary>
    public static IServiceCollection AddProjectExplorerAvalonia(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<ProjectExplorerViewModel>();
        return services;
    }
}
