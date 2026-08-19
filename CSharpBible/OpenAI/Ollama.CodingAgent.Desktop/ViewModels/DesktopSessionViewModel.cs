using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ollama.CodingAgent.Application.Diagnostics;
using Ollama.CodingAgent.Application.Extensions;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Desktop.Models;
using Ollama.CodingAgent.Desktop.Services;

namespace Ollama.CodingAgent.Desktop.ViewModels;

/// <summary>
/// Adapts shared session state and currently available local wiki services for desktop presentation.
/// </summary>
public sealed partial class DesktopSessionViewModel : ObservableObject
{
    private readonly IAgentApprovalService _approvalService;
    private readonly LocalKnowledgeBaseStore _knowledgeBaseStore;
    private readonly LocalWikiMarkdownImporter _wikiImporter;
    private readonly DesktopConfigurationState _configurationState;
    private readonly DesktopConfigurationStore _configurationStore;
    private readonly DesktopOllamaEndpointService _endpointService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopSessionViewModel"/> class.
    /// </summary>
    public DesktopSessionViewModel(
        AgentSessionViewModel session,
        IAgentApprovalService approvalService,
        LocalKnowledgeBaseStore knowledgeBaseStore,
        LocalWikiMarkdownImporter wikiImporter,
        DesktopOptions options)
        : this(
            session,
            approvalService,
            knowledgeBaseStore,
            wikiImporter,
            options,
            CreateConfigurationState(options),
            new DesktopConfigurationStore(),
            new DesktopOllamaEndpointService())
    {
    }

    private static DesktopConfigurationState CreateConfigurationState(DesktopOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new DesktopConfigurationState(new DesktopConfiguration
        {
            Endpoint = options.Endpoint,
            Model = options.Model,
            WorkspacePath = options.WorkspacePath,
        });
    }

