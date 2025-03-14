﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace MVVM_24c_CTUserControl.Views.Converters;

public class BoolToBrushConverter : IValueConverter
{
		public	Brush TrueBrush { get; set; } = Brushes.GreenYellow;
		public	Brush FalseBrush { get; set; } = Brushes.Maroon;
		public	Brush DefaultBrush { get; set; } = Brushes.DarkGray;
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			 return (value, parameter) switch
			{
				(bool b, Brush brush) when b => brush,
				(bool b, _) => b? TrueBrush:FalseBrush,
				_ => DefaultBrush
			};
		}

		//  Visible/Collapsed --> bool
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
