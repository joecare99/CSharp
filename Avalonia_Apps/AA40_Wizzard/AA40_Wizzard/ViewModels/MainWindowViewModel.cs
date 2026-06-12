using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.ViewModels;

/// <summary>
/// Provides the main shell content for the wizard sample.
/// </summary>
public sealed class MainWindowViewModel : ObservableObject, IRecipient<ValueChangedMessage<System.Globalization.CultureInfo>>
{
    private readonly IWizardContentService _contentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="contentService">The content service.</param>
    /// <param name="messenger">The message bus.</param>
    /// <param name="wizzard">The embedded wizard view model.</param>
    public MainWindowViewModel(IWizardContentService contentService, IMessenger messenger, WizzardViewModel wizzard)
    {
        _contentService = contentService;
        Wizzard = wizzard;
        messenger.Register(this);
    }

    /// <summary>
    /// Gets the embedded wizard view model.
    /// </summary>
    public WizzardViewModel Wizzard { get; }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title => _contentService.GetText(nameof(Title));

    /// <summary>
    /// Gets the window description.
    /// </summary>
    public string Description => _contentService.GetText(nameof(Description));

    /// <summary>
    /// Gets the header for the XAML tab.
    /// </summary>
    public string XamlHeader => "Xaml";

    /// <summary>
    /// Gets the header for the view-model tab.
    /// </summary>
    public string ViewModelHeader => "ViewModel";

    /// <summary>
    /// Gets the localized XAML sample text.
    /// </summary>
    public string WizzardViewSource => _contentService.GetText("WizzardView");

    /// <summary>
    /// Gets the localized view-model sample text.
    /// </summary>
    public string WizzardViewModelSource => _contentService.GetText("WizzardViewModel");

    /// <inheritdoc />
    public void Receive(ValueChangedMessage<System.Globalization.CultureInfo> message)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Title)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Description)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(WizzardViewSource)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(WizzardViewModelSource)));
    }
}