    public DesktopSessionViewModel(
        AgentSessionViewModel session,
        IAgentApprovalService approvalService,
        LocalKnowledgeBaseStore knowledgeBaseStore,
        LocalWikiMarkdownImporter wikiImporter,
        DesktopOptions options,
        DesktopConfigurationState configurationState,
        DesktopConfigurationStore configurationStore,
        DesktopOllamaEndpointService endpointService)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
        _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));
        _knowledgeBaseStore = knowledgeBaseStore ?? throw new ArgumentNullException(nameof(knowledgeBaseStore));
        _wikiImporter = wikiImporter ?? throw new ArgumentNullException(nameof(wikiImporter));
        ArgumentNullException.ThrowIfNull(options);
        _configurationState = configurationState ?? throw new ArgumentNullException(nameof(configurationState));
        _configurationStore = configurationStore ?? throw new ArgumentNullException(nameof(configurationStore));
        _endpointService = endpointService ?? throw new ArgumentNullException(nameof(endpointService));

        EditableEndpoint = _configurationState.Current.Endpoint;
        EditableModel = _configurationState.Current.Model;
        EditableWorkspacePath = _configurationState.Current.WorkspacePath;
        AvailableModels.Add(EditableModel);
        CodeWikiVaultPath = options.CodeWikiVaultPath;
        Session.PropertyChanged += OnSessionPropertyChanged;
        RefreshApprovals();
    }

    /// <summary>
    /// Gets the shared application session. Conversation, prompt, and session commands are delegated to it.
    /// </summary>
    public AgentSessionViewModel Session { get; }

    /// <summary>
    /// Gets the launch endpoint displayed by the configuration panel.
    /// </summary>
    public string Endpoint => _configurationState.Current.Endpoint;

    /// <summary>
    /// Gets the launch model displayed by the configuration panel.
    /// </summary>
    public string Model => _configurationState.Current.Model;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyConfigurationCommand))]
    [NotifyCanExecuteChangedFor(nameof(TestEndpointCommand))]
    private string _editableEndpoint;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyConfigurationCommand))]
    private string _editableModel;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyConfigurationCommand))]
    private string _editableWorkspacePath;

    public ObservableCollection<string> AvailableModels { get; } = [];

    public ObservableCollection<DesktopConfiguration> RecentConfigurations { get; } = [];

    [ObservableProperty]
    private DesktopConfiguration? _selectedRecentConfiguration;

    partial void OnSelectedRecentConfigurationChanged(DesktopConfiguration? value)
    {
        SelectRecentConfiguration(value);
    }

    [ObservableProperty]
    private string _configurationStatus = "Configuration is ready.";

    [ObservableProperty]
    private bool _isTestingEndpoint;

    [RelayCommand]
    private async Task LoadRecentConfigurationsAsync()
    {
        RecentConfigurations.Clear();
        foreach (DesktopConfiguration configuration in await _configurationStore.LoadAsync())
        {
            RecentConfigurations.Add(configuration);
        }
    }

    [RelayCommand]
    private void SelectRecentConfiguration(DesktopConfiguration? configuration)
    {
        if (configuration is null)
        {
            return;
        }

        EditableEndpoint = configuration.Endpoint;
        EditableModel = configuration.Model;
        EditableWorkspacePath = configuration.WorkspacePath;
        ConfigurationStatus = "Recent configuration loaded for editing.";
    }

    [RelayCommand(CanExecute = nameof(CanTestEndpoint))]
    private async Task TestEndpointAsync()
    {
        IsTestingEndpoint = true;
        ConfigurationStatus = "Testing Ollama endpoint.";
        bool endpointChanged = !string.Equals(Endpoint, EditableEndpoint.Trim(), StringComparison.OrdinalIgnoreCase);
        if (endpointChanged)
        {
            AvailableModels.Clear();
        }

        try
        {
            IReadOnlyList<string> models = await _endpointService.GetModelsAsync(EditableEndpoint, CancellationToken.None);
            AvailableModels.Clear();
            foreach (string model in models)
            {
                AvailableModels.Add(model);
            }

            if (!AvailableModels.Contains(EditableModel, StringComparer.OrdinalIgnoreCase))
            {
                EditableModel = AvailableModels.FirstOrDefault() ?? EditableModel;
            }

            ConfigurationStatus = $"Endpoint reachable; {AvailableModels.Count} model(s) loaded.";
        }
        catch (Exception exception)
        {
            ConfigurationStatus = $"Endpoint test failed: {exception.Message}";
        }
        finally
        {
            IsTestingEndpoint = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplyConfiguration))]
    private async Task ApplyConfigurationAsync()
    {
        try
        {
            DesktopConfiguration configuration = new DesktopConfiguration
            {
                Endpoint = EditableEndpoint,
                Model = EditableModel,
                WorkspacePath = EditableWorkspacePath,
            }.Normalize();
            _configurationState.Set(configuration);
            OnPropertyChanged(nameof(Endpoint));
            OnPropertyChanged(nameof(Model));
            Session.WorkspacePath = configuration.WorkspacePath;
            RecentConfigurations.Clear();
            foreach (DesktopConfiguration recent in await _configurationStore.RememberAsync(configuration))
            {
                RecentConfigurations.Add(recent);
            }

            ConfigurationStatus = "Configuration applied for the next prompt.";
        }
        catch (Exception exception)
        {
            ConfigurationStatus = $"Configuration could not be applied: {exception.Message}";
        }
    }

    private bool CanTestEndpoint() => !IsTestingEndpoint && !string.IsNullOrWhiteSpace(EditableEndpoint);

    private bool CanApplyConfiguration()
        => !string.IsNullOrWhiteSpace(EditableEndpoint)
            && !string.IsNullOrWhiteSpace(EditableModel)
            && !string.IsNullOrWhiteSpace(EditableWorkspacePath);

    /// <summary>
    /// Gets the shared session's current visible workflow status.
    /// </summary>
    public string Status => Session.Status;

    /// <summary>
    /// Gets the shared session's latest runtime error.
    /// </summary>
    public string? ErrorMessage => Session.ErrorMessage;

    /// <summary>
    /// Gets a value indicating whether the shared session has a visible runtime error.
    /// </summary>
    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(ErrorMessage);

    /// <summary>
    /// Gets the configured CodeWikiVault location.
    /// </summary>
    [ObservableProperty]
    private string _codeWikiVaultPath;

    /// <summary>
    /// Gets the wiki operation outcome intended for desktop presentation.
    /// </summary>
    [ObservableProperty]
    private string _wikiStatus = "No CodeWikiVault operation has been run.";

    /// <summary>
    /// Gets the current wiki search text.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchWikiCommand))]
    private string _wikiQuery = string.Empty;

    /// <summary>
    /// Gets the current pending approval requests as an observable desktop list.
    /// </summary>
    public ObservableCollection<AgentApprovalRequest> PendingApprovals { get; } = [];

    /// <summary>
    /// Gets wiki entries returned by the latest search.
    /// </summary>
    public ObservableCollection<LocalKnowledgeEntry> WikiSearchResults { get; } = [];

    /// <summary>
    /// Gets the current limitation message for planning state.
    /// </summary>
    public string PlanActivityMessage => "Planning activity is represented by the visible Thinking entries in the transcript.";

    /// <summary>
    /// Gets the current limitation message for tool state.
    /// </summary>
    public string ToolActivityMessage => "Tool calls are sent through the native Ollama tool contract when a tool-enabled loop is active.";

    /// <summary>
    /// Gets the current limitation message for Git state.
    /// </summary>
    public string GitStatusMessage => "Git readiness and repository status are not exposed to desktop clients yet.";

    /// <summary>
    /// Copies newly queued shared approvals into the observable desktop projection.
    /// </summary>
    [RelayCommand]
    private void RefreshApprovals()
    {
        PendingApprovals.Clear();
        foreach (AgentApprovalRequest request in _approvalService.PendingRequests)
        {
            PendingApprovals.Add(request);
        }
    }

    /// <summary>
    /// Approves one pending shared operation.
    /// </summary>
    [RelayCommand]
    private void Approve(AgentApprovalRequest? request)
    {
        ResolveApproval(request, approved: true);
    }

    /// <summary>
    /// Rejects one pending shared operation.
    /// </summary>
    [RelayCommand]
    private void Reject(AgentApprovalRequest? request)
    {
        ResolveApproval(request, approved: false);
    }

    /// <summary>
    /// Imports Markdown pages from the configured CodeWikiVault into the workspace-local knowledge store.
    /// </summary>
    [RelayCommand]
    private async Task ImportCodeWikiVaultAsync()
    {
        try
        {
            LocalWikiImportResult result = await _wikiImporter.ImportAsync(CodeWikiVaultPath, _knowledgeBaseStore);
            WikiStatus = $"Imported {result.ImportedCount} Markdown pages; skipped {result.SkippedCount}.";
        }
        catch (Exception ex)
        {
            WikiStatus = $"CodeWikiVault import failed: {ex.Message}";
        }
    }

    /// <summary>
    /// Searches the workspace-local knowledge store.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSearchWiki))]
    private async Task SearchWikiAsync()
    {
        try
        {
            var results = await _knowledgeBaseStore.SearchAsync(WikiQuery.Trim());
            WikiSearchResults.Clear();
            foreach (LocalKnowledgeEntry entry in results)
            {
                WikiSearchResults.Add(entry);
            }

            WikiStatus = $"Found {results.Count} wiki result(s).";
        }
        catch (Exception ex)
        {
            WikiStatus = $"Wiki search failed: {ex.Message}";
        }
    }

    private bool CanSearchWiki()
        => !string.IsNullOrWhiteSpace(WikiQuery);

    private void ResolveApproval(AgentApprovalRequest? request, bool approved)
    {
        if (request is null)
        {
            return;
        }

        bool resolved = Session.ResolveApproval(request.Id, approved);
        if (!resolved)
        {
            WikiStatus = "The selected approval is no longer pending.";
        }

        RefreshApprovals();
        OnPropertyChanged(nameof(Status));
    }

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is nameof(AgentSessionViewModel.Status)
            or nameof(AgentSessionViewModel.ErrorMessage))
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasErrorMessage));
        }

        if (eventArgs.PropertyName == nameof(AgentSessionViewModel.IsRunning)
            && !Session.IsRunning)
        {
            RefreshApprovals();
        }
    }
}
