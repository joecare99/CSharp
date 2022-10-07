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
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the TextChanged event of the edtInput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void edtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnQuit.IsEnabled = (edtInput.Text.ToLower() == "quit");
            lblTextboxHint.Margin = new Thickness(0,  (edtInput.Text != "") ? -30.0: 160.0,0,0);
        }

        /// <summary>
        /// Handles the Click event of the btnQuit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
