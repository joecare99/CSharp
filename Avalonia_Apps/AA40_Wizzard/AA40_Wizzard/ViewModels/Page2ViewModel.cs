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
/// Provides the second wizard page.
/// </summary>
public sealed partial class Page2ViewModel : ObservableObject, IRecipient<ValueChangedMessage<CultureInfo>>
{
    private readonly IWizzardModel _model;
    private readonly IWizardContentService _contentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Page2ViewModel"/> class.
    /// </summary>
    public Page2ViewModel(IWizzardModel model, IWizardContentService contentService, IMessenger messenger)
    {
        _model = model;
        _contentService = contentService;

        _model.PropertyChanged += OnModelPropertyChanged;
        messenger.Register(this);
    }

    /// <summary>
    /// Gets the label for the option list.
    /// </summary>
    public string NameLabel => "Name:";

    /// <summary>
    /// Gets the clear button text.
    /// </summary>
    public string ClearText => "Clear";

    /// <summary>
    /// Gets the localized sub options.
    /// </summary>
    public IReadOnlyList<ListEntry> SubOptions => _contentService.GetOptions(_model.SubOptions, "SubSelection");

    /// <summary>
    /// Gets or sets the selected sub option.
    /// </summary>
    public ListEntry? SubSelection
    {
        get => TryFindEntry(_model.SubSelection, SubOptions);
        set { if (value != null) _model.SubSelection = value.ID; }
    }

    [RelayCommand]
    private void Clear()
        => _model.SubSelection = -1;

    /// <inheritdoc />
    public void Receive(ValueChangedMessage<CultureInfo> message)
    {
        OnPropertyChanged(nameof(SubOptions));
        OnPropertyChanged(nameof(SubSelection));
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(IWizzardModel.SubSelection), StringComparison.Ordinal))
        {
            if (SubSelection?.ID != _model.SubSelection)
            OnPropertyChanged(nameof(SubSelection));
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.SubOptions), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(SubOptions));
            OnPropertyChanged(nameof(SubSelection));
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
