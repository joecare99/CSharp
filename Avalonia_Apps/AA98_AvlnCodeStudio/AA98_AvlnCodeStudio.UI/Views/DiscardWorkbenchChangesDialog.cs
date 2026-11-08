using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace AA98_AvlnCodeStudio.UI.Views;

public sealed class DiscardWorkbenchChangesDialog : Window
{
    public DiscardWorkbenchChangesDialog(string documents)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documents);
        Title = "Unsaved changes";
        Width = 560;
        Height = 220;
        MinWidth = 480;
        MinHeight = 200;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        Button discard = new() { Content = "Discard changes", MinWidth = 120 };
        Button cancel = new() { Content = "Cancel", MinWidth = 100 };
        discard.Click += (_, _) => Close(true);
        cancel.Click += (_, _) => Close(false);
        StackPanel buttons = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children = { discard, cancel },
        };
        Grid.SetRow(buttons, 1);
        Content = new Border
        {
            Padding = new Thickness(14),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("*,Auto"),
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Discard unsaved document changes and unapplied property drafts?{Environment.NewLine}{documents}",
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    },
                    buttons,
                },
            },
        };
    }
}
