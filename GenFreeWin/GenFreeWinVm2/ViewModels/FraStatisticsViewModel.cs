using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace GenFreeWin.ViewModels;

public partial class FraStatisticsViewModel : BaseViewModelCT, IFraStatisticsViewModel
{
    public int Persons => 1;

    public int Families => 2;

    public int Places =>3;

    public int Dates => 4;

    [ObservableProperty]
    public int _texts = 5;

    public void UpdateStat()
    {
        Texts++;
    }
}