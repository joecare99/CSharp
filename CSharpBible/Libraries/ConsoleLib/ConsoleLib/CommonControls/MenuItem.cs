// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir (extended by AI)
// Created          : 09-26-2025
// ***********************************************************************
// <copyright>
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// ***********************************************************************
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls;

/// <summary>
/// A single menu entry. Can either trigger an action or own a sub menu.
/// </summary>
public class MenuItem : CommandControl
{
    public bool IsSeparator { get; set; }
    public char ShortcutKey { get; set; } // optional display only
    public IPopup? SubMenu { get; set; }
    public ConsoleColor DisabledForeColor { get; set; } = ConsoleColor.DarkGray;
    public ConsoleColor HotColor { get; set; } = ConsoleColor.White;
    public ConsoleColor HotBackColor { get; set; } = ConsoleColor.DarkBlue;

    private bool _hover;

    public MenuItem()
    {
        ForeColor = ConsoleColor.Gray;
        BackColor = ConsoleColor.Black;
    }

    public override void SetText(string value)
    {
        base.SetText(value);
        if (!IsSeparator)
        {
            size = new Size(value.Length + 2, 1);
            if (ShortcutKey == '\0')
            {
                var idx = value.IndexOf('&');
                if (idx >= 0 && idx + 1 < value.Length && value[idx+1]!='&')
                    Accelerator = char.ToUpperInvariant(value[idx + 1]);
                else if (value.Length > 0 && value[0]>'@')
                    Accelerator = char.ToUpperInvariant(value[0]);
            }
        }
        else
        {
            size = new Size(Math.Max(1, size.Width), 1);
        }
    }

    public override void Draw()
    {
        var dim = RealDimOf(Dimension);
        if (IsSeparator)
        {
            dim.Width = Parent?.Dimension.Width-2 ?? 0;
            ConsoleFramework.Canvas.FillRect(dim, ForeColor, BackColor, '-');
            return;
        }

        var fg = Enabled ? (_hover ? HotColor : ForeColor) : DisabledForeColor;
        var bg = _hover ? HotBackColor : BackColor;
        ConsoleFramework.Canvas.FillRect(dim, fg, bg, ' ');
        var text = Text ?? "";
        // Remove accelerator marker '&'
        text = text.Replace("&&", "\u0001"); // temp placeholder for escaped &
        int accIndex = text.IndexOf('&');
        if (accIndex >= 0)
            text = text.Remove(accIndex, 1);
        text = text.Replace('\u0001', '&');

        ConsoleFramework.console.BackgroundColor = bg;
        ConsoleFramework.console.SetCursorPosition(dim.X + 1, dim.Y);
        ConsoleFramework.console.Write(text.PadRight(Math.Max(0, dim.Width - 2)));
        ConsoleFramework.console.BackgroundColor = ConsoleColor.Black;
        Valid = true;
    }

    public override void MouseEnter(Point M)
    {
        _hover = true;
        if (Parent is MenuBar mb)
        {
            mb.ShowSubMenuFor(this);
        }
        base.MouseEnter(M);
        Invalidate();
    }

    public override void MouseLeave(Point M)
    {
        _hover = false;
        base.MouseLeave(M);
        Invalidate();
    }

    public override void Click()
    {
        if (!Enabled) return;
        if (SubMenu != null)
        {
            if (!SubMenu.Visible)
                SubMenu.Show();
            else
                SubMenu.Hide();
        }
        else
        {
            (Application.Default as Application)?.Dispatch(() =>
            {
                (Parent as MenuBar)?.HideAllPopups();
            });
        }
        base.Click();
    }

    public override void HandlePressKeyEvents(Interfaces.IKeyEvent e)
    {
        if (!Enabled) return;
        if (char.ToUpperInvariant(e.KeyChar) == Accelerator && Accelerator != '\0')
        {
            e.Handled = true;
            Click();
            return;
        }
        base.HandlePressKeyEvents(e);
    }
}
