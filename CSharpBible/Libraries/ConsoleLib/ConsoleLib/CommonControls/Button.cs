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
using System.Windows.Input;

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
    //    private bool _WasPressed;
        /// <summary>
        /// The back color
        /// </summary>
        private bool _enabled = true;
        private ICommand? command;

        public ConsoleColor HLBackColor { get; set; } = ConsoleColor.Green;
        public ConsoleColor DisabledBackColor { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor DisabledFrontColor { get; set; } = ConsoleColor.Black;

        /// <summary>
        /// Presseds the specified m.
        /// </summary>
        /// <param name="M">The m.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
//        public bool Pressed(Point M) => Over(M) && !_WasPressed & (_WasPressed = ConsoleFramework.MouseButtonLeft);
        public bool Enabled { get => _enabled; set => SetEnabled(value); }

        /// <summary>
        /// Sets the specified x.
        /// </summary>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="text">The text.</param>
        /// <param name="backColor">Color of the back.</param>
        public void Set(int X, int Y, string text, ConsoleColor backColor)
        {
            BackColor =
            _ActBackColor = backColor;
            _ActForeColor = ForeColor;
            Text = text;
            size = new Size(text.Length + 2, 1);
            Position = new Point(X, Y);
        }

        /// <summary>
        /// Mouses the enter.
        /// </summary>
        /// <param name="M">The m.</param>
        public override void MouseEnter(Point M)
        {
            if (_enabled)
            {
                base.MouseEnter(M);
                _ActBackColor = HLBackColor;
                Invalidate();
            }
        }
        /// <summary>
        /// Mouses the leave.
        /// </summary>
        /// <param name="M">The m.</param>
        public override void MouseLeave(Point M)
        {
            base.MouseLeave(M);
            if (_enabled)
                _ActBackColor = BackColor;
            else
                _ActBackColor = DisabledBackColor;
            Invalidate();
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="value">The value.</param>
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

        public ICommand? Command
        {
            get => command; set
            {
                if (command != null)
                {
                    command.CanExecuteChanged -= Command_CanExecuteChanged;
                    OnClick -= CommandExecute;
                }
                command = value;
                if (command != null)
                {
                    command.CanExecuteChanged += Command_CanExecuteChanged;
                    OnClick += CommandExecute;
                    Enabled = command.CanExecute(Tag);
                }
            }
        }

        private void CommandExecute(object? sender, EventArgs e) 
            => command?.Execute(Tag);

        private void Command_CanExecuteChanged(object? sender, EventArgs e) 
            => Enabled = (sender as ICommand)?.CanExecute(Tag) ?? Enabled;
    }

}
