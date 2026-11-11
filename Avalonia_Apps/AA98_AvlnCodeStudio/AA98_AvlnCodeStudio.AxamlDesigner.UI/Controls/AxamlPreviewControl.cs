using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Globalization;
using System.Linq;
using LineShape = Avalonia.Controls.Shapes.Line;
using PolygonShape = Avalonia.Controls.Shapes.Polygon;
using PolylineShape = Avalonia.Controls.Shapes.Polyline;

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
            RoutingStrategies.Tunnel,
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

    /// <summary>Finds the foremost visible preview node without activating its inert controls.</summary>
    public AxamlPreviewNode? FindNodeAtPoint(Point point) => HitTestSelection(this, point, false).Node;

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
        if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point point = args.GetPosition(this);
        (AxamlPreviewNode? node, Control? nodeContainer, bool isResizeGrip) = HitTestSelection(this, point, false);
        if (node is null)
        {
            return;
        }

        double width = 0;
        double height = 0;
        if (isResizeGrip
            && nodeContainer is Border { Child: Grid { Children.Count: > 0 } grid }
            && grid.Children[0] is Control content)
        {
            width = content.Bounds.Width;
            height = content.Bounds.Height;
        }

        SetCurrentValue(SelectedNodeProperty, node);
        args.Handled = true;

        if (!isResizeGrip || width <= 0 || height <= 0)
        {
            return;
        }

        _resizeGesture = new ResizeGesture(node, point, width, height);
        args.Pointer.Capture(this);
    }

    private (AxamlPreviewNode? Node, Control? Container, bool IsResizeGrip) HitTestSelection(
        Visual visual, Point point, bool withinGrip)
    {
        if (!visual.IsVisible || this.TranslatePoint(point, visual) is not { } local)
        {
            return default;
        }

        bool contains = new Rect(visual.Bounds.Size).Contains(local);
        if (visual.ClipToBounds && !contains)
        {
            return default;
        }

        bool isGrip = withinGrip || visual is Control control && control.Classes.Contains("axaml-resize-grip") && contains;
        foreach (Visual child in visual.GetVisualChildren().Reverse())
        {
            var result = HitTestSelection(child, point, isGrip);
            if (result.Node is not null)
            {
                return result;
            }

            if (result.IsResizeGrip)
            {
                isGrip = true;
                break;
            }
        }

        if (contains && visual is Control taggedControl)
        {
            AxamlPreviewNode? node = taggedControl.Tag as AxamlPreviewNode ?? FindItemNode(taggedControl);
            if (node is not null)
            {
                return (node, taggedControl, isGrip);
            }
        }

        return (null, null, isGrip);
    }

    private static AxamlPreviewNode? FindItemNode(Control container)
    {
        ItemsControl? owner = ItemsControl.ItemsControlFromItemContainer(container);
        if (owner is null || owner is not (ListBox or TabControl) && owner.GetType() != typeof(ItemsControl))
        {
            return null;
        }

        AxamlPreviewNode? parent = owner.GetVisualAncestors()
            .OfType<Control>()
            .Select(static ancestor => ancestor.Tag as AxamlPreviewNode)
            .FirstOrDefault(static node => node is not null);
        if (parent is null
            || parent.TypeName != owner.GetType().Name
            || parent.Children.Count != owner.Items.Count)
        {
            return null;
        }

        int index = owner.IndexFromContainer(container);
        return index >= 0 && index < parent.Children.Count ? parent.Children[index] : null;
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
            "AutoCompleteBox" => BuildAutoCompleteBox(node),
            "Border" => BuildBorder(node),
            "Button" => BuildButton(node),
            "Calendar" => BuildCalendar(node),
            "CalendarDatePicker" => BuildCalendarDatePicker(node),
            "Canvas" => BuildCanvas(node),
            "CheckBox" => BuildCheckBox(node),
            "ColorPicker" => BuildColorPicker(node),
            "ComboBox" => ApplyTemplatedControlAppearance(new ComboBox
            {
                PlaceholderText = ParseLiteralText(node, "PlaceholderText", node.Text ?? "ComboBox"),
                ItemsSource = ParseSelectorItems(node),
                SelectedIndex = ParseSelectorIndex(node),
                IsDropDownOpen = false,
                IsEnabled = false,
            }, node),
            "ContentControl" => BuildContentControl(node),
            "ContextMenu" => BuildContextMenu(node),
            "DatePicker" => BuildDatePicker(node),
            "DockPanel" => BuildDockPanel(node),
            "Ellipse" => BuildShape(new Ellipse(), node),
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
            "Image" => BuildImagePreview(node),
            "ItemsControl" => ApplyTemplatedControlAppearance(new ItemsControl
            {
                ItemsSource = ParseSelectorItems(node),
                IsEnabled = false,
            }, node),
            "Label" => ApplyTemplatedControlAppearance(new Label
            {
                Content = BuildTextBlock(node),
            }, node),
            "Line" => BuildShape(new LineShape
            {
                StartPoint = ParsePoint(node, "StartPoint", new Point(0, 0)),
                EndPoint = ParsePoint(node, "EndPoint", new Point(80, 40)),
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            }, node),
            "TextBlock" => BuildTextBlock(node),
            "ListBox" => ApplyTemplatedControlAppearance(new ListBox
            {
                ItemsSource = ParseSelectorItems(node),
                SelectedIndex = ParseSelectorIndex(node),
                IsEnabled = false,
            }, node),
            "Menu" => BuildMenu(node),
            "MenuItem" => BuildMenuItem(node),
            "NumericUpDown" => BuildNumericUpDown(node),
            "ProgressBar" => BuildProgressBar(node),
            "Polygon" => BuildShape(new PolygonShape
            {
                Points = ParsePointList(node, [new Point(40, 0), new Point(80, 60), new Point(0, 60)]),
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            }, node),
            "Polyline" => BuildShape(new PolylineShape
            {
                Points = ParsePointList(node, [new Point(0, 0), new Point(80, 40)]),
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            }, node),
            "Rectangle" => BuildShape(new Rectangle(), node),
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
            "TimePicker" => BuildTimePicker(node),
            "TabControl" => BuildTabControl(node),
            "TreeView" => BuildTreeView(node),
            "TreeViewItem" => BuildTreeViewItem(node),
            "ToolTip" => BuildToolTip(node),
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
        AxamlPreviewNode[] attachedPreviews = node.Children
            .Where(static child => child.TypeName is "ContextMenu" or "ToolTip")
            .ToArray();
        if (attachedPreviews.Length > 0)
        {
            StackPanel preview = new() { Spacing = 6 };
            preview.Children.Add(content);
            foreach (AxamlPreviewNode attachedPreview in attachedPreviews)
            {
                preview.Children.Add(BuildControl(attachedPreview));
            }

            content = preview;
        }

        return WrapNode(node, content);
    }

    private Border WrapNode(AxamlPreviewNode node, Control content)
    {
        bool isSelected = ReferenceEquals(node, SelectedNode);
        ApplyNodeVisualProperties(node, content);
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

    private void ApplyNodeVisualProperties(AxamlPreviewNode node, Control content)
    {
        content.IsEnabled = false;
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

    private static Control BuildImagePreview(AxamlPreviewNode node)
    {
        StackPanel content = new() { Spacing = 4 };
        content.Children.Add(new TextBlock
        {
            Text = "Image preview is inert; the source is not loaded.",
            FontWeight = FontWeight.SemiBold,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
        });

        string? source = ParseOptionalLiteralText(node, "Source");
        if (source is not null)
        {
            content.Children.Add(new TextBlock
            {
                Text = $"Source: {source}",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            });
        }

        return new Border
        {
            MinWidth = 120,
            MinHeight = 48,
            Padding = new Thickness(6),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            Child = content,
        };
    }

    private static Shape BuildShape(Shape shape, AxamlPreviewNode node)
    {
        if (ParseSimpleBrush(node, "Fill") is { } fill)
        {
            shape.Fill = fill;
        }

        if (ParseSimpleBrush(node, "Stroke") is { } stroke)
        {
            shape.Stroke = stroke;
        }

        if (HasValidLiteralValue(node, "StrokeThickness"))
        {
            shape.StrokeThickness = ParseDouble(node, "StrokeThickness", 0);
        }

        return shape;
    }

    private static Point ParsePoint(AxamlPreviewNode node, string propertyName, Point fallback)
    {
        string? value = GetProperty(node, propertyName);
        if (AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value ?? string.Empty) is not null
            || value is null
            || !TryParsePoint(value, out Point point))
        {
            return fallback;
        }

        return point;
    }

    private static Point[] ParsePointList(AxamlPreviewNode node, Point[] fallback)
    {
        string? value = GetProperty(node, "Points");
        if (value is null
            || AxamlPropertyValueValidator.Validate(node.TypeName, "Points", value) is not null)
        {
            return fallback;
        }

        string[] pointValues = value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        Point[] points = new Point[pointValues.Length];
        for (int index = 0; index < pointValues.Length; index++)
        {
            if (!TryParsePoint(pointValues[index], out points[index]))
            {
                return fallback;
            }
        }

        return points;
    }

    private static bool TryParsePoint(string value, out Point point)
    {
        string[] coordinates = value.Split(',', StringSplitOptions.TrimEntries);
        if (coordinates.Length == 2
            && double.TryParse(coordinates[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x)
            && double.TryParse(coordinates[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
        {
            point = new Point(x, y);
            return true;
        }

        point = default;
        return false;
    }

    private Control BuildButton(AxamlPreviewNode node)
    {
        Button button = new()
        {
            Content = node.Text ?? "Button",
            CommandParameter = ParseOptionalLiteralText(node, "CommandParameter"),
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

    private static Control BuildDatePicker(AxamlPreviewNode node)
    {
        DatePicker datePicker = ApplyTemplatedControlAppearance(new DatePicker
        {
            DayVisible = ParseBoolean(node, "DayVisible") ?? true,
            MonthVisible = ParseBoolean(node, "MonthVisible") ?? true,
            YearVisible = ParseBoolean(node, "YearVisible") ?? true,
            IsEnabled = false,
        }, node);

        ApplyDatePickerYearRange(node, datePicker);
        DateTimeOffset? selectedDate = ParseDatePickerSelectedDate(node);
        if (selectedDate is { } date
            && date >= datePicker.MinYear
            && date <= datePicker.MaxYear)
        {
            datePicker.SelectedDate = date;
        }

        if (ParseValidatedFormat(node, "DayFormat") is { } dayFormat)
        {
            datePicker.DayFormat = dayFormat;
        }

        if (ParseValidatedFormat(node, "MonthFormat") is { } monthFormat)
        {
            datePicker.MonthFormat = monthFormat;
        }

        if (ParseValidatedFormat(node, "YearFormat") is { } yearFormat)
        {
            datePicker.YearFormat = yearFormat;
        }

        return datePicker;
    }

    private static Control BuildCalendarDatePicker(AxamlPreviewNode node)
    {
        CalendarDatePicker datePicker = ApplyTemplatedControlAppearance(new CalendarDatePicker
        {
            IsDropDownOpen = false,
            IsTodayHighlighted = ParseBoolean(node, "IsTodayHighlighted") ?? true,
            PlaceholderText = ParseLiteralText(node, "PlaceholderText", "Choose a date"),
            IsEnabled = false,
        }, node);
        if (ParseCalendarDate(node, "SelectedDate") is { } selectedDate)
        {
            datePicker.SelectedDate = selectedDate;
        }

        if (ParseBoolean(node, "UseFloatingPlaceholder") is bool useFloatingPlaceholder)
        {
            datePicker.UseFloatingPlaceholder = useFloatingPlaceholder;
        }

        return datePicker;
    }

    private static void ApplyDatePickerYearRange(AxamlPreviewNode node, DatePicker datePicker)
    {
        DateTimeOffset minimumYear = HasValidLiteralValue(node, "MinYear")
            && DateTimeOffset.TryParseExact(
                GetProperty(node, "MinYear"),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTimeOffset parsedMinimumYear)
                    ? parsedMinimumYear
                    : datePicker.MinYear;
        DateTimeOffset maximumYear = HasValidLiteralValue(node, "MaxYear")
            && DateTimeOffset.TryParseExact(
                GetProperty(node, "MaxYear"),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTimeOffset parsedMaximumYear)
                    ? parsedMaximumYear
                    : datePicker.MaxYear;
        if (minimumYear > maximumYear)
        {
            return;
        }

        if (minimumYear < datePicker.MinYear)
        {
            datePicker.MinYear = minimumYear;
            datePicker.MaxYear = maximumYear;
        }
        else
        {
            datePicker.MaxYear = maximumYear;
            datePicker.MinYear = minimumYear;
        }
    }

    private static Control BuildTimePicker(AxamlPreviewNode node)
    {
        TimePicker timePicker = ApplyTemplatedControlAppearance(new TimePicker
        {
            ClockIdentifier = AxamlPropertyValueValidator.Validate(
                "TimePicker",
                "ClockIdentifier",
                GetProperty(node, "ClockIdentifier") ?? "24HourClock") is null
                    ? GetProperty(node, "ClockIdentifier") ?? "24HourClock"
                    : "24HourClock",
            MinuteIncrement = ParseTimePickerIncrement(node, "MinuteIncrement"),
            SecondIncrement = ParseTimePickerIncrement(node, "SecondIncrement"),
            UseSeconds = ParseBoolean(node, "UseSeconds") ?? false,
            IsEnabled = false,
        }, node);

        string? selectedTime = GetProperty(node, "SelectedTime");
        if (HasValidLiteralValue(node, "SelectedTime")
            && TimeSpan.TryParseExact(
                selectedTime,
                ["hh\\:mm", "hh\\:mm\\:ss"],
                CultureInfo.InvariantCulture,
                TimeSpanStyles.None,
                out TimeSpan time))
        {
            timePicker.SelectedTime = time;
        }

        return timePicker;
    }

    private static int ParseTimePickerIncrement(AxamlPreviewNode node, string propertyName)
    {
        return HasValidLiteralValue(node, propertyName) && TryGetInt(node, propertyName, out int increment)
            ? increment
            : 1;
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
            CommandParameter = ParseOptionalLiteralText(node, "CommandParameter"),
            IsThreeState = ParseBoolean(node, "IsThreeState") ?? false,
            IsEnabled = false,
            IsChecked = ParseBoolean(node, "IsChecked"),
        }, node);
    }

    private static Control BuildCalendar(AxamlPreviewNode node)
    {
        Avalonia.Controls.Calendar calendar = ApplyTemplatedControlAppearance(new Avalonia.Controls.Calendar
        {
            DisplayMode = ParseCalendarDisplayMode(node),
            SelectionMode = ParseCalendarSelectionMode(node),
            FirstDayOfWeek = ParseCalendarFirstDayOfWeek(node),
            IsTodayHighlighted = ParseBoolean(node, "IsTodayHighlighted") ?? true,
            IsEnabled = false,
        }, node);

        DateTime? displayDateStart = ParseCalendarDate(node, "DisplayDateStart");
        DateTime? displayDateEnd = ParseCalendarDate(node, "DisplayDateEnd");
        if (displayDateStart is { } start && displayDateEnd is { } end && start > end)
        {
            displayDateStart = null;
            displayDateEnd = null;
        }

        if (displayDateEnd is { } validEnd)
        {
            calendar.DisplayDateEnd = validEnd;
        }

        if (displayDateStart is { } validStart)
        {
            calendar.DisplayDateStart = validStart;
        }

        DateTime? selectedDate = ParseCalendarDate(node, "SelectedDate");
        if (calendar.SelectionMode == CalendarSelectionMode.SingleDate
            && selectedDate is { } selected
            && IsCalendarDateInRange(selected, displayDateStart, displayDateEnd))
        {
            calendar.SelectedDate = selected;
        }

        DateTime? requestedDisplayDate = ParseCalendarDate(node, "DisplayDate");
        if (requestedDisplayDate is { } requested
            && IsCalendarDateInRange(requested, displayDateStart, displayDateEnd))
        {
            calendar.DisplayDate = requested;
        }
        else if (calendar.SelectedDate is { } selection)
        {
            calendar.DisplayDate = selection;
        }
        else if (displayDateStart is { } firstVisibleDate)
        {
            calendar.DisplayDate = firstVisibleDate;
        }
        else if (displayDateEnd is { } lastVisibleDate
            && calendar.DisplayDate > lastVisibleDate)
        {
            calendar.DisplayDate = lastVisibleDate;
        }

        return calendar;
    }

    private static AutoCompleteBox BuildAutoCompleteBox(AxamlPreviewNode node)
    {
        return ApplyTemplatedControlAppearance(new AutoCompleteBox
        {
            Text = ParseLiteralText(node, "Text", string.Empty),
            PlaceholderText = ParseLiteralText(node, "PlaceholderText", string.Empty),
            FilterMode = HasValidLiteralValue(node, "FilterMode")
                && Enum.TryParse(GetProperty(node, "FilterMode"), ignoreCase: true, out AutoCompleteFilterMode filterMode)
                && Enum.IsDefined(filterMode)
                    ? filterMode
                    : AutoCompleteFilterMode.StartsWith,
            MinimumPrefixLength = HasValidLiteralValue(node, "MinimumPrefixLength")
                && TryGetInt(node, "MinimumPrefixLength", out int minimumPrefixLength)
                    ? minimumPrefixLength
                    : 1,
            IsTextCompletionEnabled = ParseBoolean(node, "IsTextCompletionEnabled") ?? false,
            IsDropDownOpen = false,
            IsEnabled = false,
        }, node);
    }

    private static ColorPicker BuildColorPicker(AxamlPreviewNode node)
    {
        ColorPicker colorPicker = ApplyTemplatedControlAppearance(new ColorPicker
        {
            ColorModel = HasValidLiteralValue(node, "ColorModel")
                && Enum.TryParse(GetProperty(node, "ColorModel"), ignoreCase: true, out ColorModel colorModel)
                && Enum.IsDefined(colorModel)
                    ? colorModel
                    : ColorModel.Rgba,
            IsAccentColorsVisible = ParseBoolean(node, "IsAccentColorsVisible") ?? true,
            IsAlphaEnabled = ParseBoolean(node, "IsAlphaEnabled") ?? true,
            IsAlphaVisible = ParseBoolean(node, "IsAlphaVisible") ?? true,
            IsColorComponentsVisible = ParseBoolean(node, "IsColorComponentsVisible") ?? true,
            IsColorModelVisible = ParseBoolean(node, "IsColorModelVisible") ?? true,
            IsColorPaletteVisible = ParseBoolean(node, "IsColorPaletteVisible") ?? true,
            IsColorPreviewVisible = ParseBoolean(node, "IsColorPreviewVisible") ?? true,
            IsColorSpectrumSliderVisible = ParseBoolean(node, "IsColorSpectrumSliderVisible") ?? true,
            IsColorSpectrumVisible = ParseBoolean(node, "IsColorSpectrumVisible") ?? true,
            IsComponentSliderVisible = ParseBoolean(node, "IsComponentSliderVisible") ?? true,
            IsComponentTextInputVisible = ParseBoolean(node, "IsComponentTextInputVisible") ?? true,
            IsHexInputVisible = ParseBoolean(node, "IsHexInputVisible") ?? true,
            IsEnabled = false,
        }, node);

        string? value = GetProperty(node, "Color");
        if (HasValidLiteralValue(node, "Color") && Color.TryParse(value, out Color color))
        {
            colorPicker.Color = color;
        }

        return colorPicker;
    }

    private static CalendarSelectionMode ParseCalendarSelectionMode(AxamlPreviewNode node)
    {
        return HasValidLiteralValue(node, "SelectionMode")
            && Enum.TryParse(GetProperty(node, "SelectionMode"), ignoreCase: true, out CalendarSelectionMode selectionMode)
            && selectionMode is CalendarSelectionMode.None
                or CalendarSelectionMode.SingleDate
                or CalendarSelectionMode.SingleRange
                or CalendarSelectionMode.MultipleRange
                ? selectionMode
                : CalendarSelectionMode.SingleDate;
    }

    private static CalendarMode ParseCalendarDisplayMode(AxamlPreviewNode node)
    {
        return HasValidLiteralValue(node, "DisplayMode")
            && Enum.TryParse(GetProperty(node, "DisplayMode"), ignoreCase: true, out CalendarMode displayMode)
            && Enum.IsDefined(displayMode)
                ? displayMode
                : CalendarMode.Month;
    }

    private static DayOfWeek ParseCalendarFirstDayOfWeek(AxamlPreviewNode node)
    {
        return HasValidLiteralValue(node, "FirstDayOfWeek")
            && Enum.TryParse(GetProperty(node, "FirstDayOfWeek"), ignoreCase: true, out DayOfWeek firstDayOfWeek)
            && Enum.IsDefined(firstDayOfWeek)
                ? firstDayOfWeek
                : DayOfWeek.Monday;
    }

    private static DateTime? ParseCalendarDate(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        return HasValidLiteralValue(node, propertyName)
            && DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date)
                    ? date
                    : null;
    }

    private static bool IsCalendarDateInRange(DateTime date, DateTime? displayDateStart, DateTime? displayDateEnd)
    {
        return (displayDateStart is null || date >= displayDateStart.Value)
            && (displayDateEnd is null || date <= displayDateEnd.Value);
    }

    private static RadioButton BuildRadioButton(AxamlPreviewNode node)
    {
        return ApplyTemplatedControlAppearance(new RadioButton
        {
            Content = node.Text ?? "RadioButton",
            CommandParameter = ParseOptionalLiteralText(node, "CommandParameter"),
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

        if (HasValidLiteralValue(node, "FontFamily"))
        {
            control.FontFamily = new FontFamily(GetProperty(node, "FontFamily")!);
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

        if (ParseSimpleBrush(node, "PlaceholderForeground") is { } placeholderForeground)
        {
            switch (control)
            {
                case AutoCompleteBox autoCompleteBox:
                    autoCompleteBox.PlaceholderForeground = placeholderForeground;
                    break;
                case CalendarDatePicker calendarDatePicker:
                    calendarDatePicker.PlaceholderForeground = placeholderForeground;
                    break;
                case ComboBox comboBox:
                    comboBox.PlaceholderForeground = placeholderForeground;
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown.PlaceholderForeground = placeholderForeground;
                    break;
                case TextBox textBox:
                    textBox.PlaceholderForeground = placeholderForeground;
                    break;
            }
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
            CommandParameter = ParseOptionalLiteralText(node, "CommandParameter"),
            IsThreeState = ParseBoolean(node, "IsThreeState") ?? false,
            IsEnabled = false,
            IsChecked = ParseBoolean(node, "IsChecked"),
        };
        if (node.RenderProperties.TryGetValue("OffContent", out string? offContent))
        {
            toggleSwitch.OffContent = offContent;
        }

        if (node.RenderProperties.TryGetValue("OnContent", out string? onContent))
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

    private Control BuildTabControl(AxamlPreviewNode node)
    {
        TabControl tabControl = new()
        {
            IsEnabled = false,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName != "TabItem")
            {
                continue;
            }

            TabItem tabItem = ApplyTemplatedControlAppearance(new TabItem
            {
                Header = ParseLiteralText(child, "Header", child.Text ?? "Tab"),
                Content = BuildChildren(child),
            }, child);
            tabControl.Items.Add(tabItem);
        }

        tabControl.SelectedIndex = ParseSelectorIndex(node);
        return ApplyTemplatedControlAppearance(tabControl, node);
    }

    private Control BuildTreeView(AxamlPreviewNode node)
    {
        TreeView treeView = new() { IsEnabled = false };
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName == "TreeViewItem")
            {
                treeView.Items.Add(BuildTreeViewItem(child));
            }
        }

        return ApplyTemplatedControlAppearance(treeView, node);
    }

    private Control BuildTreeViewItem(AxamlPreviewNode node)
    {
        TreeViewItem item = new()
        {
            Header = ParseLiteralText(node, "Header", "Tree item"),
            IsExpanded = ParseBoolean(node, "IsExpanded") ?? false,
            IsEnabled = false,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName == "TreeViewItem")
            {
                item.Items.Add(BuildTreeViewItem(child));
            }
        }

        item = ApplyTemplatedControlAppearance(item, node);
        item.Tag = node.SourceStartOffset >= 0 ? node : null;
        if (ReferenceEquals(node, SelectedNode))
        {
            item.BorderBrush = Brushes.DodgerBlue;
            item.BorderThickness = new Thickness(1);
        }

        return item;
    }

    private Control BuildMenu(AxamlPreviewNode node)
    {
        Menu menu = new()
        {
            IsEnabled = false,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName is not ("MenuItem" or "Separator"))
            {
                continue;
            }

            menu.Items.Add(BuildMenuChild(child));
        }

        return ApplyTemplatedControlAppearance(menu, node);
    }

    private Control BuildContextMenu(AxamlPreviewNode node)
    {
        StackPanel content = new() { Spacing = 4 };
        content.Children.Add(new TextBlock
        {
            Text = "ContextMenu (closed, inert preview)",
            FontStyle = FontStyle.Italic,
        });

        StackPanel items = new() { Spacing = 2 };
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName is "MenuItem" or "Separator")
            {
                items.Children.Add(BuildMenuChild(child));
            }
        }

        content.Children.Add(items);
        return new Border
        {
            Padding = new Thickness(6),
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Gray,
            Child = content,
        };
    }

    private Control BuildToolTip(AxamlPreviewNode node)
    {
        StackPanel content = new() { Spacing = 4 };
        content.Children.Add(new TextBlock
        {
            Text = "ToolTip (closed, inert preview)",
            FontStyle = FontStyle.Italic,
        });
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName is "Border" or "TextBlock" or "StackPanel")
            {
                content.Children.Add(BuildControl(child));
            }
        }

        return new Border
        {
            Padding = new Thickness(6),
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Gray,
            Child = content,
        };
    }

    private Control BuildMenuItem(AxamlPreviewNode node)
    {
        MenuItem menuItem = new()
        {
            CommandParameter = ParseOptionalLiteralText(node, "CommandParameter"),
            GroupName = ParseLiteralText(node, "GroupName", string.Empty),
            Header = ParseLiteralText(node, "Header", "Menu item"),
            Icon = ParseOptionalLiteralText(node, "Icon") is { } iconText
                ? new TextBlock { Text = iconText }
                : null,
            InputGesture = ParseInputGesture(node, "InputGesture")
                ?? ParseInputGesture(node, "HotKey"),
            ToggleType = HasValidLiteralValue(node, "ToggleType")
                && Enum.TryParse(GetProperty(node, "ToggleType"), ignoreCase: true, out MenuItemToggleType toggleType)
                && Enum.IsDefined(toggleType)
                    ? toggleType
                    : MenuItemToggleType.None,
            IsChecked = ParseBoolean(node, "IsChecked") ?? false,
            IsSubMenuOpen = false,
            StaysOpenOnClick = ParseBoolean(node, "StaysOpenOnClick") ?? false,
            IsEnabled = false,
        };
        foreach (AxamlPreviewNode child in node.Children)
        {
            if (child.TypeName is not ("MenuItem" or "Separator"))
            {
                continue;
            }

            menuItem.Items.Add(BuildMenuChild(child));
        }

        menuItem = ApplyTemplatedControlAppearance(menuItem, node);
        ApplyNodeVisualProperties(node, menuItem);
        menuItem.Tag = node.SourceStartOffset >= 0 ? node : null;
        if (ReferenceEquals(node, SelectedNode))
        {
            menuItem.BorderBrush = Brushes.DodgerBlue;
            menuItem.BorderThickness = new Thickness(1);
        }

        return menuItem;
    }

    private Control BuildMenuChild(AxamlPreviewNode node)
    {
        if (!node.IsSupported)
        {
            return BuildControl(node);
        }

        if (node.TypeName == "MenuItem")
        {
            return BuildMenuItem(node);
        }

        if (node.TypeName == "Separator")
        {
            Separator separator = new();
            ApplyNodeVisualProperties(node, separator);
            separator.Tag = node.SourceStartOffset >= 0 ? node : null;
            return separator;
        }

        return BuildControl(node);
    }

    private static KeyGesture? ParseInputGesture(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        return string.IsNullOrEmpty(value)
            || AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is not null
                ? null
                : KeyGesture.Parse(value);
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
            string? dockValue = GetProperty(child, "DockPanel.Dock");
            if (dockValue is not null
                && Enum.TryParse(dockValue, ignoreCase: true, out Dock dock)
                && Enum.IsDefined(dock)
                && AxamlPropertyValueValidator.Validate(child.TypeName, "DockPanel.Dock", dockValue) is null)
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

    private Control BuildCanvas(AxamlPreviewNode node)
    {
        Canvas canvas = new();
        if (ParseSimpleBrush(node, "Background") is { } background)
        {
            canvas.Background = background;
        }

        foreach (AxamlPreviewNode child in node.Children)
        {
            Control control = BuildControl(child);
            if (HasValidLiteralValue(child, "Canvas.Left"))
            {
                Canvas.SetLeft(control, ParseDouble(child, "Canvas.Left", 0));
            }

            if (HasValidLiteralValue(child, "Canvas.Top"))
            {
                Canvas.SetTop(control, ParseDouble(child, "Canvas.Top", 0));
            }

            canvas.Children.Add(control);
        }

        return canvas;
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
        if (HasValidLiteralValue(node, "FontFamily"))
        {
            textBlock.FontFamily = new FontFamily(GetProperty(node, "FontFamily")!);
        }

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
            PlaceholderText = ParseLiteralText(
                node,
                "Watermark",
                ParseLiteralText(node, "PlaceholderText", string.Empty)),
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

    private static string? ParseOptionalLiteralText(AxamlPreviewNode node, string propertyName)
    {
        string? value = GetProperty(node, propertyName);
        return value is not null
            && AxamlPropertyValueValidator.Validate(node.TypeName, propertyName, value) is null
                ? value
                : null;
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

    private static int ParseSelectorIndex(AxamlPreviewNode node)
    {
        return HasValidLiteralValue(node, "SelectedIndex")
            && TryGetInt(node, "SelectedIndex", out int selectedIndex)
            && selectedIndex < node.Children.Count
                ? selectedIndex
                : -1;
    }

    private static string[] ParseSelectorItems(AxamlPreviewNode node)
    {
        return node.Children.Select(static child =>
        {
            if (child.Text is not null)
            {
                return child.Text;
            }

            if ((child.TypeName is "ComboBoxItem" or "ListBoxItem")
                && child.Children.Count == 1
                && (child.Children[0].TypeName is "TextBlock" or "Label")
                && child.Children[0].Text is { } contentText)
            {
                return contentText;
            }

            if (child.RenderProperties.TryGetValue("Content", out string? content)
                && !content.TrimStart().StartsWith('{'))
            {
                return content;
            }

            return child.TypeName;
        }).ToArray();
    }

    private static DateTimeOffset? ParseDatePickerSelectedDate(AxamlPreviewNode node)
    {
        string? value = GetProperty(node, "SelectedDate");
        return HasValidLiteralValue(node, "SelectedDate")
            && DateTimeOffset.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTimeOffset selectedDate)
                    ? selectedDate
                    : null;
    }

    private static string? GetProperty(AxamlPreviewNode node, string name)
    {
        return node.RenderProperties.TryGetValue(name, out string? value) ? value : null;
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
