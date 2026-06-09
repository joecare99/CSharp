using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace AA28_DataGridExt.ValueConverter;

/// <summary>
/// Converts plain email addresses to and from a mailto URI string.
/// </summary>
public class EmailValue : IValueConverter
{
    private const string MailToPrefix = "mailto:";

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string email && !string.IsNullOrWhiteSpace(email)
            ? $"{MailToPrefix}{email}"
            : string.Empty;

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string email && email.StartsWith(MailToPrefix, StringComparison.OrdinalIgnoreCase)
            ? email[MailToPrefix.Length..]
            : value ?? string.Empty;
}
