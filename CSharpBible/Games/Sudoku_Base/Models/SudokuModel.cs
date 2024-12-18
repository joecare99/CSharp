﻿// ***********************************************************************
// Assembly         : Sudoku_Base
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="SudokuModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sudoku_Base.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using BaseLib.Helper;
using System.Windows.Media;
using System.Globalization;
using System.Windows;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace Sudoku_Base.Models;

/// <summary>
/// Class SudokuModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="ISudokuModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="ISudokuModel" />
/// <autogeneratedoc />
public partial class SudokuModel : ObservableObject, ISudokuModel
{
    private const string csApplStart = "Application startet";
#if !NET5_0_OR_GREATER
    private const string csApplEnded = "Application ended";
#endif
    private const string cValueFkt = "Value(";
    #region Properties
    /// <summary>
    /// The timer
    /// </summary>
    /// <autogeneratedoc />
    private readonly Timer _timer;
    private readonly ISysTime _systime;
    private readonly ILog _log;

    private bool _selfInflChange = false;
    private IList<IUndoInformation> undoInformation = new List<IUndoInformation>();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
    [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
    [NotifyPropertyChangedFor(nameof(RedoIndex))]
    private int _undoIndex = -1;

    private Dictionary<System.Drawing.Point, ISudokuField> Fields = new();

    IReadOnlyList<ISudokuField> ISudokuModel.Fields => Fields.Values.ToList();
    IReadOnlyList<int?> ISudokuModel.Values => Fields.Values.Select(f=>f.Value).ToList();
    public ISudokuField this[int row, int col] => Fields[new(row, col)];
    
    public int RedoIndex => undoInformation.Count - UndoIndex - 1;

    public static IEnumerable<(string, Type)> PropTypes => [
        (nameof(Fields),typeof(IEnumerable<SudokuField>))
    ];

    IEnumerable<(string, Type)> IPersistence.PropTypes => PropTypes;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="SudokuModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public SudokuModel(ISysTime sysTime, ILog log)
    {
        _systime = sysTime;
        _log = log;
        _log.Log(csApplStart);
        _timer = new(250d);
        //            _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var field = new SudokuField(new System.Drawing.Point(row, col), null, false, Array.Empty<int>());
                Fields.Add(field.Position, field);
                field.PropertyChanging += FieldPropChanging;
                field.PropertyChanged += FieldPropChanged;
            }
        }
    }

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    ~SudokuModel()
    {
        _timer.Stop();
        _log.Log(csApplEnded);
        return;
    }
