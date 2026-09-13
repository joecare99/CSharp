using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Property.Editor;
using Property.Editor.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    /// <summary>
    /// Initializes the configuration UI state from a host registry.
    /// </summary>
    public ConfigUiViewModel(IConfigUiSectionRegistry registry, PropertyEditorViewModel propertyEditor)
    {
        ArgumentNullException.ThrowIfNull(registry);
        _propertyEditor = propertyEditor ?? throw new ArgumentNullException(nameof(propertyEditor));

        foreach (IConfigUiSection section in registry.Sections)
        {
            Sections.Add(section);
        }

        SelectedSection = Sections.Count > 0 ? Sections[0] : null;
    }

    /// <summary>Gets registered sections in host-provided order.</summary>
    public ObservableCollection<IConfigUiSection> Sections { get; } = [];

    /// <summary>Gets the shared property editor state for the selected section.</summary>
    public PropertyEditorViewModel PropertyEditor => _propertyEditor;

    /// <summary>Gets whether an operation is currently in progress.</summary>
    public bool IsBusy => SelectedSection?.State is ConfigUiSectionState.Loading
        or ConfigUiSectionState.Saving
        or ConfigUiSectionState.Resetting;

    /// <summary>Gets the selected section's localized failure message.</summary>
    public string ErrorMessage => SelectedSection?.ErrorMessage ?? string.Empty;

    /// <summary>Gets whether the selected section can be changed by the host.</summary>
    public bool CanSave => SelectedSection is { IsReadOnly: false, State: ConfigUiSectionState.Ready };

    /// <summary>
    /// Gets whether the selected section has a local, unpersisted draft.
    /// Section changes preserve such drafts; explicit load and reset discard them.
    /// </summary>
    public bool HasUnsavedChanges => SelectedSection is not null && _sectionsWithUnsavedChanges.Contains(SelectedSection);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBusy))]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    [NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
    private IConfigUiSection? _selectedSection;

    partial void OnSelectedSectionChanged(IConfigUiSection? value)
    {
        _propertyEditor.SetItems([]);
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
            await LoadSelectedSectionAsync(SelectedSection).ConfigureAwait(false);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (!CanSave || SelectedSection is null)
        {
            return;
        }

        await SelectedSection.SaveAsync().ConfigureAwait(false);
        if (SelectedSection.State == ConfigUiSectionState.Ready)
        {
            _sectionsWithUnsavedChanges.Remove(SelectedSection);
        }

        RefreshState();
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task ResetAsync()
    {
        if (!CanSave || SelectedSection is null)
        {
            return;
        }

        await SelectedSection.ResetAsync().ConfigureAwait(false);
        _sectionsWithUnsavedChanges.Remove(SelectedSection);
        _loadedSections.Add(SelectedSection);
        DisplaySection(SelectedSection);
        RefreshState();
    }

    private async Task LoadSelectedSectionAsync(IConfigUiSection section)
    {
        await section.LoadAsync().ConfigureAwait(false);
        _loadedSections.Add(section);
        _sectionsWithUnsavedChanges.Remove(section);
        if (ReferenceEquals(section, SelectedSection))
        {
            DisplaySection(section);
            RefreshState();
        }
    }

    private void RefreshState()
    {
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(CanSave));
        OnPropertyChanged(nameof(HasUnsavedChanges));
        SaveCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
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
                };
            }
        }
    }
}
