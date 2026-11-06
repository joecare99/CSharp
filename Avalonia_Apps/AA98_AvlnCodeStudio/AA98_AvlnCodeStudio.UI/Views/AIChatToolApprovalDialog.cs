using AA98_AvlnCodeStudio.Base.AI.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Views;

public sealed class AIChatToolApprovalDialog : Window
{
    public AIChatToolApprovalDialog(AIChatToolDescriptor descriptor, JsonElement arguments)
        : this(descriptor, arguments, null)
    {
    }

    public AIChatToolApprovalDialog(AIChatToolDescriptor descriptor, JsonElement arguments, Action? cancelRequest)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        Title = "Confirm AI action";
        Width = 620;
        Height = 420;
        MinWidth = 520;
        MinHeight = 320;
        CanResize = true;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        Button cancelButton = new()
        {
            Content = "Skip tool",
            MinWidth = 100,
        };
        Button cancelRequestButton = new()
        {
            Content = "Cancel request",
            MinWidth = 120,
            IsVisible = cancelRequest is not null,
        };
        Button approveButton = new()
        {
            Content = "Approve and run",
            MinWidth = 130,
        };
        cancelButton.Click += (_, _) => Close(false);
        cancelRequestButton.Click += (_, _) =>
        {
            cancelRequest?.Invoke();
            Close(false);
        };
        approveButton.Click += (_, _) => Close(true);

        TextBlock title = new()
        {
            Text = $"The assistant wants to run: {descriptor.Name}",
            FontWeight = FontWeight.SemiBold,
            TextWrapping = TextWrapping.Wrap,
        };
        TextBlock description = new()
        {
            Text = cancelRequest is null
                ? descriptor.Description
                : $"{descriptor.Description}{Environment.NewLine}Skip tool continues the conversation without this action. Cancel request stops the current turn.",
            TextWrapping = TextWrapping.Wrap,
        };
        TextBox argumentsText = new()
        {
            Text = JsonSerializer.Serialize(arguments, new JsonSerializerOptions { WriteIndented = true }),
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("monospace"),
        };
        StackPanel buttonPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children = { cancelRequestButton, cancelButton, approveButton },
        };
        Grid dialogContent = new()
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 10,
            Children = { title, description, new ScrollViewer { Content = argumentsText }, buttonPanel },
        };
        Grid.SetRow(description, 1);
        Grid.SetRow(dialogContent.Children[2], 2);
        Grid.SetRow(buttonPanel, 3);

        Content = new Border
        {
            Padding = new Thickness(14),
            Child = dialogContent,
        };
    }

    public new Task<bool> ShowDialog(Window owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        return base.ShowDialog<bool>(owner);
    }
}
