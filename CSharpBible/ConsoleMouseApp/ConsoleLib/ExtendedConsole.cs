using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleLib
{
    public static class ExtendedConsole
    {
        static ExtendedConsole()
        {

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
                        uint numRead = 0;
                        INPUT_RECORD[] record = new INPUT_RECORD[1];
                        record[0] = new INPUT_RECORD();
                        ReadConsoleInput(handleIn, record, 1, ref numRead);
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
                            uint numWritten = 0;
                            WriteConsoleInput(handleIn, record, 1, ref numWritten);
                            return;
                        }
                    }
                }).Start();
            }
        }
        // Native Input-Methods
        private static bool Run = false;

        public static void Stop() => Run = false;

        public static event EventHandler<MOUSE_EVENT_RECORD> MouseEvent;

        public static event EventHandler<KEY_EVENT_RECORD> KeyEvent;

        public static event EventHandler<WINDOW_BUFFER_SIZE_RECORD> WindowBufferSizeEvent;


        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y) => (X, Y) = (x, y);
            public Point AsPoint => new Point(X, Y);
        }

        [Flags]
        public enum EventType : ushort
        {
            KEY_EVENT = 0x0001,
            MOUSE_EVENT = 0x0002,
            WINDOW_BUFFER_SIZE_EVENT = 0x0004
        }; //more

        [Flags]
        public enum ButtonState : uint
        {
            FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
            FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
            FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
            FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
            RIGHTMOST_BUTTON_PRESSED = 0x0002
        };

        [Flags]
        public enum ControlKeyState : uint
        {
            CAPSLOCK_ON = 0x0080,
            ENHANCED_KEY = 0x0100,
            LEFT_ALT_PRESSED = 0x0002,
            LEFT_CTRL_PRESSED = 0x0008,
            NUMLOCK_ON = 0x0020,
            RIGHT_ALT_PRESSED = 0x0001,
            RIGHT_CTRL_PRESSED = 0x0004,
            SCROLLLOCK_ON = 0x0040,
            SHIFT_PRESSED = 0x0010
        };

        [Flags]
        public enum EventFlags : uint
        {
            DOUBLE_CLICK = 0x0002,
            MOUSE_HWHEELED = 0x0008,
            MOUSE_MOVED = 0x0001,
            MOUSE_WHEELED = 0x0004
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public EventType usEventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
        }

        [Flags]
        public enum Attributes :ushort {
            FOREGROUND_BLUE = 0x0001, // Text color contains blue.
            FOREGROUND_GREEN = 0x0002,  //Text color contains green.
            FOREGROUND_RED = 0x0004,    //Text color contains red.
            FOREGROUND_INTENSITY = 0x0008, //Text color is intensified.
            BACKGROUND_BLUE = 0x0010, //Background color contains blue.
            BACKGROUND_GREEN = 0x0020, //Background color contains green.
            BACKGROUND_RED = 0x0040, //Background color contains red.
            BACKGROUND_INTENSITY = 0x0080, //Background color is intensified.
            COMMON_LVB_LEADING_BYTE = 0x0100, //Leading byte.
            COMMON_LVB_TRAILING_BYTE = 0x0200,//Trailing byte.
            COMMON_LVB_GRID_HORIZONTAL = 0x0400, //Top horizontal
            COMMON_LVB_GRID_LVERTICAL = 0x0800, //Left vertical.
            COMMON_LVB_GRID_RVERTICAL = 0x1000, //Right vertical.
            COMMON_LVB_REVERSE_VIDEO = 0x4000, //Reverse foreground and background attribute.
            COMMON_LVB_UNDERSCORE = 0x8000 //Underscore.
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CHAR_INFO
        {
            [FieldOffset(0)]
            internal char UnicodeChar;
            [FieldOffset(0)]
            internal char AsciiChar;
            [FieldOffset(2)]
            internal Attributes usAttributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
            public SMALL_RECT(short x1, short y1, short x2, short y2) => (Left, Top, Right, Bottom) = (x1, y1, x2, y2);
            public Rectangle AsRectangle => new Rectangle(Left, Top,Right-Left,Bottom-Top);
            public COORD TopLeft { get => new COORD(Left, Top); set => (Left, Top) = (value.X, value.Y); }
            public COORD BottomRight { get => new COORD(Right, Bottom); set => (Right, Bottom) = (value.X, value.Y); }
        }

        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;

            public ButtonState dwButtonState;

            public ControlKeyState dwControlKeyState;

            public EventFlags dwEventFlags;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)]
            public bool bKeyDown;
            [FieldOffset(4)]
            public ushort wRepeatCount;
            [FieldOffset(6)]
            public ushort wVirtualKeyCode;
            [FieldOffset(8)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(10)]
            public byte AsciiChar;


            [FieldOffset(12)]
            public ControlKeyState dwControlKeyState;
        }
        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }


        public const uint STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);

        [Flags]
        public enum ConsoleMode : uint
        {
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008
        }; //more

        [DllImport("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleInput, out ConsoleMode lpMode);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleInput, ConsoleMode dwMode);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleOutput(IntPtr hConsoleInput, CHAR_INFO[,] lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleOutput(IntPtr hConsoleInput,[Out] CHAR_INFO[,] lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);
    }
}
