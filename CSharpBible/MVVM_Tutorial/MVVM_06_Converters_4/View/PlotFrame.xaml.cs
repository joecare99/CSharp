// ***********************************************************************
// Assembly         : MVVM_Lines_on_Grid
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="PlotFrame.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_06_Converters_4.View.Converter;
using MVVM_06_Converters_4.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_06_Converters_4.View;

/// <summary>
/// Interaktionslogik für PlotFrame.xaml
/// </summary>
public partial class PlotFrame : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotFrame"/> class.
    /// </summary>
    public PlotFrame()
    {
        InitializeComponent();
        
        DataContextChanged += (s,e)=>DataContextChange(s,e);
        
        SizeChanged += OnSizeChange;

    }

    protected  void DataContextChange(object? sender,object args)
    {
        if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
        {
            //                    pc.Row = vm.Row;
            //                  pc.Col = vm.Column;
            pc.WindowSize = new Size(Width, Height);
        }
    }

    public void OnSizeChange(object sender,SizeChangedEventArgs e)
    {
        if (this.Resources["vcPortGrid"] is WindowPortToGridLines pc)
        {
            pc.WindowSize = e.NewSize;
            if (DataContext is PlotFrameViewModel vm)
                vm.WindowSize = e.NewSize;
        }
    }
}
