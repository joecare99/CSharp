// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.__Internals;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_40_Wizzard.Models;
using MVVM_40_Wizzard.Models.Interfaces;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace MVVM_40_Wizzard.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class WizzardViewModel : BaseViewModelCT
{
    #region Properties
    /// <summary>
    /// The model
    /// </summary>
    private readonly IWizzardModel _model;
    private readonly IMessenger _messenger;

    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    public DateTime Now => _model.Now;

    /// <summary>
    /// The selected tab
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrevTabCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextTabCommand))]
    private int _selectedTab = 0;

    [ObservableProperty]
    private string _page1FrameName = new Uri($"/{Assembly.GetExecutingAssembly().GetName().Name};component/views/Page1View.xaml", UriKind.Relative).ToString();
    [ObservableProperty]
    private string _page2FrameName = new Uri($"/{Assembly.GetExecutingAssembly().GetName().Name};component/views/Page2View.xaml", UriKind.Relative).ToString();
    [ObservableProperty]
    private string _page3FrameName = new Uri($"/{Assembly.GetExecutingAssembly().GetName().Name};component/views/Page3View.xaml", UriKind.Relative).ToString();
    [ObservableProperty]
    private string _page4FrameName = new Uri($"/{Assembly.GetExecutingAssembly().GetName().Name};component/views/Page4View.xaml", UriKind.Relative).ToString();

    /// <summary>
    /// Gets a value indicating whether [tab2 enabled].
    /// </summary>
    /// <value><c>true</c> if [tab2 enabled]; otherwise, <c>false</c>.</value>
    public bool Tab2Enabled => _model.MainSelection >= 0;
    /// <summary>
    /// Gets a value indicating whether [tab3 enabled].
    /// </summary>
    /// <value><c>true</c> if [tab3 enabled]; otherwise, <c>false</c>.</value>
    public bool Tab3Enabled => Tab2Enabled && _model.SubSelection >= 0;
    /// <summary>
    /// Gets a value indicating whether [tab4 enabled].
    /// </summary>
    /// <value><c>true</c> if [tab4 enabled]; otherwise, <c>false</c>.</value>
    public bool Tab4Enabled 
        => Tab3Enabled 
        && _model.Additional1 >= 0 
        && _model.Additional2 >= 0 
        && _model.Additional3 >= 0;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    public WizzardViewModel() : this(IoC.GetRequiredService<IWizzardModel>(), IoC.GetRequiredService<IMessenger>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WizzardViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public WizzardViewModel(IWizzardModel model, IMessenger messenger)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
        _messenger = messenger;
    }

    /// <summary>
    /// Handles the <see cref="E:MPropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (this.IsProperty(e.PropertyName!))
            OnPropertyChanged(e.PropertyName);
        if (e.PropertyName == nameof(_model.MainSelection))
        {
            OnPropertyChanged(nameof(Tab2Enabled));
            OnPropertyChanged(nameof(Tab3Enabled));
            NextTabCommand.NotifyCanExecuteChanged();
        }
        if (e.PropertyName == nameof(_model.SubSelection))
        {
            OnPropertyChanged(nameof(Tab3Enabled));
            NextTabCommand.NotifyCanExecuteChanged();
        }
        if (e.PropertyName == nameof(_model.Additional1)
            || e.PropertyName == nameof(_model.Additional2)
            || e.PropertyName == nameof(_model.Additional3))
        {
            OnPropertyChanged(nameof(Tab4Enabled));
            NextTabCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Determines whether this instance [can previous tab].
    /// </summary>
    /// <returns><c>true</c> if this instance [can previous tab]; otherwise, <c>false</c>.</returns>
    private bool CanPrevTab() => SelectedTab > 0;
    /// <summary>
    /// Determines whether this instance [can next tab].
    /// </summary>
    /// <returns><c>true</c> if this instance [can next tab]; otherwise, <c>false</c>.</returns>
    private bool CanNextTab() => SelectedTab switch
    {
        0 => Tab2Enabled,
        1 => Tab3Enabled,
        2 => Tab4Enabled,
        _ => false
    };

    /// <summary>
    /// Previouses the tab.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanPrevTab))]
    private void PrevTab()
    {
        SelectedTab--;
    }

    /// <summary>
    /// Nexts the tab.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanNextTab))]
    private void NextTab()
    {
        SelectedTab++;
    }

    [RelayCommand]
    private void LangDe()
    {
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("de-DE");
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
        _messenger.Send(new ValueChangedMessage<CultureInfo>(CultureInfo.CurrentUICulture));
    }

    [RelayCommand]
    private void LangEn()
    {
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        _messenger.Send(new ValueChangedMessage<CultureInfo>(CultureInfo.CurrentUICulture));
    }

    [RelayCommand]
    private void LangFr()
    {
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
        _messenger.Send(new ValueChangedMessage<CultureInfo>(CultureInfo.CurrentUICulture));
    }
    #endregion
}
