using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Gen_FreeWin;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;

namespace GenFreeAvln.Views;

public partial class FraStatistics : UserControl
{
    private readonly IFraStatisticsViewModel _statisticsViewModel;
    private readonly IApplUserTexts _strings;

    public FraStatistics(IFraStatisticsViewModel statisticsViewModel, IApplUserTexts strings)
    {
        _statisticsViewModel = statisticsViewModel;
        _strings = strings;
        DataContext = _statisticsViewModel;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetDefaultTexts()
    {
        if (this.FindControl<TextBlock>("lblHdrPersons") is TextBlock lblHdrPersons)
        {
            lblHdrPersons.Text = _strings[EUserText.t84_Persons];
        }

        if (this.FindControl<TextBlock>("lblHdrFamilies") is TextBlock lblHdrFamilies)
        {
            lblHdrFamilies.Text = _strings[EUserText.t83_Families];
        }

        if (this.FindControl<TextBlock>("lblHdrPlaces") is TextBlock lblHdrPlaces)
        {
            lblHdrPlaces.Text = _strings[EUserText.t99_Places];
        }

        if (this.FindControl<TextBlock>("lblHdrDates") is TextBlock lblHdrDates)
        {
            lblHdrDates.Text = _strings[EUserText.t100_Dates];
        }

        if (this.FindControl<TextBlock>("lblHdrTexts") is TextBlock lblHdrTexts)
        {
            lblHdrTexts.Text = _strings[EUserText.t101_Texts];
        }
    }
}
