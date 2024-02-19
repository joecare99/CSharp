﻿// ***********************************************************************
// Assembly         : MVVM_39_MultiModelTest
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="parent.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using System;
using System.Collections.Generic;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_39_MultiModelTest.Models
{
    /// <summary>
    /// Class parent.
    /// Implements the <see cref="ObservableObject" />
    /// Implements the <see cref="MVVM_39_MultiModelTest.Models.ISystemModel" />
    /// </summary>
    /// <seealso cref="ObservableObject" />
    /// <seealso cref="MVVM_39_MultiModelTest.Models.ISystemModel" />
    /// <autogeneratedoc />
    public partial class SystemModel :ObservableObject, ISystemModel
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

        public List<IScopedModel> ScModels { get; } = new();
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemModel"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public SystemModel()
        {
            _timer = new(250d);
            _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
            _timer.Start();
        }

        public IScopedModel GetNewScopedModel()
        {
            var scope = IoC.GetNewScope();
            var model = scope.ServiceProvider.GetRequiredService<IScopedModel>();
            ScModels.Add(model);
            model.Scope = scope;
            model.parent = this; 
            return model;
        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        ~SystemModel()
        {
            _timer.Stop();
            return;
        }
#endif
        #endregion
    }
}
