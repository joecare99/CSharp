using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;

namespace Ollama.CodingAgent.Console.Presentation;

/// <summary>
/// ConsoleLib-based agent workspace with menu, transcript, prompt input, status, and planning pane.
/// </summary>
public sealed class ConsoleAgentView : Panel
{
    private readonly AgentSessionViewModel _session;
    private readonly IAgentApprovalService _approvalService;
    private readonly Terminal _transcript;
    private readonly TextBox _prompt;
    private readonly Label _status;
    private readonly ObservableCollection<string> _planningItems = [];
    private readonly Func<ConsolePlanningSnapshot> _planningProvider;

    public ConsoleAgentView(
        AgentSessionViewModel session,
        IAgentApprovalService approvalService,
        IApplication application,
        Func<ConsolePlanningSnapshot>? planningProvider = null)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        _planningProvider = planningProvider ?? (() => CreateDefaultPlanningSnapshot());

        Parent = application;
        BorderStyle = ConsoleLib.Data.BorderStyle.None;
        BackColor = ConsoleColor.Black;
        ForeColor = ConsoleColor.Gray;
        Dimension = ConsoleFramework.Canvas.ClipRect;

        MenuBar menu = new() { Parent = this, Dimension = new Rectangle(0, 0, Dimension.Width, 1) };
        MenuItem sessionMenu = new() { Text = "&Session" };
        MenuPopup sessionPopup = new();
        sessionPopup.AddItem(new MenuItem { Text = "&Clear", Command = _session.ClearCommand });
        sessionPopup.AddItem(new MenuItem { Text = "&Reload", Command = _session.ReloadCommand });
        sessionPopup.AddItem(new MenuItem { Text = "E&xit", Command = new CommunityToolkit.Mvvm.Input.RelayCommand(() => application.Stop()) });
        menu.AddRootItem(sessionMenu, sessionPopup);

        int planningWidth = Math.Max(24, Dimension.Width / 4);
        int contentTop = 2;
        int contentHeight = Math.Max(8, Dimension.Height - 6);

        _transcript = new Terminal
        {
            Parent = this,
            BorderStyle = ConsoleLib.Data.BorderStyle.Single,
            BorderColor = ConsoleColor.DarkCyan,
            ForeColor = ConsoleColor.Gray,
            BackColor = ConsoleColor.Black,
            Position = new Point(0, contentTop),
            size = new Size(Math.Max(20, Dimension.Width - planningWidth), contentHeight),
        };

        ListBox planning = new()
        {
            Parent = this,
            BorderDefinition = new ConsoleLib.CommonControls.BorderDef { Style = ConsoleLib.Data.BorderStyle.Single, BorderColor = ConsoleColor.DarkYellow },
            ForeColor = ConsoleColor.Gray,
            BackColor = ConsoleColor.Black,
            Position = new Point(Dimension.Width - planningWidth, contentTop),
            size = new Size(planningWidth, contentHeight),
            ItemsSource = _planningItems,
        };

        _prompt = new TextBox
        {
            Parent = this,
            MultiLine = false,
            Position = new Point(0, contentTop + contentHeight + 1),
            size = new Size(Math.Max(20, Dimension.Width - 12), 1),
            Active = true,
        };

        Button send = new()
        {
            Parent = this,
            Text = "&Send",
            Accelerator = 'S',
            Position = new Point(Math.Max(0, Dimension.Width - 11), contentTop + contentHeight + 1),
            size = new Size(10, 1),
            Command = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(SubmitAsync),
        };

        _status = new Label
        {
            Parent = this,
            Position = new Point(0, Dimension.Height - 1),
            size = new Size(Dimension.Width, 1),
            ForeColor = ConsoleColor.White,
            BackColor = ConsoleColor.DarkBlue,
        };

        _session.Conversation.CollectionChanged += (_, _) => RefreshTranscript();
        _session.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName is nameof(AgentSessionViewModel.Status) or nameof(AgentSessionViewModel.ErrorMessage))
            {
                RefreshStatus();
            }
        };

        RefreshTranscript();
        RefreshPlanning();
        RefreshStatus();
    }

    private async Task SubmitAsync()
    {
        string prompt = _prompt.Text.Trim();
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return;
        }

        _session.Prompt = prompt;
        _prompt.Text = string.Empty;
        await _session.SubmitCommand.ExecuteAsync(null);
        RefreshTranscript();
        RefreshPlanning();
        RefreshStatus();
    }

    private void RefreshTranscript()
    {
        _transcript.Clear();
        foreach (AgentConversationTurn turn in _session.Conversation)
        {
            _transcript.WriteLine($"{turn.Role}: {turn.Content}");
            _transcript.WriteLine();
        }
    }

    private void RefreshPlanning()
    {
        ConsolePlanningSnapshot snapshot = _planningProvider();
        _planningItems.Clear();
        _planningItems.Add($"Feature: {snapshot.Feature}");
        _planningItems.Add($"  Backlog: {snapshot.Backlog}");
        _planningItems.Add($"    Previous: {snapshot.PreviousTask}");
        _planningItems.Add($"    Current: {snapshot.CurrentTask}");
        _planningItems.Add($"    Next: {snapshot.NextTask}");
    }

    private void RefreshStatus()
    {
        _status.Text = $" {_session.Status} | Session: {_session.SessionId} | Pending approvals: {_approvalService.PendingRequests.Count}";
    }

    private ConsolePlanningSnapshot CreateDefaultPlanningSnapshot()
    {
        string currentTask = _session.Conversation.LastOrDefault(turn => turn.Role == AgentConversationRole.User)?.Content
            ?? "Waiting for task";
        return new ConsolePlanningSnapshot(
            "Coding-agent session",
            "Current conversation",
            currentTask,
            "Not available",
            "Next agent step");
    }
}
