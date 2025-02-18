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
using BaseLib.Helper;
using CSharpBible.AboutEx.ViewModels.Interfaces;
using System;
using System.Windows.Forms;
using Views;

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
        #if !NET7_0_OR_GREATER
        public object? DataContext {get;set;}
        #endif

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAboutExMain" /> class.
        /// </summary>
        public FrmAboutExMain(IFrmAboutExMainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.ShowAboutFrm1 = btnClickMe_Click;
            viewModel.ShowAboutFrm2 = btnClickMe2_Click;
            CommandBindingAttribute.Commit(this, viewModel);
        }

        /// <summary>
        /// Handles the Click event of the btnClickMe control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnClickMe_Click(string[] strings)
        {
            var f = IoC.GetRequiredService<FrmAbout>();
            (f.DataContext as IAboutViewModel)?.SetData(strings); 
            f.Show();
        }

        /// <summary>
        /// Handles the Click event of the btnClickMe2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnClickMe2_Click(string[] strings)
        {
            var f = IoC.GetRequiredService<AboutBox1>();
            (f.DataContext as IAboutViewModel)?.SetData(strings);
            f.Show();
        }
    }
}
