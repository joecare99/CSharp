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

namespace Calc64Base
{
    public interface ICalculator
    {
        long Accumulator { get; set; }
        long Memory { get; set; }
        Exception? LastError { get; set; }

        /// <summary>
        /// Occurs when [calculate operation changed].
        /// </summary>
        public event EventHandler<(string prop, object? oldVal, object? newVal)>? CalcOperationChanged;
        /// <summary>
        /// Occurs when [calculate operation error].
        /// </summary>
        public event EventHandler<Exception>? CalcOperationError;

    }
}