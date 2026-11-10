using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Globalization;
using System.Linq;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;

/// <summary>
/// Renders the supported safe AXAML preview subset without loading AXAML as executable UI.
/// </summary>
public sealed class AxamlPreviewControl : ContentControl
{
    private static readonly AxamlPreviewService s_previewService = new();

    public static readonly StyledProperty<AxamlPreviewNode?> PreviewNodeProperty =
        AvaloniaProperty.Register<AxamlPreviewControl, AxamlPreviewNode?>(nameof(PreviewNode));

    public static readonly StyledProperty<AxamlPreviewNode?> SelectedNodeProperty =
        AvaloniaProperty.Register<AxamlPreviewControl, AxamlPreviewNode?>(nameof(SelectedNode));

    public AxamlPreviewControl()
    {
        AddHandler(
            InputElement.PointerPressedEvent,
            OnPreviewPointerPressed,
            RoutingStrategies.Bubble,
            handledEventsToo: true);
        AddHandler(InputElement.PointerReleasedEvent, OnResizePointerReleased, RoutingStrategies.Bubble);
        AddHandler(InputElement.PointerCaptureLostEvent, OnResizePointerCaptureLost, RoutingStrategies.Bubble);
    }

    public event EventHandler<AxamlPreviewResizeEventArgs>? LayoutResizeRequested;

    public AxamlPreviewNode? PreviewNode
    {
        get => GetValue(PreviewNodeProperty);
        set => SetValue(PreviewNodeProperty, value);
    }

    public AxamlPreviewNode? SelectedNode
    {
        get => GetValue(SelectedNodeProperty);
        set => SetValue(SelectedNodeProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PreviewNodeProperty || change.Property == SelectedNodeProperty)
        {
            Content = PreviewNode is null
                ? null
                : BuildControl(PreviewNode);
        }
    }

    private void OnPreviewPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        Control? control = args.Source as Control;
        bool isResizeGrip = false;
        AxamlPreviewNode? node = null;
        Control? nodeContainer = null;
        while (control is not null)
        {
            if (control.Classes.Contains("axaml-resize-grip"))
            {
                isResizeGrip = true;
            }

            if (control.Tag is AxamlPreviewNode previewNode)
            {
                node = previewNode;
                nodeContainer = control;
                break;
            }

            control = control.Parent as Control;
        }

        if (node is not null)
        {
            SetCurrentValue(SelectedNodeProperty, node);
            args.Handled = true;
        }

        if (!isResizeGrip
            || node is null
            || nodeContainer is not Border { Child: Grid { Children.Count: > 0 } grid }
            || grid.Children[0] is not Control content)
        {
            return;
        }

