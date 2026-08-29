using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using ConsoleLib.Interfaces;
using AControl = Avalonia.Controls.Control;
using AGrid = Avalonia.Controls.Grid;
using APanel = Avalonia.Controls.Panel;
using AStackPanel = Avalonia.Controls.StackPanel;
using ACanvas = Avalonia.Controls.Canvas;

namespace ConsoleLib.Cxaml.Designer.Preview;

public sealed class AvaloniaPreviewRenderer
{
    private const double PreviewCellWidth = 8;
    private const double PreviewCellHeight = 28;
    private const double DefaultPanelPreviewWidth = 80 * PreviewCellWidth;
    private const double DefaultPanelPreviewHeight = 25 * PreviewCellHeight;

    public DesignerPreviewState Render(IControl consoleRoot)
    {
        if (consoleRoot is null)
            throw new ArgumentNullException(nameof(consoleRoot));

        var mappings = new List<PreviewControlMapping>();
        var root = BuildVisual(consoleRoot, Array.Empty<int>(), mappings);
        var state = new DesignerPreviewState(consoleRoot, root, mappings);
        foreach (var mapping in mappings)
        {
            var selectedId = mapping.Id;
            mapping.PreviewControl.AddHandler(InputElement.PointerPressedEvent, (_, e) =>
            {
                if (e.Source is not Visual source)
                    return;

                Visual? current = source;
                while (current is not null)
                {
                    var mapped = mappings.FirstOrDefault(candidate =>
                        ReferenceEquals(candidate.PreviewControl, current));
                    if (mapped is not null)
                    {
                        if (mapped.Id == selectedId)
                        {
                            state.ActivateSelection(selectedId);
                            e.Handled = true;
                        }
                        return;
                    }

                    current = current.GetVisualParent();
                }
            }, RoutingStrategies.Bubble, handledEventsToo: true);
        }

        return state;
    }

    private static AControl BuildVisual(
        IControl consoleControl,
        IReadOnlyList<int> sourcePath,
        ICollection<PreviewControlMapping> mappings)
    {
        var visual = CreateVisual(consoleControl, out var childHost);
        ApplyCommonProperties(consoleControl, visual);

        var mapping = new PreviewControlMapping(
            ToId(sourcePath),
            sourcePath.ToArray(),
            consoleControl,
            visual);
        mappings.Add(mapping);

        if (childHost is not null)
        {
            for (var index = 0; index < consoleControl.Children.Count; index++)
            {
                var child = BuildVisual(
                    consoleControl.Children[index],
                    Append(sourcePath, index),
                    mappings);
                ApplyGridPosition(consoleControl, consoleControl.Children[index], child);
                childHost.Children.Add(child);
            }

            if (consoleControl is ConsoleLib.CommonControls.Panel)
                ApplyPanelLayout(consoleControl, visual, childHost);
        }

        return visual;
    }

