using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
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
    private readonly DispatcherTimer _hoverTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
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
            _editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            _editor.Text = Text ?? string.Empty;
            _editor.TextChanged += OnEditorTextChanged;
            _editor.KeyDown += OnEditorKeyDown;
            _editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            _editor.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, true);
            _editor.PointerExited += OnPointerExited;
            _editor.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel, true);
            _editor.TextArea.TextView.ScrollOffsetChanged += (_, _) => DismissHoverQuickInfo();
            _hoverTimer.Tick += OnHoverTimerTick;
            ToolTip.SetServiceEnabled(_editor, false);
            ToolTip.SetPlacement(_editor, PlacementMode.Pointer);
        }
    }

    public event EventHandler? CompletionRequested;

    public event EventHandler? GoToDefinitionRequested;

    public event EventHandler? QuickInfoRequested;

    public event EventHandler? SignatureHelpRequested;

    public event EventHandler? CaretPositionChanged;

    public event Action<int>? HoverQuickInfoRequested;

    public event EventHandler? HoverQuickInfoDismissed;

    public int CaretOffset => _editor?.CaretOffset ?? 0;

    public int? HoverOffset { get; private set; }

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
        _editor.Focus(Avalonia.Input.NavigationMethod.Unspecified);
    }

    public void SetCaretOffset(int offset)
    {
        if (_editor is null)
        {
            throw new InvalidOperationException("The Avalonia editor control is unavailable.");
        }

        _editor.CaretOffset = Math.Clamp(offset, 0, (_editor.Text ?? string.Empty).Length);
        _editor.Focus(Avalonia.Input.NavigationMethod.Unspecified);
    }

    public void MoveCaretWithoutFocus(int offset)
    {
        if (_editor is not null)
        {
            _editor.CaretOffset = Math.Clamp(offset, 0, (_editor.Text ?? string.Empty).Length);
        }
    }

    public void ApplyTextChange(int start, int length, string text, int caretOffset)
    {
        if (_editor?.Document is not { } document)
        {
            throw new InvalidOperationException("The Avalonia editor document is unavailable.");
        }

        document.Replace(start, length, text);
        _editor.CaretOffset = caretOffset;
        _editor.Focus(NavigationMethod.Unspecified);
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
            DismissHoverQuickInfo();
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

        DismissHoverQuickInfo();
        _isUpdatingFromEditor = true;
        Text = _editor.Text;
        _isUpdatingFromEditor = false;
        CaretPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnCaretPositionChanged(object? sender, EventArgs e)
    {
        if (!_isUpdatingFromProperty)
        {
            DismissHoverQuickInfo();
            CaretPositionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ShowHoverQuickInfo(string text)
    {
        if (_editor is null || HoverOffset is null || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        ToolTip.SetTip(_editor, new TextBlock { Text = text, TextWrapping = Avalonia.Media.TextWrapping.Wrap, MaxWidth = 500 });
        ToolTip.SetIsOpen(_editor, true);
    }

    public void DismissHoverQuickInfo()
    {
        _hoverTimer.Stop();
        if (_editor is not null)
        {
            ToolTip.SetIsOpen(_editor, false);
            ToolTip.SetTip(_editor, null);
        }
        if (HoverOffset is not null)
        {
            HoverOffset = null;
            HoverQuickInfoDismissed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        DismissHoverQuickInfo();
        if (_editor?.Document is not { } document || e.Pointer.Type != PointerType.Mouse)
        {
            return;
        }

        var view = _editor.TextArea.TextView;
        Point point = e.GetPosition(view);
        if (!new Rect(view.Bounds.Size).Contains(point))
        {
            return;
        }
        TextViewPosition? position = view.GetPositionFloor(point + view.ScrollOffset);
        if (position is null || position.Value.Line < 1 || position.Value.Line > document.LineCount)
        {
            return;
        }
        var line = document.GetLineByNumber(position.Value.Line);
        int column = position.Value.Column;
        if (column < 1 || column > line.Length)
        {
            return;
        }
        int offset = line.Offset + column - 1;
        if (char.IsWhiteSpace(document.GetCharAt(offset)))
        {
            return;
        }

        HoverOffset = offset;
        _hoverTimer.Start();
    }

    private void OnHoverTimerTick(object? sender, EventArgs e)
    {
        _hoverTimer.Stop();
        if (HoverOffset is int offset)
        {
            HoverQuickInfoRequested?.Invoke(offset);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e) => DismissHoverQuickInfo();

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e) => DismissHoverQuickInfo();

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DismissHoverQuickInfo();
        base.OnDetachedFromVisualTree(e);
    }

    private void OnEditorKeyDown(object? sender, KeyEventArgs e)
    {
        DismissHoverQuickInfo();
        if (e.Key == Key.Space && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
        {
            e.Handled = true;
            SignatureHelpRequested?.Invoke(this, EventArgs.Empty);
        }
        else if (e.Key == Key.Space && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;
            CompletionRequested?.Invoke(this, EventArgs.Empty);
        }
        else if (e.Key == Key.F1 && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
        {
            e.Handled = true;
            QuickInfoRequested?.Invoke(this, EventArgs.Empty);
        }
        else if (e.Key == Key.F12)
        {
            e.Handled = true;
            GoToDefinitionRequested?.Invoke(this, EventArgs.Empty);
        }
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
