// ***********************************************************************
// Assembly         : Calc32
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-07-2022
// ***********************************************************************
// <copyright file="CalculatorClass.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

/// <summary>
/// The NonVisual namespace.
/// </summary>
namespace Calc32.NonVisual

{
    /// <summary>
    /// Class CalculatorClass.
    /// Implements the <see cref="Component" />
    /// </summary>
    public class CalculatorClass : Component
    {
        /// <summary>
        /// Enum Mode
        /// of Operation
        /// </summary>
        enum eOpMode
        {
            /// <summary>
            /// No mode
            /// </summary>
            NoMode =  0,

            /// <summary>
            /// The calculate result
            /// </summary>
            CalcResult = 1,
            /// <summary>
            /// The plus mode
            /// </summary>
            Plus = 2,
            /// <summary>
            /// The minus mode
            /// </summary>
            Minus = 3,
            /// <summary>
            /// The multiply mode
            /// </summary>
            Multiply = 4,
            /// <summary>
            /// The divide
            /// mode
            /// </summary>
            Divide = 5,
            /// <summary>
            /// The binary and
            /// mode
            /// </summary>
            BinaryAnd = 6,
            /// <summary>
            /// The binary or
            /// mode
            /// </summary>
            BinaryOr = 7,
            /// <summary>
            /// The binary xor
            /// mode
            /// </summary>
            BinaryXor = 8,
            /// <summary>
            /// The binary not
            /// mode
            /// </summary>
            BinaryNot = 9
        };

        #region Property
        #region private property
        // Fields
        /// <summary>
        /// The n akkumulator
        /// </summary>
        private int nAkkumulator; // Editorfeld
        /// <summary>
        /// The n mode
        /// </summary>
        private eOpMode nMode;
        /// <summary>
        /// The b edit mode
        /// </summary>
        private bool bEditMode;
        // private bool bNegMode;
        /// <summary>
        /// The n memory
        /// </summary>
        private int nMemory; // Gemerkte Zahl für Operationen
        #endregion

        /// <summary>
        /// The mode-string
        /// </summary>
        public readonly static string[] sMode = { "", "=", "+", "-", "*", "/", "&", "|", "x", "!" };
        /// <summary>
        /// Occurs when a change happens.
        /// </summary>
        public event EventHandler OnChange;

        // Properties
        /// <summary>
        /// Gets or sets the akkumulator.
        /// </summary>
        /// <value>The akkumulator.</value>
        public int Akkumulator
        {
            get => nAkkumulator;
            set
            {
                if (value == nAkkumulator) return;
                nAkkumulator = value;
                OnChange?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Gets or sets the memory.
        /// </summary>
        /// <value>The memory.</value>
        public int Memory
        {
            get => nMemory;
            set
            {
                if (value == nMemory) return;
                nMemory = value;
                OnChange?.Invoke(this, null);
            }
        }
        /// <summary>
        /// Gets the operation text.
        /// </summary>
        /// <value>The operation text.</value>
        public string OperationText => sMode[(int)nMode];
        #endregion

        #region Methode
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Calc32.NonVisual.CalculatorClass" /> class.
        /// </summary>
        public CalculatorClass()
        {
            nAkkumulator = 0;
            nMode = 0;
            OnChange = null;
        }

        /// <summary>
        /// Button of Numbers.
        /// </summary>
        /// <param name="aNumber">a number.</param>
        public void NumberButton(int aNumber)
        {
            if (bEditMode)
            {
                if (nAkkumulator < int.MaxValue / 10)
                {
                    Akkumulator = nAkkumulator * 10 + aNumber;
                }
            }
            else
            {
                bEditMode = true;
                Akkumulator = aNumber;
            }
        }

        /// <summary>
        /// Executes the specified Operation.
        /// </summary>
        /// <param name="v">The operation.</param>
        public void Operation(int v)
        {
            if ((v > 0) && (v <= (int)eOpMode.BinaryNot))
            {
                bEditMode = false;
                switch (nMode)
                {
                    case eOpMode.Plus:
                        nAkkumulator = nMemory + nAkkumulator;
                        break;
                    case eOpMode.Minus:
                        nAkkumulator = nMemory - nAkkumulator;
                        break;
                    case eOpMode.Multiply:
                        nAkkumulator = nMemory * nAkkumulator;
                        break;
                    case eOpMode.Divide:
                        nAkkumulator = nMemory / nAkkumulator;
                        break;
                    case eOpMode.BinaryAnd:
                        nAkkumulator = nMemory & nAkkumulator;
                        break;
                    case eOpMode.BinaryOr:
                        nAkkumulator = nMemory | nAkkumulator;
                        break;
                    case eOpMode.BinaryXor:
                        nAkkumulator = nMemory ^ nAkkumulator;
                        break;
                    default:
                        break;
                }

                if ((eOpMode)v == eOpMode.CalcResult)
                {
                    nMode = (eOpMode)v;
                    Memory = 0;
                }
                else if ((eOpMode) v == eOpMode.BinaryNot)
                {
                    Akkumulator = ~nAkkumulator;
                }
                else
                {
                    nMode = (eOpMode)v;
                    Memory = nAkkumulator;
                }

            }
        }

        /// <summary>
        /// Backs the space.
        /// </summary>
        public void BackSpace()
        {
            if (bEditMode)
            {
                Akkumulator = nAkkumulator / 10;
            }
            else
            {
                Akkumulator = 0;
            }
        }
        #endregion
    }
}
