// ***********************************************************************
// Assembly         : CommonDialogs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="ColorDialog.cs" company="CommonDialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace CommonDialogs.Interfaces
{
    public interface IColorDialog
    {
        bool AllowFullOpen { get; set; }
        bool AnyColor { get; set; }
        Color Color { get; set; }
        int[] CustomColors { get; set; }
        bool FullOpen { get; set; }
        bool ShowHelp { get; set; }
        bool SolidColorOnly { get; set; }
        object? Tag { get; set; }

        void Reset();
        bool? ShowDialog();
        bool? ShowDialog(object owner);
    }
}