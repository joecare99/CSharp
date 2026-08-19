using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.ViewModels;

/// <summary>
/// Provides the shared interactive session state for terminal and desktop clients.
/// </summary>
public sealed partial class AgentSessionViewModel : ObservableObject
{
    private readonly IAgentSessionService _sessionService;
    private readonly IAgentSessionStore _sessionStore;
    private readonly IAgentApprovalService _approvalService;
    private readonly SynchronizationContext? _synchronizationContext;
    private CancellationTokenSource? _runCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentSessionViewModel"/> class.
    /// </summary>
    public AgentSessionViewModel(
        IAgentSessionService sessionService,
        IAgentSessionStore sessionStore,
        IAgentApprovalService approvalService,
        string sessionId,
        string workspacePath)
        : this(sessionService, sessionStore, approvalService, sessionId, workspacePath, null)
    {
    }

    /// <summary>
    /// Initializes a new instance with a live diagnostics channel.
    /// </summary>
    public AgentSessionViewModel(
        IAgentSessionService sessionService,
        IAgentSessionStore sessionStore,
        IAgentApprovalService approvalService,
        string sessionId,
        string workspacePath,
        AgentDiagnosticsChannel? diagnosticsChannel)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _sessionStore = sessionStore ?? throw new ArgumentNullException(nameof(sessionStore));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        SessionId = sessionId;
        WorkspacePath = Path.GetFullPath(workspacePath);
        _synchronizationContext = SynchronizationContext.Current;
        if (diagnosticsChannel is not null)
        {
            diagnosticsChannel.EventRecorded += OnDiagnosticEventRecorded;
        }
    }

    /// <summary>
    /// Gets the stable session identity.
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// Gets the visible conversation history.
    /// </summary>
    public ObservableCollection<AgentConversationTurn> Conversation { get; } = [];

    /// <summary>
    /// Gets the chronological runtime activities for the active session.
    /// </summary>
    public ObservableCollection<AgentDiagnosticEvent> Activities { get; } = [];

    /// <summary>
    /// Gets the currently pending approval requests.
    /// </summary>
    public System.Collections.Generic.IReadOnlyList<AgentApprovalRequest> PendingApprovals
        => _approvalService.PendingRequests;

    /// <summary>
    /// Gets or sets the current user prompt.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _prompt = string.Empty;

    /// <summary>
    /// Gets or sets the selected workspace path.
    /// </summary>
    [ObservableProperty]
    private string _workspacePath;

    /// <summary>
    /// Gets or sets a value indicating whether a runtime request is active.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isRunning;

    /// <summary>
    /// Gets or sets the latest visible workflow status.
    /// </summary>
    [ObservableProperty]
    private string _status = "Ready.";

    /// <summary>
    /// Gets or sets the latest runtime error message.
    /// </summary>
    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Submits the current prompt to the shared agent session.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task SubmitAsync()
    {
        string submittedPrompt = Prompt.Trim();
        Prompt = string.Empty;
        ErrorMessage = null;
        IsRunning = true;
        Status = "Running agent request.";
        Conversation.Add(CreateTurn(AgentConversationRole.User, submittedPrompt));
        _runCancellationTokenSource = new CancellationTokenSource();

        try
        {
            AgentRunResult result;
            if (_sessionService is IStreamingAgentSessionService streamingSessionService)
            {
                result = await streamingSessionService.RunAsync(
                    submittedPrompt,
                    OnRuntimeUpdate,
                    _runCancellationTokenSource.Token);
            }
            else
            {
                result = await _sessionService.RunAsync(submittedPrompt, _runCancellationTokenSource.Token);
            }
            foreach (string thinking in result.Thinking)
            {
                if (!string.IsNullOrWhiteSpace(thinking))
                {
                    if (!Conversation.Any(static turn => turn.Kind == AgentConversationEntryKind.Thinking))
                    {
                        Conversation.Add(CreateTurn(AgentConversationRole.System, $"Thinking: {thinking}", AgentConversationEntryKind.Thinking));
                    }
                }
            }
            foreach (AgentConversationTurn turn in Conversation.Where(static turn => turn.IsLive))
            {
                turn.Complete();
            }
            Conversation.Add(CreateTurn(AgentConversationRole.Assistant, result.FinalResponse));
            Status = "Agent response completed.";
        }
        catch (OperationCanceledException)
        {
            Status = "Agent request cancelled.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Status = "Agent request failed.";
            Conversation.Add(CreateTurn(AgentConversationRole.System, $"Agent request failed: {ex.Message}"));
        }
        finally
        {
            _runCancellationTokenSource.Dispose();
            _runCancellationTokenSource = null;
            IsRunning = false;
        }

        await SaveAsync();
    }

    /// <summary>
    /// Cancels the currently active agent request.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        _runCancellationTokenSource?.Cancel();
        Status = "Cancelling agent request.";
    }

    /// <summary>
    /// Clears the current conversation and persists the empty session.
    /// </summary>
    [RelayCommand]
    private async Task ClearAsync()
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("An active agent request must be cancelled before clearing the session.");
        }

        Conversation.Clear();
        ErrorMessage = null;
        Status = "Session cleared.";
        await SaveAsync();
    }

    /// <summary>
    /// Reloads the persisted conversation snapshot.
    /// </summary>
    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("An active agent request must be cancelled before reloading the session.");
        }

        AgentSessionSnapshot snapshot = await _sessionStore.LoadAsync();
        if (!string.Equals(snapshot.SessionId, SessionId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("The persisted session identity does not match the active session.");
        }

        string persistedWorkspacePath = Path.GetFullPath(snapshot.WorkspacePath);
        if (!string.Equals(persistedWorkspacePath, WorkspacePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The persisted session workspace does not match the active workspace.");
        }

        Conversation.Clear();
        foreach (AgentConversationTurn turn in snapshot.Conversation)
        {
            Conversation.Add(turn);
        }

        ErrorMessage = null;
        Status = "Session reloaded.";
    }

    /// <summary>
    /// Resolves a pending state-changing operation approval.
    /// </summary>
    public bool ResolveApproval(string requestId, bool approved)
    {
        bool resolved = _approvalService.Resolve(requestId, approved);
        if (resolved)
        {
            Status = approved ? "Operation approved." : "Operation rejected.";
        }

        return resolved;
    }

    private bool CanSubmit()
        => !IsRunning && !string.IsNullOrWhiteSpace(Prompt);

    private bool CanCancel()
        => IsRunning;

    private Task SaveAsync()
        => _sessionStore.SaveAsync(new AgentSessionSnapshot
        {
            SessionId = SessionId,
            WorkspacePath = WorkspacePath,
            Conversation = [.. Conversation],
        });

    private void OnDiagnosticEventRecorded(object? sender, AgentDiagnosticEvent diagnosticEvent)
    {
        if (_synchronizationContext is null || SynchronizationContext.Current == _synchronizationContext)
        {
            Activities.Add(diagnosticEvent);
            return;
        }

        _synchronizationContext.Post(static state =>
        {
            (AgentSessionViewModel viewModel, AgentDiagnosticEvent activity) = ((AgentSessionViewModel, AgentDiagnosticEvent))state!;
            viewModel.Activities.Add(activity);
        }, (this, diagnosticEvent));
    }

    private void OnRuntimeUpdate(AgentRuntimeUpdate update)
    {
        if (_synchronizationContext is null || SynchronizationContext.Current == _synchronizationContext)
        {
            AddRuntimeUpdate(update);
            return;
        }

        _synchronizationContext.Post(static state =>
        {
            (AgentSessionViewModel viewModel, AgentRuntimeUpdate runtimeUpdate) = ((AgentSessionViewModel, AgentRuntimeUpdate))state!;
            viewModel.AddRuntimeUpdate(runtimeUpdate);
        }, (this, update));
    }

    private void AddRuntimeUpdate(AgentRuntimeUpdate update)
    {
        AgentConversationEntryKind kind = update.Kind switch
        {
            AgentRuntimeUpdateKind.Thinking => AgentConversationEntryKind.Thinking,
            AgentRuntimeUpdateKind.Tool => AgentConversationEntryKind.Tool,
            _ => AgentConversationEntryKind.Workflow,
        };

        AgentConversationTurn? liveEntry = Conversation
            .LastOrDefault(static turn => turn.IsLive && turn.Kind != AgentConversationEntryKind.Message);
        if (liveEntry is null || liveEntry.Kind != kind)
        {
            liveEntry = new AgentConversationTurn
            {
                Role = AgentConversationRole.System,
                Kind = kind,
                Content = update.Content,
                IsExpanded = false,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            Conversation.Add(liveEntry);
            liveEntry.StartLive();
        }
        else
        {
            liveEntry.UpdateContent($"{liveEntry.Content}{Environment.NewLine}{update.Content}");
        }
    }

    private static AgentConversationTurn CreateTurn(AgentConversationRole role, string content, AgentConversationEntryKind kind = AgentConversationEntryKind.Message)
        => new()
        {
            Role = role,
            Kind = kind,
            Content = content,
            CreatedAt = DateTimeOffset.UtcNow,
        };
}
