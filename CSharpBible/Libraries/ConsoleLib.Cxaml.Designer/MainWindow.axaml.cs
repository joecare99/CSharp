using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using ConsoleLib.Cxaml.Designer.ViewModels;

namespace ConsoleLib.Cxaml.Designer;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new DesignerViewModel();
    }

    private void ConsolePreviewText_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not DesignerViewModel viewModel ||
            sender is not TextBlock textBlock)
            return;

        var lines = (textBlock.Text ?? string.Empty).Split(Environment.NewLine);
        var lineCount = Math.Max(1, lines.Length);
        var characterCount = lines.Max(line => line.Length);
        if (characterCount == 0 || textBlock.Bounds.Width <= 0 || textBlock.Bounds.Height <= 0)
            return;

        var position = e.GetPosition(textBlock);
        var cellWidth = textBlock.Bounds.Width / characterCount;
        var cellHeight = textBlock.Bounds.Height / lineCount;
        var column = Math.Clamp((int)(position.X / cellWidth), 0, characterCount - 1);
        var row = Math.Clamp((int)(position.Y / cellHeight), 0, lineCount - 1);
        viewModel.ActivateConsoleSelection(column, row);
        e.Handled = true;
    }
}
