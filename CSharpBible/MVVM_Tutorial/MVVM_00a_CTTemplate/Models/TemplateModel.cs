﻿// ***********************************************************************
// Assembly         : MVVM_00a_CTTemplate
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
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_00a_CTTemplate.Models;

/// <summary>
/// Class TemplateModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="MVVM_00a_CTTemplate.Models.ITemplateModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="MVVM_00a_CTTemplate.Models.ITemplateModel" />
/// <autogeneratedoc />
public partial class TemplateModel :ObservableObject, ITemplateModel
{
    #region Properties
    /// <summary>
    /// The timer
    /// </summary>
    /// <autogeneratedoc />
    private readonly Timer _timer;
    /// <summary>
    /// Gets or sets the get now.
    /// </summary>
    /// <value>The get now.</value>
    /// <autogeneratedoc />
    public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    /// <autogeneratedoc />
    public DateTime Now { get => GetNow(); }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public TemplateModel()
    {
        _timer = new(250d);
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }

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
