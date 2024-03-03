using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using MVVM_AllExamples.Models;

namespace MVVM_AllExamples.ValueConverters;

public class ListItemToContentConverter : IValueConverter
{

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        if (value is ExItem lbi)
        {
            var v = Activator.CreateInstance(lbi.ExType);
            if (v is Page fe)
            {
                return new Frame() { Content = fe };
            }
            else
            return v;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
