using BaseLib.Interfaces;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;
using System.Linq;

namespace ConsoleLib.ExtCon;

/// <summary>
/// Default widget set that preserves the existing ExtendedConsole-backed rendering behavior.
/// </summary>
public sealed class ConsoleWidgetSet : IWidgetSet, IConsoleWidgetHost
{
    private static readonly char[] RaisedBorder = { '▀', '▌', '▛', '▜', '▙', '▟', '▌', '▐', '▀', '▄', '█' };
    private static readonly char[] LoweredBorder = { '▄', '▐', '▙', '▟', '▛', '▜', '▌', '▐', '▀', '▄', '█' };

    public IConsole console { get; private set; }
    public IExtendedConsole extendedConsole { get; private set; }

    public int WindowWidth => console.WindowWidth;
    public bool IsOutputRedirected => console.IsOutputRedirected;
    public char[] ShadeChars => ConsoleFramework.chars;
    public char[] SingleBorder => ConsoleFramework.singleBorder;
    public ushort KeyEnter => ConsoleFramework.VK_ENTER;
    public ushort KeyEsc => ConsoleFramework.VK_ESC;
    public ushort KeyTab => ConsoleFramework.VK_TAB;
    public ushort KeyLeft => ConsoleFramework.VK_LEFT;
    public ushort KeyUp => ConsoleFramework.VK_UP;
    public ushort KeyRight => ConsoleFramework.VK_RIGHT;
    public ushort KeyDown => ConsoleFramework.VK_DOWN;
    public ushort KeyHome => ConsoleFramework.VK_HOME;
    public ushort KeyEnd => ConsoleFramework.VK_END;
    public ushort KeyDelete => ConsoleFramework.VK_DELETE;
    public ushort KeyPageUp => ConsoleFramework.VK_PRIOR;
    public ushort KeyPageDown => ConsoleFramework.VK_NEXT;

    public ConsoleWidgetSet(IConsole console, IExtendedConsole extendedConsole)
    {
        this.console = ConsoleFramework.console = console;
        this.extendedConsole = ConsoleFramework.ExtendedConsole = extendedConsole;
    }

    public Rectangle ClipRect => ConsoleFramework.Canvas.ClipRect;

    public void InitializeApplication(IApplication application)
    {
        extendedConsole.MouseEvent += (_, e) => application.RaiseMouseEvent(e);
        extendedConsole.KeyEvent += (_, e) => application.RaiseKeyEvent(e);
        extendedConsole.WindowBufferSizeEvent += (_, e) => application.RaiseResizeEvent(e);
    }

    public void RunApplication(IApplication application)
    {
        application.SetRunning(true);
        while (application.Running)
        {
            application.ProcessPendingMessages();
            Control.MessageWaitHandle.WaitOne();
        }

        application.ProcessPendingMessages();
        ConsoleFramework.console.SetCursorPosition(0, application.Position.Y + application.size.Height);
    }

    public void StopApplication(IApplication application)
    {
        StopHost();
    }

    public void ClearHost() => console.Clear();

    public void StopHost() => extendedConsole.Stop();

    public void SetCursorPosition(int left, int top) => console.SetCursorPosition(left, top);

    public void Beep(int frequency, int duration) => console.Beep(frequency, duration);

    public void WriteTerminalCell(Terminal terminal, int x, int y, char character, ConsoleColor foreground, ConsoleColor background)
        => ConsoleFramework.Canvas.OutTextXY(terminal.RealDim.Left + 1 + x, terminal.RealDim.Top + 1 + y, character, foreground, background);

    public void ClearTerminalCell(Terminal terminal, int x, int y, char character, ConsoleColor foreground, ConsoleColor background)
        => ConsoleFramework.Canvas.OutTextXY(terminal.RealDim.Left + 1 + x, terminal.RealDim.Top + 1 + y, character, foreground, background);

    public void FillShadow(Rectangle rectangle, ConsoleColor foreground, ConsoleColor background, char fillChar)
        => ConsoleFramework.Canvas.FillRect(rectangle, foreground, background, fillChar);

    public void AttachControl(IControl control)
    {
    }

    public void DetachControl(IControl control)
    {
    }

    public void SynchronizeControl(IControl control)
    {
    }

