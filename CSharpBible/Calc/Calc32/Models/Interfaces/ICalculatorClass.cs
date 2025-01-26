// ***********************************************************************
// Assembly         : Calc32
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="CalculatorClass.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace Calc32.Models.Interfaces;

public interface ICalculatorClass
{
    int Accumulator { get; set; }
    int Memory { get; set; }
    string OperationText { get; }

    event EventHandler<(string, object?, object?)>? OnChange;

    void BackSpace();
    void NumberButton(int aNumber);
    void Operation(int v);
}