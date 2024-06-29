// ***********************************************************************
// Assembly         : MVVM_20_Sysdialogs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-11-2022
// ***********************************************************************
// <copyright file="SysDialogsViewModel.cs" company="MVVM_20_Sysdialogs">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommonDialogs;
using CommonDialogs.Interfaces;
using Microsoft.Win32;
using MVVM.ViewModel;
using CommunityToolkit.Mvvm.Input;

namespace MVVM_20_Sysdialogs.ViewModels
{

    /// <summary>
    /// Class SysDialogsViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class SysDialogsViewModel : BaseViewModelCT
    {
        #region Fields
        /// <summary>
        /// The file open name
        /// </summary>
        [ObservableProperty]
        private string _fileOpenName = "<Open>";
        /// <summary>
        /// The file save name
        /// </summary>
        [ObservableProperty]
        private string _fileSaveName = "<Save>";
        /// <summary>
        /// The path name
        /// </summary>
        [ObservableProperty]
        private string _pathName = "<Path>";
        /// <summary>
        /// The ext
        /// </summary>
        [ObservableProperty]
        private string _Ext = ".txt";
        /// <summary>
        /// My color
        /// </summary>
        [ObservableProperty]
        private Color _myColor = Color.White;
        /// <summary>
        /// My font
        /// </summary>
        [ObservableProperty]
        private Font _myFont = new Font("Microsoft Sans Serif", 8.25f);
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate FileDialogHandler
        /// </summary>
        /// <param name="Filename">The filename.</param>
        /// <param name="Par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool? FileDialogHandler(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept = null);
        /// <summary>
        /// Delegate ColorDialogHandler
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool? ColorDialogHandler(Color color, IColorDialog par, Action<Color, IColorDialog>? OnAccept = null);
        /// <summary>
        /// Delegate FontDialogHandler
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="par">The par.</param>
        /// <param name="OnAccept">The on accept.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool? FontDialogHandler(Font font, IFontDialog par, Action<Font, IFontDialog>? OnAccept = null);
        /// <summary>
        /// Delegate PrintDialogHandler
        /// </summary>
        /// <param name="par">The par.</param>
        /// <param name="OnPrint">The on print.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool? PrintDialogHandler(IPrintDialog par, Action<IPrintDialog>? OnPrint = null);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the file open dialog.
        /// </summary>
        /// <value>The file open dialog.</value>
        public FileDialogHandler? FileOpenDialog { get; set; }
        /// <summary>
        /// Gets or sets the file save as dialog.
        /// </summary>
        /// <value>The file save as dialog.</value>
        public FileDialogHandler? FileSaveAsDialog { get; set; }
        /// <summary>
        /// Gets or sets the directory browse dialog.
        /// </summary>
        /// <value>The directory browse dialog.</value>
        public FileDialogHandler? DirectoryBrowseDialog { get; set; }
        /// <summary>
        /// Gets or sets the d color dialog.
        /// </summary>
        /// <value>The d color dialog.</value>
        public ColorDialogHandler? dColorDialog { get; set; }
        /// <summary>
        /// Gets or sets the d font dialog.
        /// </summary>
        /// <value>The d font dialog.</value>
        public FontDialogHandler? dFontDialog { get; set; }
        /// <summary>
        /// Gets or sets the d print dialog.
        /// </summary>
        /// <value>The d print dialog.</value>
        public PrintDialogHandler? dPrintDialog { get; set; }

        //		public DelegateCommand OpenDirectoryBrowseDialogCommand { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SysDialogsViewModel"/> class.
        /// </summary>
        public SysDialogsViewModel()
        {
        }

        [RelayCommand]
        private void OpenPrintDialog()
        {
            IPrintDialog dialog = new PrintDialog()
            {
                PageRangeSelection = System.Windows.Controls.PageRangeSelection.AllPages
            };
            dPrintDialog?.Invoke(dialog, (p) => {/* ? */ });
        }

        [RelayCommand]
        private void OpenFontDialog()
        {
            IFontDialog fdPar = new FontDialog
            {
                Font = MyFont
            };
            dFontDialog?.Invoke(MyFont, fdPar, (f, p) => { MyFont = f; });
        }

        [RelayCommand]
        private void OpenColorDialog()
        {
            IColorDialog cdPar = new ColorDialog
            {
                Color = MyColor
            };
            dColorDialog?.Invoke(MyColor, cdPar, (c, p) => { MyColor = c; });
        }

        [RelayCommand]
        private void OpenDirectoryBrowseDialog()
        {
            IFileDialog bdPar = new FolderBrowserDialog()
            {
                FileName = PathName
            };
            DirectoryBrowseDialog?.Invoke(PathName, bdPar, (s, p) => { PathName = s; });
        }

        [RelayCommand]
        private void OpenFileSaveAsDialog()
        {
            IFileDialog fsPar = new FileDialogProxy<SaveFileDialog>(new()
            {
                FileName = FileSaveName
            });
            FileSaveAsDialog?.Invoke(FileSaveName, fsPar, (s, p) => { FileSaveName = s; });
        }

        [RelayCommand]
        private void OpenFileOpenDialog()
        {
            IFileDialog foPar = new FileDialogProxy<OpenFileDialog>(new()
            {
                FileName = FileOpenName
            });
            FileOpenDialog?.Invoke(FileOpenName, foPar,
                (s, p) =>
                {
                    FileOpenName = s;
                });
        }
        #endregion
    }
}
