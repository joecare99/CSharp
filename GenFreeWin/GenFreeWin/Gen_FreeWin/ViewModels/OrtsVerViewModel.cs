using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Main;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Gen_FreeWin.ViewModels;
public partial class OrtsVerViewModel: BaseViewModelCT, IOrtsVerViewModel
{
    IContainerControl IOrtsVerViewModel.View { get; set; }
    Ortsver View => (Ortsver)((IOrtsVerViewModel)this).View;

	[ObservableProperty]
	public partial bool Frame1_Visible { get; set; }

	IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction = Menue.Default;
    #region VB-Compatibility-Interfaces
    IProjectData ProjectData => Modul1.ProjectData;
    IStrings Strings => Modul1.Strings;
    IVBConversions Conversion => Modul1.Conversions;
    #endregion

    private string A4;
    private string GOO;
    private bool Verwend;
    private string Koord;
    private string Ko;
    private string Br;
    private string LR;
    private string Ort;
    private int Nr;
    private short Tastascii;
    private byte Textindex;
    private string WinPath;

    public float Modul1_La3 { get; private set; }

    private byte Modul1_aByte;
    private string Modul1_La2;
    private string Modul1_Ba1;
    private string Modul1_Ba3;
    private string Modul1_Ba5;
    private string Modul1_Ba2;
    private string Modul1_La1;
    private int Modul1_OrtNr;
    private (string, ETextKennz) Modul1_Bezeichnu;

    public void Textles(ETextKennz eTKennz, string ubgT)
    {
        View.ListBox1.Items.Clear();
        Modul1.STextles("Ortsverw.List1", eTKennz, ubgT, View.ListBox1.Items);
    }

    public void Form_Load(object sender, EventArgs e)
    {
        WinPath = Environment.GetEnvironmentVariable("Windir");

        View.RTB1.AddContextMenu();
        if (Modul1.FontSize > 0f)
        {
            View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.ListBox2.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Regular);
            View.Frame1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label19.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label20.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Label21.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Button13.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Button14.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.Button15.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox22.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox23.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox24.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox25.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox26.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox27.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox28.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            View.TextBox29.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
        View.btnNext.Text = Modul1.IText[EUserText.t155];
        View.btnPrev.Text = Modul1.IText[EUserText.t156];
        View.btnShowPlaceGE.Text = Modul1.IText[EUserText.t356];
        View.btnShowPlaceGM.Text = Modul1.IText[EUserText.t357];
        View.btnLinkGOV.Text = Modul1.IText[EUserText.t359];
        View.btnSearchGOV.Text = Modul1.IText[EUserText.t360];
        View.btnConvertKoords.Text = Modul1.IText[EUserText.t358];
        View.btnSearchName.Text = Modul1.IText[EUserText.t273];
        View.btnSearchNumber.Text = Modul1.IText[EUserText.t272];
        if (View.Button10.Text == "")
        {
            View.Button10.Text = Modul1.IText[EUserText.t158];
        }
        View.Button11.Text = Modul1.IText[EUserText.t73];
        View.Button12.Text = Modul1.IText[EUserText.tNMSave];
        View.Button13.Text = Modul1.IText[EUserText.t296];
        View.Button14.Text = Modul1.IText[EUserText.t67];
        View.Button15.Text = Modul1.IText[EUserText.t297];
        View.Button16.Text = Modul1.IText[EUserText.t129];
        View.Button17.Text = Modul1.IText[EUserText.t354];
        View.Button18.Text = Modul1.IText[EUserText.t363];
        View.Button19.Text = Modul1.IText[EUserText.t361];
        View.Button20.Text = Modul1.IText[EUserText.t362];
        View.Label1.Text = Modul1.IText[EUserText.tPlace];
        View.Label2.Text = Modul1.IText[EUserText.t140];
        View.Label3.Text = Modul1.IText[EUserText.t141];
        View.Label4.Text = Modul1.IText[EUserText.t142];
        View.Label5.Text = Modul1.IText[EUserText.t143];
        View.Label7.Text = Modul1.IText[EUserText.t146];
        View.Label8.Text = Modul1.IText[EUserText.t145];
        View.Label9.Text = Modul1.IText[EUserText.t283];
        View.Label10.Text = Modul1.IText[EUserText.t350];
        View.Label11.Text = Modul1.IText[EUserText.t149];
        View.Label13.Text = Modul1.IText[EUserText.t150];
        View.Label13.Tag = 0;
        View.Label14.Text = Modul1.IText[EUserText.t148];
        View.Label16.Text = Modul1.IText[EUserText.t147];
        View.Label18.Text = Modul1.IText[EUserText.t351];
        View.Label19.Text = Modul1.IText[EUserText.t365];
        View.Label20.Text = Modul1.IText[EUserText.t145];
        View.Label21.Text = Modul1.IText[EUserText.t146];
        View.Label22.Text = Modul1.IText[EUserText.t353];
        View.TextBox21.Text = Modul1.IText[EUserText.t352];

        var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
        View.Left = aiPos[0];
        View.Top = aiPos[1];
        View.Text = $"{Modul1.AppName} Ortsverwaltung für Mandant {Modul1.Mandant}";
        View.BackColor = Modul1.HintFarb;
        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
        View.WindowState = WiS;
        FileSystem.FileClose(99);
        if (Directory.Exists(WinPath + "\\Microsoft.net\\Framework\\v3.5"))
        {
            View.Button21.Visible = true;
        }
        if (DataModul.Place.Count == 0)
        {
            View.Label13.Text = Modul1.IText[EUserText.t150] + ": 1";
            View.Label13.Tag = 1;
        }
        else if (Modul1.Schalt < 0)
        {
            ortles(0, Math.Abs(Modul1.Schalt));
        }
        else if (Modul1.Schalt == 0)
        {
            if (DataModul.Place.Count == 0)
            {
                View.Label1.Text = Modul1.IText[EUserText.t150] + ": 1";
                View.Label1.Tag = 1;
                goto end_IL_0001;
            }
            DataModul.DB_PlaceTable.MoveFirst();
            ortles(0, 0);
        }
        else if (Modul1.Schalt == 2)
        {
            Modul1.Schalt = 1;
        }
        else
        {
            ortles(0, Modul1.Schalt);
        }
        goto end_IL_0001;
    end_IL_0001:
        ;
    }

    public void Geoles(IPlaceData cPlace)
    {
        View_ClearGeoKoor();

        View.btnShowPlaceGE.Visible = Modul1.GeolesPlace(cPlace, Setfields, false);
    }

    private void View_ClearGeoKoor()
    {
        View.edtLong1.Text = "";
        View.edtLong2.Text = "";
        View.edtLong3.Text = "";
        View.edtLat1.Text = "";
        View.edtLat2.Text = "";
        View.edtLat3.Text = "";
    }

    private void Setfields((string, string) tuple)
    {
        switch (tuple.Item1)
        {
            case "B.h":
                View.edtLat1.Text = tuple.Item2;
                break;
            case "B.m":
                View.edtLat2.Text = tuple.Item2;
                break;
            case "B.s":
                View.edtLat2.Text = tuple.Item2;
                break;
            case "L.h":
                View.edtLong1.Text = tuple.Item2;
                break;
            case "L.m":
                View.edtLong2.Text = tuple.Item2;
                break;
            case "L.s":
                View.edtLong3.Text = tuple.Item2;
                break;

        }
    }

    public void ortles(short Richt, int ubg1)
    {
        View.ListBox1.Items.Clear();
        View.Button18.Visible = false;
        ProjectData.ClearProjectError();
        switch (Richt)
        {
            case 0:
                Nr = ubg1.AsInt();
                break;
            case 3:
            case 2:
                Nr = View.Label13.Tag.AsInt();
                break;
            default:
                break;
        }
        GenFree.Interfaces.DB.IRecordset dB_PlaceTable;
        if (!SeekPlace(Richt, Nr, out IPlaceData? cPlace))
        {
            return;
        }

        SetData(cPlace!);
        Geoles(cPlace);
        Verwend = Ortverwend(cPlace.ID);
        View.Button18.Visible = true;

    }

    private void SetData(IPlaceData cPlace)
    {
        bool xPictExists = DataModul_Pictures_Exists(cPlace.ID, "O");

        View.Label13.Text = $"{Modul1.IText[EUserText.t150]}:{cPlace.ID}";
        View.Label13.Tag = cPlace.ID;
        View.edtPlace.Text = cPlace.sOrt;
        View.edtSuburb.Text = cPlace.sOrtsteil;
        View.edtCounty.Text = cPlace.sKreis;
        View.edtCountry.Text = cPlace.sLand;
        View.edtState.Text = cPlace.sStaat;
        View.edtLocator.Text = cPlace.sLoc;
        View.TextBox17.Text = cPlace.sTerr.Trim();
        View.TextBox18.Text = cPlace.sStaatk;
        View.edtAdditional.Text = cPlace.sZusatz;
        View.edtPolName.Text = cPlace.sPolName;
        View.edtZIP.Text = cPlace.sPLZ;
        View.edtGOV.Text = cPlace.sGOV;

        View.btnLinkGOV.Visible = View.edtGOV.Text.Trim() != "";
        View.RTB1.Text = "";
        View.RTB1.SelectedText = cPlace.sBem;
        View.Button16.BackColor = View.Button11.BackColor;
        View.Button16.Text = $"{Modul1.IText[EUserText.t129]} {Modul1.IText[EUserText.tNo]}";
        if (xPictExists)
        {
            View.Button16.Text = $"{Modul1.IText[EUserText.t129]} {Modul1.IText[EUserText.tYes]}";
            View.Button16.BackColor = ColorTranslator.FromOle(12648447);
        }
    }

