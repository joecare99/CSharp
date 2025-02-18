// ***********************************************************************
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
using CSharpBible.AboutEx.ViewModels.Interfaces;
using Views;

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
#if !NET7_0_OR_GREATER
        public object? DataContext {get;set;}
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAbout" /> class.
        /// </summary>
        public FrmAbout(IAboutViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            CommandBindingAttribute.Commit(this, viewModel);
            TextBindingAttribute.Commit(this, viewModel);
        }

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
