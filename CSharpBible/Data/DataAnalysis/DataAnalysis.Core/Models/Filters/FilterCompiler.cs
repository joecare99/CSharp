using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataAnalysis.Core.Models;

public static class FilterCompiler
{
    public static Func<SyslogEntry, bool>? Compile(FilterDefinition? def)
    {
        if (def is null)
            return null;
        return def switch
        {
            ValueFilterDefinition v => CompileValue(v),
            GroupFilterDefinition g => CompileGroup(g),
            _ => null
        };
    }

    private static Func<SyslogEntry, bool>? CompileGroup(GroupFilterDefinition g)
    {
        if (g.Filters is null || g.Filters.Count == 0)
        {
            return g.Negate ? _ => false : _ => true;
        }
        var compiled = g.Filters.Select(Compile).Where(p => p is not null).Cast<Func<SyslogEntry, bool>>().ToArray();
        if (compiled.Length == 0)
            return g.Negate ? _ => true : _ => false;

        Func<SyslogEntry, bool> core = g.Mode.Equals("or", StringComparison.OrdinalIgnoreCase)
        ? e =>
        {
            for (int i = 0; i < compiled.Length; i++)
                if (compiled[i](e))
                    return true;
            return false;
        }
        : e =>
        {
            for (int i = 0; i < compiled.Length; i++)
                if (!compiled[i](e))
                    return false;
            return true;
        };

        if (g.Negate)
            return e => !core(e);
        return core;
    }

    private static Func<SyslogEntry, bool>? CompileValue(ValueFilterDefinition v)
    {
        var cmp = v.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        var culture = string.IsNullOrWhiteSpace(v.Culture) ? CultureInfo.InvariantCulture : new CultureInfo(v.Culture);
        var formats = v.Formats ?? Array.Empty<string>();

        object? parsedFrom = null;
        object? parsedTo = null;

        switch (v.DataType)
        {
            case FilterDataType.Number:
                if (v.Value is not null && double.TryParse(v.Value, NumberStyles.Float, culture, out var dn))
                    parsedFrom = dn;
                if (v.ValueTo is not null && double.TryParse(v.ValueTo, NumberStyles.Float, culture, out var dn2))
                    parsedTo = dn2;
                break;
            case FilterDataType.DateTime:
                if (v.Value is not null)
                {
                    if (formats.Length > 0)
                    {
                        if (DateTimeOffset.TryParseExact(v.Value, formats, culture, DateTimeStyles.None, out var dto))
                            parsedFrom = dto;
                    }
                    else if (DateTimeOffset.TryParse(v.Value, culture, DateTimeStyles.None, out var dto2))
                        parsedFrom = dto2;
                }
                if (v.ValueTo is not null)
                {
                    if (formats.Length > 0)
                    {
                        if (DateTimeOffset.TryParseExact(v.ValueTo, formats, culture, DateTimeStyles.None, out var dto))
                            parsedTo = dto;
                    }
                    else if (DateTimeOffset.TryParse(v.ValueTo, culture, DateTimeStyles.None, out var dto2))
                        parsedTo = dto2;
                }
                break;
            case FilterDataType.Enum:
                if (!string.IsNullOrWhiteSpace(v.EnumType))
                {
                    var t = Type.GetType(v.EnumType!, throwOnError: false, ignoreCase: true);
                    if (t is not null && t.IsEnum)
                    {
                        if (v.Value is not null && Enum.TryParse(t, v.Value, ignoreCase: !v.CaseSensitive, out var ev))
                            parsedFrom = ev;
                        if (v.ValueTo is not null && Enum.TryParse(t, v.ValueTo, ignoreCase: !v.CaseSensitive, out var ev2))
                            parsedTo = ev2;
                    }
                }
                else
                {
                    // fallback: Severity Enum
                    if (v.Value is not null && Enum.TryParse(typeof(SyslogSeverity), v.Value, !v.CaseSensitive, out var sev))
                        parsedFrom = sev;
                    if (v.ValueTo is not null && Enum.TryParse(typeof(SyslogSeverity), v.ValueTo, !v.CaseSensitive, out var sev2))
                        parsedTo = sev2;
                }
                break;
            case FilterDataType.String:
            default:
                // nothing to pre-parse
                break;
        }

        return e => 
        {
            if (!TryGetFieldText(e, v.Field, out var text, out var typed))
                return false ;

            switch (v.DataType)
            {
                case FilterDataType.Number:
                    if (!TryAsDouble(text, typed, culture, out var d))
                        return false;
                    return v.Negate ^ CompareNumbers(d, v.Operator, parsedFrom as double?, parsedTo as double?);
                case FilterDataType.DateTime:
                    if (!TryAsDateTimeOffset(text, typed, culture, formats, out var dt))
                        return false;
                    return v.Negate ^ CompareDateTimes(dt, v.Operator, parsedFrom as DateTimeOffset?, parsedTo as DateTimeOffset?);
                case FilterDataType.Enum:
                    if (!TryAsEnum(text, typed, parsedFrom, out var eobj))
                        return false;
                    return v.Negate ^ CompareEnums(eobj!, v.Operator, parsedFrom, parsedTo);
                case FilterDataType.String:
                default:
                    return v.Negate ^ CompareStrings(text ?? string.Empty, v.Operator, v.Value, v.ValueTo, cmp);
            }
        };
    }

