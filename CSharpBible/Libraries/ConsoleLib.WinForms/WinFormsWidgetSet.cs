using ConsoleLib.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using WinFormsApplication = System.Windows.Forms.Application;
using WinFormsButton = System.Windows.Forms.Button;
using WinFormsControl = System.Windows.Forms.Control;
using WinFormsDockStyle = System.Windows.Forms.DockStyle;
using WinFormsHScrollBar = System.Windows.Forms.HScrollBar;
using WinFormsLabel = System.Windows.Forms.Label;
using WinFormsListBox = System.Windows.Forms.ListBox;
using WinFormsMenuStrip = System.Windows.Forms.MenuStrip;
using WinFormsMouseButtons = System.Windows.Forms.MouseButtons;
using WinFormsPanel = System.Windows.Forms.Panel;
using WinFormsRichTextBox = System.Windows.Forms.RichTextBox;
using WinFormsTextBox = System.Windows.Forms.TextBox;
using WinFormsToolStripItem = System.Windows.Forms.ToolStripItem;
using WinFormsToolStripMenuItem = System.Windows.Forms.ToolStripMenuItem;
using WinFormsToolStripSeparator = System.Windows.Forms.ToolStripSeparator;
using WinFormsVScrollBar = System.Windows.Forms.VScrollBar;

namespace ConsoleLib.WinForms;

/// <summary>
/// WinForms-backed widget set that materializes native controls for ConsoleLib descriptions.
/// </summary>
public sealed class WinFormsWidgetSet : IHostedWidgetSet
{
    private readonly NativeWidgetRegistry _registry = new();
    private readonly object _synchronizationGate = new();
    private readonly Dictionary<IControl, WinFormsMenuStrip> _menuStrips = new();
    private readonly Dictionary<IControl, WinFormsToolStripItem> _menuItems = new();
    private WinFormsHostForm? _hostForm;

    /// <inheritdoc/>
    public void InitializeApplication(IApplication application)
    {
        _hostForm ??= new WinFormsHostForm(application);
    }

    /// <inheritdoc/>
    public void AttachControl(IControl control)
    {
        if (_hostForm == null)
        {
            return;
        }

        if (TryAttachMenuControl(control))
        {
            SynchronizeControl(control);
            return;
        }

        if (_registry.TryGetWidget(control, out _))
        {
            return;
        }

        WinFormsControl widget = CreateWidget(control);
        _registry.Register(control, widget);
        AttachToParent(control, widget);
        SynchronizeControl(control);
    }

    /// <inheritdoc/>
    public void DetachControl(IControl control)
    {
        if (TryDetachMenuControl(control))
        {
            return;
        }

        if (_registry.Remove(control, out WinFormsControl? widget) && widget != null)
        {
            widget.Parent?.Controls.Remove(widget);
            widget.Dispose();
        }
    }

    /// <inheritdoc/>
    public void SynchronizeControl(IControl control)
    {
        if (TrySynchronizeMenuControl(control))
        {
            return;
        }

        if (!_registry.TryGetWidget(control, out WinFormsControl widget))
        {
            return;
        }

        widget.Visible = control.Visible;
        widget.Enabled = control.Enabled;
        widget.Text = control.Text ?? string.Empty;
        Rectangle pixelBounds = WinFormsHostForm.ToPixelBounds(control.Dimension);
        widget.Left = pixelBounds.X;
        widget.Top = pixelBounds.Y;
        widget.Width = pixelBounds.Width;
        widget.Height = pixelBounds.Height;
        widget.BackColor = ToColor(control.BackColor);
        widget.ForeColor = ToColor(control.ForeColor);

        if (control is CommonControls.TextBox textBox && widget is WinFormsTextBox nativeTextBox)
        {
            if (nativeTextBox.Multiline != textBox.MultiLine)
            {
                nativeTextBox.Multiline = textBox.MultiLine;
            }

            if (!string.Equals(nativeTextBox.Text, textBox.Text ?? string.Empty, StringComparison.Ordinal))
            {
                nativeTextBox.Text = textBox.Text ?? string.Empty;
            }
        }

        if (control is CommonControls.Button && widget is WinFormsButton nativeButton)
        {
            nativeButton.Text = control.Text ?? string.Empty;
        }

        if (control is CommonControls.Label && widget is WinFormsLabel nativeLabel)
        {
            nativeLabel.AutoSize = false;
            nativeLabel.Text = control.Text ?? string.Empty;
        }

        if (control is CommonControls.Pixel && widget is WinFormsLabel nativePixel)
        {
            nativePixel.AutoSize = false;
            nativePixel.Text = control.Text ?? string.Empty;
            nativePixel.TextAlign = ContentAlignment.MiddleCenter;
        }

        if (control is CommonControls.ListBox listBox && widget is WinFormsListBox nativeListBox)
        {
            SynchronizeListBoxItems(listBox, nativeListBox);
            if (nativeListBox.SelectedIndex != listBox.GetSelectedIndex())
            {
                nativeListBox.SelectedIndex = listBox.GetSelectedIndex();
            }
        }

        if (control is CommonControls.ScrollBar scrollBar)
        {
            SynchronizeScrollBar(scrollBar, widget);
        }

        if (control is CommonControls.Terminal terminal && widget is WinFormsRichTextBox nativeTerminal)
        {
            if (!string.Equals(nativeTerminal.Text, terminal.Text ?? string.Empty, StringComparison.Ordinal))
            {
                nativeTerminal.Text = terminal.Text ?? string.Empty;
            }
        }
    }

