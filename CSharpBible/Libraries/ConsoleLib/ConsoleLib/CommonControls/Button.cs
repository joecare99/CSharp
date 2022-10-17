// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-05-2022
// ***********************************************************************
// <copyright file="Button.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Class Button.
    /// Implements the <see cref="ConsoleLib.Control" />
    /// </summary>
    /// <seealso cref="ConsoleLib.Control" />
    public class Button : Control
    {
        /// <summary>
        /// The was pressed
        /// </summary>
        private bool _WasPressed;
        /// <summary>
        /// The back color
        /// </summary>
        private ConsoleColor _BackColor;

        /// <summary>
        /// Presseds the specified m.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Pressed(Point M) => Over(M) && !_WasPressed & (_WasPressed=ConsoleFramework.MouseButtonLeft) ;

        /// <summary>
        /// Sets the specified x.
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="text">The text.</param>
        /// <param name="backColor">Color of the back.</param>
        public void Set(int X, int Y, string text, ConsoleColor backColor)
        {
            BackColor=
            _BackColor = backColor;
            Text = text;
            size = new Size(text.Length + 2, 1);
            position = new Point(X, Y);
        }

        /// <summary>
        /// Mouses the enter.
        /// </summary>
        /// <param name="M">The m.</param>
        public override void MouseEnter(Point M)
        {
            base.MouseEnter(M);
            _BackColor = BackColor;
            BackColor = ConsoleColor.Green;
            Invalidate();
        }
        /// <summary>
        /// Mouses the leave.
        /// </summary>
        /// <param name="M">The m.</param>
        public override void MouseLeave(Point M)
        {
            base.MouseLeave(M);
            BackColor = _BackColor;
            Invalidate();
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetText(string value)
        {
            base.SetText(value);
            size = new Size(value.Length+2, 1);
        }
       

    }

}
