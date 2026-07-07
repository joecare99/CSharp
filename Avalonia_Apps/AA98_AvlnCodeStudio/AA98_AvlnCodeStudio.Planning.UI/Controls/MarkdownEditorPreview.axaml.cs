using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using System;

namespace AA98_AvlnCodeStudio.Planning.UI.Controls;

/// <summary>
/// Provides a compact markdown editor and synchronized preview surface.
/// </summary>
public partial class MarkdownEditorPreview : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<MarkdownEditorPreview, string?>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<MarkdownEditorPreview, bool>(nameof(IsReadOnly));

    private readonly TextEditor? _editor;
    private readonly TextEditor? _preview;
    private bool _isUpdatingFromEditor;
    private bool _isUpdatingFromProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownEditorPreview"/> class.
    /// </summary>
    public MarkdownEditorPreview()
    {
        InitializeComponent();

        _editor = this.FindControl<TextEditor>("PART_Editor");
        _preview = this.FindControl<TextEditor>("PART_Preview");

        if (_editor is not null)
        {
            _editor.Text = Text ?? string.Empty;
            _editor.IsReadOnly = IsReadOnly;
            _editor.TextChanged += OnEditorTextChanged;
            _editor.SyntaxHighlighting = ResolveMarkdownHighlighting();
        }

        if (_preview is not null)
        {
            _preview.Text = Text ?? string.Empty;
            _preview.SyntaxHighlighting = ResolveMarkdownHighlighting();
        }
    }

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

            if (_preview is not null && !string.Equals(_preview.Text, newText, StringComparison.Ordinal))
            {
                _preview.Text = newText;
            }

            _isUpdatingFromProperty = false;
        }

        if (change.Property == IsReadOnlyProperty && _editor is not null)
        {
            _editor.IsReadOnly = change.NewValue is true;
        }
    }

    private static IHighlightingDefinition? ResolveMarkdownHighlighting()
    {
        return HighlightingManager.Instance.GetDefinitionByExtension(".md")
            ?? HighlightingManager.Instance.GetDefinition("Markdown")
            ?? HighlightingManager.Instance.GetDefinition("MarkDown");
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

        if (_preview is not null && !string.Equals(_preview.Text, _editor.Text, StringComparison.Ordinal))
        {
            _preview.Text = _editor.Text;
        }

        _isUpdatingFromEditor = false;
    }
}
