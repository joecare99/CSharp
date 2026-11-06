using AA98_AvlnCodeStudio.Base.AI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Config.UI.Avalonia.ViewModels;
using Property.Editor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>Chooses which connection fields to inspect without selecting the active provider.</summary>
public enum AIProfileInspection
{
    ActiveConnection,
    Custom,
    OpenAI,
    Ollama,
    IFM,
}

/// <summary>Provides a display-only navigation action for a profile validation error.</summary>
public sealed class AIProfileIssueViewModel
{
    internal AIProfileIssueViewModel(AIProfileEndpointIssue issue, Action<AIProviderPreset> inspect, Func<AIProviderPreset, bool> canInspect)
    {
        Issue = issue;
        InspectCommand = new RelayCommand(() => inspect(Issue.Provider), () => canInspect(Issue.Provider));
    }

    public AIProfileEndpointIssue Issue { get; }
    public string Description => $"{Issue.Provider} profile: {Issue.Message}";
    public string InspectLabel => $"Inspect {Issue.Provider} profile";
    public IRelayCommand InspectCommand { get; }
}

/// <summary>Shows the effective AI connection for a configuration draft without persisting it.</summary>
public sealed class AIProviderDraftViewModel : ObservableObject, IDisposable
{
    private readonly ConfigUiViewModel _config;
    private readonly HashSet<IPropertyItem> _observed = [];
    private IReadOnlyList<AIProviderTemplate>? _templates;
    private bool _showAllProviderFields;
    private AIProfileInspection _inspectedProfile;
    private IReadOnlyList<AIProfileIssueViewModel> _profileIssues = [];
    private bool _disposed;
    private long _navigationRevision;

