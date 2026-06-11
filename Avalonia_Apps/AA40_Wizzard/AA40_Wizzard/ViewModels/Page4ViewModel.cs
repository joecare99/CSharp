using System;
using System.ComponentModel;
using System.Globalization;
using AA40_Wizzard.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.ViewModels;

/// <summary>
/// Provides the summary page of the wizard.
/// </summary>
public sealed class Page4ViewModel : ObservableObject, IRecipient<ValueChangedMessage<CultureInfo>>
{
    private readonly IWizzardModel _model;
    private readonly IWizardContentService _contentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Page4ViewModel"/> class.
    /// </summary>
    public Page4ViewModel(IWizzardModel model, IWizardContentService contentService, IMessenger messenger)
    {
        _model = model;
        _contentService = contentService;

        _model.PropertyChanged += OnModelPropertyChanged;
        messenger.Register(this);
    }

    /// <summary>
    /// Gets the summary label.
    /// </summary>
    public string SummaryLabel => "Your Selection:";

    /// <summary>
    /// Gets the connector label.
    /// </summary>
    public string AndLabel => "and";

    /// <summary>
    /// Gets the additional label.
    /// </summary>
    public string AdditionalLabel => "Additional:";

    /// <summary>
    /// Gets the localized main selection text.
    /// </summary>
    public string MainSelection => _model.MainSelection >= 0 ? _contentService.GetText($"MainSelection{_model.MainSelection}") : string.Empty;

    /// <summary>
    /// Gets the localized sub selection text.
    /// </summary>
    public string SubSelection => _model.SubSelection >= 0 ? _contentService.GetText($"SubSelection{_model.SubSelection}") : string.Empty;

    /// <summary>
    /// Gets the first localized additional selection text.
    /// </summary>
    public string Additional1 => _model.Additional1 >= 0 ? _contentService.GetText($"AdditSelection{_model.Additional1}") : string.Empty;

    /// <summary>
    /// Gets the second localized additional selection text.
    /// </summary>
    public string Additional2 => _model.Additional2 >= 0 ? _contentService.GetText($"AdditSelection{_model.Additional2}") : string.Empty;

    /// <summary>
    /// Gets the third localized additional selection text.
    /// </summary>
    public string Additional3 => _model.Additional3 >= 0 ? _contentService.GetText($"AdditSelection{_model.Additional3}") : string.Empty;

    /// <inheritdoc />
    public void Receive(ValueChangedMessage<CultureInfo> message)
        => RaiseAllSelectionProperties();

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(IWizzardModel.MainSelection), StringComparison.Ordinal)
            || string.Equals(e.PropertyName, nameof(IWizzardModel.SubSelection), StringComparison.Ordinal)
            || string.Equals(e.PropertyName, nameof(IWizzardModel.Additional1), StringComparison.Ordinal)
            || string.Equals(e.PropertyName, nameof(IWizzardModel.Additional2), StringComparison.Ordinal)
            || string.Equals(e.PropertyName, nameof(IWizzardModel.Additional3), StringComparison.Ordinal))
        {
            RaiseAllSelectionProperties();
        }
    }

    private void RaiseAllSelectionProperties()
    {
        OnPropertyChanged(nameof(MainSelection));
        OnPropertyChanged(nameof(SubSelection));
        OnPropertyChanged(nameof(Additional1));
        OnPropertyChanged(nameof(Additional2));
        OnPropertyChanged(nameof(Additional3));
    }
}
