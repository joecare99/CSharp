﻿// ***********************************************************************
// Assembly         : AboutEx
// Author           : Mir
// Created          : 11-11-2022
//
// Last Modified By : Mir
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="FrmAbout.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;
using System.ComponentModel;

/// <summary>
/// The Visual namespace.
/// </summary>
namespace CSharpBible.AboutEx.Visual
{
    /// <summary>
    /// Class FrmAbout.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmAbout : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAbout" /> class.
        /// </summary>
        public FrmAbout()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string ProductName { get => lblProductName.Text; set => lblProductName.Text = value; }
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Version { get => lblVersion.Text; set => lblVersion.Text = value; }
        /// <summary>
        /// Gets or sets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Copyright { get => lblCopyright.Text; set => lblCopyright.Text = value; }
        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Comments { get => lblComments.Text; set => lblComments.Text = value; }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
