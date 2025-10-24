using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AA16_UserControl2.Views;

public class TestControl : TemplatedControl
{
    public static readonly StyledProperty<object?> Text2Property =
        AvaloniaProperty.Register<TestControl, object?>(nameof(Text2));

    public object? Text2
    {
        get => GetValue(Text2Property);
        set => SetValue(Text2Property, value);
    }
}
