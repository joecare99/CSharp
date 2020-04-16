using System;
using System.Drawing;
using static ConsoleLib.NativeMethods;

namespace ConsoleLib.CommonControls
{
    public class Application : Panel
    {
        public Point MousePos { get; private set; }

        public Application()
        {
            ExtendedConsole.MouseEvent += HandleMouseEvent;
            ExtendedConsole.KeyEvent += HandleKeyEvent;
            ExtendedConsole.WindowBufferSizeEvent += HandleWinBufEvent;
            Boarder = new Char[] { };
        }

        private void HandleWinBufEvent(object sender, WINDOW_BUFFER_SIZE_RECORD e)
        {
            throw new NotImplementedException();
        }

        private void HandleKeyEvent(object sender, KEY_EVENT_RECORD e)
        {
            // Determine the Control to send the Event to
            if (e.bKeyDown)
            { }
            else
            { };     

        }

        private void HandleMouseEvent(object sender, MOUSE_EVENT_RECORD e)
        {
            if (e.dwEventFlags == EventFlags.MOUSE_MOVED)
            {
                Point lastMousePos = MousePos;
                MousePos = e.dwMousePosition.AsPoint;
                // Invoke Mouse Leave
                // Invoke Mouse Enter
            }
        }

        public virtual void Initialize()
        {

        }


    }
}
