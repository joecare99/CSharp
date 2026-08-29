using System;

namespace ConsoleLib.Interfaces;

/// <summary>Represents one terminal cell in a render frame.</summary>
public readonly struct TerminalCell : IEquatable<TerminalCell>
{
    public TerminalCell(char character, ConsoleColor foreground, ConsoleColor background)
    {
        Character = character;
        Foreground = foreground;
        Background = background;
    }

    public char Character { get; }
    public ConsoleColor Foreground { get; }
    public ConsoleColor Background { get; }

    public bool Equals(TerminalCell other) =>
        Character == other.Character && Foreground == other.Foreground && Background == other.Background;

    public override bool Equals(object? obj) => obj is TerminalCell other && Equals(other);
    public override int GetHashCode() => (Character, Foreground, Background).GetHashCode();
}
