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
    /// Paints with the currently sampled color.
    /// </summary>
    Brush,

    /// <summary>
    /// Erases pixels to transparency.
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
