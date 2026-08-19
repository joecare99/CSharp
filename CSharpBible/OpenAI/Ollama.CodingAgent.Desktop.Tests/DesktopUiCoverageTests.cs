using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Desktop.Host;
using Ollama.CodingAgent.Desktop.Models;
using Ollama.CodingAgent.Desktop.Services;
using Ollama.CodingAgent.Desktop.Widgets;
using Ollama.CodingAgent.Desktop.ViewModels;
using Ollama.CodingAgent.Desktop.Views;
using Ollama.CodingAgent.Desktop;

namespace Ollama.CodingAgent.Desktop.Tests;

[TestClass]
[DoNotParallelize]
public sealed class DesktopUiCoverageTests
{
    [ClassInitialize]
    public static void InitializeClass(TestContext _)
    {
        FieldInfo starter = typeof(Program).GetField("StartDesktopLifetime", BindingFlags.NonPublic | BindingFlags.Static)!;
        FieldInfo configure = typeof(Program).GetField("ConfigureDesktopLifetime", BindingFlags.NonPublic | BindingFlags.Static)!;
        Delegate originalStarter = (Delegate)starter.GetValue(null)!;
        try
        {
            DesktopComposition.Initialize(new DesktopOptions
            {
                Endpoint = "http://localhost:11434/",
                Model = "test-model",
                WorkspacePath = Environment.CurrentDirectory,
                SessionId = "desktop-class",
                CodeWikiVaultPath = Environment.CurrentDirectory,
            });
            configure.SetValue(null, (Action<IClassicDesktopStyleApplicationLifetime>)(lifetime =>
                Dispatcher.UIThread.Post(() => lifetime.Shutdown())));
            originalStarter.DynamicInvoke(Program.BuildAvaloniaApp(), Array.Empty<string>());
        }
        finally
        {
            configure.SetValue(null, null);
        }
    }

    [TestMethod]
    public void AvaloniaControls_LoadXamlAndBindMainWindow()
    {
        EnsureAvaloniaPlatform();
        App app = new();
        app.Initialize();
        app.Initialize();
        Assert.IsTrue(app.Styles.OfType<FluentTheme>().Any());

        Assert.IsNotNull(new CodeWikiPanel());
        Assert.IsNotNull(new ApprovalPanel());
        Assert.IsNotNull(new ActivityPanel());

        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            approvals,
            "desktop",
            Environment.CurrentDirectory);
        DesktopSessionViewModel viewModel = new(
            session,
            approvals,
            new LocalKnowledgeBaseStore(PathFor("ui-wiki.json")),
            new LocalWikiMarkdownImporter(),
            CreateOptions());
        session.Conversation.Add(new AgentConversationTurn
        {
            Role = AgentConversationRole.User,
            Content = "Conversation",
            CreatedAt = DateTimeOffset.UtcNow,
        });
        viewModel.WikiSearchResults.Add(new LocalKnowledgeEntry
        {
            Id = "wiki-1",
            Title = "Wiki page",
            Summary = "Summary",
        });
        _ = approvals.RequestApprovalAsync(new AgentApprovalRequest
        {
            Id = "approval",
            Operation = "stage",
            Preview = "git add file",
            CreatedAt = DateTimeOffset.UtcNow,
        });
        viewModel.RefreshApprovalsCommand.Execute(null);
        MainWindow window = new(viewModel);

        Assert.AreSame(viewModel, window.DataContext);
        window.ApplyTemplate();
        window.Measure(new Size(1200, 850));
        window.Arrange(new Rect(0, 0, 1200, 850));
        Assert.IsNotNull(Program.BuildAvaloniaApp());
    }

    [TestMethod]
    public void ApplicationInitialization_ComposesDesktopServicesAndHandlesLifetimes()
    {
        EnsureAvaloniaPlatform();
        DesktopOptions options = CreateOptions();
        FieldInfo servicesField = typeof(DesktopComposition).GetField("_services", BindingFlags.NonPublic | BindingFlags.Static)!;
        servicesField.SetValue(null, null);
        Assert.ThrowsExactly<InvalidOperationException>(() => DesktopComposition.GetRequiredService<DesktopSessionViewModel>());

        DesktopComposition.Initialize(options);
        Assert.IsNotNull(DesktopComposition.GetRequiredService<DesktopSessionViewModel>());
        DesktopComposition.Initialize(options);

        App app = new();
        app.Initialize();
        app.OnFrameworkInitializationCompleted();

        Assert.IsNotNull(DesktopComposition.GetRequiredService<DesktopSessionViewModel>());
    }

    [TestMethod]
    public void ProgramMain_UsesNoOpLifetimeStarterForDeterministicStartup()
    {
        FieldInfo starter = typeof(Program).GetField("StartDesktopLifetime", BindingFlags.NonPublic | BindingFlags.Static)!;
        Delegate original = (Delegate)starter.GetValue(null)!;
        try
        {
            starter.SetValue(null, (Action<AppBuilder, string[]>)((_, _) => { }));
            MethodInfo main = typeof(Program).GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
            main.Invoke(null, new object?[]
            {
                new[]
                {
                    "--endpoint", "http://localhost:11434",
                    "--model", "test-model",
                    "--workspace", Environment.CurrentDirectory,
                    "--session", "desktop",
                    "--code-wiki-vault", Environment.CurrentDirectory,
                },
            });
        }
        finally
        {
            starter.SetValue(null, original);
        }
    }

    private static DesktopSessionViewModel CreateViewModel()
    {
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            approvals,
            "desktop",
            Environment.CurrentDirectory);
        return new DesktopSessionViewModel(
            session,
            approvals,
            new LocalKnowledgeBaseStore(PathFor("ui-wiki.json")),
            new LocalWikiMarkdownImporter(),
            CreateOptions());
    }

    private static DesktopOptions CreateOptions()
        => new()
        {
            Endpoint = "http://localhost:11434/",
            Model = "test-model",
            WorkspacePath = Environment.CurrentDirectory,
            SessionId = "desktop",
            CodeWikiVaultPath = Environment.CurrentDirectory,
        };

    private static string PathFor(string name)
        => System.IO.Path.Combine(AppContext.BaseDirectory, "CoverageWorkspaces", name);

    private static bool _avaloniaPlatformConfigured;

    private static void EnsureAvaloniaPlatform()
    {
        if (_avaloniaPlatformConfigured || Avalonia.Application.Current is not null)
        {
            _avaloniaPlatformConfigured = true;
            return;
        }

        Program.BuildAvaloniaApp().SetupWithoutStarting();
        _avaloniaPlatformConfigured = true;
    }
}
