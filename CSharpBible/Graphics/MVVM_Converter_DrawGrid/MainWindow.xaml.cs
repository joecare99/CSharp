﻿using MVVM.ViewModel;
using MVVM_Converter_DrawGrid.ViewModel;
using System;
using System.Windows;

namespace MVVM_Converter_DrawGrid
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ShowClient = ShowClientinFrame;
            }
        }

        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
//                vm.FrameDataContext = e.Source as Frame
            }
        }

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
