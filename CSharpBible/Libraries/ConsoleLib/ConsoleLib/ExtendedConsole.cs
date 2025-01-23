// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="ExtendedConsole.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static ConsoleLib.NativeMethods;

namespace ConsoleLib
{
    /// <summary>
    /// Class ExtendedConsole.
    /// </summary>
    public static class ExtendedConsole
    /// <summary>
    /// The days
    /// </summary>
    {
        /// <summary>
        /// The hours
        /// </summary>
        static ExtendedConsole()
        /// <summary>
        /// The minutes
        /// </summary>
        {
            /// <summary>
            /// The seconds
            /// </summary>
            Console.WindowHeight = 50;
            Console.BufferHeight = 50;
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(inHandle, out ConsoleMode mode);
            mode &= ~ConsoleMode.ENABLE_QUICK_EDIT_MODE; //disable
            mode |= ConsoleMode.ENABLE_WINDOW_INPUT; //enable (if you want)
            mode |= ConsoleMode.ENABLE_MOUSE_INPUT; //enable
            SetConsoleMode(inHandle, mode);
            if (!Run)
            {
                Run = true;
                IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
                new Thread(() =>
                {
                    while (true)
                    {
                        /// <summary>
                        /// The the pear
                        /// </summary>
                        INPUT_RECORD[] record = new INPUT_RECORD[1];
                        /// <summary>
                        /// The the cherry
                        /// </summary>
                        record[0] = new INPUT_RECORD();
                        /// <summary>
                        /// The the raspberry
                        /// </summary>
                        ReadConsoleInput(handleIn, record, 1, out uint numRead);
                        if (Run)
                            switch (record[0].usEventType)
                            {
                                case EventType.MOUSE_EVENT:
                                    MouseEvent?.Invoke(null, record[0].MouseEvent);
                                    break;
                                case EventType.KEY_EVENT:
                                    KeyEvent?.Invoke(null, record[0].KeyEvent);
                                    break;
                                case EventType.WINDOW_BUFFER_SIZE_EVENT:
                                    WindowBufferSizeEvent?.Invoke(null, record[0].WindowBufferSizeEvent);
                                    break;
                                default:
                                    Thread.Sleep(0);
                                    break;
                            }
                        else
                        {
                            WriteConsoleInput(handleIn, record, 1, out uint numWritten);
                            return;
                        }
                    }
                }).Start();
            }
        }
        /// <summary>
        /// The run
        /// </summary>
        private static bool Run = false;

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public static void Stop() => Run = false;

        /// <summary>Occurs when a mouse event happend.</summary>
        public static event EventHandler<MOUSE_EVENT_RECORD>? MouseEvent;

        /// <summary>
        ///  Occurs when a key event happend.
        /// </summary>
        public static event EventHandler<KEY_EVENT_RECORD>? KeyEvent;

        /// <summary>Occurs when a window buffer size change event happend.</summary>
        public static event EventHandler<WINDOW_BUFFER_SIZE_RECORD>? WindowBufferSizeEvent;
    }


    /// <summary>
    /// Native Input and Output -Methods
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// Interface structure for coordinates
        /// </summary>
        public struct COORD
        {
            /// <summary>
            /// The x
            /// </summary>
            public short X;
            /// <summary>
            /// The y
            /// </summary>
            public short Y;

            /// <summary>
            /// Initializes a new instance of the <see cref="COORD" /> struct with 2 values as coordinates.
            /// </summary>
            /// <param name="x">The x coordinate.</param>
            /// <param name="y">The y coordinate.</param>
            public COORD(short x, short y) => (X, Y) = (x, y);
            /// <summary>
            /// Gets the value as a 'Point'.
            /// </summary>
            /// <value>As point.</value>
            public Point AsPoint => new(X, Y);
        }

