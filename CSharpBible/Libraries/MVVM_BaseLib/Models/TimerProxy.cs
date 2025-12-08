// ***********************************************************************
// Assembly         : Avln_BaseLib
// Author           : Mir
// Created          : 01-13-2025
//
// Last Modified By : Mir
// Last Modified On : 01-13-2025
// ***********************************************************************
// <copyright file="TimerProxy.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************

using BaseLib.Models.Interfaces;

/// <summary>
/// The Models namespace.
/// </summary>
namespace BaseLib.Models;

/// <summary>
/// Class TimerProxy.
/// Implements the <see cref="System.Timers.Timer" />
/// Implements the <see cref="ICyclTimer" />
/// </summary>
/// <seealso cref="System.Timers.Timer" />
/// <seealso cref="ICyclTimer" />
public class TimerProxy : System.Timers.Timer, ICyclTimer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimerProxy"/> class.
    /// </summary>
    public TimerProxy() : base()
    {
        Interval = 1000;
    }
}
