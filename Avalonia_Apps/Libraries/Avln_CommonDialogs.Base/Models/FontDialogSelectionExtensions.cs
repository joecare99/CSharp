namespace Avln_CommonDialogs.Base.Models;

/// <summary>
/// Provides helper methods for font dialog selections.
/// </summary>
public static class FontDialogSelectionExtensions
{
    /// <summary>
    /// Converts an object to a <see cref="FontDialogSelection"/> when possible.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted selection or <see langword="null"/>.</returns>
    public static FontDialogSelection? AsFontDialogSelection(this object? value)
        => value as FontDialogSelection;
}
