﻿// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 12-25-2021
// ***********************************************************************
// <copyright file="Label.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Class Label.
    /// Implements the <see cref="ConsoleLib.Control" />
    /// </summary>
    /// <seealso cref="ConsoleLib.Control" />
    public class Label : Control
    {
        /// <summary>
        /// Gets or sets a value indicating whether [Parent background].
        /// </summary>
        /// <value><c>true</c> if [Parent background]; otherwise, <c>false</c>.</value>
        public bool ParentBackground { get; set; }
        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void Draw()
        {
            // Draw Background
            ConsoleFramework.console.ForegroundColor = ForeColor;
            if (ParentBackground && Parent != null)
            {
                ConsoleFramework.console.BackgroundColor = Parent.BackColor;
            }
            else
            {
                ConsoleFramework.console.BackgroundColor = BackColor;
            }
          
            ConsoleFramework.Canvas.OutTextXY(RealDim.Location,(" "+(Text ?? "")+"                  ").Substring(0, Math.Min(Size.Width,(Text ?? "").Length+14)));
            ConsoleFramework.console.BackgroundColor = ConsoleColor.Black;
        }

    }
}
