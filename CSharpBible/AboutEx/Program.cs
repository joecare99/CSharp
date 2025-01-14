// ***********************************************************************
// Assembly         : AboutEx
// Author           : Mir
// Created          : 11-11-2022
//
// Last Modified By : Mir
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="Program.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;
using CSharpBible.AboutEx.Visual;

/// <summary>
/// The AboutEx namespace.
/// </summary>
namespace CSharpBible.AboutEx
{
    /// <summary>
    /// Class Program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmAboutExMain());
        }
    }
}
