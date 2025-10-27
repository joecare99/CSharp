using CommunityToolkit.Mvvm.ComponentModel;

namespace DataAnalysis.WPF.ViewModels;

public abstract partial class AggregationItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;
    protected AggregationItemViewModel(string title) => Title = title;
}
