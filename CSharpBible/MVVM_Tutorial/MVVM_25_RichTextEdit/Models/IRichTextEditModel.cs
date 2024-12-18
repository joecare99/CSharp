﻿// ***********************************************************************
// Assembly         : MVVM_25_RichTextEdit
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="IRichTextEditModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.IO;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_25_RichTextEdit.Models
{
    /// <summary>
    /// Interface IRichTextEditModel
    /// </summary>
    /// <autogeneratedoc />
    public interface IRichTextEditModel
    {
        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
        /// <autogeneratedoc />
        DateTime Now { get; }
        string EmptyText { get; }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        /// <autogeneratedoc />
        event PropertyChangedEventHandler PropertyChanged;

        string DocumentFromStream(FileStream fs);
        void DocumentToStream(FileStream fs, string document);
    }
}
