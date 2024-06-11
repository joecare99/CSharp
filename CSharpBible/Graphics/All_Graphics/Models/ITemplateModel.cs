﻿// ***********************************************************************
// Assembly         : All_Graphics
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="ITemplateModel.cs" company="JC-Soft">
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
namespace All_Graphics.Models
{
    /// <summary>
    /// Interface ITemplateModel
    /// </summary>
    /// <autogeneratedoc />
    public interface ITemplateModel
    {
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        /// <autogeneratedoc />
        DateTime Now { get; }
        List<ExItem> Examples { get; }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        /// <autogeneratedoc />
        event PropertyChangedEventHandler PropertyChanged;
    }
}