using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DataAnalysis.Core.Models;

namespace DataAnalysis.WPF.ViewModels;

public sealed partial class SeriesAggregationViewModel : AggregationItemViewModel
{
    public sealed class Item
    {
        public string Category { get; init; } = string.Empty;
        public int Value { get; init; }
        public double Percentage { get; init; } //0..100
    }

    public ObservableCollection<Item> Items { get; } = new();

    [ObservableProperty]
    private int maxValue;

    [ObservableProperty]
    private double barScale;

    public SeriesAggregationViewModel(AggregationResult agg)
    : base(agg.Title)
    {
        Rebuild(agg);
    }

    private void Rebuild(AggregationResult agg)
    {
        Items.Clear();
        int max = 0;
        var temp = new List<Item>();
        if (agg.Series is not null)
        {
            foreach (var kv in agg.Series)
            {
                temp.Add(new Item { Category = kv.Key.ToString(), Value = kv.Value });
                if (kv.Value > max) max = kv.Value;
            }
        }
        MaxValue = max;
        BarScale = max > 0 ? 300.0 / max : 1.0;
        foreach (var it in temp)
        {
            var pct = max > 0 ? (double)it.Value / max * 100.0 : 0.0;
            Items.Add(new Item { Category = it.Category, Value = it.Value, Percentage = pct });
        }
    }
}
