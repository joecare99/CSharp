using System;
using System.Globalization;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Conservative Unicode width service for terminal layout.</summary>
public sealed class UnicodeTextLayoutService : ITextLayoutService
{
    public int GetCellWidth(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        var width = 0;
        foreach (var character in text)
            width += GetCellWidth(character);
        return width;
    }

    public int GetCellWidth(char character)
    {
        if (character == '\r' || character == '\n' || char.IsControl(character))
            return 0;

        var category = CharUnicodeInfo.GetUnicodeCategory(character);
        if (category == UnicodeCategory.NonSpacingMark ||
            category == UnicodeCategory.SpacingCombiningMark ||
            category == UnicodeCategory.EnclosingMark)
            return 0;

        return IsWide(character) ? 2 : 1;
    }

    private static bool IsWide(char character) =>
        character >= '\u1100' &&
        (character <= '\u115f' ||
         character == '\u2329' || character == '\u232a' ||
         (character >= '\u2e80' && character <= '\ua4cf') ||
         (character >= '\xac00' && character <= '\ud7a3') ||
         (character >= '\uf900' && character <= '\ufaff') ||
         (character >= '\ufe10' && character <= '\ufe19') ||
         (character >= '\ufe30' && character <= '\ufe6f') ||
         (character >= '\uff00' && character <= '\uff60') ||
         (character >= '\uffe0' && character <= '\uffe6'));
}
