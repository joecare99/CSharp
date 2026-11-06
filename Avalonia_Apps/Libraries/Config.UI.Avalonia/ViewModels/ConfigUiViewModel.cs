using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Config.UI.Avalonia.ViewModels;

/// <summary>
/// Provides selection and lifecycle commands for registered configuration sections.
/// </summary>
public sealed partial class ConfigUiViewModel : ObservableObject
{
    private readonly PropertyEditorViewModel _propertyEditor;
    private readonly HashSet<IConfigUiSection> _loadedSections = [];
    private readonly HashSet<IConfigUiSection> _sectionsWithUnsavedChanges = [];
    private readonly HashSet<IPropertyItem> _subscribedProperties = [];
    private bool _isApplying;

    /// <summary>
    /// Initializes the configuration UI state from a host registry.
    /// </summary>
    public ConfigUiViewModel(
        IConfigUiSectionRegistry registry,
        PropertyEditorViewModel propertyEditor,
        string dialogTitle = "Settings")
    {
        ArgumentNullException.ThrowIfNull(registry);
        _propertyEditor = propertyEditor ?? throw new ArgumentNullException(nameof(propertyEditor));
        DialogTitle = string.IsNullOrWhiteSpace(dialogTitle)
            ? throw new ArgumentException("A dialog title is required.", nameof(dialogTitle))
            : dialogTitle;

        foreach (IConfigUiSection section in registry.Sections)
        {
            Sections.Add(section);
        }

        SelectedSection = Sections.Count > 0 ? Sections[0] : null;
    }

    /// <summary>Gets registered sections in host-provided order.</summary>
    public ObservableCollection<IConfigUiSection> Sections { get; } = [];

    /// <summary>Gets the title used by a host dialog for this provider context.</summary>
    public string DialogTitle { get; }

    /// <summary>Gets the shared property editor state for the selected section.</summary>
    public PropertyEditorViewModel PropertyEditor => _propertyEditor;

    /// <summary>Gets whether an operation is currently in progress.</summary>
    public bool IsBusy => SelectedSection?.State is ConfigUiSectionState.Loading
        or ConfigUiSectionState.Saving
        or ConfigUiSectionState.Resetting
        || _isApplying;

    /// <summary>Gets the selected section's localized failure message.</summary>
    public string ErrorMessage => SelectedSection?.ErrorMessage ?? string.Empty;

    /// <summary>Gets the selected provider's scope and target description.</summary>
    public string SelectedContextDisplay
    {
        get
        {
            if (SelectedSection is not IPropertyProvider provider)
            {
                return string.Empty;
            }

            PropertyProviderContext? context = provider.Context;
            if (context is null)
            {
                return string.Empty;
            }

            string text = context.Scope == PropertyScope.Unspecified
                ? context.TargetDisplayName
                : $"{context.Scope}: {context.TargetDisplayName}";
            if (!string.Equals(context.ProviderDisplayName, context.TargetDisplayName, StringComparison.Ordinal))
            {
                text += $" - {context.ProviderDisplayName}";
            }
            if (!string.IsNullOrWhiteSpace(context.StorageLocation))
            {
                text += $" ({context.StorageLocation})";
            }

            if (!string.IsNullOrWhiteSpace(context.ReadOnlyReason))
            {
                text += $" - {context.ReadOnlyReason}";
            }

            return text;
        }
    }

    /// <summary>Gets the result of the most recent multi-section operation.</summary>
    public string OperationMessage { get; private set; } = string.Empty;

    /// <summary>Gets whether the selected section can be changed by the host.</summary>
    public bool CanSave => !_isApplying
        && SelectedSection is { IsReadOnly: false, State: ConfigUiSectionState.Ready };

    /// <summary>Gets whether pending changes can currently be applied.</summary>
    public bool CanApplyChanges => HasUnsavedChanges && !IsBusy;

    /// <summary>
    /// Gets whether the selected section has a local, unpersisted draft.
    /// Section changes preserve such drafts; explicit load and reset discard them.
    /// </summary>
    public bool HasUnsavedChanges => _sectionsWithUnsavedChanges.Count > 0;

    /// <summary>
    /// Invoked after a section has been persisted so the host can activate its new values.
    /// </summary>
    public Func<string, Task>? SectionAppliedAsync { get; set; }

