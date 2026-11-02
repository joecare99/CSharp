using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;

/// <summary>
/// Shows a multi-file proposal and applies it only after the user chooses the explicit apply action.
/// </summary>
public sealed class WorkspaceFileChangeReviewDialog : Window
{
    private readonly WorkspaceFileChangeProposal _proposal;
    private readonly WorkspaceFileChangeService _service;
    private readonly TextBox _originalText;
    private readonly TextBox _proposedText;
    private readonly TextBlock _statusText;

    public WorkspaceFileChangeReviewDialog(
        WorkspaceFileChangeProposal proposal,
        WorkspaceFileChangeService service)
    {
        _proposal = proposal ?? throw new ArgumentNullException(nameof(proposal));
        _service = service ?? throw new ArgumentNullException(nameof(service));

        Title = $"Review workspace changes ({proposal.Changes.Count})";
        Width = 1080;
        Height = 720;
        MinWidth = 760;
        MinHeight = 480;
        CanResize = true;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        ListBox changeList = new()
        {
            ItemsSource = proposal.Changes
                .Select(change => $"{(change.IsNewFile ? "Add" : "Update")}  {change.RelativePath}")
                .ToArray(),
            MinWidth = 230,
        };
        _originalText = CreateReadOnlyEditor();
        _proposedText = CreateReadOnlyEditor();
        _statusText = new TextBlock
        {
            Text = $"Workspace: {proposal.WorkspaceRoot}",
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center,
        };

        changeList.SelectionChanged += (_, _) => ShowSelectedChange(changeList.SelectedIndex);
        Button cancelButton = new()
        {
            Content = "Cancel",
            MinWidth = 100,
        };
        Button applyButton = new()
        {
            Content = $"Apply {proposal.Changes.Count} change(s)",
            MinWidth = 150,
        };
        cancelButton.Click += (_, _) => Close(false);
        applyButton.Click += (_, _) => ApplyChanges();

        Grid comparison = new()
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 8,
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 6,
        };
        comparison.Children.Add(new TextBlock { Text = "Current content", FontWeight = FontWeight.SemiBold });
        comparison.Children.Add(new TextBlock { Text = "Proposed content", FontWeight = FontWeight.SemiBold });
        Grid.SetColumn(comparison.Children[1], 1);
        comparison.Children.Add(_originalText);
        Grid.SetRow(_originalText, 1);
        comparison.Children.Add(_proposedText);
        Grid.SetRow(_proposedText, 1);
        Grid.SetColumn(_proposedText, 1);

        Grid body = new()
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            ColumnSpacing = 10,
        };
        body.Children.Add(changeList);
        body.Children.Add(comparison);
        Grid.SetColumn(comparison, 1);

        StackPanel buttons = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children = { cancelButton, applyButton },
        };
        Grid content = new()
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            RowSpacing = 10,
            Children =
            {
                new TextBlock
                {
                    Text = "Review every file below. Applying checks that none changed since this proposal was prepared.",
                    TextWrapping = TextWrapping.Wrap,
                },
                body,
                new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("*,Auto"),
                    ColumnSpacing = 12,
                    Children = { _statusText, buttons },
                },
            },
        };
        Grid.SetRow(body, 1);
        Grid.SetRow(content.Children[2], 2);
        Grid.SetColumn(buttons, 1);
        Content = new Border
        {
            Padding = new Thickness(14),
            Child = content,
        };
        changeList.SelectedIndex = 0;
    }

    public new Task<bool> ShowDialog(Window owner)
    {
        ArgumentNullException.ThrowIfNull(owner);
        return base.ShowDialog<bool>(owner);
    }

    private static TextBox CreateReadOnlyEditor()
    {
        return new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.NoWrap,
            FontFamily = new FontFamily("monospace"),
        };
    }

    private void ShowSelectedChange(int index)
    {
        if (index < 0 || index >= _proposal.Changes.Count)
        {
            _originalText.Text = string.Empty;
            _proposedText.Text = string.Empty;
            return;
        }

        WorkspaceFileChange change = _proposal.Changes[index];
        _originalText.Text = change.OriginalContent ?? "(new file)";
        _proposedText.Text = change.ProposedContent;
        _statusText.Text = $"{change.RelativePath}  —  {(_proposal.Changes[index].IsNewFile ? "new file" : "replacement")}";
    }

    private void ApplyChanges()
    {
        try
        {
            _service.Apply(_proposal, confirmed: true);
            Close(true);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or InvalidOperationException
            or ArgumentException
            or AggregateException)
        {
            _statusText.Text = $"Changes were not fully applied: {exception.Message}";
        }
    }
}