    private static AControl CreateVisual(IControl consoleControl, out APanel? childHost)
    {
        childHost = null;
        switch (consoleControl)
        {
            case ConsoleLib.CommonControls.StackPanel stack:
                var stackPanel = new AStackPanel
                {
                    Orientation = stack.Orientation == ConsoleLib.CommonControls.Orientation.Horizontal
                        ? Avalonia.Layout.Orientation.Horizontal
                        : Avalonia.Layout.Orientation.Vertical,
                    Spacing = stack.Spacing
                };
                childHost = stackPanel;
                return stackPanel;

            case ConsoleLib.CommonControls.Grid grid:
                var aGrid = new AGrid();
                foreach (var row in grid.RowDefinitions)
                    aGrid.RowDefinitions.Add(new Avalonia.Controls.RowDefinition(ToGridLength(row.Height)));
                foreach (var column in grid.ColumnDefinitions)
                    aGrid.ColumnDefinitions.Add(new Avalonia.Controls.ColumnDefinition(ToGridLength(column.Width)));
                childHost = aGrid;
                return aGrid;

            case ConsoleLib.CommonControls.Panel:
                var panelHost = new ACanvas
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
                };
                childHost = panelHost;
                return new Border { Child = panelHost };

            case ConsoleLib.CommonControls.Button button:
                return new Avalonia.Controls.Button { Content = DisplayText(button, "Button") };

            case ConsoleLib.CommonControls.Label label:
                return new Border
                {
                    Child = new TextBlock { Text = DisplayText(label, "Label") }
                };

            case ConsoleLib.CommonControls.TextBox textBox:
                return new Avalonia.Controls.TextBox
                {
                    Text = DisplayText(textBox, "TextBox"),
                    AcceptsReturn = textBox.MultiLine,
                    IsReadOnly = true
                };

            case ConsoleLib.CommonControls.CheckBox checkBox:
                return new Avalonia.Controls.CheckBox
                {
                    Content = DisplayText(checkBox, "CheckBox"),
                    IsChecked = checkBox.IsChecked
                };

            case ConsoleLib.CommonControls.ComboBox comboBox:
                return new Avalonia.Controls.ComboBox
                {
                    ItemsSource = comboBox.Items.Count == 0
                        ? new[] { DisplayText(comboBox, "ComboBox") }
                        : comboBox.Items.ToArray(),
                    SelectedIndex = comboBox.SelectedIndex
                };

            case ConsoleLib.CommonControls.ListBox listBox:
                if (listBox.ItemsSource is null)
                {
                    return new Border
                    {
                        Child = new TextBlock { Text = DisplayText(listBox, "ListBox") }
                    };
                }

                return new Avalonia.Controls.ListBox
                {
                    ItemsSource = listBox.ItemsSource as IEnumerable
                };

            case ConsoleLib.CommonControls.TreeView treeView:
                return new Avalonia.Controls.TreeView
                {
                    ItemsSource = treeView.Nodes.Select(node => node.Text).ToArray()
                };

            case ConsoleLib.CommonControls.TileView tileView:
                return new ItemsControl
                {
                    ItemsSource = tileView.Items.Select(item => item.Text).ToArray()
                };

            default:
                return new Border
                {
                    Child = new TextBlock
                    {
                        Text = string.IsNullOrEmpty(consoleControl.Text)
                            ? consoleControl.GetType().Name
                            : consoleControl.Text
                    }
                };
        }
    }

    private static void ApplyCommonProperties(IControl source, AControl target)
    {
        target.IsVisible = source.IsVisible;
        target.IsEnabled = source.Enabled;
        if (source.size.Width > 0)
            target.Width = source.size.Width;
        if (source.size.Height > 0)
            target.Height = source.size.Height;

        var background = new SolidColorBrush(ToColor(source.GetActualBackColor()));
        var foreground = new SolidColorBrush(ToColor(source.GetActualForeColor()));
        switch (target)
        {
            case Border border:
                border.Background = background;
                border.BorderBrush = foreground;
                border.BorderThickness = new Thickness(1);
                if (border.Child is TextBlock childText)
                    childText.Foreground = foreground;
                break;
            case TextBlock textBlock:
                textBlock.Foreground = foreground;
                break;
            case Avalonia.Controls.CheckBox checkBox:
                checkBox.Foreground = foreground;
                checkBox.Background = background;
                break;
            case Avalonia.Controls.Button button:
                button.Foreground = foreground;
                button.Background = background;
                button.MinHeight = 28;
                break;
            case Avalonia.Controls.TextBox textBox:
                textBox.Foreground = foreground;
                textBox.Background = background;
                break;
            case Avalonia.Controls.ComboBox comboBox:
                comboBox.Foreground = foreground;
                comboBox.Background = background;
                break;
            case Avalonia.Controls.ListBox listBox:
                listBox.Foreground = foreground;
                listBox.Background = background;
                break;
        }
    }

    private static string DisplayText(IControl control, string typeName) =>
        string.IsNullOrWhiteSpace(control.Text) ? "[" + typeName + "]" : control.Text;

    private static void ApplyGridPosition(IControl parent, IControl child, AControl visual)
    {
        if (parent is ConsoleLib.CommonControls.Panel)
        {
            ACanvas.SetLeft(visual, child.Position.X * PreviewCellWidth);
            ACanvas.SetTop(visual, child.Position.Y * PreviewCellHeight);
            if (child.size.Width > 0)
                visual.Width = child.size.Width * PreviewCellWidth;
            if (child.size.Height > 0)
                visual.Height = child.size.Height * PreviewCellHeight;
            return;
        }

        if (parent is not ConsoleLib.CommonControls.Grid grid)
            return;

        AGrid.SetRow(visual, ConsoleLib.CommonControls.Grid.GetRow(child));
        AGrid.SetColumn(visual, ConsoleLib.CommonControls.Grid.GetColumn(child));
        AGrid.SetRowSpan(visual, ConsoleLib.CommonControls.Grid.GetRowSpan(child));
        AGrid.SetColumnSpan(visual, ConsoleLib.CommonControls.Grid.GetColumnSpan(child));
    }

    private static void ApplyPanelLayout(IControl source, AControl target, APanel childHost)
    {
        var width = source.size.Width > 0
            ? source.size.Width * PreviewCellWidth
            : DefaultPanelPreviewWidth;
        var height = source.size.Height > 0
            ? source.size.Height * PreviewCellHeight
            : DefaultPanelPreviewHeight;

        foreach (var child in source.Children)
        {
            width = Math.Max(width, (child.Position.X + Math.Max(child.size.Width, 1)) * PreviewCellWidth);
            height = Math.Max(height, (child.Position.Y + Math.Max(child.size.Height, 1)) * PreviewCellHeight);
        }

        target.Width = width;
        target.Height = height;
        target.MinWidth = width;
        target.MinHeight = height;
        target.ClipToBounds = false;
        childHost.Width = width;
        childHost.Height = height;
    }

    private static Avalonia.Controls.GridLength ToGridLength(ConsoleLib.CommonControls.GridLength length)
    {
        return length.GridUnitType switch
        {
            ConsoleLib.CommonControls.GridUnitType.Auto => Avalonia.Controls.GridLength.Auto,
            ConsoleLib.CommonControls.GridUnitType.Star => new Avalonia.Controls.GridLength(length.Value, Avalonia.Controls.GridUnitType.Star),
            _ => new Avalonia.Controls.GridLength(length.Value, Avalonia.Controls.GridUnitType.Pixel)
        };
    }

    private static IReadOnlyList<int> Append(IReadOnlyList<int> path, int index)
    {
        var result = new int[path.Count + 1];
        for (var i = 0; i < path.Count; i++)
            result[i] = path[i];
        result[^1] = index;
        return result;
    }

    private static string ToId(IReadOnlyList<int> path) =>
        path.Count == 0 ? "root" : "root/" + string.Join("/", path);

    private static Avalonia.Media.Color ToColor(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => Colors.Black,
            ConsoleColor.DarkBlue => Colors.DarkBlue,
            ConsoleColor.DarkGreen => Colors.DarkGreen,
            ConsoleColor.DarkCyan => Colors.DarkCyan,
            ConsoleColor.DarkRed => Colors.DarkRed,
            ConsoleColor.DarkMagenta => Colors.DarkMagenta,
            ConsoleColor.DarkYellow => Colors.Olive,
            ConsoleColor.Gray => Colors.Gray,
            ConsoleColor.DarkGray => Colors.DarkGray,
            ConsoleColor.Blue => Colors.Blue,
            ConsoleColor.Green => Colors.Green,
            ConsoleColor.Cyan => Colors.Cyan,
            ConsoleColor.Red => Colors.Red,
            ConsoleColor.Magenta => Colors.Magenta,
            ConsoleColor.Yellow => Colors.Yellow,
            ConsoleColor.White => Colors.White,
            _ => Colors.Transparent
        };
    }
}
