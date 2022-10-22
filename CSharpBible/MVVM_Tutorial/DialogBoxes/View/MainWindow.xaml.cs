// ***********************************************************************
// Assembly         : DialogBoxes
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
using DialogBoxes.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DialogBoxes.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
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
            var vm = (MainWindowViewModel)DataContext;
            vm.OpenDialog = (Name, email) =>
            {
                DialogWindow dialog = new DialogWindow();
                var dialogViewModel = ((DialogWindowViewModel)dialog.DataContext);
                (dialogViewModel.Name, dialogViewModel.Email) = (Name,email);
                if (dialog.ShowDialog() == true)
                {
                    return (dialogViewModel.Name, dialogViewModel.Email);
                }
                else
                    return (Name, email);
            };
            vm.OpenMessageBox = (Title, Name) => MessageBox.Show(Title, Name,MessageBoxButton.YesNo);
        }
    }
}
