using System;
using System.ComponentModel;
using System.Globalization;
using AA40_Wizzard.Model;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.ViewModels;

/// <summary>
/// Provides the interactive multi-step wizard logic.
/// </summary>
public partial class WizzardViewModel : ObservableObject, IRecipient<ValueChangedMessage<CultureInfo>>
{
    private readonly IWizzardModel _model;
    private readonly IMessenger _messenger;
    private readonly IWizardContentService _contentService;
    private readonly DispatcherTimer _clockTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WizzardViewModel"/> class.
    /// </summary>
    public WizzardViewModel(
        IWizzardModel model,
        IMessenger messenger,
        IWizardContentService contentService,
        Page1ViewModel page1,
        Page2ViewModel page2,
        Page3ViewModel page3,
        Page4ViewModel page4)
    {
        _model = model;
        _messenger = messenger;
        _contentService = contentService;
        Page1 = page1;
        Page2 = page2;
        Page3 = page3;
        Page4 = page4;

        _model.PropertyChanged += OnModelPropertyChanged;
        _messenger.Register(this);

        _clockTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(250), DispatcherPriority.Background, (_, _) =>
        {
            OnPropertyChanged(nameof(Now));
        });
        _clockTimer.Start();
    }

    /// <summary>
    /// Gets the current localized title.
    /// </summary>
    public string Title => _contentService.GetText(nameof(Title));

    /// <summary>
    /// Gets the current time.
    /// </summary>
    public DateTime Now => _model.Now;

    /// <summary>
    /// Gets the previous-tab button text.
    /// </summary>
    public string PrevTabText => _contentService.GetText("PrevTab");

    /// <summary>
    /// Gets the next-tab button text.
    /// </summary>
    public string NextTabText => _contentService.GetText("NextTab");

    /// <summary>
    /// Gets the first page header.
    /// </summary>
    public string Page1Header => _contentService.GetText("Page1");

    /// <summary>
    /// Gets the second page header.
    /// </summary>
    public string Page2Header => _contentService.GetText("Page2");

    /// <summary>
    /// Gets the third page header.
    /// </summary>
    public string Page3Header => _contentService.GetText("Page3");

    /// <summary>
    /// Gets the fourth page header.
    /// </summary>
    public string Page4Header => _contentService.GetText("Page4");

    /// <summary>
    /// Gets the first wizard page view model.
    /// </summary>
    public Page1ViewModel Page1 { get; }

    /// <summary>
    /// Gets the second wizard page view model.
    /// </summary>
    public Page2ViewModel Page2 { get; }

    /// <summary>
    /// Gets the third wizard page view model.
    /// </summary>
    public Page3ViewModel Page3 { get; }

    /// <summary>
    /// Gets the fourth wizard page view model.
    /// </summary>
    public Page4ViewModel Page4 { get; }

    /// <summary>
    /// Gets a value indicating whether the second tab is enabled.
    /// </summary>
    public bool Tab2Enabled => _model.MainSelection >= 0;

    /// <summary>
    /// Gets a value indicating whether the third tab is enabled.
    /// </summary>
    public bool Tab3Enabled => Tab2Enabled && _model.SubSelection >= 0;

    /// <summary>
    /// Gets a value indicating whether the fourth tab is enabled.
    /// </summary>
    public bool Tab4Enabled => Tab3Enabled && _model.Additional1 >= 0 && _model.Additional2 >= 0 && _model.Additional3 >= 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrevTabCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextTabCommand))]
    private int _selectedTab;

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(IWizzardModel.MainSelection), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Tab2Enabled));
            OnPropertyChanged(nameof(Tab3Enabled));
            OnPropertyChanged(nameof(Tab4Enabled));
            NextTabCommand.NotifyCanExecuteChanged();
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.SubSelection), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Tab3Enabled));
            OnPropertyChanged(nameof(Tab4Enabled));
            NextTabCommand.NotifyCanExecuteChanged();
        }
        else if (string.Equals(e.PropertyName, nameof(IWizzardModel.Additional1), StringComparison.Ordinal)
            || string.Equals(e.PropertyName, nameof(IWizzardModel.Additional2), StringComparison.Ordinal)
            || string.Equals(e.PropertyName, nameof(IWizzardModel.Additional3), StringComparison.Ordinal))
        {
            OnPropertyChanged(nameof(Tab4Enabled));
            NextTabCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanPrevTab() => SelectedTab > 0;

    private bool CanNextTab() => SelectedTab switch
    {
        0 => Tab2Enabled,
        1 => Tab3Enabled,
        2 => Tab4Enabled,
        _ => false,
    };

    [RelayCommand(CanExecute = nameof(CanPrevTab))]
    private void PrevTab()
        => SelectedTab--;

    [RelayCommand(CanExecute = nameof(CanNextTab))]
    private void NextTab()
        => SelectedTab++;

    [RelayCommand]
    private void LangDe()
        => ChangeCulture("de-DE");

    [RelayCommand]
    private void LangEn()
        => ChangeCulture("en-US");

    [RelayCommand]
    private void LangFr()
        => ChangeCulture("fr-FR");

    /// <inheritdoc />
    public void Receive(ValueChangedMessage<CultureInfo> message)
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(PrevTabText));
        OnPropertyChanged(nameof(NextTabText));
        OnPropertyChanged(nameof(Page1Header));
        OnPropertyChanged(nameof(Page2Header));
        OnPropertyChanged(nameof(Page3Header));
        OnPropertyChanged(nameof(Page4Header));
        OnPropertyChanged(nameof(Now));
    }

    private void ChangeCulture(string cultureName)
    {
        var culture = CultureInfo.CreateSpecificCulture(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        _messenger.Send(new ValueChangedMessage<CultureInfo>(culture));
    }
}
