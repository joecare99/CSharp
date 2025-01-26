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

namespace ConsoleLib.ConsoleLib.Interfaces
{
    public interface IApplication
    {
        Point MousePos { get; }
        bool running { get; }

        event EventHandler<Point>? OnCanvasResize;

        void Initialize();
        void Run();
        void Stop();
    }
}