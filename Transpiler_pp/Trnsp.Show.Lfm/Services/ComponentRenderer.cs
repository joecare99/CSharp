using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Interface for rendering LFM components as WPF controls.
/// </summary>
public interface IComponentRenderer
{
    /// <summary>
    /// Renders a component as a WPF UIElement.
    /// </summary>
    UIElement Render(LfmComponentBase component);
}

/// <summary>
/// Renders LFM components as WPF controls.
/// </summary>
public class ComponentRenderer : IComponentRenderer
{
    public UIElement Render(LfmComponentBase component)
    {
        // Order matters: more specific types first (inheritance hierarchy)
        var element = component switch
        {
            TForm form => RenderForm(form),
          
            TStaticText staticText => RenderStaticText(staticText),  // Before TLabel
            TLabel label => RenderLabel(label),
            
            TLabeledEdit labeledEdit => RenderLabeledEdit(labeledEdit),  // Before TEdit
            TMemo memo => RenderMemo(memo),  // TMemo before TEdit
            TEdit edit => RenderEdit(edit),
           
            TBitBtn bitBtn => RenderBitBtn(bitBtn),  // TBitBtn before TButton
            TButton button => RenderButton(button),
            TSpeedButton speedBtn => RenderSpeedButton(speedBtn),
           
            TScrollBox scrollBox => RenderScrollBox(scrollBox),  // Before TPanel
            TTabSheet tabSheet => RenderTabSheet(tabSheet),  // Before TPanel
            TPageControl pageControl => RenderPageControl(pageControl),  // Tab controls
            TPanel panel => RenderPanel(panel),
            TRadioGroup radioGroup => RenderRadioGroup(radioGroup),  // Before TGroupBox
            TCheckGroup checkGroup => RenderCheckGroup(checkGroup),  // Before TGroupBox
            TGroupBox groupBox => RenderGroupBox(groupBox),
          
            TCheckBox checkBox => RenderCheckBox(checkBox),
            TRadioButton radioBtn => RenderRadioButton(radioBtn),
           
            TComboBox comboBox => RenderComboBox(comboBox),
            TListBox listBox => RenderListBox(listBox),
            TTrackBar trackBar => RenderTrackBar(trackBar),
            TProgressBar progressBar => RenderProgressBar(progressBar),
            TFloatSpinEdit floatSpin => RenderFloatSpinEdit(floatSpin),  // Before TSpinEdit
            TSpinEdit spinEdit => RenderSpinEdit(spinEdit),
            TUpDown upDown => RenderUpDown(upDown),
            TStringGrid stringGrid => RenderStringGrid(stringGrid),  // Before TDrawGrid
            TDrawGrid drawGrid => RenderDrawGrid(drawGrid),
            TShape shape => RenderShape(shape),
            TBevel bevel => RenderBevel(bevel),
            TSplitter splitter => RenderSplitter(splitter),
            TPaintBox paintBox => RenderPaintBox(paintBox),  // Before TImage
            TImage image => RenderImage(image),
            TStatusBar statusBar => RenderStatusBar(statusBar),
            TToolBar toolBar => RenderToolBar(toolBar),
            TToolButton toolBtn => RenderToolButton(toolBtn),

            TPopupMenu popupMenu => RenderNonVisual(popupMenu, "📋"),  // PopupMenu still non-visual
            TMainMenu mainMenu => RenderMainMenu(mainMenu),  // MainMenu now rendered as menu bar
            TMenuItem menuItem => RenderMenuItemStandalone(menuItem),
           
            TFileSaveAs fileSaveAs => RenderNonVisual(fileSaveAs, "💾"),
            TFileOpen fileOpen => RenderNonVisual(fileOpen, "📂"),
            TFileExit fileExit => RenderNonVisual(fileExit, "🚪"),
            TAction action => RenderNonVisual(action, "▶"),
            TActionList actionList => RenderNonVisual(actionList, "⚡"),
            TImageList imageList => RenderNonVisual(imageList, "🖼"),
            TTimer timer => RenderNonVisual(timer, "⏱"),
            TUnknownComponent unknown => RenderNonVisual(unknown, "?"),
            _ => RenderUnknown(component)
        };

        // Set common properties
        if (element is FrameworkElement fe)
        {
            Canvas.SetLeft(fe, component.Left);
            Canvas.SetTop(fe, component.Top);
            fe.Width = component.Width;
            fe.Height = component.Height;
            // Use EffectiveHint for tooltip (considers linked action)
            fe.ToolTip = string.IsNullOrEmpty(component.EffectiveHint) ? null : component.EffectiveHint;
            fe.IsEnabled = component.Enabled;
            fe.Visibility = component.Visible ? Visibility.Visible : Visibility.Collapsed;
            
            // Store reference to component
            fe.Tag = component;
        }

        return element;
    }

