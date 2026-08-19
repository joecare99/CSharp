using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Desktop.Models;
using Ollama.CodingAgent.Desktop.Services;
using Ollama.CodingAgent.Desktop.ViewModels;
using Ollama.CodingAgent.Desktop.Views;
using Ollama.CodingAgent.Desktop;

namespace Ollama.CodingAgent.Desktop.Host;

/// <summary>
/// Builds the desktop-specific dependency graph around the shared application session.
/// </summary>
internal static class DesktopComposition
{
    private static ServiceProvider? _services;

    /// <summary>
    /// Configures singleton services for one desktop process.
    /// </summary>
    public static void Initialize(DesktopOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _services?.Dispose();

        ServiceCollection services = new();
        services.AddOllamaCodingAgent(options.ToRuntimeOptions());
        DesktopConfigurationState configurationState = new(new DesktopConfiguration
        {
            Endpoint = options.Endpoint,
            Model = options.Model,
            WorkspacePath = options.WorkspacePath,
        });
        services.AddSingleton(configurationState);
        services.AddSingleton<DesktopConfigurationStore>();
        services.AddSingleton<DesktopOllamaEndpointService>();
        services.AddSingleton<DesktopDynamicSessionService>(provider => new DesktopDynamicSessionService(
            options,
            () => provider.GetRequiredService<DesktopConfigurationState>().Current));
        services.AddAgentApplication(options.WorkspacePath, options.SessionId);
        services.AddSingleton<IAgentSessionService>(provider =>
            provider.GetRequiredService<DesktopDynamicSessionService>());
        services.AddSingleton(options);
        services.AddSingleton(new LocalKnowledgeBaseStore(
            Path.Combine(options.WorkspacePath, ".agent", "local-wiki.json")));
        services.AddSingleton<LocalWikiMarkdownImporter>();
        services.AddSingleton<DesktopSessionViewModel>();
        services.AddSingleton<MainWindow>();
        _services = services.BuildServiceProvider(validateScopes: true);
    }

    /// <summary>
    /// Resolves one desktop service after startup composition has completed.
    /// </summary>
    public static T GetRequiredService<T>()
        where T : notnull
        => (_services ?? throw new InvalidOperationException("Desktop composition has not been initialized."))
            .GetRequiredService<T>();
}
