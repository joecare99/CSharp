// ***********************************************************************
// Assembly         : Avln_BaseLib
// Author           : Mir
// Created          : 01-17-2025
//
// Last Modified By : Mir
// Last Modified On : 01-17-2025
// ***********************************************************************
// <copyright file="SysTime.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models.Interfaces;
using System;

/// <summary>
/// The Models namespace.
/// </summary>
namespace BaseLib.Models;

/// <summary>
/// Class SysTime.
/// Implements the <see cref="ISysTime" />
/// </summary>
/// <seealso cref="ISysTime" />
public class SysTime : ISysTime
{
    /// <summary>
    /// Gets or sets the get now.
    /// </summary>
    /// <value>The get now.</value>
    public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    public DateTime Now => GetNow();
    /// <summary>
    /// Gets the today.
    /// </summary>
    /// <value>The today.</value>
    public DateTime Today => GetNow().Date;

}