    /// <inheritdoc/>
    public void RunApplication(IApplication application)
    {
        _hostForm ??= new WinFormsHostForm(application);
        application.SetRunning(true);
        AttachControl(application);
        SynchronizeControl(application);
        WinFormsApplication.Run(_hostForm);
    }

    /// <inheritdoc/>
    public void StopApplication(IApplication application)
    {
        application.SetRunning(false);
        _hostForm?.Close();
    }

    /// <inheritdoc/>
    public void DrawControl(IControl control)
    {
        SynchronizeControl(control);
    }

    /// <inheritdoc/>
    public void DrawLabel(IControl label)
    {
        SynchronizeControl(label);
    }

    /// <inheritdoc/>
    public void DrawPixel(IControl pixel)
    {
        SynchronizeControl(pixel);
    }

    /// <inheritdoc/>
    public void DrawPanel(IGroupControl panel)
    {
        SynchronizeControl(panel);
    }

    /// <inheritdoc/>
    public void RedrawPanel(IGroupControl panel, Rectangle dimension)
    {
        SynchronizeControl(panel);
    }

    /// <inheritdoc/>
    public void DrawMenuItem(IControl menuItem)
    {
        SynchronizeControl(menuItem);
    }

    /// <inheritdoc/>
    public void DrawMenuBar(IGroupControl menuBar)
    {
        SynchronizeControl(menuBar);
    }

    /// <inheritdoc/>
    public void DrawListBox(IControl listBox)
    {
        SynchronizeControl(listBox);
    }

    /// <inheritdoc/>
    public void DrawScrollBar(IControl scrollBar)
    {
        SynchronizeControl(scrollBar);
    }

    /// <inheritdoc/>
    public void DrawTextBox(IControl textBox)
    {
        SynchronizeControl(textBox);
    }

    /// <inheritdoc/>
    public void DrawTerminal(IControl terminal)
    {
        SynchronizeControl(terminal);
    }

    /// <inheritdoc/>
    public void RedrawTerminal(IControl terminal, Rectangle dimension)
    {
        SynchronizeControl(terminal);
    }

    /// <inheritdoc/>
    public Rectangle ClipRect => new Rectangle(0,0,80,35);

    private WinFormsControl CreateWidget(IControl control)
    {
        return control switch
        {
            IApplication => new WinFormsPanel(),
            CommonControls.TextBox textBox => new WinFormsTextBox { Multiline = textBox.MultiLine },
            CommonControls.ListBox => new WinFormsListBox(),
            CommonControls.ScrollBar scrollBar => scrollBar.Vertical ? new WinFormsVScrollBar() : new WinFormsHScrollBar(),
            CommonControls.Label => new WinFormsLabel(),
            CommonControls.Button => new WinFormsButton(),
            CommonControls.Pixel => new WinFormsLabel { AutoSize = false },
            CommonControls.Terminal => new WinFormsRichTextBox { ReadOnly = true },
            CommonControls.Panel => new WinFormsPanel(),
            _ => new WinFormsPanel()
        };
    }

