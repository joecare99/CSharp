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
using MVVM_20_Sysdialogs.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using CommonDialogs.Interfaces;
using System.Drawing;

namespace MVVM_20_Sysdialogs.Views
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
        private bool? DoPrintDialog(IPrintDialog par, Action<IPrintDialog>? OnPrint)
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
        private bool? DoFontDialog(Font font, IFontDialog par, Action<Font, IFontDialog>? OnAccept)
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
        private bool? DoColorDialog(Color color, IColorDialog par, Action<Color, IColorDialog>? OnAccept)
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
        private bool? DoBrowseDialog(string PathName, IFileDialog Par, Action<string, IFileDialog>? OnAccept)
        {
            Par.FileName = PathName;
            bool? result = Par.ShowDialog();
            if (result ?? false) OnAccept?.Invoke(Par.FileName, Par);
            return result;
        }

        /// <summary>
        /// Does the file dialog.
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool? DoFileDialog(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept)
        {
            Par.FileName = Filename;
            bool? result = Par.ShowDialog(this.Parent as Window);
            if (result ?? false) OnAccept?.Invoke(Par.FileName, Par);
            return result;
        }
    }
}
