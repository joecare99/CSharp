// ***********************************************************************
// Assembly : AA18_MultiConverter
// Author : Mir
// Created :07-05-2022
//
// Last Modified By : Mir
// Last Modified On :07-05-2022
// ***********************************************************************
// <copyright file="TimeSpanConverter.cs" company="JC-Soft">
// Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Collections.Generic;
using AA18_MultiConverter.Model;

namespace AA18_MultiConverter.ValueConverter;

/// <summary>
/// Avalonia-compatible multi-binding converter implemented in XAML with Binding/Converters.
/// We will create an IMultiValueConverter equivalent using Avalonia.Data.Converters.
/// </summary>
public class TimeSpanConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var df = DateDifFormat.Days;
        if (values.Count >= 2 && values[1] is DateDifFormat d)
            df = d;
        if (values.Count >= 1 && values[0] is TimeSpan ts)
        {
            var fmt = parameter as string ?? "F2";
            return df switch
            {
                DateDifFormat.Days => ts.TotalDays.ToString(fmt, culture),
                DateDifFormat.Hours => ts.TotalHours.ToString(fmt, culture),
                DateDifFormat.Minutes => ts.TotalMinutes.ToString(fmt, culture),
                DateDifFormat.Seconds => ts.TotalSeconds.ToString(fmt, culture),
                _ => "0",
            };
        }
        return "0";
    }
}
