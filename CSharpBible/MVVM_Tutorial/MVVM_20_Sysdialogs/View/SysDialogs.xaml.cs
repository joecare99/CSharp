// ***********************************************************************
// Assembly         : MVVM_20_Sysdialogs
// Author           : Mir
// Created          : 08-09-2022
//
// Last Modified By : Mir
// Last Modified On : 08-10-2022
// ***********************************************************************
// <copyright file="SysDialogs.xaml.cs" company="MVVM_20_Sysdialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_20_Sysdialogs.ViewModel;
using System;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using CommonDialogs;
using System.Drawing;

namespace MVVM_20_Sysdialogs.View
{
    /// <summary>
    /// Interaktionslogik für SysDialogs.xaml
    /// </summary>
    public partial class SysDialogs : Page {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysDialogs"/> class.
        /// </summary>
        public SysDialogs() {
			InitializeComponent();
		}

        /// <summary>
        /// Handles the Loaded event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (SysDialogsViewModel)DataContext;
            vm.FileOpenDialog = DoFileDialog;
            vm.FileSaveAsDialog = DoFileDialog;
            vm.DirectoryBrowseDialog = DoBrowseDialog;
            vm.dColorDialog = DoColorDialog;
            vm.dFontDialog = DoFontDialog;
            vm.dPrintDialog = DoPrintDialog;
        }

        /// <summary>
        /// Does the print dialog.
        /// </summary>
        /// <param name="par">The par.</param>
        /// <param name="OnPrint">The on print.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoPrintDialog(ref PrintDialog par, Action<PrintDialog>? OnPrint)
        {
            bool? result = par.ShowDialog();
            if (result ?? false) OnPrint?.Invoke(par);
            return result;
        }

        /// <summary>
        /// Does the font dialog.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoFontDialog(Font font, ref FontDialog par, Action<Font, FontDialog>? OnAccept)
        {
            par.Font = font;
            bool? result = par.ShowDialog();
            if (result ?? false) OnAccept?.Invoke(par.Font, par);
            return result;
        }

        /// <summary>
        /// Does the color dialog.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoColorDialog(Color color, ref ColorDialog par, Action<Color, ColorDialog>? OnAccept)
        {
            par.Color = color;
            bool? result = par.ShowDialog();
            if (result ?? false) OnAccept?.Invoke(par.Color , par);
            return result;
        }

        /// <summary>
        /// Does the browse dialog.
        /// </summary>
        /// <param name="PathName">Name of the path.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoBrowseDialog(string PathName, ref FileDialog Par, Action<string, FileDialog>? OnAccept)
        {
            var fbd = new FolderBrowserDialog();               
            fbd.SelectedPath = PathName;
            bool? result = fbd.ShowDialog();
            if (result ?? false) OnAccept?.Invoke(fbd.SelectedPath, Par);
            return result;
        }

        /// <summary>
        /// Does the file dialog.
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoFileDialog(string Filename, ref FileDialog Par, Action<string, FileDialog>? OnAccept)
        {
            Par.FileName = Filename;
            bool? result = Par.ShowDialog(this.Parent as Window);
            if (result ?? false) OnAccept?.Invoke(Par.FileName, Par);
            return result;
        }
    }
}
