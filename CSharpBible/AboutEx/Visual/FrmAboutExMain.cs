// ***********************************************************************
// Assembly         : AboutEx
// Author           : Mir
// Created          : 11-11-2022
//
// Last Modified By : Mir
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="FrmAboutExMain.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;

/// <summary>
/// The Visual namespace.
/// </summary>
namespace CSharpBible.AboutEx.Visual
{
    /// <summary>
    /// Class FrmAboutExMain.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmAboutExMain : Form
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAboutExMain" /> class.
        /// </summary>
        public FrmAboutExMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the btnClickMe control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnClickMe_Click(object sender, EventArgs e)
        {
            new FrmAbout().Show();
        }

        /// <summary>
        /// Handles the Click event of the btnClickMe2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnClickMe2_Click(object sender, EventArgs e)
        {
            new AboutBox1().Show();
        }
    }
}
