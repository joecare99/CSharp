// ***********************************************************************
// Assembly    : Avln_AnimationTiming
// Author       : Mir
// Created      : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="RepeatBehaviorExample.axaml.cs" company="JC-Soft">
//   Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using System;

namespace Avln_AnimationTiming.Views.Examples;

/// <summary>
/// Interaction logic for RepeatBehaviorExample.axaml
/// </summary>
public partial class RepeatBehaviorExample : UserControl
{
    private Animation? _foreverAnimation;
    private Animation? _twiceAnimation;
    private Animation? _onceAnimation;
    private Animation? _alternateAnimation;

    public RepeatBehaviorExample()
    {
        InitializeComponent();
        SetupAnimations();

        var startButton = this.FindControl<Button>("startButton");
        var stopButton = this.FindControl<Button>("stopButton");

        if (startButton != null)
            startButton.Click += StartButton_Click;
        if (stopButton != null)
            stopButton.Click += StopButton_Click;
    }

    private void SetupAnimations()
    {
        // Animation that repeats forever
        _foreverAnimation = new Animation
        {
            Duration = TimeSpan.FromSeconds(2),
            IterationCount = IterationCount.Infinite,
            Children =
      {
      new KeyFrame
      {
         Cue = new Cue(0.0),
      Setters = { new Setter(WidthProperty, 50.0) }
   },
 new KeyFrame
       {
       Cue = new Cue(1.0),
   Setters = { new Setter(WidthProperty, 300.0) }
     }
   }
        };

        // Animation that repeats twice
        _twiceAnimation = new Animation
        {
            Duration = TimeSpan.FromSeconds(2),
            IterationCount = new IterationCount(2),
            Children =
   {
      new KeyFrame
         {
           Cue = new Cue(0.0),
       Setters = { new Setter(WidthProperty, 50.0) }
   },
      new KeyFrame
         {
       Cue = new Cue(1.0),
        Setters = { new Setter(WidthProperty, 300.0) }
     }
  }
        };

        // Animation that runs once
        _onceAnimation = new Animation
        {
            Duration = TimeSpan.FromSeconds(2),
            IterationCount = new IterationCount(1),
            Children =
      {
         new KeyFrame
       {
       Cue = new Cue(0.0),
 Setters = { new Setter(WidthProperty, 50.0) }
         },
       new KeyFrame
     {
       Cue = new Cue(1.0),
             Setters = { new Setter(WidthProperty, 300.0) }
     }
   }
        };

        // Animation with Alternate playback direction
        _alternateAnimation = new Animation
        {
            Duration = TimeSpan.FromSeconds(2),
            IterationCount = new IterationCount(4),
            PlaybackDirection = PlaybackDirection.Alternate,
            Children =
            {
       new KeyFrame
       {
       Cue = new Cue(0.0),
     Setters = { new Setter(WidthProperty, 50.0) }
         },
   new KeyFrame
     {
     Cue = new Cue(1.0),
              Setters = { new Setter(WidthProperty, 300.0) }
          }
    }
        };
    }

    private async void StartButton_Click(object? sender, RoutedEventArgs e)
    {
        var foreverRect = this.FindControl<Rectangle>("foreverRepeatingRectangle");
        var twiceRect = this.FindControl<Rectangle>("twiceRepeatingRectangle");
        var onceRect = this.FindControl<Rectangle>("onceRepeatingRectangle");
        var alternateRect = this.FindControl<Rectangle>("alternateRepeatingRectangle");

        // Looping animations use Run() instead of RunAsync()
        if (_foreverAnimation != null && foreverRect != null)
            _foreverAnimation.RunAsync(foreverRect);

        if (_twiceAnimation != null && twiceRect != null)
            _twiceAnimation.RunAsync(twiceRect);

        // Single iteration animation can use await
        if (_onceAnimation != null && onceRect != null)
            await _onceAnimation.RunAsync(onceRect);

        // Multiple iterations use Run()
        if (_alternateAnimation != null && alternateRect != null)
            _alternateAnimation.RunAsync(alternateRect);
    }

    private void StopButton_Click(object? sender, RoutedEventArgs e)
    {
        // Reset all rectangles to initial width
        var foreverRect = this.FindControl<Rectangle>("foreverRepeatingRectangle");
        var twiceRect = this.FindControl<Rectangle>("twiceRepeatingRectangle");
        var onceRect = this.FindControl<Rectangle>("onceRepeatingRectangle");
        var alternateRect = this.FindControl<Rectangle>("alternateRepeatingRectangle");

        if (foreverRect != null) foreverRect.Width = 50;
        if (twiceRect != null) twiceRect.Width = 50;
        if (onceRect != null) onceRect.Width = 50;
        if (alternateRect != null) alternateRect.Width = 50;
    }
}