    private bool DataModul_Pictures_Exists(int cPlace_PlaceNr, string sPKennz)
    {
        DataModul.DB_BildTab = DataModul.MandDB.OpenRecordset($"select * from {dbTables.Bilder} Where {dbTables.Bilder}.{PictureFields.Kennz}='{sPKennz}'");
        DataModul.DB_BildTab.FindFirst($"{dbTables.Bilder}.{PictureFields.ZuNr} = {cPlace_PlaceNr}");
        bool xPictExists = !DataModul.DB_BildTab.NoMatch;
        return xPictExists;
    }

    private bool SeekPlace(short Richt, int Nr, out IPlaceData? cPlace)
    {
        var dB_PlaceTable = DataModul.DB_PlaceTable;
        dB_PlaceTable.Index = nameof(PlaceIndex.OrtNr);
        if (dB_PlaceTable.RecordCount > 0)
        {
            dB_PlaceTable.MoveFirst();
            var iMinID = dB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
            dB_PlaceTable.MoveLast();
            var iMaxID = dB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
            dB_PlaceTable.Seek("=", Nr);
            while (dB_PlaceTable.NoMatch)
            {
                if (unchecked(Richt == 3 || Richt == 0))
                {
                    Nr++;
                }
                if (Richt == 2)
                {
                    Nr--;
                }

                if (Nr < 1)
                {
                    if (dB_PlaceTable.RecordCount > 0)
                    {
                        Nr = iMinID;
                    }
                    else
                    {
                        Nr = 1;
                    }
                }
                if (Nr > iMaxID)
                {
                    Nr = iMaxID;
                }
                dB_PlaceTable.Seek("=", Nr);
            }
        }
        else
        {
            cPlace = null;
            return false;
        }

        if (dB_PlaceTable.EOF)
        {
            _ = Interaction.MsgBox("Stop", title: "8", mb: MessageBoxButtons.OK);
        }

        return DataModul.Place.ReadData(Nr, out cPlace);
    }

