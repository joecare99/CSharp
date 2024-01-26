// ***********************************************************************
// Assembly         : MVVM_07a_CTDialogBoxes
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
using MVVM_07a_CTDialogBoxes.ViewModel;
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

namespace MVVM_07a_CTDialogBoxes.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Func<IDialogWindow> NewDialogWindow = () => new DialogWindow();
        public Func<string, string, MessageBoxButton, MessageBoxResult> MessageBoxShow = 
            (t, n,mbb) => MessageBox.Show(t, n, mbb);
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
            vm.DoOpenDialog = DoOpenDialog;
            vm.DoOpenMessageBox = DoOpenMessageBox;
        }

        private MessageBoxResult DoOpenMessageBox(string Title, string Name) 
            => MessageBoxShow(Title, Name, MessageBoxButton.YesNo);

        private (string name, string email) DoOpenDialog(string Name, string email)
        {
            IDialogWindow dialog = NewDialogWindow();
            var dialogViewModel = ((DialogWindowViewModel)dialog.DataContext);
            (dialogViewModel.Name, dialogViewModel.Email) = (Name, email);
            if (dialog.ShowDialog() == true)
            {
                return (dialogViewModel.Name, dialogViewModel.Email);
            }
            else
                return (Name, email);
        }
    }
}
