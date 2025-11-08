using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataAnalysis.Core.Models;

public static class FilterTextFormatter
{
 public static string? Describe(FilterDefinition? def)
 {
 if (def is null) return null;
 return def switch
 {
 ValueFilterDefinition v => DescribeValue(v),
 GroupFilterDefinition g => DescribeGroup(g),
 _ => null
 };
 }

 private static string DescribeGroup(GroupFilterDefinition g)
 {
 if (g.Filters is null || g.Filters.Count ==0) return g.Negate ? "NOT (—)" : "(—)";
 var inner = string.Join(g.Mode.Equals("or", StringComparison.OrdinalIgnoreCase) ? " OR " : " AND ", g.Filters.Select(Describe).Where(s => !string.IsNullOrWhiteSpace(s)));
 var s = $"({inner})";
 return g.Negate ? $"NOT {s}" : s;
 }

 private static string DescribeValue(ValueFilterDefinition v)
 {
 var field = v.Field;
 var op = v.Operator switch
 {
 FilterOperator.Eq => "=",
 FilterOperator.Ne => "!=",
 FilterOperator.Lt => "<",
 FilterOperator.Le => "<=",
 FilterOperator.Gt => ">",
 FilterOperator.Ge => ">=",
 FilterOperator.Contains => "contains",
 FilterOperator.StartsWith => "startsWith",
 FilterOperator.EndsWith => "endsWith",
 FilterOperator.In => "in",
 FilterOperator.Between => "between",
 _ => v.Operator.ToString()
 };
 string value = v.Value ?? string.Empty;
 string valueTo = v.ValueTo ?? string.Empty;

 if (v.DataType == FilterDataType.DateTime)
 {
 // Für Anzeige: ISO oder angegebene Culture
 var culture = string.IsNullOrWhiteSpace(v.Culture) ? CultureInfo.InvariantCulture : new CultureInfo(v.Culture);
 if (DateTimeOffset.TryParse(value, culture, DateTimeStyles.None, out var dto)) value = dto.ToString("s");
 if (DateTimeOffset.TryParse(valueTo, culture, DateTimeStyles.None, out var dto2)) valueTo = dto2.ToString("s");
 }

 return v.Operator == FilterOperator.Between
 ? $"{field} between '{value}' and '{valueTo}'"
 : v.Operator == FilterOperator.In
 ? $"{field} in [{value}]"
 : $"{field} {op} '{value}'";
 }
}
