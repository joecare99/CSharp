// ***********************************************************************
// Assembly   : Avln_Hello_World
// Author : Mir
// Created   : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="MainWindow.axaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Controls;
using Avln_Hello_World.ViewModels;
using System.ComponentModel;

namespace Avln_Hello_World.Views;

/// <summary>
/// Interaction logic for MainWindow.axaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
  InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
  if (DataContext is MainWindowViewModel vm)
        {
      vm.Closing();
        }
        base.OnClosing(e);
    }
}
