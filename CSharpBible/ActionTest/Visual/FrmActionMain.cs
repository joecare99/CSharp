﻿// ***********************************************************************
// Assembly         : ActionTest
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 02-02-2020
// ***********************************************************************
// <copyright file="FrmActionMain.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;

namespace CSharpBible.ActionTest.Visual
{
    /// <summary>
    /// Class FrmActionMain.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmActionMain : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrmActionMain"/> class.
        /// </summary>
        public FrmActionMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the CurrentChanged event of the bindingSource1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
