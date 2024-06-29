﻿// ***********************************************************************
// Assembly         : WPF_CustomAnimation
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="ICustomAnimationModel.cs" company="JC-Soft">
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
namespace WPF_CustomAnimation.Models
{
    /// <summary>
    /// Interface ICustomAnimationModel
    /// </summary>
    /// <autogeneratedoc />
    public interface ICustomAnimationModel
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
