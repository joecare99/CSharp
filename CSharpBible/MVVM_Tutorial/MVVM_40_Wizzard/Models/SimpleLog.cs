// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-12-2024
// ***********************************************************************
// <copyright file="SimpleLog.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Diagnostics;
using System.Globalization;

/// <summary>
/// The Models namespace.
/// </summary>
namespace MVVM_40_Wizzard.Models;

/// <summary>
/// Class SimpleLog.
/// Implements the <see cref="ILog" />
/// </summary>
/// <seealso cref="ILog" />
public class SimpleLog(ISysTime sysTime):ILog
{
    /// <summary>
    /// Gets or sets the log action.
    /// </summary>
    /// <value>The log action.</value>
    public static Action<string> LogAction { get; set; } = (message) => Debug.WriteLine(message);

    /// <summary>
    /// Gets the system time.
    /// </summary>
    /// <value>The system time.</value>
    ISysTime _sysTime { get; } = sysTime;

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Log(string message)
        => LogAction($"{_sysTime.Now.ToString(CultureInfo.InvariantCulture)}: Msg: {message}");
    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Log(string message, Exception exception)
        => LogAction($"{_sysTime.Now.ToString(CultureInfo.InvariantCulture)}: Err: {message}, {exception}");
}