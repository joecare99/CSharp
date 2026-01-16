namespace TileSetAnimator.Services;

/// <summary>
/// Provides abstractions for interacting with the shell.
/// </summary>
public interface IFileDialogService
{
    /// <summary>
    /// Shows an open-file dialog for tile sheet images.
    /// </summary>
    /// <returns>The absolute file path or null when cancelled.</returns>
    string? OpenTileSheet();

    /// <summary>
    /// Shows a save-file dialog for exporting a single tile image.
    /// </summary>
    /// <param name="suggestedFileName">Pre-filled file name.</param>
    /// <returns>The file path or null when cancelled.</returns>
    string? SaveTile(string suggestedFileName);

    /// <summary>
    /// Shows a save-file dialog for exporting a cutout tile image (PNG with alpha).
    /// </summary>
    /// <param name="suggestedFileName">Pre-filled file name.</param>
    /// <returns>The file path or null when cancelled.</returns>
    string? SaveCutout(string suggestedFileName);

    /// <summary>
    /// Shows a save dialog for exporting C# enum definitions.
    /// </summary>
    /// <param name="suggestedFileName">Default file name without extension.</param>
    string? SaveTileEnum(string suggestedFileName);

    /// <summary>
    /// Shows an open dialog for importing C# enum definitions.
    /// </summary>
    string? OpenTileEnum();

    /// <summary>
    /// Shows a save dialog for exporting a complete tile set structure (grid, tiles, animations, mini maps).
    /// </summary>
    string? SaveTileSetStructure(string suggestedFileName);

    /// <summary>
    /// Shows an open dialog for importing a complete tile set structure.
    /// </summary>
    string? OpenTileSetStructure();
}
