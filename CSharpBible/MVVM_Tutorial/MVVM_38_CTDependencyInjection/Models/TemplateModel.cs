﻿// ***********************************************************************
// Assembly         : MVVM_38_CTDependencyInjection
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="TemplateModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using BaseLib.Helper;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using System.Collections.Generic;


/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_38_CTDependencyInjection.Models;

/// <summary>
/// Class TemplateModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="ITemplateModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="ITemplateModel" />
/// <autogeneratedoc />
public partial class TemplateModel :ObservableObject, ITemplateModel
{
    #region Properties
    /// <summary>
    /// The timer
    /// </summary>
    /// <autogeneratedoc />
    private readonly ITimer _timer;
    private readonly ISysTime _sysTime;
    private readonly IUserRepository _userRep;

    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    /// <autogeneratedoc />
    public DateTime Now => _sysTime.Now;
    #endregion

    #region Methods
    public TemplateModel():this(
        IoC.GetRequiredService<ITimer>(),
        IoC.GetRequiredService<ISysTime>(),
        IoC.GetRequiredService<IUserRepository>())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public TemplateModel(ITimer timer,ISysTime sysTime, IUserRepository userRep)
    {
        _sysTime = sysTime;
        _userRep = userRep;
        _timer = timer;
        _timer.Interval = 250d;
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }

    public IEnumerable<string> GetUsers() =>
        _userRep.GetUsers();

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    ~TemplateModel()
    {
        _timer.Stop();
        return;
    }
#endif
    #endregion
}
