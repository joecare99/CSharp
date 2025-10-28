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
using CommunityToolkit.Mvvm.Input;

namespace AA20_SysDialogs.ViewModels;

public partial class SysDialogsViewModel : ObservableObject
{
    #region Fields
    [ObservableProperty]
    private string _fileOpenName = "<Open>";

    [ObservableProperty]
    private string _fileSaveName = "<Save>";

    [ObservableProperty]
    private string _pathName = "<Path>";

    [ObservableProperty]
    private string _ext = ".txt";

    [ObservableProperty]
    private Color _myColor = Color.White;

#pragma warning disable CA1416 // Platform compatibility - Font is Windows-specific
    [ObservableProperty]
    private Font _myFont = new Font("Arial", 12f);
#pragma warning restore CA1416
    #endregion

    #region Delegates
    public delegate System.Threading.Tasks.Task<bool?> FileDialogHandler(string Filename, Action<string>? OnAccept = null);
    public delegate System.Threading.Tasks.Task<bool?> ColorDialogHandler(Color color, Action<Color>? OnAccept = null);
    public delegate System.Threading.Tasks.Task<bool?> FontDialogHandler(Font font, Action<Font>? OnAccept = null);
    public delegate System.Threading.Tasks.Task<bool?> PrintDialogHandler(Action? OnPrint = null);
    #endregion

    #region Properties
    public FileDialogHandler? FileOpenDialog { get; set; }
    public FileDialogHandler? FileSaveAsDialog { get; set; }
    public FileDialogHandler? DirectoryBrowseDialog { get; set; }
    public ColorDialogHandler? dColorDialog { get; set; }
    public FontDialogHandler? dFontDialog { get; set; }
    public PrintDialogHandler? dPrintDialog { get; set; }
    #endregion

    #region Methods
    public SysDialogsViewModel()
    {
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task OpenPrintDialog()
    {
        if (dPrintDialog != null)
            await dPrintDialog.Invoke(() => { });
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task OpenFontDialog()
    {
        if (dFontDialog != null)
            await dFontDialog.Invoke(MyFont, (f) => { MyFont = f; });
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task OpenColorDialog()
    {
        if (dColorDialog != null)
            await dColorDialog.Invoke(MyColor, (c) => { MyColor = c; });
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task OpenDirectoryBrowseDialog()
    {
        if (DirectoryBrowseDialog != null)
            await DirectoryBrowseDialog.Invoke(PathName, (s) => { PathName = s; });
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task OpenFileSaveAsDialog()
    {
        if (FileSaveAsDialog != null)
            await FileSaveAsDialog.Invoke(FileSaveName, (s) => { FileSaveName = s; });
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task OpenFileOpenDialog()
    {
        if (FileOpenDialog != null)
            await FileOpenDialog.Invoke(FileOpenName, (s) => { FileOpenName = s; });
    }
    #endregion
}
