namespace Avln_ImageEditor.Controls.Models;

/// <summary>
/// Defines the tools exposed by the initial image editing surface.
/// </summary>
public enum ImageEditorTool
{
    /// <summary>
    /// Selects and inspects image content without modifying it.
    /// </summary>
    Select,

    /// <summary>
    /// Represents a brush-style painting tool for future editing operations.
    /// </summary>
    Brush,

    /// <summary>
    /// Represents an eraser-style editing tool for future editing operations.
    /// </summary>
    Eraser,

    /// <summary>
    /// Represents a fill-style editing tool for future editing operations.
    /// </summary>
    Fill,

    /// <summary>
    /// Represents a crop operation for future editing operations.
    /// </summary>
    Crop
}
