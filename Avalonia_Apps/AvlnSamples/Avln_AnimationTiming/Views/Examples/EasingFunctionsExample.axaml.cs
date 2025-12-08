// ***********************************************************************
// Assembly : Avln_AnimationTiming
// Author    : Mir
// Created      : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="EasingFunctionsExample.axaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using System.Collections.Generic;

namespace Avln_AnimationTiming.Views.Examples;

/// <summary>
/// Interaction logic for EasingFunctionsExample.axaml
/// </summary>
public partial class EasingFunctionsExample : UserControl
{
    private readonly Dictionary<string, Easing> _easings = new();
    private const double AnimationDistance = 700;
 private const double AnimationDuration = 2.0;

    public EasingFunctionsExample()
 {
    InitializeComponent();
        SetupEasings();
        
     var animateButton = this.FindControl<Button>("animateButton");
        var resetButton = this.FindControl<Button>("resetButton");
        
   if (animateButton != null)
      animateButton.Click += AnimateButton_Click;
     if (resetButton != null)
    resetButton.Click += ResetButton_Click;
  }

    private void SetupEasings()
{
   _easings["linearBall"] = new LinearEasing();
   _easings["quadInBall"] = new QuadraticEaseIn();
        _easings["quadOutBall"] = new QuadraticEaseOut();
   _easings["quadInOutBall"] = new QuadraticEaseInOut();
      _easings["cubicBall"] = new CubicEaseInOut();
        _easings["expoBall"] = new ExponentialEaseOut();
     _easings["bounceBall"] = new BounceEaseOut();
        _easings["elasticBall"] = new ElasticEaseOut();
   _easings["backBall"] = new BackEaseInOut();
    }

    private async void AnimateButton_Click(object? sender, RoutedEventArgs e)
    {
     foreach (var kvp in _easings)
        {
       var ellipse = this.FindControl<Ellipse>(kvp.Key);
  if (ellipse != null)
     {
     var animation = new Animation
    {
       Duration = TimeSpan.FromSeconds(AnimationDuration),
    Easing = kvp.Value,
       Children =
        {
  new KeyFrame
   {
      Cue = new Cue(0.0),
     Setters = { new Setter(MarginProperty, new Thickness(5, 0, 0, 0)) }
 },
     new KeyFrame
       {
     Cue = new Cue(1.0),
       Setters = { new Setter(MarginProperty, new Thickness(AnimationDistance, 0, 0, 0)) }
     }
     }
      };

   await animation.RunAsync(ellipse);
   }
   }
    }

private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
     foreach (var kvp in _easings)
     {
     var ellipse = this.FindControl<Ellipse>(kvp.Key);
    if (ellipse != null)
     {
  ellipse.Margin = new Thickness(5, 0, 0, 0);
         }
   }
 }
}