    public AIProviderDraftViewModel(
        ConfigUiViewModel config,
        CodeStudioSettings settings)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        ArgumentNullException.ThrowIfNull(settings);
        _templates = CopyTemplates(settings.KnownAIProviderTemplates);
        UseTemplateEndpointCommand = new RelayCommand(UseTemplateEndpoint, CanUseTemplateEndpoint);
        UseTemplateModelCommand = new RelayCommand(UseTemplateModel, CanUseTemplateModel);
        CopyLegacyEndpointCommand = new RelayCommand(CopyLegacyEndpoint, CanCopyLegacyEndpoint);
        CopyLegacyModelCommand = new RelayCommand(CopyLegacyModel, CanCopyLegacyModel);
        ShowInvalidProfileFieldsCommand = new RelayCommand(ShowInvalidProfileFields, CanShowInvalidProfileFields);
        _config.PropertyChanged += OnConfigChanged;
        _config.PropertyEditor.Items.CollectionChanged += OnItemsChanged;
        ObserveItems();
        Refresh();
    }

    public IRelayCommand UseTemplateEndpointCommand { get; }
    public IRelayCommand UseTemplateModelCommand { get; }
    public IRelayCommand CopyLegacyEndpointCommand { get; }
    public IRelayCommand CopyLegacyModelCommand { get; }
    public IRelayCommand ShowInvalidProfileFieldsCommand { get; }
    public IReadOnlyList<AIProfileInspection> InspectionOptions { get; } = Enum.GetValues<AIProfileInspection>();
    public event Action<AIProviderPreset, long>? ProfileEndpointNavigationRequested;

    public bool IsProfileNavigationCurrent(AIProviderPreset provider, long revision) =>
        revision == _navigationRevision && !ShowAllProviderFields && CanInspectIssue(provider);

    public AIProfileInspection InspectedProfile
    {
        get => _inspectedProfile;
        set
        {
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (SetProperty(ref _inspectedProfile, value))
            {
                _navigationRevision++;
                Refresh();
            }
        }
    }

    public bool IsVisible => _config.SelectedSection?.Section.Name == "Workbench"
        && _config.IsSelectedSectionReady;
    public bool IsBusy => _config.IsBusy;

    public string Summary
    {
        get
        {
            if (!TryGetDraft(out AIProviderPreset provider, out IPropertyItem? endpoint, out IPropertyItem? model))
            {
                return "AI connection preview unavailable.";
            }

            AIProviderTemplate? template = FindTemplate(provider);
            string endpointValue = endpoint.Value as string ?? string.Empty;
            string modelValue = model.Value as string ?? string.Empty;
            bool endpointOverride = !string.IsNullOrWhiteSpace(endpointValue);
            bool modelOverride = !string.IsNullOrWhiteSpace(modelValue);
            string? effectiveEndpoint = endpointOverride ? endpointValue : template?.Endpoint;
            string? effectiveModel = modelOverride ? modelValue : template?.DefaultModelId;
            string endpointText = FormatEndpoint(effectiveEndpoint);
            string modelText = string.IsNullOrWhiteSpace(effectiveModel) ? "(not configured)" : effectiveModel;
            string warning = provider != AIProviderPreset.Custom && template is null
                    ? " Provider template is unavailable."
                    : string.Empty;
            string endpointSource = endpointOverride ? "override" : provider == AIProviderPreset.Custom ? "unset" : "template";
            string modelSource = modelOverride ? "override" : provider == AIProviderPreset.Custom ? "unset" : "template";
            bool useProfiles = UsesProfiles(_config.SelectedSection);
            return $"Draft: {provider} ({(useProfiles ? "provider profile" : "legacy global overrides")}) | Endpoint: {endpointText} ({endpointSource}) | Model: {modelText} ({modelSource}).{warning}";
        }
    }

    public string Hint => "Inspect profile only changes which fields are shown. Empty fields use the provider template; Custom needs an endpoint. Template and copy actions target the active connection and are disabled during profile inspection. Credentials use the active connection; Save/Delete are immediate.";

    public bool CanManageActiveCredentials => IsVisible && !_config.IsBusy && !_config.HasUnsavedChanges;

    public string ProfileValidationMessage
    {
        get
        {
            if (!IsVisible || _config.SelectedSection is not { } section)
            {
                return string.Empty;
            }

            IReadOnlyList<AIProfileEndpointIssue> issues = WorkbenchSettingsValidation.GetInvalidProfileEndpoints(section);
            return issues.Count == 0 ? string.Empty : string.Join(Environment.NewLine,
                issues.Select(static issue => $"{issue.Provider} profile: {issue.Message}"));
        }
    }

    public bool HasInvalidProfileEndpoints => !string.IsNullOrEmpty(ProfileValidationMessage);
    public IReadOnlyList<AIProfileIssueViewModel> ProfileIssues => _profileIssues;

    /// <summary>Shows all connection fields without changing the selected provider or the settings draft.</summary>
    public bool ShowAllProviderFields
    {
        get => _showAllProviderFields;
        set
        {
            if (SetProperty(ref _showAllProviderFields, value))
            {
                _navigationRevision++;
                UpdateVisibleProperties();
                ShowInvalidProfileFieldsCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string ActionMessage { get; private set; } = string.Empty;

    /// <summary>Rechecks the effective preview before persistence and requires review if it changed.</summary>
    public async Task<string?> ConfirmCurrentTemplatesAsync(
        IReadOnlyList<global::Config.UI.IConfigUiSection> sections,
        Func<Task<IReadOnlyList<AIProviderTemplate>?>> loadTemplatesAsync)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(loadTemplatesAsync);
        global::Config.UI.IConfigUiSection? section = sections.SingleOrDefault(
            section => section.Section.Name == "Workbench");
        if (section is null
            || !TryGetDraft(section, out AIProviderPreset provider, out IPropertyItem endpoint, out IPropertyItem model))
        {
            return null;
        }

        AIProviderTemplate? oldTemplate = FindTemplate(provider);
        IReadOnlyList<AIProviderTemplate>? updated = CopyTemplates(await loadTemplatesAsync());
        AIProviderTemplate? newTemplate = FindTemplate(updated, provider);
        bool endpointChanged = string.IsNullOrWhiteSpace(endpoint.Value as string)
            && !string.Equals(oldTemplate?.Endpoint, newTemplate?.Endpoint, StringComparison.Ordinal);
        bool modelChanged = string.IsNullOrWhiteSpace(model.Value as string)
            && !string.Equals(oldTemplate?.DefaultModelId, newTemplate?.DefaultModelId, StringComparison.Ordinal);
        _templates = updated;
        Refresh();
        return endpointChanged || modelChanged
            ? "The selected provider template changed. Review the updated AI connection preview and apply again."
            : null;
    }

    public void Dispose()
    {
        _disposed = true;
        foreach (AIProfileIssueViewModel issue in _profileIssues)
        {
            issue.InspectCommand.NotifyCanExecuteChanged();
        }
        _config.PropertyChanged -= OnConfigChanged;
        _config.PropertyEditor.Items.CollectionChanged -= OnItemsChanged;
        foreach (var item in _config.PropertyEditor.Items)
        {
            item.IsVisible = true;
        }
        foreach (IPropertyItem item in _observed)
        {
            item.ValueChanged -= OnValueChanged;
        }
        _observed.Clear();
    }

    private bool TryGetDraft(out AIProviderPreset provider, out IPropertyItem endpoint, out IPropertyItem model)
    {
        if (IsVisible)
        {
            return TryGetDraft(_config.SelectedSection, out provider, out endpoint, out model);
        }
        provider = default;
        endpoint = null!;
        model = null!;
        return false;
    }

    private static bool TryGetDraft(
        global::Config.UI.IConfigUiSection? section,
        out AIProviderPreset provider, out IPropertyItem endpoint, out IPropertyItem model)
    {
        provider = default;
        endpoint = null!;
        model = null!;
        if (section?.Section.Name != "Workbench"
            || section.State != global::Config.UI.ConfigUiSectionState.Ready)
        {
            return false;
        }

        var properties = section.Properties;
        IPropertyItem? providerItem = properties.FirstOrDefault(item => item.Name == nameof(CodeStudioSettings.AIProvider));
        if (providerItem?.Value is not AIProviderPreset value || !Enum.IsDefined(value))
        {
            return false;
        }

        bool useProfiles = UsesProfiles(section);
        string endpointName = useProfiles ? CodeStudioSettings.GetProfileEndpointPropertyName(value) : nameof(CodeStudioSettings.AIEndpoint);
        string modelName = useProfiles ? CodeStudioSettings.GetProfileModelPropertyName(value) : nameof(CodeStudioSettings.AIModelId);
        IPropertyItem? endpointItem = properties.FirstOrDefault(item => item.Name == endpointName);
        IPropertyItem? modelItem = properties.FirstOrDefault(item => item.Name == modelName);
        if (endpointItem is null || modelItem is null)
        {
            return false;
        }

        provider = value;
        endpoint = endpointItem;
        model = modelItem;
        return true;
    }

    private static bool UsesProfiles(global::Config.UI.IConfigUiSection? section) =>
        section?.Properties.FirstOrDefault(item => item.Name == nameof(CodeStudioSettings.UseAIProviderProfiles))?.Value is true;

    private AIProviderTemplate? FindTemplate(AIProviderPreset provider) => FindTemplate(_templates, provider);

    private static AIProviderTemplate? FindTemplate(IReadOnlyList<AIProviderTemplate>? templates, AIProviderPreset provider) =>
        provider == AIProviderPreset.Custom ? null : templates?.FirstOrDefault(
            item => item is not null && string.Equals(item.Id, provider.ToString(), StringComparison.OrdinalIgnoreCase));

    private static AIProviderTemplate[]? CopyTemplates(IReadOnlyList<AIProviderTemplate>? templates) =>
        templates?.Where(static item => item is not null).Select(static item => new AIProviderTemplate
        {
            Id = item.Id,
            DisplayName = item.DisplayName,
            Endpoint = item.Endpoint,
            DefaultModelId = item.DefaultModelId,
        }).ToArray();

    private bool CanUseTemplateEndpoint() =>
        !_config.IsBusy && InspectedProfile == AIProfileInspection.ActiveConnection
        && TryGetDraft(out AIProviderPreset provider, out IPropertyItem endpoint, out _)
        && endpoint.IsEditable && !string.IsNullOrWhiteSpace(endpoint.Value as string)
        && !string.IsNullOrWhiteSpace(FindTemplate(provider)?.Endpoint);

    private bool CanUseTemplateModel() =>
        !_config.IsBusy && InspectedProfile == AIProfileInspection.ActiveConnection
        && TryGetDraft(out AIProviderPreset provider, out _, out IPropertyItem model)
        && model.IsEditable && !string.IsNullOrWhiteSpace(model.Value as string)
        && !string.IsNullOrWhiteSpace(FindTemplate(provider)?.DefaultModelId);

    private void UseTemplateEndpoint()
    {
        if (CanUseTemplateEndpoint() && TryGetDraft(out _, out IPropertyItem endpoint, out _))
        {
            ApplyEmptyOverride(endpoint);
        }
    }

    private void UseTemplateModel()
    {
        if (CanUseTemplateModel() && TryGetDraft(out _, out _, out IPropertyItem model))
        {
            ApplyEmptyOverride(model);
        }
    }

    private void ApplyEmptyOverride(IPropertyItem item)
    {
        ActionMessage = item.TrySetValue(string.Empty)
            ? string.Empty
            : item.ValidationResult.ErrorMessage ?? $"Could not update {item.DisplayName}.";
        OnPropertyChanged(nameof(ActionMessage));
    }

    private bool CanCopyLegacyEndpoint() => CanCopyLegacy(nameof(CodeStudioSettings.AIEndpoint), true);

    private bool CanCopyLegacyModel() => CanCopyLegacy(nameof(CodeStudioSettings.AIModelId), false);

    private bool CanShowInvalidProfileFields() => !_config.IsBusy && HasInvalidProfileEndpoints && !ShowAllProviderFields;

    private void ShowInvalidProfileFields() => ShowAllProviderFields = true;

    private bool CanInspectIssue(AIProviderPreset provider) => !_disposed && IsVisible && !_config.IsBusy
        && _profileIssues.Any(item => item.Issue.Provider == provider);

    private void InspectIssue(AIProviderPreset provider)
    {
        if (!CanInspectIssue(provider))
        {
            return;
        }

        AIProfileInspection inspection = provider switch
        {
            AIProviderPreset.Custom => AIProfileInspection.Custom,
            AIProviderPreset.OpenAI => AIProfileInspection.OpenAI,
            AIProviderPreset.Ollama => AIProfileInspection.Ollama,
            AIProviderPreset.IFM => AIProfileInspection.IFM,
            _ => throw new ArgumentOutOfRangeException(nameof(provider)),
        };
        ShowAllProviderFields = false;
        InspectedProfile = inspection;
        ProfileEndpointNavigationRequested?.Invoke(provider, ++_navigationRevision);
    }

    private bool CanCopyLegacy(string sourceName, bool endpoint)
    {
        if (_config.IsBusy || InspectedProfile != AIProfileInspection.ActiveConnection || !UsesProfiles(_config.SelectedSection)
            || !TryGetDraft(out _, out IPropertyItem targetEndpoint, out IPropertyItem targetModel))
        {
            return false;
        }

        IPropertyItem target = endpoint ? targetEndpoint : targetModel;
        IPropertyItem? source = _config.SelectedSection?.Properties.FirstOrDefault(item => item.Name == sourceName);
        return target.IsEditable && string.IsNullOrWhiteSpace(target.Value as string)
            && !string.IsNullOrWhiteSpace(source?.Value as string);
    }

    private void CopyLegacyEndpoint() => CopyLegacy(nameof(CodeStudioSettings.AIEndpoint), true);

    private void CopyLegacyModel() => CopyLegacy(nameof(CodeStudioSettings.AIModelId), false);

    private void CopyLegacy(string sourceName, bool endpoint)
    {
        if (!CanCopyLegacy(sourceName, endpoint)
            || !TryGetDraft(out _, out IPropertyItem targetEndpoint, out IPropertyItem targetModel))
        {
            return;
        }

        IPropertyItem target = endpoint ? targetEndpoint : targetModel;
        string source = (string)_config.SelectedSection!.Properties.First(item => item.Name == sourceName).Value!;
        ActionMessage = target.TrySetValue(source)
            ? string.Empty
            : target.ValidationResult.ErrorMessage ?? $"Could not update {target.DisplayName}.";
        OnPropertyChanged(nameof(ActionMessage));
    }

    private static string FormatEndpoint(string? endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return "(not configured)";
        }
        try
        {
            Uri uri = AIEndpointValidator.Validate(endpoint);
            return uri.GetLeftPart(UriPartial.Path) + (string.IsNullOrEmpty(uri.Query) ? string.Empty : " (query hidden)");
        }
        catch (InvalidOperationException)
        {
            return "(invalid; see endpoint field)";
        }
    }

    private void OnConfigChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(ConfigUiViewModel.SelectedSection))
        {
            _navigationRevision++;
        }
        Refresh();
    }
    private void OnValueChanged(object? sender, PropertyValueChangedEventArgs args)
    {
        _navigationRevision++;
        Refresh();
    }
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        _navigationRevision++;
        ObserveItems();
        Refresh();
    }

    private void ObserveItems()
    {
        foreach (IPropertyItem item in _observed)
        {
            item.ValueChanged -= OnValueChanged;
        }
        _observed.Clear();
        foreach (var item in _config.PropertyEditor.Items)
        {
            if (_observed.Add(item.PropertyItem))
            {
                item.PropertyItem.ValueChanged += OnValueChanged;
            }
        }
    }

    private void Refresh()
    {
        UpdateVisibleProperties();
        IReadOnlyList<AIProfileEndpointIssue> issues = IsVisible && _config.SelectedSection is { } section
            ? WorkbenchSettingsValidation.GetInvalidProfileEndpoints(section)
            : [];
        if (!_profileIssues.Select(static item => item.Issue).SequenceEqual(issues))
        {
            _profileIssues = issues.Select(issue => new AIProfileIssueViewModel(issue, InspectIssue, CanInspectIssue)).ToArray();
            OnPropertyChanged(nameof(ProfileIssues));
        }
        foreach (AIProfileIssueViewModel issue in _profileIssues)
        {
            issue.InspectCommand.NotifyCanExecuteChanged();
        }
        OnPropertyChanged(nameof(IsVisible));
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(Summary));
        OnPropertyChanged(nameof(CanManageActiveCredentials));
        OnPropertyChanged(nameof(ProfileValidationMessage));
        OnPropertyChanged(nameof(HasInvalidProfileEndpoints));
        ShowInvalidProfileFieldsCommand.NotifyCanExecuteChanged();
        UseTemplateEndpointCommand.NotifyCanExecuteChanged();
        UseTemplateModelCommand.NotifyCanExecuteChanged();
        CopyLegacyEndpointCommand.NotifyCanExecuteChanged();
        CopyLegacyModelCommand.NotifyCanExecuteChanged();
    }

    private void UpdateVisibleProperties()
    {
        if (_config.SelectedSection?.Section.Name != "Workbench"
            || _config.SelectedSection.State != global::Config.UI.ConfigUiSectionState.Ready)
        {
            return;
        }

        IPropertyItem? providerItem = _config.SelectedSection.Properties.FirstOrDefault(
            item => item.Name == nameof(CodeStudioSettings.AIProvider));
        if (providerItem?.Value is not AIProviderPreset provider || !Enum.IsDefined(provider))
        {
            return;
        }

        bool useProfiles = UsesProfiles(_config.SelectedSection);
        AIProviderPreset? inspection = InspectedProfile switch
        {
            AIProfileInspection.ActiveConnection => null,
            AIProfileInspection.Custom => AIProviderPreset.Custom,
            AIProfileInspection.OpenAI => AIProviderPreset.OpenAI,
            AIProfileInspection.Ollama => AIProviderPreset.Ollama,
            AIProfileInspection.IFM => AIProviderPreset.IFM,
            _ => throw new InvalidOperationException("Unknown profile inspection selection."),
        };
        string profileEndpoint = CodeStudioSettings.GetProfileEndpointPropertyName(inspection ?? provider);
        string profileModel = CodeStudioSettings.GetProfileModelPropertyName(inspection ?? provider);
        foreach (var item in _config.PropertyEditor.Items)
        {
            string name = item.PropertyItem.Name;
            if (name is nameof(CodeStudioSettings.AIEndpoint) or nameof(CodeStudioSettings.AIModelId))
            {
                item.IsVisible = ShowAllProviderFields || (inspection is null && !useProfiles);
            }
            else if (name.StartsWith("AIProfile", StringComparison.Ordinal))
            {
                item.IsVisible = ShowAllProviderFields || ((inspection is not null || useProfiles) && (name == profileEndpoint || name == profileModel));
            }
        }
    }
}
