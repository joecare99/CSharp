using System;
using ConsoleLib.Interfaces;

namespace ConsoleLib.CommonControls;

/// <summary>Displays determinate progress as a normalized value.</summary>
public sealed class ProgressBar : Control
{
    private double _value;

    public double Minimum { get; set; }
    public double Maximum { get; set; } = 100;

    public double Value
    {
        get => _value;
        set
        {
            if (Maximum < Minimum)
                throw new InvalidOperationException("Maximum must be greater than or equal to Minimum.");
            var clamped = Math.Max(Minimum, Math.Min(Maximum, value));
            if (Math.Abs(_value - clamped) < double.Epsilon)
                return;
            _value = clamped;
            Invalidate();
        }
    }

    public double Fraction => Maximum <= Minimum ? 0 : (Value - Minimum) / (Maximum - Minimum);

    public override void Draw()
    {
        if (WidgetSet is IFormWidgetRenderer renderer)
            renderer.DrawProgressBar(this);
        else
            base.Draw();
    }
}
