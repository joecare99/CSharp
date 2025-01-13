// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="AppModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Models namespace.
/// </summary>
namespace Avalonia_App02.Models;

/// <summary>
/// Class AppModel.
/// </summary>
public class AppModel
{
    /// <summary>
    /// The platform handle
    /// </summary>
    private IPlatformHandle _platformHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppModel"/> class.
    /// </summary>
    /// <param name="platformHandle">The platform handle.</param>
    public AppModel(IPlatformHandle platformHandle)
    {
        _platformHandle = platformHandle;      
    }
}
