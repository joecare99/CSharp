using ConsoleDisplay.View;
using System;
using TestConsole.View;

namespace TestConsole
{
    /// <summary>
    /// Class TstConsole.
    /// Implements the <see cref="MyConsoleBase" />
    /// </summary>
    /// <seealso cref="MyConsoleBase" />
    public class TstConsole : MyConsoleBase
    {
        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public override ConsoleColor ForegroundColor { get => form?.foregroundColor ?? ConsoleColor.Gray; set => form.foregroundColor = value; }
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public override ConsoleColor BackgroundColor { get => form?.backgroundColor ?? ConsoleColor.Black; set => form.backgroundColor = value; }
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        public override int WindowHeight { get => form?.WindowHeight ?? 0; set => form.WindowHeight = value; }
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public override int WindowWidth { get => form?.WindowWidth ?? 0; set => form.WindowWidth = value; }

        /// <summary>
        /// Gets a value indicating whether [key available].
        /// </summary>
        /// <value><c>true</c> if [key available]; otherwise, <c>false</c>.</value>
        public override bool KeyAvailable => form?.KeyAvailable ?? false;

        /// <summary>
        /// Gets the height of the largest window.
        /// </summary>
        /// <value>The height of the largest window.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int LargestWindowHeight => throw new NotImplementedException();

        public override string Title { get => form?.Text; set => form.Text = value; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear() => form?.Clear();

        /// <summary>
        /// Reads the key.
        /// </summary>
        /// <returns>System.Nullable&lt;ConsoleKeyInfo&gt;.</returns>
        public override ConsoleKeyInfo? ReadKey() => form?.ReadKey();

        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public override void SetCursorPosition(int left, int top) => form?.SetCursorPosition(left, top);

        /// <summary>
        /// Writes the specified ch.
        /// </summary>
        /// <param name="ch">The ch.</param>
        public override void Write(char ch) => form?.Write(ch);

        /// <summary>
        /// Writes the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        public override void Write(string? st) => form?.Write(st);

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="st">The st.</param>
        public override void WriteLine(string? st = "") => Write((st ?? "") + "\r\n");

        private TestConsoleForm form;
        /// <summary>
        /// Initializes a new instance of the <see cref="TstConsole"/> class.
        /// </summary>
        public TstConsole()
        {
            form = new TestConsoleForm();
            form.Show();
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        public override (int Left, int Top) GetCursorPosition()
        {
            return ((int Left, int Top)?)(form?.GetCursorPosition()) ?? (0, 0);
        }

        /// <summary>
        /// Beeps the specified freq.
        /// </summary>
        /// <param name="freq">The freq.</param>
        /// <param name="len">The length.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Beep(int freq, int len)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content => form.Content;

    }
}
