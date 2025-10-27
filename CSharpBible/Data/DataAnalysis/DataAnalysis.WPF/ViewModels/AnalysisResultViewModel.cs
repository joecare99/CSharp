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
                Aggregations.Add(new SeriesAggregationViewModel(agg));
            else if (agg.Matrix is not null)
                Aggregations.Add(new MatrixAggregationViewModel(agg));
        }
    }
}
