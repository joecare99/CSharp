using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAnalysis.Core.Models;

namespace DataAnalysis.WPF.ViewModels;

public sealed partial class AnalysisResultViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<AggregationItemViewModel> aggregations = new();

    public AnalysisResultViewModel(AnalysisResult result)
    {
        foreach (var agg in result.Aggregations)
        {
            if (agg.Series is not null)
            {
                // decide if this is cluster (DbScan.Vector2 keys) or normal series
                var firstKey = agg.Series.Keys.FirstOrDefault();
                if (firstKey is not null && firstKey.GetType().Name.Contains("Vector2"))
                    Aggregations.Add(new ClusterAggregationViewModel(agg));
                else
                    Aggregations.Add(new SeriesAggregationViewModel(agg));
            }
            else if (agg.Matrix is not null)
                Aggregations.Add(new MatrixAggregationViewModel(agg));
        }
    }
}
