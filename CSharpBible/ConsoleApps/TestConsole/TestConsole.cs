using BaseLib.Interfaces;
using ConsoleDisplay.View;
using System;
using TestConsole.Models.Interfaces;
using TestConsole.View;

namespace TestConsole
{
    /// <summary>
    /// Class TstConsole.
    /// Implements the <see cref="MyConsoleBase" />
    /// </summary>
    /// <seealso cref="MyConsoleBase" />
    public class TstConsole : IConsole
    {
        /// <summary>
        /// Gets or sets the color of the foreground.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public ConsoleColor ForegroundColor { get => form.ForegroundColor; set => form.ForegroundColor = value; }
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public ConsoleColor BackgroundColor { get => form.BackgroundColor; set => form.BackgroundColor = value; }
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        public int WindowHeight { get => form.WindowHeight; set => form.WindowHeight = value; }
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        public int WindowWidth { get => form.WindowWidth; set => form.WindowWidth = value; }

        /// <summary>
        /// Gets a value indicating whether [key available].
        /// </summary>
        /// <value><c>true</c> if [key available]; otherwise, <c>false</c>.</value>
        public bool KeyAvailable => form.KeyAvailable;

        /// <summary>
        /// Gets the height of the largest window.
        /// </summary>
        /// <value>The height of the largest window.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public int LargestWindowHeight => form.WindowHeight;

        public string Title { get => form.Text ?? ""; set => form.Text = value; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear() => form.Clear();

        /// <summary>
        /// Reads the key.
        /// </summary>
        /// <returns>System.Nullable&lt;ConsoleKeyInfo&gt;.</returns>
        public ConsoleKeyInfo? ReadKey() => form.ReadKey();

        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        public void SetCursorPosition(int left, int top) => form.SetCursorPosition(left, top);

        /// <summary>
        /// Writes the specified ch.
        /// </summary>
        /// <param name="ch">The ch.</param>
        public void Write(char ch) => form.Write(ch);

        /// <summary>
        /// Writes the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        public void Write(string? st) => form.Write(st);

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="st">The st.</param>
        public void WriteLine(string? st = "") => Write((st ?? "") + "\r\n");

        private readonly IConsoleHandler form;
        /// <summary>
        /// Initializes a new instance of the <see cref="TstConsole"/> class.
        /// </summary>
        public TstConsole()
        {
            form = new TestConsoleForm();
            ((TestConsoleForm)form).Show();
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        public (int Left, int Top) GetCursorPosition()
        {
            return form.GetCursorPosition();
        }

        /// <summary>
        /// Beeps the specified freq.
        /// </summary>
        /// <param name="freq">The freq.</param>
        /// <param name="len">The length.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Beep(int freq, int len)
        {
            throw new NotImplementedException();
        }

        private readonly System.Collections.Concurrent.ConcurrentQueue<string> _scriptedLines = new();

        /// <summary>
        /// Enqueue a full line (without newline) that will be returned by the next ReadLine() call.
        /// Useful for deterministic unit tests.
        /// </summary>
        public void EnqueueLine(string line) => _scriptedLines.Enqueue(line);

        /// <summary>
        /// Optional. If set, ReadLine will wait up to this duration for keys to become available.
        /// When elapsed, it throws to avoid freezing test environments.
        /// </summary>
        public TimeSpan? ReadLineTimeout { get; set; }

        public string ReadLine()
        {
            if (_scriptedLines.TryDequeue(out var scripted))
            {
                // Optional echo behavior to keep output similar to a real console
                Write(scripted);
                WriteLine();
                return scripted;
            }

            var timeout = ReadLineTimeout;
            var start = DateTime.UtcNow;

            var sb = new System.Text.StringBuilder();

            while (true)
            {
                if (timeout.HasValue && DateTime.UtcNow - start > timeout.Value)
                {
                    throw new TimeoutException("ReadLine timed out waiting for input.");
                }

                // Avoid hot spinning in test runners
                if (!KeyAvailable)
                {
                    System.Threading.Thread.Sleep(10);
                    continue;
                }

                var keyInfo = ReadKey();
                if (keyInfo == null)
                {
                    continue;
                }

                switch (keyInfo.Value.Key)
                {
                    case ConsoleKey.Enter:
                        WriteLine();
                        return sb.ToString();

                    case ConsoleKey.Backspace:
                        if (sb.Length > 0)
                        {
                            sb.Length -= 1;
                            Write("\b \b");
                        }
                        break;

                    default:
                        var ch = keyInfo.Value.KeyChar;
                        if (ch == '\0' || char.IsControl(ch))
                        {
                            break;
                        }

                        sb.Append(ch);
                        Write(ch);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content => form.Content;

        public bool IsOutputRedirected 
            => false;

        public bool CursorVisible
        {
            get => true;
            set
            {
                // Test console UI does not currently model caret visibility.
            }
        }

        public int BufferWidth => WindowWidth;

        public int BufferHeight => WindowHeight;
    }
}
