using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace AA98.MarkDown.Host.Views;

/// <summary>
/// Represents a modal dialog asking how to handle unsaved document changes.
/// </summary>
public sealed class UnsavedChangesDialog : Window
{
    private TaskCompletionSource<SaveDecision>? _decisionSource;
    private bool _decisionProvided;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsavedChangesDialog"/> class.
    /// </summary>
    /// <param name="documentTitle">The document title.</param>
    /// <param name="actionTitle">The action title shown in the message.</param>
    public UnsavedChangesDialog(string documentTitle, string actionTitle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentTitle);
        ArgumentException.ThrowIfNullOrWhiteSpace(actionTitle);

        Title = "Ungespeicherte Änderungen";
        Width = 520;
        Height = 180;
        MinWidth = 520;
        MinHeight = 180;
        MaxWidth = 700;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        TextBlock message = new()
        {
            Text = $"\"{documentTitle}\" enthält ungespeicherte Änderungen. Vor \"{actionTitle}\" speichern?",
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12),
        };

        Button saveButton = new()
        {
            Content = "Speichern",
            MinWidth = 100,
        };

        Button discardButton = new()
        {
            Content = "Nicht speichern",
            MinWidth = 120,
        };

        Button cancelButton = new()
        {
            Content = "Abbrechen",
            MinWidth = 100,
        };

        saveButton.Click += (_, _) => ResolveDecisionAndClose(SaveDecision.Save);
        discardButton.Click += (_, _) => ResolveDecisionAndClose(SaveDecision.Discard);
        cancelButton.Click += (_, _) => ResolveDecisionAndClose(SaveDecision.Cancel);

        Closed += OnDialogClosed;

        StackPanel buttonPanel = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children =
            {
                saveButton,
                discardButton,
                cancelButton,
            },
        };

        Content = new Border
        {
            Padding = new Thickness(14),
            Child = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    message,
                    buttonPanel,
                },
            },
        };
    }

    /// <summary>
    /// Shows the dialog and returns the selected save decision.
    /// </summary>
    /// <param name="owner">The owner window.</param>
    /// <returns>The chosen decision.</returns>
    public async Task<SaveDecision> ShowDialog(Window owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        _decisionSource = new TaskCompletionSource<SaveDecision>();
        _decisionProvided = false;

        _ = base.ShowDialog(owner);
        return await _decisionSource.Task;
    }

    private void ResolveDecisionAndClose(SaveDecision decision)
    {
        _decisionProvided = true;
        _decisionSource?.TrySetResult(decision);
        Close();
    }

    private void OnDialogClosed(object? sender, EventArgs e)
    {
        if (_decisionProvided)
        {
            return;
        }

        _decisionSource?.TrySetResult(SaveDecision.Cancel);
    }
}
