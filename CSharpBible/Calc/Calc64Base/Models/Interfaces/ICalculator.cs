// ***********************************************************************
// Assembly         : Calc64Base
// Author           : Mir
// Created          : 08-27-2022
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="Calc64.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

namespace Calc64Base.Models.Interfaces;

public interface ICalculator : INotifyPropertyChanged
{
    long Accumulator { get; set; }
    long Memory { get; set; }
    Exception? LastError { get; set; }
    long Register { get; set; }
    EOpMode OperationMode { get; }

    /// <summary>
    /// Occurs when [calculate operation changed].
    /// </summary>
    public event EventHandler<(string prop, object? oldVal, object? newVal)>? CalcOperationChanged;
    /// <summary>
    /// Occurs when [calculate operation error].
    /// </summary>
    public event EventHandler<Exception>? CalcOperationError;

    void BackSpace(int nBase = 10);
    void Button(int aNumber, int nBase = 10);
    void Operation(int v);
}