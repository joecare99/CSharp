// ***********************************************************************
// Assembly         : AA05_CommandParCalc
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="CommandParCalcModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Platform;
using AA05_CommandParCalc.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
namespace AA05_CommandParCalc.Models;

/// <summary>
/// Class CommandParCalcModel.
/// </summary>
public partial class CommandParCalcModel : ObservableObject, ICommandParCalcModel
{
    /// <summary>
    /// The platform handle
    /// </summary>
    private IPlatformHandle _platformHandle;
    private ISysTime _sysTime;
    private ICyclTimer _timer;

    public DateTime Now => _sysTime.Now;
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandParCalcModel"/> class.
    /// </summary>
    /// <param name="platformHandle">The platform handle.</param>
    public  CommandParCalcModel(IPlatformHandle platformHandle, ISysTime sysTime, ICyclTimer timer)
    {
        _platformHandle = platformHandle;
        _sysTime = sysTime;
        _timer = timer;
        _timer.Interval = 1000;
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }
}
