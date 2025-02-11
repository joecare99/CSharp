// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Geometry.ViewModels.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Geometry.Views;

public partial class SampleViewer : Page
{
    private const string cZoomInStoryboard = "ZoomInStoryboard";

    public SampleViewer(ISampleViewerViewModel data)
    {
        InitializeComponent();
        DataContext = data;
        data.AddView(new GeometryUsageExample());
        data.AddView(new ShapeGeometriesExample());
        data.AddView(new PathGeometryExample());
        data.AddView(new CombiningGeometriesExample());
        data.AddView(new GeometryAttributeSyntaxExample());
        data.ZoomOut += SampleSelected;
    }

    private void PageLoaded(object sender, RoutedEventArgs args)
    {
        Example1RadioButton.IsChecked = true;
    }

    private void ZoomOutStoryboardCompleted(object sender, EventArgs args) 
        => ((ISampleViewerViewModel)DataContext).ZoomOutCompleteCommand.Execute(sender);

    private void FrameContentRendered(object sender, EventArgs args)
    {
        var s = (Storyboard) Resources[cZoomInStoryboard];
        s.Begin(this);
    }

    private void ZoomInStoryboardCompleted(object sender, EventArgs e) 
        => scrollViewerBorder.Visibility = Visibility.Visible;

    private void SampleSelected(object? sender,EventArgs args)
    {
        var points = new Point3DCollection();

        var ratio = myScrollViewer.ActualWidth/myScrollViewer.ActualHeight;

        points.Add(new Point3D(5, -5*ratio, 0));
        points.Add(new Point3D(5, 5*ratio, 0));
        points.Add(new Point3D(-5, 5*ratio, 0));

        points.Add(new Point3D(-5, 5*ratio, 0));
        points.Add(new Point3D(-5, -5*ratio, 0));
        points.Add(new Point3D(5, -5*ratio, 0));

        points.Add(new Point3D(-5, 5*ratio, 0));
        points.Add(new Point3D(-5, -5*ratio, 0));
        points.Add(new Point3D(5, -5*ratio, 0));

        points.Add(new Point3D(5, -5*ratio, 0));
        points.Add(new Point3D(5, 5*ratio, 0));
        points.Add(new Point3D(-5, 5*ratio, 0));

        myGeometry.Positions = points;
        myViewport3D.Width = 100;
        myViewport3D.Height = 100*ratio;

        scrollViewerBorder.Visibility = Visibility.Hidden;
    }
}