// ***********************************************************************
// Assembly         : CSV_Viewer
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 02-29-2020
// ***********************************************************************
// <copyright file="Program.cs" company="JC Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;
using CSharpBible.CSV_Viewer.Visual;

namespace CSharpBible.CSV_Viewer
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
            Application.Run(new FrmCSVViewerMain());
        }
    }
}
