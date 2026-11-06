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
    private readonly HashSet<IConfigUiSection> _sectionsRequiringRefresh = [];
    private readonly HashSet<object> _draftsWithUncertainDiscard = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<IConfigUiSection, Dictionary<IPropertyItem, EventHandler<PropertyValueChangedEventArgs>>> _subscribedProperties =
        new(ReferenceEqualityComparer.Instance);
    private bool _isApplying;
    private int _loadingCount;

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
        || _isApplying || _loadingCount > 0;

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
    public bool CanSave => !IsBusy
        && SelectedSection is not null && !DraftNeedsRecovery(SelectedSection)
        && SelectedSection is { IsReadOnly: false, State: ConfigUiSectionState.Ready };

    /// <summary>Gets whether pending changes can currently be applied.</summary>
    public bool CanApplyChanges => HasUnsavedChanges && !IsBusy
        && !_sectionsWithUnsavedChanges.Any(DraftNeedsRecovery);

    /// <summary>
    /// Gets whether the selected section has a local, unpersisted draft.
    /// Section changes preserve such drafts; explicit load and reset discard them.
    /// </summary>
    public bool HasUnsavedChanges => _sectionsWithUnsavedChanges.Count > 0;

    /// <summary>Gets whether the selected section has current properties and its shared draft is usable.</summary>
    public bool IsSelectedSectionReady => SelectedSection is { State: ConfigUiSectionState.Ready } section
        && _loadedSections.Contains(section)
        && !DraftNeedsRecovery(section);

    /// <summary>Gets whether failed shared-draft operations require explicit recovery.</summary>
    public bool NeedsReloadRecovery => _sectionsRequiringRefresh.Count > 0;

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
    [NotifyPropertyChangedFor(nameof(IsSelectedSectionReady))]
    private IConfigUiSection? _selectedSection;

    partial void OnSelectedSectionChanged(IConfigUiSection? value)
    {
        _propertyEditor.SetItems([]);
        RefreshState();
        if (value is null)
        {
            return;
        }

        if (value is IConfigUiDraftDiscard sharedDraft && _draftsWithUncertainDiscard.Contains(sharedDraft.DraftIdentity))
        {
            OperationMessage = "The shared draft could not be discarded reliably. Retry discard/reload or Cancel before editing or applying it.";
            RefreshState();
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
        if (!IsBusy && SelectedSection is not null)
        {
            await LoadSelectedSectionAsync(SelectedSection, discardSharedDraft: true);
        }
    }

    [RelayCommand]
    private async Task RetryFailedReloadsAsync()
    {
        if (IsBusy || !NeedsReloadRecovery)
        {
            return;
        }

        _isApplying = true;
        RefreshState();
        try
        {
            List<string> failures = [];
            HashSet<object> attemptedDrafts = new(ReferenceEqualityComparer.Instance);
            foreach (IConfigUiSection section in Sections.Where(_sectionsRequiringRefresh.Contains).ToArray())
            {
                if (!_sectionsRequiringRefresh.Contains(section))
                {
                    continue;
                }

                bool retryDiscard = section is IConfigUiDraftDiscard shared
                    && _draftsWithUncertainDiscard.Contains(shared.DraftIdentity);
                if (retryDiscard && section is IConfigUiDraftDiscard draft && !attemptedDrafts.Add(draft.DraftIdentity))
                {
                    continue;
                }

                await LoadSelectedSectionAsync(section, discardSharedDraft: retryDiscard);
                if (!string.IsNullOrWhiteSpace(OperationMessage))
                {
                    failures.Add(OperationMessage);
                }
            }

            OperationMessage = failures.Count == 0 && !NeedsReloadRecovery
                ? "Affected sections were reloaded. Discarded changes cannot be recovered."
                : $"Some sections still require reloading:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
            if (SelectedSection is not null && _loadedSections.Contains(SelectedSection))
            {
                DisplaySection(SelectedSection);
            }
        }
        finally
        {
            _isApplying = false;
            RefreshState();
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
                if (ReferenceEquals(section, SelectedSection))
                {
                    DisplaySection(section);
                }
            }
        }
        finally
        {
            SynchronizeSectionProperties(section);
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

        _isApplying = true;
        RefreshState();
        bool resetCompleted = false;
        string? resetFailure = null;
        try
        {
            await section.ResetDraftAsync();
            if (section.State == ConfigUiSectionState.Ready)
            {
                resetCompleted = true;
                _sectionsWithUnsavedChanges.Add(section);
                _loadedSections.Add(section);
                if (ReferenceEquals(section, SelectedSection))
                {
                    DisplaySection(section);
                }
            }
            else
            {
                resetFailure = section.ErrorMessage;
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            resetFailure = exception.Message;
        }
        finally
        {
            if (!resetCompleted)
            {
                _sectionsWithUnsavedChanges.Add(section);
                if (section is IConfigUiDraftDiscard sharedDraft)
                {
                    MarkDraftUncertain(sharedDraft.DraftIdentity);
                    OperationMessage = "The shared draft reset did not complete and may have changed the draft. Retry discard/reload or Cancel; other unsaved changes in this draft will also be discarded.";
                }
                else
                {
                    MarkSectionForRefresh(section);
                    OperationMessage = "The draft reset did not complete and may have changed the draft. Retry discard/reload or Cancel; unsaved changes in this section will be discarded.";
                }
                if (!string.IsNullOrWhiteSpace(resetFailure))
                {
                    OperationMessage += $"{Environment.NewLine}{section.Section.DisplayName}: {resetFailure}";
                }
            }
            if (!_sectionsRequiringRefresh.Contains(section))
            {
                SynchronizeSectionProperties(section);
            }
            _isApplying = false;
            RefreshState();
        }
    }

    /// <summary>Persists every changed section and reports partial failures by section.</summary>
    public async Task<bool> ApplyChangesAsync()
    {
        if (IsBusy)
        {
            return false;
        }
        if (_sectionsWithUnsavedChanges.Any(DraftNeedsRecovery))
        {
            OperationMessage = "A shared draft may have been discarded, but some sections could not be refreshed. Retry discard/reload or Cancel before applying.";
            RefreshState();
            return false;
        }
        if (_sectionsWithUnsavedChanges.Count == 0)
        {
            return true;
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
                finally
                {
                    SynchronizeSectionProperties(section);
                }

                if (section.State != ConfigUiSectionState.Ready)
                {
                    string activation = SectionAppliedAsync is null ? string.Empty : " Runtime activation was not attempted.";
                    failures.Add($"{section.Section.DisplayName}: {section.ErrorMessage ?? "The section could not be saved."}{activation}");
                    continue;
                }

                if (ReferenceEquals(section, SelectedSection))
                {
                    DisplaySection(section);
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
                : $"Some settings were not fully applied. Sections confirmed saved and refreshed: {(savedSections.Count == 0 ? "none" : string.Join(", ", savedSections))}.{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
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
        if (IsBusy)
        {
            return false;
        }

        _isApplying = true;
        OperationMessage = string.Empty;
        RefreshState();
        try
        {
            List<string> failures = [];
            Dictionary<object, bool> discardedDrafts = new(ReferenceEqualityComparer.Instance);
            foreach (IConfigUiSection section in Sections.Where(_sectionsWithUnsavedChanges.Contains).ToArray())
            {
                try
                {
                    if (section is IConfigUiDraftDiscard sharedDraft)
                    {
                        if (!discardedDrafts.TryGetValue(sharedDraft.DraftIdentity, out bool discarded))
                        {
                            discardedDrafts.Add(sharedDraft.DraftIdentity, false);
                            await sharedDraft.DiscardDraftAsync();
                            discarded = section.State == ConfigUiSectionState.Ready;
                            discardedDrafts[sharedDraft.DraftIdentity] = discarded;
                            if (discarded)
                            {
                                _draftsWithUncertainDiscard.Remove(sharedDraft.DraftIdentity);
                                _loadedSections.Add(section);
                                _sectionsWithUnsavedChanges.Remove(section);
                                _sectionsRequiringRefresh.Remove(section);
                                SynchronizeSectionProperties(section);
                                IConfigUiSection[] relatedSections = Sections.Where(item =>
                                    !ReferenceEquals(item, section)
                                    && (_loadedSections.Contains(item) || _sectionsRequiringRefresh.Contains(item))
                                    && item is IConfigUiDraftDiscard other
                                    && ReferenceEquals(other.DraftIdentity, sharedDraft.DraftIdentity)).ToArray();
                                foreach (IConfigUiSection related in relatedSections)
                                {
                                    MarkSectionForRefresh(related);
                                }
                                foreach (IConfigUiSection related in relatedSections.Where(item => !_sectionsWithUnsavedChanges.Contains(item)))
                                {
                                    try
                                    {
                                        await related.LoadAsync();
                                        if (related.State == ConfigUiSectionState.Ready)
                                        {
                                            _loadedSections.Add(related);
                                            _sectionsRequiringRefresh.Remove(related);
                                        }
                                        else
                                        {
                                            failures.Add($"{related.Section.DisplayName}: {related.ErrorMessage ?? "The section could not be reloaded."}");
                                            MarkSectionForRefresh(related);
                                        }
                                    }
                                    catch (Exception exception) when (exception is not OperationCanceledException)
                                    {
                                        failures.Add($"{related.Section.DisplayName}: {exception.Message}");
                                        MarkSectionForRefresh(related);
                                    }
                                    finally
                                    {
                                        if (!_sectionsRequiringRefresh.Contains(related))
                                        {
                                            SynchronizeSectionProperties(related);
                                        }
                                    }
                                }
                            }
                        }
                        else if (!discarded)
                        {
                            failures.Add($"{section.Section.DisplayName}: The shared draft could not be discarded.");
                            continue;
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
                        _sectionsRequiringRefresh.Remove(section);
                    }
                    else
                    {
                        failures.Add($"{section.Section.DisplayName}: {section.ErrorMessage ?? "The section could not be reloaded."}");
                        if (section is IConfigUiDraftDiscard failedDraft
                            && discardedDrafts.TryGetValue(failedDraft.DraftIdentity, out bool discarded) && discarded)
                        {
                            MarkSectionForRefresh(section);
                        }
                        else if (section is IConfigUiDraftDiscard uncertainDraft)
                        {
                            MarkDraftUncertain(uncertainDraft.DraftIdentity);
                        }
                    }
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    failures.Add($"{section.Section.DisplayName}: {exception.Message}");
                    if (section is IConfigUiDraftDiscard failedDraft
                        && discardedDrafts.TryGetValue(failedDraft.DraftIdentity, out bool discarded) && discarded)
                    {
                        MarkSectionForRefresh(section);
                    }
                    else if (section is IConfigUiDraftDiscard uncertainDraft)
                    {
                        MarkDraftUncertain(uncertainDraft.DraftIdentity);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (section is IConfigUiDraftDiscard sharedDraft
                        && discardedDrafts.TryGetValue(sharedDraft.DraftIdentity, out bool discarded) && !discarded)
                    {
                        MarkDraftUncertain(sharedDraft.DraftIdentity);
                    }
                    throw;
                }
                finally
                {
                    if (!_sectionsRequiringRefresh.Contains(section))
                    {
                        SynchronizeSectionProperties(section);
                    }
                }
            }

            OperationMessage = failures.Count == 0
                ? "Unapplied changes were discarded."
                : $"Some settings could not be reloaded. A shared draft may already have been discarded:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}";
            if (SelectedSection is not null && _loadedSections.Contains(SelectedSection))
            {
                DisplaySection(SelectedSection);
            }
            else if (SelectedSection is not null)
            {
                _propertyEditor.SetItems([]);
            }

            return failures.Count == 0;
        }
        finally
        {
            _isApplying = false;
            RefreshState();
        }
    }

    private async Task LoadSelectedSectionAsync(IConfigUiSection section, bool discardSharedDraft = false)
    {
        _loadingCount++;
        OperationMessage = string.Empty;
        RefreshState();
        bool discardAttempted = false;
        bool discardConfirmed = false;
        try
        {
            if (discardSharedDraft && section is IConfigUiDraftDiscard sharedDraft)
            {
                discardAttempted = true;
                await sharedDraft.DiscardDraftAsync();
                if (section.State == ConfigUiSectionState.Ready)
                {
                    discardConfirmed = true;
                    _draftsWithUncertainDiscard.Remove(sharedDraft.DraftIdentity);
                    _loadedSections.Add(section);
                    _sectionsWithUnsavedChanges.Remove(section);
                    _sectionsRequiringRefresh.Remove(section);
                    SynchronizeSectionProperties(section);
                    IConfigUiSection[] relatedSections = Sections.Where(item =>
                        !ReferenceEquals(item, section)
                        && item is IConfigUiDraftDiscard relatedDraft
                        && ReferenceEquals(relatedDraft.DraftIdentity, sharedDraft.DraftIdentity)
                        && (_loadedSections.Contains(item) || _sectionsRequiringRefresh.Contains(item))).ToArray();
                    foreach (IConfigUiSection relatedSection in relatedSections)
                    {
                        MarkSectionForRefresh(relatedSection);
                    }
                    foreach (IConfigUiSection relatedSection in relatedSections)
                    {
                        try
                        {
                            await relatedSection.LoadAsync();
                            if (relatedSection.State == ConfigUiSectionState.Ready)
                            {
                                _loadedSections.Add(relatedSection);
                                _sectionsWithUnsavedChanges.Remove(relatedSection);
                                _sectionsRequiringRefresh.Remove(relatedSection);
                            }
                            else
                            {
                                OperationMessage += $"{relatedSection.Section.DisplayName}: {relatedSection.ErrorMessage ?? "The section could not be reloaded."}{Environment.NewLine}";
                                MarkSectionForRefresh(relatedSection);
                            }
                        }
                        catch (Exception exception) when (exception is not OperationCanceledException)
                        {
                            OperationMessage += $"{relatedSection.Section.DisplayName}: {exception.Message}{Environment.NewLine}";
                            MarkSectionForRefresh(relatedSection);
                        }
                        finally
                        {
                            if (!_sectionsRequiringRefresh.Contains(relatedSection))
                            {
                                SynchronizeSectionProperties(relatedSection);
                            }
                        }
                    }
                }
                else
                {
                    MarkDraftUncertain(sharedDraft.DraftIdentity);
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
                _sectionsRequiringRefresh.Remove(section);
            }
            else
            {
                string hint = discardAttempted
                    ? " The shared draft may already have been discarded; retry discard/reload or Cancel."
                    : string.Empty;
                OperationMessage = $"{section.Section.DisplayName}: {section.ErrorMessage ?? "The section could not be reloaded."}{hint}";
            }
            if (discardConfirmed && !string.IsNullOrEmpty(OperationMessage))
            {
                OperationMessage = $"The shared draft was discarded, but some sections could not be refreshed. Retry discard/reload.{Environment.NewLine}{OperationMessage}";
            }
            if (ReferenceEquals(section, SelectedSection) && !_sectionsRequiringRefresh.Contains(section))
            {
                DisplaySection(section);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            if (discardAttempted && !discardConfirmed && section is IConfigUiDraftDiscard sharedDraft)
            {
                MarkDraftUncertain(sharedDraft.DraftIdentity);
            }
            string hint = discardAttempted && !discardConfirmed
                ? " The shared draft may already have been discarded; retry discard/reload or Cancel."
                : _sectionsRequiringRefresh.Contains(section)
                    ? " Retry loading this section."
                    : string.Empty;
            OperationMessage = $"{section.Section.DisplayName}: {exception.Message}{hint}";
        }
        catch (OperationCanceledException)
        {
            if (discardAttempted && !discardConfirmed && section is IConfigUiDraftDiscard sharedDraft)
            {
                MarkDraftUncertain(sharedDraft.DraftIdentity);
            }
            throw;
        }
        finally
        {
            if (!_sectionsRequiringRefresh.Contains(section))
            {
                SynchronizeSectionProperties(section);
            }
            _loadingCount--;
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
        OnPropertyChanged(nameof(IsSelectedSectionReady));
        OnPropertyChanged(nameof(NeedsReloadRecovery));
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
        SynchronizeSectionProperties(section);
    }

    private void MarkSectionForRefresh(IConfigUiSection section)
    {
        _loadedSections.Remove(section);
        _sectionsRequiringRefresh.Add(section);
        if (_subscribedProperties.Remove(section, out var subscriptions))
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Key.ValueChanged -= subscription.Value;
            }

        }
        if (ReferenceEquals(section, SelectedSection))
        {
            _propertyEditor.SetItems([]);
        }
    }

    private void MarkDraftUncertain(object identity)
    {
        _draftsWithUncertainDiscard.Add(identity);
        foreach (IConfigUiSection section in Sections.Where(item =>
            item is IConfigUiDraftDiscard shared && ReferenceEquals(shared.DraftIdentity, identity)
            && (_loadedSections.Contains(item) || _sectionsWithUnsavedChanges.Contains(item))).ToArray())
        {
            MarkSectionForRefresh(section);
        }
    }

    private bool DraftNeedsRecovery(IConfigUiSection section)
    {
        if (_sectionsRequiringRefresh.Contains(section))
        {
            return true;
        }
        return section is IConfigUiDraftDiscard draft
            && _sectionsRequiringRefresh.Any(item => item is IConfigUiDraftDiscard other
                && ReferenceEquals(other.DraftIdentity, draft.DraftIdentity));
    }

    private void SynchronizeSectionProperties(IConfigUiSection section)
    {
        if (!_subscribedProperties.TryGetValue(section, out var subscriptions))
        {
            subscriptions = new(ReferenceEqualityComparer.Instance);
            _subscribedProperties.Add(section, subscriptions);
        }

        HashSet<IPropertyItem> current = new(section.Properties, ReferenceEqualityComparer.Instance);
        foreach (IPropertyItem property in subscriptions.Keys.Where(item => !current.Contains(item)).ToArray())
        {
            property.ValueChanged -= subscriptions[property];
            subscriptions.Remove(property);
        }

        foreach (IPropertyItem property in current)
        {
            if (!subscriptions.ContainsKey(property))
            {
                EventHandler<PropertyValueChangedEventArgs> handler = (_, _) =>
                {
                    _sectionsWithUnsavedChanges.Add(section);
                    if (ReferenceEquals(section, SelectedSection))
                    {
                        OnPropertyChanged(nameof(HasUnsavedChanges));
                    }

                    OnPropertyChanged(nameof(CanApplyChanges));
                    ApplyCommand.NotifyCanExecuteChanged();
                };
                subscriptions.Add(property, handler);
                property.ValueChanged += handler;
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
