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
    private const string PngFilter = "PNG (*.png)|*.png|Alle Dateien (*.*)|*.*";
    private const string CsFilter = "C#-Dateien (*.cs)|*.cs|Alle Dateien (*.*)|*.*";
    private const string TileSetFilter = "TileSet (*.tileset.json)|*.tileset.json|JSON (*.json)|*.json|Alle Dateien (*.*)|*.*";

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
    public string? SaveCutout(string suggestedFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = PngFilter,
            FileName = Path.ChangeExtension(suggestedFileName, ".png"),
            Title = "Export Cutout",
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

    /// <inheritdoc />
    public string? SaveTileSetStructure(string suggestedFileName)
    {
        var dialog = new SaveFileDialog
        {
            Filter = TileSetFilter,
            FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "tileset.tileset.json" : suggestedFileName,
            Title = "Export TileSet Structure",
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc />
    public string? OpenTileSetStructure()
    {
        var dialog = new OpenFileDialog
        {
            Filter = TileSetFilter,
            CheckFileExists = true,
            Title = "Import TileSet Structure",
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