    private UIElement RenderForm(TForm form)
    {
        var border = new Border
        {
            Background = new SolidColorBrush(form.Color),
            BorderBrush = Brushes.DarkGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(28) }); // Title bar
        
        // Check if form has a MainMenu
        var mainMenu = form.Children.OfType<TMainMenu>().FirstOrDefault();
        if (mainMenu != null)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Menu bar
        }
        
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Client area

        int currentRow = 0;

        // Title bar
        var titleBar = new Border
        {
            Background = new LinearGradientBrush(
                Color.FromRgb(0, 120, 215),
                Color.FromRgb(0, 90, 180),
                90),
            CornerRadius = new CornerRadius(3, 3, 0, 0)
        };
        var titleText = new TextBlock
        {
            Text = form.Caption,
            Foreground = Brushes.White,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };
        titleBar.Child = titleText;
        Grid.SetRow(titleBar, currentRow++);
        grid.Children.Add(titleBar);

        // Menu bar (if present)
        if (mainMenu != null)
        {
            var menuBar = CreateMenuBar(mainMenu);
            Grid.SetRow(menuBar, currentRow++);
            grid.Children.Add(menuBar);
        }

        // Client area
        var clientCanvas = new Canvas
        {
            Background = new SolidColorBrush(form.Color),
            ClipToBounds = true
        };
        Grid.SetRow(clientCanvas, currentRow);
        grid.Children.Add(clientCanvas);

        // Render children (except MainMenu which is handled above)
        foreach (var child in form.Children)
        {
            if (child is TMainMenu)
                continue; // Already rendered as menu bar
                
            var childElement = Render(child);
            clientCanvas.Children.Add(childElement);
        }

        border.Child = grid;
        return border;
    }

    /// <summary>
    /// Creates a WPF Menu from a TMainMenu component.
    /// </summary>
    private Menu CreateMenuBar(TMainMenu mainMenu)
    {
        var menu = new Menu
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            Padding = new Thickness(0)
        };

        // Get menu items from Children (TMenuItem children of TMainMenu)
        foreach (var child in mainMenu.Children.OfType<TMenuItem>())
        {
            var menuItem = CreateMenuItem(child);
            menu.Items.Add(menuItem);
        }

        return menu;
    }

    /// <summary>
    /// Creates a WPF MenuItem from a TMenuItem component (recursive for submenus).
    /// </summary>
    private MenuItem CreateMenuItem(TMenuItem lfmItem)
    {
        if (lfmItem.IsSeparator || lfmItem.EffectiveCaption == "-")
        {
            // Return a separator-like menu item
            return new MenuItem
            {
                Header = new Separator(),
                IsEnabled = false
            };
        }

        var menuItem = new MenuItem
        {
            Header = FormatMenuCaption(lfmItem.EffectiveCaption),
            IsCheckable = lfmItem.MenuItemChecked || lfmItem.RadioItem,
            IsChecked = lfmItem.MenuItemChecked,
            InputGestureText = FormatShortcut(lfmItem.EffectiveShortCut)
        };

        // Add icon - try embedded bitmap first, then ImageList
        var iconSource = lfmItem.GetEffectiveImage();
        if (iconSource != null)
        {
            menuItem.Icon = new Image
            {
                Source = iconSource,
                Width = 16,
                Height = 16
            };
        }

        // Add submenu items from Children
        foreach (var subChild in lfmItem.Children.OfType<TMenuItem>())
        {
            if (subChild.IsSeparator || subChild.EffectiveCaption == "-")
            {
                menuItem.Items.Add(new Separator());
            }
            else
            {
                menuItem.Items.Add(CreateMenuItem(subChild));
            }
        }

        // Also check SubItems list (in case it's used)
        foreach (var subItem in lfmItem.SubItems)
        {
            if (subItem.IsSeparator || subItem.EffectiveCaption == "-")
            {
                menuItem.Items.Add(new Separator());
            }
            else
            {
                menuItem.Items.Add(CreateMenuItem(subItem));
            }
        }

        return menuItem;
    }

    /// <summary>
    /// Formats menu caption, converting & to _ for WPF accelerator keys.
    /// </summary>
    private static string FormatMenuCaption(string caption) =>
        // WPF uses _ for accelerator keys, Delphi uses &
        caption.Replace("&", "_");

    /// <summary>
    /// Formats shortcut string for display.
    /// </summary>
    private static string FormatShortcut(string shortcut)
    {
        if (string.IsNullOrEmpty(shortcut))
            return string.Empty;

        // Convert Delphi shortcut format to readable format
        // e.g., "16463" (Ctrl+O) or "Ctrl+O"
        if (int.TryParse(shortcut, out int keyCode))
        {
            // Decode Delphi shortcut value
            return DecodeDelphiShortcut(keyCode);
        }

        return shortcut;
    }

    /// <summary>
    /// Decodes a Delphi shortcut integer value to a readable string.
    /// </summary>
    private static string DecodeDelphiShortcut(int shortcut)
    {
        if (shortcut == 0) return string.Empty;

        var modifiers = new System.Text.StringBuilder();
        
        // Delphi shortcut modifiers:
        // Shift = $2000 (8192)
        // Ctrl = $4000 (16384)
        // Alt = $8000 (32768)
        
        if ((shortcut & 0x4000) != 0) modifiers.Append("Ctrl+");
        if ((shortcut & 0x2000) != 0) modifiers.Append("Shift+");
        if ((shortcut & 0x8000) != 0) modifiers.Append("Alt+");

        int keyCode = shortcut & 0xFF;
        string keyName = keyCode switch
        {
            >= 65 and <= 90 => ((char)keyCode).ToString(), // A-Z
            >= 48 and <= 57 => ((char)keyCode).ToString(), // 0-9
            112 => "F1", 113 => "F2", 114 => "F3", 115 => "F4",
            116 => "F5", 117 => "F6", 118 => "F7", 119 => "F8",
            120 => "F9", 121 => "F10", 122 => "F11", 123 => "F12",
            13 => "Enter", 27 => "Esc", 32 => "Space",
            8 => "Backspace", 9 => "Tab", 46 => "Del", 45 => "Ins",
            36 => "Home", 35 => "End", 33 => "PgUp", 34 => "PgDn",
            37 => "Left", 38 => "Up", 39 => "Right", 40 => "Down",
            _ => $"Key{keyCode}"
        };

        return modifiers.ToString() + keyName;
    }

    /// <summary>
    /// Renders MainMenu as a standalone component (when not attached to a form).
    /// </summary>
    private UIElement RenderMainMenu(TMainMenu mainMenu)
    {
        var menu = CreateMenuBar(mainMenu);
        menu.Width = double.NaN; // Auto width
        menu.HorizontalAlignment = HorizontalAlignment.Left;
        
        return new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = menu
        };
    }

    /// <summary>
    /// Renders a standalone MenuItem (not part of a menu structure).
    /// </summary>
    private UIElement RenderMenuItemStandalone(TMenuItem menuItem)
    {
        if (menuItem.IsSeparator)
        {
            return new Separator();
        }

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        if (menuItem.BitmapImageSource != null)
        {
            var icon = new Image
            {
                Source = menuItem.BitmapImageSource,
                Width = 16,
                Height = 16,
                Margin = new Thickness(2)
            };
            Grid.SetColumn(icon, 0);
            grid.Children.Add(icon);
        }

        var textBlock = new TextBlock
        {
            Text = menuItem.EffectiveCaption.Replace("&", ""),
            Padding = new Thickness(5, 2, 5, 2),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(textBlock, 1);
        grid.Children.Add(textBlock);

        return new Border
        {
            Background = Brushes.WhiteSmoke,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = grid
        };
    }

    private UIElement RenderLabel(TLabel label) => new TextBlock
    {
        Text = label.Caption,
        Foreground = new SolidColorBrush(label.FontColor),
        FontFamily = new FontFamily(label.FontName),
        FontSize = label.FontSize,
        FontWeight = label.FontWeight,
        FontStyle = label.FontStyle,
        TextAlignment = label.Alignment,
        TextWrapping = label.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
        Background = label.Transparent ? Brushes.Transparent : new SolidColorBrush(label.Color)
    };

    private UIElement RenderStaticText(TStaticText staticText)
    {
        var border = new Border
        {
            BorderBrush = staticText.StaticBorderStyleKind == EStaticBorderStyle.None ? Brushes.Transparent : Brushes.Gray,
            BorderThickness = staticText.StaticBorderStyleKind == EStaticBorderStyle.None ? new Thickness(0) : new Thickness(1),
            Background = staticText.Transparent ? Brushes.Transparent : new SolidColorBrush(staticText.Color)
        };

        border.Child = new TextBlock
        {
            Text = staticText.Caption,
            Foreground = new SolidColorBrush(staticText.FontColor),
            FontFamily = new FontFamily(staticText.FontName),
            FontSize = staticText.FontSize,
            TextAlignment = staticText.Alignment,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(2)
        };

        return border;
    }

    private UIElement RenderEdit(TEdit edit)
    {
        if (edit.PasswordChar != '\0')
        {
            return new PasswordBox
            {
                Password = edit.Caption,
                FontFamily = new FontFamily(edit.FontName),
                FontSize = edit.FontSize,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(2)
            };
        }

        return new TextBox
        {
            Text = edit.Caption,
            IsReadOnly = edit.ReadOnly,
            MaxLength = edit.MaxLength > 0 ? edit.MaxLength : int.MaxValue,
            FontFamily = new FontFamily(edit.FontName),
            FontSize = edit.FontSize,
            TextAlignment = edit.Alignment,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(2)
        };
    }

    private UIElement RenderLabeledEdit(TLabeledEdit labeledEdit)
    {
        var container = new Grid();

        // Determine layout based on label position
        switch (labeledEdit.LabelPosKind)
        {
            case ELabelPosition.Above:
                container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                break;
            case ELabelPosition.Below:
                container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                break;
            case ELabelPosition.Left:
            case ELabelPosition.Right:
                container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                container.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                break;
        }

        var label = new TextBlock
        {
            Text = labeledEdit.LabelCaption,
            FontFamily = new FontFamily(labeledEdit.FontName),
            FontSize = labeledEdit.FontSize,
            Margin = new Thickness(0, 0, 0, labeledEdit.LabelSpacing)
        };

        var textBox = new TextBox
        {
            Text = labeledEdit.Caption,
            FontFamily = new FontFamily(labeledEdit.FontName),
            FontSize = labeledEdit.FontSize,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(2)
        };

        switch (labeledEdit.LabelPosKind)
        {
            case ELabelPosition.Above:
                Grid.SetRow(label, 0);
                Grid.SetRow(textBox, 1);
                break;
            case ELabelPosition.Below:
                Grid.SetRow(textBox, 0);
                Grid.SetRow(label, 1);
                break;
            case ELabelPosition.Left:
                Grid.SetColumn(label, 0);
                Grid.SetColumn(textBox, 1);
                label.Margin = new Thickness(0, 0, labeledEdit.LabelSpacing, 0);
                break;
            case ELabelPosition.Right:
                Grid.SetColumn(textBox, 0);
                Grid.SetColumn(label, 1);
                label.Margin = new Thickness(labeledEdit.LabelSpacing, 0, 0, 0);
                break;
        }

        container.Children.Add(label);
        container.Children.Add(textBox);

        return container;
    }

    private UIElement RenderMemo(TMemo memo) => new TextBox
    {
        Text = memo.Caption,
        AcceptsReturn = true,
        AcceptsTab = true,
        TextWrapping = memo.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
        VerticalScrollBarVisibility = memo.ScrollBars is ScrollStyle.Vertical or ScrollStyle.Both
                ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled,
        HorizontalScrollBarVisibility = memo.ScrollBars is ScrollStyle.Horizontal or ScrollStyle.Both
                ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled,
        FontFamily = new FontFamily(memo.FontName),
        FontSize = memo.FontSize,
        Padding = new Thickness(2)
    };

    private UIElement RenderButton(TButton button)
    {
        var btn = new Button
        {
            Content = button.EffectiveCaption,
            FontFamily = new FontFamily(button.FontName),
            FontSize = button.FontSize,
            Padding = new Thickness(5, 2, 5, 2)
        };

        if (button.Default)
        {
            btn.BorderThickness = new Thickness(2);
            btn.BorderBrush = Brushes.DarkBlue;
        }

        return btn;
    }

    private UIElement RenderBitBtn(TBitBtn bitBtn)
    {
        var btn = new Button
        {
            Padding = new Thickness(5, 2, 5, 2)
        };

        var stack = new StackPanel
        {
            Orientation = bitBtn.Layout is ButtonLayout.Top or ButtonLayout.Bottom 
                ? Orientation.Vertical : Orientation.Horizontal
        };

        UIElement icon;
        
        // Use actual glyph image if available
        if (bitBtn.GlyphImageSource != null)
        {
            icon = new Image
            {
                Source = bitBtn.GlyphImageSource,
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, bitBtn.Spacing, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
        }
        else
        {
            // Fallback to text icon
            icon = new TextBlock
            {
                Text = GetBitBtnIcon(bitBtn.Kind),
                Margin = new Thickness(0, 0, bitBtn.Spacing, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        var text = new TextBlock
        {
            Text = bitBtn.EffectiveCaption,
            VerticalAlignment = VerticalAlignment.Center
        };

        if (bitBtn.Layout is ButtonLayout.Right or ButtonLayout.Bottom)
        {
            stack.Children.Add(text);
            stack.Children.Add(icon);
        }
        else
        {
            stack.Children.Add(icon);
            stack.Children.Add(text);
        }

        btn.Content = stack;
        return btn;
    }

    private static string GetBitBtnIcon(BitBtnKind kind) => kind switch
    {
        BitBtnKind.OK => "✔",
        BitBtnKind.Cancel => "✖",
        BitBtnKind.Help => "❓",
        BitBtnKind.Yes => "✔",
        BitBtnKind.No => "✖",
        BitBtnKind.Close => "✖",
        BitBtnKind.Abort => "⛔",
        BitBtnKind.Retry => "🔄",
        BitBtnKind.Ignore => "➡",
        BitBtnKind.All => "☑",
        _ => ""
    };

    private UIElement RenderSpeedButton(TSpeedButton speedBtn)
    {
        var btn = new ToggleButton
        {
            Content = speedBtn.EffectiveCaption,
            IsChecked = speedBtn.Down,
            Padding = new Thickness(2)
        };

        if (speedBtn.Flat)
        {
            btn.BorderThickness = new Thickness(0);
        }

        return btn;
    }

    private UIElement RenderSpinEdit(TSpinEdit spinEdit)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(17) });

        var textBox = new TextBox
        {
            Text = spinEdit.Value.ToString(),
            TextAlignment = TextAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center,
            IsReadOnly = spinEdit.ReadOnly
        };
        Grid.SetColumn(textBox, 0);

        var upDown = new StackPanel();
        var upBtn = new RepeatButton { Content = "▲", FontSize = 6, Height = 11 };
        var downBtn = new RepeatButton { Content = "▼", FontSize = 6, Height = 11 };
        upDown.Children.Add(upBtn);
        upDown.Children.Add(downBtn);
        Grid.SetColumn(upDown, 1);

        grid.Children.Add(textBox);
        grid.Children.Add(upDown);

        return new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = grid
        };
    }

    private UIElement RenderFloatSpinEdit(TFloatSpinEdit floatSpin)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(17) });

        var textBox = new TextBox
        {
            Text = floatSpin.Value.ToString($"F{floatSpin.DecimalPlaces}"),
            TextAlignment = TextAlignment.Right,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(textBox, 0);

        var upDown = new StackPanel();
        upDown.Children.Add(new RepeatButton { Content = "▲", FontSize = 6, Height = 11 });
        upDown.Children.Add(new RepeatButton { Content = "▼", FontSize = 6, Height = 11 });
        Grid.SetColumn(upDown, 1);

        grid.Children.Add(textBox);
        grid.Children.Add(upDown);

        return new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = grid
        };
    }

    private UIElement RenderUpDown(TUpDown upDown)
    {
        var stack = new StackPanel
        {
            Orientation = upDown.Orientation == UpDownOrientation.Horizontal 
                ? Orientation.Horizontal : Orientation.Vertical
        };

        stack.Children.Add(new RepeatButton { Content = upDown.Orientation == UpDownOrientation.Horizontal ? "◀" : "▲", FontSize = 8 });
        stack.Children.Add(new RepeatButton { Content = upDown.Orientation == UpDownOrientation.Horizontal ? "▶" : "▼", FontSize = 8 });

        return new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }

    private UIElement RenderDrawGrid(TDrawGrid drawGrid)
    {
        var border = new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(drawGrid.Color)
        };

        var grid = new Grid();

        // Create columns
        for (int c = 0; c < Math.Min(drawGrid.ColCount, 20); c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(drawGrid.DefaultColWidth) });
        }

        // Create rows
        for (int r = 0; r < Math.Min(drawGrid.RowCount, 20); r++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(drawGrid.DefaultRowHeight) });
        }

        // Add cells
        for (int r = 0; r < Math.Min(drawGrid.RowCount, 20); r++)
        {
            for (int c = 0; c < Math.Min(drawGrid.ColCount, 20); c++)
            {
                var isFixed = r < drawGrid.FixedRows || c < drawGrid.FixedCols;
                var cell = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Background = isFixed ? new SolidColorBrush(drawGrid.FixedColor) : Brushes.Transparent
                };
                Grid.SetRow(cell, r);
                Grid.SetColumn(cell, c);
                grid.Children.Add(cell);
            }
        }

        border.Child = grid;
        return border;
    }

    private UIElement RenderStringGrid(TStringGrid stringGrid) => RenderDrawGrid(stringGrid);

    private UIElement RenderShape(TShape shape)
    {
        Shape wpfShape = shape.ShapeKind switch
        {
            EShapeType.Ellipse or EShapeType.Circle => new Ellipse(),
            EShapeType.Diamond => CreateDiamond(shape.Width, shape.Height),
            EShapeType.Triangle or EShapeType.TriangleDown or EShapeType.TriangleLeft or EShapeType.TriangleRight 
                => CreateTriangle(shape.ShapeKind, shape.Width, shape.Height),
            _ => new Rectangle()
        };

        wpfShape.Fill = shape.BrushStyleKind == EBrushStyle.Clear 
            ? Brushes.Transparent 
            : new SolidColorBrush(shape.BrushColor);
        wpfShape.Stroke = shape.PenStyleKind == EPenStyle.Clear 
            ? Brushes.Transparent 
            : new SolidColorBrush(shape.PenColor);
        wpfShape.StrokeThickness = shape.PenWidth;

        if (shape.PenStyleKind == EPenStyle.Dash)
            wpfShape.StrokeDashArray = [4, 2];
        else if (shape.PenStyleKind == EPenStyle.Dot)
            wpfShape.StrokeDashArray = [1, 2];
        else if (shape.PenStyleKind == EPenStyle.DashDot)
            wpfShape.StrokeDashArray = [4, 2, 1, 2];

        if (shape.ShapeKind is EShapeType.RoundRect or EShapeType.RoundSquare && wpfShape is Rectangle rect)
        {
            rect.RadiusX = 5;
            rect.RadiusY = 5;
        }

        return wpfShape;
    }

    private static Polygon CreateDiamond(double width, double height) => new Polygon
    {
        Points =
            [
                new Point(width / 2, 0),
                new Point(width, height / 2),
                new Point(width / 2, height),
                new Point(0, height / 2)
            ]
    };

    private static Polygon CreateTriangle(EShapeType type, double width, double height)
    {
        var points = type switch
        {
            EShapeType.Triangle => new PointCollection { new(width / 2, 0), new(width, height), new(0, height) },
            EShapeType.TriangleDown => new PointCollection { new(0, 0), new(width, 0), new(width / 2, height) },
            EShapeType.TriangleLeft => new PointCollection { new(width, 0), new(width, height), new(0, height / 2) },
            EShapeType.TriangleRight => new PointCollection { new(0, 0), new(width, height / 2), new(0, height) },
            _ => new PointCollection { new(width / 2, 0), new(width, height), new(0, height) }
        };

        return new Polygon { Points = points };
    }

    private UIElement RenderBevel(TBevel bevel)
    {
        var lightBrush = bevel.BevelStyleKind == EBevelStyle.Raised ? Brushes.White : Brushes.DarkGray;
        var darkBrush = bevel.BevelStyleKind == EBevelStyle.Raised ? Brushes.DarkGray : Brushes.White;

        return new Border
        {
            BorderBrush = lightBrush,
            BorderThickness = new Thickness(1, 1, 0, 0),
            Child = new Border
            {
                BorderBrush = darkBrush,
                BorderThickness = new Thickness(0, 0, 1, 1)
            }
        };
    }

    private UIElement RenderSplitter(TSplitter splitter) => new GridSplitter
    {
        Background = Brushes.LightGray,
        ShowsPreview = true
    };

    private UIElement RenderStatusBar(TStatusBar statusBar)
    {
        var sp = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
        };

        if (statusBar.SimplePanel)
        {
            sp.Children.Add(new TextBlock
            {
                Text = statusBar.SimpleText,
                Margin = new Thickness(5, 2, 5, 2)
            });
        }
        else
        {
            foreach (var panel in statusBar.Panels)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0, 0, 1, 0),
                    Width = panel.Width,
                    Child = new TextBlock
                    {
                        Text = panel.Text,
                        TextAlignment = panel.Alignment,
                        Margin = new Thickness(3, 2, 3, 2)
                    }
                };
                sp.Children.Add(border);
            }
        }

        return new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(0, 1, 0, 0),
            Child = sp
        };
    }

    private UIElement RenderToolBar(TToolBar toolBar)
    {
        var sp = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
        };

        foreach (var child in toolBar.Children)
        {
            var childElement = Render(child);
            sp.Children.Add(childElement);
        }

        return new Border
        {
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Child = sp,
            Padding = new Thickness(2)
        };
    }

    private UIElement RenderToolButton(TToolButton toolBtn)
    {
        if (toolBtn.Style == ToolButtonStyle.Separator)
        {
            return new Separator
            {
                Width = 6,
                Margin = new Thickness(2, 0, 2, 0)
            };
        }
        
        if (toolBtn.Style == ToolButtonStyle.Divider)
        {
            return new Border
            {
                Width = 2,
                Background = Brushes.Gray,
                Margin = new Thickness(2, 2, 2, 2)
            };
        }

        var btn = toolBtn.Style == ToolButtonStyle.Check
            ? (ButtonBase)new ToggleButton { IsChecked = toolBtn.Down }
            : new Button();

        // TToolButton: Show image from ImageList (not caption from Action!)
        var imageSource = toolBtn.GetImage();
        if (imageSource != null)
        {
            btn.Content = new Image
            {
                Source = imageSource,
                Width = 16,
                Height = 16
            };
        }
        else
        {
            // Fallback: show placeholder icon
            btn.Content = "🔲";
        }

        btn.Padding = new Thickness(4, 2, 4, 2);
        btn.Margin = new Thickness(1);

        return btn;
    }

    private UIElement RenderPanel(TPanel panel)
    {
        var border = new Border
        {
            Background = new SolidColorBrush(panel.Color),
            BorderThickness = GetBevelThickness(panel.BevelOuter, panel.BevelWidth)
        };

        SetBevelBorderBrush(border, panel.BevelOuter);

        var innerBorder = new Border
        {
            BorderThickness = GetBevelThickness(panel.BevelInner, panel.BevelWidth)
        };
        SetBevelBorderBrush(innerBorder, panel.BevelInner);

        var canvas = new Canvas
        {
            Background = Brushes.Transparent,
            ClipToBounds = true
        };

        if (!string.IsNullOrEmpty(panel.Caption))
        {
            var label = new TextBlock
            {
                Text = panel.Caption,
                TextAlignment = panel.Alignment,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };
            canvas.Children.Add(label);
        }

        foreach (var child in panel.Children)
        {
            var childElement = Render(child);
            canvas.Children.Add(childElement);
        }

        innerBorder.Child = canvas;
        border.Child = innerBorder;
        return border;
    }

    private UIElement RenderScrollBox(TScrollBox scrollBox)
    {
        var sv = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background = new SolidColorBrush(scrollBox.Color)
        };

        var canvas = new Canvas { ClipToBounds = true };

        foreach (var child in scrollBox.Children)
        {
            var childElement = Render(child);
            canvas.Children.Add(childElement);
        }

        sv.Content = canvas;
        return sv;
    }

    private UIElement RenderPageControl(TPageControl pageControl)
    {
        var tabControl = new TabControl
        {
            Background = new SolidColorBrush(pageControl.Color),
            TabStripPlacement = pageControl.TabPosition switch
            {
                TabPosition.Bottom => Dock.Bottom,
                TabPosition.Left => Dock.Left,
                TabPosition.Right => Dock.Right,
                _ => Dock.Top
            }
        };

        // Find the active page index
        int activeIndex = 0;
        int currentIndex = 0;

        // Add tab sheets
        foreach (var child in pageControl.Children.OfType<TTabSheet>())
        {
            if (!child.TabVisible)
                continue;

            var tabItem = new TabItem
            {
                Header = child.Caption,
                Tag = child
            };

            // Create content canvas for the tab
            var canvas = new Canvas
            {
                Background = new SolidColorBrush(child.Color),
                ClipToBounds = true
            };

            // Render children of the tab sheet
            foreach (var tabChild in child.Children)
            {
                var childElement = Render(tabChild);
                canvas.Children.Add(childElement);
            }

            tabItem.Content = canvas;
            tabControl.Items.Add(tabItem);

            // Check if this is the active page
            if (child.Name == pageControl.ActivePage)
            {
                activeIndex = currentIndex;
            }
            currentIndex++;
        }

        // Set the selected tab
        if (tabControl.Items.Count > 0)
        {
            tabControl.SelectedIndex = activeIndex;
        }

        return tabControl;
    }

    private UIElement RenderTabSheet(TTabSheet tabSheet)
    {
        // TabSheet rendered standalone (not inside a PageControl)
        var border = new Border
        {
            Background = new SolidColorBrush(tabSheet.Color),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1)
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // Tab header
        var header = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(10, 5, 10, 5)
        };
        header.Child = new TextBlock
        {
            Text = tabSheet.Caption,
            FontWeight = FontWeights.SemiBold
        };
        Grid.SetRow(header, 0);
        grid.Children.Add(header);

        // Content canvas
        var canvas = new Canvas
        {
            Background = Brushes.Transparent,
            ClipToBounds = true
        };

        foreach (var child in tabSheet.Children)
        {
            var childElement = Render(child);
            canvas.Children.Add(childElement);
        }

        Grid.SetRow(canvas, 1);
        grid.Children.Add(canvas);

        border.Child = grid;
        return border;
    }

    private static Thickness GetBevelThickness(PanelBevelStyle style, int width) => style == PanelBevelStyle.None ? new Thickness(0) : new Thickness(width);

    private static void SetBevelBorderBrush(Border border, PanelBevelStyle style) => border.BorderBrush = style switch
    {
        PanelBevelStyle.Raised => Brushes.White,
        PanelBevelStyle.Lowered => Brushes.DarkGray,
        _ => Brushes.Transparent
    };

    private UIElement RenderGroupBox(TGroupBox groupBox)
    {
        var gb = new GroupBox
        {
            Header = groupBox.Caption,
            Background = new SolidColorBrush(groupBox.Color),
            Padding = new Thickness(5)
        };

        var canvas = new Canvas
        {
            Background = Brushes.Transparent,
            ClipToBounds = true
        };

        foreach (var child in groupBox.Children)
        {
            var childElement = Render(child);
            canvas.Children.Add(childElement);
        }

        gb.Content = canvas;
        return gb;
    }

    private UIElement RenderRadioGroup(TRadioGroup radioGroup)
    {
        var gb = new GroupBox
        {
            Header = radioGroup.Caption,
            Background = new SolidColorBrush(radioGroup.Color),
            Padding = new Thickness(5)
        };

        var stack = new StackPanel();
        for (int i = 0; i < radioGroup.Items.Count; i++)
        {
            stack.Children.Add(new RadioButton
            {
                Content = radioGroup.Items[i],
                IsChecked = i == radioGroup.ItemIndex,
                Margin = new Thickness(2)
            });
        }

        gb.Content = stack;
        return gb;
    }

    private UIElement RenderCheckGroup(TCheckGroup checkGroup)
    {
        var gb = new GroupBox
        {
            Header = checkGroup.Caption,
            Background = new SolidColorBrush(checkGroup.Color),
            Padding = new Thickness(5)
        };

        var stack = new StackPanel();
        for (int i = 0; i < checkGroup.Items.Count; i++)
        {
            stack.Children.Add(new CheckBox
            {
                Content = checkGroup.Items[i],
                IsChecked = i < checkGroup.Checked.Count && checkGroup.Checked[i],
                Margin = new Thickness(2)
            });
        }

        gb.Content = stack;
        return gb;
    }

    private UIElement RenderCheckBox(TCheckBox checkBox) => new CheckBox
    {
        Content = checkBox.Caption,
        IsChecked = checkBox.State == CheckBoxState.Checked ? true
                : checkBox.State == CheckBoxState.Grayed ? null : false,
        IsThreeState = checkBox.AllowGrayed,
        FontFamily = new FontFamily(checkBox.FontName),
        FontSize = checkBox.FontSize,
        VerticalContentAlignment = VerticalAlignment.Center
    };

    private UIElement RenderRadioButton(TRadioButton radioBtn) => new RadioButton
    {
        Content = radioBtn.Caption,
        IsChecked = radioBtn.Checked,
        FontFamily = new FontFamily(radioBtn.FontName),
        FontSize = radioBtn.FontSize,
        VerticalContentAlignment = VerticalAlignment.Center
    };

    private UIElement RenderComboBox(TComboBox comboBox)
    {
        var cb = new ComboBox
        {
            IsEditable = comboBox.Style != ComboBoxStyle.DropDownList,
            Text = comboBox.Caption,
            SelectedIndex = comboBox.ItemIndex,
            MaxDropDownHeight = comboBox.DropDownCount * 20
        };

        foreach (var item in comboBox.Items)
        {
            cb.Items.Add(item);
        }

        return cb;
    }

    private UIElement RenderListBox(TListBox listBox)
    {
        var lb = new ListBox
        {
            SelectionMode = listBox.MultiSelect ? SelectionMode.Multiple : SelectionMode.Single,
            SelectedIndex = listBox.ItemIndex
        };

        foreach (var item in listBox.Items)
        {
            lb.Items.Add(item);
        }

        return lb;
    }

    private UIElement RenderTrackBar(TTrackBar trackBar) => new Slider
    {
        Minimum = trackBar.Min,
        Maximum = trackBar.Max,
        Value = trackBar.Position,
        Orientation = trackBar.Orientation == TrackBarOrientation.Vertical
                ? Orientation.Vertical : Orientation.Horizontal,
        TickFrequency = trackBar.Frequency,
        IsSnapToTickEnabled = true,
        TickPlacement = TickPlacement.BottomRight
    };

    private UIElement RenderProgressBar(TProgressBar progressBar) => new ProgressBar
    {
        Minimum = progressBar.Min,
        Maximum = progressBar.Max,
        Value = progressBar.Position,
        Orientation = progressBar.Orientation == ProgressBarOrientation.Vertical
                ? Orientation.Vertical : Orientation.Horizontal
    };

    private UIElement RenderPaintBox(TPaintBox paintBox) => new Border
    {
        Background = new SolidColorBrush(paintBox.Color == Colors.Transparent
                ? Color.FromRgb(240, 240, 240) : paintBox.Color),
        BorderBrush = Brushes.Gray,
        BorderThickness = new Thickness(1),
        Child = new TextBlock
        {
            Text = $"[{paintBox.TypeName}]",
            Foreground = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontStyle = FontStyles.Italic
        }
    };

    private UIElement RenderImage(TImage image)
    {
        // Check if there's actual image data
        if (image.ImageSource != null)
        {
            var img = new Image
            {
                Source = image.ImageSource,
                Stretch = GetStretchMode(image)
            };

            if (image.Center)
            {
                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.VerticalAlignment = VerticalAlignment.Center;
            }

            return new Border
            {
                Background = Brushes.Transparent,
                Child = img
            };
        }

        // Fallback placeholder
        return new Border
        {
            Background = Brushes.LightGray,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = "🖼",
                FontSize = Math.Min(image.Width, image.Height) * 0.5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }

    private static Stretch GetStretchMode(TImage image)
    {
        if (image.Proportional)
            return Stretch.Uniform;
        if (image.Stretch)
            return Stretch.Fill;
        return Stretch.None;
    }

    private UIElement RenderNonVisual(LfmComponentBase component, string icon)
    {
        var border = new Border
        {
            Background = Brushes.WhiteSmoke,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
            Opacity = 0.7
        };

        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        stack.Children.Add(new TextBlock
        {
            Text = icon,
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Center
        });

        stack.Children.Add(new TextBlock
        {
            Text = component.Name,
            FontSize = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis
        });

        border.Child = stack;
        return border;
    }

    private UIElement RenderUnknown(LfmComponentBase component) => new Border
    {
        Background = Brushes.LightYellow,
        BorderBrush = Brushes.Orange,
        BorderThickness = new Thickness(1),
        Child = new TextBlock
        {
            Text = $"[{component.TypeName}]\n{component.Name}",
            FontSize = 9,
            TextAlignment = TextAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        }
    };
}