        /// <summary>
        /// Structure to represent a small rectangle with 'short'-values
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            /// <summary>
            /// The left
            /// </summary>
            public short Left;
            /// <summary>
            /// The top
            /// </summary>
            public short Top;
            /// <summary>
            /// The right
            /// </summary>
            public short Right;
            /// <summary>
            /// The bottom
            /// </summary>
            public short Bottom;
            /// <summary>
            /// Initializes a new instance of the <see cref="SMALL_RECT" /> struct with 4 values as coordinates for the upper left and lower right corner.
            /// </summary>
            /// <param name="x1">The x1.</param>
            /// <param name="y1">The y1.</param>
            /// <param name="x2">The x2.</param>
            /// <param name="y2">The y2.</param>
            public SMALL_RECT(short x1, short y1, short x2, short y2) => (Left, Top, Right, Bottom) = (x1, y1, x2, y2);
            /// <summary>
            /// Initializes a new instance of the <see cref="SMALL_RECT" /> struct with 2 <see cref="COORD" /> values.
            /// </summary>
            /// <param name="topLeft">The top left corner.</param>
            /// <param name="bottomRight">The bottom right corner.</param>
            public SMALL_RECT(COORD topLeft, COORD bottomRight) => (Left, Top, Right, Bottom) = (topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
            /// <summary>
            /// Converts the <see cref="SMALL_RECT" /> to a <see cref="Rectangle" />.
            /// </summary>
            /// <value>As <see cref="Rectangle" />.</value>
            public Rectangle AsRectangle => new(Left, Top, Right - Left, Bottom - Top);
            /// <summary>
            /// Gets or sets the top left.
            /// </summary>
            /// <value>The top left.</value>
            public COORD TopLeft { get => new(Left, Top); set => (Left, Top) = (value.X, value.Y); }
            /// <summary>
            /// Gets or sets the bottom right.
            /// </summary>
            /// <value>The bottom right.</value>
            public COORD BottomRight { get => new(Right, Bottom); set => (Right, Bottom) = (value.X, value.Y); }
        }


        /// <summary>
        /// EventType<br />A handle to the type of (<a href="Console">Console</a>)-input event and the event record stored in the Event member.
        /// </summary>
        /// <example>
        /// For an example, see <a href="https://docs.microsoft.com/en-us/windows/console/reading-input-buffer-events" data-linktype="relative-path"><u><font color="#0066cc">Reading Input Buffer Events</font></u></a>.
        /// </example>
        /// <seealso cref="ReadConsoleInput" />
        /// <seealso cref="WriteConsoleInput" />
        [Flags]
        public enum EventType : ushort
        {
            /// <summary>
            ///   The <strong>Event</strong> member contains a <a href="https://docs.microsoft.com/en-us/windows/console/focus-event-record-str"><strong><u><font color="#0066cc">FOCUS_EVENT_RECORD</font></u></strong></a> structure. These events are used internally and should be ignored.
            /// </summary>            
            FOCUS_EVENT = 0x0010,
            /// <summary>
            /// The <strong>Event</strong> member contains a <a href="https://docs.microsoft.com/en-us/windows/console/key-event-record-str"><strong><u><font color="#0066cc">KEY_EVENT_RECORD</font></u></strong></a> structure with information about a keyboard event.
            /// </summary>
            KEY_EVENT = 0x0001,
            /// <summary>
            /// The <strong>Event</strong> member contains a <a href="https://docs.microsoft.com/en-us/windows/console/menu-event-record-str"><strong><u><font color="#0066cc">MENU_EVENT_RECORD</font></u></strong></a> structure. These events are used internally and should be ignored.
            /// </summary>
            MENU_EVENT = 0x0008,
            /// <summary>
            /// The <strong>Event</strong> member contains a <a href="https://docs.microsoft.com/en-us/windows/console/mouse-event-record-str"><strong><u><font color="#0066cc">MOUSE_EVENT_RECORD</font></u></strong></a> structure with information about a mouse movement or button press event.
            /// </summary>
            MOUSE_EVENT = 0x0002,
            /// <summary>
            /// The <strong>Event</strong> member contains a <a href="https://docs.microsoft.com/en-us/windows/console/window-buffer-size-record-str"><strong><u><font color="#0066cc">WINDOW_BUFFER_SIZE_RECORD</font></u></strong></a> structure with information about the new size of the console screen buffer
            /// </summary>
            WINDOW_BUFFER_SIZE_EVENT = 0x0004
        }; //more

    /// <summary>
    /// State of the mouse-buttons
    /// </summary>
    [Flags]
    public enum ButtonState : uint
    {
        /// <summary>
        /// The 1st button from left is pressed
        /// </summary>
        FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
        /// <summary>
        /// The 2nd button from left is pressed
        /// </summary>
        FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
        /// <summary>
        /// The 3rd button from left is pressed
        /// </summary>
        FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
        /// <summary>
        /// The 4th button from left is pressed
        /// </summary>
        FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
        /// <summary>
        /// The rightmost button is pressed
        /// </summary>
        RIGHTMOST_BUTTON_PRESSED = 0x0002
    };

