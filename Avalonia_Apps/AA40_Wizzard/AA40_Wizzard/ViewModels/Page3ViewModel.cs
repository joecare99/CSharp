using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using AA40_Wizzard.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.ViewModels;

/// <summary>
/// Provides the third wizard page.
/// </summary>
public sealed partial class Page3ViewModel : ObservableObject, IRecipient<ValueChangedMessage<CultureInfo>>
{
    private readonly IWizzardModel _model;
    private readonly IWizardContentService _contentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Page3ViewModel"/> class.
    /// </summary>
    public Page3ViewModel(IWizzardModel model, IWizardContentService contentService, IMessenger messenger)
    {
        _model = model;
        _contentService = contentService;

        _model.PropertyChanged += OnModelPropertyChanged;
        messenger.Register(this);
    }

    /// <summary>
    /// Gets the first additional label.
    /// </summary>
    public string Additional1Label => "Additional 1:";

    /// <summary>
    /// Gets the second additional label.
    /// </summary>
    public string Additional2Label => "Additional 2:";

    /// <summary>
    /// Gets the third additional label.
    /// </summary>
    public string Additional3Label => "Additional 3:";

    /// <summary>
    /// Gets the clear button text.
    /// </summary>
    public string ClearText => "Clear";

    /// <summary>
    /// Gets the localized additional options.
    /// </summary>
    public IReadOnlyList<ListEntry> AdditOptions => _contentService.GetOptions(_model.AdditOptions, "AdditSelection");

    /// <summary>
    /// Gets or sets the first additional option.
    /// </summary>
    public ListEntry? Additional1
    {
        get => TryFindEntry(_model.Additional1, AdditOptions);
        set => _model.Additional1 = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets or sets the second additional option.
    /// </summary>
    public ListEntry? Additional2
    {
        get => TryFindEntry(_model.Additional2, AdditOptions);
        set => _model.Additional2 = value?.ID ?? -1;
    }

    /// <summary>
    /// Gets or sets the third additional option.
    /// </summary>
    public ListEntry? Additional3
    {
        get => TryFindEntry(_model.Additional3, AdditOptions);
        set => _model.Additional3 = value?.ID ?? -1;
    }

    [RelayCommand]
    private void Clear()
    {
        Additional1 = null;
        Additional2 = null;
        Additional3 = null;
    }

    /// <inheritdoc />
    public void Receive(ValueChangedMessage<CultureInfo> message)
    {
        OnPropertyChanged(nameof(AdditOptions));
        OnPropertyChanged(nameof(Additional1));
        OnPropertyChanged(nameof(Additional2));
        OnPropertyChanged(nameof(Additional3));
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(IWizzardModel.Additional1), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Additional1));
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.Additional2), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Additional2));
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.Additional3), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Additional3));
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.AdditOptions), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(AdditOptions));
            OnPropertyChanged(nameof(Additional1));
            OnPropertyChanged(nameof(Additional2));
            OnPropertyChanged(nameof(Additional3));
        }
    }

    private static ListEntry? TryFindEntry(int id, IReadOnlyList<ListEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.ID == id)
            {
                return entry;
            }
        }

        return null;
    }
}
