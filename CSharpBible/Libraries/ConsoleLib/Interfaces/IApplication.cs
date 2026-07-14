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
using System.Drawing;

namespace ConsoleLib.Interfaces;

public interface IApplication : IGroupControl
{
    Point MousePos { get; }
    bool Running { get; }

    event EventHandler<Point>? OnCanvasResize;

    IWidgetSet WidgetSet { get; }

    //        static IApplication Default { get; }

    void Initialize();
    void Run();
    void Stop();

    void Dispatch(Action act);
    void SetRunning(bool value);
    void ProcessPendingMessages();
    void RaiseMouseEvent(IMouseEvent e);
    void RaiseKeyEvent(IKeyEvent e);
    void RaiseResizeEvent(Point size);
}