using CommunityToolkit.Mvvm.ComponentModel;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;

namespace Gen_FreeWin.ViewModels;

public partial class FrmStatisticsViewModel : BaseViewModelCT, IFraStatisticsViewModel
{
    [ObservableProperty]
    public int _persons;
    [ObservableProperty]
    public int _Families;
    [ObservableProperty]
    public int _places;
    [ObservableProperty]
    public int _dates;
    [ObservableProperty]
    public int _texts;

    public void SetDates(int count) => Dates = count;
    public void SetTexts(int Count) => Texts = Count;
    public void UpdateStat()
    {

    }
}
