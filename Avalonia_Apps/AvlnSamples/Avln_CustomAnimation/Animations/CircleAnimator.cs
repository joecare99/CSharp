// ***********************************************************************
// Assembly         : Avln_CustomAnimation
// Author  : Mir
// Created        : 01-15-2025
// ***********************************************************************
// <copyright file="CircleAnimator.cs" company="JC-Soft">
// Copyright © JC-Soft 2025 (Migrated from WPF Microsoft Sample)
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Styling;
using System;
using System.Threading.Tasks;

namespace Avln_CustomAnimation.Animations;

/// <summary>
/// Animates an element in a circular/elliptical path
/// Migrated from WPF's CircleAnimation
/// </summary>
public class CircleAnimator
{
    /// <summary>
    /// Animates an element in a circle
    /// </summary>
    /// <param name="element">Element to animate</param>
    /// <param name="radiusX">Horizontal radius</param>
    /// <param name="radiusY">Vertical radius</param>
    /// <param name="duration">Animation duration</param>
    /// <param name="iterationCount">Number of iterations</param>
    public static async Task AnimateInCircle(
        Control element,
        double radiusX,
        double radiusY,
        TimeSpan duration,
        IterationCount? iterationCount = null)
    {
        var centerX = Canvas.GetLeft(element) + element.Bounds.Width / 2;
        var centerY = Canvas.GetTop(element) + element.Bounds.Height / 2;

        var xAnimation = CreateCircleAnimation(
     Canvas.LeftProperty,
        radiusX,
    centerX,
         duration,
        iterationCount,
      isXDirection: true);

        var yAnimation = CreateCircleAnimation(
                Canvas.TopProperty,
                  radiusY,
           centerY,
       duration,
              iterationCount,
         isXDirection: false);

        // Run both animations in parallel
        await Task.WhenAll(
 xAnimation.RunAsync(element),
    yAnimation.RunAsync(element)
  );
    }

    private static Animation CreateCircleAnimation(
          AvaloniaProperty property,
    double radius,
          double center,
     TimeSpan duration,
    IterationCount? iterationCount,
          bool isXDirection)
    {
        var keyframes = new System.Collections.Generic.List<KeyFrame>();

        // Create keyframes for smooth circular motion
        for (int i = 0; i <= 360; i += 10)
        {
            var angle = i * Math.PI / 180.0;
            var value = isXDirection
                   ? Math.Cos(angle) * radius + center
        : Math.Sin(angle) * radius + center;

            keyframes.Add(new KeyFrame
            {
                Cue = new Cue(i / 360.0),
                Setters = { new Setter(property, value) }
            });
        }

        // Füge nach dem Erstellen des Animation-Objekts die Keyframes hinzu:
        var animation = new Animation
        {
            Duration = duration,
            IterationCount = iterationCount ?? new IterationCount(1)
        };
        animation.Children.AddRange(keyframes);
        return animation;
    }
}
