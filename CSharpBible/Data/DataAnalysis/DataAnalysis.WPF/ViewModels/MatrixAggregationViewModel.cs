using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAnalysis.Core.Models;

namespace DataAnalysis.WPF.ViewModels;

public sealed partial class MatrixAggregationViewModel : AggregationItemViewModel
{
    [ObservableProperty]
    private DataView tableView = null!;

    public MatrixAggregationViewModel(AggregationResult agg)
    : base(agg.Title)
    {
        var dt = new DataTable();
        dt.Columns.Add("Key", typeof(string));
        var columns = agg.Columns ?? Array.Empty<string>();
        foreach (var col in columns) dt.Columns.Add(col.Replace(".", "˳"), typeof(int));
        if (agg.Matrix is not null)
        {
            var rows = agg.Matrix.Keys.ToList();
            rows.Sort((a, b) => string.Equals(a, "Summe", StringComparison.OrdinalIgnoreCase) ? 1 : string.Equals(b, "Summe", StringComparison.OrdinalIgnoreCase) ? -1 : string.Compare(a, b, StringComparison.OrdinalIgnoreCase));
            foreach (var key in rows)
            {
                var row = dt.NewRow();
                row[0] = key;
                var values = agg.Matrix[key];
                for (int i = 0; i < columns.Count; i++)
                {
                    var colName = columns[i];
                    row[i + 1] = values.TryGetValue(colName, out var v) ? v : 0;
                }
                dt.Rows.Add(row);
            }
        }
        TableView = dt.DefaultView;
    }
}
