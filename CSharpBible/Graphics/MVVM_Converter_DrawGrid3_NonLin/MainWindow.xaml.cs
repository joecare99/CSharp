﻿// ***********************************************************************
// Assembly         : MVVM_Converter_DrawGrid3_NonLin
// Author           : Mir
// Created          : 09-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-04-2022
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using MVVM_Converter_DrawGrid3_NonLin.ViewModel;
using System;
using System.Windows;

namespace MVVM_Converter_DrawGrid3_NonLin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ShowClient = ShowClientinFrame;
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Frame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
//                vm.FrameDataContext = e.Source as Frame
            }
        }

        /// <summary>
        /// Shows the clientin frame.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>BaseViewModel.</returns>
        private BaseViewModel ShowClientinFrame(string arg)
        {
            try
            {
                this.Client.Source = new Uri(arg);
                return Client.DataContext as BaseViewModel;
            }
            catch(Exception ex) 
            {
                return null;
            };
        }
    }
}
