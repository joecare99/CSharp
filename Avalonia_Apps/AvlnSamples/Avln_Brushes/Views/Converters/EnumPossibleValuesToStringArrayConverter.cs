// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Avln_Brushes.Views.Converters;

public class EnumPossibleValuesToStringArrayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;
        return new ArrayList(Enum.GetNames(value.GetType()));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
     return null;
    }
}
