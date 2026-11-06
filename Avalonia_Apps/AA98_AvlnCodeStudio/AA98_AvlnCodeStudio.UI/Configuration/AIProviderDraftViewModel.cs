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

/// <summary>Shows the effective AI connection for a configuration draft without persisting it.</summary>
public sealed class AIProviderDraftViewModel : ObservableObject, IDisposable
{
    private readonly ConfigUiViewModel _config;
    private readonly HashSet<IPropertyItem> _observed = [];
    private IReadOnlyList<AIProviderTemplate>? _templates;

    public AIProviderDraftViewModel(
        ConfigUiViewModel config,
        CodeStudioSettings settings)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        ArgumentNullException.ThrowIfNull(settings);
        _templates = CopyTemplates(settings.KnownAIProviderTemplates);
        UseTemplateEndpointCommand = new RelayCommand(UseTemplateEndpoint, CanUseTemplateEndpoint);
        UseTemplateModelCommand = new RelayCommand(UseTemplateModel, CanUseTemplateModel);
        _config.PropertyChanged += OnConfigChanged;
        _config.PropertyEditor.Items.CollectionChanged += OnItemsChanged;
        ObserveItems();
    }

    public IRelayCommand UseTemplateEndpointCommand { get; }
    public IRelayCommand UseTemplateModelCommand { get; }

    public bool IsVisible => _config.SelectedSection?.Section.Name == "Workbench"
        && _config.SelectedSection.State == global::Config.UI.ConfigUiSectionState.Ready;

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
            return $"Draft: {provider} | Endpoint: {endpointText} ({endpointSource}) | Model: {modelText} ({modelSource}).{warning}";
        }
    }

    public string Hint => "Apply rechecks templates; template buttons change the draft. Credentials use the active connection and Save/Delete are immediate. Apply or discard settings first.";

    public bool CanManageActiveCredentials => IsVisible && !_config.IsBusy && !_config.HasUnsavedChanges;

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
        _config.PropertyChanged -= OnConfigChanged;
        _config.PropertyEditor.Items.CollectionChanged -= OnItemsChanged;
        foreach (IPropertyItem item in _observed)
        {
            item.ValueChanged -= OnValueChanged;
        }
        _observed.Clear();
    }

    private bool TryGetDraft(out AIProviderPreset provider, out IPropertyItem endpoint, out IPropertyItem model)
        => TryGetDraft(_config.SelectedSection, out provider, out endpoint, out model);

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
        IPropertyItem? endpointItem = properties.FirstOrDefault(item => item.Name == nameof(CodeStudioSettings.AIEndpoint));
        IPropertyItem? modelItem = properties.FirstOrDefault(item => item.Name == nameof(CodeStudioSettings.AIModelId));
        if (providerItem?.Value is not AIProviderPreset value || !Enum.IsDefined(value)
            || endpointItem is null || modelItem is null)
        {
            return false;
        }

        provider = value;
        endpoint = endpointItem;
        model = modelItem;
        return true;
    }

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
        !_config.IsBusy && TryGetDraft(out AIProviderPreset provider, out IPropertyItem endpoint, out _)
        && endpoint.IsEditable && !string.IsNullOrWhiteSpace(endpoint.Value as string)
        && !string.IsNullOrWhiteSpace(FindTemplate(provider)?.Endpoint);

    private bool CanUseTemplateModel() =>
        !_config.IsBusy && TryGetDraft(out AIProviderPreset provider, out _, out IPropertyItem model)
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

    private void OnConfigChanged(object? sender, PropertyChangedEventArgs args) => Refresh();
    private void OnValueChanged(object? sender, PropertyValueChangedEventArgs args) => Refresh();
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
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
        OnPropertyChanged(nameof(IsVisible));
        OnPropertyChanged(nameof(Summary));
        OnPropertyChanged(nameof(CanManageActiveCredentials));
        UseTemplateEndpointCommand.NotifyCanExecuteChanged();
        UseTemplateModelCommand.NotifyCanExecuteChanged();
    }
}
