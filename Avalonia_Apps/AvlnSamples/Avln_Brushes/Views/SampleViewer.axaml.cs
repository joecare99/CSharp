// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Avln_Brushes.ViewModels.Interfaces;
using System;
using Avalonia;
using Avalonia.Controls;

namespace Avln_Brushes.Views;

/// <summary>
/// Main window for Brush examples
/// </summary>
public partial class SampleViewer : Window
{
    public SampleViewer()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    // Constructor for DI
    public SampleViewer(ISampleViewerViewModel viewModel) : this()
    {
        DataContext = viewModel;
        if (viewModel != null)
        {
            viewModel.DoExit += DoExitCommand;
        }
    }

    private void DoExitCommand(object? sender, EventArgs e)
    {
        Close();
    }
}
