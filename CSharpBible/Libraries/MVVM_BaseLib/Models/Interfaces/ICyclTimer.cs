// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="SomeTemplateModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Timers;

namespace BaseLib.Models.Interfaces;

/// <summary>
/// Interface ICyclTimer
/// </summary>
public interface ICyclTimer
{
    /// <summary>
    /// Gets or sets the interval.
    /// </summary>
    /// <value>The interval.</value>
    double Interval { get; set; }
    /// <summary>
    /// Gets a value indicating whether this <see cref="ICyclTimer"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    bool Enabled { get; }
    /// <summary>
    /// Gets or sets a value indicating whether [automatic reset].
    /// </summary>
    /// <value><c>true</c> if [automatic reset]; otherwise, <c>false</c>.</value>
    bool AutoReset { get; set; }
    /// <summary>
    /// Occurs when [elapsed].
    /// </summary>
    event ElapsedEventHandler Elapsed;
    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start();
    /// <summary>
    /// Stops this instance.
    /// </summary>
    void Stop();

}