namespace Treppen.Export.Models;

public sealed class PrintOptions
{
    public int Dpi { get; init; } = 300;
    public int MarginLeft { get; init; } = 10;
    public int MarginTop { get; init; } = 10;
    public int MarginRight { get; init; } = 10;
    public int MarginBottom { get; init; } = 10;

    public int CellSize { get; init; } = 8;
    public string? Title { get; init; }
    public string ForegroundColor { get; init; } = "#000000";
    public string BackgroundColor { get; init; } = "#FFFFFF";
}
