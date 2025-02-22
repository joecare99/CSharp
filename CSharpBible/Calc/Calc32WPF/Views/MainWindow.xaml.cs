﻿// ***********************************************************************
// Assembly         : Calc32WPF_net
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 10-22-2022
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Calc32WPF_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Timers;
using System.Windows;

/// <summary>
/// The Calc32WPF namespace.
/// </summary>
namespace Calc32WPF.Views
    ;

/// <summary>
/// Interaktionslogik für MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// The animation timer
    /// </summary>
    readonly Timer tmrAnimation = new();
    /// <summary>
    /// The time
    /// </summary>
    public long nTime = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Calc32WPF.MainWindow" /> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
      
        tmrAnimation.Interval = 100;
        tmrAnimation.Elapsed += new ElapsedEventHandler(onAnimationTimer);
    }

    /// <summary>
    /// On animation timer.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
    private void onAnimationTimer(object? sender, ElapsedEventArgs e)
    {
        nTime = DateTime.Now.Ticks / 1000;
    }

    /// <summary>
    /// Handles the Initialized event of the frmCalc32Main control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void frmCalc32Main_Initialized(object sender, EventArgs e)
    {
        tmrAnimation.Start();
    }

}
