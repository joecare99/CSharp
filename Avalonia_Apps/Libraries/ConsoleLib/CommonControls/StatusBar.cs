using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Displays a short application status message.</summary>
public sealed class StatusBar : Control
{
    public string Status
    {
        get => Text;
        set => SetText(value ?? string.Empty);
    }

    public ConsoleColor StatusColor { get; set; } = ConsoleColor.Gray;

    public override void Draw()
    {
        if (WidgetSet is IFormWidgetRenderer renderer)
            renderer.DrawStatusBar(this);
        else
            base.Draw();
    }
}
