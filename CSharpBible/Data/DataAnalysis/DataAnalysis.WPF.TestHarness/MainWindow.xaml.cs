using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataAnalysis.Core.Models;
using DataAnalysis.WPF.ViewModels;
using DataAnalysis.WPF.Views.Controls;

namespace DataAnalysis.WPF.TestHarness;

public partial class MainWindow : Window
{
    public MainWindow() { InitializeComponent(); }

    private void OnLoadSeries(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("OnLoadSeries clicked");
        var agg = new AggregationResult
        {
            Title = "Series Demo",
            Dimensions = new[] { DimensionKind.Severity },
            Series = new Dictionary<string, int> { ["A"] = 5, ["B"] = 12, ["C"] = 3 }
        };
        var vm = new SeriesAggregationViewModel(agg);
        var view = new SeriesAggregationView { DataContext = vm };
        Host.Content = new Border { BorderBrush = Brushes.DarkGray, BorderThickness = new Thickness(1), Padding = new Thickness(8), Child = view };
    }

    private void OnLoadMatrix(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("OnLoadMatrix clicked");
        var matrix = new Dictionary<string, IReadOnlyDictionary<string, int>>
        {
            ["Device1"] = new Dictionary<string, int> { ["E1"] = 2, ["E2"] = 50000 },
            ["Device2"] = new Dictionary<string, int> { ["E1"] = 0, ["E2"] = 1 },
            ["Summe"] = new Dictionary<string, int> { ["E1"] = 2, ["E2"] = 600 }
        };
        var agg = new AggregationResult
        {
            Title = "Matrix Demo",
            Dimensions = new[] { DimensionKind.Source, DimensionKind.MessageNormalized },
            Columns = new[] { "E1", "E2" },
            Matrix = matrix
        };
        var vm = new MatrixAggregationViewModel(agg);
        var view = new MatrixAggregationView { DataContext = vm };
        Host.Content = new Border { BorderBrush = Brushes.DarkGray, BorderThickness = new Thickness(1), Padding = new Thickness(8), Child = view };
    }
}
