// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir (extended by AI)
// Created          : 09-26-2025
// ***********************************************************************
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls
{
    /// <summary>
    /// A popup menu window; parent normally is a MenuItem (root under MenuBar)
    /// </summary>
    public class MenuPopup : Panel, IPopup
    {
        public MenuPopup()
        {
            BackColor = ConsoleColor.DarkBlue;
            ForeColor = ConsoleColor.Gray;
            BoarderColor = ConsoleColor.Gray;
            Border = ConsoleFramework.singleBorder;
            Visible = false;
        }

        public void AddItem(MenuItem item)
        {
            item.Parent = this;
            LayoutItems();
        }

        public void LayoutItems()
        {
            int y = 1; // inside border
            int w = 0;
            foreach (MenuItem mi in Children)
            {
                mi.Position = new Point(1, y);
                y += 1;
                w = Math.Max(w, mi.size.Width + 2);
            }
            size = new Size(w + 2, y + 1);
            if (Visible)
               Invalidate();
        }

        public void Show()
        {
            (Parent as IGroupControl)?.BringToFront(this);
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public override void MouseLeave(Point M)
        {
            base.MouseLeave(M);
            // Optionally auto-hide? keep for now
        }
    }
}
