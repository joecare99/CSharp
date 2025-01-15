// ***********************************************************************
// Assembly         : AA05_CommandParCalc
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-13-2025
// ***********************************************************************
// <copyright file="CommandParCalcViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Reflection;
using AA05_CommandParCalc.Models.Interfaces;
using AA05_CommandParCalc.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace AA05_CommandParCalc.ViewModels;

/// <summary>
/// Class CommandParCalcViewModel.
/// Implements the <see cref="ICommandParCalcViewModel" />
/// and the <see cref="ViewModelBase" />
/// </summary>
/// <seealso cref="ViewModelBase" />
/// <seealso cref="ICommandParCalcViewModel" />
public partial class CommandParCalcViewModel : ViewModelBase, ICommandParCalcViewModel
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
    private ICommandParCalcModel _model;

    public CommandParCalcViewModel()
    {
        Greeting = $"Welcome to Avalonia! The current time is {DateTime.Now}";
        _model = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandParCalcViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public CommandParCalcViewModel(ICommandParCalcModel model)
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
