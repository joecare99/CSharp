using ConsoleLib.Data;
using System;

namespace ConsoleLib.Interfaces;

public interface IBorderDefinition
{
    BorderStyle Style { get; set; }
    ConsoleColor BorderColor { get; set; }
    char[]? CustomChars { get; set; }
}