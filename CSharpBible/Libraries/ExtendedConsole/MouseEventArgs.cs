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
using ConsoleLib.Interfaces;
using System.Drawing;

namespace ConsoleLib;

public class MouseEventArgs : IMouseEvent
{
    private NativeMethods.MOUSE_EVENT_RECORD _m;

    public MouseEventArgs(NativeMethods.MOUSE_EVENT_RECORD m)
    {
        _m = m;  
    }
    public Point MousePos => new Point(_m.dwMousePosition.X,_m.dwMousePosition.Y);

    public bool MouseButtonLeft => throw new NotImplementedException();

    public bool MouseButtonRight => throw new NotImplementedException();

    public bool MouseButtonMiddle => throw new NotImplementedException();

    public int MouseWheel => throw new NotImplementedException();

    public bool MouseMoved => throw new NotImplementedException();

    public bool ButtonEvent => throw new NotImplementedException();

    public bool Handled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}