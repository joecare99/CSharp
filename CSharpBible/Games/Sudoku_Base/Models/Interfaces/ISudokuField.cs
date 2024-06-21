// ***********************************************************************
// Assembly         : Sudoku_Base
// Author           : Mir
// Created          : 06-17-2024
//
// Last Modified By : Mir
// Last Modified On : 06-17-2024
// ***********************************************************************
// <copyright file="ISudokuField.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace Sudoku_Base.Models.Interfaces;

/// <summary>
/// Interface ISudokuField
/// Extends the <see cref="INotifyPropertyChanged" />
/// </summary>
/// <seealso cref="INotifyPropertyChanged" />
public interface ISudokuField : INotifyPropertyChanged, INotifyPropertyChanging
{
    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <value>The position.</value>
    Point Position { get; }
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    int? Value { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is predefined.
    /// </summary>
    /// <value><c>true</c> if this instance is predefined; otherwise, <c>false</c>.</value>
    bool IsPredefined { get; set; }

    /// <summary>
    /// Gets the list of possible values.
    /// </summary>
    /// <value>The possible values.</value>
    IList<int> PossibleValues { get; }

    /// <summary>
    /// Adds the possible value to the list.
    /// </summary>
    /// <param name="value">The value.</param>
    void AddPossibleValue(int value);
    /// <summary>
    /// Removes the possible value from the list.
    /// </summary>
    /// <param name="value">The value.</param>
    void RemovePossibleValue(int value);

    /// <summary>
    /// Writes to stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    void WriteToStream(Stream stream);
    /// <summary>
    /// Reads from stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    void ReadFromStream(Stream stream);
    void Clear();
}
