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
using Avalonia.Controls;
using Avalonia;
using Avalonia.Platform.Storage;
using AA20_SysDialogs.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace AA20_SysDialogs.Views;

/// <summary>
/// Interaktionslogik für SysDialogs.xaml
/// </summary>
public partial class SysDialogs : UserControl {
    /// <summary>
    /// Initializes a new instance of the <see cref="SysDialogs"/> class.
    /// </summary>
    public SysDialogs() {
			InitializeComponent();
			if (Design.IsDesignMode) return;
			
			var vm = App.Services.GetRequiredService<SysDialogsViewModel>();
			DataContext = vm;
			
			// Wire up dialog handlers
			vm.FileOpenDialog = DoFileOpenDialog;
			vm.FileSaveAsDialog = DoFileSaveDialog;
			vm.DirectoryBrowseDialog = DoFolderPickerDialog;
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
    private async Task<bool?> DoPrintDialog(Action? OnPrint)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null) return null;

        var messageBox = MessageBoxManager
            .GetMessageBoxStandard("Print", 
                "PDF-Druck wird vorbereitet...", 
                ButtonEnum.Ok);
        
        await messageBox.ShowWindowDialogAsync(window!); // Non-null assertion
        
        // TODO: PDF generieren und öffnen
        OnPrint?.Invoke();
        return true;
    }

    /// <summary>
    /// Does the font dialog.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <param name="par">The par.</param>
    /// <param name="OnAccept">The on accept.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private Task<bool?> DoFontDialog(Font font, Action<Font>? OnAccept)
    {
        // TODO: Implement Avalonia font dialog
        return Task.FromResult<bool?>(null);
    }

    /// <summary>
    /// Does the color dialog.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="par">The par.</param>
    /// <param name="OnAccept">The on accept.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private async Task<bool?> DoColorDialog(Color color, Action<Color>? OnAccept)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null) return null;

        // Einfacher Dialog mit Farbauswahl über Sliders
        var messageBox = MessageBoxManager
            .GetMessageBoxStandard("Farbauswahl", 
                $"Aktuelle Farbe: RGB({color.R}, {color.G}, {color.B})\n\nFarbauswahl-Dialog noch nicht implementiert.", 
                ButtonEnum.Ok);
        
        await messageBox.ShowWindowDialogAsync(window!);
        
        // TODO: Implement proper color picker
        return false;
    }

    /// <summary>
    /// Does the browse dialog.
    /// </summary>
    /// <param name="PathName">Name of the path.</param>
    /// <param name="Par">The par.</param>
    /// <param name="OnAccept">The on accept.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private async Task<bool?> DoFolderPickerDialog(string PathName, Action<string>? OnAccept)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null) return null;

        var folders = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            var path = folders[0].Path.LocalPath;
            OnAccept?.Invoke(path);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Does the file save dialog.
    /// </summary>
    /// <param name="Filename">The filename.</param>
    /// <param name="OnAccept">The on accept.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private async Task<bool?> DoFileSaveDialog(string Filename, Action<string>? OnAccept)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null) return null;

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save File",
            SuggestedFileName = Filename
        });

        if (file != null)
        {
            OnAccept?.Invoke(file.Path.LocalPath);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Does the file open dialog.
    /// </summary>
    /// <param name="Filename">The filename.</param>
    /// <param name="OnAccept">The on accept.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    private async Task<bool?> DoFileOpenDialog(string Filename, Action<string>? OnAccept)
    {
        var window = TopLevel.GetTopLevel(this) as Window;
        if (window == null) return null;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = false
        });

        if (files.Count > 0)
        {
            OnAccept?.Invoke(files[0].Path.LocalPath);
            return true;
        }
        return false;
    }
}
