using System;
using System.IO;
using Microsoft.Win32;

namespace TileSetAnimator.Services;

/// <summary>
/// Wraps the shell dialogs for testability.
/// </summary>
public sealed class FileDialogService : IFileDialogService
{
    private const string ImageFilter = "PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Alle Dateien (*.*)|*.*";
    private const string CsFilter = "C#-Dateien (*.cs)|*.cs|Alle Dateien (*.*)|*.*";

    /// <inheritdoc />
    public string? OpenTileSheet()
    {
        var dialog = new OpenFileDialog
        {
            Filter = ImageFilter,
            CheckFileExists = true,
            Title = Properties.Resources.LoadSheetButtonText,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc />
    public string? SaveTile(string suggestedFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = ImageFilter,
            FileName = Path.ChangeExtension(suggestedFileName, ".png"),
            Title = Properties.Resources.SaveTileDialogTitle,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc />
    public string? SaveTileEnum(string suggestedFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = CsFilter,
            FileName = Path.ChangeExtension(string.IsNullOrWhiteSpace(suggestedFileName) ? "Tiles" : suggestedFileName, ".cs"),
            Title = Properties.Resources.ExportTileEnumDialogTitle,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc />
    public string? OpenTileEnum()
    {
        var dialog = new OpenFileDialog
        {
            Filter = CsFilter,
            CheckFileExists = true,
            Title = Properties.Resources.ImportTileEnumDialogTitle,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
