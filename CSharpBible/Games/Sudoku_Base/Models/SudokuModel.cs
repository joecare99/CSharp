﻿// ***********************************************************************
// Assembly         : MVVM_41_Sudoku
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
using CommunityToolkit.Mvvm.Input;
using Sudoku_Base.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using BaseLib.Helper;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace Sudoku_Base.Models
{
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
        #region Properties
        /// <summary>
        /// The timer
        /// </summary>
        /// <autogeneratedoc />
        private readonly Timer _timer;
        private readonly ISysTime _systime;
        private readonly ILog _log;

        private bool _selfInflChange =false;
        private IList<IUndoInformation> undoInformation = new List<IUndoInformation>();
        private int _undoIndex = -1;
            
        private Dictionary<Point,ISudokuField> Fields = new();

        IReadOnlyList<ISudokuField> ISudokuModel.Fields => Fields.Values.ToList();
        public ISudokuField this[int row, int col] => Fields[new Point(row,col)];
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
            for(int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var field = new SudokuField(new Point(row, col), null, false, Array.Empty<int>());
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


        [RelayCommand]
        private void Undo()
        {
            if (_undoIndex >= 0)
            _selfInflChange = true;
            try
            {
                undoInformation[_undoIndex--].Undo();
            }
            finally
            {
                _selfInflChange = false;
            }
        }

        [RelayCommand]
        private void Redo()
        {
            if (_undoIndex < undoInformation.Count-1)
                _selfInflChange = true;
            try
            {
                undoInformation[++_undoIndex].Redo();

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
            var streamBytes = new byte[sizeof(int)];
            stream.Read(streamBytes, 0, sizeof(int));
            var c= BitConverter.ToInt32(streamBytes,0);
            Fields.Clear();
            for (int i = 0; i < Math.Min(c,81); i++)
            {
                var field = new SudokuField();
                field.ReadFromStream(stream);
                Fields.Add(field.Position, field);
                field.PropertyChanging += FieldPropChanging;
                field.PropertyChanged += FieldPropChanged;
            }
            return true;
        }

        private void FieldPropChanging(object sender, PropertyChangingEventArgs e)
        {
             if (_selfInflChange) { return; }
             while (_undoIndex < undoInformation.Count - 1)
             {
                 undoInformation.RemoveAt(undoInformation.Count - 1);
             }
             undoInformation.Add(new UndoInformation(sender as ISudokuField, [(sender?.GetProp(e.PropertyName ?? ""),null)]));
            _undoIndex++;
        }

        private void FieldPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_selfInflChange) { return; }
            if (undoInformation[_undoIndex].Field.TryGetTarget(out var target) & target == sender) 
            {
                undoInformation[_undoIndex].TryUpdateNewValue(sender?.GetProp(e.PropertyName ?? ""));
            }
        }

        public bool WriteToStream(Stream stream, bool xInclState)
        {
            if (stream == null) return false;
            stream.Write(BitConverter.GetBytes(Fields.Count), 0, sizeof(int));
            foreach (var field in Fields.Values)
            {
                field.WriteToStream(stream);
            }
            ClearUndoList();
            return true;
        }

        private void ClearUndoList()
        {
            undoInformation.Clear();
        }
        #endregion
    }
}