    /// <summary>
    /// The state of the control keys. This member can be one or more of the following values.
    /// </summary>
        [Flags]
        public enum ControlKeyState : uint
        {
            /// <summary>  The CAPS LOCK light is on.</summary>
            CAPSLOCK_ON = 0x0080,
            /// <summary>
            /// The key is enhanced.
            /// </summary>
            ENHANCED_KEY = 0x0100,
            /// <summary>
            /// The left ALT key is pressed.
            /// </summary>
            LEFT_ALT_PRESSED = 0x0002,
            /// <summary>
            /// The left CTRL key is pressed.
            /// </summary>
            LEFT_CTRL_PRESSED = 0x0008,
            /// <summary>
            /// The NUM LOCK light is on.
            /// </summary>
            NUMLOCK_ON = 0x0020,
            /// <summary>
            /// The right ALT key is pressed.
            /// </summary>
            RIGHT_ALT_PRESSED = 0x0001,
            /// <summary>
            /// The right CTRL key is pressed.
            /// </summary>
            RIGHT_CTRL_PRESSED = 0x0004,
            /// <summary>
            /// Scroll-Lock is on.
            /// </summary>
            SCROLLLOCK_ON = 0x0040,
            /// <summary>
            /// The SHIFT key is pressed.
            /// </summary>
            SHIFT_PRESSED = 0x0010
        };

        /// <summary>
        /// The type of mouse event. If this value is zero, it indicates a mouse button being pressed or released. Otherwise, this member is one of the following values.
        /// </summary>
        [Flags]
        public enum EventFlags : uint
        {
            /// <summary>
            /// The second click (button press) of a double-click occurred. The first click is returned as a regular button-press event.
            /// </summary>
            DOUBLE_CLICK = 0x0002,
            /// <summary>
            /// The horizontal mouse wheel was moved.
            /// <para>If the high word of the <strong>dwButtonState</strong> member contains a positive value, the wheel was rotated to the right. Otherwise, the wheel was rotated to the left.</para>
            /// </summary>
            MOUSE_HWHEELED = 0x0008,
            /// <summary>
            /// A change in mouse position occurred.
            /// </summary>
            MOUSE_MOVED = 0x0001,
            /// <summary>
            /// The vertical mouse wheel was moved.
            /// <para>If the high word of the <strong>dwButtonState</strong> member
            /// contains a positive value, the wheel was rotated forward, away from the
            /// user. Otherwise, the wheel was rotated backward, toward the user.</para>
            /// </summary>
            MOUSE_WHEELED = 0x0004
        };