    private static char ResolveGlyph(GlyphStyle glyphStyle)
    {
        return glyphStyle switch
        {
            GlyphStyle.Block => ConsoleFramework.chars[0],
            GlyphStyle.DenseShade => ConsoleFramework.chars[1],
            GlyphStyle.MediumShade => ConsoleFramework.chars[2],
            GlyphStyle.LightShade => ConsoleFramework.chars[3],
            GlyphStyle.Blank => ConsoleFramework.chars[4],
            GlyphStyle.PanelFill => ConsoleFramework.chars[3],
            GlyphStyle.ShadowFill => ConsoleFramework.chars[4],
            GlyphStyle.Separator => '-',
            GlyphStyle.ScrollBarVerticalDecrease => '\x18',
            GlyphStyle.ScrollBarVerticalIncrease => '\x19',
            GlyphStyle.ScrollBarHorizontalDecrease => '\x11',
            GlyphStyle.ScrollBarHorizontalIncrease => '\x10',
            GlyphStyle.ScrollBarThumb => ConsoleFramework.chars[1],
            _ => ConsoleFramework.chars[4]
        };
    }

    private static char[]? ResolveBorder(IBorderDefinition borderDefinition)
    {
        if (borderDefinition == null)
        {
            return null;
        }

        return borderDefinition.Style switch
        {
            BorderStyle.None => null,
            BorderStyle.Single => ConsoleFramework.singleBorder,
            BorderStyle.Double => ConsoleFramework.doubleBorder,
            BorderStyle.Simple => ConsoleFramework.simpleBorder,
            BorderStyle.Raised => RaisedBorder,
            BorderStyle.Lowered => LoweredBorder,
            BorderStyle.Custom => borderDefinition.CustomChars,
            _ => borderDefinition.CustomChars
        };
    }

    public void DrawControl(IControl control)
    {
        lock (control)
        {
            if (control.RealDim.Width > 0 && control.RealDim.Height > 0)
            {
                ConsoleFramework.Canvas.FillRect(control.RealDim, control.GetActualForeColor(), control.GetActualBackColor(), ResolveGlyph(GlyphStyle.Blank));
            }

            string buttonText = $"{control.Text}";
            if (buttonText.Length > control.RealDim.Width - 2)
            {
                buttonText = buttonText.Substring(0, control.RealDim.Width - 2);
            }

            if (control.RealDim.Width <= 0 || control.RealDim.Height <= 0)
            {
                return;
            }

            int innerWidth = Math.Max(0, control.RealDim.Width - 2);
            string centeredButtonText = buttonText.Length > innerWidth
                ? buttonText.Substring(0, innerWidth)
                : buttonText;

            centeredButtonText = centeredButtonText
                .PadLeft((innerWidth + centeredButtonText.Length) / 2)
                .PadRight(innerWidth);

            buttonText = $"[{centeredButtonText}]";

            ConsoleFramework.console.ForegroundColor = control.GetActualForeColor();
            ConsoleFramework.console.BackgroundColor = control.GetActualBackColor();
            ConsoleFramework.console.SetCursorPosition(control.RealDim.X, control.RealDim.Y + control.RealDim.Height / 2);
            ConsoleFramework.console.Write(buttonText);
            ConsoleFramework.console.BackgroundColor = ConsoleColor.Black;
        }
    }

    public void DrawLabel(IControl label)
    {
        Console.ForegroundColor = label.ForeColor;
        if (label is Label l && l.ParentBackground && label.Parent != null)
        {
            Console.BackgroundColor = label.Parent.BackColor;
        }
        else
        {
            Console.BackgroundColor = label.BackColor;
        }

        ConsoleFramework.Canvas.OutTextXY(
            label.RealDim.Location,
            (" " + (label.Text ?? string.Empty) + "                  ").Substring(0, Math.Min(label.size.Width, (label.Text ?? string.Empty).Length + 14)));
        Console.BackgroundColor = ConsoleColor.Black;
    }

