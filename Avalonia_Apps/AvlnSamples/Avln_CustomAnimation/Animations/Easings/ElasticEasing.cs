// ***********************************************************************
// Assembly     : Avln_CustomAnimation
// Author           : Mir
// Created          : 01-15-2025
// ***********************************************************************
// <copyright file="ElasticEasing.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025 (Migrated from WPF Microsoft Sample)
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Animation.Easings;
using System;

namespace Avln_CustomAnimation.Animations.Easings;

/// <summary>
/// Custom Elastic Easing - like something attached to a rubber band
/// Migrated from WPF's ElasticDoubleAnimation
/// </summary>
public class ElasticEasing : Easing
{
    /// <summary>
    /// How much springiness is there in the effect
    /// </summary>
    public double Springiness { get; set; } = 3.0;

    /// <summary>
    /// Number of oscillations in the effect
    /// </summary>
    public double Oscillations { get; set; } = 10.0;

    /// <summary>
    /// Edge behavior
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
        // Math magic: The cosine gives us the right wave, the timeFraction * the # of oscillations is the
        // frequency of the wave, and the amplitude (the exponent) makes the wave get smaller at the end
     // by the "springiness" factor. This is extremely similar to the bounce equation.
    var returnValue = Math.Pow((1 - timeFraction), Springiness)
       * Math.Cos(2 * Math.PI * timeFraction * Oscillations);
     return 1 - returnValue;
    }

    private double EaseIn(double timeFraction)
    {
 // Math magic: The cosine gives us the right wave, the timeFraction * the # of oscillations is the
  // frequency of the wave, and the amplitude (the exponent) makes the wave get smaller at the beginning
   // by the "springiness" factor.
        return Math.Pow(timeFraction, Springiness)
   * Math.Cos(2 * Math.PI * timeFraction * Oscillations);
    }

  private double EaseInOut(double timeFraction)
    {
        // We cut each effect in half
        if (timeFraction <= 0.5)
   {
return EaseIn(timeFraction * 2) * 0.5;
        }
        return EaseOut((timeFraction - 0.5) * 2) * 0.5 + 0.5;
    }
}
