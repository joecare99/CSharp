// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 12-24-2021
// ***********************************************************************
// <copyright file="PersonView.xaml.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using ListBinding.ViewModel;
using System;
using System.Windows;

namespace ListBinding.View
{
    /// <summary>
    /// Interaktionslogik für PersonView.xaml
    /// </summary>
    public partial class PersonView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonView"/> class.
        /// </summary>
        public PersonView()
        {
            InitializeComponent();
            ((PersonViewViewModel)DataContext).MissingData += (object sender, EventArgs e)=>ShowError();
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        public void ShowError()
        {
            MessageBox.Show("Bitte einen Vornamen oder Nachnamen eingeben.");
        }
    }
}
