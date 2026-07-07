using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using Markdown.Avalonia;
using System;
using System.Text.RegularExpressions;

namespace AA98.MarkDown.UI.Controls;

/// <summary>
/// Provides a compact markdown editor and synchronized preview surface.
/// </summary>
public partial class MarkdownEditorPreview : UserControl
{
    private static readonly Regex Heading1Regex = new("^(\\s{0,3}#+)(?!#)\\s+", RegexOptions.Multiline | RegexOptions.Compiled);

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<MarkdownEditorPreview, string?>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<MarkdownEditorPreview, bool>(nameof(IsReadOnly));

    public static readonly StyledProperty<bool> IsPreviewEnabledProperty =
        AvaloniaProperty.Register<MarkdownEditorPreview, bool>(nameof(IsPreviewEnabled), true);

    private readonly Grid? _layoutGrid;
    private readonly TextEditor? _editor;
    private readonly MarkdownScrollViewer? _preview;
    private readonly GridSplitter? _columnSplitter;
    private readonly Border? _previewBorder;
    private readonly TextBlock? _previewHeader;
    private bool _isUpdatingFromEditor;
    private bool _isUpdatingFromProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownEditorPreview"/> class.
    /// </summary>
    public MarkdownEditorPreview()
    {
        InitializeComponent();

        _layoutGrid = this.FindControl<Grid>("PART_LayoutGrid");
        _editor = this.FindControl<TextEditor>("PART_Editor");
        _preview = this.FindControl<MarkdownScrollViewer>("PART_Preview");
        _columnSplitter = this.FindControl<GridSplitter>("PART_ColumnSplitter");
        _previewBorder = this.FindControl<Border>("PART_PreviewBorder");
        _previewHeader = this.FindControl<TextBlock>("PART_PreviewHeader");

        if (_editor is not null)
        {
            _editor.Text = Text ?? string.Empty;
            _editor.IsReadOnly = IsReadOnly;
            _editor.SyntaxHighlighting = ResolveMarkdownHighlighting();
            _editor.TextChanged += OnEditorTextChanged;
        }

        if (_preview is not null)
        {
            _preview.Markdown = GetPreviewMarkdown(Text);
            _preview.AddHandler(Button.ClickEvent, OnPreviewButtonClick, RoutingStrategies.Bubble);
        }

        ApplyPreviewLayout(IsPreviewEnabled);
    }

    /// <summary>
    /// Raised when a markdown file link is activated on the preview surface.
    /// </summary>
    public event EventHandler<MarkdownLinkInvokedEventArgs>? LinkInvoked;

    /// <summary>
    /// Gets or sets the markdown text.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the editor area is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the preview pane is visible.
    /// </summary>
    public bool IsPreviewEnabled
    {
        get => GetValue(IsPreviewEnabledProperty);
        set => SetValue(IsPreviewEnabledProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty && !_isUpdatingFromEditor)
        {
            string newText = change.NewValue as string ?? string.Empty;

            _isUpdatingFromProperty = true;
            if (_editor is not null && !string.Equals(_editor.Text, newText, StringComparison.Ordinal))
            {
                _editor.Text = newText;
            }

            string previewMarkdown = GetPreviewMarkdown(newText);
            if (_preview is not null && !string.Equals(_preview.Markdown, previewMarkdown, StringComparison.Ordinal))
            {
                _preview.Markdown = previewMarkdown;
            }

            _isUpdatingFromProperty = false;
        }

        if (change.Property == IsReadOnlyProperty && _editor is not null)
        {
            _editor.IsReadOnly = change.NewValue is true;
        }

        if (change.Property == IsPreviewEnabledProperty)
        {
            ApplyPreviewLayout(change.NewValue is true);
        }
    }

    private static IHighlightingDefinition? ResolveMarkdownHighlighting()
    {
        return HighlightingManager.Instance.GetDefinitionByExtension(".md")
            ?? HighlightingManager.Instance.GetDefinition("Markdown")
            ?? HighlightingManager.Instance.GetDefinition("MarkDown");
    }

    private void ApplyPreviewLayout(bool isPreviewEnabled)
    {
        if (_layoutGrid is null)
        {
            return;
        }

        _layoutGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        _layoutGrid.ColumnDefinitions[1].Width = isPreviewEnabled ? new GridLength(6) : new GridLength(0);
        _layoutGrid.ColumnDefinitions[2].Width = isPreviewEnabled ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

        if (_columnSplitter is not null)
        {
            _columnSplitter.IsVisible = isPreviewEnabled;
        }

        if (_previewBorder is not null)
        {
            _previewBorder.IsVisible = isPreviewEnabled;
        }

        if (_previewHeader is not null)
        {
            _previewHeader.IsVisible = isPreviewEnabled;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnEditorTextChanged(object? sender, EventArgs e)
    {
        if (_editor is null || _isUpdatingFromProperty)
        {
            return;
        }

        _isUpdatingFromEditor = true;
        Text = _editor.Text;

        string previewMarkdown = GetPreviewMarkdown(_editor.Text);
        if (_preview is not null && !string.Equals(_preview.Markdown, previewMarkdown, StringComparison.Ordinal))
        {
            _preview.Markdown = previewMarkdown;
        }

        _isUpdatingFromEditor = false;
    }

    private static string GetPreviewMarkdown(string? markdown)
    {
        string source = markdown ?? string.Empty;
        return Heading1Regex.Replace(source, "$1# ");
    }

    private void OnPreviewButtonClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button button)
        {
            return;
        }

        string? linkTarget = button.CommandParameter switch
        {
            null => null,
            Uri uri => uri.OriginalString,
            _ => button.CommandParameter?.ToString(),
        };

        if (string.IsNullOrWhiteSpace(linkTarget))
        {
            linkTarget = button.Tag?.ToString();
        }

        if (string.IsNullOrWhiteSpace(linkTarget))
        {
            return;
        }

        LinkInvoked?.Invoke(this, new MarkdownLinkInvokedEventArgs(linkTarget));
    }
}
