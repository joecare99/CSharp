// ***********************************************************************
// Assembly         : MVVM_09a_CTDialogBoxes
// Author           : Mir
// Created          : 12-29-2021
//
// Last Modified By : Mir
// Last Modified On : 07-20-2022
// ***********************************************************************
// <copyright file="DialogWindow.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_09a_CTDialogBoxes.ViewModels;
using System;
using System.Windows;

namespace MVVM_09a_CTDialogBoxes.Views
{
    /// <summary>
    /// Interaktionslogik für DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window, IDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWindow"/> class.
        /// </summary>
        public DialogWindow()
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
            var vm = (DialogWindowViewModel)DataContext;
            vm.DoCancel += DoCancel;
            vm.DoOK += DoOK;
        }

        private void DoOK(object sender,EventArgs e)
        {
            DialogResult = true;
            Hide();
        }

        private void DoCancel(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
