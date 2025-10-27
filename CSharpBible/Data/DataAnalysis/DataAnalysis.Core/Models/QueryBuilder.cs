namespace DataAnalysis.Core.Models;

internal sealed class QueryBuilder
{
    private const string TotalRowKey = "Summe";
    private readonly AnalysisQuery _q;
    private readonly Dictionary<string, int>? _series;
    private readonly Dictionary<string, Dictionary<string,int>>? _matrix;

    public QueryBuilder(AnalysisQuery q)
    {
        _q = q;
        if (q.Dimensions.Count ==1)
        {
            _series = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
        else if (q.Dimensions.Count ==2)
        {
            _matrix = new Dictionary<string, Dictionary<string,int>>(StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            throw new NotSupportedException("Only1D or2D aggregations are supported.");
        }
    }

    public void Observe(SyslogEntry e)
    {
        if (_series is not null)
        {
            var key = GetKey(e, _q.Dimensions[0]);
            if (key is null) return;
            _series[key] = _series.TryGetValue(key, out var c) ? c +1 :1;
        }
        else if (_matrix is not null)
        {
            var row = GetKey(e, _q.Dimensions[0]);
            var col = GetKey(e, _q.Dimensions[1]);
            if (row is null || col is null) return;
            if (!_matrix.TryGetValue(row, out var d)) { d = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase); _matrix[row] = d; }
            d[col] = d.TryGetValue(col, out var c) ? c +1 :1;
        }
    }

    public AggregationResult Build()
    {
        if (_series is not null)
        {
            var data = _series;
            if (_q.TopN is int n)
            {
                data = data.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Take(n).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            }
            return new AggregationResult { Title = _q.Title, Dimensions = _q.Dimensions, Series = data };
        }
        else
        {
            var baseMatrix = (_q.RowTopN is int rn)
                ? _matrix!.ToDictionary(
                    kv => kv.Key,
                    kv => (IReadOnlyDictionary<string,int>) kv.Value.OrderByDescending(x => x.Value).ThenBy(x => x.Key).Take(rn).ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase)
                : _matrix!.ToDictionary(kv => kv.Key, kv => (IReadOnlyDictionary<string,int>) kv.Value, StringComparer.OrdinalIgnoreCase);

            var totals = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in baseMatrix.Values)
            {
                foreach (var kv in row)
                {
                    totals[kv.Key] = totals.TryGetValue(kv.Key, out var c) ? c + kv.Value : kv.Value;
                }
            }

            IReadOnlyList<string> columns = (_q.Columns is not null && _q.Columns.Count >0)
                ? _q.Columns
                : totals.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Select(kv => kv.Key).ToList();

            var totalRow = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
            foreach (var col in columns)
            {
                totalRow[col] = totals.TryGetValue(col, out var v) ? v :0;
            }

            var finalMatrix = new Dictionary<string, IReadOnlyDictionary<string,int>>(baseMatrix, StringComparer.OrdinalIgnoreCase)
            {
                [TotalRowKey] = totalRow
            };

            return new AggregationResult { Title = _q.Title, Dimensions = _q.Dimensions, Columns = columns, Matrix = finalMatrix };
        }
    }

    private static string? GetKey(SyslogEntry e, DimensionKind kind)
    {
        return kind switch
        {
            DimensionKind.Severity => e.Severity.ToString(),
            DimensionKind.Source => string.IsNullOrWhiteSpace(e.Source) ? "(unbekannt)" : e.Source.Trim(),
            DimensionKind.Hour => e.Timestamp is DateTimeOffset ts ? new DateTimeOffset(ts.Year, ts.Month, ts.Day, ts.Hour,0,0, ts.Offset).LocalDateTime.ToString("yyyy-MM-dd HH:00") : null,
            DimensionKind.MessageNormalized => AnalysisModel.NormalizeMessage(e.Message),
            _ => null
        };
    }
}