    public void DrawPixel(IControl pixel)
    {
        if (pixel.Parent != null && !pixel.Parent.Dimension.Contains(Point.Add(pixel.Position, (Size)pixel.Parent.Position)))
        {
            return;
        }

        Application.Default?.Dispatch(() =>
        {
            Console.BackgroundColor = pixel.BackColor;
            ConsoleFramework.Canvas.OutTextXY(pixel.RealDim.Location, $"{pixel.Text}");
            Console.BackgroundColor = ConsoleColor.Black;
        });
    }

    public void DrawPanel(IGroupControl panel)
    {
        if (panel is IHasBorder hasBorder && hasBorder.BorderDefinition != null)
        {
            char[]? border = ResolveBorder(hasBorder.BorderDefinition);
            ConsoleFramework.Canvas.FillRect(panel.RealDim, panel.ForeColor, panel.BackColor, ResolveGlyph(GlyphStyle.PanelFill));
            if (border?.Length > 5)
            {
                ConsoleFramework.Canvas.DrawRect(panel.RealDim, hasBorder.BorderDefinition.BorderColor, panel.BackColor, border);
            }
        }

        foreach (IControl child in panel.Children.Reverse())
        {
            if (!child.Visible)
            {
                continue;
            }

            if (child.Shadow)
            {
                Rectangle shadowDimension = child.Dimension;
                shadowDimension.Offset(1, 1);
                shadowDimension.Offset(panel.Position);
                ConsoleFramework.Canvas.FillRect(panel.RealDimOf(shadowDimension), ConsoleColor.DarkGray, ConsoleColor.Black, ResolveGlyph(GlyphStyle.ShadowFill));
            }

            child.Draw();
        }

        panel.Valid = true;
    }

    public void RedrawPanel(IGroupControl groupControl, Rectangle dimension)
    {
        if (groupControl is not Panel panel)
        {
            return;
        }

        char[]? border = ResolveBorder(panel.BorderDefinition);
        if (dimension.IsEmpty)
        {
            return;
        }

        Rectangle innerRect = panel.Dimension;
        innerRect.Inflate(-1, -1);
        Rectangle clip = dimension;
        clip.Intersect(innerRect);

        try
        {
            ConsoleFramework.Canvas.FillRect(panel.RealDimOf(clip), panel.ForeColor, panel.BackColor, ResolveGlyph(GlyphStyle.PanelFill));
            if (border != null && border.Length > 5 && panel.Dimension.IntersectsWith(dimension) &&
                !(innerRect.Contains(dimension.Location) && innerRect.Contains(Point.Subtract(Point.Add(dimension.Location, dimension.Size), new Size(1, 1)))))
            {
                ConsoleFramework.Canvas.DrawRect(panel.RealDim, panel.BorderColor, panel.BackColor, border);
            }

            foreach (IControl child in panel.Children.Reverse())
            {
                if (!child.Visible)
                {
                    continue;
                }

                if (child.Shadow)
                {
                    Rectangle shadowDimension = child.Dimension;
                    shadowDimension.Offset(1, 1);
                    shadowDimension.Offset(panel.Position);
                    shadowDimension.Intersect(dimension);
                    ConsoleFramework.Canvas.FillRect(panel.RealDimOf(shadowDimension), ConsoleColor.DarkGray, ConsoleColor.Black, ResolveGlyph(GlyphStyle.ShadowFill));
                }

                Rectangle childClip = dimension;
                childClip.Location = Point.Subtract(dimension.Location, (Size)panel.Dimension.Location);
                child.ReDraw(childClip);
            }

            panel.Valid = true;
        }
        catch
        {
        }
    }

    public void DrawMenuItem(IControl menuItemControl)
    {
        if (menuItemControl is not MenuItem menuItem)
        {
            return;
        }

        Rectangle dim = menuItem.RealDimOf(menuItem.Dimension);
        if (menuItem.IsSeparator)
        {
            dim.Width = menuItem.Parent?.Dimension.Width - 2 ?? 0;
            ConsoleFramework.Canvas.FillRect(dim, menuItem.ForeColor, menuItem.BackColor, ResolveGlyph(GlyphStyle.Separator));
            return;
        }

        ConsoleColor fg = menuItem.Enabled ? (menuItem.IsHovered ? menuItem.HotColor : menuItem.ForeColor) : menuItem.DisabledForeColor;
        ConsoleColor bg = menuItem.IsHovered ? menuItem.HotBackColor : menuItem.BackColor;
        ConsoleFramework.Canvas.FillRect(dim, fg, bg, ' ');
        string text = (menuItem.Text ?? string.Empty).Replace("&&", "\u0001");
        int accIndex = text.IndexOf('&');
        if (accIndex >= 0)
        {
            text = text.Remove(accIndex, 1);
        }

        text = text.Replace('\u0001', '&');
        ConsoleFramework.console.BackgroundColor = bg;
        ConsoleFramework.console.SetCursorPosition(dim.X + 1, dim.Y);
        ConsoleFramework.console.Write(text.PadRight(Math.Max(0, dim.Width - 2)));
        ConsoleFramework.console.BackgroundColor = ConsoleColor.Black;
    }

