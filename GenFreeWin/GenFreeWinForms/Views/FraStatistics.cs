using Gen_FreeWin.ViewModels.Interfaces;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.UI;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;

public partial class FraStatistics : UserControl
{
    private IFraStatisticsViewModel _statisticsViewModel;
    private IApplUserTexts Modul1_IText;

    public FraStatistics(IFraStatisticsViewModel statisticsViewModel, IApplUserTexts strings)
    {
        Modul1_IText = strings;
        InitializeComponent();

        _statisticsViewModel = statisticsViewModel;

        TextBindingAttribute.Commit(this, _statisticsViewModel);
    }

    public void SetDefaultTexts()
    {
        lblHdrPersons.Text = Modul1_IText[EUserText.t84_Persons];
        lblHdrFamilies.Text = Modul1_IText[EUserText.t83_Families];
        lblHdrPlaces.Text = Modul1_IText[EUserText.t99_Places];
        lblHdrDates.Text = Modul1_IText[EUserText.t100_Dates];
        lblHdrTexts.Text = Modul1_IText[EUserText.t101_Texts];
    }

}


