using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace VTileEdit.WPF.Converters
{
    /// <summary>
    /// Converts a width value into a height value by multiplying with the configured ratio to keep cells proportional.
    /// </summary>
    public sealed class WidthToHeightConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Gets or sets the multiplier applied to the source width. Defaults to 2 to achieve a 1:2 width-to-height ratio.
        /// </summary>
        public double Ratio { get; set; } = 2d;

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && !double.IsNaN(width))
            {
                return width * Ratio;
            }

            return DependencyProperty.UnsetValue;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
