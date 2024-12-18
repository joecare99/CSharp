// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page1ViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.ComponentModel;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_40_Wizzard.Models;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Documents;
using System.Linq;
using System.IO;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Media.Imaging;
using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Threading.Tasks;


/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace MVVM_40_Wizzard.ViewModels;

/// <summary>
/// Class Page1ViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class Page1ViewModel : BaseViewModelCT 
{
    /// <summary>
    /// The model
    /// </summary>
    private IWizzardModel _model;

    /// <summary>
    /// Gets or sets the main selection.
    /// </summary>
    /// <value>The main selection.</value>
    public ListEntry? MainSelection
    {
        get => MainOptions.FirstOrDefault((e)=>e.ID==_model.MainSelection);
        set => _model.MainSelection = value?.ID ?? -1;
    }

    public int Selection => _model.MainOptions.IndexOf(_model.MainSelection);

    /// <summary>
    /// Gets the main options.
    /// </summary>
    /// <value>The main options.</value>
    public IList<ListEntry> MainOptions 
        => _model.MainOptions.Select((i)=>new ListEntry(i, Properties.Resources.ResourceManager.GetString($"MainSelection{i}"))).ToList();

    public ImageSource? ImageSource
    {
        get
        {
            if (File.Exists($"Resources\\{CultureInfo.CurrentUICulture.Name}\\MainSelection{MainSelection?.ID}.png"))
            {
                return new BitmapImage(new System.Uri( $".\\Resources\\{CultureInfo.CurrentUICulture.Name}\\MainSelection{MainSelection?.ID}.png"));
            }
            else if (File.Exists($"Resources\\{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}\\MainSelection{MainSelection?.ID}.png"))
            {
                return new BitmapImage(new System.Uri($".\\Resources\\{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}\\MainSelection{MainSelection?.ID}.png"));
            }
            else if (File.Exists($"Resources\\MainSelection{MainSelection?.ID}.png"))
            {
                return new BitmapImage(new Uri(
                    Path.Combine(Environment.CurrentDirectory, "Resources", $"MainSelection{MainSelection?.ID}.png")
                    ));
            }
            else
            {
                return null;
            }
        }
    }

    public string? Document
    {
        get
        {
            if (File.Exists($"Resources\\{CultureInfo.CurrentUICulture.Name}\\MainSelection{MainSelection?.ID}.xaml"))
            {
                return File.ReadAllText($"Resources\\{CultureInfo.CurrentUICulture.Name}\\MainSelection{MainSelection?.ID}.xaml");
            }
            else if (File.Exists($"Resources\\{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}\\MainSelection{MainSelection?.ID}.xaml"))
            {
                return File.ReadAllText($"Resources\\{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}\\MainSelection{MainSelection?.ID}.xaml");
            }
            else if (File.Exists($"Resources\\MainSelection{MainSelection?.ID}.xaml"))
            {
                return File.ReadAllText($"Resources\\MainSelection{MainSelection?.ID}.xaml");
            }
            else
            {
                return "";
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Page1ViewModel"/> class.
    /// </summary>
    public Page1ViewModel():this(IoC.GetRequiredService<IWizzardModel>(), IoC.GetRequiredService<IMessenger>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Page1ViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public Page1ViewModel(IWizzardModel model,IMessenger messenger)
    {
        _model = model;
        _model.PropertyChanged += OnMPropertyChanged;
        AsyncMainSelection();
    }

    async private void AsyncMainSelection()
    {
        await Task.Delay(100);
        OnPropertyChanged(nameof(Selection));
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        MainSelection = null;
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
        if (e.PropertyName == nameof(MainSelection))
        {
             OnPropertyChanged(nameof(ImageSource));
             OnPropertyChanged(nameof(Document));
        }
    }

}
