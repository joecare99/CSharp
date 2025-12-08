// ***********************************************************************
// Assembly    : Avln_AnimationTiming
// Author      : Mir
// Created    : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="TransitionsExample.axaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avln_AnimationTiming.Views.Examples;

/// <summary>
/// Interaction logic for TransitionsExample.axaml
/// </summary>
public partial class TransitionsExample : UserControl
{
    private bool _isExpanded = false;

    public TransitionsExample()
    {
 InitializeComponent();
    }

    private void OnRectPressed(object? sender, PointerPressedEventArgs e)
    {
 if (sender is Rectangle rect)
      {
       // Toggle size
    _isExpanded = !_isExpanded;
      
   if (_isExpanded)
        {
     rect.Width = 250;
     rect.Height = 150;
    }
   else
     {
      rect.Width = 100;
 rect.Height = 100;
   }
        }
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        _isExpanded = false;
   
  var linearRect = this.FindControl<Rectangle>("linearRect");
 var elasticRect = this.FindControl<Rectangle>("elasticRect");
   var bounceRect = this.FindControl<Rectangle>("bounceRect");
     var cubicRect = this.FindControl<Rectangle>("cubicRect");
   var delayRect = this.FindControl<Rectangle>("delayRect");

        if (linearRect != null) { linearRect.Width = 100; linearRect.Height = 100; }
        if (elasticRect != null) { elasticRect.Width = 100; elasticRect.Height = 100; }
        if (bounceRect != null) { bounceRect.Width = 100; bounceRect.Height = 100; }
   if (cubicRect != null) { cubicRect.Width = 100; cubicRect.Height = 100; }
    if (delayRect != null) { delayRect.Width = 100; delayRect.Height = 100; }
    }
}
