using BlazorWasmDocker.Models;
using BlazorWasmDocker.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace BlazorWasmDocker.Services
{
    public partial class ConsoleHandler : ObservableObject, IConsoleHandler
    {
        [ObservableProperty]
        private ConsoleCharInfo[] screenBuffer = new ConsoleCharInfo[80 * 25];

        [ObservableProperty]
        private Size consoleSize = new Size(80, 25);

        private Color[] ccolors = new Color[typeof(ConsoleColor).GetEnumValues().Length];
        private List<KeyboardEventArgs> keyPressEventArgs = new List<KeyboardEventArgs>();
        [ObservableProperty]
        private Point cursorPosition;

        public event EventHandler<EventArgs> DoUpdate;

        public ConsoleHandler()
        {
            foreach (var c in typeof(ConsoleColor).GetEnumValues())
                ccolors[(int)c] = Color.FromArgb(((int)c / 4) % 2 * (1 + ((int)c / 8) * 2) * 85, ((int)c / 2) % 2 * (1 + ((int)c / 8) * 2) * 85, ((int)c % 2 * (1 + ((int)c / 8) * 2)) * 85);
            Write("WebConsole Ver. 1.0\r\n");
        }

        public int WindowWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WindowHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ConsoleColor ForegroundColor { get ; set ; }= ConsoleColor.Gray;
        public ConsoleColor BackgroundColor { get ; set; }

        private DateTime lastUpdate;

        public bool KeyAvailable => throw new NotImplementedException();

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get
            {
                string result = "";
                ConsoleColor lastF = ConsoleColor.Gray;
                ConsoleColor lastB = ConsoleColor.Black;
                for (var y = 0; y < ConsoleSize.Height; y++)
                {
                    for (var x = 0; x < ConsoleSize.Width; x++)
                    {
                        var scb = ScreenBuffer[x + y * ConsoleSize.Width];
                        if (scb.fgr != lastF || scb.bgr != lastB)
                        {
                            result += $"\\c{(int)scb.bgr:X}{(int)scb.fgr:X}";
                            lastF = scb.fgr;
                            lastB = scb.bgr;
                        }
                        switch (scb.ch)
                        {
                            case '\\':
                                result += @"\\";
                                break;
                            case '\t':
                                result = result.TrimEnd() + "\\t";
                                break;
                            case '\x00':
                                // TestEol
                                var flag = true;
                                for (var xx = x + 1; xx < ConsoleSize.Width && flag; xx++)
                                    flag = ScreenBuffer[xx + y * ConsoleSize.Width].ch == '\x00' && ScreenBuffer[xx + y * ConsoleSize.Width].bgr == lastB;
                                if (flag)
                                    x = ConsoleSize.Width - 1; //
                                else
                                    result += @"\x00";
                                break;
                            default:
                                result += scb.ch;
                                break;
                        }
                    }
                    if (y < ConsoleSize.Height - 1)
                        result += "\r\n";
                }

                return result.TrimEnd();
            }
        }

        public Color[] Ccolor => ccolors;

        public void Clear()
        {
            ScreenBuffer = new ConsoleCharInfo[80 * 25];
            CursorPosition = new Point(0, 0);
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        public (int Left, int Top) GetCursorPosition() => (cursorPosition.X, cursorPosition.Y);


        /// <summary>
        /// Reads the key.
        /// </summary>
        /// <returns>ConsoleKeyInfo.</returns>
        public ConsoleKeyInfo ReadKey()
        {
            while (keyPressEventArgs.Count == 0)
            {
                _DoUpdate();
                //   Application.RaiseIdle(new EventArgs());
                Thread.Sleep(1);
            }
            ConsoleKeyInfo? result = null;
            if (keyPressEventArgs.Count > 0)
            {
                var kev = keyPressEventArgs[0];
                keyPressEventArgs.RemoveAt(0);
                result = new ConsoleKeyInfo(kev.Key[0], ConsoleKey.NoName, kev.ShiftKey, kev.AltKey, kev.CtrlKey);
            }
            return result ?? new ConsoleKeyInfo();
        }

        public void SetCursorPosition(int left, int top)
        {
            CursorPosition = new Point(left, top);
        }

        public void Write(char ch)
        {
            CheckLineBreak();
            switch (ch)
            {
                case '\r': cursorPosition.X = 0; break;
                case '\n': cursorPosition.Y++; YScroll(); break;
                case '\t':
                    for (int x = cursorPosition.X; x < ((cursorPosition.X + 8) / 8) * 8 - 1; x++)
                        Write(' ');
                    var ccit = new ConsoleCharInfo() { fgr = ForegroundColor, bgr = BackgroundColor, ch = '\t' };
                    ScreenBuffer[cursorPosition.X + cursorPosition.Y * ConsoleSize.Width] = ccit;
                    cursorPosition.X++;
                    OnPropertyChanged(nameof(ScreenBuffer));
                    OnPropertyChanged(nameof(CursorPosition));
                    break;
                case '\x08':
                    cursorPosition.X = Math.Max(cursorPosition.X - 1, 0);
                    OnPropertyChanged(nameof(CursorPosition));
                    break;
                default:
                    var cci = new ConsoleCharInfo();
                    cci.ch = ch;
                    cci.fgr = ForegroundColor;
                    cci.bgr = BackgroundColor;
                    ScreenBuffer[cursorPosition.X + cursorPosition.Y * ConsoleSize.Width] = cci;
                    cursorPosition.X++;
                    OnPropertyChanged(nameof(ScreenBuffer));
                    if (DateTime.Now > lastUpdate.AddMilliseconds(15))
                        _DoUpdate();
                    break;
            }

            void CheckLineBreak()
            {
                if (cursorPosition.X >= ConsoleSize.Width)
                {
                    cursorPosition.X = 0;
                    cursorPosition.Y++;
                    OnPropertyChanged(nameof(CursorPosition));
                    YScroll();
                }
            }
        }

        public void Write(string? st)
        {
            foreach (char ch in st ?? "") Write(ch);
        }

        public void YScroll(bool force = false)
        {
            if (CursorPosition.Y >= ConsoleSize.Height || force)
            {
                for (var i = 0; i < ScreenBuffer.Length; i++)
                    ScreenBuffer[i] = i < ScreenBuffer.Length - ConsoleSize.Width ?
                        ScreenBuffer[i + ConsoleSize.Width] :
                        new ConsoleCharInfo();
                cursorPosition.Y--;
                OnPropertyChanged(nameof(CursorPosition));
                if (DateTime.Now > lastUpdate.AddMilliseconds(40))
                    _DoUpdate();
            }
        }

        private void _DoUpdate()
        {
            DoUpdate?.Invoke(this, new EventArgs());
        }
    }
}
