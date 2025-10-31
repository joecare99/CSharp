// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avln_Brushes.Views.Converters;

public class DoubleToStringConverter : IValueConverter
{
   public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
 if (value is double d)
       {
    return d.ToString("F4");
        }
        return "0.0000";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
     {
 if (value is string s && double.TryParse(s, out var result))
          {
      return result;
          }
   }
  catch (Exception)
  {
       // Ignore errors
  }
        return 0.0;
    }
}
