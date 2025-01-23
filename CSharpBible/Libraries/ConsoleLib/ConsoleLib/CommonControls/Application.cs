// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="Application.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static ConsoleLib.NativeMethods;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Class Application.
    /// Implements the <see cref="ConsoleLib.CommonControls.Panel" />
    /// </summary>
    /// <seealso cref="ConsoleLib.CommonControls.Panel" />
    public class Application : Panel
    {
        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        public Point MousePos { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="Application"/> is running.
        /// </summary>
        /// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
        public bool running { get; private set; }

        /// <summary>
        /// Occurs when [on canvas resize].
        /// </summary>
        public event EventHandler<Point>? OnCanvasResize;

        /// <summary>
        /// The m buttons
        /// </summary>
        private MouseEventArgs? MButtons=default;

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
        {
            ExtendedConsole.MouseEvent += HandleMouseEvent;
            ExtendedConsole.KeyEvent += HandleKeyEvent;
            ExtendedConsole.WindowBufferSizeEvent += HandleWinBufEvent;
            Boarder = new Char[] { };
            Control.MessageQueue = new Stack<(Action<object, EventArgs>,object,EventArgs)>();
        }

#if NET5_0_OR_GREATER
        private void HandleWinBufEvent(object? sender, WINDOW_BUFFER_SIZE_RECORD e)
#else
        /// <summary>
        /// Handles the win buf event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void HandleWinBufEvent(object sender, WINDOW_BUFFER_SIZE_RECORD e)
#endif       
        {
            Console.Clear();
            OnCanvasResize?.Invoke(this,e.dwSize.AsPoint);
            Invalidate();
        }

#if NET5_0_OR_GREATER
        private void HandleKeyEvent(object? sender, KEY_EVENT_RECORD e)
#else
        /// <summary>
        /// Handles the key event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void HandleKeyEvent(object sender, KEY_EVENT_RECORD e)
#endif        
    {
            // Determine the Control to send the Event to

            if (e.bKeyDown)
            {
                var keyEventArgs = new KeyPressEventArgs(e.UnicodeChar);
                ActiveControl?.HandlePressKeyEvents(keyEventArgs);
                if (!keyEventArgs.Handled)
                foreach ( var ctrl in children)
                {

                        ctrl.HandlePressKeyEvents(keyEventArgs);
                        if (keyEventArgs.Handled) break;
                }
            }
            else
            { };     

        }

#if NET5_0_OR_GREATER
        private void HandleMouseEvent(object? sender, MOUSE_EVENT_RECORD e)
#else
        /// <summary>
        /// Handles the mouse event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void HandleMouseEvent(object sender, MOUSE_EVENT_RECORD e) 
#endif
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

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
               
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            running = true;
            while (running)
            {
                HandleMessages();   
                // DoOnIdle

                Thread.Sleep(1);
                //   Console.Clear();
            }
        }

        /// <summary>
        /// Handles the messages.
        /// </summary>
        private void HandleMessages()
        {
            if (Control.MessageQueue != null)
            {
                int cc = Control.MessageQueue.Count;
                if (cc > 0)
                {
                    while (cc-- > 0)
                    {
                        (var Act, var sender, var arg2) = Control.MessageQueue.Pop();
                        Act?.Invoke(sender, arg2);
                    }
                    DoUpdate();
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            running = false;
        }
    }
}
