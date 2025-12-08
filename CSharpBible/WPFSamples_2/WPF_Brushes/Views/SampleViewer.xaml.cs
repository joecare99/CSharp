// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Brushes.ViewModels.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Xml;

namespace Brushes.Views;

/// <summary>
///     Interaction logic for SampleViewer.xaml
/// </summary>
public partial class SampleViewer : Page
{
    public SampleViewer(ISampleViewerViewModel data)
    {
        InitializeComponent();
        DataContext = data;
        data.DoExit += DoExitCommand;
    }

    private void MyFrameNavigated(object sender, NavigationEventArgs args)
    {
        var myFadeInAnimation = (DoubleAnimation) Resources["MyFadeInAnimationResource"];
        myFrame.BeginAnimation(OpacityProperty, myFadeInAnimation, HandoffBehavior.SnapshotAndReplace);
    }

    private void DoExitCommand(object sender, EventArgs e) => Application.Current.Shutdown();

    private void DoubleAnimation_Completed(object sender, EventArgs e)
        => ((ISampleViewerViewModel)DataContext).TransitionCommand?.Execute(e);
}