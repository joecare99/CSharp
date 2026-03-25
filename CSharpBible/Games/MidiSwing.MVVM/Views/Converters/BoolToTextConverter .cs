using System.Globalization;
using System.Windows.Data;

namespace MidiSwing.MVVM.Views.Converters;

public class BoolToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isPlaying)
        {
            return isPlaying ? "Spielt ab..." : "Melodie abspielen";
        }
        return "Melodie abspielen";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
