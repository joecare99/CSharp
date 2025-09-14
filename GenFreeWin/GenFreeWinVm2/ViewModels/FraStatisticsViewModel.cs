using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;

public partial class FraStatisticsViewModel : BaseViewModelCT, IFraStatisticsViewModel
{
    public int Persons => 1;

    [ObservableProperty]
    public partial int Families { get; set; } = 2;

    [ObservableProperty]
    public partial int Places { get; set; } = 3;

    [ObservableProperty]
    public partial int Dates { get; set; } = 4;

    [ObservableProperty]
    public partial int Texts { get; set; } = 5;

    public void UpdateStat()
    {
        Texts++;
    }

    public void SetDates(int count)
    {
        Dates = count;
    }

    public void SetTexts(int Count)
    {
        Texts = Count;
    }
}