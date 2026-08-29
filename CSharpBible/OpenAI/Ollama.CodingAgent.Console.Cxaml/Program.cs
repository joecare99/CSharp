using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Git;
using CxamlApplication = ConsoleLib.CommonControls.Application;

namespace Ollama.CodingAgent.Console.Cxaml;

/// <summary>Starts the declarative terminal workspace for the Ollama coding agent.</summary>
public static class Program
{
    /// <summary>Loads the workspace markup for the active shared agent session.</summary>
    public static CxamlLoadResult CreateView(
        AgentSessionViewModel session,
        IAgentApprovalService approvals,
        IApplication application)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(approvals);
        ArgumentNullException.ThrowIfNull(application);
        return new CodingAgentCxamlView(session, approvals, application).Load();
    }

    public static async Task<int> Main(string[] args)
    {
        try
        {
            ConsoleAgentCliOptions options = ConsoleAgentCliOptions.Parse(args);
            if (options.ShowHelp)
            {
                PrintHelp();
                return 0;
            }

            ServiceCollection services = CreateServices(options);
            if (System.Console.IsInputRedirected || System.Console.IsOutputRedirected)
            {
                services.AddSingleton<ConsoleRepl>(provider => new ConsoleRepl(
                    provider.GetRequiredService<AgentSessionViewModel>(),
                    provider.GetRequiredService<IAgentApprovalService>(),
                    provider.GetRequiredService<BaseLib.Interfaces.IConsole>(),
                    null,
                    provider.GetRequiredService<AgentDiagnosticsChannel>(),
                    null,
                    provider.GetRequiredService<ConsoleRuntimeConfiguration>()));
            }
            else
            {
                services.AddSingleton<IExtendedConsole, ExtendedConsole>();
                services.AddSingleton<IWidgetSet, ConsoleWidgetSet>();
                services.AddSingleton<CxamlApplication>();
                services.AddSingleton<IApplication>(provider => provider.GetRequiredService<CxamlApplication>());
            }

            using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
            if (System.Console.IsInputRedirected || System.Console.IsOutputRedirected)
            {
                await provider.GetRequiredService<ConsoleRepl>().RunAsync();
            }
            else
            {
                CxamlApplication application = provider.GetRequiredService<CxamlApplication>();
                application.Dimension = ConsoleFramework.Canvas.ClipRect;
                CxamlLoadResult view = CreateView(
                    provider.GetRequiredService<AgentSessionViewModel>(),
                    provider.GetRequiredService<IAgentApprovalService>(),
                    application);
                application.Add(view.Root);
                application.Visible = true;
                application.Draw();
                application.Run();
            }

            return 0;
        }
        catch (Exception exception)
        {
            System.Console.Error.WriteLine($"Unable to start the terminal client: {exception.Message}");
            return 1;
        }
    }

    internal static ServiceCollection CreateServices(ConsoleAgentCliOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ConsoleRuntimeConfiguration configuration = new(options.WorkspacePath, options.Endpoint, options.Model);
        ServiceCollection services = new();
        services.AddSingleton(options);
        services.AddOllamaCodingAgent(options.ToRuntimeOptions());
        services.AddAgentApplication(options.WorkspacePath, options.SessionId);
        services.AddSingleton(configuration);
        services.AddSingleton<ConsoleDynamicSessionService>();
        services.AddSingleton<IAgentSessionService>(provider =>
            provider.GetRequiredService<ConsoleDynamicSessionService>());
        services.AddCodingAgentGit();
        services.AddSingleton<BaseLib.Interfaces.IConsole, SystemConsoleAdapter>();
        return services;
    }

    private static void PrintHelp()
    {
        System.Console.WriteLine("""
            Ollama.CodingAgent.Console.Cxaml

            Usage:
              dotnet run --project .\Ollama.CodingAgent.Console.Cxaml\Ollama.CodingAgent.Console.Cxaml.csproj -- [options]

            Options:
              --endpoint <url>       Ollama endpoint (default: http://localhost:11434/)
              --model <name>         Model name (default: qwen2.5-coder:7b)
              --workspace <path>     Workspace and persistent-session root (default: current directory)
              --workspace-root <path> Alias for --workspace
              --session <id>         Persistent session identifier (default: default)
              --help, -h             Show this help
            """);
    }
}
