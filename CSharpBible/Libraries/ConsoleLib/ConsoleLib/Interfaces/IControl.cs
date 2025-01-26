// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-05-2022
// ***********************************************************************
// <copyright file="Control.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ConsoleLib.ConsoleLib.Interfaces
{
    public interface IControl
    {
        char Accelerator { get; set; }
        bool Active { get; set; }
        Rectangle Dimension { get; set; }
        bool IsVisible { get; }
        IControl? Parent { get; set; }
        Point Position { get; set; }
        Rectangle RealDim { get; }
        bool Shadow { get; set; }
        Size size { get; set; }
        int Tag { get; set; }
        string Text { get; set; }
        bool Valid { get; set; }
        bool Visible { get; set; }
        IControl? ActiveControl { get; set; }
        ConsoleColor BackColor { get; set; }
        ConsoleColor ForeColor { get; set; }
        IList<IControl> Children { get; }

        event EventHandler? OnActivate;
        event EventHandler? OnChange;
        event EventHandler? OnClick;
        event EventHandler<KeyEventArgs>? OnKeyPressed;
        event EventHandler? OnMouseEnter;
        event EventHandler? OnMouseLeave;
        event EventHandler<MouseEventArgs>? OnMouseMove;
        event EventHandler? OnMove;
        event EventHandler? OnResize;

        IControl Add(IControl control);
        void Click();
        void DoUpdate();
        void Draw();
        void HandlePressKeyEvents(KeyPressEventArgs e);
        void Invalidate();
        Rectangle LocalDimOf(Rectangle aDim, IControl? ancestor = null);
        void MouseClick(MouseEventArgs M);
        void MouseEnter(Point M);
        void MouseLeave(Point M);
        void MouseMove(MouseEventArgs M, Point lastMousePos);
        bool Over(Point M);
        Rectangle RealDimOf(Rectangle aDim);
        void ReDraw(Rectangle dimension);
        IControl Remove(IControl control);
        void SetText(string value);
    }
}