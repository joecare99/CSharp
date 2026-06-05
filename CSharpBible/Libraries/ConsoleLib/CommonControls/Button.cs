// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : AI Assistant
// Last Modified On : 09-26-2025
// ***********************************************************************
// <copyright file="Button.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Class Button.
/// Implements the <see cref="ConsoleLib.Control" />
/// </summary>
/// <seealso cref="ConsoleLib.Control" />
public class Button : CommandControl
{
    private bool _hasExplicitSize;
    private bool _suppressExplicitSizeTracking;

    public ConsoleColor HLBackColor { get; set; } = ConsoleColor.Green;
    public ConsoleColor DisabledBackColor { get; set; } = ConsoleColor.DarkGray;
    public ConsoleColor DisabledFrontColor { get; set; } = ConsoleColor.Black;

    protected override void SetSize(Size value)
    {
        if (!_suppressExplicitSizeTracking)
        {
            _hasExplicitSize = true;
        }

        base.SetSize(value);
    }

    private void SetAutoSize(Size value)
    {
        _suppressExplicitSizeTracking = true;
        try
        {
            base.SetSize(value);
        }
        finally
        {
            _suppressExplicitSizeTracking = false;
        }
    }

    public void Set(int X, int Y, string text, ConsoleColor backColor)
    {
        BackColor =
        _ActBackColor = backColor;
        _ActForeColor = ForeColor;
        Text = text;
        SetAutoSize(new Size(text.Length + 2, 1));
        Position = new Point(X, Y);
    }

    public override void MouseEnter(Point M)
    {
        if (Enabled)
        {
            base.MouseEnter(M);
            _ActBackColor = HLBackColor;
            NotifyWidgetStateChanged();
            Invalidate();
        }
    }

    public override void MouseLeave(Point M)
    {
        base.MouseLeave(M);
        _ActBackColor = Enabled ? BackColor : DisabledBackColor;
        NotifyWidgetStateChanged();
        Invalidate();
    }

    public override void SetText(string value)
    {
        base.SetText(value);
        if (!_hasExplicitSize)
        {
            SetAutoSize(new Size(value.Length + 2, 1));
        }
    }

    public override void Draw()
    {
        // Beispiel: Farbwahl abhängig von Enabled
        var oldFore = ForeColor;
        var oldBack = BackColor;
        if (!Enabled)
        {
            ForeColor = ConsoleColor.DarkGray;
            BackColor = ConsoleColor.Black;
        }
        base.Draw();
        ForeColor = oldFore;
        BackColor = oldBack;
    }
}
