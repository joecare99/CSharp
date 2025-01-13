// ***********************************************************************
// Assembly         : Sudoku_Base
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="SudokuModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Sudoku_Base.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using BaseLib.Helper;
namespace Sudoku_Base.Models;

public partial class SudokuField : ObservableObject, ISudokuField
{
    [ObservableProperty]
    private Point _position;

    [ObservableProperty]
    private int? _value;

    [ObservableProperty]
    private bool _IsPredefined;

    [ObservableProperty]
    private ObservableCollection<int> _possibleValues = new();
    IList<int> ISudokuField.PossibleValues => PossibleValues;

    public static IEnumerable<(string, Type)> PropTypes => [
        (nameof(Position),typeof(Point)),
            (nameof(Value), typeof(int)),
            (nameof(IsPredefined),typeof(bool)),
            ("Dummy", typeof(byte)),
            (nameof(PossibleValues),typeof(IEnumerable<int>))
        ];

    IEnumerable<(string, Type)> IPersistence.PropTypes => PropTypes;

    public SudokuField()
    {
    }

    public SudokuField(Point position, int? value, bool isPredefined, int[] pValues)
    {
        Position = position;
        Value = value;
        IsPredefined = isPredefined;
        foreach (var pValue in pValues)
            PossibleValues.Add(pValue);
    }

    public void AddPossibleValue(int value)
    {
        if (!PossibleValues.Contains(value))
        {
            PossibleValues.Add(value);
            OnPropertyChanged(nameof(PossibleValues));
        }
    }

    public void RemovePossibleValue(int value)
    {
        if (PossibleValues.Contains(value))
        {
            PossibleValues.Remove(value);
            OnPropertyChanged(nameof(PossibleValues));
        }
    }
    public void ReadFromStream(Stream stream)
    {

        ReadFromEnumerable(stream.StreamToEnumerable( PropTypes));
    }

    public void WriteToStream(Stream stream)
    {
        stream.EnumerateToStream(EnumerateProp());
    }

    public void Clear()
    {
        Value = null;
        IsPredefined = false;
        PossibleValues.Clear();
    }

    public IEnumerable<(string, object)> EnumerateProp()
    {
        yield return (nameof(Position), Position);
        yield return (nameof(Value), Value ?? -1);
        yield return (nameof(IsPredefined), IsPredefined);
        yield return ("Dummy", (byte)0);
        yield return (nameof(PossibleValues), PossibleValues);
    }

    public bool ReadFromEnumerable(IEnumerable<(string, object)> enumerable)
    {
        foreach ((string prop, object value) pv in enumerable)
        {
            switch (pv)
            {
                case (nameof(Position),Point p):
                    Position = p;
                    break;
                case (nameof(Value),int i):
                    Value = i != -1 ? i : null;
                    break;
                case (nameof(IsPredefined),bool x):
                    IsPredefined = x;
                    break;
                case (nameof(PossibleValues), IEnumerable<int?> ei):
                    PossibleValues.Clear();
                    foreach (var value in ei)
                    {
                        PossibleValues.Add(value.Value);
                    }
                    break;
                default: // Unbekannte Werte werden ignoriert
                    break;
            }
        }
        return true;
    }

}