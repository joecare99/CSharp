using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
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
        services.AddSingleton<IComponentMcpCommand, TCommand>();
        return services;
    }
}
