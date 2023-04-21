// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 07-16-2022
//
// Last Modified By : Mir
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="MyConsoleBase.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace ConsoleDisplay.View
{
    /// <summary>
    /// Class MyConsoleBase.
    /// </summary>
    public abstract class MyConsoleBase
    {
        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public abstract ConsoleColor ForegroundColor { get; set; }
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public abstract ConsoleColor BackgroundColor { get; set; }
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        public abstract int WindowHeight { get; set; }
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public abstract int WindowWidth { get; set; }
        /// <summary>
        /// Gets a value indicating whether [key available].
        /// </summary>
        /// <value><c>true</c> if [key available]; otherwise, <c>false</c>.</value>
        public abstract bool KeyAvailable { get; }
        /// <summary>
        /// Gets the height of the largest window.
        /// </summary>
        /// <value>The height of the largest window.</value>
        public abstract int LargestWindowHeight { get; }

        /// <summary>
        /// Gets if the output is redirected.
        /// </summary>
        /// <value>Gets if the output is redirected.</value>
        public abstract bool IsOutputRedirected { get; }


        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public abstract string Title { get; set; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public abstract void Clear();
        /// <summary>
        /// Reads the key.
        /// </summary>
        /// <returns>System.Nullable&lt;ConsoleKeyInfo&gt;.</returns>
        public abstract ConsoleKeyInfo? ReadKey();
        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public abstract void SetCursorPosition(int left, int top);
        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        public abstract (int Left, int Top) GetCursorPosition();

        /// <summary>
        /// Writes the specified ch.
        /// </summary>
        /// <param name="ch">The ch.</param>
        public abstract void Write(char ch);
        /// <summary>
        /// Writes the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        public abstract void Write(string? st);
        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="st">The st.</param>
        public abstract void WriteLine(string? st = "");

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="st">The st.</param>
        public abstract string ReadLine();

        /// <summary>
        /// Beeps the specified freq.
        /// </summary>
        /// <param name="freq">The freq.</param>
        /// <param name="len">The length.</param>
        public abstract void Beep(int freq, int len);

    }
}