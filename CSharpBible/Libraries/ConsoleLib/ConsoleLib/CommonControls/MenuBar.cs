// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir (extended by AI)
// Created          : 09-26-2025
// ***********************************************************************
using System;
using System.Drawing;
using System.Linq;
using ConsoleLib.Interfaces; // added

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// Horizontal top menu bar hosting root menu items (each optionally with a popup)
    /// </summary>
    public class MenuBar : Panel
    {
        public MenuBar()
        {
            BackColor = ConsoleColor.DarkGray;
            ForeColor = ConsoleColor.Black;
            BoarderColor = ConsoleColor.DarkGray;
            Border = Array.Empty<char>();
            size = new Size(ConsoleFramework.console.WindowWidth, 1);
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
            // fill background line
            var dim = RealDim;
            ConsoleFramework.Canvas.FillRect(dim, ForeColor, BackColor, ' ');
            foreach (MenuItem mi in Children.OfType<MenuItem>())
            {
                mi.Draw();
                if (mi.SubMenu != null && mi.SubMenu.Visible)
                    mi.SubMenu.Draw();
            }
            Valid = true;
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

        public override void MouseClick(global::ConsoleLib.Interfaces.IMouseEvent M) // fully qualified
        {
            base.MouseClick(M);
            if (!Over(M.MousePos))
            {
                HideAllPopups();
            }
        }
    }
}
