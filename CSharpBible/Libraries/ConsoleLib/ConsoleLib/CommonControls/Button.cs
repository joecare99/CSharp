// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : AI Assistant
// Last Modified On : 09-26-2025
// ***********************************************************************
// <copyright file="Button.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.Windows.Input;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Class Button.
    /// Implements the <see cref="ConsoleLib.Control" />
    /// </summary>
    /// <seealso cref="ConsoleLib.Control" />
    public class Button : CommandControl
    {
        public ConsoleColor HLBackColor { get; set; } = ConsoleColor.Green;
        public ConsoleColor DisabledBackColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor DisabledFrontColor { get; set; } = ConsoleColor.Black;

        public void Set(int X, int Y, string text, ConsoleColor backColor)
        {
            BackColor =
            _ActBackColor = backColor;
            _ActForeColor = ForeColor;
            Text = text;
            size = new Size(text.Length + 2, 1);
            Position = new Point(X, Y);
        }

        public override void MouseEnter(Point M)
        {
            if (Enabled)
            {
                base.MouseEnter(M);
                _ActBackColor = HLBackColor;
                Invalidate();
            }
        }

        public override void MouseLeave(Point M)
        {
            base.MouseLeave(M);
            _ActBackColor = Enabled ? BackColor : DisabledBackColor;
            Invalidate();
        }

        public override void SetText(string value)
        {
            base.SetText(value);
            size = new Size(value.Length + 2, 1);
        }

        public override void Draw()
        {
            // Beispiel: Farbwahl abhängig von Enabled
            var oldFore = ForeColor;
            var oldBack = BackColor;
            if (!Enabled)
            {
                ForeColor = ConsoleColor.DarkGray;
                BackColor = ConsoleColor.Black;
            }
            base.Draw();
            ForeColor = oldFore;
            BackColor = oldBack;
        }
    }
}
