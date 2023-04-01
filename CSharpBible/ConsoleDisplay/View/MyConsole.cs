// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 07-16-2022
//
// Last Modified By : Mir
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="MyConsole.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Reflection;

namespace ConsoleDisplay.View
{

    /// <summary>
    /// Class MyConsole.
    /// Implements the <see cref="ConsoleDisplay.View.MyConsoleBase" />
    /// </summary>
    /// <seealso cref="ConsoleDisplay.View.MyConsoleBase" />
    public class MyConsole : MyConsoleBase
    {
        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        protected PropertyInfo? foregroundColor { get; set; }
            = typeof(Console).GetProperty(nameof(ForegroundColor));
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        protected PropertyInfo? backgroundColor { get; set; }
            = typeof(Console).GetProperty(nameof(BackgroundColor));
        /// <summary>
        /// Gets or sets the key available.
        /// </summary>
        /// <value>The key available.</value>
        protected PropertyInfo? keyAvailable { get; set; }
            = typeof(Console).GetProperty(nameof(KeyAvailable));
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        protected PropertyInfo? windowHeight { get; set; }
            = typeof(Console).GetProperty(nameof(WindowHeight));
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        protected PropertyInfo? windowWidth { get; set; }
            = typeof(Console).GetProperty(nameof(WindowWidth));
        /// <summary>
        /// Gets or sets the height of the largest window.
        /// </summary>
        /// <value>The height of the largest window.</value>
        protected PropertyInfo? largestWindowHeight { get; set; }
            = typeof(Console).GetProperty(nameof(LargestWindowHeight));

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        /// <value>The height of the largest window.</value>
        protected PropertyInfo? title { get; set; }
            = typeof(Console).GetProperty(nameof(Title));

        /// <summary>
        /// Gets or sets the clear.
        /// </summary>
        /// <value>The clear.</value>
        protected MethodInfo? clear { get; set; }
            = typeof(Console).GetMember(nameof(Clear))?.First(
                (o) => true) as MethodInfo;
        /// <summary>
        /// Gets or sets the read key.
        /// </summary>
        /// <value>The read key.</value>
        protected MethodInfo? readKey { get; set; }
            = typeof(Console).GetMember(nameof(ReadKey))?.First(
                (o) => true) as MethodInfo;
        /// <summary>
        /// Gets or sets the write ch.
        /// </summary>
        /// <value>The write ch.</value>
        protected MethodInfo? write_ch { get; set; }
            = typeof(Console).GetMember(nameof(Console.Write))?.First(
                (o) => (o as MethodInfo)?.GetParameters()?[0].ParameterType==typeof(char) ) as MethodInfo;
        /// <summary>
        /// Gets or sets the write st.
        /// </summary>
        /// <value>The write st.</value>
        protected MethodInfo? write_st { get; set; }
            = typeof(Console).GetMember(nameof(Console.Write))?.First(
                (o) => o is MethodInfo m 
                    && m.GetParameters().Length==1 
                    && m.GetParameters()?[0].ParameterType == typeof(string)) as MethodInfo;
        /// <summary>
        /// Gets or sets the write st.
        /// </summary>
        /// <value>The write st.</value>
        protected MethodInfo? read_st { get; set; }
            = typeof(Console).GetMember(nameof(Console.ReadLine))?.First(
                (o) => o is MethodInfo m 
                  && (m.GetParameters().Length == 0) 
                  && m.ReturnType == typeof(string)) as MethodInfo;

        /// <summary>
        /// Gets or sets the set cursor position.
        /// </summary>
        /// <value>The set cursor position.</value>
        protected MethodInfo? setCursorPos { get; set; }
            = typeof(Console).GetMember(nameof(SetCursorPosition))?.First((o) => true) as MethodInfo;

        /// <summary>
        /// Gets or sets the beep int.
        /// </summary>
        /// <value>The beep int.</value>
        protected MethodInfo? beep_int { get; set; }
            = typeof(Console).GetMember(nameof(Beep))?.First(
                (o) => (o as MethodInfo)?.GetParameters().Length == 2) as MethodInfo;
        /// <summary>
        /// Gets or sets the get cursor position.
        /// </summary>
        /// <value>The get cursor position.</value>
        protected MethodInfo? getCursorPos { get; set; }
#if NET6_0_OR_GREATER
			= typeof(Console).GetMember(nameof(Console.GetCursorPosition))?.First((o) => true) as MethodInfo;
#else
			= typeof(Console).GetMethod(nameof(Console.CursorTop));
#endif
        /// <summary>
        /// The instance
        /// </summary>
        protected object? instance = null;

        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public override ConsoleColor ForegroundColor
        {
            get => (ConsoleColor)(foregroundColor?.GetValue(instance) ?? ConsoleColor.Gray);
            set => foregroundColor?.SetValue(instance, value);
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public override ConsoleColor BackgroundColor
        {
            get => (ConsoleColor)(backgroundColor?.GetValue(instance) ?? ConsoleColor.Gray);
            set => backgroundColor?.SetValue(instance, value);
        }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        public override int WindowHeight
        {
            get => (int)(windowHeight?.GetValue(instance) ?? 0);
            set => windowHeight?.SetValue(instance, value);
        }

        /// <summary>
        /// Gets a value indicating whether [key available].
        /// </summary>
        /// <value><c>true</c> if [key available]; otherwise, <c>false</c>.</value>
        public override bool KeyAvailable {
            get => (bool)(keyAvailable?.GetValue(instance) ?? false);
        }
        /// <summary>
        /// Gets the height of the largest window.
        /// </summary>
        /// <value>The height of the largest window.</value>
        public override int LargestWindowHeight { 
            get =>(int) (largestWindowHeight?.GetValue(instance) ?? 0);
        }
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public override int WindowWidth { 
            get => (int)(windowWidth?.GetValue(instance) ?? 0);
            set => windowWidth?.SetValue(instance, value); }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public override string Title { 
            get => (string)(title?.GetValue(instance) ??""); 
            set => title?.SetValue(instance, value); }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear() => clear?.Invoke(instance, new object[] { });
        /// <summary>
        /// Writes the specified ch.
        /// </summary>
        /// <param name="ch">The ch.</param>
        public override void Write(char ch) => write_ch?.Invoke(instance, new object[] { ch });
        /// <summary>
        /// Writes the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        public override void Write(string? st) => write_st?.Invoke(instance, new object[] { st ?? "" });
        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="st">The st.</param>
        public override void WriteLine(string? st="") => write_st?.Invoke(instance, new object[] { st+"\r\n" });

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="st">The st.</param>
        public override string ReadLine() => 
            (string)(read_st?.Invoke(instance, new object[] {}) ?? "");

        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public override void SetCursorPosition(int left, int top) => setCursorPos?.Invoke(instance, new object[] { left, top });

        /// <summary>
        /// Reads the key.
        /// </summary>
        /// <returns>System.Nullable&lt;ConsoleKeyInfo&gt;.</returns>
        public override ConsoleKeyInfo? ReadKey() => (ConsoleKeyInfo?)readKey?.Invoke(instance, new object[] { });

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        public override (int Left, int Top) GetCursorPosition() => ((int Left, int Top)?)getCursorPos?.Invoke(instance, new object[] { }) ?? (0,0);

        /// <summary>
        /// Beeps the specified freq.
        /// </summary>
        /// <param name="freq">The freq.</param>
        /// <param name="dur">The dur.</param>
        public override void Beep(int freq,int dur) => beep_int?.Invoke(instance, new object[] { freq, dur });

    }
}
