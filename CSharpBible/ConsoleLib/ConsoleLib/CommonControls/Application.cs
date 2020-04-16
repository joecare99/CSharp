using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static ConsoleLib.NativeMethods;

namespace ConsoleLib.CommonControls
{
    public class Application : Panel
    {
        public Point MousePos { get; private set; }
        public bool running { get; private set; }

        private MouseEventArgs MButtons;

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
            {
                
            }
            else
            { };     

        }

        private void HandleMouseEvent(object sender, MOUSE_EVENT_RECORD e)
        {
            if (e.dwEventFlags == EventFlags.MOUSE_MOVED)
            {
                Point lastMousePos = MousePos;
                 
                MousePos = e.dwMousePosition.AsPoint;
                MButtons = e.AsMouseEventArgs;
                MouseMove(MButtons,lastMousePos);
            }
            else if (e.dwEventFlags == 0)
            {
               
                MousePos = e.dwMousePosition.AsPoint;
                MButtons = e.AsMouseEventArgs;
                foreach (var ctrl in children)
                {
                    if (ctrl.Over(MousePos))
                        ctrl.MouseClick(MButtons);
                }
            }
        }

        public virtual void Initialize()
        {

        }

        public void Run()
        {
            running = true;
            while (running)
            {
                // Handling the Event-Queue    

                Thread.Sleep(1);
                //   Console.Clear();
            }
        }

        public void Stop()
        {
            running = false;
        }
    }
}
