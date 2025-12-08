using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Media;

namespace TextTestApp
{
    public class FontFeatureCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            // Annahme: Kommagetrennte Liste von FontFeature-Strings
            var str = (string)value;
            var features = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var collection = new FontFeatureCollection();
            foreach (var feature in features)
            {
                collection.Add(FontFeature.Parse(feature.Trim()));
            }
            return collection;
        }
    }
}
