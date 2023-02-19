// ***********************************************************************
// Assembly         : WpfDemoUI2
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="WpfDemoUI2">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using System.Windows.Controls;

namespace WpfDemoUI.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow() {
			InitializeComponent();
		}

		/// <summary>
		/// Handles the SelectionChanged event of the DataGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {

		}
	}
}
