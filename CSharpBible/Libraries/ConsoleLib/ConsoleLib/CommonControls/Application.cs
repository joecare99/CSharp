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
using BaseLib.Interfaces;
using ConsoleLib.ConsoleLib.Interfaces;
using ConsoleLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Class Application.
/// Implements the <see cref="ConsoleLib.CommonControls.Panel" />
/// </summary>
/// <seealso cref="ConsoleLib.CommonControls.Panel" />
public class Application : Panel, IApplication
{
    /// <summary>
    /// Gets the mouse Position.
    /// </summary>
    /// <value>The mouse Position.</value>
    public Point MousePos { get; private set; }
    /// <summary>
    /// Gets a value indicating whether this <see cref="Application"/> is running.
    /// </summary>
    /// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
    public bool running { get; private set; }

    public static IApplication? Default { get; private set; }

    /// <summary>
    /// Occurs when [on canvas resize].
    /// </summary>
    public event EventHandler<Point>? OnCanvasResize;

    /// <summary>
    /// The m buttons
    /// </summary>
    private IMouseEvent? MButtons = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="Application"/> class.
    /// </summary>
    public Application(IConsole console,IExtendedConsole extendedConsole)
    {
        ConsoleFramework.ExtendedConsole = extendedConsole;
        ConsoleFramework.console = console;
        ConsoleFramework.ExtendedConsole.MouseEvent += HandleMouseEvent;
        ConsoleFramework.ExtendedConsole.KeyEvent += HandleKeyEvent;
        ConsoleFramework.ExtendedConsole.WindowBufferSizeEvent += HandleWinBufEvent;
        Border = new Char[] { };
        Control.MessageQueue = new Stack<(Action<object, EventArgs>, object, EventArgs)>();
        Default = this; 
    }

    /// <summary>
    /// Handles the win buf event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void HandleWinBufEvent(object? sender, Point e)      
    {
        Console.Clear();
        OnCanvasResize?.Invoke(this, e);
        Invalidate();
    }

    /// <summary>
    /// Handles the key event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void HandleKeyEvent(object? sender, IKeyEvent e)
    {
        // Determine the Control to send the Event to

        if (e.bKeyDown)
        {
            ActiveControl?.HandlePressKeyEvents(e);
            if (!e.Handled)
                foreach (var ctrl in Children)
                {

                    ctrl.HandlePressKeyEvents(e);
                    if (e.Handled)
                        break;
                }
        }
        else
        { };

    }

    /// <summary>
    /// Handles the mouse event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void HandleMouseEvent(object? sender, IMouseEvent e) 
    {
        if (e.MouseMoved)
        {
            Point lastMousePos = MousePos;

            MousePos = e.MousePos;
            MButtons = e;
            MouseMove(e, lastMousePos);
        }
        else if (e.ButtonEvent )
        {

            MousePos = e.MousePos;
            MButtons = e;
            foreach (var ctrl in Children)
            {
                if (ctrl.Over(MousePos))
                    ctrl.MouseClick(e);
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
        ConsoleFramework.console.SetCursorPosition(0, Position.Y+size.Height);
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

    public void Dispatch(Action act)
    {
        MessageQueue?.Push(((s,e)=>act(), this, EventArgs.Empty));
    }
}
