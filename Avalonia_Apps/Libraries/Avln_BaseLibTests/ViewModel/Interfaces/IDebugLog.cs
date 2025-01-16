// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork_netTests
// Author           : Mir
// Created          : 05-14-2023
//
// Last Modified By : Mir
// Last Modified On : 01-16-2025
// ***********************************************************************
// <copyright file="IDebugLog.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace Avalonia.ViewModels.Interfaces;

/// <summary>
/// Interface IDebugLog
/// </summary>
public interface IDebugLog
{
    /// <summary>
    /// Does the log.
    /// </summary>
    /// <param name="message">The message.</param>
    void DoLog(string message);
    /// <summary>
    /// Clears the log.
    /// </summary>
    void ClearLog();
    /// <summary>
    /// Gets the debug log.
    /// </summary>
    /// <value>The debug log.</value>
    string DebugLog { get; }
}