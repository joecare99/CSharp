﻿// ***********************************************************************
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
using BaseLib.Helper;
using Calc32.Models.Interfaces;
using Microsoft.Win32;
using System;
using System.ComponentModel;

/// <summary>
/// The NonVisual namespace.
/// </summary>
namespace Calc32.Models;


/// <summary>
/// Class CalculatorClass.
/// Implements the <see cref="Component" />
/// </summary>
public class CalculatorClass : Component, ICalculatorClass
{
    /// <summary>
    /// Enum Mode
    /// of Operation
    /// </summary>
    public enum eOpMode
    {
        /// <summary>
        /// No mode
        /// </summary>
        NoMode = 0,

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
        BinaryNot = 9,
        /// <summary>
        /// The binary not
        /// mode
        /// </summary>
        Negate = 10
    };

    #region Property
    #region private property
    // Fields
    /// <summary>
    /// The accumulator
    /// </summary>
    private int _accumulator; // Editorfeld
    /// <summary>
    /// The n mode
    /// </summary>
    private eOpMode nMode { get => _nMode; set => value.SetProperty(ref _nMode, FireModeChangeEvent); }

    /// <summary>
    /// The b edit mode
    /// </summary>
    private bool bEditMode;
    // private bool bNegMode;
    /// <summary>
    /// The n memory
    /// </summary>
    private int _register; // Gemerkte Zahl für Operationen
    private eOpMode _nMode;
    private int _memory;
    #endregion

    /// <summary>
    /// The mode-string
    /// </summary>
    public readonly static string[] sMode = { "", "=", "+", "-", "*", "/", "&", "|", "x", "!", "+/-","CE","<=","C"  };
    /// <summary>
    /// Occurs when a change happens.
    /// </summary>
    public event EventHandler<(string, object?, object?)>? OnChange;

    // Properties
    /// <summary>
    /// Gets or sets the accumulator.
    /// </summary>
    /// <value>The accumulator.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Accumulator
    {
        get => _accumulator;
        set => value.SetProperty(ref _accumulator, FireChangeEvent);
    }

    /// <summary>
    /// Gets or sets the memory.
    /// </summary>
    /// <value>The memory.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Register
    {
        get => _register;
        private set => value.SetProperty(ref _register, FireChangeEvent);
    }

    /// <summary>
    /// Gets or sets the memory.
    /// </summary>
    /// <value>The memory.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int Memory
    {
        get => _memory;
        set => value.SetProperty(ref _memory, FireChangeEvent);
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
        _accumulator = 0;
        nMode = 0;
        OnChange = null;
    }
    private void FireChangeEvent(string Prop, int ol, int nw)
    {
        OnChange?.Invoke(this, (Prop, ol, nw));
    }

    private void FireModeChangeEvent(string arg1, eOpMode arg2, eOpMode arg3)
    {
        OnChange?.Invoke(this, (nameof(OperationText), sMode[(int)arg2], sMode[(int)arg3]));
    }


    /// <summary>
    /// Button of Numbers.
    /// </summary>
    /// <param name="aNumber">a number.</param>
    public void NumberButton(int aNumber)
    {
        if (bEditMode)
        {
            if (_accumulator < int.MaxValue / 10)
            {
                Accumulator = _accumulator * 10 + aNumber;
            }
        }
        else
        {
            bEditMode = true;
            Accumulator = aNumber;
        }
    }

    /// <summary>
    /// Executes the specified Operation.
    /// </summary>
    /// <param name="v">The operation.</param>
    public void Operation(int v)
    {
        if ((v > 0) && (v <= (int)eOpMode.Negate))
        {
            bEditMode &= (v == (int)eOpMode.Negate);
            Accumulator = nMode switch
            {
                eOpMode.Plus => _register + _accumulator,
                eOpMode.Minus => _register - _accumulator,
                eOpMode.Multiply => _register * _accumulator,
                eOpMode.Divide => _register / _accumulator,
                eOpMode.BinaryAnd => _register & _accumulator,
                eOpMode.BinaryOr => _register | _accumulator,
                eOpMode.BinaryXor => _register ^ _accumulator,
                _ => _accumulator
            };

            if ((eOpMode)v == eOpMode.CalcResult)
            {
                nMode = (eOpMode)v;
                Register = 0;
            }
            else if ((eOpMode)v == eOpMode.BinaryNot)
            {
                Accumulator = ~_accumulator;
            }
            else if ((eOpMode)v == eOpMode.Negate)
            {
                Accumulator = -_accumulator;
            }
            else
            {
                nMode = (eOpMode)v;
                Register = _accumulator;
            }

        }
        else if (v == -3)
        {
            Accumulator = 0;
            Register = 0;
            Memory = 0;
            nMode = eOpMode.NoMode;
        }
        else if (v == -1)
        {
            Accumulator = 0;
        }
    }

    /// <summary>
    /// Backs the space.
    /// </summary>
    public void BackSpace()
        => Accumulator = bEditMode ? _accumulator / 10 : 0;
    #endregion
}
