// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-13-2025
// ***********************************************************************
// <copyright file="SomeTemplateViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Reflection;
using Avalonia_App02.Models.Interfaces;
using Avalonia_App02.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Avalonia_App02.ViewModels;

/// <summary>
/// Class SomeTemplateViewModel.
/// Implements the <see cref="ISomeTemplateViewModel" />
/// and the <see cref="ViewModelBase" />
/// </summary>
/// <seealso cref="ViewModelBase" />
/// <seealso cref="ISomeTemplateViewModel" />
public partial class SomeTemplateViewModel : ViewModelBase, ISomeTemplateViewModel
{
    /// <summary>
    /// Gets the greeting.
    /// </summary>
    /// <value>The greeting.</value>
    [ObservableProperty]
    private string _greeting  = "Welcome to Avalonia!";

    [ObservableProperty]
    private string _title  = "Main Menu";

    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    public DateTime Now => _model.Now;

    /// <summary>
    /// The model
    /// </summary>
    private ISomeTemplateModel _model;

    public SomeTemplateViewModel()
    {
        Greeting = $"Welcome to Avalonia! The current time is {DateTime.Now}";
        _model = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SomeTemplateViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public SomeTemplateViewModel(ISomeTemplateModel model)
    {
        Greeting = $"Welcome to Avalonia! The current time is {model.Now}";
        _model = model;
        _model.PropertyChanged += Model_PropertyChanged;
    }

    /// <summary>
    /// Handles the PropertyChanged event of the Model control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.PropertyName) 
            && (GetType().GetProperty(e.PropertyName,BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase) is PropertyInfo pi ))
        {
            OnPropertyChanged(pi.Name);   
        }
    }

    [RelayCommand]
    private void Home()
    {
        Greeting = $"Hello, Avalonia!";
    }
    [RelayCommand]
    private void Actions()
    {
        Greeting = $"Action:";
    }
    [RelayCommand]
    private void Macros()
    {
        Greeting = $"Macros:";
    }
    [RelayCommand]
    private void Process()
    {
        Greeting = $"Process:";
    }
    [RelayCommand]
    private void Reports()
    {
        Greeting = $"Reports:";
    }
    [RelayCommand]
    private void History()
    {
        Greeting = $"History:";
    }
    [RelayCommand]
    private void Config()
    {
        Greeting = $"Config:";
    }


}