    private bool TryAttachMenuControl(IControl control)
    {
        if (_hostForm == null)
        {
            return false;
        }

        if (control is CommonControls.MenuBar menuBar)
        {
            if (_menuStrips.ContainsKey(control))
            {
                return true;
            }

            var menuStrip = new WinFormsMenuStrip();
            _menuStrips[control] = menuStrip;
            _hostForm.MainMenuStrip = menuStrip;
            _hostForm.Controls.Add(menuStrip);
            return true;
        }

        if (control is CommonControls.MenuItem menuItem)
        {
            if (_menuItems.ContainsKey(control))
            {
                return true;
            }

            WinFormsToolStripItem nativeItem = menuItem.IsSeparator
                ? new WinFormsToolStripSeparator()
                : new WinFormsToolStripMenuItem();

            _menuItems[control] = nativeItem;
            AttachMenuItemToParent(menuItem, nativeItem);
            ConfigureMenuItem(menuItem, nativeItem);
            return true;
        }

        if (control is CommonControls.MenuPopup)
        {
            if (control is CommonControls.MenuPopup popup)
            {
                foreach (IControl child in popup.Children)
                {
                    AttachControl(child);
                }
            }

            return true;
        }

        return false;
    }

    private void AttachMenuItemToParent(CommonControls.MenuItem menuItem, WinFormsToolStripItem nativeItem)
    {
        if (menuItem.Parent is CommonControls.MenuBar 
            && _menuStrips.TryGetValue(menuItem.Parent as IControl, out WinFormsMenuStrip? menuStrip))
        {
            if (!menuStrip.Items.Contains(nativeItem))
            {
                menuStrip.Items.Add(nativeItem);
            }

            return;
        }

        if (menuItem.Parent is CommonControls.MenuPopup popup)
        {
            CommonControls.MenuItem? owner = FindMenuPopupOwner(popup);
            if (owner != null && _menuItems.TryGetValue(owner, out WinFormsToolStripItem? ownerItem) && ownerItem is WinFormsToolStripMenuItem ownerMenuItem)
            {
                if (!ownerMenuItem.DropDownItems.Contains(nativeItem))
                {
                    ownerMenuItem.DropDownItems.Add(nativeItem);
                }
            }
        }
    }

    private bool TryDetachMenuControl(IControl control)
    {
        if (_menuItems.TryGetValue(control, out WinFormsToolStripItem? menuItem))
        {
            _menuItems.Remove(control);
            menuItem.Owner?.Items.Remove(menuItem);
            menuItem.Dispose();
            return true;
        }

        if (_menuStrips.TryGetValue(control, out WinFormsMenuStrip? menuStrip))
        {
            _menuStrips.Remove(control);
            if (_hostForm?.MainMenuStrip == menuStrip)
            {
                _hostForm.MainMenuStrip = null;
            }

            _hostForm?.Controls.Remove(menuStrip);
            menuStrip.Dispose();
            return true;
        }

        if (control is CommonControls.MenuPopup popupControl)
        {
            SynchronizeMenuPopup(popupControl);
            return true;
        }

        return false;
    }

    private bool TrySynchronizeMenuControl(IControl control)
    {
        if (control is CommonControls.MenuBar && _menuStrips.TryGetValue(control, out WinFormsMenuStrip? menuStrip))
        {
            menuStrip.Visible = control.Visible;
            menuStrip.Enabled = control.Enabled;
            menuStrip.BackColor = ToColor(control.BackColor);
            menuStrip.ForeColor = ToColor(control.ForeColor);
            return true;
        }

        if (control is CommonControls.MenuItem menuItem && _menuItems.TryGetValue(control, out WinFormsToolStripItem? nativeItem))
        {
            nativeItem.Visible = menuItem.Visible;
            nativeItem.Enabled = menuItem.Enabled;
            if (!menuItem.IsSeparator)
            {
                nativeItem.Text = menuItem.Text ?? string.Empty;

                if (nativeItem is WinFormsToolStripMenuItem nativeMenuItem)
                {
                    bool showDropDown = menuItem.SubMenu?.Visible ?? false;
                    if (!showDropDown)
                    {
                        nativeMenuItem.HideDropDown();
                    }
                }
            }

            return true;
        }

        return control is CommonControls.MenuPopup;
    }

