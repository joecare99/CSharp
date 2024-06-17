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
using CommunityToolkit.Mvvm.ComponentModel;
using Sudoku_Base.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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

    public SudokuField()
    {
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
        var streamBytes = new byte[sizeof(int)*2];
        int i;
        stream.Read(streamBytes, 0, sizeof(int)*2);
        Position =new Point( BitConverter.ToInt32(streamBytes, 0), BitConverter.ToInt32(streamBytes, sizeof(int)));
        stream.Read(streamBytes, 0, sizeof(int) * 2);
        Value = (i = BitConverter.ToInt32(streamBytes, 0))!=-1?i:null;

        IsPredefined = BitConverter.ToBoolean(streamBytes, sizeof(int));
        var count = BitConverter.ToInt16(streamBytes, sizeof(int)+2);
        streamBytes = new byte[sizeof(int) * count];
        stream.Read(streamBytes, 0, sizeof(int) * count);
        for (i=0; i<count; i++ )
        {
            PossibleValues.Add(BitConverter.ToInt32(streamBytes, sizeof(int)*i));
        }
    }

    public void WriteToStream(Stream stream)
    {
        stream.Write(BitConverter.GetBytes(Position.X), 0, sizeof(int));
        stream.Write(BitConverter.GetBytes(Position.Y), 0, sizeof(int));
        stream.Write(BitConverter.GetBytes(Value??-1), 0, sizeof(int));
        stream.Write(BitConverter.GetBytes(IsPredefined), 0, sizeof(bool));
        stream.WriteByte((byte)0);//padding
        stream.Write(BitConverter.GetBytes((short)PossibleValues.Count), 0, sizeof(short));
        foreach (var value in PossibleValues)
        {
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(int));
        }
    }
}