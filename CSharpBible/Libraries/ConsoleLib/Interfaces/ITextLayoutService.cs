namespace ConsoleLib.Interfaces;

/// <summary>Provides terminal-cell measurements for Unicode text.</summary>
public interface ITextLayoutService
{
    int GetCellWidth(string? text);
    int GetCellWidth(char character);
}
