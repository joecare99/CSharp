// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir (extended by AI)
// Created          : 09-26-2025
// ***********************************************************************
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;
using System.Linq;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Horizontal top menu bar hosting root menu items (each optionally with a popup)
/// </summary>
public class MenuBar : Panel
{
    private int _keyboardIndex = -1;

    public bool IsKeyboardActive { get; private set; }
    public bool ShowAccelerators { get; private set; }
    public MenuBar()
    {
        BackColor = ConsoleColor.DarkGray;
        ForeColor = ConsoleColor.Black;
        BorderColor = ConsoleColor.DarkGray;
        BorderStyle = BorderStyle.None;
        size = new Size(80, 1);
        Position = new Point(0, 0);
    }

    public void AddRootItem(MenuItem item, MenuPopup? popup = null)
    {
        item.Parent = this;
        item.SubMenu = popup;
        if (popup != null)
        {
            popup.Parent = this.Parent;
            popup.Visible = false;
        }
        LayoutItems();
    }

    public void LayoutItems()
    {
        int x = 0;
        foreach (MenuItem mi in Children.OfType<MenuItem>())
        {
            mi.Position = new Point(x, 0);
            x += mi.size.Width;
            if (mi.SubMenu != null)
            {
                mi.SubMenu.Position = new Point(mi.Position.X + 1, 2);
            }
        }
        size = new Size(Math.Max(size.Width, x), 1);
        Invalidate();
    }

    public override void Draw()
    {
        WidgetSet?.DrawMenuBar(this);
    }

    public void ShowSubMenuFor(MenuItem item)
    {
        var _flag = false;
        foreach (MenuItem mi in Children.OfType<MenuItem>())
        {
            if (mi.SubMenu != null && mi != item && mi.SubMenu.Visible)
            {
                mi.SubMenu.Hide();
                _flag = true;
            }
        }
        if (item.SubMenu != null && _flag)
        {
            item.SubMenu.Position = new Point(item.Position.X + 1, 2);
            item.SubMenu.Show();
        }
    }

    public void HideAllPopups()
    {
        foreach (MenuItem mi in Children.OfType<MenuItem>())
        {
            mi.SubMenu?.Hide();
        }
    }

    public bool ActivateKeyboard()
    {
        var items = Children.OfType<MenuItem>().Where(item => item.Enabled).ToArray();
        if (items.Length == 0)
            return false;

        IsKeyboardActive = true;
        _keyboardIndex = 0;
        ActiveControl = items[0];
        items[0].Active = true;
        ShowKeyboardSubMenu(items[0]);
        Invalidate();
        return true;
    }

    public void DeactivateKeyboard()
        {
            IsKeyboardActive = false;
            if (ActiveControl is not null)
                ActiveControl.Active = false;
            ActiveControl = null;
            _keyboardIndex = -1;
            HideAllPopups();
            Invalidate();
        }

    public void SetAcceleratorVisibility(bool visible)
        {
            if (ShowAccelerators == visible)
                return;
            ShowAccelerators = visible;
            Invalidate();
        }

    public override void HandlePressKeyEvents(IKeyEvent e)
    {
        if (IsKeyboardActive && e.bKeyDown)
        {
            var items = Children.OfType<MenuItem>().Where(item => item.Enabled).ToArray();
            if (items.Length != 0)
            {
                if (e.usKeyCode is (ushort)ConsoleKey.LeftArrow or (ushort)ConsoleKey.RightArrow)
                {
                    var direction = e.usKeyCode == (ushort)ConsoleKey.LeftArrow ? -1 : 1;
                    _keyboardIndex = (_keyboardIndex + direction + items.Length) % items.Length;
                    if (ActiveControl is not null)
                        ActiveControl.Active = false;
                    ActiveControl = items[_keyboardIndex];
                    ActiveControl.Active = true;
                    ShowKeyboardSubMenu(items[_keyboardIndex]);
                    e.Handled = true;
                    return;
                }

                if (e.usKeyCode == (ushort)ConsoleKey.Escape)
                {
                    DeactivateKeyboard();
                    e.Handled = true;
                    return;
                }
            }
        }

        base.HandlePressKeyEvents(e);
    }

    private void ShowKeyboardSubMenu(MenuItem item)
    {
        foreach (var candidate in Children.OfType<MenuItem>())
        {
            if (!ReferenceEquals(candidate, item))
                candidate.SubMenu?.Hide();
        }

        if (item.SubMenu is not null)
        {
            item.SubMenu.Position = new Point(item.Position.X + 1, 2);
            item.SubMenu.Show();
        }
    }

    public override void MouseClick(global::ConsoleLib.Interfaces.IMouseEvent M) // fully qualified
    {
        base.MouseClick(M);
        if (!Over(M.MousePos))
        {
            HideAllPopups();
        }
    }
}
