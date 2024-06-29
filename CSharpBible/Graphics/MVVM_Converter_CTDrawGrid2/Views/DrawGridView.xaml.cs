// ***********************************************************************
// Assembly         : MVVM_Converter_DrawGrid2
// Author           : Mir
// Created          : 08-21-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="DrawGridView.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using MVVM_Converter_CTDrawGrid2.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_CTDrawGrid2.Views
{
    /// <summary>
    /// Interaction logic for DrawGridView.xaml
    /// </summary>
    public partial class DrawGridView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawGridView" /> class.
        /// </summary>
        public DrawGridView()
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
            if (DataContext is DrawGridViewModel vm)
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
            if (DataContext is DrawGridViewModel vm)
            {
//                vm.FrameDataContext = e.Source as Frame
            }
        }

        /// <summary>
        /// Shows the clientin frame.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>BaseViewModel.</returns>
        private BaseViewModel? ShowClientinFrame(string arg)
        {
            try
            {
                this.Client.Source = new Uri(arg);
                return Client.DataContext as BaseViewModel;
            }
            catch(Exception) 
            {
                return null;
            };
        }
    }
}
