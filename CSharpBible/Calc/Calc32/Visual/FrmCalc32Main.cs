﻿// ***********************************************************************
// Assembly         : Calc32
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="FrmCalc32Main.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Calc32.NonVisual;
using MVVM.ViewModel;

namespace Calc32.Visual
{
    /// <summary>
    /// Class FrmCalc32Main.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmCalc32Main : Form
    {
        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public NotificationObject DataContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmCalc32Main" /> class.
        /// </summary>
        public FrmCalc32Main()
        {
            InitializeComponent();
//            DataContext = new BaseViewModel
        }

        /// <summary>
        /// Change event of Calculators class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void calculatorClassChange(object sender, EventArgs e)
        {
            using (CalculatorClass cc = (CalculatorClass)sender)
            {
                lblResult.Text = cc.Akkumulator.ToString();
                lblMemory.Text = cc.Memory.ToString();
                lblOperation.Text = cc.OperationText;

            }
        }

        /// <summary>
        /// Handles the Click event of the btnNummber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnNummber_Click(object sender, EventArgs e)
        {
            if (int.TryParse(((Control)sender).Tag.ToString(), out int aNumber))
            {
                calculatorClass1.NumberButton(aNumber);
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the FrmCalc32Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void FrmCalc32Main_MouseMove(object sender, MouseEventArgs e)
        {
            Point lMousePnt = e.Location;
            if (sender!= this)
            {
                lMousePnt.X += ((Control)sender).Location.X;
                lMousePnt.Y += ((Control)sender).Location.Y;
            }
            lMousePnt.Offset(-pictureBox1.Size.Width/2, -pictureBox1.Size.Height/2);
            pictureBox1.Location = lMousePnt;
        }

        /// <summary>
        /// Handles the KeyDown event of the FrmCalc32Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void FrmCalc32Main_KeyDown(object sender, KeyEventArgs e)
        {
            Char ActKey = (char)0;
            if ((char)e.KeyValue >= "0"[0] && (char)e.KeyValue <= "9"[0])
            {
                ActKey = (char)e.KeyValue;
            }
            else if ((char)e.KeyValue >= 96 && (char)e.KeyValue <= 105)
            {
                ActKey = (char)(e.KeyValue - 96 + (int)("0"[0]));
            }

            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    btnPlus.PerformClick();
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    btnMinus.PerformClick();
                    break;
                case Keys.Escape:
                    break;
                case Keys.Back:
                    btnBack.PerformClick();
                    break;
                default:
                    if (ActKey != (char)0)
                    {
                        foreach (Control c in Controls)
                        {
                            if ((c.GetType() == typeof(Button)) && (c.Text[0] == ActKey))
                            {
                                ((Button)c).Select();
                                ((Button)c).PerformClick();
                            }
                        }

                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOperator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnOperator_Click(object sender, EventArgs e)
        {
            if (int.TryParse(((Control)sender).Tag.ToString(), out int aNumber))
            {
                calculatorClass1.Operation(-aNumber);
            }

        }

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the Click event of the btnBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            calculatorClass1.BackSpace();
        }
    }
}
