﻿// ***********************************************************************
// Assembly         : WPF_MasterDetail
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="IMasterDetailModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace WPF_MasterDetail.Models
{
    /// <summary>
    /// Interface IMasterDetailModel
    /// </summary>
    /// <autogeneratedoc />
    public interface IMasterDetailModel
    {
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        /// <autogeneratedoc />
        DateTime Now { get; }
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        /// <autogeneratedoc />
        event PropertyChangedEventHandler PropertyChanged;
    }
}
