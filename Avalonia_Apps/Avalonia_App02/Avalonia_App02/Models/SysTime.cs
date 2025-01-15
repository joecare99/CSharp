// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-13-2025
//
// Last Modified By : Mir
// Last Modified On : 01-13-2025
// ***********************************************************************
// <copyright file="SysTime.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia_App02.Models.Interfaces;
using System;

/// <summary>
/// The Models namespace.
/// </summary>
namespace Avalonia_App02.Models;

/// <summary>
/// Class SysTime.
/// Implements the <see cref="ISysTime" />
/// </summary>
/// <seealso cref="ISysTime" />
public class SysTime : ISysTime
{
    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets the UTC now.
    /// </summary>
    /// <value>The UTC now.</value>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Gets the today.
    /// </summary>
    /// <value>The today.</value>
    public DateTime Today => DateTime.Today;
}