#endif
    bool CanUndo() => UndoIndex >= 0 && UndoIndex < undoInformation.Count;

    bool CanRedo() => UndoIndex < undoInformation.Count - 1 && UndoIndex >= -1;

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        if (UndoIndex >= 0)
            _selfInflChange = true;
        try
        {
            undoInformation[UndoIndex--].Undo();
        }
        finally
        {
            _selfInflChange = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        if (UndoIndex < undoInformation.Count - 1)
            _selfInflChange = true;
        try
        {
            undoInformation[++UndoIndex].Redo();
        }
        finally
        {
            _selfInflChange = false;
        }
    }

    public void Clear()
    {
        foreach (var field in Fields.Values)
        {
            field.Clear();
        }
        ClearUndoList();
    }

    public bool ReadFromStream(Stream stream)
    {
        if (stream == null) return false;
        return ReadFromEnumerable(stream.StreamToEnumerable(PropTypes)); ;
    }

    public bool ValuesFromStream(Stream stream)
    {
        if (stream == null) return false;
        return ReadFromEnumerable(stream.StreamToEnumerable(Fields.Select(pt=>($"{cValueFkt}{pt.Key.X},{pt.Key.Y})", typeof(byte))))); 
    }

    private void FieldPropChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (_selfInflChange) { return; }
        while (UndoIndex < undoInformation.Count - 1)
        {
            undoInformation.RemoveAt(undoInformation.Count - 1);
        }
        undoInformation.Add(new UndoInformation(sender as ISudokuField, [(sender?.GetProp(e.PropertyName ?? ""), null)]));
        UndoIndex++;
    }

    private void FieldPropChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_selfInflChange) { return; }
        if (undoInformation[UndoIndex].Field.TryGetTarget(out var target) & target == sender)
        {
            undoInformation[UndoIndex].TryUpdateNewValue(sender?.GetProp(e.PropertyName ?? ""));
        }
    }

    public bool WriteToStream(Stream stream, bool xInclState)
    {
        if (stream == null) return false;
        stream.EnumerateToStream(EnumerateProp());
        return true;
    }

    public bool ValuesToStream(Stream stream)
    {
        if (stream == null) return false;
        stream.EnumerateToStream(EnumerateValues());
        return true;
    }

    private void ClearUndoList()
    {
        undoInformation.Clear();
        UndoIndex = -1;
    }

    public IEnumerable<(string, object)> EnumerateProp()
    {
        yield return (nameof(Fields), Fields.Values);
    }

    public IEnumerable<(string, object)> EnumerateValues()
    {
        foreach (var pt in Fields)
            yield return ($"{cValueFkt}{pt.Key.X},{pt.Key.Y})", (byte)(pt.Value.Value ?? 0));
    }

    public bool ReadFromEnumerable(IEnumerable<(string, object)> enumerable)
    {
        foreach (var pt in enumerable)
            switch (pt)
            {
                case (nameof(Fields), IEnumerable<IPersistence> fields):

                    Fields.Clear();
                    foreach (ISudokuField field in fields)
                    {
                        Fields.Add(field.Position, field);
                        field.PropertyChanging += FieldPropChanging;
                        field.PropertyChanged += FieldPropChanged;
                    }
                    break;
                case (string s, byte value) when s.StartsWith(cValueFkt):
                    var sfield = Fields.FirstOrDefault(pt => s == $"{cValueFkt}{pt.Key.X},{pt.Key.Y})").Value;
                    if (sfield != null)
                    {
                        sfield.Clear();
                        sfield.Value = value != 0 ? value : null;
                        sfield.IsPredefined = value != 0;
                    }
                    break;

            }
        ClearUndoList();
        return true;
    }

    public void DrawSudoku(string title, object data, DrawingContext dc, Rect r)
    {
        if (data is not IReadOnlyList<int?> sudoku)
        {
            return;
        }
        var typeface = new Typeface("Arial");
        var formattedText = new FormattedText(title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, r.Width / 18.0, Brushes.Black, 1.0);
        dc.DrawText(formattedText, r.TopLeft);

        // Draw the grid
        var thinPen = new Pen(Brushes.DarkGray, 0.5);
        var normalPen = new Pen(Brushes.Black, 1.0);
        for (int i = 0; i <= 9; i++)
        {
            dc.DrawLine(i % 3 == 0 ? normalPen : thinPen, new Point(r.Left + i * r.Width / 9.0, (r.Height - r.Width) * 0.5), new Point(r.Left + i * r.Width / 9.0, (r.Height + r.Width) * 0.5));
            dc.DrawLine(i % 3 == 0 ? normalPen : thinPen, new Point(r.Left, (r.Height - r.Width) * 0.5 + i * r.Width / 9.0), new Point(r.Left + r.Width, (r.Height - r.Width) * 0.5 + i * r.Width / 9.0));
        }

        // Draw the numbers
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                if (sudoku[i + j * 9] is int iV)
                {
                    formattedText = new FormattedText(iV.ToString() ?? "", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, r.Width / 9.5, Brushes.Black, 1.0);
                    dc.DrawText(formattedText, new Point(r.Left + (i + 0.25) * r.Width / 9.0, (r.Height - r.Width) * 0.5 + j * r.Width / 9.0));
                }
    }

    #endregion
}
