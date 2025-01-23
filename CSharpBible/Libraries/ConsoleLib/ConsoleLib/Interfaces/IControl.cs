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

namespace ConsoleLib.Interfaces;

public interface IControl
{
    Rectangle Dimension { get; set; }
    Point Position { get; set; }
    Size Size { get; set; }
    Rectangle RealDim { get; }
    bool Active { get; set; }
    bool Visible { get; set; }
    bool IsVisible { get; }
    bool Shadow { get; set; }
    bool Valid { get; set; }
    ConsoleColor BackColor { get; set; }
    string Text { get; set; }
    List<IControl> Children { get; }
    IControl? ActiveControl { get; set; }
    IControl? Parent { get; set; }
    long Tag { get; set; }
    char Accelerator { get; set; }

    event EventHandler? OnClick;
    event EventHandler? OnMove;
    event EventHandler? OnResize;
    event EventHandler? OnChange;
    event EventHandler? OnActivate;
    event EventHandler? OnMouseEnter;
    event EventHandler? OnMouseLeave;
    event EventHandler<MouseEventArgs>? OnMouseMove;
    event EventHandler<KeyEventArgs>? OnKeyPressed;

    IControl Add(IControl control);
    IControl Remove(IControl control);
    void Draw();
    void Invalidate();
    Rectangle LocalDimOf(Rectangle aDim, IControl? ancestor = null);
    bool Over(Point M);
    Rectangle RealDimOf(Rectangle aDim);
    void ReDraw(Rectangle dimension);
    void HandlePressKeyEvents(KeyPressEventArgs e);
    void MouseClick(MouseEventArgs M);
    void MouseMove(MouseEventArgs M, Point lastMousePos);
    void MouseLeave(Point M);
    void MouseEnter(Point M);
    void Click();
    void DoUpdate();
}