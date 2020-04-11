using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ConsoleMouseApp.
{
    class ExtendedConsole
    {
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

    }
}
