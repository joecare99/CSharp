using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Git;

namespace Ollama.CodingAgent.Console.Tests;

[TestClass]
public sealed class ConsoleServiceRegistrationTests
{
    [TestMethod]
    public void Composition_ResolvesSharedApplicationAndGitServices()
    {
        ConsoleAgentCliOptions options = ConsoleAgentCliOptions.Parse(
        [
            "--workspace", ".",
            "--session", "console-test",
        ]);
        ServiceCollection services = new();
        services.AddOllamaCodingAgent(options.ToRuntimeOptions());
        services.AddAgentApplication(options.WorkspacePath, options.SessionId);
        services.AddCodingAgentGit();
        services.AddSingleton<BaseLib.Interfaces.IConsole, SystemConsoleAdapter>();
        services.AddSingleton<ConsoleRepl>();

        using ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);

        Assert.IsNotNull(serviceProvider.GetRequiredService<AgentSessionViewModel>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IAgentApprovalService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IGitWorkspaceService>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<IGitOperationExecutor>());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ConsoleRepl>());
    }

    [TestMethod]
    public void Composition_ResolvesApplicationInterfaceFromConcreteApplication()
    {
        ServiceCollection services = new();
        services.AddSingleton(Substitute.For<ConsoleLib.Interfaces.IWidgetSet>());
        services.AddSingleton<ConsoleLib.CommonControls.Application>();
        services.AddSingleton<ConsoleLib.Interfaces.IApplication>(serviceProvider =>
            serviceProvider.GetRequiredService<ConsoleLib.CommonControls.Application>());

        using ServiceProvider serviceProvider = services.BuildServiceProvider(validateScopes: true);

        ConsoleLib.CommonControls.Application application =
            serviceProvider.GetRequiredService<ConsoleLib.CommonControls.Application>();
        ConsoleLib.Interfaces.IApplication applicationInterface =
            serviceProvider.GetRequiredService<ConsoleLib.Interfaces.IApplication>();

        Assert.AreSame(application, applicationInterface);
    }
}
