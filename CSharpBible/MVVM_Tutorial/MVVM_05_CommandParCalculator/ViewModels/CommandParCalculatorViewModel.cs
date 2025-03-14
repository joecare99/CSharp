﻿// ***********************************************************************
// Assembly         : MVVM_05_CommandParCalculator_net
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="CommandParCalculatorViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_05_CommandParCalculator.Model;
using System;
using System.ComponentModel;

/// <summary>
/// The ViewModels namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_05_CommandParCalculator.ViewModels;

/// <summary>
/// Class CommandParCalculatorViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
/// <autogeneratedoc />
public class CommandParCalculatorViewModel : BaseViewModel
{
    #region Properties
    /// <summary>
    /// Gets or sets the get model.
    /// </summary>
    /// <value>The get model.</value>
    /// <autogeneratedoc />
    public static Func<ICalculatorModel> GetModel { get; set; } = () => CalculatorModel.Instance;
    /// <summary>
    /// The model
    /// </summary>
    /// <autogeneratedoc />
    private ICalculatorModel _model;
    /// <summary>
    /// Gets or sets the number command.
    /// </summary>
    /// <value>The number command.</value>
    /// <autogeneratedoc />
    public RelayCommand<ENumbers> NumberCommand { get; set; }
    /// <summary>
    /// Gets or sets the operator command.
    /// </summary>
    /// <value>The operator command.</value>
    /// <autogeneratedoc />
    public RelayCommand<EOperations> OperatorCommand { get; set; }
    /// <summary>
    /// Gets or sets the calculator command.
    /// </summary>
    /// <value>The calculator command.</value>
    /// <autogeneratedoc />
    public RelayCommand<ECommands> CalculatorCommand { get; set; }

    /// <summary>
    /// Gets the accumulator.
    /// </summary>
    /// <value>The accumulator.</value>
    /// <autogeneratedoc />
    public double Accumulator => _model.Accumulator;
    /// <summary>
    /// Gets the memory.
    /// </summary>
    /// <value>The memory.</value>
    /// <autogeneratedoc />
    public double Memory => _model.Memory ?? double.NaN;
    /// <summary>
    /// Gets the register.
    /// </summary>
    /// <value>The register.</value>
    /// <autogeneratedoc />
    public double Register => _model.Register ?? double.NaN;
    /// <summary>
    /// Gets the status.
    /// </summary>
    /// <value>The status.</value>
    /// <autogeneratedoc />
    public string Status => $"{_model.CalcError} {_model.TrigMode} ";
    #endregion

    #region Methods
    /// <summary>
    /// Determines whether this instance can operator the specified e o.
    /// </summary>
    /// <param name="eO">The e o.</param>
    /// <returns><c>true</c> if this instance can operator the specified e o; otherwise, <c>false</c>.</returns>
    /// <autogeneratedoc />
    public bool canOperator(EOperations eO) => FuncProxy(eO, _model.canOperator);
    /// <summary>
    /// Determines whether this instance can command the specified e c.
    /// </summary>
    /// <param name="eC">The e c.</param>
    /// <returns><c>true</c> if this instance can command the specified e c; otherwise, <c>false</c>.</returns>
    /// <autogeneratedoc />
    public bool canCommand(ECommands eC) => FuncProxy(eC, _model.canCommand);

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandParCalculatorViewModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public CommandParCalculatorViewModel()
    {
        _model = GetModel();
        _model.PropertyChanged += OnPropertyChanged;
        NumberCommand = new(_model.NumberCmd);
        OperatorCommand = new(_model.OperatorCmd,canOperator);
        CalculatorCommand = new(_model.CalcCmd, canCommand);
        foreach (var d in _model.Dependencies)
            AddPropertyDependency(d.Dest, d.Src);
        AddPropertyDependency(nameof(OperatorCommand), nameof(canOperator));
        AddPropertyDependency(nameof(CalculatorCommand), nameof(canCommand));
    }

    /// <summary>
    /// Handles the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    /// <autogeneratedoc />
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Accumulator))
            RaisePropertyChanged(e.PropertyName);
        if (e.PropertyName == nameof(Memory))
            RaisePropertyChanged(e.PropertyName);
        if (e.PropertyName == nameof(Register))
            RaisePropertyChanged(e.PropertyName);
    }
    #endregion

}