    private static void ConfigureMenuItem(CommonControls.MenuItem menuItem, WinFormsToolStripItem nativeItem)
    {
        if (nativeItem is WinFormsToolStripMenuItem nativeMenuItem)
        {
            nativeMenuItem.Click += (_, _) => menuItem.Click();
        }
    }

    private void SynchronizeMenuPopup(CommonControls.MenuPopup popup)
    {
        foreach (IControl child in popup.Children)
        {
            AttachControl(child);
            SynchronizeControl(child);
        }

        CommonControls.MenuItem? owner = FindMenuPopupOwner(popup);
        if (owner == null || !_menuItems.TryGetValue(owner, out WinFormsToolStripItem? ownerItem) || ownerItem is not WinFormsToolStripMenuItem nativeOwnerItem)
        {
            return;
        }

        if (popup.Visible)
        {
            if (!nativeOwnerItem.DropDown.Visible)
            {
                nativeOwnerItem.ShowDropDown();
            }
        }

        else if (nativeOwnerItem.DropDown.Visible)
        {
            nativeOwnerItem.HideDropDown();
        }
    }

    private CommonControls.MenuItem? FindMenuPopupOwner(CommonControls.MenuPopup popup)
    {
        foreach (var pair in _menuItems)
        {
            if (pair.Key is CommonControls.MenuItem menuItem && ReferenceEquals(menuItem.SubMenu, popup))
            {
                return menuItem;
            }
        }

        return null;
    }

    private void AttachToParent(IControl control, WinFormsControl widget)
    {
        ConfigureWidget(control, widget);

        if (control is IApplication && _hostForm != null)
        {
            widget.Dock = WinFormsDockStyle.Fill;
            _hostForm.Controls.Add(widget);
            return;
        }

        if (control.Parent is IControl parent && _registry.TryGetWidget(parent, out WinFormsControl parentWidget))
        {
            parentWidget.Controls.Add(widget);
        }
        else if (_hostForm != null)
        {
            _hostForm.Controls.Add(widget);
        }
    }

