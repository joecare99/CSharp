// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-22-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="CalculatorViewModel.cs" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Calc32.Models;
using System;
using Calc32.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Calc32.ViewModels.Interfaces;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace Calc32.ViewModels;

/// <summary>
/// Class CalculatorViewModel.
/// Implements the <see cref="T:MVVM.ViewModel.BaseViewModel" />
/// </summary>
public partial class CalculatorViewModel : ObservableObject, ICalculatorViewModel
{
    #region Properties
    #region private properties
    /// <summary>
    /// The calculator class
    /// </summary>
    private readonly ICalculatorClass calculatorClass;
    #endregion

    /// <summary>
    /// Gets or sets the akkumulator.
    /// <see cref="CalculatorClass.Accumulator" />
    /// </summary>
    /// <value>The akkumulator.</value>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BackSpaceCommand))]
    [NotifyCanExecuteChangedFor(nameof(OperationCommand))]
    public int _accumulator;

    /// <summary>
    /// Gets or sets the memory.
    /// <see cref="CalculatorClass.Memory" />
    /// </summary>
    /// <value>The memory.</value>
    public int Memory => calculatorClass.Memory;

    /// <summary>
    /// Gets the operation text.
    /// </summary>
    /// <value>The operation text.</value>
    public string OperationText => calculatorClass.OperationText;

    #endregion
    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Calc32WPF.ViewModel.CalculatorViewModel" /> class.
    /// </summary>
    public CalculatorViewModel(ICalculatorClass calculatorClass)
    {
        calculatorClass.OnChange += CalculatorClass_OnChange;
        this.calculatorClass = calculatorClass;
    }


    private bool canBackspace() => calculatorClass.Accumulator != 0;
    [RelayCommand(CanExecute = nameof(canBackspace))]
    private void BackSpace() => calculatorClass.BackSpace();

    private bool canOperation() => calculatorClass.Accumulator != 0;
    [RelayCommand(CanExecute = nameof(canOperation))]
    private void Operation(object? o) => calculatorClass.Operation(int.Parse((string?)o ?? ""));

    [RelayCommand]
    private void Number(object? o) => calculatorClass.NumberButton(int.Parse((string?)o ?? ""));

    /// <summary>
    /// Handles the OnChange event of the CalculatorClass control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void CalculatorClass_OnChange(object? sender, (string, object?, object?) e)
    {
        if (e.Item1 == nameof(Accumulator))
            Accumulator = (int)e.Item3!;
        else if (e.Item1 == nameof(Memory))
            OnPropertyChanged(nameof(Memory));
        else if (e.Item1 == nameof(OperationText))
            OnPropertyChanged(nameof(OperationText));
    }
    #endregion
}
