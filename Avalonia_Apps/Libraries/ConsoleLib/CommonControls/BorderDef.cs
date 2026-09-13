using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using System;

namespace ConsoleLib.CommonControls;

public sealed class BorderDef : IBorderDefinition
{
    public BorderStyle Style { get; set; } = BorderStyle.None;

    public ConsoleColor BorderColor { get; set; }

    public char[]? CustomChars { get; set; }
}
