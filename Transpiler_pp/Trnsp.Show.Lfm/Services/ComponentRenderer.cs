using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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
        // Order matters: more specific types first (TMemo before TEdit)
        var element = component switch
        {
            TForm form => RenderForm(form),
            TLabel label => RenderLabel(label),
            TMemo memo => RenderMemo(memo),  // TMemo before TEdit (inheritance)
            TEdit edit => RenderEdit(edit),
            TBitBtn bitBtn => RenderBitBtn(bitBtn),  // TBitBtn before TButton
            TButton button => RenderButton(button),
            TSpeedButton speedBtn => RenderSpeedButton(speedBtn),
            TScrollBox scrollBox => RenderScrollBox(scrollBox),  // TScrollBox before TPanel
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
            TPaintBox paintBox => RenderPaintBox(paintBox),  // TPaintBox before TImage
            TImage image => RenderImage(image),
            TTimer timer => RenderNonVisual(timer, "?"),
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
            fe.ToolTip = string.IsNullOrEmpty(component.Hint) ? null : component.Hint;
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
            CornerRadius = new CornerRadius(3)
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(28) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

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
        Grid.SetRow(titleBar, 0);
        grid.Children.Add(titleBar);

        // Client area
        var clientCanvas = new Canvas
        {
            Background = new SolidColorBrush(form.Color),
            ClipToBounds = true
        };
        Grid.SetRow(clientCanvas, 1);
        grid.Children.Add(clientCanvas);

        // Render children
        foreach (var child in form.Children)
        {
            var childElement = Render(child);
            clientCanvas.Children.Add(childElement);
        }

        border.Child = grid;
        return border;
    }

    private UIElement RenderLabel(TLabel label)
    {
        return new TextBlock
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

    private UIElement RenderMemo(TMemo memo)
    {
        return new TextBox
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
    }

    private UIElement RenderButton(TButton button)
    {
        var btn = new Button
        {
            Content = button.Caption,
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

        var icon = new TextBlock
        {
            Text = GetBitBtnIcon(bitBtn.Kind),
            Margin = new Thickness(0, 0, bitBtn.Spacing, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var text = new TextBlock
        {
            Text = bitBtn.Caption,
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
        BitBtnKind.OK => "?",
        BitBtnKind.Cancel => "?",
        BitBtnKind.Help => "?",
        BitBtnKind.Yes => "?",
        BitBtnKind.No => "?",
        BitBtnKind.Close => "?",
        BitBtnKind.Abort => "?",
        BitBtnKind.Retry => "??",
        BitBtnKind.Ignore => "?",
        BitBtnKind.All => "?",
        _ => ""
    };

    private UIElement RenderSpeedButton(TSpeedButton speedBtn)
    {
        var btn = new ToggleButton
        {
            Content = speedBtn.Caption,
            IsChecked = speedBtn.Down,
            Padding = new Thickness(2)
        };

        if (speedBtn.Flat)
        {
            btn.BorderThickness = new Thickness(0);
        }

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

        // Add caption if present
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

        // Render children
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

    private static Thickness GetBevelThickness(PanelBevelStyle style, int width)
    {
        return style == PanelBevelStyle.None ? new Thickness(0) : new Thickness(width);
    }

    private static void SetBevelBorderBrush(Border border, PanelBevelStyle style)
    {
        border.BorderBrush = style switch
        {
            PanelBevelStyle.Raised => Brushes.White,
            PanelBevelStyle.Lowered => Brushes.DarkGray,
            _ => Brushes.Transparent
        };
    }

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

    private UIElement RenderCheckBox(TCheckBox checkBox)
    {
        return new CheckBox
        {
            Content = checkBox.Caption,
            IsChecked = checkBox.State == CheckBoxState.Checked ? true 
                : checkBox.State == CheckBoxState.Grayed ? null : false,
            IsThreeState = checkBox.AllowGrayed,
            FontFamily = new FontFamily(checkBox.FontName),
            FontSize = checkBox.FontSize,
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }

    private UIElement RenderRadioButton(TRadioButton radioBtn)
    {
        return new RadioButton
        {
            Content = radioBtn.Caption,
            IsChecked = radioBtn.Checked,
            FontFamily = new FontFamily(radioBtn.FontName),
            FontSize = radioBtn.FontSize,
            VerticalContentAlignment = VerticalAlignment.Center
        };
    }

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

    private UIElement RenderTrackBar(TTrackBar trackBar)
    {
        return new Slider
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
    }

    private UIElement RenderProgressBar(TProgressBar progressBar)
    {
        return new ProgressBar
        {
            Minimum = progressBar.Min,
            Maximum = progressBar.Max,
            Value = progressBar.Position,
            Orientation = progressBar.Orientation == ProgressBarOrientation.Vertical 
                ? Orientation.Vertical : Orientation.Horizontal
        };
    }

    private UIElement RenderPaintBox(TPaintBox paintBox)
    {
        return new Border
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
    }

    private UIElement RenderImage(TImage image)
    {
        return new Border
        {
            Background = Brushes.LightGray,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = "??",
                FontSize = Math.Min(image.Width, image.Height) * 0.5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
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

    private UIElement RenderUnknown(LfmComponentBase component)
    {
        return new Border
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
}
