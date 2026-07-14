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
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Class Application.
/// Implements the <see cref="ConsoleLib.CommonControls.Panel" />
/// </summary>
/// <seealso cref="ConsoleLib.CommonControls.Panel" />
public class Application : Panel, IApplication, IHasWidgetSet
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
    public bool Running { get; private set; }

    public new IWidgetSet WidgetSet { get; private set; }

    public static IApplication? Default { get; private set; }

    /// <summary>
    /// Occurs when [on canvas resize].
    /// </summary>
    public event EventHandler<Point>? OnCanvasResize;

    /// <summary>
    /// The m buttons
    /// </summary>
    private IMouseEvent? MButtons = default;

    public Application(IWidgetSet widgetSet)
    {
        this.WidgetSet = widgetSet;
        BorderStyle = BorderStyle.None;
        Control.MessageQueue ??= new ConcurrentQueue<(Action<object, EventArgs>, object, EventArgs)>();
        this.WidgetSet.InitializeApplication(this);
        Default = this;
    }

    /// <summary>
    /// Handles the win buf event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void HandleWinBufEvent(object? sender, Point e)
    {
        (WidgetSet as IConsoleWidgetHost)?.ClearHost();
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
            base.HandlePressKeyEvents(e);
        }
        else
        { }
        ;

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
        else if (e.ButtonEvent)
        {

            MousePos = e.MousePos;
            MButtons = e;
            foreach (var ctrl in Children.ToList())
            {
                if (ctrl.Over(MousePos))
                    ctrl.MouseClick(e);
            }
        }
        else
        {
            if (e.MouseWheel != 0)
                MouseMove(e, e.MousePos);
        }
    }

    public void RaiseMouseEvent(IMouseEvent e)
    {
        HandleMouseEvent(this, e);
    }

    public void RaiseKeyEvent(IKeyEvent e)
    {
        HandleKeyEvent(this, e);
    }
    public void RaiseResizeEvent(Point size)
    {
        HandleWinBufEvent(this, size);
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
        Running = true;
        WidgetSet.RunApplication(this);
        ProcessPendingMessages();
        (WidgetSet as IConsoleWidgetHost)?.SetCursorPosition(0, Position.Y + size.Height);
    }

    /// <summary>
    /// Handles the messages.
    /// </summary>
    public void ProcessPendingMessages()
    {
        bool processed = false;
        while (Control.TryDequeueMessage(out var workItem))
        {
            workItem.handler?.Invoke(workItem.sender, workItem.args);
            processed = true;
        }
        if (processed)
        {
            DoUpdate();
        }
    }

    public void SetRunning(bool value)
    {
        Running = value;
    }

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop()
    {
        Running = false;
        Control.EnqueueMessage(static (_, _) => { }, this, EventArgs.Empty);
        WidgetSet.StopApplication(this);
    }

    public void Dispatch(Action act)
    {
        if (act == null)
            return;
        Control.EnqueueMessage((_, _) => act(), this, EventArgs.Empty);
    }
}
