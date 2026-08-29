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
using System.Drawing;
using System.Linq;
using System.Threading;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Class Application.
/// Implements the <see cref="ConsoleLib.CommonControls.Panel" />
/// </summary>
/// <seealso cref="ConsoleLib.CommonControls.Panel" />
public class Application : Panel, IApplication, IHasWidgetSet, IDisposable
{
    private const ushort AltVirtualKey = 0x12;
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

    /// <summary>Gets the application-scoped callback queue.</summary>
    public new IMessageQueue MessageQueue { get; }

    /// <summary>Gets the application-scoped dispatcher.</summary>
    public IDispatcher Dispatcher { get; }

    /// <summary>Gets the application-scoped scheduler.</summary>
    public IScheduler Scheduler { get; }

    /// <summary>Gets the keyboard focus manager for this application.</summary>
    public IFocusManager FocusManager { get; }

    public static IApplication? Default { get; private set; }

    /// <summary>
    /// Occurs when [on canvas resize].
    /// </summary>
    public event EventHandler<Point>? OnCanvasResize;

    /// <summary>
    /// The m buttons
    /// </summary>
    private IMouseEvent? MButtons = default;
    private IControl? _focusBeforeMenu;
    private int _disposed;

    public Application(IWidgetSet widgetSet)
    {
        this.WidgetSet = widgetSet;
        BorderStyle = BorderStyle.None;
        MessageQueue = new ApplicationMessageQueue();
        Dispatcher = new ApplicationDispatcher(MessageQueue);
        Scheduler = new ApplicationScheduler(Dispatcher, new SystemClock());
        FocusManager = new FocusManager(this);
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
        var menuBar = FindMenuBar(this);
        if (e.usKeyCode == (ushort)ConsoleKey.F10 && e.bKeyDown && menuBar is not null)
        {
            if (menuBar.IsKeyboardActive)
            {
                menuBar.DeactivateKeyboard();
                if (_focusBeforeMenu is not null)
                    FocusManager.Focus(_focusBeforeMenu);
                else
                    FocusManager.Clear();
                _focusBeforeMenu = null;
            }
            else
            {
                _focusBeforeMenu = FocusManager.FocusedControl;
                FocusManager.Focus(menuBar);
                menuBar.ActivateKeyboard();
            }
            e.Handled = true;
        }

        if (e.usKeyCode == AltVirtualKey && menuBar is not null)
        {
            menuBar.SetAcceleratorVisibility(e.bKeyDown);
        }

        if (e.bKeyDown && e.usKeyCode == (ushort)ConsoleKey.Tab)
        {
            var modifiers = KeyModifiers.None;
            if ((e.dwControlKeyState & 0x10) != 0)
                modifiers |= KeyModifiers.Shift;
            if ((e.dwControlKeyState & (0x08 | 0x04)) != 0)
                modifiers |= KeyModifiers.Control;
            if ((e.dwControlKeyState & (0x02 | 0x01)) != 0)
                modifiers |= KeyModifiers.Alt;

            e.Handled = FocusManager.HandleKey(new KeyInput(ConsoleKey.Tab, e.KeyChar, modifiers, true));
        }

        if (!e.Handled)
            base.HandlePressKeyEvents(e);

    }

    private static MenuBar? FindMenuBar(IControl control)
    {
        foreach (var child in control.Children)
        {
            if (child is MenuBar menuBar)
                return menuBar;
            var nested = FindMenuBar(child);
            if (nested is not null)
                return nested;
        }
        return null;
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
        Dispatcher.ProcessPending();
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
        if (Volatile.Read(ref _disposed) != 0)
            return;

        Running = false;
        Control.EnqueueMessage(static (_, _) => { }, this, EventArgs.Empty);
        WidgetSet.StopApplication(this);
    }

    public void Dispatch(Action act)
    {
        if (act == null || Volatile.Read(ref _disposed) != 0)
            return;
        Dispatcher.Dispatch(act);
    }

    /// <summary>Stops the application and releases application-scoped services.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        Running = false;
        Scheduler.Dispose();
        (MessageQueue as IDisposable)?.Dispose();
        if (ReferenceEquals(Default, this))
            Default = null;
    }
}