    [RelayCommand]
    private void Next()
    {
        //Discarded unreachable code: IL_0320
        ortles(3, Modul1.Ubg);
        if (View.CheckBox1.CheckState != CheckState.Checked)
        {
            return;
        }
        string adrString = View.edtPlace.Text + " " + View.edtSuburb.Text + " " + View.edtCountry.Text + " " + View.edtState.Text;
        Geocode geocode = GeocodeAdress(adrString);
        Modul1.UbgT = geocode.Longitude;
        if (Modul1.UbgT == "")
        {
            _ = Interaction.MsgBox(" Koordinaten konnten nicht ermittelt werden, zu wenig oder falsche Angaben");
        }
        checked
        {
            if (Modul1.UbgT != "")
            {
                Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ".");
                string text;
                if (Modul1_aByte > 0)
                {
                    Rechnen(Modul1.UbgT);
                    text = Strings.Right("   " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 3) + "°";
                    Modul1_Ba1 = text + Modul1_Ba3.Trim() + "' " + Modul1_Ba5.Trim();
                }
                Modul1.UbgT = geocode.Latitude;
                Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ".");
                if (Modul1_aByte > 0)
                {
                    Rechnen(Modul1.UbgT);
                    text = Strings.Right("   " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 3) + "°";
                    Modul1_Ba2 = text + Modul1_Ba3.Trim() + "' " + Modul1_Ba5.Trim();
                }
                if ((View.edtLat1.Text.Trim() != Modul1_Ba2.Left(3).Trim()
                    || View.edtLat2.Text != Strings.Mid(Modul1_Ba2, 5, 2)
                    || View.edtLat3.Text != Strings.Mid(Modul1_Ba2, 9, 2)
                    || View.edtLong1.Text.Trim() != Modul1_Ba1.Left(3).Trim()
                    || View.edtLong2.Text != Strings.Mid(Modul1_Ba1, 5, 2)
                    || View.edtLong3.Text != Strings.Mid(Modul1_Ba1, 9, 2))
                    && Interaction.MsgBox("B:" + Modul1_Ba2 + "   L:" + Modul1_Ba1, title: "Ermittelte Koordinaten übernehmen?", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    View.edtLat1.Text = Modul1_Ba2.Left(3);
                    View.edtLat2.Text = Strings.Mid(Modul1_Ba2, 5, 2);
                    View.edtLat3.Text = Strings.Mid(Modul1_Ba2, 9, 2);
                    View.edtLong1.Text = Modul1_Ba1.Left(3);
                    View.edtLong2.Text = Strings.Mid(Modul1_Ba1, 5, 2);
                    View.edtLong3.Text = Strings.Mid(Modul1_Ba1, 9, 2);
                    Geoles1();
                    View.btnShowPlaceGE.Visible = true;
                    View.Button22.PerformClick();
                }
            }
        }
    }
    [RelayCommand]
    private void Prev()
    {
        ortles(2, Modul1.Ubg);
    }
    [RelayCommand]
    private void ShowPlaceGE()
    {
        GOO = Modul1.TempPath + "\\GenPluswin.kml";
        _ = Process.Start(GOO);
    }
    [RelayCommand]
    private void ShowPlaceGM()
    {
        Ort = "";
        if (View.edtZIP.Text.Trim() != ""
            && (Strings.Asc(View.edtZIP.Text) > 46)
            && (Strings.Asc(View.edtZIP.Text) < 58)
            && DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsInt() > 0)
        {
            // Hä ?
            Ort = (DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].Value + " ");
        }
        if (View.edtSuburb.Text != "")
        {
            Ort += View.edtSuburb.Text;
        }
        else
        {
            Ort += View.edtPlace.Text;
        }
        // nochmal Hä ?
        (LR, Br) = GoogleItf_GetKoordFromKmlFile();
        Ko = Br.Trim() + "," + LR.Trim();
        Koord = $"http://maps.google.de/maps?f=q&hl=de&q={Ort}&ie=ANSI&z=15&ll={Ko}";
        _ = Process.Start(Koord);
        goto end_IL_0001_2;
    end_IL_0001_2:
        return;
    }

    private (string, string) GoogleItf_GetKoordFromKmlFile()
    {
        string Value = default;
        string DateiName;
        FileSystem.FileClose(99);
        DateiName = Modul1.TempPath + "\\GenPluswin.kml";
        string lR = default;
        string br = default;
        if (File.Exists(DateiName))
        {
            FileSystem.FileOpen(99, Modul1.TempPath + "\\GenPluswin.kml", OpenMode.Input);
            while (!FileSystem.EOF(99))
            {
                FileSystem.Input(99, ref Value);
                if (Strings.InStr(Value, "<longitude>") != 0)
                {
                    Value = Value.Replace("<longitude>", "");
                    lR = Value.Replace("</longitude>", "");
                }
                if (Strings.InStr(Value, "<latitude>") != 0)
                {
                    Value = Value.Replace("<latitude>", "");
                    br = Value.Replace("</latitude>", "");
                }
            }
        }
        return (lR, br);
    }

    [RelayCommand]
    private void LinkGOV()
    {
        _ = Process.Start("http://gov.genealogy.net/item/show?id=" + View.edtGOV.Text);
    }

    [RelayCommand]
    private void SearchGOV()
    {
        _ = Process.Start("http://gov.genealogy.net");
    }

    [RelayCommand]
    private void ConvertKoords()
    {
        Frame1_Visible = true;
        _ = View.TextBox22.Focus();
        View.Button15.Visible = false;
    }

    public void Button13_Click(object sender, EventArgs e)
    {
        if (View.TextBox22.Text == "" || View.TextBox27.Text == "")
        {
            return;
        }
        string text = View.TextBox27.Text;
        float num = Strings.InStr(text, ".");
        if (num == 0f)
        {
            _ = Interaction.MsgBox("Eingabefehler");
            return;
        }
        checked
        {
            View.TextBox26.Text = text.Left((int)Math.Round(num - 1f));
            string text2 = Strings.Mid(text, (int)Math.Round(num + 1f), text.Length);
            float num2 = default;
            float num4 = default;
            if (text2.AsInt() > 0.0)
            {
                text2 = text2 + "0000".Left(4);
                num2 = (float)(text2.AsDouble() / 10000.0 * 60.0);
                float num3 = num2 - (float)Math.Floor(num2);
                num2 = (float)Math.Floor(num2);
                num4 = (float)Math.Floor(num3 / 100f * 6000f);
            }
            View.TextBox28.Text = Strings.Format(num2, "##00");
            View.TextBox29.Text = Strings.Format(num4, "##00");
            string text3 = View.TextBox22.Text;
            num = Strings.InStr(text3, ".");
            if (num == 0f)
            {
                _ = Interaction.MsgBox("Eingabefehler");
                return;
            }
            View.TextBox23.Text = text3.Left((int)Math.Round(num - 1f));
            string text4 = Strings.Mid(text3, (int)Math.Round(num + 1f), text3.Length);
            float num5 = 0f;
            float num6 = 0f;
            if (text4.AsInt() > 0.0)
            {
                text4 = text4 + "0000".Left(4);
                num5 = (float)(text4.AsDouble() / 10000.0 * 60.0);
                float num7 = num5 - (float)Math.Floor(num5);
                num5 = (float)Math.Floor(num5);
                num6 = (float)Math.Floor(num7 / 100f * 6000f);
            }
            View.TextBox24.Text = Strings.Format(num5, "##00");
            View.TextBox25.Text = Strings.Format(num6, "##00");
            View.Button15.Visible = true;
        }
    }

    public void Button15_Click(object sender, EventArgs e)
    {
        if (!((View.TextBox23.Text.Trim() == "") | (View.TextBox27.Text.Trim() == "")))
        {
            View.edtLat1.Text = View.TextBox23.Text;
            View.edtLat2.Text = View.TextBox24.Text;
            View.edtLat3.Text = View.TextBox25.Text;
            View.edtLong1.Text = View.TextBox26.Text;
            View.edtLong2.Text = View.TextBox28.Text;
            View.edtLong3.Text = View.TextBox29.Text;
            View.btnShowPlaceGE.Visible = true;
            View.TextBox22.Text = "";
            View.TextBox23.Text = "";
            View.TextBox24.Text = "";
            View.TextBox25.Text = "";
            View.TextBox26.Text = "";
            View.TextBox27.Text = "";
            View.TextBox28.Text = "";
            View.TextBox29.Text = "";
            Frame1_Visible = false;
            if (!View.Button19.Visible)
            {
                View.Button12.Visible = true;
            }
        }
    }

    public void Button14_Click(object sender, EventArgs e)
    {
        View.TextBox22.Text = "";
        View.TextBox23.Text = "";
        View.TextBox24.Text = "";
        View.TextBox25.Text = "";
        View.TextBox26.Text = "";
        View.TextBox27.Text = "";
        View.TextBox28.Text = "";
        View.TextBox29.Text = "";
        Frame1_Visible = false;
    }

    public void Button12_Click(object sender, EventArgs e)
    {
        View.btnNext.Visible = true;
        View.btnPrev.Visible = true;
        View.btnShowPlaceGE.Visible = true;
        View.btnShowPlaceGM.Visible = true;
        View.btnLinkGOV.Visible = true;
        View.btnSearchGOV.Visible = true;
        View.btnConvertKoords.Visible = true;
        View.btnSearchName.Visible = true;
        View.btnSearchNumber.Visible = true;
        View.Button10.Visible = true;
        View.Button11.Visible = true;
        View.Button21.Visible = true;
        View.Button23.Visible = true;
        if (Directory.Exists(WinPath + "\\Microsoft.net\\Framework\\v3.5"))
        {
            View.Button21.Visible = true;
        }
        View.Button12.Visible = false;
        View.Button17.Visible = false;

        View.ListBox1.Items.Clear();
        var xAddnew = DataModul.Place.ReadData(View.Label13.Tag.AsInt(), out var cPlace);
        if (xAddnew)
        {
            cPlace = DataModul.Place.CreateNew();
        }
        byte b = 0;
        cPlace.iOrt = SaveText(View.edtPlace.Text, ETextKennz.H_, ref b);
        cPlace.iOrtsteil = SaveText(View.edtSuburb.Text, ETextKennz.I_, ref b);
        cPlace.iKreis = SaveText(View.edtCounty.Text, ETextKennz.J_, ref b);
        cPlace.iLand = SaveText(View.edtCountry.Text, ETextKennz.K_, ref b);
        cPlace.iStaat = SaveText(View.edtState.Text, ETextKennz.L_, ref b);
        cPlace.sPolName = View.edtState.Text;
        SaveText(View.edtState.Text, ETextKennz.S_, ref b);
        if (b == 0)
        {
            _ = Interaction.MsgBox("Ein leerer Ort kann nicht gespeichert werden!");
            Modul1.Ubg = View.Label13.Tag.AsInt();
            ortles(0, Modul1.Ubg);
        }
        else
        {
            cPlace.sLoc = View.edtLocator.Text.ToUpper().Trim();
            cPlace.sL = "    " + View.edtLong1.Text.Trim().Right(4) + "," + "  " + View.edtLong2.Text.Trim().Right(2) + "  " + View.edtLong3.Text.Trim().Right(2);
            cPlace.sB = "    " + View.edtLat1.Text.Trim().Right(4) + "," + "  " + View.edtLat2.Text.Trim().Right(2) + "  " + View.edtLat3.Text.Trim().Right(2);
            cPlace.sTerr = Strings.UCase(View.TextBox17.Text.Trim().Left(3));
            cPlace.sStaatk = Strings.UCase(View.TextBox18.Text.Trim().Left(3));
            cPlace.sZusatz = View.edtAdditional.Text.Trim().Left(10);
            if (View.edtZIP.Text.TrimEnd() == "")
            {
                View.edtZIP.Text = "0";
            }
            cPlace.sPLZ = View.edtZIP.Text.Trim().Left(10);
            View.RTB1.Text = View.RTB1.Text.Trim();
            if (View.RTB1.Text == "")
            {
                View.RTB1.Text = " ";
            }
            cPlace.sBem = View.RTB1.Text;
            View.edtGOV.Text = View.edtGOV.Text.ToUpper();
            cPlace.sGOV = View.edtGOV.Text.Trim().Left(20);
            DataModul.Place.Commit(cPlace);

            var sPrefix = View.edtSuburb.Text.Trim() != "" ? $"-{View.edtSuburb.Text.Trim()} " : " ";
            DataModul_DOSB_OrtS_Update(cPlace, $"{View.edtPlace.Text.Trim()}{sPrefix} {View.edtCounty.Text.Trim()} {View.edtCountry.Text.Trim()} {View.edtState.Text.Trim()}");

            Geoles(cPlace);
            Modul1.Ubg = View.Label13.Tag.AsInt();
            ortles(0, Modul1.Ubg);
        }
    }

    private int Handle_Err3022(ref string title, ref int num11)
    {
        int num5 = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
        int num6 = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
        int num7 = DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt();
        int num8 = DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt();
        int num9 = DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt();
        int num10 = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
        if (Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString()) != "")
        {
            _ = Interaction.MsgBox("Dieser Ort ist schon vorhanden. Der zu löschende Ort enthält Bemerkungen. Diese würden gelöscht. Aus Sicherheitsgründen wird die Aktion abgebrochen.");
            return num10;
        }
        else
        {
            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.Orte);
            DataModul.DB_PlaceTable.Seek("=", num5, num6, num7, num8, num9);
            if (num10 == DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt())
            {
                return num10;
            }
            if (DataModul.DB_PlaceTable.NoMatch)
            {
                title = 1.AsString();
                return num10;
            }
            else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt() != num5)
            {
                title = 2.AsString();
                return num10;
            }
            else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() != num6)
            {
                title = 3.AsString();
                return num10;
            }
            else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt() != num7)
            {
                title = 4.AsString();
                return num10;
            }
            else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt() != num8)
            {
                title = 5.AsString();
                return num10;
            }
            else if (DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt() != num9)
            {
                title = 6.AsString();
                return num10;
            }
            else
            {
                num11 = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
                if (num11 == 0)
                {
                    _ = Interaction.MsgBox("Neuort");
                    return num10;
                }
                else if (num10 == 0)
                {
                    _ = Interaction.MsgBox("Altort");
                    return num10;
                }
                foreach (var cEvent in DataModul.Event.ReadAll(EventIndex.PText, num10))
                {
                    cEvent.SetPropValue(EEventProp.iOrt, num11);
                    cEvent.Update();
                }
                DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OrtNr);
                DataModul.DB_PlaceTable.Seek("=", num10);
                if (!DataModul.DB_PlaceTable.NoMatch)
                {
                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt() == num10)
                    {
                        DataModul.DB_PlaceTable.Delete();
                    }
                    DataModul.DOSB_OrtSTable.Index = "OrtNr";
                    DataModul.DOSB_OrtSTable.Seek("=", num10);
                    if (!DataModul.DOSB_OrtSTable.NoMatch)
                    {
                        DataModul.DOSB_OrtSTable.Delete();
                    }
                    Modul1.Ubg = num11;
                    ortles(0, Modul1.Ubg);
                }
                else
                {
                    Modul1.Ubg = num11;
                    ortles(0, Modul1.Ubg);
                }
                if (View.Button19.Visible)
                {
                    _ = Interaction.MsgBox("Stop", title: "6", mb: MessageBoxButtons.OK);
                    View.Close();
                }
                else
                {
                    View.Button12.Visible = false;
                    View.Button17.Visible = false;
                    View.Button11.Visible = true;
                    View.btnNext.Visible = true;
                    View.btnPrev.Visible = true;
                    View.btnSearchName.Visible = true;
                    View.btnSearchNumber.Visible = true;
                    View.Button11.Visible = true;
                    View.Button11.Visible = true;
                    View.Button21.Visible = true;
                    View.Button23.Visible = true;
                    View.ListBox2.Items.Clear();
                    num10 = 0;
                    num11 = 0;
                }
            }





        }

        return num10;
    }

    private static void DataModul_DOSB_OrtS_Update(IPlaceData cPlace, string sPrefix)
    {
        DataModul.DOSB_OrtSTable.Index = "OrtNr";
        DataModul.DOSB_OrtSTable.Seek("=", cPlace.ID);
        if (DataModul.DOSB_OrtSTable.NoMatch)
        {
            DataModul.DOSB_OrtSTable.AddNew();
        }
        else
        {
            DataModul.DOSB_OrtSTable.Edit();
        }
        DataModul.DOSB_OrtSTable.Fields["Name"].Value = sPrefix;
        DataModul.DOSB_OrtSTable.Fields["Nr"].Value = cPlace.ID;
        DataModul.DOSB_OrtSTable.Update();
    }

    private int SaveText(string text, ETextKennz eText, ref byte b)
    {
        if (text != "")
        {
            b = 1;
            return Modul1.TextSpeich(text.TrimEnd(), "", eText);
        }
        else
        {
            return 0;
        }
    }

    public void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        switch (Textindex)
        {
            case 1:
                View.edtPlace.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtSuburb.Focus();
                break;
            case 2:
                View.edtSuburb.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtCounty.Focus();
                break;
            case 3:
                View.edtCounty.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtCountry.Focus();
                break;
            case 4:
                View.edtCountry.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtState.Focus();
                break;
            case 5:
                View.edtState.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtAdditional.Focus();
                break;
            case 15:
                View.edtAdditional.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtPolName.Focus();
                break;
            case 16:
                View.edtPolName.Text = Strings.Trim(View.ListBox1.SelectedItem.AsString().Left(240));
                View.ListBox1.Items.Clear();
                _ = View.edtZIP.Focus();
                break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                break;
        }
    }

    [RelayCommand]
    public void SearchNumber()
    {
        Modul1.Ubg = checked((int)Math.Round(Interaction.InputBox("Nummer des Ortes", "Suche eines Ortes nach Nummer").AsDouble()));
        ortles(0, Modul1.Ubg);
    }

    public void Button10_Click(object sender, EventArgs e)
    {
        View.Close();
    }

    public void Button16_Click(object sender, EventArgs e)
    {
        Modul1.Ubg = checked((int)Math.Round(Strings.Mid(View.Label13.Text, Modul1.IText[EUserText.t150].Length + 2, 10).AsDouble()));
        Modul1.sPKennz = "O";
        _ = MainProject.Forms.Bilder.ShowDialog();
        MainProject.Forms.Bilder.Close();
        ortles(0, Modul1.Ubg);
    }

    public void Textbox1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = Tastascii = checked((short)Strings.Asc(eventArgs.KeyChar));
        if ((View.ModifierKeys & Keys.Alt) != 0)
        {
            Tastascii = 0;
            return;
        }
        string name = ((TextBox)eventSender).Name;
        if (name == View.edtPlace.Name)
        {
            if (num == 13)
            {
                _ = View.edtSuburb.Focus();
            }
        }
        else if (name == View.edtSuburb.Name)
        {
            if (num == 13)
            {
                _ = View.edtCounty.Focus();
            }
        }
        else if (name == View.edtCounty.Name)
        {
            if (num == 13)
            {
                _ = View.edtCountry.Focus();
            }
        }
        else if (name == View.edtCountry.Name)
        {
            if (num == 13)
            {
                _ = View.edtState.Focus();
            }
        }
        else if (name == View.edtState.Name)
        {
            if (num == 13)
            {
                _ = View.edtLocator.Focus();
            }
        }
        else if (name == View.edtLocator.Name)
        {
            if (num == 13)
            {
                _ = View.edtGOV.Focus();
            }
        }
        else if (name == View.edtGOV.Name)
        {
            if (num == 13)
            {
                _ = View.edtLat1.Focus();
            }
        }
        else if (name == View.edtLat1.Name)
        {
            if (num == 13)
            {
                _ = View.edtLat2.Focus();
            }
        }
        else if (name == View.edtLat2.Name)
        {
            if (num == 13)
            {
                _ = View.edtLat3.Focus();
            }
        }
        else if (name == View.edtLat3.Name)
        {
            if (num == 13)
            {
                _ = View.edtLong1.Focus();
            }
        }
        else if (name == View.edtLong1.Name)
        {
            if (num == 13)
            {
                _ = View.edtLong2.Focus();
            }
        }
        else if (name == View.edtLong2.Name)
        {
            if (num == 13)
            {
                _ = View.edtLong3.Focus();
            }
        }
        else if (name == View.edtLong3.Name)
        {
            Locber();
            if (num == 13)
            {
                _ = View.edtAdditional.Focus();
            }
        }
        else if (name == View.edtAdditional.Name)
        {
            if (num == 13)
            {
                _ = View.edtPolName.Focus();
            }
        }
        else if (name == View.edtPolName.Name)
        {
            if (num == 13)
            {
                _ = View.edtZIP.Focus();
            }
        }
        else if (name == View.edtZIP.Name)
        {
            if (num == 13)
            {
                _ = View.TextBox17.Focus();
            }
        }
        else if (name == View.TextBox17.Name && num == 13)
        {
            _ = View.TextBox18.Focus();
        }
    }

    public void TextBox1_KeyUp(object sender, KeyEventArgs e)
    {
        //Discarded unreachable code: IL_020c, IL_0223, IL_0245
        if ((View.ModifierKeys & Keys.Alt) != 0)
        {
            Tastascii = 0;
        }
        if (Tastascii == 0)
        {
            return;
        }
        View.ListBox1.Visible = true;
        View.ListBox2.Visible = false;
        View.btnNext.Visible = false;
        View.btnPrev.Visible = false;
        View.btnShowPlaceGE.Visible = false;
        View.btnShowPlaceGM.Visible = false;
        View.btnLinkGOV.Visible = false;
        View.btnSearchGOV.Visible = false;
        View.btnConvertKoords.Visible = false;
        View.btnSearchName.Visible = false;
        View.btnSearchNumber.Visible = false;
        View.Button10.Visible = false;
        View.Button11.Visible = false;
        View.Button23.Visible = false;
        if (!View.Button19.Visible)
        {
            View.Button12.Visible = true;
        }
        View.Button17.Visible = true;
        switch (((TextBox)sender).TabIndex)
        {
            case 1:
                Modul1.eTKennz = ETextKennz.H_;
                break;
            case 2:
                Modul1.eTKennz = ETextKennz.I_;
                break;
            case 3:
                Modul1.eTKennz = ETextKennz.J_;
                break;
            case 4:
                Modul1.eTKennz = ETextKennz.K_;
                break;
            case 5:
                Modul1.eTKennz = ETextKennz.L_;
                break;
            case 6:
                if (View.edtLocator.Text.Length > 6)
                {
                    Interaction.Beep();
                    View.edtLocator.Text = View.edtLocator.Text.Left(6);
                    View.edtLocator.SelectionStart = 6;
                }
                else
                {
                    View.ListBox1.Items.Clear();
                }
                return;
            case 16:
                Modul1.eTKennz = ETextKennz.S_;
                View.ListBox1.Items.Clear();
                return;
            default:
                View.ListBox1.Items.Clear();
                return;
        }
        Modul1.UbgT = ((TextBox)sender).Text;
        if (Modul1.UbgT != "")
        {
            Textindex = checked((byte)((TextBox)sender).TabIndex);
            Textles(Modul1.eTKennz, Modul1.UbgT);
        }
    }

    [RelayCommand]
    private void SearchName()
    {
        View_Ortmaskleer();
        View.ListBox1.Visible = false;

        View.edtPlace.Visible = false;
        View.edtSuburb.Visible = false;
        View.edtCounty.Visible = false;
        View.edtCountry.Visible = false;
        View.edtState.Visible = false;
        View.TextBox30.Visible = true;

        View.Button11.Visible = false;
        View.btnNext.Visible = false;
        View.btnPrev.Visible = false;
        View.btnSearchNumber.Visible = false;
        View.btnShowPlaceGE.Visible = false;
        View.btnShowPlaceGM.Visible = false;
        View.btnLinkGOV.Visible = false;
        View.btnSearchGOV.Visible = false;
        View.btnConvertKoords.Visible = false;
        View.Button17.Visible = false;
        View.Button19.Visible = false;

        View.Label22.Visible = true;
        _ = View.TextBox30.Focus();
        if (View.TextBox30.Text != "")
        {
            Modul1.UbgT1 = View.TextBox30.Text;
            View.TextBox30.Text = "";
            View.TextBox30.Text = Modul1.UbgT1;
        }
        View.ListBox1.Visible = false;

        View.ListBox2.Visible = true;
    }

    public void ListBox2_DoubleClick(object sender, EventArgs e)
    {

        View.TextBox30.Visible = false;
        View.Label22.Visible = false;
        ProjectData.ClearProjectError();
        var ubg = View.ListBox2.SelectedItem.AsString().Right(10).AsInt();
        View.Button18.Visible = false;
        if (DataModul.Place.ReadData(ubg, out var cPlace))
        {
            View.edtPlace.Visible = true;
            View.edtSuburb.Visible = true;
            View.edtCounty.Visible = true;
            View.edtCountry.Visible = true;
            View.edtState.Visible = true;
            View.edtLocator.Visible = true;
            View.edtLat1.Visible = true;
            View.edtLong1.Visible = true;
            View.edtGOV.Visible = true;
            View.edtLat2.Visible = true;
            View.edtLat3.Visible = true;
            View.edtLong2.Visible = true;
            View.edtLong3.Visible = true;

            SetData(cPlace);
            Geoles(cPlace);
            Verwend = Ortverwend(cPlace.ID);
            View.Button18.Visible = true;
            View.Button11.Visible = true;
            View.btnNext.Visible = true;
            View.btnPrev.Visible = true;
            View.btnSearchNumber.Visible = true;
            View.btnConvertKoords.Visible = true;
            View.btnSearchGOV.Visible = true;
            View.Button10.Visible = true;
            View.btnShowPlaceGM.Visible = true;
        }
        else
        {
            _ = Interaction.MsgBox("Ort Nr.:" + ubg.AsString() + " nicht vorhanden");
        }
    }
    private void View_Ortmaskleer()
    {
        View.edtPlace.Text = "";
        View.edtSuburb.Text = "";
        View.edtCounty.Text = "";
        View.edtCountry.Text = "";
        View.edtState.Text = "";
        View.edtLocator.Text = "";
        View.edtGOV.Text = "";
        View_ClearGeoKoor();
        View.edtZIP.Text = "";
        View.edtAdditional.Text = "";
        View.edtPolName.Text = "";
        View.TextBox17.Text = "";
        View.TextBox18.Text = "";
        View.RTB1.Text = "";
    }

    public void TextBox30_TextChanged(object sender, EventArgs e)
    {
        if (View.TextBox30.Text == "")
        {
            return;
        }
        View.ListBox2.Items.Clear();
        Modul1.UbgT = View.TextBox30.Text.Trim();
        DataModul.DOSB_OrtSTable.Index = "OrtSu";
        DataModul.DOSB_OrtSTable.Seek(">=", Modul1.UbgT);
        var M1_Iter = 1;
        checked
        {
            while (!DataModul.DOSB_OrtSTable.EOF)
            {
                if (!DataModul.DOSB_OrtSTable.NoMatch)
                {
                    _ = View.ListBox2.Items.Add(((((DataModul.DOSB_OrtSTable.Fields["Name"].Value) + ("                                          "))) + (DataModul.DOSB_OrtSTable.Fields["Nr"].AsString())));
                }
                else
                {
                    _ = Interaction.MsgBox("Stop", title: "10", mb: MessageBoxButtons.OK);
                }
                DataModul.DOSB_OrtSTable.MoveNext();
                M1_Iter++;
                int i = M1_Iter;
                int num = 250;
                if (i > num)
                {
                    break;
                }
            }
        }
    }

    public void Button11_Click(object sender, EventArgs e)
    {
        View.btnNext.Visible = false;
        View.btnPrev.Visible = false;
        View.btnShowPlaceGE.Visible = false;
        View.btnShowPlaceGM.Visible = false;
        View.btnLinkGOV.Visible = false;
        View.btnSearchGOV.Visible = false;
        View.btnConvertKoords.Visible = false;
        View.btnSearchName.Visible = false;
        View.btnSearchNumber.Visible = false;
        View.Button10.Visible = false;
        View.Button11.Visible = false;

        View.Button23.Visible = false;

        View.ListBox2.Visible = false;

        View.ListBox1.Visible = true;

        View.Button12.Visible = true;
        View.Button17.Visible = true;
        View_Ortmaskleer();
        DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OrtNr);
        if (DataModul.Place.Count > 0)
        {
            DataModul.DB_PlaceTable.MoveLast();
            View.Label13.Text = Modul1.IText[EUserText.t150] + ":" + Conversion.Str(DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt() + 1);
        }
        else
        {
            View.Label13.Text = Modul1.IText[EUserText.t150] + ": 1";
        }
        _ = View.edtPlace.Focus();
    }

    public bool Ortverwend(int iPlaceNr)
    {
        DataModul.DB_EventTable.Index = nameof(EventIndex.EOrt);
        DataModul.DB_EventTable.Seek("=", iPlaceNr);
        if (!DataModul.DB_EventTable.NoMatch)
        {
            return true;
        }
        DataModul.DB_OFBTable.Index = "Indnum";
        DataModul.DB_OFBTable.Seek("=", iPlaceNr);
        if (!DataModul.DB_OFBTable.EOF
            && !DataModul.DB_OFBTable.NoMatch
            && DataModul.DB_OFBTable.Fields[OFBFields.Kennz].AsString() == "OO")
        {
            return true;
        }
        return false;
    }

    public void OrtverwendA()
    {
        Verwend = true;
        var flag = false;
        foreach (var cPlace in DataModul.Place.ReadAll(PlaceIndex.OrtNr))
        {

            Verwend = true;
            if (!DataModul.Event.Exists(EventIndex.EOrt, cPlace.ID))
            {
                Verwend = false;
                DataModul.DB_OFBTable.Index = "Indnum";
                while (!DataModul.DB_OFBTable.EOF)
                {
                    DataModul.DB_OFBTable.Seek("=", cPlace.ID);
                    if (DataModul.DB_OFBTable.NoMatch)
                    {
                        Verwend = false;
                        break;
                    }
                    else if (DataModul.DB_OFBTable.Fields[OFBFields.Kennz].AsString() == "OO")
                    {
                        Verwend = true;
                        break;
                    }
                }
                if (!Verwend)
                {
                    SetData(cPlace);
                    Geoles(cPlace);
                    short num5 = checked((short)Interaction.MsgBox("Ort wird nicht verwendet, Löschen?", mb: MessageBoxButtons.YesNoCancel));
                    switch (num5)
                    {
                        case 6:
                            DataModul.Place.Delete(cPlace.ID);
                            break;
                        case 2:
                            View.Close();
                            goto end_IL_0001_3;
                        default:
                            break;
                    }
                }
            }
        }
        if (flag)
        {
            _ = Interaction.MsgBox("Keine weiteren nichtverwendeten Orte vorhanden!");
            goto end_IL_0001_3;
        }
        _ = Interaction.MsgBox("Keine nichtverwendeten Orte vorhanden!");
    end_IL_0001_3: // <========== 3
        return;
    }

    public void Locber()
    {
        float num = 1f;
        if (View.edtLat1.Text.Trim() == "")
        {
            num = 0f;
        }
        if (View.edtLat2.Text.Trim() == "")
        {
            num = 0f;
        }
        if (View.edtLong1.Text.Trim() == "")
        {
            num = 0f;
        }
        if (View.edtLong2.Text.Trim() == "")
        {
            num = 0f;
        }
        if (num == 0f)
        {
            _ = Interaction.MsgBox("Eingabe unvollständig, berechenen des Locatorcodes nicht möglich");
            return;
        }
        float num2 = View.edtLat1.Text.AsInt();
        float num3 = View.edtLat2.Text.AsInt();
        float num4 = View.edtLat3.Text.AsInt();
        if (num2 < 0f)
        {
            num3 *= -1f;
            num4 *= -1f;
        }
        int num5 = checked((int)Math.Round(3600f * num2 + 60f * num3 + num4));
        if (num5 is > 324000 or < (-324000))
        {
            _ = Interaction.MsgBox("Falsche Eingabe");
        }
        float num6 = View.edtLong1.Text.AsInt();
        float num7 = View.edtLong2.Text.AsInt();
        float num8 = View.edtLong3.Text.AsInt();
        if (num6 < 0f)
        {
            num7 *= -1f;
            num8 *= -1f;
        }
        float num9 = 3600f * num6 + 60f * num7 + num8;
        if (num9 is > 648000f or < (-648000f))
        {
            _ = Interaction.MsgBox("Falsche Eingabe");
        }
        checked
        {
            float num10 = 324000 + num5;
            float number = num10 / 36000f;
            float num11 = (float)Math.Floor(number);
            var Modul1_An = (int)Math.Round(65f + num11);
            string text = Strings.Chr(Modul1_An).AsString();
            float num12 = 648000f + num9;
            float number2 = num12 / 72000f;
            float num13 = (float)Math.Floor(number2);
            float num14 = 65f + num13;
            string text2 = Strings.Chr((int)Math.Round(num14)).AsString();
            float num15 = (num12 - 648000f) / 72000f;
            num15 = (num15 - (float)Math.Floor(num15)) * 10f;
            float num16 = (float)Math.Floor(num15);
            float num17 = (num10 - 324000f) / 36000f;
            num17 = (num17 - (float)Math.Floor(num17)) * 10f;
            float num18 = (float)Math.Floor(num17);
            float num19 = num15 - (float)Math.Floor(num15);
            float number3 = 24f * num19;
            number3 = (float)Math.Floor(number3);
            float num20 = 65f + number3;
            string text3 = Strings.Chr((int)Math.Round(num20)).AsString();
            float num21 = num17 - (float)Math.Floor(num17);
            num11 = 24f * num21;
            num11 = Conversion.Int(num11);
            float num22 = 65f + num11;
            string text4 = Strings.Chr((int)Math.Round(num22)).AsString();
            if (View.edtLocator.Text.Trim() == "")
            {
                View.edtLocator.Text = text2 + text + num16.AsString().TrimStart() + num18.AsString().TrimStart() + text3 + text4;
            }
            else if (View.edtLocator.Text != text2 + text + num16.AsString().TrimStart() + num18.AsString().TrimStart() + text3 + text4)
            {
                string text5 = "Errechneter Locator-Code ";
                text5 = text5 + "\r" + text2 + text + num16.AsString().TrimStart() + num18.AsString().TrimStart() + text3 + text4;
                text5 += "\r weicht vom eingetragenen ab";
                text5 += "\r\rErechneten Wert übernehmen?";
                var Modul1_Value = Interaction.MsgBox(text5, title: "", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question);
                if (Modul1_Value == DialogResult.Yes)
                {
                    View.edtLocator.Text = text2 + text + num16.AsString().TrimStart() + num18.AsString().TrimStart() + text3 + text4;
                    View.Button22.PerformClick();
                }
            }
        }
    }

    public void Button19_Click(object sender, EventArgs e)
    {
        int num4;
        var xAddnew = DataModul.Place.ReadData(View.Label13.Tag.AsInt(), out var cPlace);
        if (xAddnew)
        {
            cPlace = DataModul.Place.CreateNew();
        }
        //Discarded unreachable code: IL_0cff
        byte b2 = 0;
        cPlace.iOrt = SaveText(View.edtPlace.Text, ETextKennz.H_, ref b2);
        cPlace.iOrtsteil = SaveText(View.edtSuburb.Text, ETextKennz.I_, ref b2);
        cPlace.iKreis = SaveText(View.edtCounty.Text, ETextKennz.J_, ref b2);
        cPlace.iLand = SaveText(View.edtCountry.Text, ETextKennz.K_, ref b2);
        cPlace.iStaat = SaveText(View.edtState.Text, ETextKennz.L_, ref b2);

        if (b2 == 0)
        {
            Modul1.UbgT = "";
            Modul1.Ubg = 0;
            cPlace.ClearChangedProps();
        }
        else
        {
            if (View.edtPolName.Text != "")
            {
                Modul1.UbgT = View.edtPolName.Text.TrimEnd();
                Modul1.eTKennz = ETextKennz.S_;
                Modul1.Ubg = Modul1.TextSpeich(Modul1.UbgT, Modul1.UbgT1, Modul1.eTKennz, Modul1.PersInArb, Modul1.LfNR);
                cPlace.sPolName = Modul1.Ubg.AsString();
            }
            else
            {
                cPlace.sPolName = "0";
            }
            cPlace.sLoc = View.edtLocator.Text.ToUpper().Trim();
            cPlace.sL = View.edtLong1.Text.Trim().PadLeft(4) + "," + View.edtLong2.Text.Trim().PadLeft(2) + View.edtLong3.Text.Trim().PadLeft(2);
            cPlace.sB = View.edtLat1.Text.Trim().PadLeft(4) + "," + View.edtLat2.Text.Trim().PadLeft(2) + View.edtLat3.Text.Trim().PadLeft(2);
            cPlace.sTerr = Strings.UCase(View.TextBox17.Text.Trim().Left(3));
            cPlace.sStaatk = Strings.UCase(View.TextBox18.Text.Trim().Left(3));
            cPlace.sZusatz = View.edtAdditional.Text.Trim().Left(10);
            if (View.edtPolName.Text.Trim() == "")
            {
                View.edtPolName.Text = " ";
            }
            if (View.edtZIP.Text.TrimEnd() == "")
            {
                View.edtZIP.Text = "0";
            }
            cPlace.sPLZ = View.edtZIP.Text.TrimEnd();
            View.RTB1.Text = View.RTB1.Text.Trim();
            if (View.RTB1.Text == "")
            {
                View.RTB1.Text = " ";
            }
            cPlace.sBem = View.RTB1.Text;

            Modul1_OrtNr = View.Label13.Tag.AsInt();
            if (Modul1_OrtNr != 0)
            {
                DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].Value = Modul1_OrtNr;
            }
            View.edtGOV.Text = View.edtGOV.Text.ToUpper();
            cPlace.sGOV = View.edtGOV.Text.Trim();
            DataModul.Place.Commit(cPlace);

            var sPrefix = View.edtSuburb.Text.Trim() != "" ? "-" + View.edtSuburb.Text.Trim() + " " : " ";
            string sPlace = View.edtPlace.Text.Trim() + sPrefix + View.edtCounty.Text.Trim() + " " + View.edtCountry.Text.Trim() + " " + View.edtState.Text.Trim();

            DataModul_DOSB_OrtsTable_SetData(Modul1_OrtNr, sPlace);
            Modul1.UbgT = sPlace;
            Modul1.Ubg = Modul1_OrtNr;
        }

        View.Close();
        Views.FrmEreignis ereignis = MainProject.Forms.Ereignis;
        ereignis.TextBox5.Tag = Modul1_OrtNr;
        ereignis.TextBox5.Text = Modul1.UbgT;
        ereignis.ListBox1.Visible = false;
        ereignis.ListBox2.Visible = false;
        ereignis.ListBox4.Visible = false;
        ereignis.Button13.Visible = false;
        ereignis.ListBox3.Visible = false;
        ereignis.Button14.Visible = false;
        ereignis.Label8.Visible = false;
        Modul1.UbgT = "";
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 5
        return;
    }

    private static void DataModul_DOSB_OrtsTable_SetData(int Modul1_OrtNr, string sPlace)
    {
        DataModul.DOSB_OrtSTable.Index = "OrtNr";
        DataModul.DOSB_OrtSTable.Seek("=", Modul1_OrtNr);
        if (DataModul.DOSB_OrtSTable.NoMatch)
        {
            DataModul.DOSB_OrtSTable.AddNew();
        }
        else
        {
            DataModul.DOSB_OrtSTable.Edit();
        }
        DataModul.DOSB_OrtSTable.Fields["Name"].Value = sPlace;
        DataModul.DOSB_OrtSTable.Fields["Nr"].Value = Modul1_OrtNr;
        DataModul.DOSB_OrtSTable.Update();
    }

    private void Handle_Err3022_2(int num2)
    {
        int num5 = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
        int num6 = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
        int num7 = DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt();
        int num8 = DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt();
        int num9 = DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt();
        int num10 = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
        if (Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString()) != "")
        {
            _ = Interaction.MsgBox("Dieser Ort ist schon vorhanden. Der zu löschende (neue)Ort enthält Bemerkungen. Diese würden gelöscht. Aus Sicherheitsgründen wird die Aktion abgebrochen.");
            return;
        }
        else
        {
            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.Orte);
            DataModul.DB_PlaceTable.Seek("=", num5, num6, num7, num8, num9);
            if (num10 == DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt())
            {
                Debugger.Break();
                return;
            }
            else if (!DataModul.DB_PlaceTable.NoMatch && DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt() == num5 && DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() == num6 && DataModul.DB_PlaceTable.Fields[PlaceFields.Kreis].AsInt() == num7 && DataModul.DB_PlaceTable.Fields[PlaceFields.Land].AsInt() == num8 && DataModul.DB_PlaceTable.Fields[PlaceFields.Staat].AsInt() == num9)
            {
                int ubg = DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt();
                Modul1.Ubg = ubg;
                ortles(0, Modul1.Ubg);
                Modul1_OrtNr = View.Label13.Tag.AsInt();
                ProjectData.ClearProjectError();
                if (num2 == 0)
                {
                    throw ProjectData.CreateProjectError(-2146828268);
                }
            }
        }

    }

    public void Button17_Click(object sender, EventArgs e)
    {
        checked
        {
            if (!View.Button19.Visible)
            {
                Modul1.Ubg = (int)Math.Round(Strings.Mid(View.Label13.Text, Modul1.IText[EUserText.t150].Length + 2, 10).AsDouble());
                Modul1.Schalt = (byte)Math.Round(Strings.Mid(View.Label13.Text, Modul1.IText[EUserText.t150].Length + 2, 10).AsDouble());
                View_Ortmaskleer();
                View.btnNext.Visible = true;
                View.btnPrev.Visible = true;
                View.btnShowPlaceGE.Visible = true;
                View.btnShowPlaceGM.Visible = true;
                View.btnLinkGOV.Visible = true;
                View.btnSearchGOV.Visible = true;
                View.btnConvertKoords.Visible = true;
                View.btnSearchName.Visible = true;
                View.btnSearchNumber.Visible = true;
                View.Button10.Visible = true;
                View.Button11.Visible = true;
                View.Button21.Visible = true;
                View.Button23.Visible = true;
                if (Directory.Exists(WinPath + "\\Microsoft.net\\Framework\\v3.5"))
                {
                    View.Button21.Visible = true;
                }
                View.Button12.Visible = false;
                View.Button17.Visible = false;
                Nr = (int)Math.Round(Strings.Mid(View.Label13.Text, Modul1.IText[EUserText.t150].Length + 2, 10).AsDouble());
                ortles(0, Modul1.Ubg);
            }
            else
            {
                View.Close();
            }
        }
    }

    public void Button18_Click(object sender, EventArgs e)
    {
        DataModul.DOSB_OrtSTable.Index = "OrtNr";
        checked
        {
            DataModul.DOSB_OrtSTable.Seek("=", View.Label13.Tag.AsInt());
            if (!DataModul.DOSB_OrtSTable.NoMatch 
                && (DataModul.DOSB_OrtSTable.Fields["Nr"].AsInt() == View.Label13.Tag.AsInt()))
            {
                DataModul.DOSB_OrtSTable.Delete();
            }
            DataModul.DB_PlaceTable.Index = nameof(PlaceIndex.OrtNr);
            DataModul.DB_PlaceTable.Seek("=", View.Label13.Tag.AsInt());
            if (!DataModul.DB_PlaceTable.NoMatch 
                && (DataModul.DB_PlaceTable.Fields[PlaceFields.OrtNr].AsInt() == View.Label13.Tag.AsInt()))
            {
                DataModul.DB_PlaceTable.Delete();
            }
            if (View.ListBox2.Visible)
            {
                View.ListBox2.Items.Clear();
                Modul1.UbgT = View.TextBox30.Text.Trim();
                DataModul.DOSB_OrtSTable.Index = "OrtSu";
                DataModul.DOSB_OrtSTable.Seek(">=", Modul1.UbgT);
                var M1_Iter = 1;
                while (M1_Iter <= 250
                    && !DataModul.DOSB_OrtSTable.EOF)
                {
                    if (!DataModul.DOSB_OrtSTable.NoMatch)
                    {
                        _ = View.ListBox2.Items.Add(((((DataModul.DOSB_OrtSTable.Fields["Name"].Value) + ("                                          "))) + (DataModul.DOSB_OrtSTable.Fields["Nr"].AsString())));
                    }
                    else
                    {
                        _ = Interaction.MsgBox("Stop", title: "10", mb: MessageBoxButtons.OK);
                    }
                    DataModul.DOSB_OrtSTable.MoveNext();
                    M1_Iter++;
                }
            }
            ortles(0, Modul1.Ubg);
        }
    }

    public void RTB1_KeyUp(object sender, KeyEventArgs e)
    {
        View.btnNext.Visible = false;
        View.btnPrev.Visible = false;
        View.btnShowPlaceGE.Visible = false;
        View.btnShowPlaceGM.Visible = false;
        View.btnLinkGOV.Visible = false;
        View.btnSearchGOV.Visible = false;
        View.btnConvertKoords.Visible = false;
        View.btnSearchName.Visible = false;
        View.btnSearchNumber.Visible = false;
        View.Button10.Visible = false;
        View.Button11.Visible = false;
        View.Button21.Visible = false;
        View.Button23.Visible = false;
        if (!View.Button19.Visible)
        {
            View.Button12.Visible = true;
        }
        View.Button17.Visible = true;
    }

    public void Button20_Click(object sender, EventArgs e)
    {
        OrtverwendA();
    }

    public void Button21_Click(object sender, EventArgs e)
    {
        string adrString = View.edtPlace.Text + " " + View.edtSuburb.Text + " " + View.edtCountry.Text + " " + View.edtState.Text;
        Geocode geocode = GeocodeAdress(adrString);
        Modul1.UbgT = geocode.Longitude;
        checked
        {
            if (Modul1.UbgT != "")
            {
                Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ".");
                string text;
                if (Modul1_aByte > 0)
                {
                    Rechnen(Modul1.UbgT);
                    text = Strings.Right("   " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 3) + "°";
                    Modul1_Ba1 = text + Modul1_Ba3.Trim() + "' " + Modul1_Ba5.Trim();
                }
                Modul1.UbgT = geocode.Latitude;
                Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ".");
                if (Modul1_aByte > 0)
                {
                    Rechnen(Modul1.UbgT);
                    text = Strings.Right("   " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 3) + "°";
                    Modul1_Ba2 = text + Modul1_Ba3.Trim() + "' " + Modul1_Ba5.Trim();
                }
                if (Interaction.MsgBox("B:" + Modul1_Ba2 + "   L:" + Modul1_Ba1, title: "Ermittelte Koordinaten übernehmen?", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    View.edtLat1.Text = Modul1_Ba2.Left(3);
                    View.edtLat2.Text = Strings.Mid(Modul1_Ba2, 5, 2);
                    View.edtLat3.Text = Strings.Mid(Modul1_Ba2, 9, 2);
                    View.edtLong1.Text = Modul1_Ba1.Left(3);
                    View.edtLong2.Text = Strings.Mid(Modul1_Ba1, 5, 2);
                    View.edtLong3.Text = Strings.Mid(Modul1_Ba1, 9, 2);
                    Geoles1();
                    View.btnShowPlaceGE.Visible = true;
                    View.Button22.PerformClick();
                }
            }
            else
            {
                _ = Interaction.MsgBox(" Koordinaten konnten nicht ermittelt werden, zu wenig oder falsche Angaben");
            }
        }
    }

    public void Geoles1()
    {
        string text = View.edtPlace.Text.Trim();
        if (View.edtSuburb.Text.Trim() != "")
        {
            text = text + "-" + View.edtSuburb.Text.Trim();
        }

        string text3 = View.edtLat1.Text.Trim() + ".";
        float num = (float)(View.edtLat2.Text.AsInt() / 60.0 * 100.0);
        float num2 = (float)(View.edtLat3.Text.AsInt() / 3600.0 * 100.0);
        float num3 = num + num2;
        num3 *= 100f;
        checked
        {
            num3 = (short)Math.Round(num3);
            string text4 = Strings.Format(num3, "####0000");
            text3 += text4;
            string text2 = View.edtLong1.Text.Trim() + ".";
            num = (float)(View.edtLong2.Text.AsInt() / 60.0 * 100.0);
            num2 = (float)(View.edtLong3.Text.AsInt() / 3600.0 * 100.0);
            num3 = num + num2;
            num3 *= 100f;
            num3 = (short)Math.Round(num3);
            text4 = Strings.Format(num3, "####0000");
            text2 += text4;
            Modul1.GEExportPlace(text, (text2, text3), false);
        }
    }

    public Geocode GeocodeAdress(string adrString, string delim = ",")
    {
        int num = 0;
        Geocode result = default;
        while (num <= 10)
        {
            string geocodeXMLAddress = Modul1.sGeocodeXMLAddress;
            if (adrString == "")
            {
                return result;
            }
            XDocument xDocument = XDocument.Load(string.Format(geocodeXMLAddress, $"+{Strings.Replace(adrString, delim, "+,")}"));
            if (Strings.InStr((string)xDocument.Root, "OK") != 0)
            {
                XElement xElement = xDocument.Descendants("location").First();
                result.Latitude = xElement.Descendants().First().Value;
                result.Longitude = xElement.Descendants().Last().Value;
                return result;
            }
            num++;
        }
        return result;
    }

    public void Rechnen(string Ubgt)
    {
        Modul1_La1 = Ubgt.AsString();
        Modul1_aByte = (byte)Strings.InStr(Modul1_La1, ".");
        Modul1_La2 = Strings.Mid(Modul1_La1, unchecked(Modul1_aByte) + 1, Modul1_La1.Length);
        int Modul1_La5 = default;
        if (Modul1_La2.AsInt() > 0.0)
        {
            Modul1_La2 = Modul1_La2 + "0000".Left(4);
            Modul1_La3 = (float)(Modul1_La2.AsDouble() / 10000.0 * 60.0);
            var Modul1_La4 = Modul1_La3 - Conversion.Int(Modul1_La3);
            Modul1_La3 = Conversion.Int(Modul1_La3);
            Modul1_La5 = (int)Conversion.Int(Modul1_La4 / 100f * 6000f);
        }
        Modul1_Ba3 = Strings.Format(Modul1_La3, "00");
        Modul1_Ba5 = Strings.Format(Modul1_La5, "00");

    }

    public void TextBox13_TextChanged(object sender, EventArgs e)
    {
        if (View.edtLong3.Text.Trim().Length == 2)
        {
            Locber();
        }
    }

    public void Button22_Click(object sender, EventArgs e)
    {
        View.ListBox1.Visible = true;
        View.ListBox2.Visible = false;
        View.btnNext.Visible = false;
        View.btnPrev.Visible = false;
        View.btnShowPlaceGM.Visible = false;
        View.btnLinkGOV.Visible = false;
        View.btnSearchGOV.Visible = false;
        View.btnConvertKoords.Visible = false;
        View.btnSearchName.Visible = false;
        View.btnSearchNumber.Visible = false;
        View.Button10.Visible = false;
        View.Button11.Visible = false;
        if (!View.Button19.Visible)
        {
            View.Button12.Visible = true;
        }
        View.Button17.Visible = true;
    }

    public void TextBox31_TextChanged(object sender, EventArgs e)
    {
        if (View.TextBox31.Text == "")
        {
            return;
        }
        View.ListBox3.Items.Clear();
        var num = 0;
        Modul1.UbgT = View.TextBox31.Text.Trim();
        DataModul_OSBOrtS_ForeachIdxC("OrtSu", Modul1.UbgT, (i, s) => Filter_ByKoordinateB(i, s, (i, s)
            => View.ListBox3.Items.Add(new ListItem<int>($"{s,-42} {i}", i))) && num++ < 256);
        View.Label25.Text = (num - View.ListBox3.Items.Count).AsString() + " Orte wegen fehlender  Koordinaten übersprungen";
    }

    public void TextBox32_TextChanged(object sender, EventArgs e)
    {
        if (View.TextBox32.Text == "")
        {
            return;
        }
        View.ListBox4.Items.Clear();
        var num = 0;
        Modul1.UbgT = View.TextBox32.Text.Trim();
        DataModul_OSBOrtS_ForeachIdxC("OrtSu", Modul1.UbgT, (i, s) => Filter_ByKoordinateB(i, s, (i, s)
            => View.ListBox4.Items.Add(new ListItem<int>($"{s,-42} {i}", i))) && num++ < 256);
        View.Label35.Text = (num - View.ListBox4.Items.Count).AsString() + " Orte wegen fehlender  Koordinaten übersprungen";
    }


    private void DataModul_OSBOrtS_ForeachIdxC(string sIdx, string sSrch, Func<int, string, bool> Handle_OrtName)
    {
        GenFree.Interfaces.DB.IRecordset dOSB_OrtSTable = DataModul.DOSB_OrtSTable;
        dOSB_OrtSTable.Index = sIdx;
        dOSB_OrtSTable.Seek(">=", sSrch);
        while (!dOSB_OrtSTable.EOF && !dOSB_OrtSTable.NoMatch)
        {
            int iOrt = dOSB_OrtSTable.Fields["Nr"].AsInt();
            string sName = dOSB_OrtSTable.Fields["Name"].AsString();

            if (!Handle_OrtName(iOrt, sName))
                break;

            dOSB_OrtSTable.MoveNext();
        }
    }

    private bool Filter_ByKoordinateB(int iOrt, string sName, Action<int, string> AddNewMethod)
    {
        GenFree.Interfaces.DB.IRecordset dB_PlaceTable = DataModul.DB_PlaceTable;
        dB_PlaceTable.Seek("=", iOrt);
        if (!dB_PlaceTable.NoMatch)
        {
            string sOrt_B = dB_PlaceTable.Fields[PlaceFields.B].AsString();
            if (sOrt_B.Length > 1)
            {
                AddNewMethod(iOrt, sName);
            }
        }
        return true;
    }


    public void ListBox3_DoubleClick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_059c
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            View.Label28.Text = "";
                            goto IL_0014;
                        case 1707:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 1:
                                        break;
                                    default:
                                        goto end_IL_0001;
                                }
                                int num4 = unchecked(num2 + 1);
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 58:
                                    case 59:
                                    case 60:
                                    case 61:
                                    case 62:
                                        goto end_IL_0001_2;
                                    case 63:
                                        goto end_IL_0001_3;
                                }
                                goto default;
                            }
                        end_IL_0001_2:
                            break;
                        IL_0014:
                            num = 2;
                            View.Label29.Text = "";
                            View.Label33.Text = "";
                            ProjectData.ClearProjectError();
                            num3 = -2;
                            DataModul.DB_PlaceTable.Seek("=", View.ListBox3.SelectedItem.AsString().Right(10));
                            if (DataModul.DB_PlaceTable.NoMatch)
                            {
                                break;
                            }
                            string Place_Koord_L = Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.L].AsString());
                            string Place_Koord_B = Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.B].AsString());
                            if (Place_Koord_B.Trim().Length > 1)
                            {
                                Place_Koord_B = Strings.Replace(Place_Koord_B, ";", ",");
                                Place_Koord_B = Strings.Replace(Place_Koord_B, ".", ",");
                                Modul1.UbgT = Place_Koord_B;
                                if (Modul1.UbgT.Left(1) == "-")
                                {
                                    Modul1.UbgT1 = "-";
                                    Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
                                }
                                else
                                {
                                    Modul1.UbgT1 = "";
                                }
                                if (Modul1.UbgT.Left(1) == "+")
                                {
                                    Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
                                }
                                if (Strings.InStr(Modul1.UbgT, ",") != 0)
                                {
                                    Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ",");
                                    Modul1.UbgT1 = Modul1.UbgT1 + Strings.Trim(Strings.Right("    " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 4)) + ".";
                                    Modul1.UbgT = Strings.Mid(Modul1.UbgT, unchecked(Modul1_aByte) + 1, Modul1.UbgT.Length);
                                    A4 = Modul1.DezRechnen(A4, ubgT: Modul1.UbgT);
                                    Modul1.UbgT1 += A4.Trim();
                                    if (Modul1.UbgT1.Trim().Length > 1)
                                    {
                                        View.Label29.Text = Modul1.UbgT1.Trim();
                                        Modul1.UbgT1 = "";
                                    }
                                }
                            }
                            if (Place_Koord_L.Trim().Length <= 1)
                            {
                                break;
                            }
                            Place_Koord_L = Strings.Replace(Place_Koord_L, ";", ",");
                            Place_Koord_L = Strings.Replace(Place_Koord_L, ".", ",");
                            Modul1.UbgT = Place_Koord_L;
                            if (Modul1.UbgT.Left(1) == "-")
                            {
                                Modul1.UbgT1 = "-";
                                Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
                            }
                            else
                            {
                                Modul1.UbgT1 = "";
                            }
                            if (Modul1.UbgT.Left(1) == "+")
                            {
                                Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
                            }
                            if (Strings.InStr(Modul1.UbgT, ",") == 0)
                            {
                                break;
                            }
                            Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ",");
                            Modul1.UbgT1 = Modul1.UbgT1 + Strings.Trim(Strings.Right("    " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 4)) + ".";
                            Modul1.UbgT = Strings.Mid(Modul1.UbgT, unchecked(Modul1_aByte) + 1, Modul1.UbgT.Length);
                            A4 = Modul1.DezRechnen(A4, ubgT: Modul1.UbgT);
                            Modul1.UbgT1 += A4.Trim();
                            if (Modul1.UbgT1.Trim().Length <= 1)
                            {
                                break;
                            }
                            View.Label28.Text = Modul1.UbgT1.Trim();
                            Modul1.UbgT1 = "";
                            break;
                    }
                    num = 62;
                    View.Label27.Text = "Gewählter Ort\n" + View.ListBox3.SelectedItem.AsString().Left(-10);
                    break;
                }
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1707;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_3:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void ListBox4_DoubleClick(object sender, EventArgs e)
    {

        View.Label30.Text = "";

        View.Label31.Text = "";
        View.Label33.Text = "";
        ProjectData.ClearProjectError();
        DataModul.DB_PlaceTable.Seek("=", View.ListBox4.SelectedItem.AsString().Right(10));
        if (DataModul.DB_PlaceTable.NoMatch)
        {
            return;
        }
        var Place_Koord_L = Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.L].AsString());
        var Place_Koord_B = Strings.Trim(DataModul.DB_PlaceTable.Fields[PlaceFields.B].AsString());
        if (Place_Koord_B.Trim().Length > 1)
        {
            Place_Koord_B = Strings.Replace(Place_Koord_B, ";", ",");
            Place_Koord_B = Strings.Replace(Place_Koord_B, ".", ",");
            Modul1.UbgT = Place_Koord_B;
            if (Modul1.UbgT.Left(1) == "-")
            {
                Modul1.UbgT1 = "-";
                Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
            }
            else
            {
                Modul1.UbgT1 = "";
            }
            if (Modul1.UbgT.Left(1) == "+")
            {
                Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
            }
            if (Strings.InStr(Modul1.UbgT, ",") != 0)
            {
                Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ",");
                Modul1.UbgT1 = Modul1.UbgT1 + Strings.Trim(Strings.Right("    " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 4)) + ".";
                Modul1.UbgT = Strings.Mid(Modul1.UbgT, unchecked(Modul1_aByte) + 1, Modul1.UbgT.Length);
                A4 = Modul1.DezRechnen(A4, ubgT: Modul1.UbgT);
                Modul1.UbgT1 += A4.Trim();
                if (Modul1.UbgT1.Trim().Length > 1)
                {
                    View.Label31.Text = Modul1.UbgT1.Trim();
                    Modul1.UbgT1 = "";
                }
            }
        }
        if (Place_Koord_L.Trim().Length <= 1)
        {
            return;
        }
        Place_Koord_L = Strings.Replace(Place_Koord_L, ";", ",");
        Place_Koord_L = Strings.Replace(Place_Koord_L, ".", ",");
        Modul1.UbgT = Place_Koord_L;
        if (Modul1.UbgT.Left(1) == "-")
        {
            Modul1.UbgT1 = "-";
            Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
        }
        else
        {
            Modul1.UbgT1 = "";
        }
        if (Modul1.UbgT.Left(1) == "+")
        {
            Modul1.UbgT = Strings.Mid(Modul1.UbgT, 2, Modul1.UbgT.Length);
        }
        if (Strings.InStr(Modul1.UbgT, ",") == 0)
        {
            return;
        }
        Modul1_aByte = (byte)Strings.InStr(Modul1.UbgT, ",");
        Modul1.UbgT1 = Modul1.UbgT1 + Strings.Trim(Strings.Right("    " + Modul1.UbgT.Left(unchecked(Modul1_aByte) - 1), 4)) + ".";
        Modul1.UbgT = Strings.Mid(Modul1.UbgT, unchecked(Modul1_aByte) + 1, Modul1.UbgT.Length);
        A4 = Modul1.DezRechnen(A4, ubgT: Modul1.UbgT);
        Modul1.UbgT1 += A4.Trim();
        if (Modul1.UbgT1.Trim().Length <= 1)
        {
            return;
        }
        View.Label30.Text = Modul1.UbgT1.Trim();
        Modul1.UbgT1 = "";
    //    }
    //    num = 62;
    //    View.Label32.Text = "Gewählter Ort\n" + View.ListBox4.SelectedItem.AsString().Left(-10);
    //    break;
    //}
    end_IL_0001_3:
        ;
    }
    public void Button23_Click(object sender, EventArgs e)
    {
        View.Panel1.Location = new Point(1, 3);
        View.Panel1.Visible = true;
        _ = View.TextBox31.Focus();
    }

    public void Button24_Click(object sender, EventArgs e)
    {
        View.Panel1.Visible = false;
    }

    public void Label27_TextChanged(object sender, EventArgs e)
    {
        if ((View.Label29.Text != "") & (View.Label28.Text != "") & (View.Label30.Text != "") & (View.Label31.Text != ""))
        {
            var instance = Strings.Split(View.Label27.Text, "\n");
            var instance2 = Strings.Split(View.Label32.Text, "\n");
            View.Label33.Text = "Die Luflinien-Entfernung zwischen " + Strings.Trim(
                instance[0].AsString()) + " und " + Strings.Trim(
                    instance2[0].AsString()) + " beträgt \n\n" + 
                    Conversion.Str(Math.Floor(PlaceHelpers.CalcDistance(View.Label28.Text.AsInt(), View.Label29.Text.AsInt(), View.Label30.Text.AsInt(), View.Label31.Text.AsInt()))) + " Km";
        }
    }

    public void Label32_TextChanged(object sender, EventArgs e)
    {
        if ((View.Label29.Text != "") & (View.Label28.Text != "") & (View.Label30.Text != "") & (View.Label31.Text != ""))
        {
            var instance = Strings.Split(View.Label27.Text, "\n");
            var instance2 = Strings.Split(View.Label32.Text, "\n");
            View.Label33.Text = "Die Luftlinien-Entfernung zwischen " + Strings.Trim(
                instance[0].AsString()) + " und " + Strings.Trim(
                    instance2[0].AsString()) + " beträgt \n\n" + 
                    Conversion.Str(Math.Floor(PlaceHelpers.CalcDistance(View.Label28.Text.AsInt(), View.Label29.Text.AsInt(), View.Label30.Text.AsInt(), View.Label31.Text.AsInt()))) + " Km";
        }
    }
}
