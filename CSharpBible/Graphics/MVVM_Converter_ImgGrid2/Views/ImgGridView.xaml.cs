// ***********************************************************************
// Assembly         : MVVM_Converter_ImgGrid2
// Author           : Mir
// Created          : 08-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-21-2022
// ***********************************************************************
// <copyright file="ImgGridView.xaml.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using MVVM_Converter_ImgGrid2.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_ImgGrid2.Views
{
    /// <summary>
    /// Interaction logic for ImgGridView.xaml
    /// </summary>
    public partial class ImgGridView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImgGridView"/> class.
        /// </summary>
        public ImgGridView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImgGridViewModel vm)
            {
                vm.ShowClient = _ShowClientinFrame;
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Frame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImgGridViewModel vm)
            {
//                vm.FrameDataContext = e.Source as Frame
            }
        }

        /// <summary>
        /// Shows the clientin frame.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns>BaseViewModel.</returns>
        private BaseViewModel? _ShowClientinFrame(string arg)
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
