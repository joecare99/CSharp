using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;

namespace AA16_UserControl1.Views;

public partial class DoubleButtonUC : UserControl
{
    public DoubleButtonUC()
    {
        InitializeComponent();
        VisData1 = true;
        VisData2 = true;
    }

    public static readonly StyledProperty<ICommand?> Command1Property =
        AvaloniaProperty.Register<DoubleButtonUC, ICommand?>(nameof(Command1));

    public static readonly StyledProperty<ICommand?> Command2Property =
        AvaloniaProperty.Register<DoubleButtonUC, ICommand?>(nameof(Command2));

    public ICommand? Command1
    {
        get => GetValue(Command1Property);
        set => SetValue(Command1Property, value);
    }

    public ICommand? Command2
    {
        get => GetValue(Command2Property);
        set => SetValue(Command2Property, value);
    }

    public bool VisData1 { get; set; }
    public bool VisData2 { get; set; }

    public string? CommandParameter1 { get; set; }
    public string? CommandParameter2 { get; set; }

    public string? Tooltip1 { get; set; }
    public string? Tooltip2 { get; set; }

    public string? Image1 { get; set; }
    public string? Image2 { get; set; }
}