    private static Color ToColor(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => Color.Black,
            ConsoleColor.DarkBlue => Color.DarkBlue,
            ConsoleColor.DarkGreen => Color.DarkGreen,
            ConsoleColor.DarkCyan => Color.DarkCyan,
            ConsoleColor.DarkRed => Color.DarkRed,
            ConsoleColor.DarkMagenta => Color.DarkMagenta,
            ConsoleColor.DarkYellow => Color.Olive,
            ConsoleColor.Gray => Color.Gray,
            ConsoleColor.DarkGray => Color.DarkGray,
            ConsoleColor.Blue => Color.Blue,
            ConsoleColor.Green => Color.Green,
            ConsoleColor.Cyan => Color.Cyan,
            ConsoleColor.Red => Color.Red,
            ConsoleColor.Magenta => Color.Magenta,
            ConsoleColor.Yellow => Color.Yellow,
            ConsoleColor.White => Color.White,
            _ => Color.Transparent
        };
    }

    private void ConfigureWidget(IControl control, WinFormsControl widget)
    {
        switch (control)
        {
            case IApplication application:
                widget.MouseMove += (_, e) => application.RaiseMouseEvent(new WinFormsMouseEventAdapter(e, widget));
                widget.MouseWheel += (_, e) => application.RaiseMouseEvent(new WinFormsMouseEventAdapter(e, widget));
                widget.MouseDown += (_, e) => application.RaiseMouseEvent(new WinFormsMouseEventAdapter(e, widget));
                widget.MouseUp += (_, e) => application.RaiseMouseEvent(new WinFormsMouseEventAdapter(e, widget));
                break;

            case CommonControls.Button button when widget is WinFormsButton nativeButton:
                nativeButton.Click += (_, _) => button.Click();
                break;

            case CommonControls.MenuItem menuItem when widget is WinFormsButton nativeMenuItemButton:
                nativeMenuItemButton.Click += (_, _) => menuItem.Click();
                break;

            case CommonControls.TextBox textBox when widget is WinFormsTextBox nativeTextBox:
                nativeTextBox.TextChanged += (_, _) =>
                {
                    lock (_synchronizationGate)
                    {
                        if (!string.Equals(textBox.Text, nativeTextBox.Text, StringComparison.Ordinal))
                        {
                            textBox.ApplyNativeText(nativeTextBox.Text);
                        }
                    }
                };
                break;

            case CommonControls.ListBox listBox when widget is WinFormsListBox nativeListBox:
                nativeListBox.SelectedIndexChanged += (_, _) =>
                {
                    lock (_synchronizationGate)
                    {
                        if (nativeListBox.SelectedIndex != listBox.GetSelectedIndex())
                        {
                            listBox.ApplyNativeSelectedIndex(nativeListBox.SelectedIndex);
                        }
                    }
                };
                break;

            case CommonControls.ScrollBar scrollBar when widget is WinFormsVScrollBar nativeVScrollBar:
                nativeVScrollBar.ValueChanged += (_, _) => ApplyScrollBarValue(scrollBar, nativeVScrollBar.Value);
                break;

            case CommonControls.ScrollBar scrollBar when widget is WinFormsHScrollBar nativeHScrollBar:
                nativeHScrollBar.ValueChanged += (_, _) => ApplyScrollBarValue(scrollBar, nativeHScrollBar.Value);
                break;
        }
    }

    private void SynchronizeListBoxItems(CommonControls.ListBox listBox, WinFormsListBox nativeListBox)
    {
        IList? itemsSource = listBox.GetItemsSource();
        nativeListBox.BeginUpdate();
        try
        {
            nativeListBox.Items.Clear();
            if (itemsSource == null)
            {
                return;
            }

            foreach (object? item in itemsSource)
            {
                nativeListBox.Items.Add(item);
            }
        }
        finally
        {
            nativeListBox.EndUpdate();
        }
    }

    private void SynchronizeScrollBar(CommonControls.ScrollBar scrollBar, WinFormsControl widget)
    {
        if (widget is WinFormsVScrollBar vertical)
        {
            vertical.Minimum = scrollBar.Minimum;
            vertical.Maximum = scrollBar.Maximum + scrollBar.LargeChange - 1;
            vertical.LargeChange = Math.Max(1, scrollBar.LargeChange);
            vertical.Value = Math.Max(vertical.Minimum, Math.Min(scrollBar.Value, vertical.Maximum - vertical.LargeChange + 1));
        }
        else if (widget is WinFormsHScrollBar horizontal)
        {
            horizontal.Minimum = scrollBar.Minimum;
            horizontal.Maximum = scrollBar.Maximum + scrollBar.LargeChange - 1;
            horizontal.LargeChange = Math.Max(1, scrollBar.LargeChange);
            horizontal.Value = Math.Max(horizontal.Minimum, Math.Min(scrollBar.Value, horizontal.Maximum - horizontal.LargeChange + 1));
        }
    }

    private void ApplyScrollBarValue(CommonControls.ScrollBar scrollBar, int value)
    {
        lock (_synchronizationGate)
        {
            if (scrollBar.Value != value)
            {
                scrollBar.Value = value;
            }
        }
    }

    private sealed class WinFormsMouseEventAdapter :  IMouseEvent
    {
        public WinFormsMouseEventAdapter(System.Windows.Forms.MouseEventArgs args, WinFormsControl source)
        {
            Point location = source.PointToClient(args.Location);
            MousePos = new Point(
                Math.Max(0, location.X / WinFormsHostForm.CharacterWidth),
                Math.Max(0, location.Y / WinFormsHostForm.CharacterHeight));
            MouseButtonLeft = args.Button.HasFlag(WinFormsMouseButtons.Left);
            MouseButtonRight = args.Button.HasFlag(WinFormsMouseButtons.Right);
            MouseButtonMiddle = args.Button.HasFlag(WinFormsMouseButtons.Middle);
            MouseWheel = args.Delta;
            MouseMoved = args.Button == WinFormsMouseButtons.None && args.Delta == 0;
            ButtonEvent = args.Button != WinFormsMouseButtons.None;
        }

        public Point MousePos { get; }
        public bool MouseButtonLeft { get; }
        public bool MouseButtonRight { get; }
        public bool MouseButtonMiddle { get; }
        public int MouseWheel { get; }
        public bool MouseMoved { get; }
        public bool ButtonEvent { get; }
        public bool Handled { get; set; }
    }
}