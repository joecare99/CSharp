﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestConsole.View
{
    /// <summary>
    /// Struct ConsoleCharInfo
    /// </summary>
    public struct ConsoleCharInfo
    {
        /// <summary>
        /// The ch
        /// </summary>
        public char ch='\x00';
        /// <summary>
        /// The FGR
        /// </summary>
        public ConsoleColor fgr = ConsoleColor.Gray;
        /// <summary>
        /// The BGR
        /// </summary>
        public ConsoleColor bgr = ConsoleColor.Black;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCharInfo"/> struct.
        /// </summary>
        public ConsoleCharInfo(){}
    }

    /// <summary>
    /// Class TestConsoleForm.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class TestConsoleForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestConsoleForm"/> class.
        /// </summary>
        public TestConsoleForm()
        {
            InitializeComponent();
            foreach (var c in typeof(ConsoleColor).GetEnumValues())
                ccolors[(int)c] = Color.FromArgb(((int)c / 4) % 2 * (1 + ((int)c / 8) * 2) * 85, ((int)c / 2) % 2 * (1 + ((int)c / 8) * 2) * 85, ((int)c % 2 * (1 + ((int)c / 8) * 2)) * 85);
            Write("Testconsole Ver. 1.0\r\n");
        }


        protected ConsoleCharInfo[] ScreenBuffer = new ConsoleCharInfo[80 * 25];
        protected ConsoleCharInfo[] OutBuffer = new ConsoleCharInfo[80 * 25];
        protected Size ConsoleSize = new Size(80, 25);

        private Color[] ccolors = new Color[typeof(ConsoleColor).GetEnumValues().Length];
        private List<KeyPressEventArgs> keyPressEventArgs = new List<KeyPressEventArgs>();
        private Point cursorPosition;

        /// <summary>
        /// Reads the key.
        /// </summary>
        /// <returns>ConsoleKeyInfo.</returns>
        internal ConsoleKeyInfo ReadKey()
        {
            while (keyPressEventArgs.Count == 0)
            {
                Application.DoEvents();
                //   Application.RaiseIdle(new EventArgs());
                Thread.Sleep(1);
            }
            ConsoleKeyInfo? result = null;
            if (keyPressEventArgs.Count > 0)
            {
                var kev = keyPressEventArgs[0];
                keyPressEventArgs.RemoveAt(0);
                result = new ConsoleKeyInfo(kev.KeyChar, ConsoleKey.NoName, false, false, false);
            }
            return result ?? new ConsoleKeyInfo();
        }
        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        internal (int Left, int Top) GetCursorPosition() => (cursorPosition.X, cursorPosition.Y);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal void Clear()
        {
            ScreenBuffer = new ConsoleCharInfo[80 * 25];
            cursorPosition = new Point(0, 0);
            pictureBox1.Invalidate();
        }
        /// <summary>
        /// Writes the specified ch.
        /// </summary>
        /// <param name="ch">The ch.</param>
        internal void Write(char ch)
        {
            CheckLineBreak();
            switch (ch)
            {
                case '\r': cursorPosition.X = 0; break;
                case '\n': cursorPosition.Y++; YScroll(); break;
                case '\t':
                    for (int x = cursorPosition.X; x < ((cursorPosition.X + 8) / 8) * 8-1; x++)
                        Write(' ');
                    var ccit = new ConsoleCharInfo() {fgr=foregroundColor,bgr=backgroundColor,ch='\t' };
                    ScreenBuffer[cursorPosition.X + cursorPosition.Y * ConsoleSize.Width] = ccit;
                    cursorPosition.X++;
                    pictureBox1?.Invalidate();
                    break;
                case '\x08':
                    cursorPosition.X = Math.Max(cursorPosition.X - 1, 0);
                    break;
                default:
                    var cci = new ConsoleCharInfo();
                    cci.ch = ch;
                    cci.fgr = foregroundColor;
                    cci.bgr = backgroundColor;
                    ScreenBuffer[cursorPosition.X + cursorPosition.Y * ConsoleSize.Width] = cci;
                    cursorPosition.X++;
                    pictureBox1?.Invalidate();
                    if (DateTime.Now > lastUpdate.AddMilliseconds(15))
                        DoUpdate();
                    break;
            }

            void CheckLineBreak()
            {
                if (cursorPosition.X >= ConsoleSize.Width)
                {
                    cursorPosition.X = 0;
                    cursorPosition.Y++;
                    YScroll();
                }
            }

            void YScroll(bool force = false)
            {
                if (cursorPosition.Y >= ConsoleSize.Height || force)
                {
                    for (var i = 0; i < ScreenBuffer.Length; i++)
                        ScreenBuffer[i] = i < ScreenBuffer.Length - ConsoleSize.Width ?
                            ScreenBuffer[i + ConsoleSize.Width] :
                            new ConsoleCharInfo();
                    cursorPosition.Y--;
                    if (DateTime.Now > lastUpdate.AddMilliseconds(40))
                        DoUpdate();
                }
            }
        }

        /// <summary>
        /// Writes the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        internal void Write(string? st)
        {
            foreach (char ch in st ?? "") Write(ch);
        }
        /// <summary>
        /// The foreground color
        /// </summary>
        internal ConsoleColor foregroundColor = ConsoleColor.Gray;
        /// <summary>
        /// The background color
        /// </summary>
        internal ConsoleColor backgroundColor = ConsoleColor.Black;
        /// <summary>
        /// Gets or sets the width of the window.
        /// </summary>
        /// <value>The width of the window.</value>
        internal int WindowWidth
        {
            get => ConsoleSize.Width;
            set
            {
                ConsoleSize.Width = value;
                // Todo: Adjust Screenbuffer & OutBuffer   
            }
        }
        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        /// <value>The height of the window.</value>
        internal int WindowHeight
        {
            get => ConsoleSize.Height;
            set
            {
                ConsoleSize.Height = value;
                // Todo: Adjust Screenbuffer & OutBuffer
            }
        }

        private DateTime lastUpdate;

        /// <summary>
        /// Gets a value indicating whether [key available].
        /// </summary>
        /// <value><c>true</c> if [key available]; otherwise, <c>false</c>.</value>
        internal bool KeyAvailable { get => keyPressEventArgs.Count>0; }
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get
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
                    if ( y < ConsoleSize.Height - 1)
                        result += "\r\n";
                }
                
                return result.TrimEnd();
            }  
        }

        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal void SetCursorPosition(int left, int top)
        {
            cursorPosition = new Point(left, top);
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Font f =new Font("Consolas",14);
            var fh=f.GetHeight(e.Graphics)-1;
            var fw = fh/2-1;
            for (int i = 0; i < ScreenBuffer.Length; i++)
            {
                e.Graphics.FillRectangle(new SolidBrush(ccolors[(int)ScreenBuffer[i].bgr]), (i % ConsoleSize.Width) * fw+2, (i / ConsoleSize.Width) * fh, fw, fh);
                Brush brush = new SolidBrush(ccolors[(int)ScreenBuffer[i].fgr]);
                e.Graphics.DrawString($"{ScreenBuffer[i].ch}", f, brush, (i % ConsoleSize.Width) * fw-2, (i / ConsoleSize.Width) * fh);
            }
            lastUpdate = DateTime.Now;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void TestConsoleForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            keyPressEventArgs.Add(e);
        }

        private void DoUpdate()
        {            
                Application.DoEvents();       
        }
    }
}
