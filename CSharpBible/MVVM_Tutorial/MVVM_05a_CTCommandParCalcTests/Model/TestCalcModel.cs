// ***********************************************************************
// Assembly         : MVVM_05a_CTCommandParCalc_netTests
// Author           : Mir
// Created          : 05-11-2023
//
// Last Modified By : Mir
// Last Modified On : 05-08-2023
// ***********************************************************************
// <copyright file="TestCalcModel.cs" company="JC-Soft">
//     Copyright � JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using MVVM_05a_CTCommandParCalc.Data;
using MVVM_05a_CTCommandParCalc.Model.Interfaces;
using System;
using System.Collections.Generic;

/// <summary>
/// The Model namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_05a_CTCommandParCalc.Model;

/// <summary>
/// Class TestCalcModel.
/// Implements the <see cref="NotificationObject" />
/// Implements the <see cref="MVVM_05a_CTCommandParCalc.Model.ICalculatorModel" />
/// </summary>
/// <seealso cref="NotificationObject" />
/// <seealso cref="MVVM_05a_CTCommandParCalc.Model.ICalculatorModel" />
/// <autogeneratedoc />
internal class TestCalcModel : NotificationObject, ICalculatorModel
{
    /// <summary>
    /// The accumulator
    /// </summary>
    /// <autogeneratedoc />
    private double _accumulator = 0d;
    /// <summary>
    /// The register
    /// </summary>
    /// <autogeneratedoc />
    private double? _register;
    /// <summary>
    /// The memory
    /// </summary>
    /// <autogeneratedoc />
    private double? _memory;
    /// <summary>
    /// The trig mode
    /// </summary>
    /// <autogeneratedoc />
    private ETrigMode _trigMode;
    /// <summary>
    /// The calculate error
    /// </summary>
    /// <autogeneratedoc />
    private ECalcError _calcError;
    /// <summary>
    /// The stack size
    /// </summary>
    /// <autogeneratedoc />
    private int _stackSize;
    /// <summary>
    /// The do log
    /// </summary>
    /// <autogeneratedoc />
    private Action<string> _doLog;

    /// <summary>
    /// Gets or sets the accumulator.
    /// </summary>
    /// <value>The accumulator.</value>
    /// <autogeneratedoc />
    public double Accumulator { get => _accumulator; set => SetProperty(ref _accumulator, value); }
    /// <summary>
    /// Gets or sets the register.
    /// </summary>
    /// <value>The register.</value>
    /// <autogeneratedoc />
    public double? Register { get => _register; set => SetProperty(ref _register, value); }
    /// <summary>
    /// Gets or sets the memory.
    /// </summary>
    /// <value>The memory.</value>
    /// <autogeneratedoc />
    public double? Memory { get => _memory; set => SetProperty(ref _memory, value); }
    /// <summary>
    /// Gets the dependencies.
    /// </summary>
    /// <value>The dependencies.</value>
    /// <autogeneratedoc />
    public IEnumerable<(string Dest, string Src)> Dependencies => new[]{
        (nameof(canOperator),nameof(Accumulator)),
        (nameof(canOperator),nameof(Register)),
    };
    /// <summary>
    /// Gets or sets the trig mode.
    /// </summary>
    /// <value>The trig mode.</value>
    /// <autogeneratedoc />
    public ETrigMode TrigMode { get => _trigMode; set => SetProperty(ref _trigMode, value); }
    /// <summary>
    /// Gets or sets the calculate error.
    /// </summary>
    /// <value>The calculate error.</value>
    /// <autogeneratedoc />
    public ECalcError CalcError { get => _calcError; set => SetProperty(ref _calcError, value); }
    /// <summary>
    /// Gets or sets the size of the stack.
    /// </summary>
    /// <value>The size of the stack.</value>
    /// <autogeneratedoc />
    public int StackSize { get => _stackSize; set => SetProperty(ref _stackSize, value); }
    /// <summary>
    /// Gets or sets a value indicating whether [x result].
    /// </summary>
    /// <value><c>true</c> if [x result]; otherwise, <c>false</c>.</value>
    /// <autogeneratedoc />
    public bool xResult { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestCalcModel"/> class.
    /// </summary>
    /// <param name="DoLog">The do log.</param>
    /// <autogeneratedoc />
    public TestCalcModel(Action<string> DoLog)
    {
        _doLog = DoLog;
    }

    /// <summary>
    /// Calculates the command.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <autogeneratedoc />
    public void CalcCmd(ECommands o)
    {
        _doLog($"CalcCmd({o})");
    }

    /// <summary>
    /// Determines whether this instance can command the specified e c.
    /// </summary>
    /// <param name="eC">The e c.</param>
    /// <returns><c>true</c> if this instance can command the specified e c; otherwise, <c>false</c>.</returns>
    /// <autogeneratedoc />
    public bool canCommand(ECommands eC)
    {
        _doLog($"canCommand({eC})={xResult}");
        return xResult;
    }

    /// <summary>
    /// Determines whether this instance can operator the specified e o.
    /// </summary>
    /// <param name="eO">The e o.</param>
    /// <returns><c>true</c> if this instance can operator the specified e o; otherwise, <c>false</c>.</returns>
    /// <autogeneratedoc />
    public bool canOperator(EOperations eO)
    {
        _doLog($"canOperator({eO}={xResult})");
        return xResult;
    }

    /// <summary>
    /// Numbers the command.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <autogeneratedoc />
    public void NumberCmd(ENumbers o)
    {
        _doLog($"NumberCmd({o})");
    }

    /// <summary>
    /// Operators the command.
    /// </summary>
    /// <param name="eO">The e o.</param>
    /// <autogeneratedoc />
    public void OperatorCmd(EOperations eO)
    {
        _doLog($"OperatorCmd({eO})");
    }
}
