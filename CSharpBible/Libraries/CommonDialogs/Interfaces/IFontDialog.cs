// ***********************************************************************
// Assembly         : CommonDialogs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="FontDialog.cs" company="CommonDialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace CommonDialogs.Interfaces
{
    public interface IFontDialog
    {
        System.Drawing.Font Font { get; set; }
        bool? ShowDialog();
        bool? ShowDialog(object owner);
    }
}