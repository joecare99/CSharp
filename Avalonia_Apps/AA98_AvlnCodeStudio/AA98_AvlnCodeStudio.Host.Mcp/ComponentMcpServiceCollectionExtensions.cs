using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using AA98_AvlnCodeStudio.Base.AI.Services;
using System;

namespace AA98_AvlnCodeStudio.Host.Mcp;

public static class ComponentMcpServiceCollectionExtensions
{
    public static IServiceCollection AddComponentMcpServer(
        this IServiceCollection services,
        string serverName,
        string version,
        Action<IMcpServerBuilder>? configureServer = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(serverName);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);

        services.AddSingleton<ComponentMcpCommandCatalog>();
        IMcpServerBuilder server = services
            .AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
                {
                    Name = serverName,
                    Version = version,
                };
            })
            .WithStdioServerTransport()
            .WithTools<ComponentMcpTools>();
        configureServer?.Invoke(server);

        return services;
    }

    public static IServiceCollection AddComponentMcpCommand<TCommand>(this IServiceCollection services)
        where TCommand : class, IComponentMcpCommand
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<TCommand>();
        services.AddSingleton<IComponentMcpCommand>(static provider => provider.GetRequiredService<TCommand>());
        services.AddSingleton<IAIChatTool, ComponentMcpAIChatToolAdapter<TCommand>>();
        return services;
    }

    /// <summary>
    /// Registers one workbench command contribution for both local use and explicit MCP export.
    /// </summary>
    public static IServiceCollection AddWorkbenchMcpCommand<TCommand>(this IServiceCollection services)
        where TCommand : class, IWorkbenchMcpCommandContribution
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<TCommand>();
        services.AddSingleton<IWorkbenchCommandContribution>(static provider => provider.GetRequiredService<TCommand>());
        services.AddSingleton<IWorkbenchMcpCommandContribution>(static provider => provider.GetRequiredService<TCommand>());
        services.AddSingleton<IComponentMcpCommand, WorkbenchMcpCommandAdapter<TCommand>>();
        services.AddSingleton<IAIChatTool, WorkbenchAIChatToolAdapter<TCommand>>();
        return services;
    }
}
