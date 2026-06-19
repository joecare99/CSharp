using System.Collections.Generic;

namespace CommonDialogs.Models;

public sealed class FontDialogSelection
{
    public string? FamilyName { get; set; }

    public double Size { get; set; } = 12d;

    public bool IsBold { get; set; }

    public bool IsItalic { get; set; }

    public bool IsUnderline { get; set; }

    public bool IsStrikethrough { get; set; }

    public DialogColor Color { get; set; } = new(0xFF000000, "Black");

    public FontDialogSelection Clone()
        => new()
        {
            FamilyName = FamilyName,
            Size = Size,
            IsBold = IsBold,
            IsItalic = IsItalic,
            IsUnderline = IsUnderline,
            IsStrikethrough = IsStrikethrough,
            Color = Color
        };

    public override string ToString()
    {
        var effects = new List<string>();

        if (IsBold)
            effects.Add(nameof(IsBold).Substring(2));

        if (IsItalic)
            effects.Add(nameof(IsItalic).Substring(2));

        if (IsUnderline)
            effects.Add(nameof(IsUnderline).Substring(2));

        if (IsStrikethrough)
            effects.Add(nameof(IsStrikethrough).Substring(2));

        var effectText = effects.Count > 0 ? string.Join(", ", effects) : "Regular";
        return $"{FamilyName ?? "<Default>"}, {Size:0.##} pt, {effectText}, {Color}";
    }
}