// ***********************************************************************
// Assembly         : DataGridEx
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 12-24-2021
// ***********************************************************************
// <copyright file="Form1.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;

namespace DataGridEx
{
    /// <summary>
    /// Class Form1.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class Form1 : Form
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the CurrentChanged event of the customerBindingSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void customerBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
