﻿// ***********************************************************************
// Assembly         : MVVM_00_IoCTemplate
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
using BaseLib.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_00_IoCTemplate.Models
{
    /// <summary>
    /// Class TemplateModel.
    /// Implements the <see cref="ObservableObject" />
    /// Implements the <see cref="MVVM_00_IoCTemplate.Models.ITemplateModel" />
    /// </summary>
    /// <seealso cref="ObservableObject" />
    /// <seealso cref="MVVM_00_IoCTemplate.Models.ITemplateModel" />
    /// <autogeneratedoc />
    public partial class TemplateModel :ObservableObject, ITemplateModel
    {
        private const string csApplStart = "Application startet";
#if !NET5_0_OR_GREATER
        private const string csApplEnded = "Application ended";
#endif
        #region Properties
        /// <summary>
        /// The timer
        /// </summary>
        /// <autogeneratedoc />
        private readonly Timer _timer;
        private readonly ISysTime _systime;
        private readonly ILog _log;
     
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        /// <autogeneratedoc />
        public DateTime Now { get => _systime.Now; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateModel"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public TemplateModel(ISysTime sysTime,ILog log)
        {
            _systime = sysTime;
            _log = log;
            _log.Log(csApplStart);
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
            _log.Log(csApplEnded);
            return;
        }
#endif
        #endregion
    }
}
