using System;

namespace ConsoleLib.CommonControls;

/// <summary>Represents one selectable tile item.</summary>
public sealed class TileItem
{
    public TileItem(string text, object? value = null)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Value = value;
    }

    public string Text { get; set; }
    public object? Value { get; }
}
