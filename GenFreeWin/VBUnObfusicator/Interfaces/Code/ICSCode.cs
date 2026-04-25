// ***********************************************************************
// Assembly         : VBUnObfusicator
// Author           : Mir
// Created          : 09-26-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="ICSCode.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

/// <summary>
/// The Models namespace.
/// </summary>
namespace VBUnObfusicator.Interfaces.Code
{
    /// <summary>
    /// Interface ICSCode
    /// </summary>
    public partial interface ICSCode : ICodeBase
    {
        bool DoWhile { get; set; }

        /// <summary>
        /// Removes the single source labels1.
        /// </summary>
        /// <param name="cStruct">The c structure.</param>
        void RemoveSingleSourceLabels1(ICodeBlock cStruct);
        /// <summary>
        /// Reorders the labels.
        /// </summary>
        /// <param name="cStruct">The c structure.</param>
        void ReorderLabels(ICodeBlock cStruct);
    }
}