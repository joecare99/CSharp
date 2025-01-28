// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-15-2022
// ***********************************************************************
// <copyright file="ConsoleFramework.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleLib.Interfaces;

public interface IExtendedConsole 
{
    event EventHandler<IMouseEvent>? MouseEvent;
    event EventHandler<IKeyEvent>? KeyEvent;
    event EventHandler<Point>? WindowBufferSizeEvent;

    void Stop();
}