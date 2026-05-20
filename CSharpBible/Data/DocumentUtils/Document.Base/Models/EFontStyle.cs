namespace Document.Base.Models;

/// <summary>
/// Represents a compact legacy font-style enumeration.
/// </summary>
/// <remarks>
/// The values combine common formatting attributes into a single integer-based style code for
/// compatibility with older document APIs.
/// </remarks>
public enum EFontStyle
{
    /// <summary>No explicit styling.</summary>
    Default = 0,

    /// <summary>Bold text.</summary>
    Bold = 1,

    /// <summary>Italic text.</summary>
    Italic = 2,

    /// <summary>Bold and italic text.</summary>
    BoldItalic = 3,

    /// <summary>Underlined text.</summary>
    Underline = 4,

    /// <summary>Underlined, bold, and italic text.</summary>
    UnderlineBoldItalic = 7,

    /// <summary>Underlined and bold text.</summary>
    UnderlineBold = 5,

    /// <summary>Underlined and italic text.</summary>
    UnderlineItalic = 6,

    /// <summary>Struck-out text.</summary>
    Strikeout = 8
}
