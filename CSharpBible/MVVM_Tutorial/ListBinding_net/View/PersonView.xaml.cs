// ***********************************************************************
// Assembly         : ListBinding_net
// Author           : Mir
// Created          : 07-01-2022
//
// Last Modified By : Mir
// Last Modified On : 12-24-2021
// ***********************************************************************
// <copyright file="PersonView.xaml.cs" company="ListBinding_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ListBinding.ViewModel;
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
using System.Windows.Shapes;

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
