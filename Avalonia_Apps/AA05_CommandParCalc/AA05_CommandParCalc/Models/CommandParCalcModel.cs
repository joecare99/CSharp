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
using AA05_CommandParCalc.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

/// <summary>
/// The Models namespace.
/// </summary>
namespace AA05_CommandParCalc.Models;

/// <summary>
/// Class CommandParCalcModel.
/// </summary>
public partial class CommandParCalcModel : ObservableObject, ICommandParCalcModel
{
    private readonly ISysTime _sysTime;
    private readonly ICyclTimer _timer;

    public DateTime Now => _sysTime.Now;
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandParCalcModel"/> class.
    /// </summary>
    public CommandParCalcModel(ISysTime sysTime, ICyclTimer timer)
    {
        _sysTime = sysTime;
        _timer = timer;
        _timer.Interval = 1000;
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }
}
