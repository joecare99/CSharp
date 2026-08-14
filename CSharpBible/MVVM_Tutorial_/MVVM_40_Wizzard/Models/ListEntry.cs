// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page1ViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Models namespace.
/// </summary>
namespace MVVM_40_Wizzard.Models;

/// <summary>
/// Class ListEntry.
/// </summary>
public class ListEntry(int value, string text)
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public int ID { get; } = value;
    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text { get; } = text;

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => Text;
}
