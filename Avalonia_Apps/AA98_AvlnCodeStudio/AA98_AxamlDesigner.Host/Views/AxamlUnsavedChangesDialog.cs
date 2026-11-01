using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;
using System.Threading.Tasks;

namespace AA98_AxamlDesigner.Host.Views;

public sealed class AxamlUnsavedChangesDialog : Window
{
    private TaskCompletionSource<AxamlSaveDecision>? _decisionSource;
    private bool _decisionProvided;

    public AxamlUnsavedChangesDialog(string fileName, string actionTitle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(actionTitle);

        Title = "Unsaved AXAML changes";
        Width = 520;
        Height = 180;
        MinWidth = 520;
        MinHeight = 180;
        MaxWidth = 700;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        TextBlock message = new()
        {
            Text = $"\"{fileName}\" has unsaved changes. Save before {actionTitle}?",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12),
        };
        Button saveButton = new() { Content = "Save", MinWidth = 100 };
        Button discardButton = new() { Content = "Discard", MinWidth = 100 };
        Button cancelButton = new() { Content = "Cancel", MinWidth = 100 };
        saveButton.Click += (_, _) => ResolveDecisionAndClose(AxamlSaveDecision.Save);
        discardButton.Click += (_, _) => ResolveDecisionAndClose(AxamlSaveDecision.Discard);
        cancelButton.Click += (_, _) => ResolveDecisionAndClose(AxamlSaveDecision.Cancel);
        Closed += OnDialogClosed;

        Content = new Border
        {
            Padding = new Thickness(14),
            Child = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    message,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { saveButton, discardButton, cancelButton },
                    },
                },
            },
        };
    }

    public async Task<AxamlSaveDecision> ShowAndGetDecisionAsync(Window owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _decisionSource = new TaskCompletionSource<AxamlSaveDecision>();
        _decisionProvided = false;
        _ = base.ShowDialog(owner);
        return await _decisionSource.Task;
    }

    private void ResolveDecisionAndClose(AxamlSaveDecision decision)
    {
        _decisionProvided = true;
        _decisionSource?.TrySetResult(decision);
        Close();
    }

    private void OnDialogClosed(object? sender, EventArgs e)
    {
        if (!_decisionProvided)
        {
            _decisionSource?.TrySetResult(AxamlSaveDecision.Cancel);
        }
    }
}

public enum AxamlSaveDecision
{
    Save,
    Discard,
    Cancel,
}
