// ***********************************************************************
// Assembly         : ActionTestWPF
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 03-12-2020
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using ActionTest.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ActionTestWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow(IActionMainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.ExitAction = (o) => Close();
        }
    }
}
