namespace Avln_CommonDialogs.Base.Models;

/// <summary>
/// Represents a UI-agnostic font dialog selection.
/// </summary>
public sealed class FontDialogSelection
{
    /// <summary>
    /// Gets or sets the font family name.
    /// </summary>
    public string? FamilyName { get; set; }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double Size { get; set; } = 12d;

    /// <summary>
    /// Gets or sets a value indicating whether bold weight is selected.
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether italic style is selected.
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether underline decoration is selected.
    /// </summary>
    public bool IsUnderline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether strikethrough decoration is selected.
    /// </summary>
    public bool IsStrikethrough { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether overline decoration is selected.
    /// </summary>
    public bool IsOverline { get; set; }

    /// <summary>
    /// Gets or sets the ARGB color value.
    /// </summary>
    public uint ArgbColor { get; set; } = 0xFF000000;

    /// <summary>
    /// Creates a shallow copy of the selection.
    /// </summary>
    /// <returns>A copied selection instance.</returns>
    public FontDialogSelection Clone()
        => new()
        {
            FamilyName = FamilyName,
            Size = Size,
            IsBold = IsBold,
            IsItalic = IsItalic,
            IsUnderline = IsUnderline,
            IsStrikethrough = IsStrikethrough,
            IsOverline = IsOverline,
            ArgbColor = ArgbColor
        };

    /// <summary>
    /// Returns a readable representation of the selection.
    /// </summary>
    /// <returns>A summary string.</returns>
    public override string ToString()
    {
        var effects = new List<string>();

        if (IsBold)
            effects.Add(nameof(IsBold)[2..]);

        if (IsItalic)
            effects.Add(nameof(IsItalic)[2..]);

        if (IsUnderline)
            effects.Add(nameof(IsUnderline)[2..]);

        if (IsStrikethrough)
            effects.Add(nameof(IsStrikethrough)[2..]);

        if (IsOverline)
            effects.Add(nameof(IsOverline)[2..]);

        var effectText = effects.Count > 0 ? string.Join(", ", effects) : "Regular";
        return $"{FamilyName ?? "<Default>"}, {Size:0.##} pt, {effectText}, #{ArgbColor:X8}";
    }
}
