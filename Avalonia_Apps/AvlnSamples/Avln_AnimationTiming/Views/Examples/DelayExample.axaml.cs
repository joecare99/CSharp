// ***********************************************************************
// Assembly    : Avln_AnimationTiming
// Author       : Mir
// Created      : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="DelayExample.axaml.cs" company="JC-Soft">
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
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace Avln_AnimationTiming.Views.Examples;

/// <summary>
/// Interaction logic for DelayExample.axaml
/// </summary>
public partial class DelayExample : UserControl
{
    public DelayExample()
    {
InitializeComponent();
        
 var startButton = this.FindControl<Button>("startButton");
     var resetButton = this.FindControl<Button>("resetButton");
        
   if (startButton != null)
     startButton.Click += StartButton_Click;
   if (resetButton != null)
       resetButton.Click += ResetButton_Click;
    }

    private async void StartButton_Click(object? sender, RoutedEventArgs e)
  {
        var statusText = this.FindControl<TextBlock>("statusText");
   if (statusText != null)
       statusText.Text = "Animations running...";

   await Task.WhenAll(
  AnimateRectangle("noDelayRect", TimeSpan.Zero),
      AnimateRectangle("delay05Rect", TimeSpan.FromSeconds(0.5)),
AnimateRectangle("delay1Rect", TimeSpan.FromSeconds(1)),
     AnimateRectangle("delay15Rect", TimeSpan.FromSeconds(1.5)),
   AnimateRectangle("delay2Rect", TimeSpan.FromSeconds(2))
   );

        if (statusText != null)
   statusText.Text = "All animations completed!";
 }

    private async Task AnimateRectangle(string name, TimeSpan delay)
    {
   var rect = this.FindControl<Rectangle>(name);
   if (rect == null) return;

        var animation = new Animation
        {
       Duration = TimeSpan.FromSeconds(1.5),
     Delay = delay,
      Easing = new CubicEaseInOut(),
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
     Setters = { new Setter(WidthProperty, 400.0) }
    }
     }
};

        await animation.RunAsync(rect);
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
     var noDelayRect = this.FindControl<Rectangle>("noDelayRect");
  var delay05Rect = this.FindControl<Rectangle>("delay05Rect");
   var delay1Rect = this.FindControl<Rectangle>("delay1Rect");
  var delay15Rect = this.FindControl<Rectangle>("delay15Rect");
 var delay2Rect = this.FindControl<Rectangle>("delay2Rect");
  var statusText = this.FindControl<TextBlock>("statusText");

        if (noDelayRect != null) noDelayRect.Width = 50;
      if (delay05Rect != null) delay05Rect.Width = 50;
        if (delay1Rect != null) delay1Rect.Width = 50;
      if (delay15Rect != null) delay15Rect.Width = 50;
        if (delay2Rect != null) delay2Rect.Width = 50;
   if (statusText != null) statusText.Text = "";
    }
}
