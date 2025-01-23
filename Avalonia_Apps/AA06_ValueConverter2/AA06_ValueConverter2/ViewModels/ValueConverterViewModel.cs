// ***********************************************************************
// Assembly         : AA06_ValueConverter2
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-13-2025
// ***********************************************************************
// <copyright file="ValueConverterViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Reflection;
using AA06_ValueConverter2.Models.Interfaces;
using AA06_ValueConverter2.ViewModels.Interfaces;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace AA06_ValueConverter2.ViewModels;

/// <summary>
/// Class ValueConverterViewModel.
/// Implements the <see cref="IValueConverterViewModel" />
/// and the <see cref="ViewModelBase" />
/// </summary>
/// <seealso cref="ViewModelBase" />
/// <seealso cref="IValueConverterViewModel" />
public partial class ValueConverterViewModel : ViewModelBase, IValueConverterViewModel
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

    [ObservableProperty]
    private double _inputValue; 
    public double ResultValue => _model.ResultValue;

    /// <summary>
    /// The model
    /// </summary>
    private IValueConverterModel _model;

    public ValueConverterViewModel()
    {
        Greeting = $"Welcome to Avalonia! The current time is {DateTime.Now}";
        _model = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueConverterViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    public ValueConverterViewModel(IValueConverterModel model)
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
            if (pi.CanWrite) pi.SetValue(this, _model.GetProp(e.PropertyName));
            OnPropertyChanged(pi.Name);   
        }
    }

    partial void OnInputValueChanged(double oldValue, double newValue)
    {
        if (newValue == oldValue || newValue == _model.InputValue) return;
        _model.InputValue = newValue;
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
