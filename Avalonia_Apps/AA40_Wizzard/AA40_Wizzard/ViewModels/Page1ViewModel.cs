using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using AA40_Wizzard.Model;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.ViewModels;

/// <summary>
/// Provides the first wizard page.
/// </summary>
public sealed partial class Page1ViewModel : ObservableObject, IRecipient<ValueChangedMessage<CultureInfo>>
{
    private readonly IWizzardModel _model;
    private readonly IWizardContentService _contentService;
    private IReadOnlyList<ListEntry> _mainOptions = Array.Empty<ListEntry>();
    private ListEntry? _mainSelection;
    private Bitmap? _image;
    private Control? _documentPreview;
    private bool _isSynchronizingSelection;

    /// <summary>
    /// Initializes a new instance of the <see cref="Page1ViewModel"/> class.
    /// </summary>
    public Page1ViewModel(IWizzardModel model, IWizardContentService contentService, IMessenger messenger)
    {
        _model = model;
        _contentService = contentService;

        _model.PropertyChanged += OnModelPropertyChanged;
        messenger.Register(this);

        RefreshOptionsAndSelection();
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
    /// Gets the localized main options.
    /// </summary>
    public IReadOnlyList<ListEntry> MainOptions
    {
        get => _mainOptions;
        private set => SetProperty(ref _mainOptions, value);
    }

    /// <summary>
    /// Gets or sets the selected main option.
    /// </summary>
    public ListEntry? MainSelection
    {
        get => _mainSelection;
        set
        {
            if (!SetProperty(ref _mainSelection, value))
            {
                return;
            }

            if (_isSynchronizingSelection)
            {
                return;
            }

            _model.MainSelection = value?.ID ?? -1;
        }
    }

    /// <summary>
    /// Gets the localized image.
    /// </summary>
    public Bitmap? Image
    {
        get => _image;
        private set => SetProperty(ref _image, value);
    }

    /// <summary>
    /// Gets the localized document preview control.
    /// </summary>
    public Control? DocumentPreview
    {
        get => _documentPreview;
        private set => SetProperty(ref _documentPreview, value);
    }

    [RelayCommand]
    private void Clear()
        => MainSelection = null;

    /// <inheritdoc />
    public void Receive(ValueChangedMessage<CultureInfo> message)
        => RefreshOptionsAndSelection();

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(IWizzardModel.MainSelection), StringComparison.Ordinal))
        {
            RefreshSelection();
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.MainOptions), StringComparison.Ordinal))
        {
            RefreshOptionsAndSelection();
        }
    }

    private void RefreshOptionsAndSelection()
    {
        MainOptions = _contentService.GetOptions(_model.MainOptions, "MainSelection");
        RefreshSelection();
    }

    private void RefreshSelection()
    {
        var selectedEntry = TryFindEntry(_model.MainSelection, MainOptions);
        _isSynchronizingSelection = true;
        try
        {
            MainSelection = selectedEntry;
        }
        finally
        {
            _isSynchronizingSelection = false;
        }

        Image = _contentService.GetImage(_model.MainSelection);
        DocumentPreview = _contentService.GetDocumentPreview(_model.MainSelection);
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
