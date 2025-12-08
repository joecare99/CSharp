// ***********************************************************************
// Assembly         : Avln_BaseLib
// Author           : Mir
// Created          : 01-17-2025
//
// Last Modified By : Mir
// Last Modified On : 01-16-2025
// ***********************************************************************
// <copyright file="ILog.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace BaseLib.Models.Interfaces;

/// <summary>
/// Interface ILog
/// </summary>
public interface ILog
{
    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Log(string message);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Log(string message, Exception exception);
}
