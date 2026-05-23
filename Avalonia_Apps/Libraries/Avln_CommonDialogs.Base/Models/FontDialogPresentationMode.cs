namespace Avln_CommonDialogs.Base.Models;

/// <summary>
/// Defines how the font dialog should be presented.
/// </summary>
public enum FontDialogPresentationMode
{
    /// <summary>
    /// Uses the default presentation chosen by the implementation.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Presents the font picker in a separate dialog window.
    /// </summary>
    Window = 1,

    /// <summary>
    /// Presents the font picker as an in-place overlay when supported.
    /// </summary>
    Overlay = 2
}
