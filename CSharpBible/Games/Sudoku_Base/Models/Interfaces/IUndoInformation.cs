// ***********************************************************************
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
using System;
using System.Collections.Generic;

namespace Sudoku_Base.Models.Interfaces;

public interface IUndoInformation
{
   WeakReference<ISudokuField> Field { get; }
   IList<(object? ov,object? nv)> list { get; }

    void Redo();
    void Undo();

    void TryUpdateNewValue(object newValue);
}