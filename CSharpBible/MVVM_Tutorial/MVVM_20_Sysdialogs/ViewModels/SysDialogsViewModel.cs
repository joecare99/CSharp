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
using CommonDialogs;
using CommonDialogs.Interfaces;
using Microsoft.Win32;
using MVVM.ViewModel;

namespace MVVM_20_Sysdialogs.ViewModels
{

    /// <summary>
    /// Class SysDialogsViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class SysDialogsViewModel : BaseViewModel {
		#region Fields
		/// <summary>
		/// The file open name
		/// </summary>
		private string _fileOpenName="<Open>";
		/// <summary>
		/// The file save name
		/// </summary>
		private string _fileSaveName = "<Save>";
		/// <summary>
		/// The path name
		/// </summary>
		private string _pathName = "<Path>";
		/// <summary>
		/// The ext
		/// </summary>
		private string _Ext = ".txt";
		/// <summary>
		/// My color
		/// </summary>
		private Color _myColor=Color.White;
		/// <summary>
		/// My font
		/// </summary>
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
		public delegate bool? FileDialogHandler(string Filename, IFileDialog Par, Action<string, IFileDialog>? OnAccept=null);
		/// <summary>
		/// Delegate ColorDialogHandler
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="par">The par.</param>
		/// <param name="OnAccept">The on accept.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public delegate bool? ColorDialogHandler(Color color, IColorDialog par, Action<Color, IColorDialog>? OnAccept=null);
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
		/// Gets or sets the name of the file open.
		/// </summary>
		/// <value>The name of the file open.</value>
		public string FileOpenName { get=> _fileOpenName; set=>SetProperty(ref _fileOpenName, value); }
		/// <summary>
		/// Gets or sets the name of the file save.
		/// </summary>
		/// <value>The name of the file save.</value>
		public string FileSaveName { get => _fileSaveName; set => SetProperty(ref _fileSaveName, value); }
		/// <summary>
		/// Gets or sets the name of the path.
		/// </summary>
		/// <value>The name of the path.</value>
		public string PathName { get => _pathName; set => SetProperty(ref _pathName, value); }
		/// <summary>
		/// Gets or sets the ext.
		/// </summary>
		/// <value>The ext.</value>
		public string Ext { get => _Ext; set => SetProperty(ref _Ext, value); }
		/// <summary>
		/// Gets or sets my color.
		/// </summary>
		/// <value>My color.</value>
		public Color MyColor { get => _myColor; set => SetProperty(ref _myColor, value); }
		/// <summary>
		/// Gets or sets my font.
		/// </summary>
		/// <value>My font.</value>
		public Font MyFont { get => _myFont; set => SetProperty(ref _myFont, value); }

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

		/// <summary>
		/// Gets or sets the open file open dialog command.
		/// </summary>
		/// <value>The open file open dialog command.</value>
		public DelegateCommand OpenFileOpenDialogCommand{ get; set; }
		/// <summary>
		/// Gets or sets the open directory browse dialog command.
		/// </summary>
		/// <value>The open directory browse dialog command.</value>
		public DelegateCommand OpenDirectoryBrowseDialogCommand { get; set; }
		/// <summary>
		/// Gets or sets the open file save as dialog command.
		/// </summary>
		/// <value>The open file save as dialog command.</value>
		public DelegateCommand OpenFileSaveAsDialogCommand { get; set; }
		/// <summary>
		/// Gets or sets the open color dialog command.
		/// </summary>
		/// <value>The open color dialog command.</value>
		public DelegateCommand OpenColorDialogCommand { get; set; }
		/// <summary>
		/// Gets or sets the open font dialog command.
		/// </summary>
		/// <value>The open font dialog command.</value>
		public DelegateCommand OpenFontDialogCommand { get; set; }
		/// <summary>
		/// Gets or sets the open print dialog command.
		/// </summary>
		/// <value>The open print dialog command.</value>
		public DelegateCommand OpenPrintDialogCommand { get; set; }


		//		public DelegateCommand OpenDirectoryBrowseDialogCommand { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="SysDialogsViewModel"/> class.
		/// </summary>
		public SysDialogsViewModel() {
			OpenFileOpenDialogCommand = new DelegateCommand((o) => {
                IFileDialog foPar = new FileDialogProxy<OpenFileDialog>(new() 
				{ 
                    FileName = FileOpenName
                });
                FileOpenDialog?.Invoke(FileOpenName,foPar , 
					(s,p) => { 
						FileOpenName = s; });
			});

			OpenFileSaveAsDialogCommand = new DelegateCommand((o) => {
                IFileDialog fsPar = new FileDialogProxy<SaveFileDialog>(new() 
                {
                    FileName = FileSaveName
                });
                FileSaveAsDialog?.Invoke(FileSaveName, fsPar, (s,p) => { FileSaveName = s; });
			});

			OpenDirectoryBrowseDialogCommand = new DelegateCommand((o) => {
                IFileDialog bdPar = new FolderBrowserDialog()
                {
                    FileName = PathName
                };
                DirectoryBrowseDialog?.Invoke(PathName, bdPar, (s, p) => { PathName = s; });
			});

			OpenColorDialogCommand = new DelegateCommand((o) => {
                IColorDialog cdPar = new ColorDialog
                {
                    Color = MyColor
                };
                dColorDialog?.Invoke(MyColor, cdPar, (c, p) => { MyColor = c; });
			});

			OpenFontDialogCommand = new DelegateCommand((o) => {
                IFontDialog fdPar = new FontDialog
                {
                    Font = MyFont
                };
                dFontDialog?.Invoke(MyFont, fdPar, (f, p) => { MyFont = f; });
			});

			OpenPrintDialogCommand = new DelegateCommand((o) => {
                IPrintDialog dialog = new PrintDialog()
                {
                    PageRangeSelection = System.Windows.Controls.PageRangeSelection.AllPages
                };
                dPrintDialog?.Invoke(dialog, (p) => {/* ? */ });
			});
		}
		#endregion
	}
}
