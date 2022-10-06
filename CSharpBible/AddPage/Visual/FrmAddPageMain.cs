// ***********************************************************************
// Assembly         : AddPage
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 02-02-2020
// ***********************************************************************
// <copyright file="FrmAddPageMain.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;

namespace CSharpBible.AddPage.Visual
{
    /// <summary>
    /// Class FrmAddPageMain.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmAddPageMain : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrmAddPageMain"/> class.
        /// </summary>
        public FrmAddPageMain()
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
        /// Handles the Click event of the btnAddPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnAddPage_Click(object sender, EventArgs e)
        {
            TabPage tb = new TabPage();
            tb.Name = "tabPage" + (tabControl1.TabCount + 1).ToString();
            tb.Padding = new System.Windows.Forms.Padding(3);
            tb.TabIndex = tabControl1.TabCount;
            tb.Text = tb.Name;
            tb.UseVisualStyleBackColor = true;
            tabControl1.Controls.Add(tb);
        }

        /// <summary>
        /// Handles the Click event of the btnAddControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnAddControl_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.Count < 1)
            {
                CheckBox cb = new CheckBox();
                cb.AutoSize = true;
                cb.Location = new System.Drawing.Point(200, 100);
                cb.Name = "checkBox1";
                cb.Size = new System.Drawing.Size(113, 24);
                cb.TabIndex = 0;
                cb.Text = "checkBox1";
                cb.UseVisualStyleBackColor = true;
                tabControl1.SelectedTab.Controls.Add(cb);
            }
        }
    }
}
