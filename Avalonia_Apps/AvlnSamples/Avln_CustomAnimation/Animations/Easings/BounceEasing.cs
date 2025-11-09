// ***********************************************************************
// Assembly    : Avln_CustomAnimation
// Author        : Mir
// Created          : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="BounceEasing.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025 (Migrated from WPF Microsoft Sample)
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Animation.Easings;
using System;

namespace Avln_CustomAnimation.Animations.Easings;

/// <summary>
/// Custom Bounce Easing with configurable bounces and bounciness
/// Migrated from WPF's BounceDoubleAnimation
/// </summary>
public class BounceEasing : Easing
{
    /// <summary>
    /// Number of bounces in the effect
    /// </summary>
    public int Bounces { get; set; } = 5;

    /// <summary>
    /// Specifies the amount by which the element springs back
    /// </summary>
    public double Bounciness { get; set; } = 3.0;

    /// <summary>
    /// Edge behavior: EaseIn, EaseOut, or EaseInOut
    /// </summary>
    public EdgeBehavior EdgeBehavior { get; set; } = EdgeBehavior.EaseOut;

    public override double Ease(double progress)
    {
        return EdgeBehavior switch
     {
            EdgeBehavior.EaseIn => EaseIn(progress),
        EdgeBehavior.EaseOut => EaseOut(progress),
        EdgeBehavior.EaseInOut => EaseInOut(progress),
     _ => progress
    };
    }

    private double EaseOut(double timeFraction)
    {
        // Math magic: The cosine gives us the right wave, the timeFraction is the frequency of the wave,
   // the absolute value keeps every value positive (so it "bounces" off the midpoint of the cosine
     // wave, and the amplitude (the exponent) makes the sine wave get smaller and smaller at the end.
        var returnValue = Math.Abs(Math.Pow((1 - timeFraction), Bounciness)
         * Math.Cos(2 * Math.PI * timeFraction * Bounces));
  return 1 - returnValue;
    }

    private double EaseIn(double timeFraction)
    {
        // Math magic: The cosine gives us the right wave, the timeFraction is the amplitude of the wave,
    // the absolute value keeps every value positive (so it "bounces" off the midpoint of the cosine
        // wave, and the amplitude (the exponent) makes the sine wave get bigger and bigger towards the end.
        return Math.Abs(Math.Pow(timeFraction, Bounciness)
        * Math.Cos(2 * Math.PI * timeFraction * Bounces));
    }

    private double EaseInOut(double timeFraction)
    {
        // We cut each effect in half by multiplying the time fraction by two and halving the distance.
        if (timeFraction <= 0.5)
        {
            return EaseIn(timeFraction * 2) * 0.5;
        }
        else
        {
       return EaseOut((timeFraction - 0.5) * 2) * 0.5 + 0.5;
        }
    }
}

/// <summary>
/// Edge behavior enumeration
/// </summary>
public enum EdgeBehavior
{
    EaseIn,
 EaseOut,
    EaseInOut
}
