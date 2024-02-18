// ***********************************************************************
// Assembly         : MVVM_09_DialogBoxes
// Author           : Mir
// Created          : 12-29-2021
//
// Last Modified By : Mir
// Last Modified On : 07-20-2022
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_09_DialogBoxes.ViewModel;
using System;
using System.Windows;

namespace MVVM_09_DialogBoxes.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Func<IDialogWindow> NewDialogWindow = () => new DialogWindow();
        public Func<string, string, MessageBoxButton, MessageBoxResult> MessageBoxShow =
            (t, n, mbb) => MessageBox.Show(t, n, mbb);

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
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.DoOpenDialog = (Name, email) =>
            {
                IDialogWindow dialog = NewDialogWindow();
                var dialogViewModel = ((DialogWindowViewModel)dialog.DataContext);
                (dialogViewModel.Name, dialogViewModel.Email) = (Name,email);
                if (dialog.ShowDialog() == true)
                {
                    return (dialogViewModel.Name, dialogViewModel.Email);
                }
                else
                    return (Name, email);
            };
            vm.DoOpenMessageBox = (Title, Name) => MessageBoxShow(Title, Name,MessageBoxButton.YesNo);
        }
    }
}
