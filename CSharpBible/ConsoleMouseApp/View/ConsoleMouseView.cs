// ***********************************************************************
// Assembly         : ConsoleMouseApp
// Author           : Mir
// Created          : 08-18-2022
//
// Last Modified By : Mir
// Last Modified On : 08-18-2022
// ***********************************************************************
// <copyright file="ConsoleMouseView.cs" company="JC-Soft">
//     Copyright © 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleLib;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleMouseApp.View
{
    /// <summary>
    /// Class ConsoleMouseView.
    /// Implements the <see cref="ConsoleLib.CommonControls.Application" />
    /// </summary>
    /// <seealso cref="ConsoleLib.CommonControls.Application" />
    public class ConsoleMouseView : ConsoleLib.CommonControls.Application
    {
        #region Properties
#if NET5_0_OR_GREATER
        private static ConsoleLib.CommonControls.Button? One;
        private static ConsoleLib.CommonControls.Label? lblMousePos;
#else
        /// <summary>
        /// The one
        /// </summary>
        private static ConsoleLib.CommonControls.Button One;
        /// <summary>
        /// The label mouse position
        /// </summary>
        private static ConsoleLib.CommonControls.Label lblMousePos;
#endif
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleMouseView"/> class.
        /// </summary>
        public ConsoleMouseView()
        {
            var cl = ConsoleFramework.Canvas.ClipRect;
            cl.Inflate(-3, -3);

            Visible = false;
            Border = ConsoleFramework.singleBorder;
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.DarkGray;
            BoarderColor = ConsoleColor.Green;
            Dimension = cl;

            One = new ConsoleLib.CommonControls.Button
            {
                Parent = this,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                Shadow = true,
                Position = new Point(5, 10),
                Text = "░░1░░"
            };
            One.OnClick += One_Click;

            var Panel2 = CreatePanel(new Rectangle(3, 15, 30, 10));

            lblMousePos = new ConsoleLib.CommonControls.Label
            {
                Parent = this,
                ForeColor = ConsoleColor.Gray,
                ParentBackground = true,
                Position = new Point(40, 2),
                Text = "lblMousePos",
                size = new Size(15, 1)
            };

            Visible = true;
            OnMouseMove += App_MouseMove;

        }

        /// <summary>
        /// Creates the panel.
        /// </summary>
        /// <param name="cl">The cl.</param>
        /// <returns>ConsoleLib.CommonControls.Panel.</returns>
        private ConsoleLib.CommonControls.Panel CreatePanel(Rectangle cl)
        {
            var result = new ConsoleLib.CommonControls.Panel
            {
                Parent = this,
                Border = ConsoleFramework.doubleBoarder,
                ForeColor = ConsoleColor.Blue,
                BackColor = ConsoleColor.DarkBlue,
                BoarderColor = ConsoleColor.Green,
                Dimension = cl,
                Shadow = true
            };

            var btnOK = new ConsoleLib.CommonControls.Button
            {
                Parent = result,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                Shadow = true,
                Position = new Point(2, 2),
                Text = "░░░OK░░░",
            };
            btnOK.OnClick += btnOK_Click;

            var btnCancel = new ConsoleLib.CommonControls.Button
            {
                Parent = result,
                ForeColor = ConsoleColor.White,
                BackColor = ConsoleColor.Gray,
                Shadow = true,
                Position = new Point(14, 2),
                Text = "░Cancel░",
            };
            btnCancel.OnClick += btnCancel_Click;

            return result;
        }

        /// <summary>
        /// Handles the MouseMove event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void App_MouseMove(object sender,IMouseEvent e)
        {
            if (lblMousePos == null) return;
            lblMousePos.Text = e.MousePos.ToString();
        }

#if NET5_0_OR_GREATER
        private void btnCancel_Click(object? sender, EventArgs e)
#else
        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
#endif
        {
            Stop();
        }

#if NET5_0_OR_GREATER
        private void btnOK_Click(object? sender, EventArgs e)
#else
        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
#endif
        {
            Console.Write("OK");
        }

#if NET5_0_OR_GREATER
        private void One_Click(object? sender, EventArgs e)
#else
        /// <summary>
        /// Handles the <see cref="E:Click" /> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void One_Click(object sender, EventArgs e)
#endif
        {
            Console.Write("1");
        }
#endregion
    }    
}
