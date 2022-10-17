﻿// ***********************************************************************
// Assembly         : Calc32Cons
// Author           : Mir
// Created          : 08-05-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="ConsoleCalcView.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleLib;
using Calc32.NonVisual;
using System;
using System.Drawing;

namespace Calc32Cons.Visual
{
    /// <summary>
    /// Class ConsoleCalcView.
    /// Implements the <see cref="ConsoleLib.CommonControls.Panel" />
    /// </summary>
    /// <seealso cref="ConsoleLib.CommonControls.Panel" />
    public class ConsoleCalcView : ConsoleLib.CommonControls.Panel
    {
        /// <summary>
        /// The calculator
        /// </summary>
        private static CalculatorClass Calculator = new CalculatorClass();
        /// <summary>
        /// The label akkumulator
        /// </summary>
        private static ConsoleLib.CommonControls.Label lblAkkumulator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCalcView"/> class.
        /// </summary>
        /// <param name="App">The application.</param>
        public ConsoleCalcView(ConsoleLib.Control App = null)
        {
            parent = App;
            Boarder = ConsoleFramework.doubleBoarder;
            ForeColor = ConsoleColor.Blue;
            BackColor = ConsoleColor.DarkBlue;
            BoarderColor = ConsoleColor.Green;
            dimension = new Rectangle(3, 5, 50, 20);
            shaddow = true;

            ConsoleLib.CommonControls.Button[] btnNumbers = new ConsoleLib.CommonControls.Button[10];

            for (int i = 0; i < 10; i++)
            {
                var p = new Point(((i + 2) % 3) + 1, (i + 2) / 3);
                btnNumbers[i] = new ConsoleLib.CommonControls.Button
                {
                    parent = this,
                    ForeColor = ConsoleColor.White,
                    BackColor = ConsoleColor.DarkGray,
                    shaddow = true,
                    position = new Point(p.X * 8 + 2, 14 - p.Y * 2),
                    Tag = i,
                    Accellerator = i.ToString()[0],
                    Text = $"░{i}░"
                };
                btnNumbers[i].OnClick += btnNumber_Click;
            };
            btnNumbers[0].position = new Point(btnNumbers[0].position.X - 8, btnNumbers[0].position.Y);

            ConsoleLib.CommonControls.Button[] btnCommandss = new ConsoleLib.CommonControls.Button[10];
            for (int i = 1; i < 10; i++)
            {
                var p = new Point(0, 0);
                switch (i)
                {
                    case 1: (p.X, p.Y) = (4, 1); break;
                    case 2: (p.X, p.Y) = (4, 3); break;
                    case 3: (p.X, p.Y) = (4, 4); break;
                    case 4: (p.X, p.Y) = (3, 4); break;
                    case 5: (p.X, p.Y) = (2, 4); break;
                    case 6: (p.X, p.Y) = (5, 0); break;
                    case 7: (p.X, p.Y) = (5, 1); break;
                    case 8: (p.X, p.Y) = (5, 2); break;
                    case 9: (p.X, p.Y) = (3, 0); break;
                    default: (p.X, p.Y) = (0, 0); break;
                };
                btnCommandss[i] = new ConsoleLib.CommonControls.Button
                {
                    parent = this,
                    ForeColor = ConsoleColor.White,
                    BackColor = ConsoleColor.DarkGray,
                    shaddow = true,
                    position = new Point(p.X * 8 + 2, 14 - p.Y * 2),
                    Tag = -i,
                    Accellerator = (i == 1) ? '=' : CalculatorClass.sMode[i][0],
                    Text = (i == 1) ? "░=░" : $"░{CalculatorClass.sMode[i]}░"
                };
                btnCommandss[i].OnClick += btnCommand_Click;
            }
            btnCommandss[1].size = new Size(5, 3);
            btnCommandss[2].size = new Size(5, 3);

            lblAkkumulator = new ConsoleLib.CommonControls.Label
            {
                parent = this,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.DarkCyan,
                position = new Point(2, 1),
                size = new Size(38, 1),
                Text = "             "
            };

            var btnCancel = new ConsoleLib.CommonControls.Button
            {
                parent = this,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.DarkGray,
                shaddow = true,
                position = new Point(14, 16),
                Text = "░Close░",
            };
            btnCancel.OnClick += btnCancel_Click;

            Calculator.OnChange += Calculator_OnChange;
        }

        /// <summary>
        /// Handles the Click event of the btnCommand control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCommand_Click(object sender, EventArgs e)
        {
            Calculator.Operation(-((ConsoleLib.CommonControls.Button)sender).Tag);
        }

        /// <summary>
        /// Handles the OnChange event of the Calculator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Calculator_OnChange(object sender, EventArgs e)
        {
            lblAkkumulator.Text = Calculator.Akkumulator.ToString();
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            
            (parent as ConsoleLib.CommonControls.Application).Stop();
            
        }

        /// <summary>
        /// Handles the Click event of the btnNumber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnNumber_Click(object sender, EventArgs e)
        {
            Calculator.NumberButton(((ConsoleLib.CommonControls.Button)sender).Tag);
        }

    }
}
