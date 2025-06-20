using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Gen_FreeWin;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;

namespace GenFreeWpf.ViewModels;

public partial class FraStatisticsViewModel : ObservableObject, IFraStatisticsViewModel
{
    private readonly IApplUserTexts _userTexts;

    public FraStatisticsViewModel(IApplUserTexts userTexts)
    {
        _userTexts = userTexts;
        UpdateStat();
    }

    public string PersonsHeader => _userTexts[EUserText.t84_Persons];
    public string FamiliesHeader => _userTexts[EUserText.t83_Families];
    public string PlacesHeader => _userTexts[EUserText.t99_Places];
    public string DatesHeader => _userTexts[EUserText.t100_Dates];
    public string TextsHeader => _userTexts[EUserText.t101_Texts];

    [ObservableProperty]
    private int persons;

    [ObservableProperty]
    private int families;

    [ObservableProperty]
    private int places;

    [ObservableProperty]
    private int dates;

    [ObservableProperty]
    private int texts;

    public void UpdateStat()
    {
        // Hier sollten die echten Werte aus dem Model geladen werden
        Persons = 0;
        Families = 0;
        Places = 0;
        Dates = 0;
        Texts = 0;
    }

    public void SetDates(int count)
    {
        throw new System.NotImplementedException();
    }

    public void SetTexts(int Count)
    {
        throw new System.NotImplementedException();
    }
}
