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

        public bool Enabled { get => _enabled; set => SetEnabled(value); }

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
            if (_enabled)
            {
                base.MouseEnter(M);
                _ActBackColor = HLBackColor;
                Invalidate();
            }
        }

        public override void MouseLeave(Point M)
        {
            base.MouseLeave(M);
            _ActBackColor = _enabled ? BackColor : DisabledBackColor;
            Invalidate();
        }

        public override void SetText(string value)
        {
            base.SetText(value);
            size = new Size(value.Length + 2, 1);
        }

        private void SetEnabled(bool value)
        {
            _enabled = value;
            _ActBackColor = _enabled ? BackColor : DisabledBackColor;
            _ActForeColor = _enabled ? ForeColor : DisabledFrontColor;
            Invalidate();
        }

        public override ICommand? Command
        {
            get => base.Command; set
            {
                base.Command = value;
                if (value != null)
                {
                    Enabled = value.CanExecute(Tag);
                }
            }
        }

        protected override void Command_CanExecuteChanged(object? sender, EventArgs e)
        {
            Enabled = (sender as ICommand)?.CanExecute(Tag) ?? Enabled;
            base.Command_CanExecuteChanged(sender, e);
        }
    }
}
