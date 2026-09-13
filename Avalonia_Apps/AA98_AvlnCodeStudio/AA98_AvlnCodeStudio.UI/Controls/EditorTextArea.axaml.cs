using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using Code.Navigation;
using System;

namespace AA98_AvlnCodeStudio.UI.Controls;

/// <summary>
/// Hosts the AvaloniaEdit control and exposes bindable text content.
/// </summary>
public partial class EditorTextArea : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<EditorTextArea, string?>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    private readonly TextEditor? _editor;
    private bool _isUpdatingFromEditor;
    private bool _isUpdatingFromProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorTextArea"/> class.
    /// </summary>
    public EditorTextArea()
    {
        InitializeComponent();
        _editor = this.FindControl<TextEditor>("PART_Editor");
        if (_editor is not null)
        {
            _editor.Text = Text ?? string.Empty;
            _editor.TextChanged += OnEditorTextChanged;
        }
    }

    /// <summary>
    /// Gets or sets the editor text.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Focuses the editor and moves the caret to the supplied neutral source
    /// location. Coordinates outside the loaded document are clamped.
    /// </summary>
    /// <param name="location">The source location to activate.</param>
    public void FocusAndMoveCaret(CodeLocation location)
    {
        ArgumentNullException.ThrowIfNull(location);

        if (_editor is null)
        {
            throw new InvalidOperationException("The Avalonia editor control is unavailable.");
        }

        var text = _editor.Text ?? string.Empty;
        var requestedOffset = location.Offset ?? GetOffset(text, location.Line, location.Column);
        _editor.CaretOffset = Math.Clamp(requestedOffset, 0, text.Length);
        _editor.Focus();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty && _editor is not null && !_isUpdatingFromEditor)
        {
            var newText = change.NewValue as string ?? string.Empty;
            if (string.Equals(_editor.Text, newText, StringComparison.Ordinal))
            {
                return;
            }

            _isUpdatingFromProperty = true;
            _editor.Text = newText;
            _isUpdatingFromProperty = false;
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
        _isUpdatingFromEditor = false;
    }

    private static int GetOffset(string text, int? line, int? column)
    {
        if (line is null)
        {
            return 0;
        }

        var currentLine = 1;
        var offset = 0;
        while (offset < text.Length && currentLine < line.Value)
        {
            if (text[offset++] == '\n')
            {
                currentLine++;
            }
        }

        if (currentLine < line.Value)
        {
            return text.Length;
        }

        var requestedColumn = column ?? 1;
        var lineEnd = text.IndexOf('\n', offset);
        if (lineEnd < 0)
        {
            lineEnd = text.Length;
        }

        return Math.Min(offset + requestedColumn - 1, lineEnd);
    }
}