        _resizeGesture = new ResizeGesture(
            node,
            args.GetPosition(this),
            content.Bounds.Width,
            content.Bounds.Height);
        args.Pointer.Capture(this);
    }

    private ResizeGesture? _resizeGesture;

    private void OnResizePointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (_resizeGesture is not { } gesture)
        {
            return;
        }

        Point currentPosition = args.GetPosition(this);
        _resizeGesture = null;
        args.Pointer.Capture(null);
        if (gesture.Width <= 0 || gesture.Height <= 0)
        {
            return;
        }

        double width = Math.Clamp(gesture.Width + currentPosition.X - gesture.StartPosition.X, 16, 4096);
        double height = Math.Clamp(gesture.Height + currentPosition.Y - gesture.StartPosition.Y, 16, 4096);
        LayoutResizeRequested?.Invoke(this, new AxamlPreviewResizeEventArgs(gesture.Node, width, height));
    }

    private void OnResizePointerCaptureLost(object? sender, PointerCaptureLostEventArgs args)
    {
        _resizeGesture = null;
    }

    private Control BuildControl(AxamlPreviewNode node)
    {
        if (node.IsRecoveryRoot)
        {
            StackPanel recoveredContent = new() { Spacing = 8 };
            recoveredContent.Children.Add(new Border
            {
                Padding = new Thickness(8),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.DarkOrange,
                Child = new TextBlock
                {
                    Text = node.Diagnostic,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                },
            });
            foreach (AxamlPreviewNode child in node.Children)
            {
                recoveredContent.Children.Add(BuildControl(child));
            }

            return recoveredContent;
        }

        if (!node.IsSupported)
        {
            return WrapNode(node, new Border
            {
                Padding = new Thickness(8),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.DarkOrange,
                Child = new TextBlock { Text = node.Diagnostic ?? $"Unsupported control: {node.TypeName}" },
            });
        }

        Control content = node.TypeName switch
        {
            "Border" => BuildBorder(node),
            "Button" => BuildButton(node),
            "CheckBox" => BuildCheckBox(node),
            "ComboBox" => ApplyTemplatedControlAppearance(new ComboBox
            {
                PlaceholderText = ParseLiteralText(node, "PlaceholderText", node.Text ?? "ComboBox"),
                IsEnabled = false,
            }, node),
            "ContentControl" => BuildContentControl(node),
            "DockPanel" => BuildDockPanel(node),
            "Expander" => ApplyTemplatedControlAppearance(new Expander
            {
                Header = node.Text ?? "Expander",
                IsExpanded = ParseBoolean(node, "IsExpanded") ?? true,
                Content = BuildChildren(node),
            }, node),
            "Grid" => BuildGrid(node),
            "GroupBox" => ApplyTemplatedControlAppearance(new GroupBox
            {
                Header = node.Text ?? "Group",
                Content = BuildChildren(node),
            }, node),
            "Label" => ApplyTemplatedControlAppearance(new Label
            {
                Content = BuildTextBlock(node),
            }, node),
            "TextBlock" => BuildTextBlock(node),
            "ListBox" => new ListBox
            {
                ItemsSource = node.Children.Select(static child => child.Text ?? child.TypeName).ToArray(),
                IsEnabled = false,
            },
            "NumericUpDown" => BuildNumericUpDown(node),
            "ProgressBar" => BuildProgressBar(node),
            "RadioButton" => BuildRadioButton(node),
            "ScrollViewer" => BuildScrollViewer(node),
            "Separator" => new Separator(),
            "Slider" => ApplyTemplatedControlAppearance(new Slider
            {
                Minimum = ParseDouble(node, "Minimum", 0),
                Maximum = ParseDouble(node, "Maximum", 100),
                Value = ParseDouble(node, "Value", 50),
                TickFrequency = ParseOptionalDouble(node, "TickFrequency") is double tickFrequency && tickFrequency >= 0
                    ? tickFrequency
                    : 0,
                IsSnapToTickEnabled = ParseBoolean(node, "IsSnapToTickEnabled") ?? false,
                IsDirectionReversed = ParseBoolean(node, "IsDirectionReversed") ?? false,
                TickPlacement = Enum.TryParse(GetProperty(node, "TickPlacement"), ignoreCase: true, out TickPlacement tickPlacement)
                    && Enum.IsDefined(tickPlacement)
                        ? tickPlacement
                        : TickPlacement.None,
                Orientation = Enum.TryParse(GetProperty(node, "Orientation"), ignoreCase: true, out Orientation sliderOrientation)
                    ? sliderOrientation
                    : Orientation.Horizontal,
                IsEnabled = false,
            }, node),
            "StackPanel" => BuildStackPanel(node),
            "TextBox" => BuildTextBox(node),
            "ToggleSwitch" => BuildToggleSwitch(node),
            "UniformGrid" => BuildUniformGrid(node),
            "Viewbox" => new Viewbox
            {
                Child = node.Children.FirstOrDefault() is { } child ? BuildControl(child) : null,
                Stretch = Enum.TryParse(GetProperty(node, "Stretch"), ignoreCase: true, out Stretch stretch)
                    ? stretch
                    : Stretch.Uniform,
                StretchDirection = AxamlPropertyValueValidator.Validate(
                    node.TypeName,
                    "StretchDirection",
                    GetProperty(node, "StretchDirection") ?? string.Empty) is null
                    && Enum.TryParse(GetProperty(node, "StretchDirection"), ignoreCase: true, out StretchDirection stretchDirection)
                        ? stretchDirection
                        : StretchDirection.Both,
            },
            "WrapPanel" => BuildWrapPanel(node),
            "Window" or "UserControl" => BuildChildren(node),
            _ => new TextBlock { Text = node.TypeName },
        };
        return WrapNode(node, content);
    }

    private Border WrapNode(AxamlPreviewNode node, Control content)
    {
        bool isSelected = ReferenceEquals(node, SelectedNode);
        content.Margin = ParseThickness(node, "Margin");
        content.HorizontalAlignment = ParseAlignment(node, "HorizontalAlignment");
        content.VerticalAlignment = ParseVerticalAlignment(node);
        if (node.IsSupported)
        {
            if (HasValidLiteralValue(node, "FlowDirection")
                && Enum.TryParse(GetProperty(node, "FlowDirection"), ignoreCase: true, out Avalonia.Media.FlowDirection flowDirection)
                && Enum.IsDefined(flowDirection))
            {
                content.FlowDirection = flowDirection;
            }

            if (HasValidLiteralValue(node, "UseLayoutRounding")
                && ParseBoolean(node, "UseLayoutRounding") is bool useLayoutRounding)
            {
                content.UseLayoutRounding = useLayoutRounding;
            }

            content.Opacity = ParseDouble(node, "Opacity", 1);
            content.ClipToBounds = ParseBoolean(node, "ClipToBounds") ?? false;
            content.IsVisible = ParseBoolean(node, "IsVisible") ?? true;
        }

        if (ParseOptionalDouble(node, "Width") is double width)
        {
            content.Width = width;
        }

        if (ParseOptionalDouble(node, "Height") is double height)
        {
            content.Height = height;
        }

        if (ParseOptionalSizeConstraint(node, "MinWidth") is double minWidth)
        {
            content.MinWidth = minWidth;
        }

        if (ParseOptionalSizeConstraint(node, "MaxWidth") is double maxWidth)
        {
            content.MaxWidth = maxWidth;
        }

        if (ParseOptionalSizeConstraint(node, "MinHeight") is double minHeight)
        {
            content.MinHeight = minHeight;
        }

        if (ParseOptionalSizeConstraint(node, "MaxHeight") is double maxHeight)
        {
            content.MaxHeight = maxHeight;
        }

        content.IsHitTestVisible = node.Children.Count > 0;
        Grid wrapper = new();
        wrapper.Children.Add(content);
        if (isSelected && CanResize(node))
        {
            wrapper.Children.Add(new Border
            {
                Width = 12,
                Height = 12,
                Margin = new Thickness(2),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
                Background = Brushes.DodgerBlue,
                Classes = { "axaml-resize-grip" },
            });
        }

        return new Border
        {
            Tag = node.SourceStartOffset >= 0 ? node : null,
            Child = wrapper,
            Background = Brushes.Transparent,
            BorderBrush = isSelected ? Brushes.DodgerBlue : Brushes.Transparent,
            BorderThickness = new Thickness(isSelected ? 1 : 0),
        };
    }

    private static bool CanResize(AxamlPreviewNode node)
    {
        return s_previewService.CanEditProperty(node, "Width")
            && s_previewService.CanEditProperty(node, "Height");
    }

    private Control BuildBorder(AxamlPreviewNode node)
    {
        Border border = new()
        {
            Padding = ParseThickness(node, "Padding"),
            BorderThickness = ParseThickness(node, "BorderThickness"),
            CornerRadius = ParseCornerRadius(node),
            Child = node.Children.FirstOrDefault() is { } child ? BuildControl(child) : null,
        };
        if (ParseSimpleBrush(node, "BorderBrush") is { } borderBrush)
        {
            border.BorderBrush = borderBrush;
        }

        string? background = GetProperty(node, "Background");
        if (!string.IsNullOrWhiteSpace(background) && !background.StartsWith('{'))
        {
            try
            {
                border.Background = Brush.Parse(background);
            }
            catch (FormatException)
            {
            }
        }

        return border;
    }

    private static Button BuildButton(AxamlPreviewNode node)
    {
        Button button = new()
        {
            Content = node.Text ?? "Button",
            IsEnabled = false,
            IsDefault = false,
            IsCancel = false,
            HorizontalAlignment = ParseAlignment(node, "HorizontalAlignment"),
            VerticalAlignment = ParseVerticalAlignment(node),
        };
        return ApplyTemplatedControlAppearance(button, node);
    }

    private static Control BuildNumericUpDown(AxamlPreviewNode node)
    {
        NumericUpDown numericUpDown = new()
        {
            AllowSpin = ParseBoolean(node, "AllowSpin") ?? true,
            ButtonSpinnerLocation = Enum.TryParse(GetProperty(node, "ButtonSpinnerLocation"), ignoreCase: true, out Location spinnerLocation)
                && Enum.IsDefined(spinnerLocation)
                    ? spinnerLocation
                    : Location.Right,
            ClipValueToMinMax = ParseBoolean(node, "ClipValueToMinMax") ?? false,
            Increment = ParseOptionalDecimal(node, "Increment") ?? 1m,
            Minimum = ParseOptionalDecimal(node, "Minimum") ?? decimal.MinValue,
            Maximum = ParseOptionalDecimal(node, "Maximum") ?? decimal.MaxValue,
            ParsingNumberStyle = ParseNumericUpDownNumberStyle(node),
            Value = ParseOptionalDecimal(node, "Value"),
            ShowButtonSpinner = ParseBoolean(node, "ShowButtonSpinner") ?? true,
            TextAlignment = ParseTextAlignment(node, TextAlignment.Start),
            IsEnabled = false,
        };
        if (ParseValidatedFormat(node, "FormatString") is { } formatString)
        {
            numericUpDown.FormatString = formatString;
        }

        return ApplyTemplatedControlAppearance(numericUpDown, node);
    }

    private static NumberStyles ParseNumericUpDownNumberStyle(AxamlPreviewNode node)
    {
        string value = GetProperty(node, "ParsingNumberStyle") ?? "Any";
        return AxamlPropertyValueValidator.Validate("NumericUpDown", "ParsingNumberStyle", value) is null
            && Enum.TryParse(value, ignoreCase: true, out NumberStyles numberStyle)
                ? numberStyle
                : NumberStyles.Any;
    }

    private static Control BuildProgressBar(AxamlPreviewNode node)
    {
        ProgressBar progressBar = new()
        {
            Minimum = ParseDouble(node, "Minimum", 0),
            Maximum = ParseDouble(node, "Maximum", 100),
            Value = ParseDouble(node, "Value", 50),
            IsIndeterminate = ParseBoolean(node, "IsIndeterminate") ?? false,
            ShowProgressText = ParseBoolean(node, "ShowProgressText") ?? false,
            Orientation = Enum.TryParse(GetProperty(node, "Orientation"), ignoreCase: true, out Orientation progressOrientation)
                ? progressOrientation
                : Orientation.Horizontal,
            IsEnabled = false,
        };
        if (ParseValidatedFormat(node, "ProgressTextFormat") is { } format)
        {
            progressBar.ProgressTextFormat = format;
        }

        return ApplyTemplatedControlAppearance(progressBar, node);
    }

    private static CheckBox BuildCheckBox(AxamlPreviewNode node)
    {
        return ApplyTemplatedControlAppearance(new CheckBox
        {
            Content = node.Text ?? "CheckBox",
            IsThreeState = ParseBoolean(node, "IsThreeState") ?? false,
            IsEnabled = false,
            IsChecked = ParseBoolean(node, "IsChecked"),
        }, node);
    }

    private static RadioButton BuildRadioButton(AxamlPreviewNode node)
    {
        return ApplyTemplatedControlAppearance(new RadioButton
        {
            Content = node.Text ?? "RadioButton",
            GroupName = ParseLiteralText(node, "GroupName", string.Empty),
            IsEnabled = false,
            IsChecked = ParseBoolean(node, "IsChecked"),
        }, node);
    }

    private static T ApplyTemplatedControlAppearance<T>(T control, AxamlPreviewNode node)
        where T : TemplatedControl
    {
        HorizontalAlignment? horizontalContentAlignment = HasValidLiteralValue(node, "HorizontalContentAlignment")
            ? ParseAlignment(node, "HorizontalContentAlignment")
            : null;
        VerticalAlignment? verticalContentAlignment = HasValidLiteralValue(node, "VerticalContentAlignment")
            ? ParseVerticalAlignment(node, "VerticalContentAlignment")
            : null;
        if (control is ContentControl contentControl)
        {
            if (horizontalContentAlignment is { } horizontal)
            {
                contentControl.HorizontalContentAlignment = horizontal;
            }

            if (verticalContentAlignment is { } vertical)
            {
                contentControl.VerticalContentAlignment = vertical;
            }
        }
        else if (control is ComboBox comboBox)
        {
            if (horizontalContentAlignment is { } horizontal)
            {
                comboBox.HorizontalContentAlignment = horizontal;
            }

            if (verticalContentAlignment is { } vertical)
            {
                comboBox.VerticalContentAlignment = vertical;
            }
        }
        else if (control is NumericUpDown numericUpDown)
        {
            if (horizontalContentAlignment is { } horizontal)
            {
                numericUpDown.HorizontalContentAlignment = horizontal;
            }

            if (verticalContentAlignment is { } vertical)
            {
                numericUpDown.VerticalContentAlignment = vertical;
            }
        }
        else if (control is TextBox textBox)
        {
            if (horizontalContentAlignment is { } horizontal)
            {
                textBox.HorizontalContentAlignment = horizontal;
            }

            if (verticalContentAlignment is { } vertical)
            {
                textBox.VerticalContentAlignment = vertical;
            }
        }

        if (ParseSimpleBrush(node, "Background") is { } background)
        {
            control.Background = background;
        }

        if (ParseSimpleBrush(node, "BorderBrush") is { } borderBrush)
        {
            control.BorderBrush = borderBrush;
        }

        if (HasValidLiteralValue(node, "BorderThickness"))
        {
            control.BorderThickness = ParseThickness(node, "BorderThickness");
        }

        if (HasValidLiteralValue(node, "CornerRadius"))
        {
            control.CornerRadius = ParseCornerRadius(node);
        }

        if (HasValidLiteralValue(node, "FontSize"))
        {
            control.FontSize = ParseDouble(node, "FontSize", 14);
        }

        if (HasValidLiteralValue(node, "FontStyle"))
        {
            control.FontStyle = ParseFontStyle(node);
        }

        if (HasValidLiteralValue(node, "FontWeight"))
        {
            control.FontWeight = ParseFontWeight(node);
        }

        if (ParseSimpleBrush(node, "Foreground") is { } foreground)
        {
            control.Foreground = foreground;
        }

        if (HasValidLiteralValue(node, "Padding"))
        {
            control.Padding = ParseThickness(node, "Padding");
        }

        return control;
    }

    private static bool HasValidLiteralValue(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        return !string.IsNullOrWhiteSpace(value)
            && AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is null;
    }

    private static ToggleSwitch BuildToggleSwitch(AxamlPreviewNode node)
    {
        ToggleSwitch toggleSwitch = new()
        {
            Content = node.Text ?? "ToggleSwitch",
            IsThreeState = ParseBoolean(node, "IsThreeState") ?? false,
            IsEnabled = false,
            IsChecked = ParseBoolean(node, "IsChecked"),
        };
        if (node.Properties.TryGetValue("OffContent", out string? offContent))
        {
            toggleSwitch.OffContent = offContent;
        }

        if (node.Properties.TryGetValue("OnContent", out string? onContent))
        {
            toggleSwitch.OnContent = onContent;
        }

        return ApplyTemplatedControlAppearance(toggleSwitch, node);
    }

    private Control BuildContentControl(AxamlPreviewNode node)
    {
        return ApplyTemplatedControlAppearance(new ContentControl
        {
            Content = BuildChildren(node),
        }, node);
    }

    private Control BuildDockPanel(AxamlPreviewNode node)
    {
        DockPanel panel = new()
        {
            HorizontalSpacing = ParseOptionalSizeConstraint(node, "HorizontalSpacing") ?? 0,
            LastChildFill = ParseBoolean(node, "LastChildFill") ?? true,
            VerticalSpacing = ParseOptionalSizeConstraint(node, "VerticalSpacing") ?? 0,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            Control control = BuildControl(child);
            if (Enum.TryParse(GetProperty(child, "DockPanel.Dock"), ignoreCase: true, out Dock dock))
            {
                DockPanel.SetDock(control, dock);
            }

            panel.Children.Add(control);
        }

        return panel;
    }

    private Control BuildGrid(AxamlPreviewNode node)
    {
        Grid grid = new()
        {
            ColumnSpacing = ParseOptionalSizeConstraint(node, "ColumnSpacing") ?? 0,
            RowSpacing = ParseOptionalSizeConstraint(node, "RowSpacing") ?? 0,
            ShowGridLines = ParseBoolean(node, "ShowGridLines") ?? false,
        };
        foreach (string row in node.RowDefinitions)
        {
            grid.RowDefinitions.Add(new RowDefinition(ParseGridLength(row)));
        }

        foreach (string column in node.ColumnDefinitions)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(ParseGridLength(column)));
        }

        if (grid.RowDefinitions.Count == 0)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }

        if (grid.ColumnDefinitions.Count == 0)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        foreach (AxamlPreviewNode child in node.Children)
        {
            Control control = BuildControl(child);
            if (TryGetInt(child, "Grid.Row", out int row) && row >= 0 && row < grid.RowDefinitions.Count)
            {
                Grid.SetRow(control, row);
            }

            if (TryGetInt(child, "Grid.Column", out int column) && column >= 0 && column < grid.ColumnDefinitions.Count)
            {
                Grid.SetColumn(control, column);
            }

            if (TryGetInt(child, "Grid.RowSpan", out int rowSpan) && rowSpan > 0 && rowSpan <= grid.RowDefinitions.Count)
            {
                Grid.SetRowSpan(control, rowSpan);
            }

            if (TryGetInt(child, "Grid.ColumnSpan", out int columnSpan) && columnSpan > 0 && columnSpan <= grid.ColumnDefinitions.Count)
            {
                Grid.SetColumnSpan(control, columnSpan);
            }

            grid.Children.Add(control);
        }

        return grid;
    }

    private Control BuildTextBlock(AxamlPreviewNode node)
    {
        TextBlock textBlock = new()
        {
            Text = node.Text ?? string.Empty,
            FontSize = HasValidLiteralValue(node, "FontSize") ? ParseDouble(node, "FontSize", 14) : 14,
            FontStyle = HasValidLiteralValue(node, "FontStyle") ? ParseFontStyle(node) : FontStyle.Normal,
            FontWeight = HasValidLiteralValue(node, "FontWeight") ? ParseFontWeight(node) : FontWeight.Normal,
            LineHeight = HasValidLiteralValue(node, "LineHeight") ? ParseLineHeight(node) : double.NaN,
            MaxLines = HasValidLiteralValue(node, "MaxLines") ? ParseInt(node, "MaxLines") : 0,
            TextTrimming = HasValidLiteralValue(node, "TextTrimming") ? ParseTextTrimming(node) : TextTrimming.None,
            TextWrapping = HasValidLiteralValue(node, "TextWrapping")
                ? ParseTextWrapping(node, TextWrapping.Wrap)
                : TextWrapping.Wrap,
            TextAlignment = HasValidLiteralValue(node, "TextAlignment")
                ? ParseTextAlignment(node, TextAlignment.Left)
                : TextAlignment.Left,
            HorizontalAlignment = ParseAlignment(node, "HorizontalAlignment"),
            VerticalAlignment = ParseVerticalAlignment(node),
        };
        if (ParseSimpleBrush(node, "Foreground") is { } foreground)
        {
            textBlock.Foreground = foreground;
        }

        return textBlock;
    }

    private static TextBox BuildTextBox(AxamlPreviewNode node)
    {
        TextBox textBox = new()
        {
            Text = node.Text,
            PlaceholderText = GetProperty(node, "Watermark") ?? GetProperty(node, "PlaceholderText"),
            AcceptsReturn = ParseBoolean(node, "AcceptsReturn") ?? false,
            // Keep Tab available for host focus navigation in the read-only preview.
            AcceptsTab = false,
            MaxLength = ParseInt(node, "MaxLength"),
            FontSize = ParseDouble(node, "FontSize", 14),
            FontStyle = ParseFontStyle(node),
            FontWeight = ParseFontWeight(node),
            HorizontalAlignment = ParseAlignment(node, "HorizontalAlignment"),
            TextAlignment = ParseTextAlignment(node, TextAlignment.Left),
            TextWrapping = ParseTextWrapping(node, TextWrapping.NoWrap),
            UseFloatingPlaceholder = ParseBoolean(node, "UseFloatingPlaceholder") ?? false,
            VerticalAlignment = ParseVerticalAlignment(node),
            IsReadOnly = true,
        };
        if (HasValidLiteralValue(node, "MaxLines") && TryGetInt(node, "MaxLines", out int maxLines))
        {
            textBox.MaxLines = maxLines;
        }

        if (HasValidLiteralValue(node, "MinLines") && TryGetInt(node, "MinLines", out int minLines))
        {
            textBox.MinLines = minLines;
        }

        if (HasValidLiteralValue(node, "LineHeight"))
        {
            textBox.LineHeight = ParseDouble(node, "LineHeight", 0);
        }

        if (ParseSimpleBrush(node, "PlaceholderForeground") is { } placeholderForeground)
        {
            textBox.PlaceholderForeground = placeholderForeground;
        }

        return ApplyTemplatedControlAppearance(textBox, node);
    }

    private Control BuildScrollViewer(AxamlPreviewNode node)
    {
        return new ScrollViewer
        {
            Content = BuildChildren(node),
            HorizontalScrollBarVisibility = ParseScrollBarVisibility(node, "HorizontalScrollBarVisibility"),
            VerticalScrollBarVisibility = ParseScrollBarVisibility(node, "VerticalScrollBarVisibility"),
        };
    }

    private static ScrollBarVisibility ParseScrollBarVisibility(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        return value is not null
            && AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is null
            && Enum.TryParse(value, ignoreCase: true, out ScrollBarVisibility visibility)
                ? visibility
                : ScrollBarVisibility.Auto;
    }

    private Control BuildStackPanel(AxamlPreviewNode node)
    {
        StackPanel panel = new()
        {
            Orientation = Enum.TryParse(GetProperty(node, "Orientation"), ignoreCase: true, out Orientation orientation)
                ? orientation
                : Orientation.Vertical,
            Spacing = ParseOptionalSizeConstraint(node, "Spacing") ?? 0,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            panel.Children.Add(BuildControl(child));
        }

        return panel;
    }

    private Control BuildWrapPanel(AxamlPreviewNode node)
    {
        WrapPanel panel = new()
        {
            Orientation = Enum.TryParse(GetProperty(node, "Orientation"), ignoreCase: true, out Orientation orientation)
                ? orientation
                : Orientation.Horizontal,
            ItemHeight = ParseOptionalSizeConstraint(node, "ItemHeight") ?? double.NaN,
            ItemSpacing = ParseOptionalSizeConstraint(node, "ItemSpacing") ?? 0,
            ItemWidth = ParseOptionalSizeConstraint(node, "ItemWidth") ?? double.NaN,
            ItemsAlignment = Enum.TryParse(GetProperty(node, "ItemsAlignment"), ignoreCase: true, out WrapPanelItemsAlignment itemsAlignment)
                && Enum.IsDefined(itemsAlignment)
                    ? itemsAlignment
                    : WrapPanelItemsAlignment.Start,
            LineSpacing = ParseOptionalSizeConstraint(node, "LineSpacing") ?? 0,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            panel.Children.Add(BuildControl(child));
        }

        return panel;
    }

    private Control BuildUniformGrid(AxamlPreviewNode node)
    {
        UniformGrid grid = new()
        {
            ColumnSpacing = ParseOptionalSizeConstraint(node, "ColumnSpacing") ?? 0,
            Rows = ParseInt(node, "Rows"),
            Columns = ParseInt(node, "Columns"),
            FirstColumn = ParseInt(node, "FirstColumn"),
            RowSpacing = ParseOptionalSizeConstraint(node, "RowSpacing") ?? 0,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            grid.Children.Add(BuildControl(child));
        }

        return grid;
    }

    private Control BuildChildren(AxamlPreviewNode node)
    {
        if (node.Children.Count == 1)
        {
            return BuildControl(node.Children[0]);
        }

        StackPanel panel = new();
        foreach (AxamlPreviewNode child in node.Children)
        {
            panel.Children.Add(BuildControl(child));
        }

        return panel;
    }

    private static GridLength ParseGridLength(string value)
    {
        if (string.Equals(value, "Auto", StringComparison.OrdinalIgnoreCase))
        {
            return GridLength.Auto;
        }

        if (value.EndsWith('*'))
        {
            string weight = value[..^1];
            return double.TryParse(weight, CultureInfo.InvariantCulture, out double starValue)
                && double.IsFinite(starValue) && starValue >= 0
                ? new GridLength(starValue, GridUnitType.Star)
                : GridLength.Star;
        }

        return double.TryParse(value, CultureInfo.InvariantCulture, out double pixelValue)
            && double.IsFinite(pixelValue) && pixelValue >= 0
            ? new GridLength(pixelValue)
            : GridLength.Auto;
    }

    private static Thickness ParseThickness(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        if (string.IsNullOrWhiteSpace(value) || value.StartsWith('{'))
        {
            return new Thickness(0);
        }

        double[] values = value.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries)
            .Select(part => double.TryParse(part, CultureInfo.InvariantCulture, out double number) ? number : double.NaN)
            .ToArray();
        if (values.Length == 1 && double.IsFinite(values[0]))
        {
            return new Thickness(values[0]);
        }

        if (values.Length == 2 && values.All(double.IsFinite))
        {
            return new Thickness(values[0], values[1]);
        }

        if (values.Length == 4 && values.All(double.IsFinite))
        {
            return new Thickness(values[0], values[1], values[2], values[3]);
        }

        return new Thickness(0);
    }

    private static CornerRadius ParseCornerRadius(AxamlPreviewNode node)
    {
        string? value = GetProperty(node, "CornerRadius");
        if (string.IsNullOrWhiteSpace(value)
            || AxamlPropertyValueValidator.Validate(node.TypeName, "CornerRadius", value) is not null)
        {
            return new CornerRadius(0);
        }

        double[] radii = value.Split(',', StringSplitOptions.TrimEntries)
            .Select(part => double.Parse(part, CultureInfo.InvariantCulture))
            .ToArray();
        return radii.Length == 1
            ? new CornerRadius(radii[0])
            : new CornerRadius(radii[0], radii[1], radii[2], radii[3]);
    }

    private static HorizontalAlignment ParseAlignment(AxamlPreviewNode node, string propertyName)
    {
        return Enum.TryParse(GetProperty(node, propertyName), ignoreCase: true, out HorizontalAlignment alignment)
            && Enum.IsDefined(alignment)
            ? alignment
            : HorizontalAlignment.Stretch;
    }

    private static VerticalAlignment ParseVerticalAlignment(AxamlPreviewNode node, string propertyName = "VerticalAlignment")
    {
        return Enum.TryParse(GetProperty(node, propertyName), ignoreCase: true, out VerticalAlignment alignment)
            && Enum.IsDefined(alignment)
            ? alignment
            : VerticalAlignment.Stretch;
    }

    private static FontWeight ParseFontWeight(AxamlPreviewNode node)
    {
        return GetProperty(node, "FontWeight")?.ToUpperInvariant() switch
        {
            "THIN" => FontWeight.Thin,
            "LIGHT" => FontWeight.Light,
            "NORMAL" => FontWeight.Normal,
            "MEDIUM" => FontWeight.Medium,
            "SEMIBOLD" => FontWeight.SemiBold,
            "BOLD" => FontWeight.Bold,
            "EXTRABOLD" => FontWeight.ExtraBold,
            "BLACK" => FontWeight.Black,
            _ => FontWeight.Normal,
        };
    }

    private static FontStyle ParseFontStyle(AxamlPreviewNode node)
    {
        return Enum.TryParse(GetProperty(node, "FontStyle"), ignoreCase: true, out FontStyle style)
            ? style
            : FontStyle.Normal;
    }

    private static TextAlignment ParseTextAlignment(AxamlPreviewNode node, TextAlignment fallback)
    {
        return Enum.TryParse(GetProperty(node, "TextAlignment"), ignoreCase: true, out TextAlignment alignment)
            && Enum.IsDefined(alignment)
            ? alignment
            : fallback;
    }

    private static TextWrapping ParseTextWrapping(AxamlPreviewNode node, TextWrapping defaultValue)
    {
        return Enum.TryParse(GetProperty(node, "TextWrapping"), ignoreCase: true, out TextWrapping wrapping)
            ? wrapping
            : defaultValue;
    }

    private static TextTrimming ParseTextTrimming(AxamlPreviewNode node)
    {
        return GetProperty(node, "TextTrimming")?.ToUpperInvariant() switch
        {
            "CHARACTERELLIPSIS" => TextTrimming.CharacterEllipsis,
            "WORDELLIPSIS" => TextTrimming.WordEllipsis,
            _ => TextTrimming.None,
        };
    }

    private static IBrush? ParseSimpleBrush(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        if (string.IsNullOrWhiteSpace(value)
            || AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is not null)
        {
            return null;
        }

        try
        {
            return Brush.Parse(value);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static double? ParseOptionalDouble(AxamlPreviewNode node, string propertyName)
    {
        return double.TryParse(GetProperty(node, propertyName), CultureInfo.InvariantCulture, out double value)
            && double.IsFinite(value)
                ? value
                : null;
    }

    private static double? ParseOptionalSizeConstraint(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        return value is not null
            && s_previewService.CanEditProperty(node, propertyName)
            && AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is null
                ? ParseOptionalDouble(node, propertyName)
                : null;
    }

    private static decimal? ParseOptionalDecimal(AxamlPreviewNode node, string propertyName)
    {
        return decimal.TryParse(
            GetProperty(node, propertyName),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out decimal value)
                ? value
                : null;
    }

    private static double ParseDouble(AxamlPreviewNode node, string propertyName, double fallback)
    {
        return double.TryParse(GetProperty(node, propertyName), CultureInfo.InvariantCulture, out double value)
            && double.IsFinite(value)
            ? value
            : fallback;
    }

    private static double ParseLineHeight(AxamlPreviewNode node)
    {
        double? value = ParseOptionalDouble(node, "LineHeight");
        return value is > 0 ? value.Value : double.NaN;
    }

    private static string ParseLiteralText(AxamlPreviewNode node, string propertyName, string fallback)
    {
        string? value = GetProperty(node, propertyName);
        return value is null || value.TrimStart().StartsWith('{') ? fallback : value;
    }

    private static string? ParseValidatedFormat(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        if (value is null)
        {
            return null;
        }

        string trimmed = value.TrimStart();
        if (trimmed.StartsWith('{'))
        {
            if (!trimmed.StartsWith("{}", StringComparison.Ordinal))
            {
                return null;
            }

            value = value.Remove(value.Length - trimmed.Length, 2);
        }

        return AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is null
            ? value
            : null;
    }

    private static bool? ParseBoolean(AxamlPreviewNode node, string propertyName)
    {
        return bool.TryParse(GetProperty(node, propertyName), out bool value) ? value : null;
    }

    private static bool TryGetInt(AxamlPreviewNode node, string propertyName, out int value)
    {
        return int.TryParse(GetProperty(node, propertyName), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    private static int ParseInt(AxamlPreviewNode node, string propertyName)
    {
        return int.TryParse(GetProperty(node, propertyName), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
            && value >= 0
                ? value
                : 0;
    }

    private static string? GetProperty(AxamlPreviewNode node, string name)
    {
        return node.Properties.TryGetValue(name, out string? value) ? value : null;
    }

    private sealed record ResizeGesture(AxamlPreviewNode Node, Point StartPosition, double Width, double Height);
}

public sealed class AxamlPreviewResizeEventArgs(
    AxamlPreviewNode node,
    double width,
    double height) : EventArgs
{
    public AxamlPreviewNode Node { get; } = node;

    public double Width { get; } = width;

    public double Height { get; } = height;
}
