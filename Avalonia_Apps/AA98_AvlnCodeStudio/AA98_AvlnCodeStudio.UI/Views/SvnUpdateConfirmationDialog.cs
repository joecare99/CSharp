using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Views;

public sealed class SvnUpdateConfirmationDialog : Window
{
    public SvnUpdateConfirmationDialog(string workingCopyPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workingCopyPath);

        Title = "Confirm SVN update";
        Width = 560;
        Height = 260;
        MinWidth = 460;
        MinHeight = 220;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        Button cancelButton = new()
        {
            Content = "Cancel",
            MinWidth = 90,
        };
        Button updateButton = new()
        {
            Content = "Update working copy",
            MinWidth = 150,
        };
        cancelButton.Click += (_, _) => Close(false);
        updateButton.Click += (_, _) => Close(true);

        StackPanel content = new()
        {
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = "This will update the working copy from the SVN server. Local modifications are not committed; conflicts may require manual resolution.",
                    TextWrapping = TextWrapping.Wrap,
                },
                new TextBox
                {
                    Text = workingCopyPath,
                    IsReadOnly = true,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("monospace"),
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Spacing = 8,
                    Children = { cancelButton, updateButton },
                },
            },
        };
        Content = new Border
        {
            Padding = new Thickness(14),
            Child = content,
        };
    }

    public new Task<bool> ShowDialog(Window owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        return base.ShowDialog<bool>(owner);
    }
}
