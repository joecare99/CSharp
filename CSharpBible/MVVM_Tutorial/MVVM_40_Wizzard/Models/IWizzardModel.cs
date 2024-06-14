﻿// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="IWizzardModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_40_Wizzard.Models
{
    /// <summary>
    /// Interface IWizzardModel
    /// </summary>
    /// <autogeneratedoc />
    public interface IWizzardModel
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

        int MainSelection { get; set; }

        IList<int> MainOptions { get; set; }

        int SubSelection { get; set; }
        IList<int> SubOptions { get; set; }

        int Additional1 { get; set; }
        int Additional2 { get; set; }
        int Additional3 { get; set; }

    }
}
