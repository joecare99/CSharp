using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MVVM_27_DataGrid.ValueConverter
{
    public class EmailValue : IValueConverter
    {
        const string Prefix = "mailto:";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string email && !string.IsNullOrEmpty(email)? $"{Prefix}{email}" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string email && email.ToLower().StartsWith(Prefix) ? email.Substring(Prefix.Length)  : value ?? String.Empty; 
        }
    }
}
