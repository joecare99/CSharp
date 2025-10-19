using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CommunityToolkit.Mvvm.Input;

namespace AA16_UserControl1.Views;

public partial class LabeldMaxLengthTextbox : UserControl
{
    public static readonly StyledProperty<string?> CaptionProperty =
        AvaloniaProperty.Register<LabeldMaxLengthTextbox, string?>(nameof(Caption));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<LabeldMaxLengthTextbox, string?>(
            nameof(Text),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IRelayCommand?> CommandProperty =
        AvaloniaProperty.Register<LabeldMaxLengthTextbox, IRelayCommand?>(nameof(Command));

    public LabeldMaxLengthTextbox()
    {
        InitializeComponent();
    }

    public string? Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IRelayCommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public int MaxLength { get; set; } = 50;
}
