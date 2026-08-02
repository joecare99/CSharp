using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public interface IOrtsVerViewModel : INotifyPropertyChanged
{
    string edtPlace_Text { get; set; }
    string edtSuburb_Text { get; set; }
    string edtCounty_Text { get; set; }
    string edtCountry_Text { get; set; }
    string edtState_Text { get; set; }
    string edtLocator_Text { get; set; }
    string edtLat1_Text { get; set; }
    string edtLong1_Text { get; set; }
    string edtGOV_Text { get; set; }
    string edtLat2_Text { get; set; }
    string edtLat3_Text { get; set; }
    string edtLong2_Text { get; set; }
    string edtLong3_Text { get; set; }
    string edtZIP_Text { get; set; }
    string edtAdditional_Text { get; set; }
    string edtPolName_Text { get; set; }
    string TextBox17_Text { get; set; }
    string TextBox18_Text { get; set; }
    string TextBox22_Text { get; set; }
    string TextBox23_Text { get; set; }
    string TextBox24_Text { get; set; }
    string TextBox25_Text { get; set; }
    string TextBox26_Text { get; set; }
    string TextBox27_Text { get; set; }
    string TextBox28_Text { get; set; }
    string TextBox29_Text { get; set; }
    string TextBox30_Text { get; set; }
    string TextBox31_Text { get; set; }
    string TextBox32_Text { get; set; }
    string RTB1_Text { get; set; }

    bool Frame1_Visible { get; }
    bool TextBox30_Visible { get; }
    bool btnNext_Visible { get; set; }
    bool btnPrev_Visible { get; set; }
    bool btnShowPlaceGE_Visible { get; set; }
    bool btnShowPlaceGM_Visible { get; set; }
    bool btnLinkGOV_Visible { get; set; }
    bool btnSearchGOV_Visible { get; set; }
    bool btnConvertKoords_Visible { get; set; }
    bool btnSearchName_Visible { get; set; }
    bool btnSearchNumber_Visible { get; set; }
    bool CloseViewButtonVisible { get; set; }
    bool BeginEditButtonVisible { get; set; }
    bool SavePlaceButtonVisible { get; set; }
    bool Button15_Visible { get; set; }
    bool Button17_Visible { get; set; }
    bool Button18_Visible { get; set; }
    bool Button19_Visible { get; set; }

    EUserText CoordinatesDecimalHintText { get; set; }
    EUserText btnNext_Text { get; set; }
    EUserText btnPrev_Text { get; set; }
    EUserText btnShowPlaceGE_Text { get; set; }
    EUserText btnShowPlaceGM_Text { get; set; }
    EUserText btnLinkGOV_Text { get; set; }
    string Label25_Text { get; set; }
    string Label27_Text { get; set; }
    string Label32_Text { get; set; }
    string Label28_Text { get; set; }
    string Label29_Text { get; set; }
    string Label30_Text { get; set; }
    string Label31_Text { get; set; }
    string Label33_Text { get; set; }
    string Label35_Text { get; set; }
    EUserText btnConvertKoords_Text { get; set; }
    EUserText btnSearchName_Text { get; set; }
    EUserText btnSearchNumber_Text { get; set; }
    EUserText CloseViewButtonText { get; set; }

    IContainerControl View { get; set; }
    IRelayCommand CloseViewCommand { get; }
    IRelayCommand BeginEditCommand { get; }
    IRelayCommand SavePlaceCommand { get; }
    IRelayCommand CalculateCoordinatesCommand { get; }
    IRelayCommand ApplyCoordinatesCommand { get; }
    IRelayCommand CloseCoordinateConverterCommand { get; }
    IRelayCommand OpenPicturesCommand { get; }
    IRelayCommand CancelEditCommand { get; }
    IRelayCommand DeletePlaceCommand { get; }
    IRelayCommand SaveAndReturnCommand { get; }
    IRelayCommand FindUnusedPlacesCommand { get; }
    IRelayCommand DetectCoordinatesCommand { get; }
    IRelayCommand ResetSearchViewCommand { get; }
    IRelayCommand OpenDistancePanelCommand { get; }
    IRelayCommand CloseDistancePanelCommand { get; }
    IRelayCommand NextCommand { get; }
    IRelayCommand PrevCommand { get; }
    IRelayCommand ShowPlaceGECommand { get; }
    IRelayCommand ShowPlaceGMCommand { get; }
    IRelayCommand LinkGOVCommand { get; }
    IRelayCommand SearchGOVCommand { get; }
    IRelayCommand ConvertKoordsCommand { get; }
    IRelayCommand SearchNameCommand { get; }
    IRelayCommand SearchNumberCommand { get; }
    EUserText btnSearchGOV_Text { get; set; }
    bool ListBox1_Visible { get; }
    bool ListBox2_Visible { get; }
    bool Button23_Visible { get; }
    bool Button21_Visible { get; }
    string Button16_Text { get; }
    string Label1_Text { get; }
    string Label13_Text { get; }
    EUserText CancelEditButtonText { get; }
    EUserText DeletePlaceButtonText { get; }
    EUserText SaveAndReturnButtonText { get; }
    EUserText FindUnusedPlacesButtonText { get; }
    bool Panel1_Visible { get; }
    bool edtPlace_Visible { get; }
    bool edtLocator_Visible { get; }
    EUserText LatitudeLabelText { get; set; }
    EUserText LongitudeLabelText { get; set; }
    EUserText GovLabelText { get; set; }
    EUserText AdditionalOutputLabelText { get; set; }
    EUserText StateLabelText { get; set; }
    EUserText SuburbLabelText { get; set; }
    EUserText CountyLabelText { get; set; }
    EUserText CountryLabelText { get; set; }
    EUserText PlaceLabelText { get; set; }
    EUserText LocatorLabelText { get; set; }
    EUserText PostalCodeLabelText { get; set; }
    EUserText AdditionalLabelText { get; set; }
    EUserText Label17_Text { get; set; }
    EUserText PoliticalNameLabelText { get; set; }
    EUserText ConverterLatitudeLabelText { get; set; }
    EUserText ConverterLongitudeLabelText { get; set; }
    EUserText ConverterHeaderLabelText { get; set; }
    EUserText SearchPromptLabelText { get; set; }

    void Form_Load(object sender, EventArgs e);
    void ListBox1_DoubleClick(object sender, EventArgs e);
    void ListBox2_DoubleClick(object s, EventArgs e);
    void ListBox3_DoubleClick(object s, EventArgs e);
    void ListBox4_DoubleClick(object s, EventArgs e);
    void Locber();
    void RTB1_KeyUp(object sender, KeyEventArgs e);
    void TextBox1_KeyUp(object sender, KeyEventArgs e);
}