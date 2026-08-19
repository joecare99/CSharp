using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using Microsoft.Extensions.DependencyInjection;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Git;
using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;

namespace Ollama.CodingAgent.Console;

/// <summary>
/// Starts the persistent terminal adapter for the Ollama coding agent.
/// </summary>
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            ConsoleAgentCliOptions options = ConsoleAgentCliOptions.Parse(args);
            if (options.ShowHelp)
            {
                PrintHelp();
                return 0;
            }

            ServiceCollection services = new();
            ConsoleRuntimeConfiguration configuration = new(options.WorkspacePath, options.Endpoint, options.Model);
            services.AddSingleton(options);
            services.AddOllamaCodingAgent(options.ToRuntimeOptions());
            services.AddAgentApplication(options.WorkspacePath, options.SessionId);
            services.AddSingleton(configuration);
            services.AddSingleton<ConsoleDynamicSessionService>();
            services.AddSingleton<IAgentSessionService>(provider =>
                provider.GetRequiredService<ConsoleDynamicSessionService>());
            services.AddCodingAgentGit();
            services.AddSingleton<BaseLib.Interfaces.IConsole, SystemConsoleAdapter>();
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
                services.AddSingleton<ConsoleLib.Interfaces.IExtendedConsole, ConsoleLib.ExtendedConsole>();
                services.AddSingleton<ConsoleLib.Interfaces.IWidgetSet, ConsoleWidgetSet>();
                services.AddSingleton<ConsoleLib.CommonControls.Application>();
                services.AddSingleton<ConsoleLib.Interfaces.IApplication>(serviceProvider =>
                    serviceProvider.GetRequiredService<ConsoleLib.CommonControls.Application>());
                services.AddSingleton<ConsoleAgentView>();
            }

            using ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);
            if (System.Console.IsInputRedirected || System.Console.IsOutputRedirected)
            {
                await serviceProvider.GetRequiredService<ConsoleRepl>().RunAsync();
            }
            else
            {
                ConsoleLib.CommonControls.Application application = serviceProvider.GetRequiredService<ConsoleLib.CommonControls.Application>();
                application.Dimension = ConsoleFramework.Canvas.ClipRect;
                _ = serviceProvider.GetRequiredService<ConsoleAgentView>();
                application.Visible = true;
                application.Draw();
                application.Run();
            }
            return 0;
        }
        catch (Exception ex)
        {
            System.Console.Error.WriteLine($"Unable to start the terminal client: {ex.Message}");
            return 1;
        }
    }

    private static void PrintHelp()
    {
        System.Console.WriteLine("""
            Ollama.CodingAgent.Console

            Usage:
              dotnet run --project .\Ollama.CodingAgent.Console\Ollama.CodingAgent.Console.csproj -- [options]

            Options:
              --endpoint <url>       Ollama endpoint (default: http://localhost:11434/)
              --model <name>         Model name (default: qwen2.5-coder:7b)
              --workspace <path>     Workspace and persistent-session root (default: current directory)
              --workspace-root <path> Alias for --workspace
              --session <id>         Persistent session identifier (default: default)
              --help, -h             Show this help

            The session is stored under <workspace>\.agent\sessions\<session>.json.
            LLM traffic is logged by default under <workspace>\.agent\logs\<session>.jsonl.
            Credentials are redacted before persistence. The --debug-log switch is planned, but not available yet.
            """);
    }
}
