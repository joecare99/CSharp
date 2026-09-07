using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;

namespace ConsoleLib.Cxaml.Designer.Controls;

public sealed partial class SourceEditor : UserControl
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SourceEditor, string?>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<int> CaretOffsetProperty =
        AvaloniaProperty.Register<SourceEditor, int>(nameof(CaretOffset), defaultValue: 0);

    private readonly TextEditor _editor;
    private bool _updating;

    public SourceEditor()
    {
        InitializeComponent();
        _editor = this.FindControl<TextEditor>("PART_Editor")
            ?? throw new InvalidOperationException("The source editor control is missing.");
        _editor.TextChanged += EditorOnTextChanged;
        _editor.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public int CaretOffset
    {
        get => GetValue(CaretOffsetProperty);
        set => SetValue(CaretOffsetProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty && !_updating)
            _editor.Text = change.NewValue as string ?? string.Empty;
        if (change.Property == CaretOffsetProperty && !_updating)
            _editor.CaretOffset = Math.Clamp((int)change.NewValue!, 0, _editor.Text?.Length ?? 0);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void EditorOnTextChanged(object? sender, EventArgs e)
    {
        if (_updating)
            return;
        _updating = true;
        Text = _editor.Text;
        _updating = false;
    }

    private void CaretOnPositionChanged(object? sender, EventArgs e)
    {
        if (_updating)
            return;
        _updating = true;
        CaretOffset = _editor.CaretOffset;
        _updating = false;
    }
}
