// ***********************************************************************
// Assembly         : AA06_Converters_4
// Author        : Mir
// Created      : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="PlotFrame.axaml.cs" company="JC-Soft">
//   (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA06_Converters_4.View.Converter;
using AA06_Converters_4.ViewModels;
using Avalonia;
using Avalonia.Controls;

namespace AA06_Converters_4.View;

/// <summary>
/// Interaktionslogik für PlotFrame.axaml
/// </summary>
public partial class PlotFrame : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotFrame"/> class.
    /// </summary>
    public PlotFrame(PlotFrameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
      
        DataContextChanged += (s,e)=>DataContextChange(s,e);
        
        PropertyChanged += OnPropertyChanged;
    }

    protected void DataContextChange(object? sender, EventArgs args)
    {
      if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
      {
      pc.WindowSize = new Size(Bounds.Width, Bounds.Height);
        }
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
      if (e.Property == BoundsProperty)
        {
        if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
            {
    pc.WindowSize = new Size(Bounds.Width, Bounds.Height);
    if (DataContext is PlotFrameViewModel vm)
    vm.WindowSize = new Size(Bounds.Width, Bounds.Height);
  }
        }
}
}