    public void DrawMenuBar(IGroupControl menuBarControl)
    {
        if (menuBarControl is not MenuBar menuBar)
        {
            return;
        }

        Rectangle dim = menuBar.RealDim;
        ConsoleFramework.Canvas.FillRect(dim, menuBar.ForeColor, menuBar.BackColor, ' ');
        foreach (MenuItem menuItem in menuBar.Children.OfType<MenuItem>())
        {
            menuItem.Draw();
            if (menuItem.SubMenu != null && menuItem.SubMenu.Visible)
            {
                menuItem.SubMenu.Draw();
            }
        }

        menuBar.Valid = true;
    }

    public void DrawListBox(IControl listBoxControl)
    {
        if (listBoxControl is not ListBox listBox)
        {
            return;
        }

        Rectangle dim = listBox.RealDim;
        ConsoleFramework.Canvas.FillRect(dim, listBox.ForeColor, listBox.BackColor, ' ');
        int rows = listBox.GetVisibleRows();
        int colWidth = listBox.size.Width - (listBox.GetNeedScrollBar() ? 1 : 0);
        for (int line = 0; line < rows; line++)
        {
            int itemIndex = listBox.GetTopIndex() + line;
            int y = dim.Y + line;
            int x = dim.X;
            string text = string.Empty;
            bool selectedLine = itemIndex == listBox.GetSelectedIndex();
            bool hoverLine = itemIndex == listBox.GetHoverIndex() && !selectedLine;
            if (itemIndex < listBox.GetItemCount() && itemIndex >= 0)
            {
                text = listBox.GetItemAt(itemIndex)?.ToString() ?? string.Empty;
            }

            if (text.Length > colWidth)
            {
                text = text.Substring(0, colWidth);
            }

            if (colWidth > 0)
            {
                ConsoleColor fg = selectedLine ? listBox.SelectedForeColor : (hoverLine ? listBox.HoverForeColor : listBox.ForeColor);
                ConsoleColor bg = selectedLine ? listBox.SelectedBackColor : (hoverLine ? listBox.HoverBackColor : listBox.BackColor);
                ConsoleFramework.Canvas.OutTextXY(x, y, text.PadRight(colWidth, ' '), fg, bg);
            }
        }

        listBox.GetVerticalScrollBar()?.Draw();
    }

