// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avln_Geometry.ViewModels.Interfaces;

namespace Avln_Geometry.Views;

public partial class SampleViewer : UserControl
{
    public SampleViewer(ISampleViewerViewModel data)
    {
        InitializeComponent();
        DataContext = data;
        
        // Add all example views
        data.AddView(new GeometryUsageExample());
        data.AddView(new ShapeGeometriesExample());
        data.AddView(new PathGeometryExample());
        data.AddView(new GeometryAttributeSyntaxExample());
        data.AddView(new CombiningGeometriesExample());
        
        // Set first example as default
        data.ZoomOutCompleteCommand.Execute(null);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Example1RadioButton.IsChecked = true;
    }
}