    /// <summary>
    /// Optionally validates the complete set of changed sections before any section is saved.
    /// Return a user-facing message to keep the draft and cancel persistence. The callback
    /// must not mutate sections or persist settings; exceptions are displayed as validation errors.
    /// </summary>
    public Func<IReadOnlyList<IConfigUiSection>, Task<string?>>? ValidateChangesAsync { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBusy))]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    [NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
    private IConfigUiSection? _selectedSection;

    partial void OnSelectedSectionChanged(IConfigUiSection? value)
    {
        _propertyEditor.SetItems([]);
        RefreshState();
        if (value is null)
        {
            return;
        }

        if (_loadedSections.Contains(value))
        {
            DisplaySection(value);
            return;
        }

        _ = LoadSelectedSectionAsync(value);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (SelectedSection is not null)
        {
            await LoadSelectedSectionAsync(SelectedSection, discardSharedDraft: true);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (!CanSave || SelectedSection is not IConfigUiSection section)
        {
            return;
        }

        _isApplying = true;
        RefreshState();
        try
        {
            IConfigUiSection[] sectionsToValidate = Sections
                .Where(item => ReferenceEquals(item, section) || _sectionsWithUnsavedChanges.Contains(item))
                .ToArray();
            string? validationError = await ValidateDraftAsync(sectionsToValidate);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                OperationMessage = $"Settings were not saved: {validationError}";
                return;
            }

            await section.SaveAsync();
            if (section.State == ConfigUiSectionState.Ready)
            {
                _sectionsWithUnsavedChanges.Remove(section);
            }
        }
        finally
        {
            _isApplying = false;
            RefreshState();
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task ResetAsync()
    {
        if (!CanSave || SelectedSection is not IConfigUiSection section)
        {
            return;
        }

        await section.ResetDraftAsync();
        if (section.State == ConfigUiSectionState.Ready)
        {
            _sectionsWithUnsavedChanges.Add(section);
            _loadedSections.Add(section);
            if (ReferenceEquals(section, SelectedSection))
            {
                DisplaySection(section);
            }
        }

        RefreshState();
    }

    /// <summary>Persists every changed section and reports partial failures by section.</summary>
    public async Task<bool> ApplyChangesAsync()
    {
        if (_isApplying || _sectionsWithUnsavedChanges.Count == 0)
        {
            return !_isApplying;
        }

        _isApplying = true;
        OperationMessage = string.Empty;
        RefreshState();
        List<string> failures = [];
        List<string> savedSections = [];

        try
        {
            IConfigUiSection[] changedSections = Sections
                .Where(_sectionsWithUnsavedChanges.Contains)
                .ToArray();

            string[] validationFailures = changedSections
                .SelectMany(section => section.Properties
                    .Where(static property => !property.ValidationResult.IsValid)
                    .Select(property =>
                        $"{section.Section.DisplayName} - {property.DisplayName}: {property.ValidationResult.ErrorMessage ?? "The value is invalid."}"))
                .ToArray();
            if (validationFailures.Length > 0)
            {
                OperationMessage = $"Settings were not applied because some values are invalid:{Environment.NewLine}{string.Join(Environment.NewLine, validationFailures)}";
                return false;
            }

            string? validationError = await ValidateDraftAsync(changedSections);
            if (!string.IsNullOrWhiteSpace(validationError))
            {
                OperationMessage = $"Settings were not applied: {validationError}";
                return false;
            }

            foreach (IConfigUiSection section in changedSections)
            {
                try
                {
                    await section.SaveAsync();
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    failures.Add($"{section.Section.DisplayName}: {exception.Message}");
                    continue;
                }

                if (section.State != ConfigUiSectionState.Ready)
                {
                    failures.Add($"{section.Section.DisplayName}: {section.ErrorMessage ?? "The section could not be saved."}");
                    continue;
                }

                savedSections.Add(section.Section.DisplayName);
                if (SectionAppliedAsync is not null)
                {
                    try
                    {
                        await SectionAppliedAsync(section.Section.Name);
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        failures.Add($"{section.Section.DisplayName} was saved, but its runtime changes could not be activated: {exception.Message}");
                        continue;
                    }
                }

                _sectionsWithUnsavedChanges.Remove(section);
            }

            OperationMessage = failures.Count == 0
                ? "All changed settings were saved and activated."
                : $"Some settings were not fully applied. Saved sections: {(savedSections.Count == 0 ? "none" : string.Join(", ", savedSections))}.{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
            return failures.Count == 0;
        }
        finally
        {
            _isApplying = false;
            RefreshState();
        }
    }

    /// <summary>Discards unsaved edits by reloading all changed sections from storage.</summary>
    public async Task<bool> DiscardChangesAsync()
    {
        if (_isApplying)
        {
            return false;
        }

        OperationMessage = string.Empty;
        List<string> failures = [];
        HashSet<object> discardedDrafts = new(ReferenceEqualityComparer.Instance);
        foreach (IConfigUiSection section in Sections.Where(_sectionsWithUnsavedChanges.Contains).ToArray())
        {
            try
            {
                if (section is IConfigUiDraftDiscard sharedDraft)
                {
                    if (discardedDrafts.Add(sharedDraft.DraftIdentity))
                    {
                        await sharedDraft.DiscardDraftAsync();
                    }
                    else
                    {
                        await section.LoadAsync();
                    }
                }
                else
                {
                    await section.LoadAsync();
                }
                if (section.State == ConfigUiSectionState.Ready)
                {
                    _loadedSections.Add(section);
                    _sectionsWithUnsavedChanges.Remove(section);
                }
                else
                {
                    failures.Add($"{section.Section.DisplayName}: {section.ErrorMessage ?? "The section could not be reloaded."}");
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                failures.Add($"{section.Section.DisplayName}: {exception.Message}");
            }
        }

        OperationMessage = failures.Count == 0
            ? "Unapplied changes were discarded."
            : $"Some changes could not be discarded:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
        if (SelectedSection is not null && _loadedSections.Contains(SelectedSection))
        {
            DisplaySection(SelectedSection);
        }

        RefreshState();
        return failures.Count == 0;
    }

    private async Task LoadSelectedSectionAsync(IConfigUiSection section, bool discardSharedDraft = false)
    {
        RefreshState();
        try
        {
            if (discardSharedDraft && section is IConfigUiDraftDiscard sharedDraft)
            {
                await sharedDraft.DiscardDraftAsync();
                if (section.State == ConfigUiSectionState.Ready)
                {
                    foreach (IConfigUiSection relatedSection in Sections)
                    {
                        if (ReferenceEquals(relatedSection, section)
                            || relatedSection is not IConfigUiDraftDiscard)
                        {
                            continue;
                        }

                        await relatedSection.LoadAsync();
                        if (relatedSection.State == ConfigUiSectionState.Ready)
                        {
                            _loadedSections.Add(relatedSection);
                            _sectionsWithUnsavedChanges.Remove(relatedSection);
                        }
                    }
                }
            }
            else
            {
                await section.LoadAsync();
            }

            _loadedSections.Add(section);
            _sectionsWithUnsavedChanges.Remove(section);
            if (ReferenceEquals(section, SelectedSection))
            {
                DisplaySection(section);
            }
        }
        finally
        {
            RefreshState();
        }
    }

    private void RefreshState()
    {
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(SelectedContextDisplay));
        OnPropertyChanged(nameof(CanSave));
        OnPropertyChanged(nameof(HasUnsavedChanges));
        OnPropertyChanged(nameof(CanApplyChanges));
        OnPropertyChanged(nameof(OperationMessage));
        SaveCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
        ApplyCommand.NotifyCanExecuteChanged();
    }

    private async Task<string?> ValidateDraftAsync(IReadOnlyList<IConfigUiSection> sections)
    {
        if (ValidateChangesAsync is null)
        {
            return null;
        }

        try
        {
            return await ValidateChangesAsync(sections);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return $"Validation failed: {exception.Message}";
        }
    }

    private void DisplaySection(IConfigUiSection section)
    {
        _propertyEditor.SetItems(section.Properties);
        foreach (PropertyEditorItemViewModel property in _propertyEditor.Items)
        {
            if (_subscribedProperties.Add(property.PropertyItem))
            {
                property.PropertyItem.ValueChanged += (_, _) =>
                {
                    _sectionsWithUnsavedChanges.Add(section);
                    if (ReferenceEquals(section, SelectedSection))
                    {
                        OnPropertyChanged(nameof(HasUnsavedChanges));
                    }

                    OnPropertyChanged(nameof(CanApplyChanges));
                    ApplyCommand.NotifyCanExecuteChanged();
                };
            }
        }
    }

    private bool CanApply() => CanApplyChanges;

    [RelayCommand(CanExecute = nameof(CanApply))]
    private async Task Apply()
    {
        await ApplyChangesAsync();
    }
}
