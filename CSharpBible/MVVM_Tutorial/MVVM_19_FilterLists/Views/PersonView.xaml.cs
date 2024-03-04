// ***********************************************************************
// Assembly         : ListBinding
// Author           : Mir
// Created          : 12-23-2021
//
// Last Modified By : Mir
// Last Modified On : 12-24-2021
// ***********************************************************************
// <copyright file="PersonView.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM_19_FilterLists.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_19_FilterLists.Views
{
    /// <summary>
    /// Interaktionslogik für PersonView.xaml
    /// </summary>
    public partial class PersonView : Page
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
