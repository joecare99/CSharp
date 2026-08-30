using ConsoleLib;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.Showcase.Views;

/// <summary>Renders an animated glyph frame using the native ConsoleLib canvas.</summary>
public sealed class EffectPanel : Control
{
    private string _frame = string.Empty;

    public ConsoleColor EffectForeground { get; set; } = ConsoleColor.Cyan;

    public void SetFrame(string frame)
    {
        _frame = frame ?? string.Empty;
        Invalidate();
    }

    public override void Draw()
    {
        var canvas = ConsoleFramework.Canvas;
        var dimension = RealDim;
        canvas.FillRect(dimension, EffectForeground, ConsoleColor.Black, ' ');
        if (dimension.Width > 0 && dimension.Height > 0)
        {
            var text = _frame.Length > dimension.Width ? _frame[..dimension.Width] : _frame;
            canvas.OutTextXY(dimension.X, dimension.Y, text, EffectForeground, ConsoleColor.Black);
        }

        Valid = true;
    }
}
