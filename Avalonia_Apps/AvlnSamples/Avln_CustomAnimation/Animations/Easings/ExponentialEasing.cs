// ***********************************************************************
// Assembly  : Avln_CustomAnimation
// Author         : Mir
// Created     : 01-15-2025
// ***********************************************************************
// <copyright file="ExponentialEasing.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025 (Migrated from WPF Microsoft Sample)
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Animation.Easings;
using System;

namespace Avln_CustomAnimation.Animations.Easings;

/// <summary>
/// Custom Exponential Easing - gets exponentially faster/slower
/// Migrated from WPF's ExponentialDoubleAnimation
/// </summary>
public class ExponentialEasing : Easing
{
  /// <summary>
    /// Exponential rate of growth
    /// </summary>
    public double Power { get; set; } = 2.0;

    /// <summary>
    /// Edge behavior
    /// </summary>
    public EdgeBehavior EdgeBehavior { get; set; } = EdgeBehavior.EaseIn;

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

    private double EaseIn(double timeFraction)
    {
   // Math magic: simple exponential growth
        return Math.Pow(timeFraction, Power);
  }

    private double EaseOut(double timeFraction)
    {
  // Math magic: simple exponential decay
        return Math.Pow(timeFraction, 1 / Power);
    }

    private double EaseInOut(double timeFraction)
    {
      // We cut each effect in half
     if (timeFraction <= 0.5)
        {
  return EaseOut(timeFraction * 2) * 0.5;
        }
        return EaseIn((timeFraction - 0.5) * 2) * 0.5 + 0.5;
    }
}
