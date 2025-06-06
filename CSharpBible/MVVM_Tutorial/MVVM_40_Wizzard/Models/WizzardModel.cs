﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="WizzardModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM_40_Wizzard.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.Models;

/// <summary>
/// Class WizzardModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="MVVM_40_Wizzard.Models.IWizzardModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="MVVM_40_Wizzard.Models.IWizzardModel" />
/// <autogeneratedoc />
public partial class WizzardModel :ObservableObject, IWizzardModel
{
    private const string csAppStartet = "WizzardModel created";
    private const string csAppStopped = "WizzardModel stopped";
    #region Properties
    /// <summary>
    /// The timer
    /// </summary>
    /// <autogeneratedoc />
    private readonly Timer _timer;
    private readonly ILog _log;
    private readonly ISysTime _sysTime;
    public DateTime Now
        => _sysTime.Now;

    [ObservableProperty]
    private int _mainSelection = -1;

    [ObservableProperty]
    private IList<int> _mainOptions = [0, 1, 3, 4, 6, 8, 9, 10];

    [ObservableProperty]
    private int _subSelection = -1;

    [ObservableProperty]
    private IList<int> _subOptions = [ 1,2, 3, 5, 6, 7, 9, 11];

    [ObservableProperty]
    private int _additional1 = -1;
    [ObservableProperty]
    private int _additional2 = -1;
    [ObservableProperty]
    private int _additional3 = -1;

    [ObservableProperty]
    private IList<int> _additOptions = [10, 12, 13, 15, 16, 17, 19, 21];

    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="WizzardModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public WizzardModel(ISysTime sysTime, ILog log)
    {
        _sysTime = sysTime;
        _log = log;
        _log.Log(csAppStartet);
        _timer = new(250d);
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }

    partial void OnMainSelectionChanged(int value)
    {
        Additional1 = -1;
        Additional2 = -1;
        Additional3 = -1;
    }

    partial void OnSubSelectionChanged(int value)
    {
        Additional1 = -1;
        Additional2 = -1;
        Additional3 = -1;
    }

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    ~WizzardModel()
    {
        _timer.Stop();
        _log.Log(csAppStopped);
        return;
    }
#endif
    #endregion
}