        /// <summary>
        /// Describes an input event in the console input buffer. These records can be read from the input buffer by using the <a href="https://docs.microsoft.com/en-us/windows/console/readconsoleinput" data-linktype="relative-path"><strong><u><font color="#0066cc">ReadConsoleInput</font></u></strong></a> or <a href="https://docs.microsoft.com/en-us/windows/console/peekconsoleinput" data-linktype="relative-path"><strong><u><font color="#0066cc">PeekConsoleInput</font></u></strong></a> function, or written to the input buffer by using the <a href="https://docs.microsoft.com/en-us/windows/console/writeconsoleinput" data-linktype="relative-path"><strong><u><font color="#0066cc">WriteConsoleInput</font></u></strong></a> function.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            /// <summary>
            /// The us event type
            /// </summary>
            [FieldOffset(0)]
            public EventType usEventType;
            /// <summary>
            /// The key event
            /// </summary>
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            /// <summary>
            /// The mouse event
            /// </summary>
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            /// <summary>
            /// The window buffer size event
            /// </summary>
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
        }

        /// <summary>
        /// Enum DateDifFormat
        /// </summary>
        [Flags]
        public enum Attributes :ushort {
            /// <summary>
            /// The foreground blue
            /// </summary>
            FOREGROUND_BLUE = 0x0001, // Text color contains blue.
            /// <summary>
            /// The foreground green
            /// </summary>
            FOREGROUND_GREEN = 0x0002,  //Text color contains green.
            /// <summary>
            /// The foreground red
            /// </summary>
            FOREGROUND_RED = 0x0004,    //Text color contains red.
            /// <summary>
            /// The foreground intensity
            /// </summary>
            FOREGROUND_INTENSITY = 0x0008, //Text color is intensified.
            /// <summary>
            /// The background blue
            /// </summary>
            BACKGROUND_BLUE = 0x0010, //Background color contains blue.
            /// <summary>
            /// The background green
            /// </summary>
            BACKGROUND_GREEN = 0x0020, //Background color contains green.
            /// <summary>
            /// The background red
            /// </summary>
            BACKGROUND_RED = 0x0040, //Background color contains red.
            /// <summary>
            /// The background intensity
            /// </summary>
            BACKGROUND_INTENSITY = 0x0080, //Background color is intensified.
            /// <summary>
            /// The common LVB leading byte
            /// </summary>
            COMMON_LVB_LEADING_BYTE = 0x0100, //Leading byte.
            /// <summary>
            /// The common LVB trailing byte
            /// </summary>
            COMMON_LVB_TRAILING_BYTE = 0x0200,//Trailing byte.
            /// <summary>
            /// The common LVB grid horizontal
            /// </summary>
            COMMON_LVB_GRID_HORIZONTAL = 0x0400, //Top horizontal
            /// <summary>
            /// The common LVB grid lvertical
            /// </summary>
            COMMON_LVB_GRID_LVERTICAL = 0x0800, //Left vertical.
            /// <summary>
            /// The common LVB grid rvertical
            /// </summary>
            COMMON_LVB_GRID_RVERTICAL = 0x1000, //Right vertical.
            /// <summary>
            /// The common LVB reverse video
            /// </summary>
            COMMON_LVB_REVERSE_VIDEO = 0x4000, //Reverse foreground and background attribute.
            /// <summary>
            /// The common LVB underscore
            /// </summary>
            COMMON_LVB_UNDERSCORE = 0x8000 //Underscore.
        }

        /// <summary>
        /// Struct CHAR_INFO
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct CHAR_INFO
        {
            /// <summary>
            /// The unicode character
            /// </summary>
            [FieldOffset(0)]
            internal char UnicodeChar;
            /// <summary>
            /// The ASCII character
            /// </summary>
            [FieldOffset(0)]
            internal char AsciiChar;
            /// <summary>
            /// The us attributes
            /// </summary>
            [FieldOffset(2)]
            internal Attributes usAttributes;
        }

        /// <summary>
        /// Struct MOUSE_EVENT_RECORD
        /// </summary>
        public struct MOUSE_EVENT_RECORD
        {
            /// <summary>
            /// The dw mouse position
            /// </summary>
            public COORD dwMousePosition;

            /// <summary>
            /// The dw button state
            /// </summary>
            public ButtonState dwButtonState;

            /// <summary>
            /// The dw control key state
            /// </summary>
            public ControlKeyState dwControlKeyState;

            /// <summary>
            /// The dw event flags
            /// </summary>
            public EventFlags dwEventFlags;

            /// <summary>
            /// Gets as mouse event arguments.
            /// </summary>
            /// <value>As mouse event arguments.</value>
            public MouseEventArgs AsMouseEventArgs 
            { 
                get
                {
                    MouseButtons btn = default;
                    if (dwButtonState == ButtonState.FROM_LEFT_1ST_BUTTON_PRESSED)
                        btn |= MouseButtons.Left;
                    if (dwButtonState == ButtonState.FROM_LEFT_2ND_BUTTON_PRESSED)
                        btn |= MouseButtons.Middle;
                    if (dwButtonState == ButtonState.RIGHTMOST_BUTTON_PRESSED)
                        btn |= MouseButtons.Right;
                    return new MouseEventArgs(btn, 1, dwMousePosition.X, dwMousePosition.Y, 0);
                }  
            }
        }

        /// <summary>
        /// Struct KEY_EVENT_RECORD
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            /// <summary>
            /// If the key is pressed, this member is <strong>TRUE</strong>. Otherwise, this member is <strong>FALSE</strong> (the key is released).
            /// </summary>
            [FieldOffset(0)]
            public bool bKeyDown;
            /// <summary>
            /// The repeat count, which indicates that a key is being held down. For
            /// example, when a key is held down, you might get five events with this
            /// member equal to 1, one event with this member equal to 5, or multiple
            /// events with this member greater than or equal to 1.
            /// </summary>
            [FieldOffset(4)]
            public ushort wRepeatCount;
            /// <summary>
            /// A <a href="https://msdn.microsoft.com/library/windows/desktop/dd375731(v=vs.85).aspx" data-linktype="external"><u><font color="#0066cc">virtual-key code</font></u></a> that identifies the given key in a device-independent manner.
            /// </summary>
            [FieldOffset(6)]
            public ushort wVirtualKeyCode;
            /// <summary>
            /// The virtual scan code of the given key that represents the device-dependent value generated by the keyboard hardware.
            /// </summary>
            [FieldOffset(8)]
            public ushort wVirtualScanCode;
            /// <summary>
            /// Translated Unicode character.
            /// </summary>
            [FieldOffset(10)]
            public char UnicodeChar;
            /// <summary>
            /// Translated ASCII character.
            /// </summary>
            [FieldOffset(10)]
            public byte AsciiChar;


            /// <summary>
            /// The state of the control keys. This member can be one or more of the following values.
            /// </summary>
            [FieldOffset(12)]
            public ControlKeyState dwControlKeyState;
        }
        /// <summary>
        /// Struct WINDOW_BUFFER_SIZE_RECORD
        /// </summary>
        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            /// <summary>
            /// The dw size
            /// </summary>
            public COORD dwSize;
        }


        /// <summary>
        /// The standard input handle
        /// </summary>
        public const uint STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12);

        /// <summary>
        /// Gets the standard handle.
        /// </summary>
        /// <param name="nStdHandle">The n standard handle.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);

        /// <summary>
        /// The input or output mode to be set. If the <em>hConsoleHandle</em> 
        /// parameter is an input handle, the mode can be one or more of the 
        /// following values. When a console is created, all input modes except <strong>ENABLE_WINDOW_INPUT</strong> are enabled by default.
        /// </summary>
        /// <seealso cref="SetConsoleMode">SetConsoleMode function</seealso>
        /// <seealso cref="GetConsoleMode">GetConsoleMode function</seealso>
        /// <summary>
        /// Enum TestEnum
        /// </summary>
        [Flags]
        public enum ConsoleMode : uint
        {
            /// <summary>
            /// If the mouse pointer is within the borders of the console window and the
            /// window has the keyboard focus, mouse events generated by mouse movement
            /// and button presses are placed in the input buffer. These events are
            /// discarded by <a href="https://msdn.microsoft.com/library/windows/desktop/aa365467"><strong><u><font color="#0066cc">ReadFile</font></u></strong></a> or <a href="https://docs.microsoft.com/en-us/windows/console/readconsole"><strong><u><font color="#0066cc">ReadConsole</font></u></strong></a>, even when this mode is enabled.
            /// </summary>
            ENABLE_MOUSE_INPUT = 0x0010,
            /// <summary>
            /// This flag enables the user to use the mouse to select and edit text.
            /// <para>To enable this mode, use <code>ENABLE_QUICK_EDIT_MODE | ENABLE_EXTENDED_FLAGS</code>. To disable this mode, use <strong>ENABLE_EXTENDED_FLAGS</strong> without this flag.</para>
            /// </summary>
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            /// <summary>
            /// Required to enable or disable extended flags. See <strong>ENABLE_INSERT_MODE</strong> and <strong>ENABLE_QUICK_EDIT_MODE</strong>.
            /// </summary>
            ENABLE_EXTENDED_FLAGS = 0x0080,
            /// <summary>
            /// Characters read by the <a href="https://msdn.microsoft.com/library/windows/desktop/aa365467"><strong><u><font color="#0066cc">ReadFile</font></u></strong></a> or <a href="https://docs.microsoft.com/en-us/windows/console/readconsole"><strong><u><font color="#0066cc">ReadConsole</font></u></strong></a> function are written to the active screen buffer as they are read. This mode can be used only if the <strong>ENABLE_LINE_INPUT</strong> mode is also enabled.
            /// </summary>
            ENABLE_ECHO_INPUT = 0x0004,
            /// <summary>
            /// User interactions that change the size of the console screen
            /// buffer are reported in the console's input buffer. Information about
            /// these events can be read from the input buffer by applications using the
            /// <a href="https://docs.microsoft.com/en-us/windows/console/readconsoleinput"><strong><u><font color="#0066cc">ReadConsoleInput</font></u></strong></a>
            /// function, but not by those using
            /// <a href="https://msdn.microsoft.com/library/windows/desktop/aa365467"><strong><u><font color="#0066cc">ReadFile</font></u></strong></a>
            /// or
            /// <a href="https://docs.microsoft.com/en-us/windows/console/readconsole"><strong><u><font color="#0066cc">ReadConsole</font></u></strong></a>
            /// </summary>
            ENABLE_WINDOW_INPUT = 0x0008
        }; //more

        /// <summary>
        /// Retrieves the current input mode of a console's input buffer or the current output mode of a console screen buffer.
        /// </summary>
        /// <param name="hConsoleInput">A handle to the console input buffer or the console screen buffer. The handle must have the <strong>GENERIC_READ</strong> access right. For more information, see <a href="https://docs.microsoft.com/en-us/windows/console/console-buffer-security-and-access-rights"><u><font color="#0066cc">Console Buffer Security and Access Rights</font></u></a>.</param>
        /// <param name="lpMode">A pointer to a variable that receives the current mode of the specified buffer.</param>
        /// <returns><para>If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero. To get extended error information, call <a href="https://msdn.microsoft.com/library/windows/desktop/ms679360" data-linktype="external"><strong><u><font color="#0066cc">GetLastError</font></u></strong></a>.</para></returns>
        /// <example>For an example, see <see cref="https://docs.microsoft.com/en-us/windows/console/reading-input-buffer-events">Reading Input Buffer Events</see>.</example>
        /// <seealso cref="https://docs.microsoft.com/en-us/windows/console/getconsolemode">GetConsoleMode Console API</seealso>
        /// <seealso cref="https://docs.microsoft.com/en-us/windows/console/console-functions">Console Functions</seealso>
        [DllImport("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleInput, out ConsoleMode lpMode);

        /// <summary>
        /// Sets the input mode of a console's input buffer or the output mode of a console screen buffer.
        /// </summary>
        /// <param name="hConsoleInput">A handle to the console input buffer or a console screen buffer. The handle must have the <strong>GENERIC_READ</strong> access right. For more information, see <a href="https://docs.microsoft.com/en-us/windows/console/console-buffer-security-and-access-rights" data-linktype="relative-path"><u><font color="#0066cc">Console Buffer Security and Access Rights</font></u></a>.</param>
        /// <param name="dwMode">The input or output mode to be set. If the hConsoleHandle parameter is an input handle, the mode can be one or more of the following values. When a console is created, all input modes except ENABLE_WINDOW_INPUT are enabled by default.</param>
        /// <returns><para>If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero. To get extended error information, call GetLastError.</para></returns>
        /// <example>
        /// For an example, see <a href="https://docs.microsoft.com/en-us/windows/console/reading-input-buffer-events" data-linktype="relative-path"><u><font color="#0066cc">Reading Input Buffer Events</font></u></a>.
        /// </example>
        /// <seealso cref="https://docs.microsoft.com/en-us/windows/console/setconsolemode">SetConsoleMode MS API-documentation</seealso>
        /// <remarks>A console consists of an input buffer and one or more screen buffers.
        /// The mode of a console buffer determines how the console behaves during
        /// input and output (I/O) operations. One set of flag constants is used
        /// with input handles, and another set is used with screen buffer (output)
        /// handles. Setting the output modes of one screen buffer does not affect
        /// the output modes of other screen buffers.</remarks>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleInput, ConsoleMode dwMode);

        /// <summary>
        /// Reads the console input.
        /// </summary>
        /// <param name="hConsoleInput">A handle to the console input buffer. The handle must have the <strong>GENERIC_READ</strong> access right. For more information, see <a href="https://docs.microsoft.com/en-us/windows/console/console-buffer-security-and-access-rights" data-linktype="relative-path"><u><font color="#0066cc">Console Buffer Security and Access Rights</font></u></a>.</param>
        /// <param name="lpBuffer">A pointer to an array of <a href="https://docs.microsoft.com/en-us/windows/console/input-record-str" data-linktype="relative-path"><strong><u><font color="#0066cc">INPUT_RECORD</font></u></strong></a> structures that receives the input buffer data.</param>
        /// <param name="nLength">The size of the array pointed to by the <em>lpBuffer</em> parameter, in array elements.</param>
        /// <param name="lpNumberOfEventsRead">Variable that receives the number of input records read.</param>
        /// <returns><para>If the function succeeds, the return value is nonzero.</para>
        /// <para>If the function fails, the return value is zero. To get extended error information, call <a href="https://msdn.microsoft.com/library/windows/desktop/ms679360" data-linktype="external"><strong><u><font color="#0066cc">GetLastError</font></u></strong></a>.</para></returns>
        /// <example>
        /// For an example, see <a href="https://docs.microsoft.com/en-us/windows/console/reading-input-buffer-events" data-linktype="relative-path"><u><font color="#0066cc">Reading Input Buffer Events</font></u></a>.
        /// </example>
        /// <seealso cref="ReadConsoleInput" />
        /// <remarks><para>If the number of records requested in the <em>nLength</em> parameter
        /// exceeds the number of records available in the buffer, the number
        /// available is read. The function does not return until at least one input
        /// record has been read.</para>
        /// <para>A process can specify a console input buffer handle in one of the <a href="https://msdn.microsoft.com/library/windows/desktop/ms687069" data-linktype="external"><u><font color="#0066cc">wait functions</font></u></a>
        /// to determine when there is unread console input. When the input buffer
        /// is not empty, the state of a console input buffer handle is signaled.</para>
        /// <para>To determine the number of unread input records in a console's input buffer, use the <a href="https://docs.microsoft.com/en-us/windows/console/getnumberofconsoleinputevents" data-linktype="relative-path"><strong><u><font color="#0066cc">GetNumberOfConsoleInputEvents</font></u></strong></a> function. To read input records from a console input buffer without affecting the number of unread records, use the <a href="https://docs.microsoft.com/en-us/windows/console/peekconsoleinput" data-linktype="relative-path"><strong><u><font color="#0066cc">PeekConsoleInput</font></u></strong></a> function. To discard all unread records in a console's input buffer, use the <a href="https://docs.microsoft.com/en-us/windows/console/flushconsoleinputbuffer" data-linktype="relative-path"><strong><u><font color="#0066cc">FlushConsoleInputBuffer</font></u></strong></a> function.</para>
        /// <para>This function uses either Unicode characters or 8-bit characters from
        /// the console's current code page. The console's code page defaults
        /// initially to the system's OEM code page. To change the console's code
        /// page, use the <a href="https://docs.microsoft.com/en-us/windows/console/setconsolecp" data-linktype="relative-path"><strong><u><font color="#0066cc">SetConsoleCP</font></u></strong></a> or <a href="https://docs.microsoft.com/en-us/windows/console/setconsoleoutputcp" data-linktype="relative-path"><strong><u><font color="#0066cc">SetConsoleOutputCP</font></u></strong></a> functions, or use the <strong>chcp</strong> or <strong>mode con cp select=</strong> commands.</para></remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

        /// <summary>
        /// Writes the console input.
        /// </summary>
        /// <param name="hConsoleInput">The h console input.</param>
        /// <param name="lpBuffer">The lp buffer.</param>
        /// <param name="nLength">Length of the n.</param>
        /// <param name="lpNumberOfEventsWritten">The lp number of events written.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsWritten);

        /// <summary>
        /// Writes to the console output.
        /// </summary>
        /// <param name="hConsoleInput">The console input.</param>
        /// <param name="lpBuffer">The buffer.</param>
        /// <param name="dwBufferSize">Size of the buffer.</param>
        /// <param name="dwBufferCoord">The buffer coordinate.</param>
        /// <param name="lpWriteRegion">The write region.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<para />If the function fails, the return value is zero. To get extended error information, call <see cref="GetLastError" />.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleOutput(IntPtr hConsoleInput, CHAR_INFO[,] lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);

        /// <summary>
        /// Reads from the console output.
        /// </summary>
        /// <param name="hConsoleInput">The console input.</param>
        /// <param name="lpBuffer">The buffer.</param>
        /// <param name="dwBufferSize">Size of the buffer.</param>
        /// <param name="dwBufferCoord">The buffer coordinate.</param>
        /// <param name="lpWriteRegion">The write region.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<para />If the function fails, the return value is zero. To get extended error information, call <see cref="GetLastError" />.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleOutput(IntPtr hConsoleInput,[Out] CHAR_INFO[,] lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);
    }
}
