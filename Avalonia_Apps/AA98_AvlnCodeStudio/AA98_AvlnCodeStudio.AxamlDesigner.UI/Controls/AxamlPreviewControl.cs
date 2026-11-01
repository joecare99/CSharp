using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia;
using Avalonia.Controls;
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
        AddHandler(InputElement.PointerPressedEvent, OnPreviewPointerPressed, RoutingStrategies.Bubble);
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
        if (!node.IsSupported)
        {
            return WrapNode(node, new Border
            {
                Padding = new Thickness(8),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.DarkOrange,
                Child = new TextBlock { Text = $"Unsupported control: {node.TypeName}" },
            });
        }

        Control content = node.TypeName switch
        {
            "Border" => BuildBorder(node),
            "Button" => new Button
            {
                Content = node.Text ?? "Button",
                IsEnabled = false,
                HorizontalAlignment = ParseAlignment(node, "HorizontalAlignment"),
                VerticalAlignment = ParseVerticalAlignment(node),
            },
            "CheckBox" => new CheckBox
            {
                Content = node.Text ?? "CheckBox",
                IsEnabled = false,
                IsChecked = ParseBoolean(node, "IsChecked"),
            },
            "ComboBox" => new ComboBox
            {
                PlaceholderText = node.Text ?? "ComboBox",
                IsEnabled = false,
            },
            "ContentControl" => BuildContentControl(node),
            "DockPanel" => BuildDockPanel(node),
            "Grid" => BuildGrid(node),
            "Label" or "TextBlock" => BuildTextBlock(node),
            "ListBox" => new ListBox
            {
                ItemsSource = node.Children.Select(static child => child.Text ?? child.TypeName).ToArray(),
                IsEnabled = false,
            },
            "ProgressBar" => new ProgressBar
            {
                Minimum = ParseDouble(node, "Minimum", 0),
                Maximum = ParseDouble(node, "Maximum", 100),
                Value = ParseDouble(node, "Value", 50),
            },
            "RadioButton" => new RadioButton
            {
                Content = node.Text ?? "RadioButton",
                IsEnabled = false,
                IsChecked = ParseBoolean(node, "IsChecked"),
            },
            "ScrollViewer" => BuildScrollViewer(node),
            "Separator" => new Separator(),
            "Slider" => new Slider
            {
                Minimum = ParseDouble(node, "Minimum", 0),
                Maximum = ParseDouble(node, "Maximum", 100),
                Value = ParseDouble(node, "Value", 50),
                IsEnabled = false,
            },
            "StackPanel" => BuildStackPanel(node),
            "TextBox" => new TextBox
            {
                Text = node.Text,
                PlaceholderText = GetProperty(node, "Watermark") ?? GetProperty(node, "PlaceholderText"),
                IsReadOnly = true,
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
        if (ParseOptionalDouble(node, "Width") is double width)
        {
            content.Width = width;
        }

        if (ParseOptionalDouble(node, "Height") is double height)
        {
            content.Height = height;
        }

        content.IsHitTestVisible = false;
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
            Tag = node,
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
            Child = node.Children.FirstOrDefault() is { } child ? BuildControl(child) : null,
        };
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

    private Control BuildContentControl(AxamlPreviewNode node)
    {
        return new ContentControl
        {
            Content = BuildChildren(node),
        };
    }

    private Control BuildDockPanel(AxamlPreviewNode node)
    {
        DockPanel panel = new();
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
        Grid grid = new();
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
            if (TryGetInt(child, "Grid.Row", out int row))
            {
                Grid.SetRow(control, row);
            }

            if (TryGetInt(child, "Grid.Column", out int column))
            {
                Grid.SetColumn(control, column);
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
            FontSize = ParseDouble(node, "FontSize", 14),
            FontWeight = ParseFontWeight(node),
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = ParseAlignment(node, "HorizontalAlignment"),
            VerticalAlignment = ParseVerticalAlignment(node),
        };
        return textBlock;
    }

    private Control BuildScrollViewer(AxamlPreviewNode node)
    {
        return new ScrollViewer
        {
            Content = BuildChildren(node),
        };
    }

    private Control BuildStackPanel(AxamlPreviewNode node)
    {
        StackPanel panel = new()
        {
            Orientation = Enum.TryParse(GetProperty(node, "Orientation"), ignoreCase: true, out Orientation orientation)
                ? orientation
                : Orientation.Vertical,
            Spacing = ParseDouble(node, "Spacing", 0),
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
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            panel.Children.Add(BuildControl(child));
        }

        return panel;
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
                ? new GridLength(starValue, GridUnitType.Star)
                : GridLength.Star;
        }

        return double.TryParse(value, CultureInfo.InvariantCulture, out double pixelValue)
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

    private static HorizontalAlignment ParseAlignment(AxamlPreviewNode node, string propertyName)
    {
        return Enum.TryParse(GetProperty(node, propertyName), ignoreCase: true, out HorizontalAlignment alignment)
            ? alignment
            : HorizontalAlignment.Stretch;
    }

    private static VerticalAlignment ParseVerticalAlignment(AxamlPreviewNode node)
    {
        return Enum.TryParse(GetProperty(node, "VerticalAlignment"), ignoreCase: true, out VerticalAlignment alignment)
            ? alignment
            : VerticalAlignment.Stretch;
    }

    private static FontWeight ParseFontWeight(AxamlPreviewNode node)
    {
        return string.Equals(GetProperty(node, "FontWeight"), "Bold", StringComparison.OrdinalIgnoreCase)
            ? FontWeight.Bold
            : FontWeight.Normal;
    }

    private static double? ParseOptionalDouble(AxamlPreviewNode node, string propertyName)
    {
        return double.TryParse(GetProperty(node, propertyName), CultureInfo.InvariantCulture, out double value)
            && double.IsFinite(value)
                ? value
                : null;
    }

    private static double ParseDouble(AxamlPreviewNode node, string propertyName, double fallback)
    {
        return double.TryParse(GetProperty(node, propertyName), CultureInfo.InvariantCulture, out double value)
            ? value
            : fallback;
    }

    private static bool? ParseBoolean(AxamlPreviewNode node, string propertyName)
    {
        return bool.TryParse(GetProperty(node, propertyName), out bool value) ? value : null;
    }

    private static bool TryGetInt(AxamlPreviewNode node, string propertyName, out int value)
    {
        return int.TryParse(GetProperty(node, propertyName), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
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
