// ***********************************************************************
// Assembly         : MVVM_17_1_CSV_Laden
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 08-13-2022
// ***********************************************************************
// <copyright file="DataPointsViewModel.cs" company="MVVM_17_1_CSV_Laden">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace MVVM_17_1_CSV_Laden.ViewModel
{
    /// <summary>
    /// Struct SWindowPort
    /// </summary>
    public struct SWindowPort
    {
        /// <summary>
        /// The port
        /// </summary>
        public RectangleF port;
        /// <summary>
        /// The window size
        /// </summary>
        public System.Windows.Size WindowSize;
        /// <summary>
        /// The parent
        /// </summary>
        public DataPointsViewModel Parent;
    }
}