    private static bool TryGetFieldText(SyslogEntry e, string field, out string? text, out object? typed)
    {
        text = null;
        typed = null;
        if (string.Equals(field, "Timestamp", StringComparison.OrdinalIgnoreCase))
        {
            if (e.Timestamp is DateTimeOffset dto)
            { typed = dto; text = dto.ToString("o"); return true; }
            return false;
        }
        if (string.Equals(field, "Severity", StringComparison.OrdinalIgnoreCase))
        {
            typed = e.Severity;
            text = e.Severity.ToString();
            return true;
        }
        if (string.Equals(field, "Source", StringComparison.OrdinalIgnoreCase))
        {
            typed = text = e.Source ?? string.Empty;
            return true;
        }
        if (string.Equals(field, "Message", StringComparison.OrdinalIgnoreCase))
        {
            typed = text = e.Message ?? string.Empty;
            return true;
        }
        // Attributes (case-insensitive key match)
        if (e.Attributes is { Count: > 0 })
        {
            foreach (var kv in e.Attributes)
            {
                if (string.Equals(kv.Key, field, StringComparison.OrdinalIgnoreCase))
                { typed = text = kv.Value ?? string.Empty; return true; }
            }
        }
        return false;
    }

    private static bool TryAsDouble(string? text, object? typed, CultureInfo culture, out double d)
    {
        if (typed is double dd)
        { d = dd; return true; }
        if (double.TryParse(text, NumberStyles.Float, culture, out d))
            return true;
        return false;
    }

    private static bool TryAsDateTimeOffset(string? text, object? typed, CultureInfo culture, string[] formats, out DateTimeOffset value)
    {
        if (typed is DateTimeOffset dto)
        { value = dto; return true; }
        if (formats.Length > 0 && DateTimeOffset.TryParseExact(text, formats, culture, DateTimeStyles.None, out value))
            return true;
        if (DateTimeOffset.TryParse(text, culture, DateTimeStyles.None, out value))
            return true;
        value = default;
        return false;
    }

    private static bool TryAsEnum(string? text, object? typed, object? parsedFrom, out object? value)
    {
        value = null;
        if (typed is Enum e)
        { value = e; return true; }
        if (parsedFrom is not null && parsedFrom.GetType().IsEnum)
        {
            if (Enum.TryParse(parsedFrom.GetType(), text, true, out var ev))
            { value = ev; return true; }
        }
        else
        {
            if (Enum.TryParse(typeof(SyslogSeverity), text, true, out var sev))
            { value = sev; return true; }
        }
        return false;
    }

    private static bool CompareStrings(string text, FilterOperator op, string? value, string? valueTo, StringComparison cmp)
    {
        return op switch
        {
            FilterOperator.Eq => string.Equals(text, value ?? string.Empty, cmp),
            FilterOperator.Ne => !string.Equals(text, value ?? string.Empty, cmp),
            FilterOperator.Contains => value is not null && text.IndexOf(value, cmp) >= 0,
            FilterOperator.StartsWith => value is not null && text.StartsWith(value, cmp),
            FilterOperator.EndsWith => value is not null && text.EndsWith(value, cmp),
            FilterOperator.In => value is not null && value.Split('\u001F', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(v => string.Equals(text, v, cmp)),
            FilterOperator.Between => value is not null && valueTo is not null && string.Compare(text, value, cmp) >= 0 && string.Compare(text, valueTo, cmp) <= 0,
            FilterOperator.Lt => value is not null && string.Compare(text, value, cmp) < 0,
            FilterOperator.Le => value is not null && string.Compare(text, value, cmp) <= 0,
            FilterOperator.Gt => value is not null && string.Compare(text, value, cmp) > 0,
            FilterOperator.Ge => value is not null && string.Compare(text, value, cmp) >= 0,
            _ => false
        };
    }

    private static bool CompareNumbers(double actual, FilterOperator op, double? from, double? to)
    {
        return op switch
        {
            FilterOperator.Eq => from is not null && actual == from,
            FilterOperator.Ne => from is not null && actual != from,
            FilterOperator.Lt => from is not null && actual < from,
            FilterOperator.Le => from is not null && actual <= from,
            FilterOperator.Gt => from is not null && actual > from,
            FilterOperator.Ge => from is not null && actual >= from,
            FilterOperator.Between => from is not null && to is not null && actual >= from && actual <= to,
            _ => false
        };
    }

    private static bool CompareDateTimes(DateTimeOffset actual, FilterOperator op, DateTimeOffset? from, DateTimeOffset? to)
    {
        return op switch
        {
            FilterOperator.Eq => from is not null && actual == from,
            FilterOperator.Ne => from is not null && actual != from,
            FilterOperator.Lt => from is not null && actual < from,
            FilterOperator.Le => from is not null && actual <= from,
            FilterOperator.Gt => from is not null && actual > from,
            FilterOperator.Ge => from is not null && actual >= from,
            FilterOperator.Between => from is not null && to is not null && actual >= from && actual <= to,
            _ => false
        };
    }

    private static bool CompareEnums(object actual, FilterOperator op, object? from, object? to)
    {
        int cmp(object a, object b) => Comparer<int>.Default.Compare(Convert.ToInt32(a), Convert.ToInt32(b));
        return op switch
        {
            FilterOperator.Eq => from is not null && Equals(actual, from),
            FilterOperator.Ne => from is not null && !Equals(actual, from),
            FilterOperator.Lt => from is not null && cmp(actual, from) < 0,
            FilterOperator.Le => from is not null && cmp(actual, from) <= 0,
            FilterOperator.Gt => from is not null && cmp(actual, from) > 0,
            FilterOperator.Ge => from is not null && cmp(actual, from) >= 0,
            FilterOperator.Between => from is not null && to is not null && cmp(actual, from) >= 0 && cmp(actual, to) <= 0,
            _ => false
        };
    }
}
