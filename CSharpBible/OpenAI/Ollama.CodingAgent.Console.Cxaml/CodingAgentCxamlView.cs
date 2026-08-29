using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Presentation;

namespace Ollama.CodingAgent.Console.Cxaml;

/// <summary>Connects agent-session projections and resizing behavior to the declarative workspace.</summary>
internal sealed class CodingAgentCxamlView
{
    private readonly IAgentApprovalService _approvalService;
    private readonly IApplication _application;
    private readonly ObservableCollection<string> _planningItems = [];
    private readonly AgentSessionViewModel _session;
    private Button? _cancel;
    private Button? _clear;
    private Button? _exit;
    private Terminal? _transcript;
    private ListBox? _planning;
    private TextBox? _prompt;
    private Button? _reload;
    private Button? _send;
    private Label? _status;

    public CodingAgentCxamlView(
        AgentSessionViewModel session,
        IAgentApprovalService approvalService,
        IApplication application)
    {
        _session = session;
        _approvalService = approvalService;
        _application = application;
    }

    public CxamlLoadResult Load()
    {
        string resourceName = typeof(CodingAgentCxamlView).Assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".Views.Main.cxaml", StringComparison.Ordinal));
        using Stream stream = typeof(CodingAgentCxamlView).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The coding-agent CXAML view resource is missing.");
        using StreamReader reader = new(stream);
        CxamlLoadResult result = new CxamlLoader().Load(reader, new CxamlLoadContext(_session));

        Panel root = GetControl<Panel>(result, "Workspace");
        _transcript = GetControl<Terminal>(result, "Transcript");
        _planning = GetControl<ListBox>(result, "Planning");
        _status = GetControl<Label>(result, "Status");
        _prompt = GetControl<TextBox>(result, "Prompt");
        _send = GetControl<Button>(result, "Send");
        _clear = GetControl<Button>(result, "Clear");
        _reload = GetControl<Button>(result, "Reload");
        _cancel = GetControl<Button>(result, "Cancel");
        _exit = GetControl<Button>(result, "Exit");
        _planning.ItemsSource = _planningItems;

        _prompt.MultiLine = false;
        _prompt.Active = true;
        _exit.OnClick += (_, _) => _application.Stop();

        _session.Conversation.CollectionChanged += OnConversationChanged;
        _session.PropertyChanged += OnSessionPropertyChanged;
        _application.OnCanvasResize += (_, _) => Arrange(root);
        Arrange(root);
        RefreshTranscript();
        RefreshPlanning();
        RefreshStatus();
        return result;
    }

    private void Arrange(Panel root)
    {
        if (_transcript is null || _planning is null || _prompt is null || _send is null
            || _clear is null || _reload is null || _cancel is null || _exit is null || _status is null)
        {
            throw new InvalidOperationException("The coding-agent CXAML view has not been initialized.");
        }

        int width = Math.Max(40, _application.size.Width);
        int height = Math.Max(12, _application.size.Height);
        int planningWidth = Math.Max(24, width / 4);
        int contentTop = 2;
        int contentHeight = Math.Max(8, height - 6);
        int contentWidth = Math.Max(20, width - planningWidth);

        root.size = new System.Drawing.Size(width, height);
        _transcript.Position = new System.Drawing.Point(0, contentTop);
        _transcript.size = new System.Drawing.Size(contentWidth, contentHeight);
        _planning.Position = new System.Drawing.Point(contentWidth, contentTop);
        _planning.size = new System.Drawing.Size(planningWidth, contentHeight);

        _prompt.Position = new System.Drawing.Point(0, contentTop + contentHeight + 1);
        _prompt.size = new System.Drawing.Size(Math.Max(20, width - 34), 1);
        _send.Position = new System.Drawing.Point(Math.Max(20, width - 33), contentTop + contentHeight + 1);
        _clear.Position = new System.Drawing.Point(Math.Max(29, width - 24), contentTop + contentHeight + 1);
        _reload.Position = new System.Drawing.Point(Math.Max(37, width - 15), contentTop + contentHeight + 1);
        _cancel.Position = new System.Drawing.Point(0, height - 2);
        _exit.Position = new System.Drawing.Point(Math.Max(0, width - 8), height - 2);
        _status.Position = new System.Drawing.Point(0, height - 1);
        _status.size = new System.Drawing.Size(width, 1);
    }

    private void OnConversationChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        RefreshTranscript();
        RefreshPlanning();
        RefreshStatus();
    }

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is nameof(AgentSessionViewModel.Status)
            or nameof(AgentSessionViewModel.ErrorMessage))
        {
            RefreshStatus();
        }
    }

    private void RefreshTranscript()
    {
        if (_transcript is null)
        {
            return;
        }

        _transcript.Clear();
        foreach (AgentConversationTurn turn in _session.Conversation)
        {
            _transcript.WriteLine($"{turn.Role}: {turn.Content}");
            _transcript.WriteLine();
        }
    }

    private void RefreshPlanning()
    {
        string currentTask = _session.Conversation.LastOrDefault(static turn => turn.Role == AgentConversationRole.User)?.Content
            ?? "Waiting for task";
        ConsolePlanningSnapshot snapshot = new(
            "Coding-agent session",
            "Current conversation",
            currentTask,
            "Not available",
            "Next agent step");
        _planningItems.Clear();
        _planningItems.Add($"Feature: {snapshot.Feature}");
        _planningItems.Add($"  Backlog: {snapshot.Backlog}");
        _planningItems.Add($"    Previous: {snapshot.PreviousTask}");
        _planningItems.Add($"    Current: {snapshot.CurrentTask}");
        _planningItems.Add($"    Next: {snapshot.NextTask}");
    }

    private void RefreshStatus()
    {
        if (_status is not null)
        {
            _status.Text = $" {_session.Status} | Session: {_session.SessionId} | Pending approvals: {_approvalService.PendingRequests.Count}";
        }
    }

    private static TControl GetControl<TControl>(CxamlLoadResult result, string name)
        where TControl : class, IControl
        => result.NamedControls.TryGetValue(name, out IControl? control) && control is TControl typedControl
            ? typedControl
            : throw new CxamlParseException($"The coding-agent CXAML view is missing '{name}'.");
}
