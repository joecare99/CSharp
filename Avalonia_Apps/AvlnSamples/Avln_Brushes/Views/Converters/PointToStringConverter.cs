// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Avln_Brushes.Views.Converters;

public class PointToStringConverter : IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Point p)
        {
            return p.X.ToString("F4") + "," + p.Y.ToString("F4");
 }
        return "0.0000,0.0000";
   }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
  try
    {
     if (value is string s)
            {
          return Point.Parse(s);
        }
        }
        catch (Exception)
        {
  // Ignore errors
 }
        return new Point(0, 0);
    }
}