    public void DrawScrollBar(IControl scrollBarControl)
    {
        if (scrollBarControl is not ScrollBar scrollBar)
        {
            return;
        }

        Rectangle dim = scrollBar.RealDim;
        ConsoleColor bg = scrollBar.BackColor;
        ConsoleColor thumbBase = scrollBar.TrackColor;
        if (!scrollBar.Enabled)
        {
            bg = scrollBar.DisabledBackColor;
            thumbBase = scrollBar.DisabledThumbBackColor;
        }

        ConsoleFramework.Canvas.FillRect(dim, scrollBar.ForeColor, bg, ResolveGlyph(GlyphStyle.PanelFill));
        (int thumbPos, int thumbLen) = scrollBar.GetThumbData();
        char decGlyph = scrollBar.Vertical ? ResolveGlyph(GlyphStyle.ScrollBarVerticalDecrease) : ResolveGlyph(GlyphStyle.ScrollBarHorizontalDecrease);
        char incGlyph = scrollBar.Vertical ? ResolveGlyph(GlyphStyle.ScrollBarVerticalIncrease) : ResolveGlyph(GlyphStyle.ScrollBarHorizontalIncrease);

        if (scrollBar.Vertical)
        {
            ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Y,
                decGlyph,
                !scrollBar.Enabled ? scrollBar.DisabledColor : (scrollBar.IsHoveringDecreaseArrow() ? scrollBar.ArrowHotColor : scrollBar.ArrowColor),
                !scrollBar.Enabled ? bg : (scrollBar.IsHoveringDecreaseArrow() ? scrollBar.ArrowHotBackColor : bg));
            ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Bottom - 1,
                incGlyph,
                !scrollBar.Enabled ? scrollBar.DisabledColor : (scrollBar.IsHoveringIncreaseArrow() ? scrollBar.ArrowHotColor : scrollBar.ArrowColor),
                !scrollBar.Enabled ? bg : (scrollBar.IsHoveringIncreaseArrow() ? scrollBar.ArrowHotBackColor : bg));
        }
        else
        {
            ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Y,
                decGlyph,
                !scrollBar.Enabled ? scrollBar.DisabledColor : (scrollBar.IsHoveringDecreaseArrow() ? scrollBar.ArrowHotColor : scrollBar.ArrowColor),
                !scrollBar.Enabled ? bg : (scrollBar.IsHoveringDecreaseArrow() ? scrollBar.ArrowHotBackColor : bg));
            ConsoleFramework.Canvas.OutTextXY(dim.Right - 1, dim.Y,
                incGlyph,
                !scrollBar.Enabled ? scrollBar.DisabledColor : (scrollBar.IsHoveringIncreaseArrow() ? scrollBar.ArrowHotColor : scrollBar.ArrowColor),
                !scrollBar.Enabled ? bg : (scrollBar.IsHoveringIncreaseArrow() ? scrollBar.ArrowHotBackColor : bg));
        }

        if (thumbLen > 0)
        {
            bool hot = scrollBar.IsHoveringThumb() || scrollBar.IsDraggingThumb();
            ConsoleColor fg = !scrollBar.Enabled ? scrollBar.DisabledColor : (hot ? scrollBar.ThumbHotColor : scrollBar.ThumbColor);
            ConsoleColor tbg = !scrollBar.Enabled ? thumbBase : (hot ? scrollBar.ThumbHotBackColor : thumbBase);
            if (scrollBar.Vertical)
            {
                for (int i = 0; i < thumbLen; i++)
                {
                    ConsoleFramework.Canvas.OutTextXY(dim.X, dim.Y + thumbPos + i, ResolveGlyph(GlyphStyle.ScrollBarThumb), fg, tbg);
                }
            }
            else
            {
                for (int i = 0; i < thumbLen; i++)
                {
                    ConsoleFramework.Canvas.OutTextXY(dim.X + thumbPos + i, dim.Y, ResolveGlyph(GlyphStyle.ScrollBarThumb), fg, tbg);
                }
            }
        }
    }

    public void DrawTextBox(IControl textBoxControl)
    {
        if (textBoxControl is not TextBox textBox)
        {
            return;
        }

        textBox.UpdateBlinkState();
        Rectangle r = textBox.RealDim;
        for (int y = 0; y < textBox.size.Height; y++)
        {
            string line = textBox.GetDisplayLine(textBox.GetFirstVisibleLine() + y);
            string lineOut = line.Length > textBox.size.Width ? line.Substring(0, textBox.size.Width) : line.PadRight(textBox.size.Width, ' ');
            ConsoleColor fg = textBox.Enabled ? textBox.ForeColor : textBox.DisabledForeColor;
            for (int x = 0; x < textBox.size.Width; x++)
            {
                ConsoleFramework.Canvas.OutTextXY(r.Left + x, r.Top + y, lineOut[x], fg, textBox.BackColor);
            }
        }

        if (textBox.Active && textBox.Enabled && textBox.GetCaretLine() >= textBox.GetFirstVisibleLine() && textBox.GetCaretLine() < textBox.GetFirstVisibleLine() + textBox.size.Height)
        {
            int cy = textBox.GetCaretLine() - textBox.GetFirstVisibleLine();
            int cx = Math.Min(textBox.GetCaretColumn(), textBox.size.Width - 1);
            if (textBox.ShouldShowCaret())
            {
                string caretLine = textBox.GetDisplayLine(textBox.GetCaretLine());
                char caretChar = cx < caretLine.Length ? caretLine[cx] : ' ';
                ConsoleFramework.Canvas.OutTextXY(r.Left + cx, r.Top + cy, caretChar, textBox.BackColor, textBox.CaretColor);
            }
        }
    }

    public void DrawTerminal(IControl terminalControl)
    {
        if (terminalControl is not Terminal terminal)
        {
            return;
        }

        char[]? border = ResolveBorder(terminal.BorderDefinition);
        if (border != null && border.Length > 5)
        {
            ConsoleFramework.Canvas.DrawRect(terminal.RealDim, terminal.BorderColor, terminal.BackColor, border);
        }

        Rectangle r = terminal.RealDim;
        for (int y = 0; y < terminal.size.Height - 2; y++)
        {
            for (int x = 0; x < terminal.size.Width - 2; x++)
            {
                ScreenCell cell = terminal.GetScreenCell(x, y);
                ConsoleFramework.Canvas.OutTextXY(r.Left + 1 + x, r.Top + 1 + y, cell.c, cell.fc.Fg, cell.fc.Bg);
            }
        }

        foreach (Control child in terminal.Children.OfType<Control>())
        {
            if (!child.Visible)
            {
                continue;
            }

            if (child.Shadow)
            {
                Rectangle shadowDimension = child.Dimension;
                shadowDimension.Offset(1, 1);
                shadowDimension.Offset(terminal.Position);
                ConsoleFramework.Canvas.FillRect(terminal.RealDimOf(shadowDimension), ConsoleColor.DarkGray, ConsoleColor.Black, ResolveGlyph(GlyphStyle.ShadowFill));
            }

            child.Draw();
        }
    }

    public void RedrawTerminal(IControl terminalControl, Rectangle dimension)
    {
        if (terminalControl is not Terminal terminal)
        {
            return;
        }

        char[]? border = ResolveBorder(terminal.BorderDefinition);
        if (dimension.IsEmpty)
        {
            return;
        }

        Rectangle innerRect = terminal.Dimension;
        innerRect.Inflate(-1, -1);
        Rectangle clip = dimension;
        clip.Intersect(innerRect);
        try
        {
            if (border != null && border.Length > 5 && terminal.Dimension.IntersectsWith(dimension) &&
                !(innerRect.Contains(dimension.Location) && innerRect.Contains(Point.Subtract(Point.Add(dimension.Location, dimension.Size), new Size(1, 1)))))
            {
                ConsoleFramework.Canvas.DrawRect(terminal.RealDim, terminal.BorderColor, terminal.BackColor, border);
            }

            Rectangle r = terminal.RealDim;
            for (int y = 0; y < terminal.size.Height - 2; y++)
            {
                for (int x = 0; x < terminal.size.Width - 2; x++)
                {
                    if (clip.Contains(x + 1 + terminal.Position.X, y + 1 + terminal.Position.Y))
                    {
                        ScreenCell cell = terminal.GetScreenCell(x, y);
                        ConsoleFramework.Canvas.OutTextXY(r.Left + 1 + x, r.Top + 1 + y, cell.c, cell.fc.Fg, cell.fc.Bg);
                    }
                }
            }

            foreach (Control child in terminal.Children.OfType<Control>())
            {
                if (!child.Visible)
                {
                    continue;
                }

                if (child.Shadow)
                {
                    Rectangle shadowDimension = child.Dimension;
                    shadowDimension.Offset(1, 1);
                    shadowDimension.Offset(terminal.Position);
                    shadowDimension.Intersect(dimension);
                    ConsoleFramework.Canvas.FillRect(terminal.RealDimOf(shadowDimension), ConsoleColor.DarkGray, ConsoleColor.Black, ResolveGlyph(GlyphStyle.ShadowFill));
                }

                Rectangle childClip = dimension;
                childClip.Location = Point.Subtract(dimension.Location, (Size)terminal.Dimension.Location);
                child.ReDraw(childClip);
            }

            terminal.Valid = true;
        }
        catch
        {
        }
    }

    public void SetTitle(string v)
    {
        console.Title = v;
    }
}
