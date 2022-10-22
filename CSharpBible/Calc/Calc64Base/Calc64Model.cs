﻿// ***********************************************************************
// Assembly         : Calc64Base
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="Calc64Model.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using System;

namespace Calc64Base
{
    /// <summary>
    /// Class Calc64Model.
    /// Implements the <see cref="Calc64Base.Calc64" />
    /// </summary>
    /// <seealso cref="Calc64Base.Calc64" />
    public class Calc64Model : Calc64
    {
        /// <summary>
        /// Enum Exceptions
        /// </summary>
        public enum eOpMode
        {
            /// <summary>
            /// The no mode
            /// </summary>
            NoMode = 0,
            /// <summary>
            /// The calculate result
            /// </summary>
            CalcResult = 1, Plus = 2, Minus = 3, Multiply = 4, Divide = 5,
            /// <summary>
            /// The none
            /// </summary>
            BinaryAnd = 6, BinaryOr = 7, BinaryXor = 8, BinaryNot = 9,
            /// <summary>
            /// The e div by zero ex
            /// </summary>
            Power = 10, Negate=11, Modulo=12,
            /// <summary>
            /// The nand
            /// </summary>
            Nand = 13, Nor=14, XNor=15, BinaryEquals=16,

            /// <summary>
            /// The memory retreive
            /// </summary>
            MemRetreive = 20, MemStore=21, MemAdd=22, MemSubtract=23, MemClear=24
        };

        /// <summary>
        /// Gets or sets the operation mode.
        /// </summary>
        /// <value>The operation mode.</value>
        public eOpMode OperationMode { get => _nMode; set => Property.SetProperty(ref _nMode, value, PropChange); }

        /// <summary>
        /// The b edit mode
        /// </summary>
        private bool bEditMode;
        /// <summary>
        /// The n mode
        /// </summary>
        private eOpMode _nMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Calc64Model"/> class.
        /// </summary>
        public Calc64Model()
        {
            OperationMode = 0;
        }

        public static string GetShortDesc(eOpMode e) => (e!=eOpMode.CalcResult)? Calc64.ToCalcOperation((int)e)?.ShortDescr ?? "":"=";
        /// <summary>
        /// Buttons the specified a number.
        /// </summary>
        /// <param name="aNumber">a number.</param>
        /// <param name="nBase">The n base.</param>
        public void Button(int aNumber,int nBase=10)
        {
            if (!bEditMode)
                (bEditMode, Accumulator) = (true, aNumber);
            else if (aNumber<nBase && Accumulator < Int64.MaxValue / nBase)
                    Accumulator = Accumulator * nBase + aNumber;
            
        }

        /// <summary>
        /// Operations the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        public void Operation(int v)
        {
            if (v > 0)
            {
                bEditMode = false;
                if (OperationMode > eOpMode.CalcResult && ((eOpMode)v == eOpMode.CalcResult || IsRegisterOpeation(v)))
                    DoOpeation((int)OperationMode);
                if ((eOpMode)v == eOpMode.CalcResult)
                    (OperationMode, Register) = ((eOpMode)v, 0);
                else if (IsRegisterOpeation(v))
                    (OperationMode, Register) = ((eOpMode)v, Accumulator);
                else DoOpeation(v);
            }
            if (v<0)
                switch(-v)
                {
                    // clear editor
                    case 1: Accumulator = 0; break;
                        // Clear all
                    case 3: (Accumulator, Register, OperationMode) = (0, 0,eOpMode.NoMode); break;
                    default:break;
                }
        }


        /// <summary>
        /// Backs the space.
        /// </summary>
        /// <param name="nBase">The n base.</param>
        public void BackSpace(int nBase=10)
        {
            if (!bEditMode)
                (bEditMode, Accumulator) = (true, 0);
            else Accumulator = /*floor*/ Accumulator / nBase;

        }
    }
}
