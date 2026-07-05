using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel : BaseViewModelCT, INamenSuchViewModel
{
    private static EventHandler IdleEvent;
    public bool EreiRf;
    public bool Scheid;
    private int An;
    private short BemSch;
    private short ID;
    private string[] KontSP;
    private string[] KontSP1;
    private short LfNR;
    private int Fambehk;
    private int I1;
    private int A;
    private int Z;
    private int U;
    private int PersSp;
    private string Namen;
    private string Kennzt;
    private int[] Vorn;
    private string[] Ruf;
    private int privaus;
    private short Beruf;
    // 
    private IList<string> asOption = new string[50]; // korresponding Enum: EOutCfg
    private BoolProxy<EOutCfg, string> Option;
    private DateTime Datu;
    IModul1 Modul1;
    IGenPersistence Persistence => Modul1.Persistence;
    [Obsolete]
    IProjectData ProjectData => Modul1.ProjectData;
    IInteraction Interaction => Menue.Default;
    [Obsolete]
    IVBInformation Information => Modul1.Information;
    [Obsolete]
    IStrings Strings => Modul1.Strings;

    [ObservableProperty]
    public partial bool Male_Checked { get; set; }

    [ObservableProperty]
    public partial bool Females_Checked { get; set; }

    [ObservableProperty]
    public partial bool FamOnly_Checked { get; set; }

    [ObservableProperty]
    public partial bool Selection_Checked { get; set; }

    [ObservableProperty]
    public partial bool Male2_Checked { get; set; }

    [ObservableProperty]
    public partial bool Female2_Checked { get; set; }

    [ObservableProperty]
    public partial bool OmitSpouse_Checked { get; set; }

    [ObservableProperty]
    public partial int PersNr { get; set; }

    [ObservableProperty]
    public partial int FamNr { get; set; }

    [ObservableProperty]
    public partial string Text1_Text { get; set; }

    [ObservableProperty]
    public partial string Text2_Text { get; set; }
    private bool xDeathMark;

    public bool Male_Visible { get; private set; }
    public bool Females_Visible { get; private set; }
    public bool FamOnly_Visible { get; private set; }
    public bool Male2_Visible { get; private set; }
    public bool Female2_Visible { get; private set; }
    public bool OmitSpouse_Enabled { get; private set; }
    public bool Male_Enabled { get; private set; }
    public bool Female_Enabled { get; private set; }
    public bool FamOnly_Enabled { get; private set; }

    private bool Label4_Visible;
    private bool Label10_Visible;

    public bool OmitSpouse_Visible { get; private set; }
    public bool Text2_Visible { get; private set; }

    private string Label3_Text;
    private bool ComboBox1_Visible;
    private bool PersonSheet_Visible;
    private bool FamilySheet_Visible;
    private int Modul1_priv;
    private (string, ETextKennz) Modul1_Bezeichnu;

    public System.Collections.ObjectModel.ObservableCollection<ListItem<(int Item1, int Item2, int Item3)>> List1_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> List2_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<(EEventArt, int, short)>> List3_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> List4_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> List5_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> List7_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> ListBox1_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> ComboBox1_Items { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Label1_Text { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Label5_Text { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Label7_Text { get; } = new();
    public System.Collections.ObjectModel.ObservableCollection<ListItem<int>> Label8_Text { get; } = new();

    IContainerControl INamenSuchViewModel.View { get; set; }

    [Obsolete]
    Namensuch View => (Namensuch)(this as INamenSuchViewModel).View;

    FraNameSrchSelection fraNameSrchSelection1 => View.fraNameSrchSelection1;

    private IDocument Document => Namensuch.Default.fraPreview1;

    [Obsolete]
    private PictureBox PictureBox1 => Namensuch.Default.PictureBox1;
    [Obsolete]
    private ComboBox ComboBox1 => Namensuch.Default.ComboBox1;
    [Obsolete]
    private ComboBox ComboBox2 => Namensuch.Default.ComboBox2;
    [Obsolete]
    private GroupBox Frame3 => Namensuch.Default.Frame3;
    [Obsolete]
    private ListBox ListBox1 => Namensuch.Default.ListBox1;
    [Obsolete]
    private ListBox List1 => Namensuch.Default.List1;
    [Obsolete]
    private Cursor Cursor { get; set; }



    public bool xComboBox2AddT308 { get; internal set; }
    public bool xComboBox2AddT309 { get; internal set; }
    public bool IsNotReadOnly => Modul1.Typ != DriveType.CDRom;

    public ListItem<int> ComboBox1_SelectedItem { get; private set; }
    public ListItem<int> ListBox1_SelectedItem { get; private set; }
    public ListItem<int> List5_SelectedItem { get; private set; }
    public bool StartSearch_Visible { get; private set; }

    public Action DoHide { get; set; }
    public bool Ready_Visible { get; private set; }
    public bool Label9_Visible { get; private set; }
    public int FamPerschalt { get; set; }

    // ====================================================================
    // Service Layer Properties (NEW: For modern service integration)
    // ====================================================================
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "";

    [ObservableProperty]
    public partial int SearchResultCount { get; set; }

    // ====================================================================
    // Service Layer Commands (NEW: Stubs - to be implemented)
    // ====================================================================
    public IAsyncRelayCommand ExecuteSearchCommand => throw new NotImplementedException("Implement via service integration");
    public IRelayCommand ClearResultsCommand => throw new NotImplementedException("Implement via clearer method");
    public IRelayCommand<string> SearchTextChangedCommand => throw new NotImplementedException("Implement text change handler");
    public IRelayCommand OmitSpouseToggledCommand => throw new NotImplementedException("Implement spouse toggle handler");
    public IRelayCommand<ListItem<int>> SelectListItemCommand => throw new NotImplementedException("Implement list item selection");

    public static event EventHandler Idle
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        add => IdleEvent = (EventHandler)Delegate.Combine(IdleEvent, value);
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        remove => IdleEvent = (EventHandler)Delegate.Remove(IdleEvent, value);
    }

    public NamenSuchViewModel()
    {
        Option = new(() => asOption, (s) => s == "Y", (b) => b ? "Y" : "");
        //        View.ToolTip1 = null;
        //        View.CommonDialog1Save = null;
        KontSP = new string[102];
        KontSP1 = new string[102];
        Vorn = new int[16];
        Ruf = new string[16];
        for (int i = 0; i < 16; i++)
        {
            List1_Items.Add(null!);
            List1_Items.Add(null!);
            List5_Items.Add(null!);
            List7_Items.Add(null!);
        }
    }


    public Bitmap PicResizeByWidth(Image SourceImage, int Newheigth)
    {
        decimal d = new(Newheigth / (double)SourceImage.Height);
        int num = Convert.ToInt32(decimal.Multiply(d, new decimal(SourceImage.Width)));
        Bitmap bitmap = new(num, Newheigth);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle rect = new(0, 0, num, Newheigth);
            graphics.DrawImage(SourceImage, rect);
        }
        return bitmap;
    }

    public Image AutoSizeImage(Image oBitmap, int maxWidth, int maxHeight, bool bStretch = false)
    {
        //Discarded unreachable code: IL_00bb, IL_00c2
        float num = (float)(maxWidth / (double)maxHeight);
        int num2 = oBitmap.Width;
        int num3 = oBitmap.Height;
        float num4 = (float)(num2 / (double)num3);
        if (num2 > maxWidth || num3 > maxHeight || bStretch)
        {
            checked
            {
                if (num4 <= num)
                {
                    num2 = (int)Math.Round(num2 / (num3 / (double)maxHeight));
                    num3 = maxHeight;
                }
                else
                {
                    num3 = (int)Math.Round(num3 / (num2 / (double)maxWidth));
                    num2 = maxWidth;
                }
                Bitmap bitmap = new(num2, num3);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Rectangle rect = new(0, 0, num2, num3);
                    graphics.DrawImage(oBitmap, rect);
                }
                return bitmap;
            }
        }
        return oBitmap;
    }

    [RelayCommand]
    private void FamOnlyChecked()
    {
        Male2_Checked = false;
        Female2_Checked = false;
        Male_Checked = false;
        Females_Checked = false;

        if (FamOnly_Checked)
        {
            Male2_Visible = false;
            Female2_Visible = false;
            Male_Visible = true;
            Females_Visible = true;
        }
        else
        {
            Male_Visible = false;
            Females_Visible = false;
            Male2_Visible = true;
            Female2_Visible = true;
        }
        Listleer();
    }

    [Obsolete]
    public void Check2_CheckStateChanged(object eventSender, EventArgs eventArgs)
    {
        if (eventSender == View.chbFamOnly)
        {
            Male2_Checked = false;
            Female2_Checked = false;
            Male_Checked = false;
            Females_Checked = false;
        }
        if (eventSender != View.chbSelection)
        {
            if (FamOnly_Checked)
            {
                Male2_Visible = false;
                Female2_Visible = false;

                Male_Visible = true;
                Females_Visible = true;
            }
            else
            {
                Male_Visible = false;
                Females_Visible = false;

                Male2_Visible = true;
                Female2_Visible = true;
            }
            _ = ComboBox1.Focus();
        }
    }

    [RelayCommand]
    private void CheckMale()
    {
        Females_Checked = false;
        Listleer();
    }

    [RelayCommand]
    private void CheckFemale()
    {
        Male_Checked = false;
        Listleer();

    }

    [RelayCommand]
    private void CheckMale2()
    {
        Female2_Checked = false;
        Listleer();
    }

    [RelayCommand]
    private void CheckFemale2()
    {
        Male2_Checked = false;
        Listleer();
    }

    [Obsolete]
    public void Check2_MouseDown(object eventSender, MouseEventArgs eventArgs)
    {
        ;
    }

    private void chbOmitSpouse_CheckStateChanged(object eventSender, EventArgs eventArgs)
    {
        Male_Checked = false;
        Females_Checked = false;
        FamOnly_Checked = false;
        if (OmitSpouse_Checked)
        {
            Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr.";
            Male_Visible = false;
            Females_Visible = false;
            FamOnly_Visible = false;
        }
        else
        {
            Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
            Male2_Visible = true;
            Female2_Visible = true;
            FamOnly_Visible = true;
        }
        if (ComboBox1.Text != "")
        {
            StartSearch();
        }
    }

    private void Combo1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        _ = Interaction.MsgBox(num.ToString());
        if (num == 13)
        {
            StartSearch();
        }
        eventArgs.KeyChar = Strings.Chr(num);
        if (num != 0)
        {
        }
    }

    [Obsolete]
    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        var Index = View.Command1.GetIndex((Button)eventSender);


        Modul1.UbgT1 = "";
        switch (Index)
        {
            case 0:
                break;
            case 1:
                PersonSheet();
                break;
            case 2:
                FamilySheet();
                break;
            case 3:
                StartSearch();
                break;
            case 5:
            case 6:
                Command1_Section56(Index);
                break;
            case 7:
                PrintList();
                break;
            default:
                View.Cursor = Cursors.Default;
                break;
        }
    }

    private void Clear_Label1_Text()
    {
        Label1_Text[15] = new("");
        Label1_Text[27] = new("");
        Label1_Text[16] = new("");
        Label1_Text[17] = new("");
        Label1_Text[18] = new("");
        Label1_Text[19] = new("");
        Label1_Text[20] = new("");
        Label1_Text[21] = new("");
        Label1_Text[22] = new("");
        Label1_Text[23] = new("");
        Label1_Text[24] = new("");
        Label1_Text[25] = new("");
        Label1_Text[26] = new("");
    }


    int ErrHndl_Section3(int num2, ref int num4)
    {
        checked
        {
            int num5;
            if (Information.Err().Number == 53)
            {
                FileSystem.FileClose(6);
                FileSystem.FileOpen(6, Modul1.TempPath + "\\List.dat", OpenMode.Output);
                num4 = 0;
                while (num4 <= 2)
                {
                    FileSystem.PrintLine(6, "0");
                    num4++;
                }
                FileSystem.FileClose(6);
                ProjectData.ClearProjectError();
                if (num2 == 0)
                {
                    throw ProjectData.CreateProjectError(-2146828268);
                }
            }
            num5 = num2;
            return num5;
        }
    }


    [RelayCommand]
    private void FamilySheet()
    {
        Document.ClearDocument();
        Clear_Label1_Text();

        DataModul.DB_FamilyTable.MoveLast();
        int iMax = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();

        var text3 = fraNameSrchSelection1.NumbersText;
        fraNameSrchSelection1.eChb43Txt = EUserText.t341;
        var iErg = Text2List(text3, iMax, List7_Items as IList<IListItem<int>>);
        if (iErg == -1)
        {
            _ = Interaction.MsgBox("Höchste Familiennummer ist " + iMax.AsString());
            return;
        }
        else if (iErg == -2)
        {
            _ = Interaction.MsgBox("Endnummer muß größer sein als Startnummer");
            return;
        }

        FraPreview fraPreview = View.fraPreview1;
        IDocument document = fraPreview;
        fraPreview.Top = 0;
        fraPreview.Left = 0;
        fraPreview.Height = View.Height;
        fraPreview.Visible = true;
        fraPreview.Width = View.Width;
        fraPreview.AdjustLayout();
        document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
        ComboBox1_Visible = false;
        StartSearch_Visible = false;
        fraPreview.Visible = true;
        fraNameSrchSelection1.Visible = true;

        Persistence.ReadStringsTemp("List.dat", asOption);
        Persistence.ReadStringsInit("Druck_ini.dat", asOption, false);

        SetCheckBoxState(Option);
        fraNameSrchSelection1.Visible = true;

        asOption[(int)EOutCfg.o10_EmitIDs] = "";
        if (An == 0)
        {
            return;
        }
        fraNameSrchSelection1.Visible = false;
        foreach (var Item in List7_Items)
        {
            if (!(Item == List7_Items[0]))
            {
                document.AppendText("\n\n");
            }

            Modul1.FamInArb = Item.ItemData<int>();
            FamNr = Modul1.FamInArb;

            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
            if (DataModul.DB_FamilyTable.NoMatch)
            {
                continue;
            }
            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
            document.SetFont(new Font("Arial", 16f, FontStyle.Bold));
            document.AppendText(Modul1.IText[EUserText.t443] + " ");
            document.SetFont(new Font("Arial", 16f, FontStyle.Regular));
            if (Modul1.Schalt == 4 & Modul1.Family.Mann > 0)
            {
                PersNr = Modul1.Family.Mann;
            }
            Modul1.PersInArb = PersNr;
            if (Modul1.Schalt == 4)
            {
                Modul1.PersInArb = Modul1.Family.Mann;
            }
            if (Familie.Default.Visible)
            {
                Modul1.PersInArb = Modul1.Family.Mann;
            }
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
            Namen = Modul1.Person.FullSurName.TrimEnd() + " ";
            if (Namen.Trim() == "")
            {
                Namen = " unbekannt";
            }
            document.SetFont(new Font("Arial", 16f, FontStyle.Bold));
            var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
            document.AppendText(sPrefix + Modul1.Person.Givennames.Trim() + " " + Namen.Trim());
            document.SetFont(new Font("Arial", 16f, FontStyle.Regular));
            document.AppendText(" " + Modul1.IText[EUserText.t446] + " ");
            var mann = Modul1.Family.Mann;
            var frau = Modul1.Family.Frau;
            Modul1.Family.Mann = 0;
            Modul1.Family.Frau = 0;
            Modul1.PersInArb = mann == PersNr ? frau : mann;
            Modul1.FamInArb = FamNr;
            if (Familie.Default.Visible)
            {
                Modul1.PersInArb = frau;
            }
            if (Modul1.PersInArb > 0)
            {
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                Namen = Modul1.Person.FullSurName.TrimEnd() + " ";
            }
            else
            {
                Namen = Modul1.IText[EUserText.tUnknown];
            }
            document.SetFont(new Font("Arial", 16f, FontStyle.Bold));
            document.SetAlignment(HorizontalAlignment.Center);
            sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
            document.AppendText(sPrefix + Modul1.Person.Givennames.Trim() + " " + Namen.Trim());
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            if (Option[EOutCfg.o10_EmitIDs])
            {
                document.AppendText(" [" + Modul1.FamInArb.AsString().Trim() + "]\n");
            }
            else
                document.AppendText("\n");
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            document.AppendText(Modul1.IText[EUserText.t448] + " " + DateTime.Today.AsString());
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            document.AppendText(" " + Modul1.IText[EUserText.t202] + " " + Modul1.User.Owner.Trim() + " " + Modul1.IText[EUserText.t447] + " " + Modul1.Verz + "\n");
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            document.SetAlignment(HorizontalAlignment.Left);
            if (Option[EOutCfg.o13])
            {
                DataModul.DB_PictureTable.Index = "Perkenn  ";
                DataModul.DB_PictureTable.Seek("=", "F", Modul1.FamInArb);
                while (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Modul1.FamInArb))
                {
                    var text4 = "";
                    string Pictures_sPfad = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString();
                    string Pictures_sDatei = DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
                    text4 = Strings.Left(Pictures_sPfad.AsString(), 1) == "#"
                        ? (Modul1.Verz + Strings.Mid(Pictures_sPfad.AsString(), 1, Pictures_sPfad.Length) + Pictures_sDatei).AsString()
                        : (Pictures_sPfad + Pictures_sDatei);
                    document.AppendText(("\nBild: " + text4 + " " + DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value).AsString());
                    _ = document.AppendTextIfNd();
                    DataModul.DB_PictureTable.MoveNext();
                }
            }
            document.AppendTextIfNd("\n", 2);
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
            document.AppendText(Modul1.IText[EUserText.t162] + ":\n");
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));

            if (mann > 0)
            {
                EPerles(0, FamNr, mann, Option[EOutCfg.o10_EmitIDs], document);
                document.AppendText("\n");
            }
            else
                document.AppendText(Modul1.IText[EUserText.tUnknown] + ".\n");
            Modul1.FamInArb = FamNr;
            document.SetIndent(0);
            //S = 0f;
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            Heidat(out Scheid, Modul1.FamInArb, document);
            Bildaus("F");
            //S = 1f;

            //  List<int> aiPers;
            int num4 = 0;
            HandlePersonEvent(document, Modul1.FamInArb, ELinkKennz.lkWitnOfEngage, EUserText.tWitnOfEngage);
            HandlePersonEvent(document, Modul1.FamInArb, ELinkKennz.lkMarrWitness, EUserText.t128, EUserText.tMarrWitness);

            if (null != DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value && DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString().Length > 1)
            {
                if (num4 < 2)
                {
                    document.AppendText("\n" + Modul1.IText[EUserText.tMarrWitness] + ":");
                }

                document.AppendText((DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].Value + ".").AsString());
            }

            HandlePersonEvent(document, Modul1.FamInArb, ELinkKennz.lkWitnOfMarr, EUserText.tWitnOfMarr);


            Retweg3(document);
            document.AppendText("\n");
            int famInArb = Modul1.FamInArb;
            if (!Scheid)
            {
                document.AppendText(" mit");
            }
            else
                document.AppendText(" von");
            if (Option[EOutCfg.o14])
            {
                document.AppendText("\n");
            }
            document.SetIndent(0);
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
            document.AppendText("\n" + Modul1.IText[EUserText.t163] + ":\n");
            document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            if (frau > 0)
            {
                EPerles(0, famInArb, frau, Option[EOutCfg.o10_EmitIDs], document);
            }
            else
                document.AppendText(Modul1.IText[EUserText.tUnknown]);
            Modul1.FamInArb = famInArb;
            document.SetIndent(0);
            Berufe(EEventArt.eA_602, document);
            Berufe(EEventArt.eA_603, document);
            document.SetIndent(0);

            if (DataModul.Family.ReadData(Modul1.FamInArb, out var family))
            {
                document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                document.AppendText("\n" + Modul1.IText[EUserText.tCommonFamName] + ":");
                document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                document.AppendText(" " + family.sName.Trim() + " ");
                if (Option[EOutCfg.o04_Family])
                {
                    if (family.sBem[0] != "")
                    {
                        document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                        document.AppendText("\n" + Modul1.IText[EUserText.t287] + ":");
                        document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                        document.AppendText(" " + family.sBem[0] + '\n');
                    }
                }
            }
            if (Option[EOutCfg.o39])
            {
                var QuText = "";
                DataModul.DB_SourceLinkTable.Index = "Tab";
                DataModul.DB_SourceLinkTable.Seek("=", 2, Modul1.FamInArb);
                while (!DataModul.DB_SourceLinkTable.NoMatch
                    && !DataModul.DB_SourceLinkTable.EOF)
                {
                    int SourceLink_i1 = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt();
                    int SourceLink_iPerFam = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt();
                    if (SourceLink_i1 == 2 &&
                        SourceLink_iPerFam == Modul1.FamInArb)
                    {
                        DataModul.DB_QuTable.Index = "NR";
                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                        if (!DataModul.DB_QuTable.NoMatch)
                        {
                            QuText = QuText.Trim().Length > 0
                                ? (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].AsString())
                                : (QuText + DataModul.DB_QuTable.Fields[QuFields._2].AsString());
                            if (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                            {
                                string left;
                                if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value)
                                {
                                    left = DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() != ""
                                        ? " " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " "
                                        : " " + Modul1.IText[EUserText.t449] + " ";
                                }
                                else
                                    left = " " + Modul1.IText[EUserText.t449] + " ";
                                QuText = (QuText + left + DataModul.DB_SourceLinkTable.Fields[3].Value).AsString();
                            }
                        }
                        if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value)
                        {
                            if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                            {
                                QuText = (QuText + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value + "<").AsString();
                            }
                        }
                        if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value)
                        {
                            if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                            {
                                QuText = (QuText + " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value + "==").AsString();
                            }
                        }
                    }
                    DataModul.DB_SourceLinkTable.MoveNext();
                }
                if (null != DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value)
                {
                    if (Strings.Len(Strings.Trim(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString())) > 0)
                    {
                        QuText = (QuText + "; " + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value).AsString();
                    }
                }
                if (QuText.Trim() != "")
                {
                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                    document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                    document.AppendText("\n" + Modul1.IText[EUserText.t450] + " ");
                    document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    document.AppendText(QuText.Trim() + ".");
                    QuText = "";
                    document.AppendText("\n");
                }
            }
            Modul1.FamInArb = FamNr;
            Modul1.eLKennz = ELinkKennz.lkChild;

            _ = document.AppendTextIfNd("\n");
            var KText = Modul1.IText[EUserText.t451];
            Kindles(ref KText, Modul1.FamInArb, Document);
            Modul1.FamInArb = FamNr;
            Modul1.eLKennz = ELinkKennz.lkAdoptedChild;
            KText = Modul1.IText[EUserText.t452];
            Kindles(ref KText, Modul1.FamInArb, Document);
        }
        fraPreview.DocumentRenew();
        View.Cursor = Cursors.Default;
    }

    void AppendPerson(IDocument doc, int iPers)
    {
        var person = new CPersonData();
        Modul1.Person_ReadNames(iPers, person);
        doc.AppendText((person.Prae.Trim() + " " + person.Givennames.Trim()).Trim() + " " + person.SurName.Trim());
        doc.AppendText("; ");
    }

    void HandlePersonEvent(IDocument fraPreview1, int iFamNr, ELinkKennz eLKennz, EUserText eHdrText, EUserText? eHdrMz = null)
    {
        List<int> aiPers = new();
        var num4 = 1;
        foreach (var link in DataModul.Link.ReadAllFams(iFamNr, eLKennz))
        {
            if (num4++ > 99) break;
            aiPers.Add(link.iPersNr);
        }

        if (aiPers.Count == 1)
        {
            fraPreview1.AppendText("\n" + Modul1.IText[eHdrText] + ": ");
        }
        else if (aiPers.Count >= 1)
        {
            fraPreview1.AppendText("\n" + Modul1.IText[eHdrMz ?? eHdrText] + ": ");
        }

        foreach (var p in aiPers)
        {
            Modul1.PersInArb = p;
            AppendPerson(fraPreview1, p);
        }
        fraPreview1.ReplaceLast("; ", ". ");

    }

    private int Text2List(string text3, int iMax, IList<IListItem<int>> items)
    {
        int nr;
        int iIdx1;
        items.Clear();
        nr = text3.AsInt();
        if ((iIdx1 = text3.IndexOf("-")) >= 0)
        {
            var iStart = text3.Substring(0, iIdx1 - 1).AsInt();
            var iEnd = text3.Substring(iIdx1 + 1).AsInt();

            if (iMax < iStart)
            {
                return -1;
            }
            if (iMax < iEnd)
            {
                return -1;
                //                iEnd = iMax;
            }
            if (iEnd < iStart)
            {
                return -2;
            }
            var num14 = iStart;
            while (num14 <= iEnd)
            {
                items.Add(new ListItem<int>(num14.AsString(), num14));
                num14++;
            }
            nr = items[0].ItemData;
        }
        else if (text3.Contains(";"))
        {
            text3 += ";";
            while (text3.Length > 1)
            {
                iIdx1 = text3.IndexOf(";");
                items.Add(new ListItem<int>(text3.Substring(0, iIdx1 - 1), text3.Substring(0, iIdx1 - 1).AsInt()));
                text3 = text3.Remove(0, iIdx1);
            }
            nr = items[0].ItemData;
        }
        if (items.Count == 0 && nr > 0)
            items.Add(new ListItem<int>(nr.AsString(), nr));
        return 0;

    }

    [RelayCommand]
    private void StartSearch()
    {
        View.Cursor = Cursors.WaitCursor;
        View.ListBox1.Visible = false;
        View.List1.Visible = true;
        Listleer();
        if (ComboBox1.Text.Trim() == "")
        {
            ComboBox1.Text = ComboBox1_Items[0].AsString();
        }
        if (Text1_Text.Trim() != ComboBox1.Text.Trim())
        {
            Text1_Text = "";
            Text1_Text = ComboBox1.Text.Trim();
            if (Text1_Text.Trim() == "")
            {
                _ = Interaction.MsgBox("Suchbegriff muss angegeben werden");
                return;
            }
        }
        if (ComboBox1_Items[0] != ComboBox1_SelectedItem && ComboBox1_SelectedItem.ItemString != "")
        {
            ComboBox1_Items.Insert(0, ComboBox1_SelectedItem);
            Suchspeich();
        }
        if (Text1_Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Suchbegriff muss angegeben werden");
        }
        else
        {
            Listfuell();
            _ = View.ComboBox1.Focus();
        }
        View.Cursor = Cursors.Default;
    }

    [RelayCommand]
    private void PrintList()
    {
        Modul1.Listbox3Clip(List1_Items, 1);
        View.Cursor = Cursors.Default;
    }

    private void Command1_Section56(int Index)
    {
        Persistence.WriteStringsTemp("List.dat", asOption);
        ReadCheckBoxState(Option);

        Persistence.WriteStringsInit("Druck_ini.dat", asOption);
        privaus = fraNameSrchSelection1.ePrintPrivacy.AsInt() - 1;

        An = 1;
        if (Index == 6)
        {
            PersonSheet();
        }
        if (Index == 5)
        {
            FamilySheet();
        }
        //       fraNameSrchSelection1.Visible = false;
        View.Cursor = Cursors.Default;
    }


    [RelayCommand]
    private void PersonSheet()
    {
        Document.ClearDocument();
        Clear_Label1_Text();

        Persistence.ReadBoolsTemp("List.dat", Option);
        fraNameSrchSelection1.eChb43Txt = EUserText.t336;

        Document.SetHangingIndent(20);
        ComboBox2.Visible = false;
        View.fraPreview1.Top = 0;
        View.fraPreview1.Left = 0;
        View.fraPreview1.Height = View.Height;
        View.fraPreview1.Visible = true;
        View.fraPreview1.Width = View.Width;
        View.fraPreview1.AdjustLayout();
        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
        ComboBox1.Visible = false;
        StartSearch_Visible = false;
        ComboBox1.Visible = false;
        StartSearch_Visible = false;
        Modul1.Family.Mann = 0;
        Modul1.Family.Frau = 0;

        SetCheckBoxState(Option);
        fraNameSrchSelection1.Visible = true;
        if (An == 0)
        {
            return;
        }
        var text6 = fraNameSrchSelection1.NumbersText;

        int persInArb = text6.AsInt();
        var liPers = List7_Items;
        var iErg = Text2List(text6, int.MaxValue, liPers as IList<IListItem<int>>);

        while (liPers.Count > 0)
        {
            persInArb = liPers.ItemString(0).AsInt();
            liPers.RemoveAt(0);
            PersNr = persInArb;
            DataModul.DB_PersonTable.Seek("=", persInArb);
            if (!DataModul.DB_PersonTable.NoMatch)
            {
                break;
            }

            persInArb = PersNr;
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            var sPrefix = Modul1.Person.Prefix != "" ? Modul1.Person.Prefix + " " : "";
            var SurName = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
            Namen = SurName.TrimEnd() + " ";

            Document.SetFont(new Font("Arial", 16f, FontStyle.Bold));
            Document.SetAlignment(HorizontalAlignment.Center);
            Document.AppendText(Modul1.IText[EUserText.tPersonSheetFor]);
            Document.SetFont(new Font("Arial", 16f, FontStyle.Regular));
            Document.AppendText(" " + sPrefix + Modul1.Person.Givennames.TrimEnd() + " ");
            Document.SetFont(new Font("Arial", 16f, FontStyle.Bold));
            Document.AppendText(Namen);
            Document.SetFont(new Font("Arial", 13.01f, FontStyle.Regular));
            if (Option[EOutCfg.o10_EmitIDs])
            {
                Document.AppendText("<" + persInArb.AsString().Trim() + ">");
            }
            Document.AppendText("\n");
            Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            Document.AppendText(Modul1.IText[EUserText.t448] + " " + DateTime.Today.AsString());
            Document.AppendText(" " + Modul1.IText[EUserText.t202] + " " + Modul1.User.Owner.Trim() + " " + Modul1.IText[EUserText.t447] + " " + Modul1.Verz + "\n");
            Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            Document.SetAlignment(HorizontalAlignment.Left);
            Document.SetFont(new Font("Arial", 11f, FontStyle.Regular));
            EPerles(0, Modul1.Family.ID, persInArb, Option[EOutCfg.o10_EmitIDs], Document);

            IList<int> aiFam = [];
            if (Modul1.Family.Mann > 0)
            {
                persInArb = Modul1.Family.Mann;
                aiFam = Modul1.Link_Famsuch(persInArb, Modul1.eLKennz = ELinkKennz.lkFather);
            }
            if (Modul1.Family.Frau > 0)
            {
                persInArb = Modul1.Family.Frau;
                aiFam = Modul1.Link_Famsuch(persInArb, Modul1.eLKennz = ELinkKennz.lkMother);
            }
            List3_Items.Clear();
            var num29 = aiFam.Count;
            int num4 = 1;
            while (num4 <= num29)
            {
                List3_Items.Add(new(aiFam[num4 - 1].AsString(), (EEventArt.eA_Unknown, aiFam[num4 - 1], (short)0)));
                num4++;
            }

            num4 = 0;
            while (num4 <= List3_Items.Count - 2)
            {
                if (List3_Items[num4] == List3_Items[num4 + 1])
                {
                    List3_Items.RemoveAt(num4 + 1);
                    num4 = 0;
                }
                else
                {
                    num4++;
                }
            }
            A = 0;

            num4 = 1;
            while (num4 <= 99)
            {
                Modul1.Family.Kind[num4] = 0;
                num4++;
            }
            U = 0;
            while (U <= List3_Items.Count - 1)
            {
                Modul1.FamInArb = List3_Items[U].AsInt();
                num4 = 1;
                foreach (var link in DataModul.Link.ReadAllFams(Modul1.FamInArb, ELinkKennz.lkChild))
                {
                    Modul1.Family.Kind[num4] = link.iPersNr;
                    if (num4++ > 99) break;
                    A++;
                }
                U++;
            }

            if (A > 1)
            {
                Document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                if (U > 1)
                {
                    Document.AppendText("\nGeschwister und Halbgeschwister:\n");
                }
                else
                    Document.AppendText("\n" + Modul1.IText[EUserText.tSiblings] + ":\n");
                Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                Kisort(Modul1.Family, List2_Items);
                Document.SetHangingIndent(30);
                num4 = 0;
                while (num4 <= List2_Items.Count - 1)
                {
                    Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    persInArb = List2_Items[num4].ItemData;
                    _ = Document.AppendTextIfNd("\n");
                    Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    if (Option[EOutCfg.o14])
                    {
                        Document.AppendText("\n");
                    }
                    Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    if (persInArb != PersNr)
                    {
                        Modul1.Person_ReadNames(persInArb, Modul1.Person);
                        if (Option[EOutCfg.o10_EmitIDs])
                        {
                            Document.AppendText(num4 + 1.AsString() + ". <" + persInArb.AsString() + "> " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                        }
                        else
                            Document.AppendText(num4 + 1.AsString() + ". " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                        Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                        Document.AppendText(" " + Modul1.Person.FullSurName.Trim());
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                        Document.AppendText(", ");
                        var pt = DataModul.Person.Seek(persInArb);
                        if (pt.Fields[PersonFields.religi].AsInt() > 0)
                        {
                            Document.AppendText(", " + DataModul.TextLese1(pt.Fields[PersonFields.religi].AsInt()));
                        }
                        Bildaus("P");
                        //Datsch = 1f;
                        var Idned = 0f;
                        Datr1(ref Idned, persInArb);
                    }
                    else
                    {
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                        Document.AppendText(num4 + 1.AsString() + Modul1.IText[EUserText.t235] + "\n");
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    }
                    num4++;
                }
            }
            persInArb = PersNr;
            Modul1.eLKennz = DataModul.Person.GetSex(persInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
            aiFam = Modul1.Link_Famsuch(persInArb, Modul1.eLKennz);
            List<(DateTime, int)> liList8 = new();
            liList8.Clear();
            var num37 = aiFam.Count;
            num4 = 1;
            while (num4 <= num37)
            {
                Modul1.FamInArb = aiFam[num4 - 1];
                Modul1.UbgT = Strings.Mid(Modul1.UbgT, 11, Modul1.UbgT.Length);
                EEventArt eArt = EEventArt.eA_500;
                while (eArt <= EEventArt.eA_505)
                {
                    if ((Datu = DataModul.Event.GetDate(eArt, Modul1.FamInArb)) != default
                        && eArt != EEventArt.eA_504)
                    {
                        break;
                    }
                    eArt++;
                }
                if (Datu == default)
                {
                    Datu = DataModul.Event.GetDate(EEventArt.eA_601, Modul1.FamInArb);
                }
                liList8.Add((Datu, Modul1.FamInArb));
                num4++;
            }
            Document.AppendText("\n");
            Document.SetIndent(0);
            Document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
            if (liList8.Count == 1)
            {
                Document.AppendText("\nEigene Verbindung:");
            }
            if (liList8.Count > 1)
            {
                Document.AppendText("\nEigene Verbindungen:");
            }
            Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            Document.SetIndent(0);
            I1 = 0;
            while (I1 <= liList8.Count - 1)
            {
                Modul1.FamInArb = Strings.Mid(liList8[I1].AsString(), 9, 10).AsInt();
                if (Modul1.FamInArb != 0)
                {
                    Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    if (liList8.Count > 1)
                    {
                        _ = Document.AppendTextIfNd("\n");
                        Document.AppendText(I1 + 1.AsString() + ". ");
                    }
                    Scheid = false;
                    Heidat(out Scheid, Modul1.FamInArb, Document);
                    persInArb = PersNr;
                    var sPSex = DataModul.Person.GetSex(persInArb);

                    Modul1.eLKennz = sPSex switch
                    {
                        "F" => ELinkKennz.lkFather,
                        "M" => ELinkKennz.lkMother,
                        _ => ELinkKennz.lkNone,
                    };

                    if (DataModul.Link.GetPersonFam(Modul1.FamInArb, Modul1.eLKennz, out persInArb))
                    {
                        Modul1.Person_ReadNames(persInArb, Modul1.Person);
                        Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                        if (!Scheid)
                        {
                            Document.AppendText(" mit");
                        }
                        else
                            Document.AppendText(" von");
                        Document.AppendText("\n");
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                        Document.SetIndent(0);
                        if (Option[EOutCfg.o10_EmitIDs])
                        {
                            Document.AppendText("<" + persInArb.AsString() + "> " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                        }
                        else
                            Document.AppendText((Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                        Document.AppendText(" " + Modul1.Person.FullSurName);
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                        if (Modul1.Person.Alias != "")
                        {
                            Document.AppendText(" (" + Modul1.Person.Alias.TrimEnd() + ")");
                        }
                        else
                            Document.AppendText("");
                        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                        var pt = DataModul.Person.Seek(persInArb);
                        if (pt.Fields[PersonFields.religi].AsInt() > 0)
                        {
                            var ubg = pt.Fields[PersonFields.religi].AsInt();
                            _ = DataModul.Textlese(ubg, out var ubgT, out _);
                            Document.AppendText(", " + ubgT);
                        }
                        Bildaus("P");
                        var Idned = 0f;
                        Datr1(ref Idned, persInArb);

                    }
                    else
                    {
                        Document.AppendText("\n");
                        Document.SetIndent(0);
                        Document.AppendText("Mit unbekanntem Partner\n");
                    }
                    A = 0;
                    Fambehk = Modul1.FamInArb;
                    var KText = Modul1.IText[EUserText.t451];
                    Modul1.eLKennz = ELinkKennz.lkChild;
                    Kindles(ref KText, Modul1.FamInArb, Document);
                    Modul1.FamInArb = Fambehk;
                    Modul1.eLKennz = ELinkKennz.lkAdoptedChild;
                    KText = Modul1.IText[EUserText.t452];
                    Kindles(ref KText, Modul1.FamInArb, Document);
                    Document.SetIndent(0);
                    Document.AppendText("\n");
                    I1++;
                }
                else
                    break;
            }

        }
        View.fraPreview1.DocumentRenew();
        View.Cursor = Cursors.Default;
    }

    [RelayCommand]
    private void Close()
    {
        int num;
        Modul1.Ubg = 0;
        if (Selection_Checked)
        {
            DoHide();
        }
        // <========== 3
        num = 34;
        if (!Selection_Checked)
        {
            Male_Checked = false;
            Females_Checked = false;
            FamOnly_Checked = false;
            Male2_Checked = false;
            Female2_Checked = false;
            Listleer();
            ComboBox1.Text = "";
            DoHide();
        }
        View.Cursor = Cursors.Default;
    }

    private void ReadCheckBoxState(IList<bool> aus)
        => fraNameSrchSelection1.ReadCheckBoxState(aus);

    private void SetCheckBoxState(IList<bool> aus)
        => fraNameSrchSelection1.SetCheckBoxState(aus);

    [RelayCommand]
    private void FormLoad()
    {
        ProjectData.ClearProjectError();
        if (IsNotReadOnly)
        {

            if (Persistence.ExistFileInit("Gr.dat"))
            {
                Persistence.WriteBoolsInit("Gr.dat", [false]);
            }
        }
        EreiRf = Persistence.ReadBoolInit("Gr.dat");
        var WiS = FormWindowState.Maximized;
        if (IsNotReadOnly)
        {
            Modul1.Persistence.ReadEnumInit("Windowstate", out WiS);
        }
        View.WindowState = WiS;
        var _ints = Persistence.ReadIntsProg("maspos.dat", 2);
        View.Left = _ints[0];
        View.Top = _ints[1];
        View.fraPreview1.Top = 0;
        View.fraPreview1.Left = 0;
        View.fraPreview1.Height = View.Height;
        View.fraPreview1.Width = View.Width;
        Document.SetFont(View.Font);


        DataModul.DT_DescendentTable.MoveFirst();
        xComboBox2AddT308 = DataModul.DT_DescendentTable.Fields["Gen"].AsInt() > 0;
        DataModul.DT_AncesterTable.MoveFirst();
        xComboBox2AddT309 = DataModul.DT_AncesterTable.RecordCount > 0;


        short num4 = 0;
        while (num4 <= 15)
        {
            View.Label7[num4].Visible = false;
            num4 = (short)unchecked(num4 + 1);
        }
        List1.Visible = true;
        View.fraPreview1.Height = View.Height;
        View.fraPreview1.Width = View.Width;
        View.BackColor = Modul1.HintFarb;
        ProjectData.ClearProjectError();
        var num3 = 3;
        if (IsNotReadOnly)
        {
            if (Selection_Checked)
            {
                FileSystem.FileClose(99);
                FileSystem.FileOpen(99, Modul1.Verz + "SUCH.DAT", OpenMode.Append);
                FileSystem.FileClose(99);
                FileSystem.FileOpen(99, Modul1.Verz + "SUCH.DAT", OpenMode.Input);
                ProjectData.ClearProjectError();
                num3 = 4;
                ComboBox1_Items.Clear();
                num4 = 1;
                while (num4 <= 10)
                {
                    var text = FileSystem.LineInput(99);
                    if (text.Trim() != "")
                    {
                        ComboBox1_Items.Add(new(text));
                    }
                    num4 = (short)unchecked(num4 + 1);
                }
                ComboBox1.Text = ComboBox1_Items[0].AsString();
                _ = ComboBox1.Focus();
                ComboBox1.SelectionStart = ComboBox1.Text.Length;
                return;
            }
        }
        if (Modul1.Suchschalt == 10)
        {
            Ready_Visible = true;
            Label9_Visible = true;
            List1_Items.Clear();
            List1.Visible = true;
            List1.Width = View.Width - 100;
        }
        num4 = 0;
        while (num4 <= 15)
        {
            View.Label5[num4].Font = View.Font;
            num4 = (short)unchecked(num4 + 1);
        }
        NameSuch_Load_End();


        void NameSuch_Load_End()
        {
            ProjectData.ClearProjectError();
            Persistence.ReadStringsInit("Druck_ini.dat", asOption, false);

            if (Option[EOutCfg.o24])
            {
                Option[EOutCfg.o24] = true;
            }

            if (IsNotReadOnly)
            {
                var _s = Persistence.ReadStringsMand("SUCH.DAT", 10);
                ComboBox1_Items.Clear();
                foreach (var s in _s)
                    if (!string.IsNullOrEmpty(s))
                    {
                        ComboBox1_Items.Add(new(s));
                    }
            }
        }
    }

    [RelayCommand]
    private void FormClosed()
    {
        Modul1.Suchschalt = 0;
        if (IsNotReadOnly)
        {
            Suchspeich();
        }
    }

    [RelayCommand]
    private void Label5_DoubleClick(int index)
    {
        if (Modul1.Suchschalt == 2)
        {
            return;
        }
        if (Modul1.Suchschalt == 10)
        {
            return;
        }
        PersNr = checked(Label7_Text[index].ItemData<int>());
        if (Modul1.PersInArb <= 0)
        {
            return;
        }
        if (Modul1.Suchschalt == 1)
        {
            DoHide();
            Familie.Default.Hide();
            Modul1.Ad = true;
            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
            Modul1.Ubg = Modul1.PersInArb;
        }
        else
        {
            PersNr = Modul1.PersInArb;
            DoHide();
        }
    }

    [RelayCommand]
    private void Label6_DoubleClick(int index)
    {
        Modul1.Ubg = checked(Label8_Text[index].AsInt());
        if (Modul1.Suchschalt == 2)
        {
            DoHide();
        }
        if (Modul1.Suchschalt == 0)
        {
            return;
        }
        Modul1.Schalt = 0;
        if (Modul1.Ubg > 0)
        {
            DoHide();
            Personen.Default.Hide();
            Familie.Default.Show(Modul1.Ubg);
        }
    }

    private void List1_SelectedIndexChanged(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_1844
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int famInArb = default;
        int frau = default;
        short num9 = default;
        IList<int> aiFams;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 7474:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_189c;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_18a0;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            PersonSheet_Visible = true;
                            PersNr = ComboBox2.Text == Modul1.IText[EUserText.t311]
                                | ComboBox2.Text == Modul1.IText[EUserText.t312]
                                ? List1.SelectedItem.ItemData<(int, int, int)>().Item2
                                : List1.SelectedItem.ItemData<(int, int, int)>().Item1;
                            lErl = 10;
                            PersonSheet_Visible = PersNr > 0.0;
                            FamNr = List1.SelectedItem.ItemData<(int, int, int)>().Item3;
                            FamilySheet_Visible = FamNr > 0.0;
                            Modul1.FamInArb = 0;
                            if (List1.SelectedItem is ListItem<(int, int, int)>)
                            {
                                Modul1.FamInArb = List1.SelectedItem.ItemData<(int, int, int)>().Item3;
                            }
                            short num5 = 0;
                            while (num5 <= 7)
                            {
                                Label8_Text[num5] = new("");
                                num5 = (short)unchecked(num5 + 1);
                            }
                            num5 = 0;
                            while (num5 <= 15)
                            {
                                Label5_Text[num5] = new("");
                                Label7_Text[num5] = new("");
                                num5 = (short)unchecked(num5 + 1);
                            }
                            Label1_Text[15] = new("");
                            Label1_Text[16] = new("");
                            Label1_Text[17] = new("");
                            Label1_Text[18] = new("");
                            Label1_Text[19] = new("");
                            Label1_Text[20] = new("");
                            Label1_Text[21] = new("");
                            Label1_Text[22] = new("");
                            Label1_Text[23] = new("");
                            Label1_Text[24] = new("");
                            Label1_Text[25] = new("");
                            Label1_Text[26] = new("");
                            Label1_Text[27] = new("");
                            if (Modul1.FamInArb > 0)
                            {
                                DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                Label8_Text[5] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                famInArb = Modul1.FamInArb;
                                var asMarrd = Datlf(Modul1.FamInArb);
                                Label1_Text[27] = new("oo " + asMarrd[0] + " " + asMarrd[1]);
                                frau = Modul1.Family.Frau;
                                if (Modul1.Family.Mann > 0)
                                {
                                    Modul1.PersInArb = Modul1.Family.Mann;

                                    UpdateLabelsPers(Modul1.PersInArb, [(s) => Label7_Text[4] = new(s, s.AsInt()), (s) => Label5_Text[4] = new(s),
                                        (s) => Label1_Text[15] = new(s),
                                        (s) => Label1_Text[16] = new(s)], Option[EOutCfg.o35]);

                                    aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkChild);
                                    Modul1.FamInArb = aiFams.FirstOrDefault();
                                    Label8_Text[7] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                    Modul1.Family.Mann = 0;
                                    frau = Modul1.Family.Frau;
                                    Modul1.Family.Frau = 0;
                                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                    UpdateLabelsMann(Modul1.Family.Mann);
                                    UpdateLabelsFrau(Modul1.Family.Frau);
                                }
                                if (frau > 0)
                                {
                                    Modul1.PersInArb = frau;

                                    UpdateLabelsPers(Modul1.PersInArb, [(s) => Label7_Text[5] = new(s, s.AsInt()), (s) => Label5_Text[5] = new(s), (s) => Label1_Text[18] = new(s), (s) => Label1_Text[17] = new(s)], Option[EOutCfg.o35]);

                                    aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkChild);
                                    Modul1.FamInArb = aiFams.FirstOrDefault();
                                    Label8_Text[6] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                    Modul1.Family.Mann = 0;
                                    Modul1.Family.Frau = 0;
                                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                    if (Modul1.Family.Mann > 0)
                                    {
                                        UpdateLabelsPers(Modul1.Family.Mann, [(s) => Label7_Text[2] = new(s, s.AsInt()), (s) => Label5_Text[2] = new(s), (s) => Label1_Text[24] = new(s), (s) => Label1_Text[23] = new(s)], Option[EOutCfg.o35]);
                                    }
                                    if (Modul1.Family.Frau > 0)
                                    {
                                        UpdateLabelsPers(Modul1.Family.Frau, [(s) => Label7_Text[3] = new(s, s.AsInt()), (s) => Label5_Text[3] = new(s), (s) => Label1_Text[26] = new(s), (s) => Label1_Text[25] = new(s)], Option[EOutCfg.o35]);
                                    }
                                }
                                List4_Items.Clear();
                                Modul1.FamInArb = famInArb;
                                kindsuch(Modul1.FamInArb);
                                if (List4_Items.Count <= 0)
                                {
                                    goto end_IL_0001_2;
                                }
                                num9 = (short)(List4_Items.Count - 1);
                                num5 = 0;
                                while (num5 <= num9
                                    && num5 <= 4)
                                {

                                    Modul1.PersInArb = List4_Items.ItemData<int>(num5);
                                    Label7_Text[num5 + 6] = new(Modul1.PersInArb.AsString(), Modul1.PersInArb);
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    string text = List4_Items[num5].AsString().Left(4).AsString() + " ";
                                    if (text.AsInt() == 0.0)
                                    {
                                        text = "";
                                    }
                                    if (text.Trim() != "")
                                    {
                                        text = "*" + text.Trim();
                                    }
                                    Label5_Text[num5 + 6] = new(text.Trim() + " " + Modul1.Person.Givennames + " " + Modul1.Person.SurName);
                                    Modul1.eLKennz = DataModul.Person.GetSex(Modul1.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                                    aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz);
                                    if (aiFams.Count == 0)
                                        Label5_Text[num5 + 11] = new("Keine Ehe");
                                    else if (aiFams.Count > 1)
                                        Label5_Text[num5 + 11] = new("mehrere Ehen");
                                    else
                                    {
                                        Modul1.FamInArb = aiFams[0];
                                        if (num5 == 1)
                                        {
                                            Label8_Text[0] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                        }
                                        if (num5 == 2)
                                        {
                                            Label8_Text[1] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                        }
                                        if (num5 == 3)
                                        {
                                            Label8_Text[3] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                        }
                                        if (num5 == 4)
                                        {
                                            Label8_Text[4] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                        }
                                        if (num5 == 0)
                                        {
                                            Label8_Text[2] = new(Modul1.FamInArb.AsString(), Modul1.FamInArb);
                                        }
                                        Modul1.Family.Mann = 0;
                                        Modul1.Family.Frau = 0;
                                        DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                                        Modul1.PersInArb = Modul1.eLKennz == ELinkKennz.lkFather ? Modul1.Family.Frau : Modul1.Family.Mann;
                                        Label7_Text[num5 + 11] = new(Modul1.PersInArb.AsString(), Modul1.PersInArb);
                                        if (Modul1.PersInArb > 0)
                                        {
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            Label5_Text[num5 + 11] = new(Modul1.Person.Givennames + " " + Modul1.Person.SurName, Modul1.PersInArb);
                                        }
                                        else
                                            Label5_Text[num5 + 11] = new(" unbekannter Partner");

                                    }


                                    num5 = (short)unchecked(num5 + 1);
                                }
                                goto end_IL_0001_2;
                            }
                            UpdateLabelsPers(PersNr, [(s) => Label7_Text[4] = new(s, s.AsInt()), (s) => Label5_Text[4] = new(s), (s) => Label1_Text[15] = new(s), (s) => Label1_Text[16] = new(s)], Option[EOutCfg.o35]);
                            aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz = ELinkKennz.lkChild);
                            Modul1.FamInArb = aiFams.FirstOrDefault();
                            Modul1.Family.Mann = 0;
                            frau = Modul1.Family.Frau;
                            Modul1.Family.Frau = 0;
                            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                            if (Modul1.Family.Mann > 0)
                            {
                                UpdateLabelsPers(Modul1.Family.Mann, [(s) => Label7_Text[0] = new(s, s.AsInt()), (s) => Label5_Text[0] = new(s), (s) => Label1_Text[20] = new(s), (s) => Label1_Text[19] = new(s)], Option[EOutCfg.o35]);
                            }
                            if (Modul1.Family.Frau > 0)
                            {
                                UpdateLabelsPers(Modul1.Family.Frau, [(s) => Label7_Text[1] = new(s, s.AsInt()), (s) => Label5_Text[1] = new(s), (s) => Label1_Text[22] = new(s), (s) => Label1_Text[21] = new(s)], Option[EOutCfg.o35]);
                            }
                            goto end_IL_0001_2;

                        IL_189c:
                            num4 = unchecked(num2 + 1);
                            goto IL_18a0;
                        IL_18a0:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 6:
                                case 166:
                                case 230:
                                case 231:
                                case 282:
                                case 283:
                                case 284:
                                case 289:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 7474;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }

    }

    private void UpdateLabelsMann(int iPerson)
        => UpdateLabelsPers(iPerson, [(s) => Label7_Text[0] = new(s, s.AsInt()), (s) => Label5_Text[0] = new(s), (s) => Label1_Text[20] = new(s), (s) => Label1_Text[19] = new(s)], Option[EOutCfg.o35]);
    private void UpdateLabelsFrau(int iPerson)
        => UpdateLabelsPers(iPerson, [(s) => Label7_Text[1] = new(s, s.AsInt()), (s) => Label5_Text[1] = new(s), (s) => Label1_Text[22] = new(s), (s) => Label1_Text[21] = new(s)], Option[EOutCfg.o35]);

    private void UpdateLabelsPers(int iPerson, IList<Action<string>> settext, bool xShort)
    {
        if (iPerson > 0)
        {
            var person = new CPersonData();
            Modul1.Person_ReadNames(iPerson, person);
            if (person.Suffix.Trim() != "")
            {
                person.SetFullSurname(person.SurName + " " + person.Suffix);
            }
            if (person.Prefix != "")
            {
                person.SetFullSurname(person.Prefix + " " + person.SurName);
            }
            var asDates = Datr(iPerson, xShort);
            settext[0].Invoke(iPerson.AsString());
            settext[1].Invoke(person.Givennames + " " + person.FullSurName);
            settext[2].Invoke(asDates[0] + " " + asDates[1]);
            settext[3].Invoke(asDates[2] + " " + asDates[3]);
        }
    }


    private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        if (!Selection_Checked)
        {
            Male_Checked = false;
            Females_Checked = false;
            FamOnly_Checked = false;
            Male2_Checked = false;
            Female2_Checked = false;
        }
        checked
        {
            if (!(List1.SelectedItem is ListItem<(int, int, int)>) && Modul1.Schalt == 1)
            {
                if (Modul1.Suchschalt == 2)
                {
                    _ = Interaction.MsgBox("Für eine Person kann kein Familienblatt gedruckt werden");
                    return;
                }
                if (ComboBox2.Text == Modul1.IText[EUserText.t311]
                    | ComboBox2.Text == Modul1.IText[EUserText.t312])
                {
                    PersNr = List1.SelectedItem.ItemData<(int, int, int)>().Item2;

                    Modul1.PersInArb = PersNr;
                }
                else
                {
                    PersNr = List1.SelectedItem.ItemData<(int, int, int)>().Item1;
                }
                if (PersNr > 0)
                {
                    if (Selection_Checked)
                    {
                        DoHide();
                    }
                    else
                    {
                        Listleer();
                        DoHide();
                    }
                    FamPerschalt = 1;
                }
            }
            else if (ComboBox2.Text == Modul1.IText[EUserText.t311] | ComboBox2.Text == Modul1.IText[EUserText.t312])
            {
                Modul1.SuchPer = List1.SelectedItem.ItemData<(int, int, int)>().Item2;
                Modul1.Ubg = List1.SelectedItem.ItemData<(int, int, int)>().Item2;
                Close();
            }
            else
            {
                Modul1.Suchfam = List1.SelectedItem.ItemData<(int, int, int)>().Item3;
                Modul1.SuchPer = List1.SelectedItem.ItemData<(int, int, int)>().Item1;
                Modul1.Ubg = Modul1.Schalt == 1 ? List1.SelectedItem.ItemData<(int, int, int)>().Item3 : List1.SelectedItem.ItemData<(int, int, int)>().Item1;
                Modul1.Schalt = 0;
                if (!Selection_Checked)
                {
                    Listleer();
                    ComboBox1.Text = "";
                }
                DoHide();
            }
        }
    }


    public string Datwand1(DateTime dDatu, string Ds)
    {
        //while (true)
        //{
        //    try
        //    {
        //        /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
        //        ;
        //        int num4;
        //        byte num6;
        //        byte b2;
        //        switch (try0001_dispatch)
        //        {
        //            default:
        //                ProjectData.ClearProjectError();
        //                num3 = 2;
        //                goto IL_0009;
        //            case 1305:
        //                {
        //                    num2 = num;
        //                    switch (num3 <= -2 ? 1 : num3)
        //                    {
        //                        case 2:
        //                            break;
        //                        case 1:
        //                            goto IL_0413;
        //                        default:
        //                            goto end_IL_0001;
        //                    }
        //                    if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, (Information.Err().Number).AsString()) == DialogResult.Cancel)
        //                    {
        //                        ProjectData.EndApp();
        //                    }
        //                    ProjectData.ClearProjectError();
        //                    if (num2 == 0)
        //                    {
        //                        throw ProjectData.CreateProjectError(-2146828268);
        //                    }
        //                    num4 = num2;
        //                    goto IL_0417;
        //                }
        //            end_IL_0001:
        //                break;
        //            IL_0009:
        // num = 2;
        //if (Datu < 8)
        //{
        //    Datu = Datu.PadRight(8, ' ');
        //}
        //if (Conversions.Val(Strings.Mid(Datu, 7, 2)) == 0.0)
        //{
        //    StringType.MidStmtStr(ref Datu, 7, 2, "  ");
        //}
        //if (Conversions.Val(Strings.Mid(Datu, 5, 2)) == 0.0)
        //{
        //    StringType.MidStmtStr(ref Datu, 5, 2, "  ");
        //}
        string Datu = $"{dDatu.Day} {dDatu.Month} {dDatu.Year}";
        Datu = Datu.Trim();
        if (Datu.Length > 0)
        {
            byte b = 1;
            while (b <= (uint)2)
            {
                int num5 = Strings.InStr(Datu, " ");
                if (num5 > 0f)
                {
                    Strings.MidStmtStr(ref Datu, checked(num5), 1, ".");
                }
                b = checked((byte)unchecked((uint)(b + 1)));
            }
        }
        ////Discarded unreachable code: IL_03c1
        //int try0001_dispatch = -1;
        //int num3 = default;
        //int num2 = default;
        //int num = default;
        string left = Ds;
        if (left is "U" or "u")
        {
            Datu = "um " + Datu;
        }
        else if (left is "V" or "v")
        {
            Datu = "vor " + Datu;
        }
        else if (left is "N" or "n")
        {
            Datu = "nach " + Datu;
        }
        else if (left == "?")
        {
            Datu += " ?";
        }
        else if (left is "R" or "r")
        {
            Datu = "errech. " + Datu;
        }
        else if (left is "Z" or "z")
        {
            Datu = "zwischen " + Datu;
        }
        else if (left is "a" or "A")
        {
            Datu = " und " + Datu;
        }
        else if (left is "b" or "B")
        {
            Datu = " bis " + Datu;
        }
        else if (Beruf == 0 && Datu.Length == 10)
        {
            Datu = "am " + Datu;
        }

        // Ds = "";
        return Datu;
        //                break;
        //            IL_0413:
        //                num4 = num2 + 1;
        //                goto IL_0417;
        //            IL_0417:
        //                num2 = 0;
        //                switch (num4)
        //                {
        //                    case 1:
        //                        break;
        //                }
        //                goto default;
        //        }
        //    }
        //    catch (Exception obj) when (num3 != 0 && num2 == 0)
        //    {
        //        ProjectData.SetProjectError(obj);
        //        try0001_dispatch = 1305;
        //        continue;
        //    }
        //    throw ProjectData.CreateProjectError(-2146828237);
        //}
        //if (num2 != 0)
        //{
        //    ProjectData.ClearProjectError();
        //}
    }
    //public void Datles1_()
    //{
    //    //Discarded unreachable code: IL_1dcb, IL_1e1d
    //    int try0001_dispatch = -1;
    //    int num3 = default;
    //    int num2 = default;
    //    int num = default;
    //    short num4 = default;
    //    int lErl = default;
    //    short num7 = default;
    //    int number = default;
    //    float num10 = default;
    //    float num11 = default;
    //    short num12 = default;
    //    short num14 = default;
    //    short num16 = default;
    //    int num17 = default;
    //    string Ds = default;
    //    string QuText = default;
    //    string left = default;
    //    float num19 = default;
    //    while (true)
    //    {
    //        try
    //        {
    //            /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
    //            ;
    //            checked
    //            {
    //                short num5;
    //                short num8;
    //                int num9;
    //                short num13;
    //                short num15;
    //                byte LD;
    //                int Ortnr;
    //                string LD2;
    //                short num18;
    //                short num6;
    //                short Schalt;
    //                switch (try0001_dispatch)
    //                {
    //                    default:
    //                        ProjectData.ClearProjectError();
    //                        num3 = 2;
    //                        goto IL_0009;
    //                    case 9080:
    //                        {
    //                            num2 = num;
    //                            switch ((num3 <= -2) ? 1 : num3)
    //                            {
    //                                case 2:
    //                                    break;
    //                                case 1:
    //                                    goto IL_1ebe;
    //                                default:
    //                                    goto end_IL_0001;
    //                            }
    //                            goto IL_1dcd;
    //                        }
    //                    end_IL_0001:
    //                        break;
    //                    IL_0009:
    //                        num = 2;
    //                        Beruf = 0;
    //                        goto IL_0013;
    //                    IL_0013:
    //                        num = 3;
    //                        Modul1.Datum1 = "";
    //                        goto IL_0020;
    //                    IL_0020:
    //                        num = 4;
    //                        Modul1.Datum2 = "";
    //                        goto IL_002d;
    //                    IL_002d:
    //                        num = 5;
    //                        num10 = 0f;
    //                        goto IL_0036;
    //                    IL_0036:
    //                        num = 6;
    //                        num11 = 0f;
    //                        goto IL_003f;
    //                    IL_003f:
    //                        num = 7;
    //                        num12 = 1;
    //                        goto IL_0045;
    //                    IL_0045: // <========== 3
    //                        num = 8;
    //                        Modul1.Kont[num12] = "";
    //                        goto IL_0056;
    //                    IL_0056:
    //                        num = 9;
    //                        num12 = (short)unchecked(num12 + 1);
    //                        num13 = num12;
    //                        num6 = 50;
    //                        if (num13 <= num6)
    //                        {
    //                            goto IL_0045;
    //                        }
    //                        else
    //                        {
    //                            goto IL_006c;
    //                        }
    //                    IL_006c:
    //                        num = 10;
    //                        num7 = 101;
    //                        goto IL_0074;
    //                    IL_0074: // <========== 3
    //                        num = 11;
    //                        num14 = 0;
    //                        goto IL_007b;
    //                    IL_007b: // <========== 3
    //                        num = 12;
    //                        Modul1.Kont1[num14] = "";
    //                        goto IL_008d;
    //                    IL_008d:
    //                        num = 13;
    //                        num14 = (short)unchecked(num14 + 1);
    //                        num15 = num14;
    //                        num6 = 20;
    //                        if (num15 <= num6)
    //                        {
    //                            goto IL_007b;
    //                        }
    //                        else
    //                        {
    //                            goto IL_00a3;
    //                        }
    //                    IL_00a3:
    //                        num = 14;
    //                        Modul1.Ubg = num7;
    //                        goto IL_00ae;
    //                    IL_00ae:
    //                        num = 15;
    //                        Modul1.Modul1.Art = Modul1.Ubg;
    //                        goto IL_00bd;
    //                    IL_00bd:
    //                        num = 16;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
    //                        goto IL_00d1;
    //                    IL_00d1:
    //                        num = 17;
    //                        DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
    //                        goto IL_013a;
    //                    IL_013a:
    //                        num = 18;
    //                        if (!DataModul.DB_EventTable.NoMatch)
    //                        {
    //                            goto IL_0153;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1d9f;
    //                        }
    //                    IL_0153:
    //                        num = 21;
    //                        if ((num16 == 2) & (Conversions.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()) == num17))
    //                        {
    //                            goto IL_018a;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0196;
    //                        }
    //                    IL_018a:
    //                        num = 22;
    //                        num16 = 3;
    //                        goto end_IL_0001_2;
    //                    IL_0196:
    //                        num = 25;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Options_priv].AsInt())
    //                        {
    //                            goto IL_01c6;
    //                        }
    //                        else
    //                        {
    //                            goto IL_01ef;
    //                        }
    //                    IL_01c6:
    //                        num = 26;
    //                        Modul1.Options_priv = (DataModul.DB_EventTable.Fields[EventFields.Options_priv].AsInt()).AsString();
    //                        goto IL_0204;
    //                    IL_01ef:
    //                        num = 28;
    //                        goto IL_01f4;
    //                    IL_01f4:
    //                        num = 29;
    //                        Modul1.Options_priv = 0.AsString();
    //                        goto IL_0204;
    //                    IL_0204: // <========== 3
    //                        num = 31;
    //                        if (!(Modul1.Options_priv.AsInt() > privaus.AsDouble()))
    //                        {
    //                            goto IL_022a;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1d9f;
    //                        }
    //                    IL_022a:
    //                        num = 34;
    //                        if (Modul1.Ubg == 103)
    //                        {
    //                            goto IL_023d;
    //                        }
    //                        else
    //                        {
    //                            goto IL_02b1;
    //                        }
    //                    IL_023d:
    //                        num = 35;
    //                        Modul1.SterbMerk = false;
    //                        goto IL_0247;
    //                    IL_0247:
    //                        num = 36;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.tot].Value)
    //                        {
    //                            goto IL_0277;
    //                        }
    //                        else
    //                        {
    //                            goto IL_02b1;
    //                        }
    //                    IL_0277:
    //                        num = 37;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.tot].AsString() == "J"))
    //                        {
    //                            goto IL_02a5;
    //                        }
    //                        else
    //                        {
    //                            goto IL_02b1;
    //                        }
    //                    IL_02a5:
    //                        num = 38;
    //                        Modul1.SterbMerk = true;
    //                        goto IL_02b1;
    //                    IL_02b1: // <========== 5
    //                        num = 42;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate() != default))
    //                        {
    //                            goto IL_02e3;
    //                        }
    //                        else
    //                        {
    //                            goto IL_054f;
    //                        }
    //                    IL_02e3:
    //                        num = 43;
    //                        Datu = (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()).AsString();
    //                        goto IL_030b;
    //                    IL_030b:
    //                        num = 44;
    //                        if (num16 == 1)
    //                        {
    //                            goto IL_0319;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0350;
    //                        }
    //                    IL_0319:
    //                        num = 45;
    //                        Modul1.Kont[Modul1.Ubg - 100] = (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()).AsString();
    //                        goto IL_1d9f;
    //                    IL_0350:
    //                        num = 48;
    //                        if (num16 == 10)
    //                        {
    //                            goto IL_035f;
    //                        }
    //                        else
    //                        {
    //                            goto IL_03c7;
    //                        }
    //                    IL_035f:
    //                        num = 49;
    //                        Modul1.Kont[Modul1.Ubg - 100] = DataModul.DB_EventTable.Fields[EventFields.Reg].AsString();
    //                        goto IL_0390;
    //                    IL_0390:
    //                        num = 50;
    //                        Modul1.Kont[Modul1.Ubg - 90] = (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()).AsString();
    //                        goto IL_1d9f;
    //                    IL_03c7:
    //                        num = 53;
    //                        Ds = (DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()).AsString();
    //                        goto IL_03eb;
    //                    IL_03eb:
    //                        num = 54;
    //                        Datwand1(ref Datu, ref Ds);
    //                        goto IL_03fe;
    //                    IL_03fe:
    //                        num = 55;
    //                        Modul1.Kont1[1] = Datu;
    //                        goto IL_0410;
    //                    IL_0410:
    //                        num = 56;
    //                        if (Modul1.Ubg == 101)
    //                        {
    //                            goto IL_0423;
    //                        }
    //                        else
    //                        {
    //                            goto IL_044d;
    //                        }
    //                    IL_0423:
    //                        num = 57;
    //                        Modul1.Datum2 = "           " + Datu.Right( 10);
    //                        goto IL_0443;
    //                    IL_0443:
    //                        num = 58;
    //                        num10 = 1f;
    //                        goto IL_044d;
    //                    IL_044d: // <========== 3
    //                        num = 60;
    //                        if (unchecked(Modul1.Ubg == 102 && num10 == 0f))
    //                        {
    //                            goto IL_0469;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0489;
    //                        }
    //                    IL_0469:
    //                        num = 61;
    //                        Modul1.Datum2 = "           " + Datu.Right( 10);
    //                        goto IL_0489;
    //                    IL_0489: // <========== 3
    //                        num = 63;
    //                        if (Modul1.Ubg == 103)
    //                        {
    //                            goto IL_049c;
    //                        }
    //                        else
    //                        {
    //                            goto IL_04c6;
    //                        }
    //                    IL_049c:
    //                        num = 64;
    //                        Modul1.Datum1 = "           " + Datu.Right( 10);
    //                        goto IL_04bc;
    //                    IL_04bc:
    //                        num = 65;
    //                        num11 = 1f;
    //                        goto IL_04c6;
    //                    IL_04c6: // <========== 3
    //                        num = 67;
    //                        if (unchecked(Modul1.Ubg == 104 && num11 == 0f))
    //                        {
    //                            goto IL_04e2;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0502;
    //                        }
    //                    IL_04e2:
    //                        num = 68;
    //                        Modul1.Datum1 = "           " + Datu.Right( 10);
    //                        goto IL_0502;
    //                    IL_0502: // <========== 3
    //                        num = 70;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.VChr].Value !=  "0"))
    //                        {
    //                            goto IL_0530;
    //                        }
    //                        else
    //                        {
    //                            goto IL_054f;
    //                        }
    //                    IL_0530:
    //                        num = 71;
    //                        Modul1.Kont1[1] = Modul1.Kont1[1] + " M1_DTxt2.Chr";
    //                        goto IL_054f;
    //                    IL_054f: // <========== 4
    //                        num = 74;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default))
    //                        {
    //                            goto IL_0581;
    //                        }
    //                        else
    //                        {
    //                            goto IL_065c;
    //                        }
    //                    IL_0581:
    //                        num = 75;
    //                        Datu = (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate()).AsString();
    //                        goto IL_05a9;
    //                    IL_05a9:
    //                        num = 76;
    //                        Ds = (DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString()).AsString();
    //                        goto IL_05cd;
    //                    IL_05cd:
    //                        num = 77;
    //                        Datwand1(ref Datu, ref Ds);
    //                        goto IL_05e0;
    //                    IL_05e0:
    //                        num = 78;
    //                        if (Strings.Trim((DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString()).AsString()) == "")
    //                        {
    //                            goto IL_061b;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0639;
    //                        }
    //                    IL_061b:
    //                        num = 79;
    //                        Modul1.Kont1[3] = " / " + Datu;
    //                        goto IL_065c;
    //                    IL_0639:
    //                        num = 81;
    //                        goto IL_063e;
    //                    IL_063e:
    //                        num = 82;
    //                        Modul1.Kont1[3] = " " + Datu;
    //                        goto IL_065c;
    //                    IL_065c: // <========== 4
    //                        num = 85;
    //                        Modul1.UbgT = "";
    //                        goto IL_066a;
    //                    IL_066a:
    //                        num = 86;
    //                        if (Conversions.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()) > 0.0)
    //                        {
    //                            goto IL_06a5;
    //                        }
    //                        else
    //                        {
    //                            goto IL_07f1;
    //                        }
    //                    IL_06a5:
    //                        num = 87;
    //                        Modul1.Kont2[6] = "";
    //                        goto IL_06b6;
    //                    IL_06b6:
    //                        num = 88;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Zusatz].Value)
    //                        {
    //                            goto IL_06e6;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0711;
    //                        }
    //                    IL_06e6:
    //                        num = 89;
    //                        Modul1.Kont2[6] = DataModul.DB_EventTable.Fields[EventFields.Zusatz].AsString();
    //                        goto IL_0711;
    //                    IL_0711: // <========== 3
    //                        num = 91;
    //                        Ortnr = (Conversions.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()));
    //                        Schalt = 0;
    //                        LD = 0;
    //                        Ortles(ref Ortnr, ref Schalt, ref LD);
    //                        goto IL_0753;
    //                    IL_0753:
    //                        num = 92;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString()) != "")
    //                        {
    //                            goto IL_0791;
    //                        }
    //                        else
    //                        {
    //                            goto IL_07d1;
    //                        }
    //                    IL_0791:
    //                        num = 93;
    //                        Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString());
    //                        goto IL_07d1;
    //                    IL_07d1: // <========== 3
    //                        num = 95;
    //                        Modul1.Kont1[5] = Modul1.UbgT;
    //                        goto IL_07e2;
    //                    IL_07e2:
    //                        num = 96;
    //                        Modul1.UbgT = "";
    //                        goto IL_07f1;
    //                    IL_07f1: // <========== 3
    //                        num = 98;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.DatumText].Value)
    //                        {
    //                            goto IL_0824;
    //                        }
    //                        else
    //                        {
    //                            goto IL_08b1;
    //                        }
    //                    IL_0824:
    //                        num = 99;
    //                        Ortnr = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt();
    //                        LD2 = "";
    //                        Modul1.KontU = DataModul.TextLese1(Ortnr);
    //                        goto IL_0869;
    //                    IL_0869:
    //                        num = 100;
    //                        if (Modul1.KontU != "")
    //                        {
    //                            goto IL_0889;
    //                        }
    //                        else
    //                        {
    //                            goto IL_08b1;
    //                        }
    //                    IL_0889:
    //                        num = 101;
    //                        Modul1.Kont1[1] = Modul1.Kont1[1] + " (" + Modul1.KontU + ") ";
    //                        goto IL_08b1;
    //                    IL_08b1: // <========== 4
    //                        num = 104;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() >  0))
    //                        {
    //                            goto IL_08e0;
    //                        }
    //                        else
    //                        {
    //                            goto IL_094b;
    //                        }
    //                    IL_08e0:
    //                        num = 105;
    //                        Ortnr = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
    //                        LD2 = "";
    //                        Modul1.KontU = DataModul.TextLese1(Ortnr);
    //                        goto IL_0925;
    //                    IL_0925:
    //                        num = 106;
    //                        Modul1.Kont1[7] = " " + Modul1.KontU.Trim() + " ";
    //                        goto IL_094b;
    //                    IL_094b: // <========== 3
    //                        num = 108;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt() >  0))
    //                        {
    //                            goto IL_097a;
    //                        }
    //                        else
    //                        {
    //                            goto IL_09e0;
    //                        }
    //                    IL_097a:
    //                        num = 109;
    //                        Ortnr = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
    //                        LD2 = "";
    //                        Modul1.KontU = DataModul.TextLese1(Ortnr);
    //                        goto IL_09bf;
    //                    IL_09bf:
    //                        num = 110;
    //                        Modul1.Kont1[6] = Modul1.KontU.Trim() + " ";
    //                        goto IL_09e0;
    //                    IL_09e0: // <========== 3
    //                        num = 112;
    //                        if (!CheckBox18.Checked)
    //                        {
    //                            goto IL_09fb;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0c13;
    //                        }
    //                    IL_09fb:
    //                        num = 113;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Causal].Value)
    //                        {
    //                            goto IL_0a2e;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0c13;
    //                        }
    //                    IL_0a2e:
    //                        num = 114;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.Causal].AsInt() > 0))
    //                        {
    //                            goto IL_0a60;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0c13;
    //                        }
    //                    IL_0a60:
    //                        num = 115;
    //                        Ortnr = DataModul.DB_EventTable.Fields[EventFields.Causal].AsInt();
    //                        LD2 = "";
    //                        Modul1.KontU = DataModul.TextLese1(Ortnr);
    //                        goto IL_0aa5;
    //                    IL_0aa5:
    //                        num = 116;
    //                        Modul1.Kont1[17] = " " + Modul1.KontU.Trim() + " ";
    //                        goto IL_0acb;
    //                    IL_0acb:
    //                        num = 117;
    //                        Modul1.KontU = "";
    //                        goto IL_0ad9;
    //                    IL_0ad9:
    //                        num = 118;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.an].Value)
    //                        {
    //                            goto IL_0b09;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0b7f;
    //                        }
    //                    IL_0b09:
    //                        num = 119;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.an].AsInt() > 0))
    //                        {
    //                            goto IL_0b38;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0b7f;
    //                        }
    //                    IL_0b38:
    //                        num = 120;
    //                        Ortnr = DataModul.DB_EventTable.Fields[EventFields.an].AsInt();
    //                        LD2 = "";
    //                        Modul1.KontU = DataModul.TextLese1(Ortnr);
    //                        goto IL_0b7f;
    //                    IL_0b7f: // <========== 4
    //                        num = 123;
    //                        if (Modul1.KontU.Trim() == "")
    //                        {
    //                            goto IL_0ba1;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0baf;
    //                        }
    //                    IL_0ba1:
    //                        num = 124;
    //                        Modul1.KontU = "an";
    //                        goto IL_0baf;
    //                    IL_0baf: // <========== 3
    //                        num = 126;
    //                        if (Modul1.KontU.Trim() == "°")
    //                        {
    //                            goto IL_0bd1;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0bdf;
    //                        }
    //                    IL_0bd1:
    //                        num = 127;
    //                        Modul1.KontU = "";
    //                        goto IL_0bdf;
    //                    IL_0bdf: // <========== 3
    //                        num = 129;
    //                        Modul1.Kont1[17] = " " + Modul1.KontU.Trim() + Modul1.Kont1[17] + " ";
    //                        goto IL_0c13;
    //                    IL_0c13: // <========== 5
    //                        num = 133;
    //                        if (Option[EOutCfg.o34])
    //                        {
    //                            goto IL_0c39;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0cf6;
    //                        }
    //                    IL_0c39:
    //                        num = 134;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Reg].Value)
    //                        {
    //                            goto IL_0c6f;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0cf6;
    //                        }
    //                    IL_0c6f:
    //                        num = 135;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString()) != "")
    //                        {
    //                            goto IL_0cb0;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0cf6;
    //                        }
    //                    IL_0cb0:
    //                        num = 136;
    //                        Modul1.UbgT = Modul1.UbgT + " (Urk.-PerNr.: " + Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString()) + ") ";
    //                        goto IL_0cf6;
    //                    IL_0cf6: // <========== 5
    //                        num = 140;
    //                        if ((Strings.RTrim(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString()) != "") | (Strings.RTrim(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString()) != ""))
    //                        {
    //                            goto IL_0d6f;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0f65;
    //                        }
    //                    IL_0d6f:
    //                        num = 141;
    //                        Modul1.Kont[Modul1.Ubg - 90] = Modul1.Kont1[1] + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[4] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + Modul1.UbgT;
    //                        goto IL_0df0;
    //                    IL_0df0:
    //                        num = 142;
    //                        Modul1.UbgT = "";
    //                        goto IL_0e01;
    //                    IL_0e01:
    //                        num = 143;
    //                        Jobdreh(ref Modul1.Kont[Modul1.Ubg - 90]);
    //                        goto IL_0e21;
    //                    IL_0e21:
    //                        num = 144;
    //                        if ((Option[EOutCfg.o02]) & (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString()) != ""))
    //                        {
    //                            goto IL_0e78;
    //                        }
    //                        else
    //                        {
    //                            goto IL_0ec0;
    //                        }
    //                    IL_0e78:
    //                        num = 145;
    //                        Modul1.Kont[Modul1.Ubg - 85] = "{" + Strings.RTrim(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString()) + "}";
    //                        goto IL_0ec0;
    //                    IL_0ec0: // <========== 3
    //                        num = 147;
    //                        if ((Option[EOutCfg.o03]) & (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString()) != ""))
    //                        {
    //                            goto IL_0f17;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1040;
    //                        }
    //                    IL_0f17:
    //                        num = 148;
    //                        Modul1.Kont[Modul1.Ubg - 80] = "{" + Strings.RTrim(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString()) + "}";
    //                        goto IL_1040;
    //                    IL_0f65:
    //                        num = 151;
    //                        goto IL_0f6d;
    //                    IL_0f6d:
    //                        num = 152;
    //                        Modul1.Kont[Modul1.Ubg - 90] = Modul1.Kont1[1] + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[4] + Modul1.Kont1[17] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + Modul1.UbgT.Replace( "  ", " ");
    //                        goto IL_100e;
    //                    IL_100e:
    //                        num = 153;
    //                        Modul1.UbgT = "";
    //                        goto IL_101f;
    //                    IL_101f:
    //                        num = 154;
    //                        Jobdreh(ref Modul1.Kont[Modul1.Ubg - 90]);
    //                        goto IL_1040;
    //                    IL_1040: // <========== 4
    //                        num = 156;
    //                        if (Option[EOutCfg.o34])
    //                        {
    //                            goto IL_1066;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1136;
    //                        }
    //                    IL_1066:
    //                        num = 157;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Reg].Value)
    //                        {
    //                            goto IL_109c;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1136;
    //                        }
    //                    IL_109c:
    //                        num = 158;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString()) != "")
    //                        {
    //                            goto IL_10dd;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1136;
    //                        }
    //                    IL_10dd:
    //                        num = 159;
    //                        Modul1.Kont[Modul1.Ubg - 90] = Modul1.Kont[Modul1.Ubg - 90] + " (Urk.-PerNr.: " + Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Reg].AsString()) + ") ";
    //                        goto IL_1136;
    //                    IL_1136: // <========== 5
    //                        num = 163;
    //                        if (Option[EOutCfg.o39])
    //                        {
    //                            goto IL_115c;
    //                        }
    //                        else
    //                        {
    //                            goto IL_19b4;
    //                        }
    //                    IL_115c:
    //                        num = 164;
    //                        DataModul.DB_TTable.Index = "Tab22";
    //                        goto IL_1173;
    //                    IL_1173:
    //                        num = 165;
    //                        DataModul.DB_TTable.Seek("=", 3, Modul1.PersInArb, Modul1.Ubg, 0);
    //                        goto IL_11d7;
    //                    IL_11d7:
    //                        num = 166;
    //                        QuText = "";
    //                        goto IL_177a;
    //                    IL_11ea:
    //                        num = 169;
    //                        if (!DataModul.DB_TTable.NoMatch)
    //                        {
    //                            goto IL_1207;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1767;
    //                        }
    //                    IL_1207:
    //                        num = 170;
    //                        if (null != DataModul.DB_TTable.Fields[TFields.Modul1.Art].AsInt())
    //                        {
    //                            goto IL_123d;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1767;
    //                        }
    //                    IL_123d:
    //                        num = 171;
    //                        if (!Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject((DataModul.DB_TTable.Fields[0 ].AsInt() != 3), (DataModul.DB_TTable.Fields[1].AsInt() > Modul1.PersInArb)), (DataModul.DB_TTable.Fields[TFields.Modul1.Art].AsInt() !=  Modul1.Ubg)), (DataModul.DB_TTable.Fields[TFields.Weiter].AsInt() !=  0))))
    //                        {
    //                            goto IL_1302;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1797;
    //                        }
    //                    IL_1302:
    //                        num = 174;
    //                        DataModul.DB_QuTable.Index = "NR";
    //                        goto IL_1319;
    //                    IL_1319:
    //                        num = 175;
    //                        DataModul.DB_QuTable.Seek("=", DataModul.DB_TTable.Fields[TFields._3]);
    //                        goto IL_1380;
    //                    IL_1380:
    //                        num = 176;
    //                        if (!DataModul.DB_QuTable.NoMatch)
    //                        {
    //                            goto IL_139d;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1767;
    //                        }
    //                    IL_139d:
    //                        num = 177;
    //                        if (Modul1.Kont[Modul1.Ubg - 70] == "")
    //                        {
    //                            goto IL_13c6;
    //                        }
    //                        else
    //                        {
    //                            goto IL_13f2;
    //                        }
    //                    IL_13c6:
    //                        num = 178;
    //                        Modul1.Kont[Modul1.Ubg - 70] = Modul1.IText[EUserText.t450] + " ";
    //                        goto IL_13f2;
    //                    IL_13f2: // <========== 3
    //                        num = 180;
    //                        if (QuText != "")
    //                        {
    //                            goto IL_1412;
    //                        }
    //                        else
    //                        {
    //                            goto IL_144c;
    //                        }
    //                    IL_1412:
    //                        num = 181;
    //                        QuText = (((QuText) + ( (("; ") + ( DataModul.DB_QuTable.Fields[QuFields._2].Value))))).AsString();
    //                        goto IL_147c;
    //                    IL_144c:
    //                        num = 183;
    //                        goto IL_1454;
    //                    IL_1454:
    //                        num = 184;
    //                        QuText = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
    //                        goto IL_147c;
    //                    IL_147c: // <========== 3
    //                        num = 186;
    //                        if (DataModul.DB_TTable.Fields[3].AsString().Trim() != "")
    //                        {
    //                            goto IL_14c1;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1606;
    //                        }
    //                    IL_14c1:
    //                        num = 187;
    //                        if (null != DataModul.DB_TTable.Fields[TFields.Aus].Value)
    //                        {
    //                            goto IL_14f7;
    //                        }
    //                        else
    //                        {
    //                            goto IL_15a3;
    //                        }
    //                    IL_14f7:
    //                        num = 188;
    //                        if (DataModul.DB_TTable.Fields[TFields.Aus].AsString().Trim() != "")
    //                        {
    //                            goto IL_1538;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1575;
    //                        }
    //                    IL_1538:
    //                        num = 189;
    //                        left = " " + DataModul.DB_TTable.Fields[TFields.Aus].AsString().Trim() + " ";
    //                        goto IL_15cf;
    //                    IL_1575:
    //                        num = 191;
    //                        goto IL_157d;
    //                    IL_157d:
    //                        num = 192;
    //                        left = " " + Modul1.IText[EUserText.t449] + " ";
    //                        goto IL_15cf;
    //                    IL_15a3:
    //                        num = 195;
    //                        goto IL_15ab;
    //                    IL_15ab:
    //                        num = 196;
    //                        left = " " + Modul1.IText[EUserText.t449] + " ";
    //                        goto IL_15cf;
    //                    IL_15cf: // <========== 4
    //                        num = 198;
    //                        QuText = (((QuText) + ( ((left) + ( DataModul.DB_TTable.Fields[3].Value))))).AsString();
    //                        goto IL_1606;
    //                    IL_1606: // <========== 3
    //                        num = 200;
    //                        if (null != DataModul.DB_TTable.Fields[TFields.Orig].Value)
    //                        {
    //                            goto IL_1639;
    //                        }
    //                        else
    //                        {
    //                            goto IL_16ad;
    //                        }
    //                    IL_1639:
    //                        num = 201;
    //                        if ((DataModul.DB_TTable.Fields[TFields.Orig].Value !=  ""))
    //                        {
    //                            goto IL_166a;
    //                        }
    //                        else
    //                        {
    //                            goto IL_16ad;
    //                        }
    //                    IL_166a:
    //                        num = 202;
    //                        QuText = Conversions.ToString(Operators.ConcatenateObject(QuText, ((((" >") + ( DataModul.DB_TTable.Fields[TFields.Orig].Value))) + ( "<"))));
    //                        goto IL_16ad;
    //                    IL_16ad: // <========== 4
    //                        num = 205;
    //                        if (null != DataModul.DB_TTable.Fields[TFields.Kom].Value)
    //                        {
    //                            goto IL_16e0;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1754;
    //                        }
    //                    IL_16e0:
    //                        num = 206;
    //                        if ((DataModul.DB_TTable.Fields[TFields.Kom].Value !=  ""))
    //                        {
    //                            goto IL_1711;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1754;
    //                        }
    //                    IL_1711:
    //                        num = 207;
    //                        QuText = Conversions.ToString(Operators.ConcatenateObject(QuText, ((((" ==") + ( DataModul.DB_TTable.Fields[TFields.Kom].Value))) + ( "=="))));
    //                        goto IL_1754;
    //                    IL_1754: // <========== 4
    //                        num = 210;
    //                        Zeiweg(ref QuText);
    //                        goto IL_1767;
    //                    IL_1767: // <========== 5
    //                        num = 214;
    //                        DataModul.DB_TTable.MoveNext();
    //                        goto IL_177a;
    //                    IL_177a: // <========== 3
    //                        num = 168;
    //                        if (!DataModul.DB_TTable.EOF)
    //                        {
    //                            goto IL_11ea;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1797;
    //                        }
    //                    IL_1797: // <========== 3
    //                        num = 216;
    //                        if (QuText.Trim() != "")
    //                        {
    //                            goto IL_17bc;
    //                        }
    //                        else
    //                        {
    //                            goto IL_180d;
    //                        }
    //                    IL_17bc:
    //                        num = 217;
    //                        Zeiweg(ref QuText);
    //                        goto IL_17cc;
    //                    IL_17cc:
    //                        num = 218;
    //                        Modul1.Kont[Modul1.Ubg - 70] = Modul1.IText[EUserText.t450] + " " + QuText.Trim();
    //                        goto IL_17fe;
    //                    IL_17fe:
    //                        num = 219;
    //                        QuText = "";
    //                        goto IL_180d;
    //                    IL_180d: // <========== 3
    //                        num = 221;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Bem3].Value)
    //                        {
    //                            goto IL_1843;
    //                        }
    //                        else
    //                        {
    //                            goto IL_19b4;
    //                        }
    //                    IL_1843:
    //                        num = 222;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString()) != "")
    //                        {
    //                            goto IL_1884;
    //                        }
    //                        else
    //                        {
    //                            goto IL_18d2;
    //                        }
    //                    IL_1884:
    //                        num = 223;
    //                        Modul1.Kont1[9] = Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString());
    //                        goto IL_18b7;
    //                    IL_18b7:
    //                        num = 224;
    //                        Zeiweg(ref Modul1.Kont1[9]);
    //                        goto IL_18d2;
    //                    IL_18d2: // <========== 3
    //                        num = 226;
    //                        if ((Modul1.Kont1[9]).Trim() != "")
    //                        {
    //                            goto IL_1900;
    //                        }
    //                        else
    //                        {
    //                            goto IL_19b4;
    //                        }
    //                    IL_1900:
    //                        num = 227;
    //                        if (Modul1.Kont[Modul1.Ubg - 70] == "")
    //                        {
    //                            goto IL_1929;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1968;
    //                        }
    //                    IL_1929:
    //                        num = 228;
    //                        Modul1.Kont[Modul1.Ubg - 70] = Modul1.IText[EUserText.t450] + " " + (Modul1.Kont1[9]).Trim() + ".";
    //                        goto IL_19b4;
    //                    IL_1968:
    //                        num = 230;
    //                        goto IL_1970;
    //                    IL_1970:
    //                        num = 231;
    //                        Modul1.Kont[Modul1.Ubg - 70] = Modul1.Kont[Modul1.Ubg - 70] + "; " + (Modul1.Kont1[9]).Trim() + ".";
    //                        goto IL_19b4;
    //                    IL_19b4: // <========== 6
    //                        num = 236;
    //                        PersSp = Modul1.PersInArb;
    //                        goto IL_19c6;
    //                    IL_19c6:
    //                        num = 237;
    //                        num4 = 1;
    //                        goto IL_19d0;
    //                    IL_19d0: // <========== 3
    //                        num = 238;
    //                        KontSP1[num4] = Modul1.Kont1[num4];
    //                        goto IL_19e9;
    //                    IL_19e9:
    //                        num = 239;
    //                        KontSP[num4] = Modul1.Kont[num4];
    //                        goto IL_1a02;
    //                    IL_1a02:
    //                        num = 240;
    //                        Modul1.Kont[num4] = "";
    //                        goto IL_1a17;
    //                    IL_1a17:
    //                        num = 241;
    //                        Modul1.Kont1[num4] = "";
    //                        goto IL_1a2c;
    //                    IL_1a2c:
    //                        num = 242;
    //                        num4 = (short)unchecked(num4 + 1);
    //                        num18 = num4;
    //                        num6 = 100;
    //                        if (num18 <= num6)
    //                        {
    //                            goto IL_19d0;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1a45;
    //                        }
    //                    IL_1a45:
    //                        num = 243;
    //                        Modul1.PersInArb = PersSp;
    //                        goto IL_1a57;
    //                    IL_1a57:
    //                        num = 244;
    //                        Modul1.UbgT1 = "";
    //                        goto IL_1a68;
    //                    IL_1a68:
    //                        num = 245;
    //                        Modul1.LfNR = 0;
    //                        goto IL_1a76;
    //                    IL_1a76:
    //                        num = 246;
    //                        num19 = Modul1.Modul1.Art;
    //                        goto IL_1a83;
    //                    IL_1a83:
    //                        num = 247;
    //                        if (Option[EOutCfg.o32])
    //                        {
    //                            goto IL_1aa9;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1c74;
    //                        }
    //                    IL_1aa9:
    //                        num = 248;
    //                        Schalt = (short)Math.Round(num19);
    //                        Zeugsu(ref Schalt);
    //                        num19 = Schalt;
    //                        goto IL_1ac7;
    //                    IL_1ac7:
    //                        num = 249;
    //                        Modul1.PersInArb = PersSp;
    //                        goto IL_1ad9;
    //                    IL_1ad9:
    //                        num = 250;
    //                        Modul1.Modul1.Art = num7;
    //                        goto IL_1ae8;
    //                    IL_1ae8:
    //                        num = 251;
    //                        DataModul.DB_EventTable.Seek("=", num7.AsString(), Modul1.PersInArb.AsString(), "0");
    //                        goto IL_1b51;
    //                    IL_1b51:
    //                        num = 252;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.Bem4].Value)
    //                        {
    //                            goto IL_1b87;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1c74;
    //                        }
    //                    IL_1b87:
    //                        num = 253;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString()) != "")
    //                        {
    //                            goto IL_1bcb;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1c74;
    //                        }
    //                    IL_1bcb:
    //                        num = 254;
    //                        if (Modul1.UbgT1.Trim() != "")
    //                        {
    //                            goto IL_1bf3;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1c38;
    //                        }
    //                    IL_1bf3:
    //                        num = 255;
    //                        Modul1.UbgT1 = Modul1.UbgT1.Trim() + "; " + Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString());
    //                        goto IL_1c74;
    //                    IL_1c38:
    //                        num = 257;
    //                        goto IL_1c40;
    //                    IL_1c40:
    //                        num = 258;
    //                        Modul1.UbgT1 = Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString());
    //                        goto IL_1c74;
    //                    IL_1c74: // <========== 6
    //                        num = 263;
    //                        lErl = 22;
    //                        goto IL_1c7f;
    //                    IL_1c7f:
    //                        num = 264;
    //                        Modul1.PersInArb = PersSp;
    //                        goto IL_1c91;
    //                    IL_1c91:
    //                        num = 265;
    //                        Modul1.Ubg = (Modul1.Modul1.Art);
    //                        goto IL_1ca9;
    //                    IL_1ca9:
    //                        num = 266;
    //                        num4 = 1;
    //                        goto IL_1cb3;
    //                    IL_1cb3: // <========== 3
    //                        num = 267;
    //                        Modul1.Kont1[num4] = KontSP1[num4];
    //                        goto IL_1ccc;
    //                    IL_1ccc:
    //                        num = 268;
    //                        Modul1.Kont[num4] = KontSP[num4];
    //                        goto IL_1ce5;
    //                    IL_1ce5:
    //                        num = 269;
    //                        KontSP[num4] = "";
    //                        goto IL_1cfb;
    //                    IL_1cfb:
    //                        num = 270;
    //                        KontSP1[num4] = "";
    //                        goto IL_1d11;
    //                    IL_1d11:
    //                        num = 271;
    //                        num4 = (short)unchecked(num4 + 1);
    //                        num5 = num4;
    //                        num6 = 100;
    //                        if (num5 <= num6)
    //                        {
    //                            goto IL_1cb3;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1d2a;
    //                        }
    //                    IL_1d2a:
    //                        num = 272;
    //                        if (Modul1.UbgT1.Trim() != "")
    //                        {
    //                            goto IL_1d52;
    //                        }
    //                        else
    //                        {
    //                            goto IL_1d9f;
    //                        }
    //                    IL_1d52:
    //                        num = 273;
    //                        Modul1.Kont[(Modul1.Modul1.Art - 60f)] = "Zeugen: " + Modul1.UbgT1.Trim() + ".";
    //                        goto IL_1d8c;
    //                    IL_1d8c:
    //                        num = 274;
    //                        Modul1.UbgT1 = "";
    //                        goto IL_1d9f;
    //                    IL_1d9f: // <========== 7
    //                        num = 276;
    //                        lErl = 2;
    //                        goto IL_1da9;
    //                    IL_1da9:
    //                        num = 277;
    //                        num7 = (short)unchecked(num7 + 1);
    //                        num8 = num7;
    //                        num6 = 104;
    //                        if (num8 > num6)
    //                        {
    //                            goto end_IL_0001_2;
    //                        }
    //                        goto IL_0074;
    //                    IL_1dcd:
    //                        num = 279;
    //                        if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, (Information.Err().Number).AsString()) == DialogResult.Cancel)
    //                        {
    //                            ProjectData.EndApp();
    //                        }
    //                        goto IL_1dfd;
    //                    IL_1dfd:
    //                        num = 282;
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                        goto IL_1eba;
    //                    IL_1e47:
    //                        num = 288;
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                        goto IL_1ebe;
    //                    IL_1eba:
    //                        num9 = num2;
    //                        goto IL_1ec2;
    //                    IL_1ebe:
    //                        num9 = unchecked(num2 + 1);
    //                        goto IL_1ec2;
    //                    IL_1ec2:
    //                        num2 = 0;
    //                        switch (num9)
    //                        {
    //                            case 1:
    //                                break;
    //                            case 2:
    //                                goto IL_0009;
    //                            case 3:
    //                                goto IL_0013;
    //                            case 4:
    //                                goto IL_0020;
    //                            case 5:
    //                                goto IL_002d;
    //                            case 6:
    //                                goto IL_0036;
    //                            case 7:
    //                                goto IL_003f;
    //                            case 8:
    //                                goto IL_0045;
    //                            case 9:
    //                                goto IL_0056;
    //                            case 10:
    //                                goto IL_006c;
    //                            case 11:
    //                                goto IL_0074;
    //                            case 12:
    //                                goto IL_007b;
    //                            case 13:
    //                                goto IL_008d;
    //                            case 14:
    //                                goto IL_00a3;
    //                            case 15:
    //                                goto IL_00ae;
    //                            case 16:
    //                                goto IL_00bd;
    //                            case 17:
    //                                goto IL_00d1;
    //                            case 18:
    //                                goto IL_013a;
    //                            case 20:
    //                            case 21:
    //                                goto IL_0153;
    //                            case 22:
    //                                goto IL_018a;
    //                            case 24:
    //                            case 25:
    //                                goto IL_0196;
    //                            case 26:
    //                                goto IL_01c6;
    //                            case 28:
    //                                goto IL_01ef;
    //                            case 29:
    //                                goto IL_01f4;
    //                            case 27:
    //                            case 30:
    //                            case 31:
    //                                goto IL_0204;
    //                            case 33:
    //                            case 34:
    //                                goto IL_022a;
    //                            case 35:
    //                                goto IL_023d;
    //                            case 36:
    //                                goto IL_0247;
    //                            case 37:
    //                                goto IL_0277;
    //                            case 38:
    //                                goto IL_02a5;
    //                            case 39:
    //                            case 40:
    //                            case 41:
    //                            case 42:
    //                                goto IL_02b1;
    //                            case 43:
    //                                goto IL_02e3;
    //                            case 44:
    //                                goto IL_030b;
    //                            case 45:
    //                                goto IL_0319;
    //                            case 47:
    //                            case 48:
    //                                goto IL_0350;
    //                            case 49:
    //                                goto IL_035f;
    //                            case 50:
    //                                goto IL_0390;
    //                            case 52:
    //                            case 53:
    //                                goto IL_03c7;
    //                            case 54:
    //                                goto IL_03eb;
    //                            case 55:
    //                                goto IL_03fe;
    //                            case 56:
    //                                goto IL_0410;
    //                            case 57:
    //                                goto IL_0423;
    //                            case 58:
    //                                goto IL_0443;
    //                            case 59:
    //                            case 60:
    //                                goto IL_044d;
    //                            case 61:
    //                                goto IL_0469;
    //                            case 62:
    //                            case 63:
    //                                goto IL_0489;
    //                            case 64:
    //                                goto IL_049c;
    //                            case 65:
    //                                goto IL_04bc;
    //                            case 66:
    //                            case 67:
    //                                goto IL_04c6;
    //                            case 68:
    //                                goto IL_04e2;
    //                            case 69:
    //                            case 70:
    //                                goto IL_0502;
    //                            case 71:
    //                                goto IL_0530;
    //                            case 72:
    //                            case 73:
    //                            case 74:
    //                                goto IL_054f;
    //                            case 75:
    //                                goto IL_0581;
    //                            case 76:
    //                                goto IL_05a9;
    //                            case 77:
    //                                goto IL_05cd;
    //                            case 78:
    //                                goto IL_05e0;
    //                            case 79:
    //                                goto IL_061b;
    //                            case 81:
    //                                goto IL_0639;
    //                            case 82:
    //                                goto IL_063e;
    //                            case 80:
    //                            case 83:
    //                            case 84:
    //                            case 85:
    //                                goto IL_065c;
    //                            case 86:
    //                                goto IL_066a;
    //                            case 87:
    //                                goto IL_06a5;
    //                            case 88:
    //                                goto IL_06b6;
    //                            case 89:
    //                                goto IL_06e6;
    //                            case 90:
    //                            case 91:
    //                                goto IL_0711;
    //                            case 92:
    //                                goto IL_0753;
    //                            case 93:
    //                                goto IL_0791;
    //                            case 94:
    //                            case 95:
    //                                goto IL_07d1;
    //                            case 96:
    //                                goto IL_07e2;
    //                            case 97:
    //                            case 98:
    //                                goto IL_07f1;
    //                            case 99:
    //                                goto IL_0824;
    //                            case 100:
    //                                goto IL_0869;
    //                            case 101:
    //                                goto IL_0889;
    //                            case 102:
    //                            case 103:
    //                            case 104:
    //                                goto IL_08b1;
    //                            case 105:
    //                                goto IL_08e0;
    //                            case 106:
    //                                goto IL_0925;
    //                            case 107:
    //                            case 108:
    //                                goto IL_094b;
    //                            case 109:
    //                                goto IL_097a;
    //                            case 110:
    //                                goto IL_09bf;
    //                            case 111:
    //                            case 112:
    //                                goto IL_09e0;
    //                            case 113:
    //                                goto IL_09fb;
    //                            case 114:
    //                                goto IL_0a2e;
    //                            case 115:
    //                                goto IL_0a60;
    //                            case 116:
    //                                goto IL_0aa5;
    //                            case 117:
    //                                goto IL_0acb;
    //                            case 118:
    //                                goto IL_0ad9;
    //                            case 119:
    //                                goto IL_0b09;
    //                            case 120:
    //                                goto IL_0b38;
    //                            case 121:
    //                            case 122:
    //                            case 123:
    //                                goto IL_0b7f;
    //                            case 124:
    //                                goto IL_0ba1;
    //                            case 125:
    //                            case 126:
    //                                goto IL_0baf;
    //                            case 127:
    //                                goto IL_0bd1;
    //                            case 128:
    //                            case 129:
    //                                goto IL_0bdf;
    //                            case 130:
    //                            case 131:
    //                            case 132:
    //                            case 133:
    //                                goto IL_0c13;
    //                            case 134:
    //                                goto IL_0c39;
    //                            case 135:
    //                                goto IL_0c6f;
    //                            case 136:
    //                                goto IL_0cb0;
    //                            case 137:
    //                            case 138:
    //                            case 139:
    //                            case 140:
    //                                goto IL_0cf6;
    //                            case 141:
    //                                goto IL_0d6f;
    //                            case 142:
    //                                goto IL_0df0;
    //                            case 143:
    //                                goto IL_0e01;
    //                            case 144:
    //                                goto IL_0e21;
    //                            case 145:
    //                                goto IL_0e78;
    //                            case 146:
    //                            case 147:
    //                                goto IL_0ec0;
    //                            case 148:
    //                                goto IL_0f17;
    //                            case 151:
    //                                goto IL_0f65;
    //                            case 152:
    //                                goto IL_0f6d;
    //                            case 153:
    //                                goto IL_100e;
    //                            case 154:
    //                                goto IL_101f;
    //                            case 149:
    //                            case 150:
    //                            case 155:
    //                            case 156:
    //                                goto IL_1040;
    //                            case 157:
    //                                goto IL_1066;
    //                            case 158:
    //                                goto IL_109c;
    //                            case 159:
    //                                goto IL_10dd;
    //                            case 160:
    //                            case 161:
    //                            case 162:
    //                            case 163:
    //                                goto IL_1136;
    //                            case 164:
    //                                goto IL_115c;
    //                            case 165:
    //                                goto IL_1173;
    //                            case 166:
    //                                goto IL_11d7;
    //                            case 169:
    //                                goto IL_11ea;
    //                            case 170:
    //                                goto IL_1207;
    //                            case 171:
    //                                goto IL_123d;
    //                            case 173:
    //                            case 174:
    //                                goto IL_1302;
    //                            case 175:
    //                                goto IL_1319;
    //                            case 176:
    //                                goto IL_1380;
    //                            case 177:
    //                                goto IL_139d;
    //                            case 178:
    //                                goto IL_13c6;
    //                            case 179:
    //                            case 180:
    //                                goto IL_13f2;
    //                            case 181:
    //                                goto IL_1412;
    //                            case 183:
    //                                goto IL_144c;
    //                            case 184:
    //                                goto IL_1454;
    //                            case 182:
    //                            case 185:
    //                            case 186:
    //                                goto IL_147c;
    //                            case 187:
    //                                goto IL_14c1;
    //                            case 188:
    //                                goto IL_14f7;
    //                            case 189:
    //                                goto IL_1538;
    //                            case 191:
    //                                goto IL_1575;
    //                            case 192:
    //                                goto IL_157d;
    //                            case 195:
    //                                goto IL_15a3;
    //                            case 196:
    //                                goto IL_15ab;
    //                            case 190:
    //                            case 193:
    //                            case 194:
    //                            case 197:
    //                            case 198:
    //                                goto IL_15cf;
    //                            case 199:
    //                            case 200:
    //                                goto IL_1606;
    //                            case 201:
    //                                goto IL_1639;
    //                            case 202:
    //                                goto IL_166a;
    //                            case 203:
    //                            case 204:
    //                            case 205:
    //                                goto IL_16ad;
    //                            case 206:
    //                                goto IL_16e0;
    //                            case 207:
    //                                goto IL_1711;
    //                            case 208:
    //                            case 209:
    //                            case 210:
    //                                goto IL_1754;
    //                            case 211:
    //                            case 212:
    //                            case 213:
    //                            case 214:
    //                                goto IL_1767;
    //                            case 167:
    //                            case 168:
    //                            case 215:
    //                                goto IL_177a;
    //                            case 172:
    //                            case 216:
    //                                goto IL_1797;
    //                            case 217:
    //                                goto IL_17bc;
    //                            case 218:
    //                                goto IL_17cc;
    //                            case 219:
    //                                goto IL_17fe;
    //                            case 220:
    //                            case 221:
    //                                goto IL_180d;
    //                            case 222:
    //                                goto IL_1843;
    //                            case 223:
    //                                goto IL_1884;
    //                            case 224:
    //                                goto IL_18b7;
    //                            case 225:
    //                            case 226:
    //                                goto IL_18d2;
    //                            case 227:
    //                                goto IL_1900;
    //                            case 228:
    //                                goto IL_1929;
    //                            case 230:
    //                                goto IL_1968;
    //                            case 231:
    //                                goto IL_1970;
    //                            case 229:
    //                            case 232:
    //                            case 233:
    //                            case 234:
    //                            case 235:
    //                            case 236:
    //                                goto IL_19b4;
    //                            case 237:
    //                                goto IL_19c6;
    //                            case 238:
    //                                goto IL_19d0;
    //                            case 239:
    //                                goto IL_19e9;
    //                            case 240:
    //                                goto IL_1a02;
    //                            case 241:
    //                                goto IL_1a17;
    //                            case 242:
    //                                goto IL_1a2c;
    //                            case 243:
    //                                goto IL_1a45;
    //                            case 244:
    //                                goto IL_1a57;
    //                            case 245:
    //                                goto IL_1a68;
    //                            case 246:
    //                                goto IL_1a76;
    //                            case 247:
    //                                goto IL_1a83;
    //                            case 248:
    //                                goto IL_1aa9;
    //                            case 249:
    //                                goto IL_1ac7;
    //                            case 250:
    //                                goto IL_1ad9;
    //                            case 251:
    //                                goto IL_1ae8;
    //                            case 252:
    //                                goto IL_1b51;
    //                            case 253:
    //                                goto IL_1b87;
    //                            case 254:
    //                                goto IL_1bcb;
    //                            case 255:
    //                                goto IL_1bf3;
    //                            case 257:
    //                                goto IL_1c38;
    //                            case 258:
    //                                goto IL_1c40;
    //                            case 256:
    //                            case 259:
    //                            case 260:
    //                            case 261:
    //                            case 262:
    //                            case 263:
    //                                goto IL_1c74;
    //                            case 264:
    //                                goto IL_1c7f;
    //                            case 265:
    //                                goto IL_1c91;
    //                            case 266:
    //                                goto IL_1ca9;
    //                            case 267:
    //                                goto IL_1cb3;
    //                            case 268:
    //                                goto IL_1ccc;
    //                            case 269:
    //                                goto IL_1ce5;
    //                            case 270:
    //                                goto IL_1cfb;
    //                            case 271:
    //                                goto IL_1d11;
    //                            case 272:
    //                                goto IL_1d2a;
    //                            case 273:
    //                                goto IL_1d52;
    //                            case 274:
    //                                goto IL_1d8c;
    //                            case 19:
    //                            case 32:
    //                            case 46:
    //                            case 51:
    //                            case 275:
    //                            case 276:
    //                                goto IL_1d9f;
    //                            case 277:
    //                                goto IL_1da9;
    //                            case 279:
    //                                goto IL_1dcd;
    //                            case 280:
    //                            case 282:
    //                                goto IL_1dfd;
    //                            case 283:
    //                            case 284:
    //                                num = 284;
    //                                number = Information.Err().Number;
    //                                goto case 286;
    //                            case 286:
    //                            case 287:
    //                                num = 287;
    //                                if (number == 94)
    //                                {
    //                                    goto IL_1e47;
    //                                }
    //                                goto case 291;
    //                            case 288:
    //                                goto IL_1e47;
    //                            case 291:
    //                            case 292:
    //                                num = 292;
    //                                if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, (Information.Err().Number).AsString()) == DialogResult.Cancel)
    //                                {
    //                                    ProjectData.EndApp();
    //                                }
    //                                goto case 293;
    //                            case 293:
    //                            case 295:
    //                                num = 295;
    //                                ProjectData.ClearProjectError();
    //                                if (num2 == 0)
    //                                {
    //                                    throw ProjectData.CreateProjectError(-2146828268);
    //                                }
    //                                goto IL_1eba;
    //                            default:
    //                                goto end_IL_0001;
    //                            case 23:
    //                            case 278:
    //                            case 285:
    //                            case 289:
    //                            case 290:
    //                            case 296:
    //                            case 297:
    //                            case 298:
    //                                goto end_IL_0001_2;
    //                        }
    //                        goto default;
    //                }
    //            }
    //        }
    //        catch (Exception obj) when (num3 != 0 && num2 == 0)
    //        {
    //            ProjectData.SetProjectError(obj, lErl);
    //            try0001_dispatch = 9080;
    //            continue;
    //        }
    //        throw ProjectData.CreateProjectError(-2146828237);
    //    end_IL_0001_2: // <========== 3
    //        break;
    //    }
    //    if (num2 != 0)
    //    {
    //        ProjectData.ClearProjectError();
    //    }
    //}
    public void Datles1(int persInArb, IPersonData person)
    {
        var num = 2;

        Beruf = 0;
        var Modul1_sDat_Death = "";
        var Modul1_sDat_Birth = "";
        float num10 = 0f;
        float num11 = 0f;
        short num12 = 1;
        short num16 = 0;
        while (num12 <= 50)
        {
            Modul1.Kont[num12] = "";
            num12 = (short)unchecked(num12 + 1);
        }
        EEventArt iEventType = EEventArt.eA_Birth;
        while (iEventType <= EEventArt.eA_Burial)
        {
            bool xBreak = false;
            short num14 = 0;
            while (num14 <= 20)
            {
                Modul1.Kont1[num14] = "";
                num14 = (short)unchecked(num14 + 1);
            }

            var cEvt = DataModul.Event.ReadDataPl(iEventType, persInArb, out xBreak);

            if (cEvt == null && xBreak)
            {
                num16 = 3;
                break;
            }
            else if (cEvt == null)
            {
                iEventType++;
                continue;
            }

            if (!(cEvt.iPrivacy > privaus))
            {
                if (iEventType == EEventArt.eA_Death)
                {
                    xDeathMark = cEvt.sDeath == "J";
                }

                string Ds;
                if (cEvt.dDatumV != default)
                {
                    Datu = cEvt.dDatumV;
                    if (num16 == 1)
                    {
                        Modul1.Kont[(int)iEventType - 100] = cEvt.dDatumV.AsString();
                        iEventType++;
                        continue;
                    }
                    else if (num16 == 10)
                    {
                        Modul1.Kont[(int)iEventType - 100] = cEvt.sReg;
                        person.SetData(cEvt);

                        iEventType++;
                        continue;
                    }
                    else
                    {
                        Ds = cEvt.sDatumV_S;
                        var sDatu = Datwand1(Datu, Ds);
                        Modul1.Kont1[1] = Datu.AsString();
                        if (iEventType == EEventArt.eA_Birth)
                        {
                            Modul1_sDat_Birth = $"{Datu:-10}";
                            num10 = 1f;
                        }
                        if (unchecked(iEventType == EEventArt.eA_Baptism && num10 == 0f))
                        {
                            Modul1_sDat_Birth = $"{Datu:-10}";
                        }
                        if (iEventType == EEventArt.eA_Death)
                        {
                            Modul1_sDat_Death = $"{Datu:-10}";
                            num11 = 1f;
                        }
                        if (unchecked(iEventType == EEventArt.eA_Burial && num11 == 0f))
                        {
                            Modul1_sDat_Death = $"{Datu:-10}";
                        }

                        if (cEvt.sVChr != "0")
                        {
                            Modul1.Kont1[1] = Modul1.Kont1[1] + " v.Chr";
                        }
                    }

                }

                if (cEvt.dDatumB != default)
                {
                    Datu = cEvt.dDatumB;
                    Ds = cEvt.sDatumB_S;
                    var sDatu = Datwand1(Datu, Ds);
                    Modul1.Kont1[3] = cEvt.sDatumB_S.Trim() == "" ? " / " + sDatu : " " + sDatu;
                }
                Modul1.UbgT = "";
                if (cEvt.iOrt > 0)
                {
                    Modul1.UbgT = Place_ReadData(cEvt.iOrt, 0, 0, Option[EOutCfg.o35], cEvt.sZusatz);
                    if (cEvt.sOrt_S.Trim() != "")
                    {
                        Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + cEvt.sOrt_S.Trim();
                    }
                    Modul1.Kont1[5] = Modul1.UbgT;
                    Modul1.UbgT = "";
                }

                if (0 != cEvt.iDatumText && cEvt.sDatumText != "")
                {
                    Modul1.Kont1[1] += " (" + cEvt.sDatumText + ") ";
                }

                if (0 < cEvt.iKBem)
                {
                    Modul1.Kont1[7] = " " + cEvt.sKBem.Trim() + " ";
                }

                if (0 < cEvt.iPlatz)
                {
                    Modul1.Kont1[6] = cEvt.sPlatz.Trim() + " ";
                }

                if (!Option[EOutCfg.NoCauseOfDeath] && 0 < cEvt.iCausal)
                {
                    Modul1.Kont1[17] = " " + cEvt.sCausal.Trim() + " ";
                    var KontU = "";
                    if (0 < cEvt.iAn)
                    {
                        KontU = cEvt.sAn;
                    }
                    if (KontU.Trim() == "")
                    {
                        KontU = "an";
                    }
                    if (KontU.Trim() == "°")
                    {
                        KontU = "";
                    }
                    Modul1.Kont1[17] = " " + KontU.Trim() + Modul1.Kont1[17] + " ";
                }

                if (Option[EOutCfg.o34]
                   && string.IsNullOrEmpty(cEvt.sReg.Trim()))
                {
                    Modul1.UbgT = Modul1.UbgT + " (Urk.-Nr.: " + cEvt.sReg.Trim() + ") ";
                }

                var iEventIdx = (int)iEventType - 90;
                if (cEvt.sBem[1].TrimEnd() != ""
                    | cEvt.sBem[2].TrimEnd() != "")
                {

                    Modul1.Kont[iEventIdx] = Modul1.Kont1[1] + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[4] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + Modul1.UbgT;
                    Modul1.UbgT = "";
                    Modul1.Kont[iEventIdx] = Jobdreh(Modul1.Kont[iEventIdx], ereiRf: EreiRf);
                    if (Option[EOutCfg.o02] & cEvt.sBem[1].Trim() != "")
                    {
                        Modul1.Kont[(int)iEventType - 85] = "{" + cEvt.sBem[1].AsString().TrimEnd() + "}";
                    }
                    if (Option[EOutCfg.o03] & cEvt.sBem[2].AsString().Trim() != "")
                    {
                        Modul1.Kont[(int)iEventType - 80] = "{" + cEvt.sBem[2].AsString().TrimEnd() + "}";
                    }
                }
                else
                {
                    Modul1.Kont[iEventIdx] = Modul1.Kont1[1] + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[4] + Modul1.Kont1[17] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + Modul1.UbgT.Replace("  ", " ");
                    Modul1.UbgT = "";
                    Modul1.Kont[iEventIdx] = Jobdreh(Modul1.Kont[iEventIdx], ereiRf: EreiRf);
                }
                if (Option[EOutCfg.o34]
                    && null != cEvt.sReg
                        && cEvt.sReg.Trim() != "")
                {
                    Modul1.Kont[iEventIdx] = Modul1.Kont[iEventIdx] + " (Urk.-Nr.: " + cEvt.sReg.Trim() + ") ";
                }

                if (Option[EOutCfg.o39])
                {
                    string QuText = "";

                    foreach (CSourceLinkData Src in DataModul.SourceLink.ReadAll(persInArb, iEventType))
                    {
                        if (ReadQuField2(Src, out var sQuField2))
                        {
                            if (Modul1.Kont[(int)iEventType - 70] == "")
                            {
                                Modul1.Kont[(int)iEventType - 70] = Modul1.IText[EUserText.t450] + " ";
                            }
                            QuText = QuText != "" ? (QuText + "; " + sQuField2).AsString() : sQuField2;

                            if (Src.sEntry.AsString().Trim() != "")
                            {
                                string left;
                                if (null != Src.sPage)
                                {
                                    left = Src.sPage.AsString().Trim() != "" ? " " + Src.sPage.AsString().Trim() + " " : " " + Modul1.IText[EUserText.t449] + " ";
                                }
                                else
                                {
                                    left = " " + Modul1.IText[EUserText.t449] + " ";
                                }
                                QuText = (QuText + left + Src.sEntry).AsString();
                            }

                            if (null != Src.sOriginalText)
                            {
                                if (Src.sOriginalText != "")
                                {
                                    QuText = (QuText + " >" + Src.sOriginalText + "<");
                                }
                            }

                            if (null != Src.sComment)
                            {
                                if (Src.sComment != "")
                                {
                                    QuText = (QuText + " ==" + Src.sComment + "==");
                                }
                            }
                            QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                        }
                    }
                    if (QuText.Trim() != "")
                    {
                        QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                        Modul1.Kont[(int)iEventType - 70] = Modul1.IText[EUserText.t450] + " " + QuText.Trim();
                        QuText = "";
                    }

                    if ("" != cEvt.sBem[3])
                    {
                        if (cEvt.sBem[3].Trim() != "")
                        {
                            Modul1.Kont1[9] = cEvt.sBem[3].Trim();
                            Modul1.Kont1[9] = Zeiweg(Modul1.Kont1[9], !Option[EOutCfg.o07_KeepFormat]);
                        }
                        if (Modul1.Kont1[9].Trim() != "")
                        {
                            Modul1.Kont[(int)iEventType - 70] = Modul1.Kont[(int)iEventType - 70] == ""
                                ? Modul1.IText[EUserText.t450] + " " + Modul1.Kont1[9].Trim() + "."
                                : Modul1.Kont[(int)iEventType - 70] + "; " + Modul1.Kont1[9].Trim() + ".";
                        }
                    }
                }
                PersSp = persInArb;
                short num4 = 1;
                while (num4 <= 100)
                {
                    KontSP1[num4] = Modul1.Kont1[num4];
                    KontSP[num4] = Modul1.Kont[num4];
                    Modul1.Kont[num4] = "";
                    Modul1.Kont1[num4] = "";
                    num4 = (short)unchecked(num4 + 1);
                }
                persInArb = PersSp;
                Modul1.UbgT1 = "";
                LfNR = 0;
                if (Option[EOutCfg.o32])
                {
                    Zeugsu(iEventType);
                    persInArb = PersSp;
                    string sEvtBem4 = DataModul.Event.GetValue(persInArb, iEventType, EventFields.Bem4, (f) => f.AsString());
                    if (null != sEvtBem4)
                    {
                        if (sEvtBem4.AsString().Trim() != "")
                        {
                            Modul1.UbgT1 = Modul1.UbgT1.Trim() != "" ? Modul1.UbgT1.Trim() + "; " + sEvtBem4.AsString().Trim() : sEvtBem4.AsString().Trim();
                        }
                    }
                }
                persInArb = PersSp;
                num4 = 1;
                while (num4 <= 100)
                {
                    Modul1.Kont1[num4] = KontSP1[num4];
                    Modul1.Kont[num4] = KontSP[num4];
                    KontSP[num4] = "";
                    KontSP1[num4] = "";
                    num4 = (short)unchecked(num4 + 1);
                }
                if (Modul1.UbgT1.Trim() != "")
                {
                    Modul1.Kont[(int)iEventType - 60] = "Zeugen: " + Modul1.UbgT1.Trim() + ".";
                    Modul1.UbgT1 = "";
                }
            }

            iEventType++;
        }

    }
    private static bool ReadQuField2(CSourceLinkData Src, out string sQuField2)
    {
        sQuField2 = "";
        var dB_QuTable = DataModul.DB_QuTable;
        dB_QuTable.Index = "NR";
        dB_QuTable.Seek("=", Src.iQuNr);
        bool flag;
        if (flag = !dB_QuTable.NoMatch)
        {
            sQuField2 = dB_QuTable.Fields["2"].AsString();
        }

        return flag;
    }

    public static string Place_ReadData(int iOrtnr, short Schalt, byte LD, bool Opt, string sEvtZus = "")
        => DataModul.Place.ReadData(iOrtnr, out var cPlace)
          ? DataModul.Place.FullName(cPlace, LD != 0, Opt && Schalt == 0)
          : "";


    public void Ahnles(int persInArb, out string sAncestors, out string sAncesterList, out string sDescendent, out bool xCont)
    {
        var dT_AncesterTable = DataModul.DT_AncesterTable;

        xCont = false;
        sAncestors = "";
        sAncesterList = "";
        sDescendent = "";

        dT_AncesterTable.Index = "PerNr";
        dT_AncesterTable.Seek("=", persInArb);
        while (!dT_AncesterTable.EOF
            && !dT_AncesterTable.NoMatch)
        {
            int Anc_iAhn = dT_AncesterTable.Fields["Ahn"].AsInt();
            int Anc_iPersNr = dT_AncesterTable.Fields["PerNr"].AsInt();
            int Anc_iCont = dT_AncesterTable.Fields["Weiter"].AsInt();
            int Anc_iGen = dT_AncesterTable.Fields["Gen"].AsInt();

            if (Anc_iAhn == 0 || Anc_iPersNr != persInArb)
                break;

            sAncesterList += sAncesterList == "" ? Anc_iAhn.AsString() : $"; {Anc_iAhn}";
            sAncestors = $"Generation {Anc_iGen} Ahn-Nr.: {sAncesterList.Trim()}";

            if (Anc_iCont != 0)
            {
                xCont = true;
            }
            else
            {
                break;
            }
            dT_AncesterTable.MoveNext();
        }

        var dT_DescendentTable = DataModul.DT_DescendentTable;
        dT_DescendentTable.Index = "PerNr";
        dT_DescendentTable.Seek("=", persInArb);
        if (!dT_DescendentTable.NoMatch)
        {
            int Desc_iNr = dT_DescendentTable.Fields["Nr"].AsInt();
            int Desc_iGen = dT_DescendentTable.Fields["Gen"].AsInt();
            sDescendent =
                (string.Concat(Modul1.IText[EUserText.t239] + " ", Desc_iGen.AsString()) + "-" + Desc_iNr).AsString();
        }
    }
    public void kindsuch(int famInArb)
    {
        //Discarded unreachable code: IL_032b, IL_039a
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 1190:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_03ea;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_03ed;
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        byte b = 1;
                        foreach (var link in DataModul.Link.ReadAllFams(famInArb, ELinkKennz.lkChild))
                        {

                            string text = "";
                            Modul1.PersInArb = link.iPersNr;

                            var dt1 = DataModul.Event.GetDate(EEventArt.eA_Birth, Modul1.PersInArb);
                            if (dt1 == default)
                                dt1 = DataModul.Event.GetDate(EEventArt.eA_Baptism, Modul1.PersInArb);

                            if (dt1 == default)
                                text = "00000000";
                            else
                                text = dt1.ToString("yyyyMMdd");

                            lErl = 2;
                            List4_Items.Add(new(text + Modul1.PersInArb.AsString(), Modul1.PersInArb));
                            if (b++ > 99) break;
                        }
                        goto end_IL_0001_2;
                    IL_03ea:
                        num4 = num2 + 1;
                        goto IL_03ed;
                    IL_03ed:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 5:
                            case 9:
                            case 12:
                            case 38:
                            case 43:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 1190;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Listfuell()
    {

        if (!Option[EOutCfg.o12])
        {
            asOption[(int)EOutCfg.o12] = "Y";
        }
        if (!Option[EOutCfg.o13])
        {
            asOption[(int)EOutCfg.o13] = "Y";
        }
        //num5 = 0f;
        if (ComboBox1.Text == "")
        {
            if (ComboBox1.Text[0].AsString() != "")
            {
                ComboBox1.Text = ComboBox1_Items[0].AsString();
                Text1_Text = ComboBox1.Text[0].AsString();
            }
        }
        if (ComboBox1.Text == "")
        {
            View.Cursor = Cursors.Default;
            List1_Items.Add(new("Ende der Liste"));
            goto end_IL_0001_2;
        }
        string text;
        string str;
        string text2;
        string item;
        ComboBox1.Text = Text1_Text;
        string prompt;
        int num8;
        int num7;
        if (Text1_Text != "")
        {
            string left = ComboBox2.Text;
            int num6;
            if (left == Modul1.IText[EUserText.t314])
            {
                DataModul.DSB_SearchTable.Index = "Persuch";
                DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
                if (!OmitSpouse_Checked)
                {
                    Zeigfam();
                }
                else
                {
                    Zeig(DataModul.DSB_SearchTable);
                }
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t315])
            {
                DataModul.DSB_SearchTable.Index = "Persuch";
                DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
                Zeigfamdat();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t313])
            {
                DataModul.DSB_SearchTable.Index = "Nummer";
                if (Text1_Text == "")
                {
                    Text1_Text = "0";
                }
                DataModul.DSB_SearchTable.Seek(">=", Text1_Text.AsInt());
                if (!OmitSpouse_Checked)
                {
                    Zeigfam();
                }
                else
                {
                    Zeig(DataModul.DSB_SearchTable);
                }
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t312])
            {
                Listleer();
                num6 = $"{Text1_Text}0000".Left(8).AsInt();
                if (num6 <= 0)
                {
                    num6 = 1;
                }
                num7 = 0;
                foreach (var cEv in DataModul.Event.ReadAllGt(EventIndex.DatInd, num6))
                    if (cEv.eArt == EEventArt.eA_Death || cEv.eArt == EEventArt.eA_Burial)
                    {
                        if (cEv.eArt == EEventArt.eA_Death)
                        {
                            Kennzt = Modul1.sDeathMark;
                        }
                        if (cEv.eArt == EEventArt.eA_Burial)
                        {
                            Kennzt = Modul1.sBurialMark;
                        }
                        var Pers_sSex = DataModul.Person.GetSex(cEv.iPerFamNr);
                        if (!(Female2_Checked && Pers_sSex == "M")
                            && !(Male2_Checked && Pers_sSex == "F"))
                        {
                            if (cEv.iPerFamNr != PersNr)
                            {
                                var persInArb = cEv.iPerFamNr;
                                var sOrt = "";
                                if (cEv.iOrt > 0)
                                {
                                    sOrt = Place_ReadData(cEv.iOrt, 1, 0, Option[EOutCfg.o35]);
                                }
                                text2 = $"{Kennzt}{cEv.dDatumV:yyyy-MM-dd} {sOrt,-17}";
                                Modul1.Person_ReadNames(cEv.iPerFamNr, Modul1.Person);

                                if (Text2_Text == ""
                                    || Modul1.Person.SurName.ToUpper().Trim().Left(Text2_Text.Length) == Text2_Text.ToUpper())
                                {
                                    if (Modul1.Person.Prefix.Trim() != "")
                                    {
                                        Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                                    }
                                    item = $"{Modul1.Person.FullSurName + "," + Modul1.Person.Givennames,40}{text2}    {cEv.iPerFamNr,-10}";
                                    num7++;
                                    List1_Items.Add(new(item, (persInArb, 0, cEv.iPerFamNr)));
                                    if (num7 == Modul1.Aus[12].AsInt())
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                View.Cursor = Cursors.Default;
                List1_Items.Add(new("Ende der Liste"));
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t311])
            {
                Listleer();
                num6 = $"{Text1_Text}0000".Left(8).AsInt();
                if (num6 <= 0)
                {
                    num6 = 1;
                }
                num7 = 0;
                foreach (var cEv in DataModul.Event.ReadAllGt(EventIndex.DatInd, num6))
                {
                    if (cEv.eArt == EEventArt.eA_Birth || cEv.eArt == EEventArt.eA_Baptism)
                    {
                        if (cEv.eArt == EEventArt.eA_Birth)
                        {
                            Kennzt = Modul1.sBirthMark;
                        }
                        if (cEv.eArt == EEventArt.eA_Baptism)
                        {
                            Kennzt = Modul1.sBaptismMark;
                        }
                        var iPers = cEv.iPerFamNr;
                        string Person_sSex = DataModul.Person.GetSex(iPers);
                        if (!(Female2_Checked & Person_sSex == "M").AsBool()
                            && !(Male2_Checked & Person_sSex == "F").AsBool())
                        {
                            if (iPers != PersNr)
                            {
                                var persInArb = iPers;
                                Modul1.UbgT = "";
                                if (cEv.iOrt > 0)
                                {
                                    Modul1.UbgT = Place_ReadData(cEv.iOrt, 1, 0, Option[EOutCfg.o35]);
                                }
                                text = " " + Modul1.UbgT + "                    ".Left(17);
                                str = Strings.Right("         " + cEv.dDatumV.AsString(), 9);
                                text2 = Kennzt + str.Left(5) + "-" + Strings.Mid(str, 6, 2) + "-" + Strings.Mid(str, 8, 2) + text;
                                Modul1.Person_ReadNames(iPers, Modul1.Person);
                                if (Text2_Text == "" || Modul1.Person.SurName.ToUpper().Trim().Left(Text2_Text.Length) == Text2_Text.ToUpper())
                                {
                                    num7++;
                                    if (Modul1.Person.Prefix.Trim() != "")
                                    {
                                        Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                                    }
                                    item = Strings.Left(Modul1.Person.FullSurName + "," + Modul1.Person.Givennames + "                                          ", 40) + text2 + "    " + Strings.Right("          " + cEv.iPerFamNr.AsString(), 10);
                                    List1_Items.Add(new(item, (cEv.iPerFamNr, 0, 0)));
                                    if (num7 == Modul1.Aus[12].AsInt())
                                    {
                                        break;
                                    }
                                }
                                item = "";
                            }
                        }
                    }
                }
                View.Cursor = Cursors.Default;
                List1_Items.Add(new("Ende der Liste"));
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t319])
            {
                DataModul.DSB_SearchTable.Index = "Aliassuch";
                DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
                if (!OmitSpouse_Checked)
                {
                    Zeigfam();
                }
                else
                {
                    Zeig(DataModul.DSB_SearchTable);
                }
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t308])
            {
                Listleer();
                Modul1.UbgT = " " + Text1_Text;
                if (Modul1.UbgT.Right(1) != ".")
                {
                    Modul1.UbgT += ".";
                }
                prompt = "Die Eingabe der Nachfahren-Nummer muss in der korrekten Form erfolgen.\nImmer in Blöcken von einem Leerzeichen, einer Ziffer und einem Punkt oder zwei Ziffern und ein Punkt.\n";
                if (Modul1.UbgT.Length / 3.0 != Conversion.Int(Modul1.UbgT.Length / 3.0))
                {
                    _ = Interaction.MsgBox(prompt);
                    View.Cursor = Cursors.Default;
                    List1_Items.Add(new("Ende der Liste"));
                    goto end_IL_0001_2;
                }
                else
                {
                    num8 = Modul1.UbgT.Length;
                    num7 = 3;
                    goto IL_14cb;
                }
            }
            else if (left == Modul1.IText[EUserText.t309])
            {
                Listleer();
                Modul1.UbgT = (new string(' ', 40) + Text1_Text).Right(40);
                DataModul.DT_AncesterTable.MoveFirst();
                DataModul.DT_AncesterTable.Index = "Ahnen";
                DataModul.DT_AncesterTable.Seek(">=", Modul1.UbgT);
                while (!DataModul.DT_AncesterTable.EOF)
                {
                    Modul1.PersInArb = DataModul.DT_AncesterTable.Fields["Pernr"].AsInt();
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);

                    var item2_ = Strings.Left(Modul1.Person.SurName.TrimEnd() + "," + Modul1.Person.Givennames + new string(' ', 80), 44) + Strings.Right("          " + DataModul.DT_AncesterTable.Fields["PerNr"].AsInt().AsString(), 10) + "             " + DataModul.DT_AncesterTable.Fields["gen"].AsString() + "   " + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim();
                    List1_Items.Add(new(item2_, (Modul1.PersInArb, 0, 0)));
                    DataModul.DT_AncesterTable.MoveNext();
                }
                goto IL_1fb0;
            }
            else if (left == Modul1.IText[EUserText.t316])
            {
                if (Text1_Text == "")
                {
                    Text1_Text = "0";
                }
                Zeigfamanl();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t317])
            {
                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.BeaDat);
                if (Text1_Text == "")
                {
                    Text1_Text = "0";
                }
                DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
                DataModul.DB_PersonTable.Seek(">=", Text1_Text.AsInt());
                Zeigfamanl2();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t318])
            {
                DataModul.DSB_SearchTable.Index = "K_Phonsuch";
                DataModul.DSB_SearchTable.Seek(">=", Module2.Koelner_Phonetic(Text1_Text), 0);
                if (!OmitSpouse_Checked)
                {
                    Zeigfam();
                }
                else
                {
                    Zeig(DataModul.DSB_SearchTable);
                }
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t320])
            {
                DataModul.DSB_SearchTable.Index = "Soundsuch";
                DataModul.DSB_SearchTable.Seek(">=", Module2.GetSoundEx(Text1_Text), 0);
                if (!OmitSpouse_Checked)
                {
                    Zeigfam();
                }
                else
                {
                    Zeig(DataModul.DSB_SearchTable);
                }
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t321])
            {
                DataModul.DSB_SearchTable.Index = "Leitsuch";
                DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
                if (!OmitSpouse_Checked)
                {
                    Zeigfam();
                }
                else
                {
                    Zeig(DataModul.DSB_SearchTable);
                }
                goto end_IL_0001_2;
            }
        }
        goto IL_1fb0;

    IL_14cb:
        var num9 = num7;
        var num10 = num8;
        while (num7 <= num8)
        {
            if (Strings.Mid(Modul1.UbgT, num7, 1) == ".")
            {
                num7 += 3;
            }
            else
            {
                _ = Interaction.MsgBox(prompt);
                break;
            }
        }

        string item2;
        if (num7 > num8)
        {
            if (Modul1.UbgT == " 1.")
            {
                Modul1.UbgT = " 1";
            }
            DataModul.DT_DescendentTable.Index = "nr";
            DataModul.DT_DescendentTable.Seek(">=", Modul1.UbgT);
            var iPers = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
            Modul1.Person_ReadNames(iPers, Modul1.Person);

            item2 = string.Concat(string.Concat(string.Concat(Strings.Left(Modul1.Person.SurName.TrimEnd() + "," + Modul1.Person.Givennames + new string(' ', 80), 42) + "          " + DataModul.DT_DescendentTable.Fields["Pr"].AsString().Right(10), "         "), "  " + DataModul.DT_DescendentTable.Fields["gen"].AsString().Trim().Right(2)), "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value).AsString();
            List1_Items.Add(new(item2, (iPers, 0, DataModul.DT_DescendentTable.Fields["Nr"].AsInt())));
            if (Modul1.UbgT == " 1")
            {
                Modul1.UbgT = " 1.";
                DataModul.DT_DescendentTable.Seek(">=", Modul1.UbgT);
            }
            else
            {
                DataModul.DT_DescendentTable.MoveNext();
            }

            int num11 = Modul1.Aus[(int)EOutCfg.o13].AsInt();
            num7 = 1;
            while (num7 <= num11)
            {
                iPers = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                Modul1.Person_ReadNames(iPers, Modul1.Person);

                item2 = string.Concat(string.Concat(string.Concat(Strings.Left(Modul1.Person.SurName.TrimEnd() + "," + Modul1.Person.Givennames + new string(' ', 80), 42) + "          " + DataModul.DT_DescendentTable.Fields["Pr"].AsString().Right(10), "         "), "  " + DataModul.DT_DescendentTable.Fields["gen"].AsString().Trim().Right(2)), "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value).AsString();
                List1_Items.Add(new(item2, (-DataModul.DT_DescendentTable.Fields["Nr"].AsInt(), 0, 0)));
                DataModul.DT_DescendentTable.MoveNext();
                num7++;
            }
            goto IL_1fb0;
        }
    IL_1fb0: // <========== 16
        View.Cursor = Cursors.Default;
        List1_Items.Add(new("Ende der Liste"));
        goto end_IL_0001_2;

    end_IL_0001_2: return;
    }

    public static string[] Datr(int persInArb, bool xShort)
    {
        var asDates = Datl(persInArb, xShort);
        if (asDates[2] != "")
        {
            asDates[2] = _Modul1.Instance.sDeathMark + asDates[2];
        }
        if (asDates[0] != "")
        {
            asDates[0] = _Modul1.Instance.sBirthMark + asDates[0];
        }
        return asDates;
    }

    public void Datr1(ref float Idned, int persInArb1)
    {
        Datles1(persInArb1, Modul1.Person);
        float num = 1f;
        if (Modul1.Person.Birthday.Trim() != "" | Modul1.Kont[16].Length > 0 | Modul1.Kont[21].Length > 0 | Modul1.Kont[31].Trim() != "" | Modul1.Kont[41].Trim() != "")
        {
            if (Option[EOutCfg.o14])
            {
                Document.AppendText("\n");
                if (Idned is 0f or 1f or 3f)
                {
                }
                Document.AppendText(Modul1.DTxt[1] + " " + Modul1.Person.Birthday.Trim() + ". ");
                num = 0f;
            }
            else
            {
                Document.AppendText(" " + Modul1.DTxt[1] + " " + Modul1.Person.Birthday.Trim() + ". ");
                num = 0f;
            }
            if (BemSch == 1)
            {
                if (Option[EOutCfg.o02] && Modul1.Kont[16].Length > 0)
                {
                    if (num == 1f)
                    {
                        Document.AppendText(" " + Modul1.DTxt[1]);
                        num = 0f;
                    }
                    string QuText = Modul1.Kont[16];
                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                    Document.AppendText(" " + QuText);
                }
                if (Option[EOutCfg.o03])
                {
                    if (Modul1.Kont[21].Length > 0)
                    {
                        if (num == 1f)
                        {
                            Document.AppendText(" " + Modul1.DTxt[1]);
                        }
                        string QuText = Modul1.Kont[21];
                        QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                        Document.AppendText(" " + QuText);
                    }
                    if (Modul1.Person.Birthday.Trim() == "")
                    {
                    }
                }
            }
            if (Modul1.Kont[41] != "")
            {
                Document.AppendText(" " + Modul1.Kont[41]);
            }
            if (Modul1.Kont[31] != "")
            {
                Document.AppendText(" " + Modul1.Kont[31]);
            }
            num = 1f;
        }
        if (Modul1.Person.Baptised.Trim() != "" | Modul1.Kont[17].Length > 0 | Modul1.Kont[22].Length > 0 | Modul1.Kont[32].Trim() != "" | Modul1.Kont[42].Trim() != "")
        {
            if (Option[EOutCfg.o14])
            {
                Document.AppendText("\n");
                if (Idned is 0f or 1f)
                {
                }
                Document.AppendText(Modul1.DTxt[2] + " " + Modul1.Person.Baptised.Trim() + ". ");
                num = 0f;
            }
            else
            {
                Document.AppendText(" " + Modul1.DTxt[2] + " " + Modul1.Person.Baptised.Trim() + ". ");
                num = 0f;
            }
        }
        if (BemSch == 1)
        {
            if (Option[EOutCfg.o02] && Modul1.Kont[17].Length > 0)
            {
                if (num == 1f)
                {
                    Document.AppendText(" " + Modul1.DTxt[2]);
                    num = 0f;
                }
                string QuText = Modul1.Kont[17];
                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                Document.AppendText(" " + QuText);
            }
            if (Option[EOutCfg.o03] && Modul1.Kont[22].Length > 0)
            {
                if (num == 1f)
                {
                    Document.AppendText(" " + Modul1.DTxt[2]);
                }
                string QuText = Modul1.Kont[22];
                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                Document.AppendText(" " + QuText);
            }
        }
        if (Modul1.Kont[42] != "")
        {
            Document.AppendText(" " + Modul1.Kont[42]);
        }
        if (Modul1.Kont[32] != "")
        {
            Document.AppendText(" " + Modul1.Kont[32]);
        }
        if (Option[EOutCfg.o30])
        {
            int persInArb = persInArb1;
            int num2 = 0;
            foreach (var link in DataModul.Link.ReadAllFams(persInArb1, ELinkKennz.lkGodparent))
            {
                persInArb1 = link.iPersNr;
                Modul1.Person_ReadNames(link.iPersNr, Modul1.Person);
                Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                if (num2 == 0)
                {
                    _ = TrimEnd(Document);
                    Document.AppendText(" " + Modul1.IText[EUserText.tGodparents].Replace("&", "") + ": " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.FullSurName.Trim());
                    num2 = 1;
                }
                else
                {
                    Document.AppendText((Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.FullSurName.Trim());
                }
                if (!Option[EOutCfg.o31_GodpWithoutData])
                {
                    Datles1(link.iPersNr, Modul1.Person);
                    var sBirth = Modul1.Person.Birthday;
                    if (sBirth == "")
                    {
                        sBirth = Modul1.Person.Baptised;
                    }
                    if (sBirth.Trim() != "")
                    {
                        Document.AppendText($" {Modul1.sBirthMark} " + sBirth.Trim());
                    }
                    var sDeath = Modul1.Person.Death;
                    if (sDeath == "")
                    {
                        sDeath = Modul1.Person.Burial;
                    }
                    if (sDeath.Trim() != "" & sBirth.Trim() != "")
                    {
                        Document.AppendText(",");
                    }
                    if (sDeath.Trim() != "")
                    {
                        Document.AppendText(Modul1.DTxt[3] + " " + Modul1.Person.Death.Trim());
                    }
                    Document.AppendText("; ");
                }
                else
                {
                    Document.AppendText("; ");
                }

            }
            persInArb1 = persInArb;
            var pt = DataModul.Person.Seek(persInArb1);
            if (null != pt.Fields[PersonFields.Bem2].Value
                && Strings.Trim(pt.Fields[PersonFields.Bem2].AsString()) != "")
            {
                if (num2 == 0)
                {
                    Document.AppendText(" " + Modul1.IText[EUserText.tGodparents].Replace("&", "") + ": ");
                }
                Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                Document.AppendText(Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString()));
                Retweg3(Document);
            }
        }
        Retweg3(Document);
        _ = Document.TrimEnd(";");
        _ = Document.AppendTextIfNd(".");
        int famInArb = Modul1.FamInArb;
        Modul1.FamInArb = famInArb;
        if (Option[EOutCfg.o40])
        {
            Sterdat(0);
        }
        if (!Option[EOutCfg.o43])
        {
            Berufe(EEventArt.eA_300, Document);
            Berufe(EEventArt.eA_301, Document);
            Berufe(EEventArt.eA_302, Document);
            _ = Document.AppendTextIfNd("\n");
            Berufe(EEventArt.eA_105, Document);
            _ = Document.AppendTextIfNd("\n");
            Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
            Berufe(EEventArt.eA_106, Document);
        }
        if (!Option[EOutCfg.o40])
        {
            Sterdat(0);
        }
        _ = Document.AppendTextIfNd("\n");
        Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
    }

    public static string Zeiweg(string QuText, bool xStrip)
    {
        if (xStrip)
        {
            QuText = QuText.Replace("\t", " ");
            QuText = QuText.Replace("\n", " ");
            QuText = QuText.Replace("\r", " ");
            QuText = QuText.Replace("  ", " ");
            QuText = QuText.Trim();
        }
        return QuText;
    }

    public void Berufe(EEventArt eArt, IDocument rtbAnz)
    {
        //Discarded unreachable code: IL_2efd
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num7;
                    string QuText;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            QuText = "";
                            goto IL_000a;
                        case 14030:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_2f90;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_2f90;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num7 = num2;
                                    goto IL_2f94;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_000a:
                            num = 2;
                            int privaus1 = privaus;
                            int persInArb1 = Modul1.PersInArb;
                            var evtList = List3_Items;

                            string QuText2 = "";
                            Modul1.UbgT1 = "";
                            ProjectData.ClearProjectError();

                            num3 = 2;
                            string sEvent;
                            sEvent = "";
                            evtList.Clear();
                            EEventArt eEventArt = eArt;
                            int iFamPers = persInArb1;
                            if (eEventArt > EEventArt.eA_601)
                            {
                                iFamPers = Modul1.FamInArb;
                            }
                            //                           var dB_EventTable = DataModul.DB_EventTable;
                            foreach (var evt in DataModul.Event.ReadEventsBeSu(iFamPers, eEventArt))
                            {
                                if (evt.iPrivacy <= privaus1 && evt.iLfNr >= 1)
                                {
                                    string sEventDate = evt.dDatumV != default ? evt.dDatumV.AsString() : "";
                                    sEventDate += 0 != evt.iDatumText ? evt.sDatumText.FrameIfNEoW(" (", ")") : "";
                                    string sEventNote = evt.iKBem > 0 ? " " + evt.sKBem.Trim() + " " : " ";
                                    sEventNote += evt.iHausNr > 0 ? evt.sHausNr.Trim() + " " : "";

                                    sEvent = sEventDate + sEventNote + new string(' ', 240).Left(240) + evt.iLfNr.AsString();
                                    if (eEventArt == EEventArt.eA_105 && Option[EOutCfg.o47])
                                    {
                                        sEvent = evt.sArtText + sEvent;
                                    }

                                    evtList.Add(new ListItem<(EEventArt, int, short)>(sEvent, (evt.eArt, evt.iPerFamNr, (short)evt.iLfNr)));
                                }
                            }
                            lErl = 13;

                            if (evtList.Count <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            if (eEventArt == EEventArt.eA_300)
                            {
                                Anz_AppendNewlineIfNeeded(rtbAnz);
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                rtbAnz.AppendText(evtList.Count switch
                                {
                                    1 => Modul1.IText[EUserText.tOccupation] + ": ",
                                    _ => Modul1.IText[EUserText.tTitle] + " ",
                                });
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            }
                            else if (eEventArt == EEventArt.eA_301)
                            {
                                Anz_AppendNewlineIfNeeded(rtbAnz);
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                if (evtList.Count >= 1)
                                {
                                    rtbAnz.AppendText(Modul1.IText[EUserText.t70] + " ");
                                }
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            }
                            else if (eEventArt == EEventArt.eA_302)
                            {
                                Anz_AppendNewlineIfNeeded(rtbAnz);
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                rtbAnz.AppendText(evtList.Count switch
                                {
                                    1 => Modul1.IText[EUserText.t444] + " ",
                                    _ => Modul1.IText[EUserText.t445] + " ",
                                });
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            }
                            else if (eEventArt == EEventArt.eA_602)
                            {
                                Retweg3(Document);
                                rtbAnz.AppendText("\n\n");
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                rtbAnz.AppendText(evtList.Count switch
                                {
                                    1 => "Wohnung der Familie: ",
                                    _ => "Wohnungen der Familie: ",
                                });
                                rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                Modul1.UbgT1 = "";
                            }

                            short num11 = (short)(evtList.Count - 1);
                            short num10 = 0;
                            while (num10 <= num11)
                            {
                                var LfNR = evtList[num10].ItemData<(int iArt, int iPers, int iLfNr)>().iLfNr;
                                var xBreak = !DataModul.Event.ReadData(((EEventArt)Beruf, iFamPers, (short)LfNR), out var evt);
                                if (xBreak || (evt.iLfNr != LfNR))
                                {
                                    _ = Interaction.MsgBox("7: FehlerStop");
                                }
                                var num4 = 0;
                                while (num4++ <= 15)
                                {
                                    Modul1.Kont1[num4] = "";
                                }
                                Datu = default;
                                if (evt.iPerFamNr != iFamPers)
                                {
                                    break;
                                }
                                var sDatu = "";
                                if (evt.dDatumV != default)
                                {
                                    Datu = evt.dDatumV;
                                    sDatu = Datwand1(Datu, evt.sDatumV_S);
                                    Modul1.Kont1[1] = Datu.AsString();
                                }
                                if (evt.dDatumB != default)
                                {
                                    Datu = evt.dDatumB;
                                    sDatu = Datwand1(Datu, evt.sDatumB_S);
                                    if (sDatu.Left(2) == "am")
                                    {
                                        sDatu = Strings.Mid(sDatu, 4, sDatu.Length);
                                    }
                                    if (sDatu != "" & evt.sDatumB_S.Trim() == "")
                                    {
                                        sDatu = " bis " + sDatu;
                                    }
                                    if (Modul1.Kont1[1].Left(2) == "am")
                                    {
                                        Modul1.Kont1[1] = Strings.Mid(Modul1.Kont1[1], 4, Modul1.Kont1[1].Length);
                                    }
                                    if (evt.sDatumV_S.Trim() == "")
                                    {
                                        if (Modul1.Kont1[1].Trim() != "")
                                        {
                                            Modul1.Kont1[1] = Modul1.Kont1[1].Length < 10 ? "von " + Modul1.Kont1[1] : "vom " + Modul1.Kont1[1];
                                        }
                                        Modul1.Kont1[3] = " " + sDatu;
                                    }
                                    if (Modul1.Kont1[3] == "" & sDatu.Trim() != "")
                                    {
                                        Modul1.Kont1[3] = " " + sDatu;
                                    }
                                }
                                Modul1.UbgT = "";
                                if (0 != evt.iDatumText)
                                {
                                    Modul1.Kont1[1] += evt.sDatumText.FrameIfNEoW(" (", ")");
                                }
                                if (eEventArt == EEventArt.eA_105
                                    && Option[EOutCfg.o34]
                                    && "" != evt.sReg
                                    && evt.sReg.Trim() != "")
                                {
                                    Modul1.Kont1[10] = " (Urk.-Nr.: " + evt.sReg.Trim() + ") ";
                                }
                                if (evt.iKBem > 0)
                                {
                                    Modul1.Kont1[7] = evt.sKBem.FrameIfNEoW(" ");
                                }
                                if (0 != evt.iHausNr)
                                {
                                    Modul1.Kont1[7] += evt.sHausNr.Trim() + " ";
                                }

                                if (eEventArt == EEventArt.eA_603)
                                {
                                    Modul1.Kont[0] = evt.sArtText;
                                }
                                else if (eEventArt == EEventArt.eA_105)
                                {
                                    Anz_AppendNewlineIfNeeded(rtbAnz);
                                    Modul1.Kont[0] = 0 != evt.iArtText ? evt.sArtText : "";
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                    rtbAnz.AppendText(Modul1.Kont[0].Trim() + ": ");
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                }
                                if (eEventArt == EEventArt.eA_106)
                                {
                                    Anz_AppendNewlineIfNeeded(rtbAnz);
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                    rtbAnz.AppendText("Heimatort/recht: ");
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                }
                                if (eEventArt == EEventArt.eA_603)
                                {
                                    Anz_AppendNewlineIfNeeded(rtbAnz);
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                                    rtbAnz.AppendText(Modul1.Person.SurName.Trim() + ": ");
                                    rtbAnz.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                }
                                if (evt.iOrt > 0)
                                {
                                    Modul1.UbgT = Place_ReadData(evt.iOrt, 0, 0, Option[EOutCfg.o35], evt.sZusatz);
                                    if (evt.sOrt_S != "")
                                    {
                                        Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + evt.sOrt_S.Trim();
                                    }
                                    Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                    Modul1.UbgT = "";
                                }
                                Modul1.Kont1[4] = evt.sDatumB_S;
                                if (evt.iPlatz > 0)
                                {
                                    Modul1.Kont1[6] = evt.sPlatz.FrameIfNEoW(" ");
                                }
                                Modul1.Kont1[4] = "";
                                if (eEventArt < EEventArt.eA_400)
                                {
                                    if (Option[EOutCfg.o02] && evt.sBem[1] != " ")
                                    {
                                        Modul1.Kont1[2] = evt.sBem[1].Trim().FrameIfNEoW("{", "}");
                                    }
                                    if (Option[EOutCfg.o03] && evt.sBem[2] != " ")
                                    {
                                        Modul1.Kont1[4] = evt.sBem[2].Trim().FrameIfNEoW("{", "}");
                                    }
                                }
                                else
                                {
                                    if (Option[EOutCfg.o02] && evt.sBem[1] != " ")
                                    {
                                        Modul1.Kont1[2] = evt.sBem[1].Trim().FrameIfNEoW("{", "}");
                                    }
                                    if (Option[EOutCfg.o03] && evt.sBem[2] != " ")
                                    {
                                        Modul1.Kont1[4] = evt.sBem[2].Trim().FrameIfNEoW("{", "}");
                                    }
                                }
                                if (Modul1.Kont1[2].Trim() != "" | Modul1.Kont1[4].Trim() != "")
                                {
                                    QuText = " " + Modul1.Kont1[2].Trim() + " " + Modul1.Kont1[4].Trim();
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                }
                                if (eEventArt != EEventArt.eA_603)
                                {
                                }
                                sEvent = Jobdreh(sEvent, ereiRf: EreiRf);
                                sEvent = sEvent + " " + QuText;
                                QuText = "";
                                if (Option[EOutCfg.o39])
                                {
                                    DataModul.DB_SourceLinkTable.Index = "Tab22";
                                    DataModul.DB_SourceLinkTable.Seek("=", 3, iFamPers, eEventArt, LfNR);
                                    QuText2 = "";
                                    while (!DataModul.DB_SourceLinkTable.EOF && !DataModul.DB_SourceLinkTable.NoMatch
                                            && 0 != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt())
                                    {
                                        if (
                                            DataModul.DB_SourceLinkTable.Fields[0].AsInt() != 3 |
                                            DataModul.DB_SourceLinkTable.Fields[1].AsInt() > iFamPers |
                                            DataModul.DB_SourceLinkTable.Fields[1].AsInt() > persInArb1 |
                                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() != eEventArt |
                                            DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() != LfNR)
                                            break;

                                        DataModul.DB_QuTable.Index = "NR";
                                        DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                                        if (!DataModul.DB_QuTable.NoMatch)
                                        {
                                            if (QuText2 == "")
                                            {
                                                QuText2 = ". " + Modul1.IText[EUserText.t450] + " ";
                                            }
                                            QuText2 = QuText2.Trim().Length > 10
                                                ? (QuText2 + "; " + DataModul.DB_QuTable.Fields[QuFields._2].Value).AsString()
                                                : (QuText2 + DataModul.DB_QuTable.Fields[QuFields._2].Value).AsString();
                                            if (null != DataModul.DB_SourceLinkTable.Fields[3].Value & DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                            {
                                                QuText2 = null == DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value
                                                    ? QuText2 + " " + Modul1.IText[EUserText.t449] + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim()
                                                    : QuText2 + ", " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                            }
                                            if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value)
                                            {
                                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                                                {
                                                    QuText2 = (QuText2 + " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value + "<").AsString();
                                                }
                                                QuText2 = Zeiweg(QuText2, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                            }
                                            if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value)
                                            {
                                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                                {
                                                    QuText2 = (QuText2 + " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value + "==").AsString();
                                                }
                                                QuText2 = Zeiweg(QuText2, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                            }
                                        }
                                        DataModul.DB_SourceLinkTable.MoveNext();
                                    }
                                    if (null != evt.sBem[3])
                                    {
                                        if (evt.sBem[3].Trim() != "")
                                        {
                                            Modul1.Kont1[9] = evt.sBem[3].Trim();
                                        }
                                        if (Modul1.Kont1[9].Trim() != "")
                                        {
                                            if (QuText2 == "")
                                            {
                                                QuText2 = ". " + Modul1.IText[EUserText.t450] + " " + Modul1.Kont1[9].Trim();
                                            }
                                            else
                                            {
                                                QuText2 = QuText2 + "; " + Modul1.Kont1[9].Trim();
                                                Modul1.Kont1[9] = "";
                                            }
                                            if (QuText2.Trim() != "")
                                            {
                                                QuText2 = Zeiweg(QuText2, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                                QuText2 = QuText2.Trim();
                                                if (QuText2.Right(1) == ";")
                                                {
                                                    QuText2 = Strings.Trim(QuText2.Left(QuText2.Trim().Length - 1));
                                                }
                                                QuText2 = QuText2.Trim() + ".";
                                            }
                                        }
                                    }
                                }
                                if (eEventArt <= EEventArt.eA_499)
                                {
                                    int persInArb = persInArb1;
                                    int num15 = 1;
                                    while (num15 <= 100)
                                    {
                                        KontSP[num15] = Modul1.Kont[num15];
                                        Modul1.Kont[num15] = "";
                                        num15 += 1;
                                    }

                                    EEventArt num17 = eEventArt;
                                    if (Option[EOutCfg.o32])
                                    {
                                        Zeugsu(num17);
                                        Modul1.UbgT1 = DataModul.Event.GetValue(persInArb, num17, EventFields.Bem4, (f) => f.AsString());
                                    }
                                    persInArb1 = persInArb;
                                    num15 = 1;
                                    while (num15 <= 100)
                                    {
                                        Modul1.Kont[num15] = KontSP[num15];
                                        KontSP[num15] = "";
                                        num15 += 1;
                                    }
                                    persInArb1 = persInArb;
                                    if (Modul1.UbgT1.Trim() != "")
                                    {
                                        Modul1.UbgT1 = " Zeugen: " + Modul1.UbgT1.Trim() + ".";
                                    }
                                }
                                lErl = 10;
                                if (Modul1.Kont1[10].Trim() != "")
                                {
                                    Modul1.Kont1[10] = " " + Modul1.Kont1[10].Trim() + " ";
                                }
                                sEvent = sEvent.Trim() + Modul1.Kont1[10] + QuText2 + Modul1.UbgT1;
                                QuText2 = "";
                                Modul1.UbgT1 = "";
                                if (sEvent.Trim().Right(1) != ".")
                                {
                                    sEvent = sEvent.Trim() + ". ";
                                }
                                if (eEventArt == EEventArt.eA_105 || num10 == 0)
                                {
                                    AppendTextIfNE(rtbAnz, sEvent);
                                }
                                else if (eEventArt != EEventArt.eA_603)
                                {
                                    if (sEvent.Trim() != "")
                                    {
                                        rtbAnz.AppendText("\n" + sEvent);
                                    }
                                }
                                else if (sEvent.Trim() != "")
                                {
                                    rtbAnz.AppendText(sEvent);
                                }



                                lErl = 22;
                                num10 = (short)unchecked(num10 + 1);
                            }

                            {
                                eEventArt = 0;
                            }
                            goto end_IL_0001_2;
                        IL_2f90:
                            num7 = unchecked(num2 + 1);
                            goto IL_2f94;
                        IL_2f94:
                            num2 = 0;
                            switch (num7)
                            {
                                case 1:
                                    break;
                                case 17:
                                case 86:
                                case 102:
                                case 115:
                                case 131:
                                case 160:
                                case 450:
                                case 459:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 14030;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 8
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }

        void Anz_AppendNewlineIfNeeded(IDocument rtbAnz)
        {
            _ = rtbAnz.AppendTextIfNd("\n");
        }

        static void AppendTextIfNE(IDocument rtbAnz, string sText)
        {
            if (!string.IsNullOrWhiteSpace(sText))
            {
                rtbAnz.AppendText(sText);
            }
        }
    }

    public void Kindles(ref string KText, int famInArb, IDocument iDoc)
    {
        //Discarded unreachable code: IL_0846
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int num4;
        float Idned;
        switch (try0001_dispatch)
        {
            default:
                ProjectData.ClearProjectError();
                num3 = 2;
                num = 2;
                DataModul.Link.ReadFamily(famInArb, Modul1.Family);

                iDoc.SetIndent(0);
                if (Modul1.Family.Kinder.Count == 0)
                {
                    goto end_IL_0001_2;
                }

                iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                iDoc.AppendText("\n" + KText);
                iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                Kisort(Modul1.Family, List2_Items);
                short num11 = (short)(List2_Items.Count - 1);
                short num5 = 0;
                while (num5 < num11)
                {
                    iDoc.AppendText("\n");
                    iDoc.SetHangingIndent(30);

                    Modul1.PersInArb = List2_Items[num5].ItemData<int>();
                    iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    if (Option[EOutCfg.o10_EmitIDs])
                    {
                        iDoc.AppendText(num5 + 1.AsString() + ". <" + Modul1.PersInArb.AsString() + "> " + Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim());
                    }
                    else
                    {
                        iDoc.AppendText(num5 + 1.AsString() + ". " + Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim());
                    }
                    iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    iDoc.AppendText(" " + Modul1.Person.FullSurName.Trim());
                    iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    // Modul1.PersInArb = Modul1.PerSatzLes(Modul1.PersInArb, Modul1.Schalt);
                    iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    if (Modul1.Person.Alias != "")
                    {
                        iDoc.AppendText(" (" + Modul1.Person.Alias.TrimEnd() + ")");
                    }
                    else
                    {
                        iDoc.AppendText("");
                    }
                    iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    var pt = DataModul.Person.Seek(Modul1.PersInArb);
                    if (pt.Fields[PersonFields.religi].AsInt() > 0)
                    {
                        (var ubgT, Modul1.Kont[15]) = DataModul.TextLese2(pt.Fields[PersonFields.religi].AsInt());
                        iDoc.AppendText(", " + ubgT);
                    }
                    Bildaus("P");
                    AppendRelatives(Modul1.PersInArb, iDoc);
                    BemSch = 1;
                    ID = 0;
                    Idned = 0f;
                    Datr1(ref Idned, Modul1.PersInArb);
                    if (Option[EOutCfg.o01_Person]
                        && DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString() != " ")
                    {
                        string QuText = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                        QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                        if (QuText.Trim() != "")
                        {
                            iDoc.AppendText("{" + QuText + "}.\n");
                            QuText = "";
                        }
                    }
                    BemSch = 0;
                    ID = 0;
                    Kindhei(Modul1.PersInArb);
                    if (Option[EOutCfg.o14])
                    {
                        iDoc.SetIndent(20 + ID);
                    }
                    if (Option[EOutCfg.o14])
                    {
                        _ = iDoc.AppendTextIfNd("\n");
                    }
                    iDoc.SetIndent(0);
                    iDoc.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    num5 = (short)unchecked(num5 + 1);
                }
                goto end_IL_0001_2;

            IL_0898:
                num4 = unchecked(num2 + 1);
                goto IL_089c;
            IL_089c:
                num2 = 0;
                switch (num4)
                {
                    case 1:
                        break;
                    case 93:
                    case 94:
                    case 99:
                        goto end_IL_0001_2;
                }
                goto default;
        }
    end_IL_0001_2: // <========== 3
        return;
    }

    public void Kisort(IFamilyData family, IList<ListItem<int>> items)
    {
        items.Clear();
        foreach (var persInArb in family.Kind)
        {

            Datu = DataModul.Event.GetDate(EEventArt.eA_Birth, persInArb);
            if (Datu == default)
            {
                Datu = DataModul.Event.GetDate(EEventArt.eA_Baptism, persInArb);
            }
            string text;
            if (Datu == default)
                text = "00000000";
            else
                text = Datu.ToString("yyyyMMdd");

            items.Add(new($"{text} {persInArb}", persInArb));
        }
    }

    public void EPerles(short Index, int FamSP1, int Persp1, bool Opt10, IDocument document)
    {
        //Discarded unreachable code: IL_485c
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    int persInArb2;
                    int persInArb3;
                    string InText;
                    int Ortnr;
                    short Art;
                    float Idned;
                    var person = new CPersonData();
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 21374:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_48b4;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_48b8;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            // Referenzen
                            int M1_PersInArb = Modul1.PersInArb;
                            var M1_Kont0 = Modul1.Person.SurName;
                            var M1_Givennames = Modul1.Person.Givennames;
                            var M1_Kont4 = Modul1.Person.Alias;
                            var M1_Kont5 = Modul1.Person.Clan;
                            var M1_Kont6 = Modul1.Person.Stat;
                            var M1_Prae = Modul1.Person.Prae;
                            var M1_Kont11 = Modul1.Person.Birthday;
                            var M1_Kont12 = Modul1.Person.Baptised;
                            var M1_Kont13 = Modul1.Person.Death;
                            var M1_Kont14 = Modul1.Person.Burial;
                            string M1_Kont16 = "";
                            string M1_Kont17 = "";
                            string M1_Kont21 = "";
                            string M1_Kont22 = "";
                            string M1_Kont25 = "";
                            string M1_Kont31 = "";
                            string M1_Kont32 = "";
                            string M1_Kont41 = "";
                            string M1_Kont42 = "";

                            int M1_Ubg;
                            string M1_UbgT;
                            int persInArb = M1_PersInArb;
                            var M1_DTxt = Modul1.DTxt;

                            Font font_Arial_11_Bold = new("Arial", 11.01f, FontStyle.Bold);
                            Font font_Arial_11_Reg = new("Arial", 11.01f, FontStyle.Regular);

                            var dB_PersonTable = DataModul.DB_PersonTable;

                            Modul1.Person_ReadNames(M1_PersInArb, person);
                            int Pers_iReligi = dB_PersonTable.Fields[PersonFields.religi].AsInt();

                            if (M1_Prae.Trim() != "")
                            {
                                M1_Prae = M1_Prae.Trim() + " ";
                            }

                            if (Option[EOutCfg.o10_EmitIDs])
                            {
                                document.AppendText($"<{M1_PersInArb}> ");
                            }
                            document.AppendText(M1_Prae);
                            document.AppendText(M1_Givennames.TrimEnd() + " ");
                            _ = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                            string selectedText = M1_Kont0.TrimEnd();
                            document.SetFont(font_Arial_11_Bold);
                            document.AppendText(selectedText);
                            document.SetFont(font_Arial_11_Reg);
                            if (M1_Kont5 != "")
                            {
                                document.AppendText($" Sippe {M1_Kont5.TrimEnd()}");
                            }
                            //M1_PersInArb = Modul1.PerSatzLes(M1_PersInArb, Modul1.Schalt);
                            document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                            if (M1_Kont4 != "")
                            {
                                document.AppendText($" ({M1_Kont4.TrimEnd()})");
                            }
                            else
                            {
                                document.AppendText("");
                            }
                            document.SetFont(font_Arial_11_Reg);

                            if (Pers_iReligi > 0)
                            {
                                M1_Ubg = Pers_iReligi.AsInt();
                                M1_UbgT = DataModul.TextLese1(M1_Ubg);
                                document.AppendText(", " + M1_UbgT);
                            }
                            Bildaus("P");
                            if (M1_Kont6.Trim() != "")
                            {
                                document.AppendText(" " + M1_Kont6.TrimEnd() + " ");
                            }
                            document.SetFont(font_Arial_11_Reg);
                            AppendRelatives(Modul1.PersInArb, document);
                            Datles1(Modul1.PersInArb, Modul1.Person);
                            document.AppendText(M1_Kont25);
                            Retweg3(Document);
                            document.SetHangingIndent(40);
                            document.AppendText("\n");
                            document.SetFont(font_Arial_11_Reg);
                            document.SetIndent(0);
                            Datles1(Modul1.PersInArb, Modul1.Person);
                            string QuText;
                            if (M1_Kont11.Trim() != ""
                                | M1_Kont16.Length > 0
                                | M1_Kont21.Length > 0
                                | M1_Kont31.Trim() != ""
                                | M1_Kont41.Trim() != "")
                            {
                                document.SetIndent(20);
                                document.AppendText(M1_DTxt[1] + " " + M1_Kont11.Trim() + ".");
                                if (Option[EOutCfg.o02]
                                    && M1_Kont16.Trim() != "")
                                {
                                    QuText = M1_Kont16;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (Option[EOutCfg.o03]
                                    && M1_Kont21.Trim() != "")
                                {
                                    QuText = M1_Kont21;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (M1_Kont31.Trim() != "")
                                {
                                    QuText = M1_Kont31;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (document.AppendTextIfNd("."))
                                    document.AppendText(" ");
                                if (M1_Kont41 != "")
                                {
                                    document.AppendText(" " + M1_Kont41);
                                }
                            }
                            if (M1_Kont12.Trim() != ""
                                | M1_Kont17.Length > 0
                                | M1_Kont22.Length > 0
                                | M1_Kont32.Trim() != ""
                                | M1_Kont42.Trim() != "")
                            {
                                document.AppendText("\n" + M1_DTxt[2] + " " + M1_Kont12.Trim() + ".");
                                if (Option[EOutCfg.o02]
                                    && M1_Kont17.Length > 0)
                                {
                                    QuText = M1_Kont17;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                if (Option[EOutCfg.o03])
                                {
                                    if (M1_Kont22.Length > 0)
                                    {
                                        QuText = M1_Kont22;
                                        QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                        document.AppendText(" " + QuText);
                                        QuText = "";
                                    }
                                }
                                if (M1_Kont32.Trim() != "")
                                {
                                    QuText = M1_Kont32;
                                    QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                    document.AppendText(" " + QuText);
                                    QuText = "";
                                }
                                _ = document.AppendTextIfNd(".");
                                if (M1_Kont42 != "")
                                {
                                    document.AppendText(" " + M1_Kont42);
                                }
                            }
                            int num11;
                            if (Index == 1 | Index == 2)
                            {
                                if (Option[EOutCfg.o30])
                                {
                                    persInArb2 = M1_PersInArb;
                                    int num5 = 0;

                                    foreach (var Link in DataModul.Link.ReadAllFams(persInArb2, ELinkKennz.lkGodparent))
                                    {
                                        Modul1.Person_ReadNames(Link.iPersNr, Modul1.Person);
                                        var sFullname = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                        document.SetFont(font_Arial_11_Reg);
                                        document.SetIndent(20);
                                        if (num5 == 0)
                                        {
                                            document.AppendText("\n");
                                            document.SetFont(font_Arial_11_Reg);
                                            document.AppendText(" " + Modul1.IText[EUserText.tGodparents].Replace("&", "") + ": " + (M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + sFullname.Trim());
                                            num5 = 1;
                                        }
                                        else
                                        {
                                            document.AppendText((M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + sFullname.Trim());
                                        }
                                        if (Option[EOutCfg.o31_GodpWithoutData])
                                        {
                                            Datles1(Link.iPersNr, Modul1.Person);
                                            if (M1_Kont11 == "")
                                            {
                                                M1_Kont11 = M1_Kont12;
                                            }
                                            if (M1_Kont11.Trim() != "")
                                            {
                                                document.AppendText(" * " + M1_Kont11.Trim());
                                            }
                                            if (M1_Kont13 == "")
                                            {
                                                M1_Kont13 = M1_Kont14;
                                            }
                                            if (M1_Kont14.Trim() != "" & M1_Kont11.Trim() != "")
                                            {
                                                document.AppendText(",");
                                            }
                                            if (M1_Kont14.Trim() != "")
                                            {
                                                document.AppendText(M1_DTxt[3] + " " + M1_Kont13.Trim());
                                            }
                                            document.AppendText("; ");
                                        }
                                        else
                                        {
                                            document.AppendText("; ");
                                        }
                                    }
                                    M1_PersInArb = persInArb;
                                    var dB_PersonTable2 = DataModul.Person.Seek(M1_PersInArb);
                                    string Pers_sBem2 = dB_PersonTable2.Fields[PersonFields.Bem2].AsString();

                                    if (null != Pers_sBem2
                                        && Pers_sBem2.Trim() != "")
                                    {
                                        if (num5 == 0)
                                        {
                                            document.AppendText(" " + Modul1.IText[EUserText.tGodparents].Replace("&", "") + ": ");
                                        }
                                        document.AppendText(Pers_sBem2.Trim());
                                    }
                                    M1_PersInArb = persInArb;
                                }
                                _ = document.AppendTextIfNd(".");
                                if (Option[EOutCfg.o40])
                                {
                                    Sterdat(0);
                                }
                                _ = TrimEnd(document);
                                _ = document.TrimEnd(";");
                                _ = document.AppendTextIfNd(".");
                                var liList5 = new List<(DateTime, int)>();
                                if (Option[EOutCfg.o38])
                                {
                                    liList5.Clear();
                                    PersNr = M1_PersInArb.AsInt();

                                    foreach (var link in DataModul.Link.ReadAllPers(M1_PersInArb, ELinkKennz.lkGodparent))
                                    {
                                        M1_PersInArb = link.iFamNr;

                                        Datu = DataModul.Event.GetDate(EEventArt.eA_Baptism, M1_PersInArb, out _);
                                        if (Datu == default)
                                        {
                                            Datu = DataModul.Event.GetDate(EEventArt.eA_Birth, M1_PersInArb, out _);
                                        }

                                        liList5.Add((Datu, M1_PersInArb));
                                    }

                                    short num6 = (short)(liList5.Count - 1);
                                    short num7 = 0;
                                    while (num7 <= num6)
                                    {
                                        M1_PersInArb = liList5[num7].Item2;
                                        Modul1.Person_ReadNames(M1_PersInArb, Modul1.Person);
                                        _ = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                        Datu = liList5[num7].Item1;

                                        document.AppendText("\n" + Modul1.IText[EUserText.t348] + ": " + Datu + " bei " + (M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + M1_Kont0.Trim());
                                        M1_PersInArb = PersNr;
                                        num7 = (short)unchecked(num7 + 1);
                                    }
                                }
                                M1_PersInArb = persInArb;
                                if (Option[EOutCfg.o37])
                                {
                                    Zeuge_Bei(Modul1.PersInArb);
                                }
                                M1_PersInArb = persInArb;
                                persInArb3 = M1_PersInArb;
                                foreach (var link in DataModul.Link.ReadAllFams(M1_PersInArb, ELinkKennz.lk9))
                                {
                                    M1_PersInArb = link.iPersNr;
                                    Modul1.Person_ReadNames(M1_PersInArb, Modul1.Person);
                                    M1_Kont0 = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                    document.AppendText("\n");
                                    document.AppendText("Verbundene Person: ");
                                    document.AppendText((M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + M1_Kont0.Trim());
                                    Datles1(Modul1.PersInArb, Modul1.Person);
                                    if (M1_Kont11 == "")
                                    {
                                        M1_Kont11 = M1_Kont12;
                                    }
                                    if (M1_Kont11.Trim() != "")
                                    {
                                        document.AppendText(" * " + M1_Kont11.Trim() + ".");
                                    }
                                    if (M1_Kont13 == "")
                                    {
                                        M1_Kont13 = M1_Kont14;
                                    }
                                    if (M1_Kont14.Trim() != "")
                                    {
                                        document.AppendText(M1_DTxt[3] + " " + M1_Kont13.Trim() + ".");
                                    }
                                    M1_PersInArb = persInArb;
                                }
                                M1_PersInArb = persInArb;
                                foreach (var link in DataModul.Link.ReadAllPers(M1_PersInArb, ELinkKennz.lk9))
                                {
                                    M1_PersInArb = link.iFamNr;
                                    Modul1.Person_ReadNames(M1_PersInArb, Modul1.Person);
                                    M1_Kont0 = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                                    /* ??
                                    Datu = DataModul.Event_GetDate(M1_PersInArb, EEventArt.eA_Baptism, out Modul1.Ds);
                                    if (Datu == default)
                                    {
                                        Datu = DataModul.Event_GetDate(M1_PersInArb, EEventArt.eA_Birth, out Modul1.Ds);
                                    }

                                    if (Datu != default)
                                        sDatu = Datwand1(Datu, Modul1.Ds);
                                    else
                                    {
                                        sDatu = "";
                                    }
                                    */
                                    document.AppendText("\nVerbunden mit " + (M1_Prae.Trim() + " " + M1_Givennames.Trim()).Trim() + " " + M1_Kont0.Trim());
                                    M1_PersInArb = persInArb;
                                }
                                Modul1.eLKennz = ELinkKennz.lkWitnOfEngage;
                                var aiFam = DataModul.Link.GetPersonFams(M1_PersInArb, Modul1.eLKennz);

                                liList5.Clear();
                                int num10 = aiFam.Count;
                                num11 = 1;
                                while (num11 <= num10)
                                {
                                    Modul1.FamInArb = aiFam[num11 - 1];
                                    Datu = DataModul.Event.GetDate(EEventArt.eA_MarrReligious, aiFam[num11 - 1]);

                                    liList5.Add((Datu, Modul1.FamInArb));
                                    num11++;
                                }
                                InText = "Verlobungszeuge";
                                Art = 501;
                                Zeugenaus(ref InText, ref Art, liList5);
                                M1_PersInArb = persInArb;
                                aiFam.Clear();
                                Modul1.eLKennz = ELinkKennz.lkMarrWitness;
                                aiFam = DataModul.Link.GetPersonFams(M1_PersInArb, Modul1.eLKennz);
                                liList5.Clear();
                                int num14 = aiFam.Count;
                                num11 = 1;
                                while (num11 <= num14)
                                {
                                    Modul1.FamInArb = aiFam[num11 - 1];
                                    Datu = DataModul.Event.GetDate(EEventArt.eA_MarrReligious, aiFam[num11 - 1]);

                                    liList5.Add((Datu, Modul1.FamInArb));
                                    num11++;
                                }
                                InText = "Trauzeuge";
                                Art = 502;
                                Zeugenaus(ref InText, ref Art, liList5);
                                M1_PersInArb = persInArb;
                                aiFam.Clear();
                                Modul1.eLKennz = ELinkKennz.lkWitnOfMarr;
                                aiFam = DataModul.Link.GetPersonFams(M1_PersInArb, Modul1.eLKennz);
                                liList5.Clear();
                                int num16 = aiFam.Count;
                                num11 = 1;
                                while (num11 <= num16)
                                {
                                    Modul1.FamInArb = aiFam[num11 - 1];
                                    Datu = DataModul.Event.GetDate(EEventArt.eA_MarrReligious, aiFam[num11 - 1]);

                                    liList5.Add((Datu, Modul1.FamInArb));
                                    num11++;
                                }
                                InText = "kirchl. Trauzeuge";
                                Art = 503;
                                Zeugenaus(ref InText, ref Art, liList5);
                                M1_PersInArb = persInArb;
                            }
                            if (Option[EOutCfg.o43] | false)
                            {
                                Berufe(EEventArt.eA_300, Document);
                                Berufe(EEventArt.eA_301, Document);
                                Berufe(EEventArt.eA_302, Document);
                                _ = document.AppendTextIfNd();
                                Berufe(EEventArt.eA_105, Document);
                                _ = document.AppendTextIfNd();
                                document.SetFont(font_Arial_11_Reg);
                                Berufe(EEventArt.eA_106, Document);
                            }
                            _ = document.AppendTextIfNd();
                            document.SetFont(font_Arial_11_Reg);
                            if (Option[EOutCfg.o40])
                            {
                                Sterdat(1);
                            }
                            _ = document.AppendTextIfNd();

                            string Pers_sBem1 = dB_PersonTable.Fields[PersonFields.Bem1].AsString();

                            if (Option[EOutCfg.o01_Person] && Pers_sBem1.Trim() != "")
                            {
                                QuText = Pers_sBem1.Trim();
                                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                document.AppendText("{" + QuText + "}.\n");
                                QuText = "";
                            }
                            Quellen();
                            Persp1 = M1_PersInArb;
                            Modul1.eLKennz = ELinkKennz.lkFather;
                            if (dB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                            }
                            if (Index == 2)
                            {
                                Weitehen(FamSP1, GetFraPreview11());
                            }
                            M1_PersInArb = persInArb;
                            Modul1.Family.Mann = 0;
                            Modul1.Family.Frau = 0;
                            M1_Ubg = Adoeltsuch(Modul1.PersInArb);
                            Adoelt(M1_Ubg, Document);
                            M1_PersInArb = persInArb;
                            Modul1.Family.Mann = 0;
                            Modul1.Family.Frau = 0;
                            M1_Ubg = Modul1.Eltsuch(M1_PersInArb);
                            if (M1_Ubg > 0)
                                do
                                {
                                    document.AppendText("\n");
                                    document.SetIndent(40);
                                    ID = 200;
                                    document.SetFont(font_Arial_11_Bold);
                                    document.AppendText(Modul1.IText[EUserText.t82] + "\n");
                                    document.SetFont(font_Arial_11_Reg);
                                    Modul1.FamInArb = M1_Ubg;
                                    if (!DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkFather, out M1_PersInArb))
                                    {
                                        Modul1.Family.Mann = M1_PersInArb;

                                        Modul1.Person_ReadNames(M1_PersInArb, person);
                                        string FullName = Namerw(person.SurName, person.Prefix, person.Suffix);
                                        if (Option[EOutCfg.o10_EmitIDs])
                                        {
                                            document.AppendText("<" + M1_PersInArb.AsString() + "> " + M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                        }
                                        else
                                        {
                                            document.AppendText(M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                        }
                                        document.SetFont(font_Arial_11_Bold);
                                        document.AppendText(" " + FullName);
                                        document.SetFont(font_Arial_11_Reg);
                                        if (M1_Kont4 != "")
                                        {
                                            document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                                            document.AppendText(" (" + M1_Kont4.TrimEnd() + ")");
                                        }
                                        else
                                        {
                                            document.AppendText("");
                                        }
                                        document.SetFont(font_Arial_11_Reg);
                                        var pt = DataModul.Person.Seek(M1_PersInArb);
                                        if (pt.Fields[PersonFields.religi].AsInt() > 0)
                                        {
                                            M1_Ubg = pt.Fields[PersonFields.religi].AsInt();
                                            M1_UbgT = DataModul.TextLese1(M1_Ubg);
                                            document.AppendText(", " + M1_UbgT);
                                        }
                                        Bildaus("P");
                                        AppendRelatives(Modul1.PersInArb, document);
                                    }
                                    else
                                    {
                                        document.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                                        M1_PersInArb = 0;
                                    }
                                    document.SetFont(font_Arial_11_Reg);
                                    BemSch = 0;
                                    Datu = default;
                                    if (M1_PersInArb > 0)
                                    {
                                        Idned = 0f;
                                        Datr1(ref Idned, Modul1.PersInArb);
                                    }
                                    num11 = 1;
                                    while (num11 <= 6)
                                    {
                                        Modul1.Kont1[num11] = "";
                                        num11++;
                                    }
                                    EEventArt eArt;
                                    if ((!DataModul.Event.ReadData((eArt = EEventArt.eA_Marriage, Modul1.FamInArb, 0), out var cEv)
                                            || cEv.dDatumV == default && cEv.dDatumB == default)
                                        && (!DataModul.Event.Exists((eArt = EEventArt.eA_MarrReligious, Modul1.FamInArb, 0))
                                            || cEv.dDatumV == default && cEv.dDatumB == default))
                                    {
                                        break;
                                    }
                                    Modul1_priv = cEv.iPrivacy;
                                    if (!(Modul1_priv > privaus))
                                    {
                                        if (cEv.dDatumV != default)
                                        {
                                            Datu = cEv.dDatumV;
                                            var sDatu = Datwand1(Datu, cEv.sDatumV_S);
                                            Modul1.Kont1[1] = sDatu;
                                        }
                                        if (cEv.dDatumB != default)
                                        {
                                            Datu = cEv.dDatumB;
                                            var sDatu = Datwand1(Datu, cEv.sDatumB_S);
                                            if (Modul1.Kont1[1] != "")
                                            {
                                                sDatu = " / " + sDatu;
                                            }
                                            Modul1.Kont1[3] = sDatu;
                                        }
                                        M1_UbgT = "";
                                        if (cEv.iOrt > 0)
                                        {
                                            Ortnr = cEv.iOrt;
                                            Modul1.UbgT = Place_ReadData(Ortnr, 1, 0, Option[EOutCfg.o35], cEv.sZusatz);
                                            if (Strings.Trim(cEv.sOrt_S) != "")
                                            {
                                                M1_UbgT = M1_UbgT.TrimEnd() + " " + Strings.Trim(cEv.sOrt_S);
                                            }
                                        }
                                        num11 = 1;
                                        while (num11 <= 6)
                                        {
                                            if (Modul1.Kont1[num11] == "0")
                                            {
                                                Modul1.Kont1[num11] = "";
                                            }
                                            num11++;
                                        }
                                        document.SetIndent(50);
                                        if (Option[EOutCfg.o14])
                                        {
                                            document.SetIndent(70);
                                        }
                                        _ = document.AppendTextIfNd(" ");

                                        string text2 = "";
                                        if (cEv.eArt == EEventArt.eA_Marriage)
                                        {
                                            text2 = M1_DTxt[7];
                                        }
                                        if (cEv.eArt == EEventArt.eA_MarrReligious)
                                        {
                                            text2 = M1_DTxt[8];
                                        }
                                        _ = document.AppendTextIfNd();
                                        if (Option[EOutCfg.o34])
                                        {
                                            if ("" != cEv.sReg)
                                            {
                                                if (cEv.sReg.Trim() != "")
                                                {
                                                    M1_UbgT = M1_UbgT + " (Urk.-Nr.: " + cEv.sReg.Trim() + ") ";
                                                }
                                            }
                                        }
                                        if (Option[EOutCfg.o10_EmitIDs])
                                        {
                                            document.AppendText(text2 + " [" + Modul1.FamInArb.AsString() + "] " + Modul1.Kont1[1] + " " + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + M1_UbgT + " mit\n");
                                            M1_UbgT = "";
                                        }
                                        else
                                        {
                                            document.AppendText(text2 + " " + Modul1.Kont1[1] + " " + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + M1_UbgT + " mit\n");
                                            M1_UbgT = "";
                                        }
                                    }
                                }
                                while (false);
                            else
                            {
                                Modul1.FamInArb = 0;
                                goto end_IL_0001_2;
                            }
                            lErl = 33;
                            document.SetIndent(40);
                            _ = document.AppendTextIfNd();
                            document.SetFont(font_Arial_11_Reg);
                            if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out M1_PersInArb))
                            {
                                Modul1.Family.Frau = M1_PersInArb;

                                Modul1.Person_ReadNames(M1_PersInArb, person);
                                var FullName = Namerw(person.SurName, person.Prefix, person.Suffix);
                                if (Option[EOutCfg.o10_EmitIDs])
                                {
                                    document.AppendText("<" + M1_PersInArb.AsString() + "> " + M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                }
                                else
                                {
                                    document.AppendText(M1_Prae.Trim() + " " + M1_Givennames.Trim());
                                }
                                document.SetFont(font_Arial_11_Bold);
                                document.AppendText(" " + FullName);
                                document.SetFont(font_Arial_11_Reg);
                                document.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                                if (M1_Kont4 != "")
                                {
                                    document.AppendText(" (" + M1_Kont4.TrimEnd() + ")");
                                }
                                else
                                {
                                    document.AppendText("");
                                }
                                document.SetFont(font_Arial_11_Reg);
                                var pt = DataModul.Person.Seek(M1_PersInArb);
                                if (pt.Fields[PersonFields.religi].AsInt() > 0)
                                {
                                    M1_Ubg = pt.Fields[PersonFields.religi].AsInt().AsInt();
                                    M1_UbgT = DataModul.TextLese1(M1_Ubg);
                                    document.AppendText(", " + M1_UbgT);
                                }
                                Bildaus("P");
                                AppendRelatives(Modul1.PersInArb, document);
                                Idned = 0f;
                                Datr1(ref Idned, Modul1.PersInArb);
                                _ = document.AppendTextIfNd();
                                document.SetFont(font_Arial_11_Reg);
                            }
                            else
                            {
                                document.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                                document.SetFont(font_Arial_11_Reg);
                            }
                            goto end_IL_0001_2;
                        IL_48b4:
                            num4 = unchecked(num2 + 1);
                            goto IL_48b8;
                        IL_48b8:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 669:
                                case 674:
                                case 681:
                                case 686:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 21374;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }



    public void AppendRelatives(int persInArb, IDocument edtAnzeige)
    {
        bool xAppendAnc = Option[EOutCfg.o11];
        bool xAppendDesc = Option[EOutCfg.o12];
        if (xAppendAnc
            || xAppendDesc)
        {
            Font fArial11Underl = new Font("Arial", 11.01f, FontStyle.Underline);
            Font fArial11Reg = new Font("Arial", 11.01f, FontStyle.Regular);
            Ahnles(persInArb, out _, out string Modul1_Kont11, out string Modul1_Kont13, out _);
            if (xAppendAnc && Modul1_Kont11.Trim() != "")
            {
                edtAnzeige.SetFont(fArial11Underl);
                edtAnzeige.AppendText(" ");
                edtAnzeige.AppendText("Ahnen-Nr.: " + Modul1_Kont11.Trim() + ".");
                edtAnzeige.SetFont(fArial11Reg);
            }
            if (xAppendDesc && Modul1_Kont13.Trim() != "")
            {
                edtAnzeige.AppendText(" ");
                edtAnzeige.SetFont(fArial11Underl);
                edtAnzeige.AppendText(Modul1_Kont13);
                edtAnzeige.SetFont(fArial11Reg);
            }
            edtAnzeige.SetFont(fArial11Reg);
        }
    }

    public void Kindhei(int persInArb1)
    {
        string sPerSex;
        Modul1.eLKennz = (sPerSex = DataModul.Person.GetSex(persInArb1)) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;

        var aiFam = Modul1.Link_Famsuch(persInArb1, Modul1.eLKennz);
        var liList5 = new List<(DateTime, int)>();
        liList5.Clear();
        checked
        {
            int num = aiFam.Count;
            int M1_Iter = 1;
            while (M1_Iter <= num)
            {
                Modul1.FamInArb = aiFam[M1_Iter - 1];
                EEventArt num3 = EEventArt.eA_500;
                while (num3 <= EEventArt.eA_505)
                {
                    if ((Datu = DataModul.Event.GetDate(num3, Modul1.FamInArb)) != default
                        && num3 != EEventArt.eA_504)
                        break;
                    num3++;
                }
                if (Datu.AsInt() == 0.0)
                {
                    Datu = DataModul.Event.GetDate(EEventArt.eA_601, Modul1.FamInArb);
                }
                liList5.Add((Datu, Modul1.FamInArb));
                M1_Iter++;
            }

            IDocument edtAnzeige = Document;
            if (Option[EOutCfg.o14])
            {
                _ = edtAnzeige.AppendTextIfNd();
                edtAnzeige.SetIndent(90);
            }
            edtAnzeige.SetIndent(0);
            _ = edtAnzeige.AppendTextIfNd(" ");
            if (liList5.Count == 1)
            {
                edtAnzeige.AppendText("Verbindung:");
            }
            if (liList5.Count > 1)
            {
                edtAnzeige.AppendText("Verbindungen:");
            }
            short num6 = (short)(liList5.Count - 1);
            short num7 = 0;
            while (num7 <= num6)
            {
                Modul1.FamInArb = liList5[num7].Item2;
                Heidat(out Scheid, Modul1.FamInArb, Document);
                LPweg();
                Bildaus("F");
                if (Option[EOutCfg.o14])
                {
                    edtAnzeige.AppendText("\n");
                    edtAnzeige.SetIndent(30);
                    edtAnzeige.AppendText("mit ");
                }
                else
                {
                    edtAnzeige.AppendText(" mit ");
                }
                Modul1.eLKennz = sPerSex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                if (DataModul.Link.GetFamPerson(Modul1.FamInArb, Modul1.eLKennz, out var persInArb2))
                {
                    Modul1.Person_ReadNames(persInArb2, Modul1.Person);
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    if (Option[EOutCfg.o10_EmitIDs])
                    {
                        edtAnzeige.AppendText("<" + persInArb2.AsString() + "> " + Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim());
                    }
                    else
                    {
                        edtAnzeige.AppendText(Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim());
                    }
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                    edtAnzeige.AppendText(" " + Modul1.Person.FullSurName);
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                    if (Modul1.Person.Alias != "")
                    {
                        edtAnzeige.AppendText(" (" + Modul1.Person.Alias.TrimEnd() + ") ");
                    }
                    else
                    {
                        edtAnzeige.AppendText("");
                    }
                    edtAnzeige.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                    var pt = DataModul.Person.Seek(persInArb2);
                    if (pt.Fields[PersonFields.religi].AsInt() > 0)
                    {
                        var ubg = pt.Fields[PersonFields.religi].AsInt();
                        _ = DataModul.Textlese(ubg, out var ubgT, out _);
                        edtAnzeige.AppendText(", " + ubgT);
                    }
                    Bildaus("P");
                    AppendRelatives(persInArb2, edtAnzeige);
                    ID = 600;
                    float Idned = 0f;
                    Datr1(ref Idned, persInArb2);
                }
                else
                {
                    edtAnzeige.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                }
                num7++;
            }
            edtAnzeige.SetIndent(0);
        }
    }

    public void Heidat(out bool scheid, int famInArb, IDocument richTextBox)
    {
        //Discarded unreachable code: IL_1d96
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num4 = default;
        int num6 = default;
        EEventArt eArt = default;
        string text = default;
        string QuText = default;
        scheid = false;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num5;
                    int Ortnr;
                    string LD2;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 8948:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1e26;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_1e26;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num5 = num2;
                                    goto IL_1e2a;
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            richTextBox.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            if (Option[EOutCfg.o10_EmitIDs])
                            {
                                richTextBox.AppendText("[" + famInArb.AsString() + "]");
                            }
                            DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                            DataModul.DB_FamilyTable.Seek("=", famInArb);
                            if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1)
                            {
                                richTextBox.AppendText("Aussereheliche Verbindung");
                            }
                            richTextBox.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                            num6 = 0;
                            goto IL_0159;
                        IL_0159: // <========== 3
                            num = 13;
                            eArt = num6 switch
                            {
                                0 => EEventArt.eA_501,
                                1 => EEventArt.eA_500,
                                2 => EEventArt.eA_507,
                                3 => EEventArt.eA_505,
                                4 => EEventArt.eA_Marriage,
                                5 => EEventArt.eA_MarrReligious,
                                6 => EEventArt.eA_504,
                                7 => EEventArt.eA_506,
                                _ => eArt,
                            };
                            Datu = default;
                            short num10 = 1;
                            while (num10 <= 15)
                            {
                                Modul1.Kont1[num10] = "";
                                num10++;
                            }
                            if (DataModul.Event.ReadData(eArt, famInArb, out var cEv) && !(cEv.iPrivacy > privaus))
                            {
                                Datu = cEv.dDatumV;
                                if (Datu.AsInt() > 0.0)
                                {
                                    var sDatu = Datwand1(Datu, cEv.sDatumV_S);
                                    Modul1.Kont1[1] = Datu.AsString();
                                    if (cEv.sVChr != "0")
                                    {
                                        Modul1.Kont1[1] = Modul1.Kont1[1] + " VChr";
                                    }
                                }
                                else
                                {
                                    Modul1.Kont1[1] = "";
                                }
                                if (cEv.dDatumB != default)
                                {
                                    Datu = cEv.dDatumB;
                                    var sDatu = Datwand1(Datu, cEv.sDatumB_S);
                                    if (Modul1.Kont1[1] != "")
                                    {
                                        sDatu = " / " + sDatu;
                                    }
                                    Modul1.Kont1[3] = sDatu;
                                }
                                Modul1.UbgT = "";
                                if (cEv.iOrt > 0)
                                {
                                    Modul1.UbgT = Place_ReadData(cEv.iOrt, 0, 0, Option[EOutCfg.o35], cEv.sZusatz);
                                    if (cEv.sOrt_S.Trim() != "")
                                    {
                                        Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + Strings.Trim(cEv.sOrt_S);
                                    }
                                    Modul1.Kont1[5] = Modul1.UbgT;
                                }
                                if (0 != cEv.iDatumText)
                                {
                                    Modul1.Kont1[14] = " " + cEv.sDatumText + " ";
                                }
                                if (cEv.iKBem > 0)
                                {
                                    Modul1.Kont1[7] = cEv.sKBem.Trim();
                                }
                                if (0 != cEv.iHausNr)
                                {
                                    Modul1.Kont1[7] = Modul1.Kont1[7] + " " + cEv.sHausNr.Trim() + " ";
                                }
                                if (cEv.iPlatz > 0)
                                {
                                    Modul1.Kont1[6] = cEv.sPlatz.Trim();
                                }

                                bool flag = false;
                                DataModul.DB_SourceLinkTable.Index = "Tab22";
                                DataModul.DB_SourceLinkTable.Seek("=", 3, famInArb, cEv.eArt, 0);
                                flag |= !DataModul.DB_SourceLinkTable.NoMatch;
                                flag |= "" != cEv.sBem[3];
                                flag |= Option[EOutCfg.o05] & cEv.sBem[1].Trim() != "";
                                flag |= Option[EOutCfg.o06] & cEv.sBem[2].Trim() != "";
                                flag |= Modul1.Kont1[1].Trim() != ""
                                    | Modul1.Kont1[2].Trim() != ""
                                    | Modul1.Kont1[3].Trim() != ""
                                    | Modul1.Kont1[5].Trim() != ""
                                    | Modul1.Kont1[6].Trim() != ""
                                    | Modul1.Kont1[7].Trim() != ""
                                    | Modul1.UbgT.Trim() != "";
                                if (flag)
                                {
                                    text = "";
                                    scheid |= eArt == EEventArt.eA_504;
                                    text = eArt switch
                                    {
                                        EEventArt.eA_500 => Modul1.DTxt[5],
                                        EEventArt.eA_501 => Modul1.DTxt[6],
                                        EEventArt.eA_Marriage => Modul1.DTxt[7],
                                        EEventArt.eA_MarrReligious => Modul1.DTxt[8],
                                        EEventArt.eA_504 => Modul1.DTxt[9],
                                        EEventArt.eA_505 => Modul1.DTxt[10],
                                        EEventArt.eA_507 => Modul1.DTxt[15],
                                        _ => text,
                                    };
                                    _ = richTextBox.AppendTextIfNd(" ");
                                    if (Option[EOutCfg.o14])
                                    {
                                        richTextBox.AppendText("\n");
                                        if (richTextBox.GetIndent() == 0)
                                        {
                                            richTextBox.SetIndent(20);
                                        }
                                    }
                                    Modul1.Job = Jobdreh(Modul1.Job, ereiRf: EreiRf);
                                    if (Option[EOutCfg.o34] && "" != cEv.sReg.Trim())
                                    {
                                        Modul1.Job = Modul1.Job + " (Urk.-Nr.: " + Strings.Trim(cEv.sReg) + ") ";
                                    }
                                    richTextBox.AppendText(text + " " + Modul1.Job);
                                    Modul1.Job = "";
                                    LfNR = 0;
                                    Modul1.Kont1[20] = "";
                                    if (Strings.RTrim(cEv.sBem[1]) != "" && Option[EOutCfg.o05])
                                    {
                                        richTextBox.AppendText(" {" + Strings.RTrim(cEv.sBem[1]) + "}");
                                    }
                                    if (Strings.RTrim(cEv.sBem[2]) != "")
                                    {
                                        if (Option[EOutCfg.o06])
                                        {
                                            richTextBox.AppendText(" {" + Strings.RTrim(cEv.sBem[2]) + "}");
                                        }
                                    }
                                    if (Option[EOutCfg.o39])
                                    {
                                        QuText = "";
                                        //flag2 = false;
                                        DataModul.DB_SourceLinkTable.Index = "Tab22";
                                        DataModul.DB_SourceLinkTable.Seek("=", 3, famInArb, cEv.eArt, 0);
                                        if (!DataModul.DB_SourceLinkTable.NoMatch)
                                        {

                                            _ = richTextBox.AppendTextIfNd(".");
                                            richTextBox.AppendText(Modul1.IText[EUserText.t450] + " ");
                                            //flag2 = true;
                                            while (!DataModul.DB_SourceLinkTable.EOF
                                                && !DataModul.DB_SourceLinkTable.NoMatch
                                                && 0 != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsInt()
                                                && DataModul.DB_SourceLinkTable.Fields[0].AsInt() == 3
                                                && DataModul.DB_SourceLinkTable.Fields[1].AsInt() <= famInArb
                                                && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Art].AsEnum<EEventArt>() == cEv.eArt
                                                && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].AsInt() == 0)
                                            {
                                                DataModul.DB_QuTable.Index = "Nr";
                                                DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[2]);
                                                QuText = QuText != ""
                                                    ? (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].Value).AsString()
                                                    : DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                                                if (DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                                {
                                                    QuText = null == DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value
                                                        ? QuText + ", " + Modul1.IText[EUserText.t449] + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim()
                                                        : QuText + ", " + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim();
                                                }
                                                if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value)
                                                {
                                                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                                                    {
                                                        QuText = string.Concat(QuText, " >" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() + "<");
                                                    }
                                                }
                                                if (null != DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value)
                                                {
                                                    if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                                    {
                                                        QuText = string.Concat(QuText, " ==" + DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() + "==");
                                                    }
                                                }
                                                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                                                DataModul.DB_SourceLinkTable.MoveNext();
                                            }

                                        }
                                        if ("" != cEv.sBem[3])
                                        {
                                            if (Strings.Trim(cEv.sBem[3]) != "")
                                            {
                                                Modul1.UbgT = Strings.RTrim(cEv.sBem[3]);
                                                Modul1.UbgT = Zeiweg(Modul1.UbgT, !Option[EOutCfg.o07_KeepFormat]);
                                                QuText = QuText == "" ? ". " + Modul1.IText[EUserText.t450] + " " + Modul1.UbgT.TrimEnd() : QuText.Trim() + "; " + Modul1.UbgT.TrimEnd();
                                            }
                                            richTextBox.AppendText(QuText);
                                            QuText = "";
                                            _ = richTextBox.AppendTextIfNd(".");
                                        }
                                    }
                                    goto IL_19be;
                                }
                                else
                                {
                                    goto IL_19be;
                                }
                            }
                            else
                            {
                                goto IL_1d11;
                            }
                        IL_19be: // <========== 4
                            num = 259;
                            if (Option[EOutCfg.o32])
                            {
                                Zeugsu(eArt);
                                Modul1.Kont1[20] = DataModul.Event.GetValue(famInArb, eArt, EventFields.Bem4, (f) => f.AsString());
                                if (Modul1.Kont1[20].Trim() != "")
                                {
                                    _ = richTextBox.AppendTextIfNd(".");
                                    if (unchecked(eArt == EEventArt.eA_Marriage || eArt == EEventArt.eA_MarrReligious))
                                    {
                                        richTextBox.AppendText(" Trauzeugen: " + Modul1.Kont1[20].Trim());
                                    }
                                    else
                                    {
                                        richTextBox.AppendText(" Zeugen: " + Modul1.Kont1[20].Trim());
                                    }
                                    _ = richTextBox.AppendTextIfNd(".");
                                }
                            }
                            goto IL_1d11;
                        IL_1d11: // <========== 4
                            num = 288;
                            lErl = 22;
                            num6++;
                            if (num6 <= 7)
                            {
                                goto IL_0159;
                            }
                            else
                            {
                                if (num4 == 1)
                                {
                                    richTextBox.AppendText(" mit\n");
                                }
                                richTextBox.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                                goto end_IL_0001_2;
                            }
                        IL_1e26:
                            num5 = unchecked(num2 + 1);
                            goto IL_1e2a;
                        IL_1e2a:
                            num2 = 0;
                            switch (num5)
                            {
                                case 1:
                                    break;
                                case 13:
                                    goto IL_0159;
                                case 255:
                                case 256:
                                case 257:
                                case 258:
                                case 259:
                                    goto IL_19be;
                                case 44:
                                case 53:
                                case 285:
                                case 286:
                                case 287:
                                case 288:
                                    goto IL_1d11;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 8948;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public IDocument GetFraPreview11()
    {
        return this.Document;
    }

    public void Weitehen(int FamSP1, IDocument document)
    {
        var aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz);
        List<(DateTime, int, EEventArt)> ltFams = new();

        if (aiFams.Count == 1)
        {
            return;
        }

        EEventArt eArt;
        foreach (var iFam in aiFams)
        {
            if (iFam != FamSP1)
            {
                eArt = EEventArt.eA_500;
                while (eArt <= EEventArt.eA_505)
                {
                    if ((Datu = DataModul.Event.GetDate(eArt, iFam)) != default
                        && eArt != EEventArt.eA_504)
                    {
                        break;
                    }
                    eArt++;
                }
                ltFams.Add((Datu, iFam, eArt));
            }
        }
        Modul1.eLKennz = Modul1.eLKennz == ELinkKennz.lkFather ? ELinkKennz.lkMother : ELinkKennz.lkFather;

        if (aiFams.Count < 3)
        {
            document.AppendText("\nWeitere Verbindung: ");
        }
        else
        {
            document.AppendText("\nWeitere Verbindungen: ");
        }

        Datu = default;
        IPersonData person = IoC.GetRequiredService<IPersonData>();
        foreach (var diF in ltFams)
        {
            (DateTime left, int iFam, eArt) = diF;
            if (iFam != 0)
            {
                string Ds = default;
                if (left != default)
                {
                    Datu = DataModul.Event.GetDate(eArt, iFam, out Ds);
                }
                var sDatu = Datwand1(Datu, Ds);

                if (Option[EOutCfg.o10_EmitIDs])
                {
                    document.AppendText("[" + iFam.AsString() + "] " + Datu + " mit ");
                }
                else
                {
                    document.AppendText(Datu + " mit ");
                }
                if (DataModul.Link.GetFamPerson(iFam, Modul1.eLKennz, out var iOtherPerson))
                {
                    Modul1.Person_ReadNames(iOtherPerson, person);
                    person.SetFullSurname(Namerw(person.SurName, person.Prefix, person.Suffix));
                    if (Option[EOutCfg.o10_EmitIDs])
                    {
                        document.AppendText($"<{iOtherPerson}> {(person.Prae.Trim() + " " + person.Givennames.Trim()).Trim()}");
                    }
                    else
                    {
                        document.AppendText((person.Prae.Trim() + " " + person.Givennames.Trim()).Trim());
                    }
                    document.SetFont(new Font("Arial", 11.01f, FontStyle.Bold));
                    document.AppendText(" " + Modul1.Person.FullSurName);
                    document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                }
                else
                {
                    document.AppendText(" UNBEKANNT");
                }
                if (iFam != ltFams.Last().Item2)
                {
                    document.AppendText("; ");
                }
                else
                {
                    document.AppendText(".");
                }
            }
            else break;
        }
        document.AppendText("\n");

        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 3
        return;
    }
    public void Quellen()
    {
        //Discarded unreachable code: IL_06a2
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string QuText = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        if (!Option[EOutCfg.o39])
                        {
                            goto end_IL_0001;
                        }
                        goto IL_0029;
                    case 2066:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_06f2;
                                default:
                                    goto end_IL_0001_2;
                            }
                            if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_06f5;
                        }
                    end_IL_0001_2:
                        break;
                    IL_0029:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        DataModul.DB_SourceLinkTable.Index = "Tab";
                        DataModul.DB_SourceLinkTable.Seek("=", 1, Modul1.PersInArb);
                        QuText = "";
                        while (!DataModul.DB_SourceLinkTable.EOF
                            && !DataModul.DB_SourceLinkTable.NoMatch
                            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._1].AsInt() == 1
                            && DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._2].AsInt() == Modul1.PersInArb)
                        {
                            DataModul.DB_QuTable.Index = "NR";
                            DataModul.DB_QuTable.Seek("=", DataModul.DB_SourceLinkTable.Fields[SourceLinkFields._3]);
                            if (!DataModul.DB_QuTable.NoMatch)
                            {
                                QuText = QuText.Trim().Length > 0
                                    ? (QuText + "; " + DataModul.DB_QuTable.Fields[QuFields._2].Value).AsString()
                                    : (QuText + DataModul.DB_QuTable.Fields[QuFields._2].Value).AsString();
                                if (null != DataModul.DB_SourceLinkTable.Fields[3].Value & DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
                                {
                                    QuText += $", {(null == DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value
                                        ? $"{Modul1.IText[EUserText.t449]} {DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim()}"
                                        : DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Aus].AsString().Trim() + " " + DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim())}";
                                }
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString() != "")
                                {
                                    QuText += $" >{DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Orig].AsString()}<";
                                }
                                if (DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString() != "")
                                {
                                    QuText += $" =={DataModul.DB_SourceLinkTable.Fields[SourceLinkFields.Kom].AsString()}==";
                                }
                                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                            }
                            DataModul.DB_SourceLinkTable.MoveNext();
                        }
                        goto IL_0524;
                    IL_0524: // <========== 3
                        num = 46;
                        var pt = DataModul.Person.Seek(Modul1.PersInArb);
                        if (null != DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value)
                        {
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Length > 0)
                            {
                                QuText = QuText.Trim() != ""
                                    ? (QuText + ". " + DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value).AsString()
                                    : DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString();
                            }
                        }
                        if (QuText.Trim() == "")
                        {
                            goto end_IL_0001;
                        }
                        QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                        Document.AppendText(Modul1.IText[EUserText.t450] + " " + QuText.Trim() + ".");
                        QuText = "";
                        Document.AppendText("\n");
                        goto end_IL_0001;
                    IL_06f2:
                        num4 = num2 + 1;
                        goto IL_06f5;
                    IL_06f5:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 42:
                            case 43:
                            case 44:
                            case 8:
                            case 9:
                            case 45:
                            case 12:
                            case 46:
                                goto IL_0524;
                            case 2:
                            case 62:
                            case 63:
                            case 68:
                                goto end_IL_0001;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 2066;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void Zeig(IRecordset dSB_SearchTable)
    {
        var M1_Iter = 0;
        while (M1_Iter < Modul1.Aus[(int)EOutCfg.o12].AsInt()
            && !dSB_SearchTable.NoMatch && !dSB_SearchTable.EOF)
        {
            var lListItem = SearchTab_GetListItem(dSB_SearchTable, Male2_Checked, Female2_Checked);
            if (lListItem != null)
                List1_Items.Add(lListItem);
            dSB_SearchTable.MoveNext();
            M1_Iter++;
        }
    }


    private ListItem<(int, int, int)>? SearchTab_GetListItem(IRecordset dSB_SearchTable, bool xIgnWoman, bool xIgnMan)
    {

        var persInArb = dSB_SearchTable.Fields["Nummer"].AsInt();
        var sPerSex = DataModul.Person.GetSex(persInArb);
        if (!(xIgnWoman && sPerSex == "F"
            || xIgnMan && sPerSex == "M"))
        {
            if (null == dSB_SearchTable.Fields["iKenn"].Value)
            {
                dSB_SearchTable.Edit();
                dSB_SearchTable.Fields["iKenn"].Value = " ";
                dSB_SearchTable.Update();
            }
            if (dSB_SearchTable.Fields["iKenn"].AsString() == "9")
            {
                dSB_SearchTable.Edit();
                dSB_SearchTable.Fields["iKenn"].Value = " ";
                dSB_SearchTable.Update();
            }
            var text3 = " " + dSB_SearchTable.Fields["iKenn"].AsString();
            var text2 = (text3 + "      " + dSB_SearchTable.Fields["Datum"].AsString()).Right(4) + " ";
            if (Strings.Mid(text2, 4).AsDouble() == 0.0)
            {
                text2 = "       ";
            }
            var text = Strings.Left(dSB_SearchTable.Fields["Name"].AsString() + new string(' ', 40), 40);
            var num4 = Strings.InStr(text, ",");
            if (num4 > 25f)
            {
                text = text.Left(25) + Strings.Mid(text, num4, text.Length);
            }

            return new($"{text,40} {text2}{dSB_SearchTable.Fields["Nummer"].AsString(),-10}", (persInArb, 0, 0));
        }
        else
            return null;
    }

    public void Zeigfam()
    {

        short num5 = 0;
        var dB_PersonTable = DataModul.DB_PersonTable;
        dB_PersonTable.Index = nameof(PersonIndex.PerNr);
        string text = (" " + DataModul.DSB_SearchTable.Fields["iKenn"].Value).AsString();
        string text2 = text + "      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString().Right(6) + " ";
        if (text2.Right(4).AsDouble() == 0.0)
        {
            text2 = "       ";
        }
        Modul1.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);

        Modul1.Person.SetFullSurname((Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName + " " + Modul1.Person.Suffix + Modul1.Person.Clan).Trim());

        string text3 = Modul1.Person.FullSurName.Left(25).TrimEnd() + "," + Modul1.Person.Givennames;

        dB_PersonTable.Index = nameof(PersonIndex.PerNr);
        dB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
        string Persex = dB_PersonTable.Fields[PersonFields.Sex].AsString();
        var aiFam = Modul1.Ehesuch(Modul1.PersInArb, Persex);
        ELinkKennz eLKennz = Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
        foreach (var Fam in aiFam)
        {
            Modul1.Famzeig(Fam, eLKennz);
        }
        num5 = 0;
        while (!DataModul.DSB_SearchTable.EOF
            && num5 < Modul1.Aus[(int)EOutCfg.o13].AsInt())
        {
            text = (" " + DataModul.DSB_SearchTable.Fields["iKenn"].Value).AsString();
            text2 = text + "      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString().Right(4);
            if (null != DataModul.DSB_SearchTable.Fields["Sich"].Value)
            {
                text2 = (text2 + DataModul.DSB_SearchTable.Fields["Sich"].Value).AsString();
            }
            text2 = text2 + "   ".Left(7);
            if (Conversion.Val(Strings.Mid(text2.Trim(), 2, 4)) == 0.0)
            {
                text2 = "       ";
            }
            Modul1.PersInArb = DataModul.DSB_SearchTable.Fields["Nummer"].AsInt();
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            var sClan = View.CheckBox19.Checked && Modul1.Person.Clan != "" ? "(" + Modul1.Person.Clan + ")" : "";
            Modul1.Person.SetFullSurname((Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName + " " + Modul1.Person.Suffix + sClan).Trim());
            text3 = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
            dB_PersonTable.Index = nameof(PersonIndex.PerNr);
            dB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
            Persex = dB_PersonTable.Fields[PersonFields.Sex].AsString();
            if ((!Male2_Checked || Persex != "F") && (!Female2_Checked || Persex != "M"))
            {
                IList<int> aiFam2 = null;
                if ((Male_Checked && Persex == "M")
                    || (Females_Checked && Persex == "F")
                    || !Male_Checked & !Females_Checked)
                {
                    aiFam2 = Modul1.Ehesuch(Modul1.PersInArb, Persex);
                }
                eLKennz = Persex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;
                int num13;
                if (aiFam2 != null)
                {
                    foreach (var Fam in aiFam2)
                    {
                        var LiText = Modul1.Famzeig(Fam, eLKennz);
                        num13 = Strings.InStr(text3, ",");
                        if (num13 > 25f)
                        {
                            text3 = text3.Left(25) + Strings.Mid(text3, num13, text3.Length);
                        }
                        List1_Items.Add(new(Strings.Left(text3 + new string(' ', 40).Left(40) + text2 + "              " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10) + LiText, 134) + new string(' ', 10) + Fam.AsString().Right(10), (0, 0, Fam)));
                        if (List1_Items.Count >= Modul1.Aus[(int)EOutCfg.o13].AsInt())
                        {
                            goto end_IL_0001_3;
                        }
                    }
                }
                else
                {
                    num13 = Strings.InStr(text3, ",");
                    if (num13 > 25f)
                    {
                        text3 = text3.Left(25) + Strings.Mid(text3, num13, text3.Length);
                    }
                    if (!FamOnly_Checked)
                    {
                        List1_Items.Add(new(text3 + new string(' ', 40).Left(40) + text2 + "           " + DataModul.DSB_SearchTable.Fields["Nummer"].AsString().Right(10)
                            , (DataModul.DSB_SearchTable.Fields["Nummer"].AsInt(), 0, 0)));
                    }
                }
            }
            DataModul.DSB_SearchTable.MoveNext();
        }
        goto end_IL_0001_3;

    end_IL_0001_3: // <========== 5
        return;
    }
    public void Zeigfamdat()
    {
        int num5 = 0;
        int num6 = checked((int)Math.Round((Text1_Text + "0000").Left(8).AsDouble()));
        //iEventType = 0;
        foreach (var cEv in DataModul.Event.ReadAllGt(EventIndex.DatInd, num6))
            if (num5++ > Modul1.Aus[(int)EOutCfg.o13].AsInt()) break;
            else
            {

                var eArt = cEv.eArt;
                if (eArt > EEventArt.eA_499 && (eArt <= EEventArt.eA_601))
                {
                    Kennzt = eArt switch
                    {
                        EEventArt.eA_500 => " P",
                        EEventArt.eA_501 => " V",
                        EEventArt.eA_Marriage => " H",
                        EEventArt.eA_MarrReligious => " K",
                        EEventArt.eA_504 => " S",
                        EEventArt.eA_505 => " E",
                        EEventArt.eA_507 => " D",
                        EEventArt.eA_601 => " F",
                        _ => " ",
                    };
                    DataModul.Link.ReadFamily(cEv.iPerFamNr, Modul1.Family);
                    var num8 = 0f;
                    int persInArb;
                    if (Text2_Text != "")
                    {
                        persInArb = Modul1.Family.Frau;
                        Modul1.Person_ReadNames(persInArb, Modul1.Person);
                        if (Modul1.Person.SurName.ToUpper().Trim().Left(Text2_Text.Length) == Text2_Text.ToUpper())
                        {
                            num8 = 1f;
                        }
                        persInArb = Modul1.Family.Mann;
                        Modul1.Person_ReadNames(persInArb, Modul1.Person);
                        if (Modul1.Person.SurName.ToUpper().Trim().Left(Text2_Text.Length) == Text2_Text.ToUpper())
                        {
                            num8 = 1f;
                        }
                    }
                    else
                    {
                        num8 = 1f;
                    }
                    if (num8 == 1f)
                    {
                        var LiText = "";
                        if (Modul1.Family.Mann == 0)
                        {
                            LiText = " " + new string(' ', 40).Left(40) + "       " + "              ".Right(10) + " ";
                        }
                        else
                        {
                            persInArb = Modul1.Family.Mann;
                            Modul1.Person_ReadNames(persInArb, Modul1.Person);
                            if (Modul1.Person.Prefix.Trim() != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                            }
                            var text = Modul1.Person.FullSurName.Left(25).TrimEnd() + "," + Modul1.Person.Givennames;
                            DataModul.DSB_SearchTable.Index = "Nummer";
                            DataModul.DSB_SearchTable.Seek("=", persInArb);
                            var text2 = (" " + DataModul.DSB_SearchTable.Fields["iKenn"].Value).AsString();
                            var text3 = text2 + "      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString().Right(4);
                            if (null != DataModul.DSB_SearchTable.Fields["Sich"].Value)
                            {
                                text3 = (text3 + DataModul.DSB_SearchTable.Fields["Sich"].Value).AsString();
                            }
                            text3 = text3 + "   ".Left(7);
                            if (text3.Right(4).AsDouble() == 0.0)
                            {
                                text3 = "       ";
                            }
                            LiText = text + new string(' ', 40).Left(40) + text3 + "              " + persInArb.AsString().Right(10) + " ";
                        }
                        var famInArb = cEv.iPerFamNr;
                        Datu = cEv.dDatumV;
                        var text4 = $"{Datu.Day:00}.{Datu.Month:00}.{Datu.Year:0000}";
                        LiText = LiText + text4 + Kennzt + " mit ";
                        persInArb = Modul1.Family.Frau;
                        Modul1.Person_ReadNames(persInArb, Modul1.Person);
                        if (Modul1.Person.Prefix.Trim() != "")
                        {
                            Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                        }
                        LiText += Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
                        LiText = (LiText + new string(' ', 134)).Left(134);
                        List1_Items.Add(new(LiText + famInArb.AsString(), (0, 0, famInArb)));

                    }
                }
            }
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 5
        return;
    }



    public void Zeugenaus(ref string InText, ref short Art, List<(DateTime, int)> liList5)
    {

        short num6 = (short)(liList5.Count - 1);
        short num5 = 0;
        while (num5 <= num6)
        {
            Modul1.FamInArb = liList5[num5].Item2;
            Datu = DataModul.Event.GetDate((EEventArt)Art, Modul1.FamInArb);
            var sDatu = Datwand1(Datu, "");
            var LiText = " " + sDatu + " bei ";
            Datu = default;
            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
            Modul1.PersInArb = Modul1.Family.Mann;
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            var text = (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.SurName.Trim().ToUpper();
            LiText += text;
            Modul1.PersInArb = Modul1.Family.Frau;
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            text = (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.SurName.Trim().ToUpper();
            Document.AppendText("\n" + InText + LiText + " und " + text);
            num5 = (short)unchecked(num5 + 1);
        }
    }
    public void Zeugsu(EEventArt eEventArt)
    {
        //Discarded unreachable code: IL_0778
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
                    int num4;
                    string text3;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            text3 = "";
                            goto IL_000b;
                        case 2348:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_07ca;
                                    default:
                                        goto end_IL_0001;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_07ce;
                            }
                        end_IL_0001:
                            break;
                        IL_000b:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            string text4;
                            Modul1.Nr = eEventArt < EEventArt.eA_499 ? Modul1.PersInArb : Modul1.FamInArb;
                            DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.FamSu);
                            DataModul.DB_WitnessTable.Seek("=", Modul1.Nr, "10");

                            Modul1.eWKennz = 10;
                            DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ZeugSu);
                            DataModul.DB_WitnessTable.Seek("=", Modul1.Nr, Modul1.eWKennz, eEventArt, LfNR);
                            short num6 = 1;
                            while (!DataModul.DB_WitnessTable.EOF
                                && num6 <= 99
                                && !DataModul.DB_WitnessTable.NoMatch
                                && DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt() == Modul1.Nr
                                && DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsInt() == 10
                                && DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>() == eEventArt
                                && DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt() == LfNR)
                            {
                                text4 = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsInt().AsString() + Strings.Right("          " +
                                   DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt().AsString(), 10);
                                text3 += text4;
                                DataModul.DB_WitnessTable.MoveNext();
                                num6 = (short)unchecked(num6 + 1);
                            }
                            Modul1.Kont1[20] = "";
                            string str = "";
                            short num9 = (short)Math.Round(text3.Length / 14.0);
                            short num5 = 1;
                            while (num5 <= num9)
                            {
                                text4 = text3.Left(14);
                                text3 = Strings.Mid(text3, 15, text3.Length);
                                Modul1.PersInArb = Strings.Mid(text4, 5, 10).AsInt();
                                num6 = 1;
                                while (num6 <= 99)
                                {
                                    Modul1.Kont[num6] = "";
                                    num6 = (short)unchecked(num6 + 1);
                                }
                                if (!Option[EOutCfg.o33])
                                {
                                    Datles2(Modul1.PersInArb);
                                }
                                string text = "";
                                string text2 = "";

                                if (Modul1.Person.Birthday.Trim() != "")
                                {
                                    text = Modul1.DTxt[1] + " " + Modul1.Person.Birthday;
                                }
                                if (text.Trim() == "" & Modul1.Person.Baptised.Trim() != "")
                                {
                                    text = Modul1.DTxt[2] + " " + Modul1.Person.Baptised;
                                }
                                text = text.Trim();

                                if (Modul1.Person.Death.Trim() != "")
                                {
                                    text2 = Modul1.DTxt[3] + " " + Modul1.Person.Death;
                                }
                                if (text2.Trim() == "" & Modul1.Person.Burial.Trim() != "")
                                {
                                    text2 = Modul1.DTxt[4] + " " + Modul1.Person.Burial;
                                }
                                text2 = text2.Trim();

                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                                str = str.Trim() != ""
                                    ? str.Trim() + "; " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.FullSurName.Trim().ToUpper() + " " + text + " " + text2
                                    : " " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.FullSurName.Trim().ToUpper() + " " + text + " " + text2;
                                num5 = (short)unchecked(num5 + 1);
                            }
                            Modul1.UbgT1 = str.TrimEnd();
                            goto end_IL_0001_2;
                        IL_07ca:
                            num4 = unchecked(num2 + 1);
                            goto IL_07ce;
                        IL_07ce:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 2348;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Adoelt(int iFamily, IDocument doc)
    {
        ID = 40;
        checked
        {
            if (iFamily > 0)
            {
                // References
                Font fntArial11Bold = new Font("Arial", 11.01f, FontStyle.Bold);
                Font fntArial11Reg = new Font("Arial", 11.01f, FontStyle.Regular);

                bool xNumIs6 = Option[EOutCfg.o10_EmitIDs];
                doc.SetIndent(ID);
                doc.AppendText("\n");
                doc.SetFont(fntArial11Bold);
                doc.AppendText("Adoptiveltern: \n");
                doc.SetFont(fntArial11Reg);

                if (DataModul.Link.GetFamPerson(iFamily, ELinkKennz.lkFather, out int iFather))
                {
                    Modul1.Family.Mann = iFather;
                    Modul1.Person_ReadNames(iFather, Modul1.Person);
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    if (xNumIs6)
                    {
                        doc.AppendText($"<{iFather}> {(Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim()}");
                    }
                    else
                    {
                        doc.AppendText((Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                    }
                    doc.SetFont(fntArial11Bold);
                    doc.AppendText(" " + Modul1.Person.FullSurName);
                    doc.SetFont(fntArial11Reg);
                    if (Modul1.Person.Alias != "")
                    {
                        doc.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                        doc.AppendText(" (" + Modul1.Person.Alias.TrimEnd() + ")");
                    }
                    else
                    {
                        doc.AppendText("");
                    }
                    doc.SetFont(fntArial11Reg);
                    var pt = DataModul.Person.Seek(Modul1.PersInArb);
                    if (pt.Fields[PersonFields.religi].AsInt() > 0)
                    {
                        var sRel = DataModul.TextLese1(pt.Fields[PersonFields.religi].AsInt());
                        doc.AppendText(", " + sRel);
                    }
                    Bildaus("P");
                    AppendRelatives(Modul1.PersInArb, doc);
                }
                else
                {
                    doc.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                    Modul1.PersInArb = 0;
                }
                doc.SetFont(fntArial11Reg);
                BemSch = 0;
                float Idned = 0f;
                Datr1(ref Idned, Modul1.PersInArb);

                EEventArt eArt;
                if (!DataModul.Event.Exists((eArt = EEventArt.eA_Marriage, iFamily, 0)))
                {
                    eArt = EEventArt.eA_MarrReligious;
                }
                if (!DataModul.Event.ReadData(eArt, iFamily, out var cEv))
                {
                    short num = 1;
                    short num2;
                    short num3;
                    do
                    {
                        Modul1.Kont1[num] = "";
                        num = (short)unchecked(num + 1);
                        num2 = num;
                        num3 = 10;
                    }
                    while (num2 <= num3);
                    if (cEv.dDatumV != default)
                    {
                        Datu = cEv.dDatumV;
                        var sDatu = Datwand1(Datu, cEv.sDatumV_S);
                        Modul1.Kont1[1] = Datu.AsString();
                    }
                    if (cEv.dDatumB != default)
                    {
                        Datu = cEv.dDatumB;
                        var sDatu = Datwand1(Datu, cEv.sDatumB_S);
                        if (Modul1.Kont1[1] != "")
                        {
                            sDatu = " / " + sDatu;
                        }
                        Modul1.Kont1[3] = sDatu;
                    }
                    Modul1.UbgT = "";
                    if (cEv.iOrt > 0)
                    {
                        Modul1.UbgT = Place_ReadData(cEv.iOrt, 1, 0, Option[EOutCfg.o35]);
                        if (cEv.sOrt_S.Trim() != "")
                        {
                            Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + cEv.sOrt_S.Trim();
                        }
                    }
                    num = 1;
                    short num4;
                    do
                    {
                        if (Modul1.Kont1[num] == "0")
                        {
                            Modul1.Kont1[num] = "";
                        }
                        num = (short)unchecked(num + 1);
                        num4 = num;
                        num3 = 6;
                    }
                    while (num4 <= num3);
                    doc.SetIndent(ID + 10);
                    if (Option[EOutCfg.o14])
                    {
                        doc.SetIndent(ID + 50);
                    }
                    string text = Modul1.DTxt[7];
                    if (cEv.eArt == EEventArt.eA_MarrReligious)
                    {
                        text = Modul1.DTxt[8];
                    }
                    if (Option[EOutCfg.o10_EmitIDs])
                    {
                        doc.AppendText("\n" + text + " [" + iFamily.AsString() + "] " + Modul1.Kont1[1] + " " + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + Modul1.UbgT + " mit\n");
                        Modul1.UbgT = "";
                    }
                    else
                    {
                        doc.AppendText("\n" + text + " " + Modul1.Kont1[1] + " " + Modul1.Kont1[2] + Modul1.Kont1[3] + Modul1.Kont1[5] + Modul1.Kont1[6] + " " + Modul1.UbgT + " mit\n");
                        Modul1.UbgT = "";
                    }
                }
                _ = doc.AppendTextIfNd();

                if (DataModul.Link.GetFamPerson(iFamily, ELinkKennz.lkMother, out int iMother))
                {
                    Modul1.PersInArb = iMother;
                    Modul1.Family.Frau = iMother;
                    Modul1.Person_ReadNames(iMother, Modul1.Person);
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    if (Option[EOutCfg.o10_EmitIDs])
                    {
                        doc.AppendText("<" + Modul1.PersInArb.AsString() + ">% " + (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                    }
                    else
                    {
                        doc.AppendText((Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim());
                    }
                    doc.SetFont(fntArial11Bold);
                    doc.AppendText(" " + Modul1.Person.FullSurName);
                    doc.SetFont(fntArial11Reg);
                    doc.SetFont(new Font("Arial", 11.01f, FontStyle.Italic));
                    if (Modul1.Person.Alias != "")
                    {
                        doc.AppendText(" (" + Modul1.Person.Alias.TrimEnd() + ")");
                    }
                    else
                    {
                        doc.AppendText("");
                    }
                    doc.SetFont(fntArial11Reg);
                    var pt = DataModul.Person.Seek(Modul1.PersInArb);
                    if (pt.Fields[PersonFields.religi].AsInt() > 0)
                    {
                        var sRel = DataModul.TextLese1(pt.Fields[PersonFields.religi].AsInt());
                        doc.AppendText(", " + sRel);
                    }
                    Bildaus("P");
                    AppendRelatives(Modul1.PersInArb, doc);
                    Idned = 0f;
                    Datr1(ref Idned, Modul1.PersInArb);
                    _ = doc.AppendTextIfNd();
                }
                else
                {
                    doc.AppendText(Modul1.IText[EUserText.tSpouseUnknown] + "\n");
                }
            }
            else
            {
                Modul1.FamInArb = 0;
            }
        }
    }

    public int Adoeltsuch(int persInArb)
    {
        var aiFams = DataModul.Link.GetPersonFams(persInArb, ELinkKennz.lkAdoptedChild);
        if (aiFams.Count == 1) return aiFams[0];
        if (aiFams.Count > 1)
        {
            string text2 = "Person " + persInArb.AsString() + " ist in den Familien " + string.Join(", ", aiFams) + " als Kind eingebunden. Eine Person kann aber nur in einer Familie als Kind sein.";
            text2 += "\nBitte diesen Fehler zuerst korrigieren.";
            _ = Interaction.MsgBox(text2, "Schwerer Datenfehler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        return 0;
    }

    public static string Namerw(string value, string val2, string Kont2)
    {
        value = value.ToUpper();
        if (val2 != "")
        {
            value = val2 + " " + value;
        }

        if (Kont2 != "")
        {
            value = value + " " + Kont2;
        }
        return value.Trim();
    }

    private void Text1_TextChanged(object eventSender, EventArgs eventArgs)
    {
        Listleer();
    }

    public void Zeuge_Bei(int persInArb)
    {
        string text = default;
        string text2 = default;
        string text3 = default;
        string text5 = default;
        string text6 = default;
        string str = default;
        string str2 = default;
        List3_Items.Clear();
        int Witness_iPerNr;
        string Witness_sKennz;
        DataModul.DB_WitnessTable.Index = nameof(WitnessIndex.ElSu);
        DataModul.DB_WitnessTable.Seek("=", persInArb, "10");
        short num6 = 1;
        while (num6 <= 99
            && !DataModul.DB_WitnessTable.EOF
        && !DataModul.DB_WitnessTable.NoMatch
        && (Witness_iPerNr = DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt()) == persInArb
           && (Witness_sKennz = DataModul.DB_WitnessTable.Fields[WitnessFields.Kennz].AsString()) == "10")
        {
            EEventArt Witness_eArt = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsEnum<EEventArt>();
            int Witness_iLfNr = DataModul.DB_WitnessTable.Fields[WitnessFields.LfNr].AsInt();
            int Witness_iFamNr = DataModul.DB_WitnessTable.Fields[WitnessFields.FamNr].AsInt();

            if (DataModul.DB_WitnessTable.NoMatch)
            {
                _ = Interaction.MsgBox("F26");
                Debugger.Break();
            }
            else
            {
                var sLiTxt = Witness_eArt.AsString() + Conversion.Str(
                    Witness_iLfNr) + Strings.Right("          " + Conversion.Str(
                        Witness_iFamNr), 10);
                var Datu = DataModul.Event.GetDate(Witness_eArt, Witness_iFamNr);
                text5 = Datu == default ? "        " : Datu.ToString("yyyyMMdd");

                sLiTxt = text5 + sLiTxt;
                List3_Items.Add(new(sLiTxt, (Witness_eArt, Witness_iFamNr, (short)Witness_iLfNr)));
            }
            DataModul.DB_WitnessTable.MoveNext();
            num6++;
        }
        foreach (var Item in List3_Items)
        {
            var (Datu, Witness_iFamNr, Witness_eArt, Witness_iLfNr) = Item.ItemData<(DateTime, int, EEventArt, int)>();
            //Modul1_Per1 = Strings.Mid(Modul1_Per1, 17, Modul1_Per1.Length);
            if (Witness_eArt > EEventArt.eA_499)
            {
                Modul1.FamInArb = Witness_iFamNr;
                DataModul.Link.ReadFamily(Witness_iFamNr, Modul1.Family);
                str = "";
                if (Modul1.Family.Mann > 0)
                {
                    persInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(persInArb, Modul1.Person);
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    str = (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.FullSurName;
                }
                str2 = "";
                if (Modul1.Family.Frau > 0)
                {
                    persInArb = Modul1.Family.Frau;
                    Modul1.Person_ReadNames(persInArb, Modul1.Person);
                    Modul1.Person.SetFullSurname(Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix));
                    str2 = (Modul1.Person.Prae.Trim() + " " + Modul1.Person.Givennames.Trim()).Trim() + " " + Modul1.Person.FullSurName;
                }
                if (str.Trim() != "" & str2.Trim() != "")
                {
                    text2 = str.Trim() + " und " + str2.Trim();
                }
                else
                {
                    text2 = (str.Trim() + " " + str2.Trim()).Trim();
                }
            }
            else
            {
                persInArb = Witness_iFamNr;
                Modul1.Person_ReadNames(persInArb, Modul1.Person);
                var SurName = Namerw(Modul1.Person.SurName, Modul1.Person.Prefix, Modul1.Person.Suffix);
                text2 = (Modul1.Person.Givennames + " " + SurName).Trim();
            }

            text3 = "";
            int iLink = Witness_eArt <= EEventArt.eA_499 ? persInArb : Modul1.FamInArb;
            if ((Datu = DataModul.Event.GetDate(Witness_eArt, iLink, out var sD_S)) != default)
            {
                var sDatu = Datwand1(Datu, sD_S);
                text3 = Datu + " ";
            }
            EEventArt left = Witness_eArt;
            var AAA = DataModul.Event.GetValue((Witness_eArt, iLink, (short)Witness_iLfNr), EventFields.ArtText, 0);
            (text6, text) = left switch
            {
                EEventArt.eA_Birth => (" (" + text3 + "bei der Geburt)", "Zeuge " + text3 + "bei der Geburt von "),
                EEventArt.eA_Baptism => (" (" + text3 + "bei der Taufe)", "Zeuge " + text3 + "bei der Taufe von "),
                EEventArt.eA_Death => (" (" + text3 + "beim Tod)", "Zeuge " + text3 + "beim Tod von "),
                EEventArt.eA_Burial => (" (" + text3 + "beim Begräbnis)", "Zeuge " + text3 + "beim Begräbnis von "),
                EEventArt.eA_105 when DataModul.TextLese1(AAA) is string s && s != "" => (" (" + text3 + "bei " + s.Trim() + ")", "Zeuge " + text3 + "bei " + s.Trim() + "von "),
                EEventArt.eA_105 => (" (" + text3 + "bei der Sonst.Datum)", "Zeuge " + text3 + "beim Sonst. Datum von "),
                EEventArt.eA_106 => (" (" + text3 + "beim Heimatort)", "Zeuge " + text3 + "beim Heimatort von "),
                EEventArt.eA_300 => (" (" + text3 + "beim Beruf)", "Zeuge " + text3 + "beim Beruf von "),
                EEventArt.eA_301 => (" (" + text3 + "beim Titel)", "Zeuge " + text3 + "beim Titel von "),
                EEventArt.eA_302 => (" (" + text3 + "beim Wohnort)", "Zeuge " + text3 + "beim Wohnort von "),
                EEventArt.eA_500 => (" (" + text3 + "bei der Proklamation)", "Zeuge " + text3 + "bei bei der Proklamation von "),
                EEventArt.eA_501 => (" (" + text3 + "bei der Verlobung)", "Zeuge " + text3 + "bei der Verlobung von "),
                EEventArt.eA_Marriage => (" (" + text3 + "bei der Heirat)", "Trauzeuge " + text3 + "bei der Heirat von "),
                EEventArt.eA_MarrReligious => (" (" + text3 + "bei der Kirchl. Heir.)", "Trauzeuge " + text3 + "bei der Kirchl. Heir. von "),
                EEventArt.eA_504 => (" (" + text3 + "bei der Scheidung)", "Zeuge " + text3 + "bei der Scheidung von "),
                EEventArt.eA_505 => (" (" + text3 + "bei der Eheänl. Beziehung)", "Zeuge " + text3 + "bei der Eheänl. Beziehung von "),
                EEventArt.eA_506 => (" (" + text3 + "Eheänl. Beziehung)", ""),
                EEventArt.eA_507 => (" (" + text3 + "Dimissoriale)", "Zeuge " + text3 + "bei der Dimissoriale von "),
                EEventArt.eA_602 => (" (" + text3 + "bei der Wohnung)", "Zeuge " + text3 + "beim Wohnungseintag von "),
                EEventArt.eA_603 when DataModul.TextLese1(AAA) is string s && s != "" => (" (" + text3 + "bei " + s.Trim() + ")", "Zeuge " + text3 + "bei " + s.Trim() + "von "),
                EEventArt.eA_603 => (" (" + text3 + "bei der Sonst.Datum)", "Zeuge " + text3 + "beim Sonst. Datum von "),
                _ => ("", ""),
            };
            Document.AppendText("\n" + text + text2);
        }

    }
    public void Datles2(int persInArb)
    {
        var Modul1_sDat_Death = "";
        var Modul1_sDat_Birth = "";
        short num8 = 0;
        short num9 = 0;
        short num10 = 1;
        while (num10 <= 50)
        {
            Modul1.Kont[num10] = "";
            num10 = (short)unchecked(num10 + 1);
        }
        EEventArt eEventArt = EEventArt.eA_Birth;
        while (eEventArt <= EEventArt.eA_Burial)
        {
            string[] Kont1 = new string[21];
            Kont1.Initialize();

            //int ubg = Modul1.Ubg;
            Modul1.Art = eEventArt;
            if (DataModul.Event.ReadData(eEventArt, persInArb, out var cEvt, 0))
            {
                Modul1_priv = 0 != cEvt.iPrivacy
                    ? cEvt.iPrivacy
                    : 0;
                if (!(Modul1_priv.AsInt() > privaus))
                {
                    if (cEvt.dDatumV != default)
                    {
                        Datu = cEvt.dDatumV;
                        var sDatu2 = Datwand1(Datu, cEvt.sDatumV_S);
                        Kont1[1] = Datu.AsString();
                        if (eEventArt == EEventArt.eA_Birth)
                        {
                            Modul1_sDat_Birth = $"           {Datu}".Right(10);
                            num8 = 1;
                        }
                        if (unchecked(eEventArt == EEventArt.eA_Baptism && num8 == 0))
                        {
                            Modul1_sDat_Birth = $"           {Datu}".Right(10);
                        }
                        if (eEventArt == EEventArt.eA_Death)
                        {
                            Modul1_sDat_Death = $"           {Datu}".Right(10);
                            num9 = 1;
                        }
                        if (unchecked(eEventArt == EEventArt.eA_Burial && num9 == 0))
                        {
                            Modul1_sDat_Death = $"           {Datu}".Right(10);
                        }
                    }
                    if (cEvt.dDatumB != default)
                    {
                        Datu = cEvt.dDatumB;
                        var sDatu = Datwand1(Datu, cEvt.sDatumB_S);
                        Kont1[3] = cEvt.sDatumB_S.Trim() == ""
                            ? " / " + Datu
                            : " " + Datu;
                    }
                    Modul1.UbgT = "";
                    if (cEvt.iOrt > 0)
                    {
                        Modul1.UbgT = Ortles2(cEvt.iOrt);
                        if (cEvt.sOrt_S.Trim() != "")
                        {
                            Modul1.UbgT = Modul1.UbgT.TrimEnd() + " " + cEvt.sOrt_S.Trim();
                        }
                    }
                    Modul1.Kont[(int)eEventArt - 90] = Kont1[1] + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[6] + " " + Modul1.UbgT;
                    Modul1.UbgT = "";
                }
            }
            eEventArt++;
        }
    }

    public string Ortles2(int Ortnr)
    {
        if (DataModul.Place.ReadData(Ortnr, out var place))
        {
            var bBB2 = place.sOrtsteil;
            if (bBB2 != "")
            {
                bBB2 = "-" + bBB2;
            }
            var bBB3 = place.sZusatz;
            if (bBB3.Trim() == "")
            {
                bBB3 = "in";
            }
            return bBB3 + " " + place.sOrt.TrimEnd() + bBB2.TrimEnd();
        }
        else
        {
            return "";
        }
    }

    public static string[] Datl(int persInArb, bool xShort)
    {
        var result = new string[4];
        var arDates = DataModul.Event.GetPersonDates(persInArb, out _, HndlPlace);
        for (var num = 1; num <= 4; num++)
        {
            if (arDates[num] > DateTime.MinValue)
            {
                result[(num - 1) & 2] = (num & 1) switch
                {
                    1 => $"{arDates[num].Year}",
                    0 when result[(num - 1) & 2] == "" => $"{arDates[num].Year}",
                    _ => result[(num - 1) & 2],
                };
            }
        }
        return result;

        void HndlPlace(EEventArt iEvent, int iPlace, string sPlace)
        {
            if (iPlace > 0)
            {
                string sPlace2 = "";
                if (iPlace > 0)
                {
                    sPlace2 = Place_ReadData(iPlace, 0, 1, xShort);
                    if (sPlace.Trim() != "")
                    {
                        sPlace2 = sPlace2.TrimEnd() + " " + sPlace.AsString().Trim();
                    }
                }
    ;
                result[((int)iEvent - 101) | 1] = ((int)iEvent & 1) switch
                {
                    1 => sPlace2,
                    0 when result[(int)iEvent - 101 - 1 | 1] == "" => sPlace2,
                    _ => result[(int)iEvent - 101 - 1 | 1]
                };
            }
        }
    }

    public void Retweg3(IDocument Display)
    {
        while (!Display.IsEmpty)
        {
            if (TrimEnd(Display)) continue;
            if (Display.TrimEnd("\n")) continue;
            if (Display.TrimEnd("\r")) continue;
            break;
        }
    }

    public bool TrimEnd(IDocument display)
    {
        return display.TrimEnd();
    }

    public void Listleer()
    {
        PersonSheet_Visible = false;
        FamilySheet_Visible = false;
        var M1_Iter = 0;
        checked
        {
            int i;
            int num;
            do
            {
                Label5_Text[(short)M1_Iter] = new("");
                M1_Iter++;
                i = M1_Iter;
                num = 15;
            }
            while (i <= num);
            List1_Items.Clear();
        }
    }

    private void ComboBox1_KeyUp(object sender, KeyEventArgs e)
    {
        Keys num = e.KeyCode;
        if (num == Keys.Enter)
        {
            StartSearch();
        }
        else if (Option[EOutCfg.o44_PictOrginalSize])
        {
            if (ComboBox2.Text == Modul1.IText[EUserText.t314] && num > Keys.Enter)
            {
                ListBox1.Items.Clear();
                ListBox1.Width = ComboBox1.Width;
                ListBox1.Visible = true;
                Modul1.STextles(nameof(Namensuch), ETextKennz.tkName, ComboBox1.Text, ListBox1.Items);
            }
        }
        else
        {
            ListBox1.Visible = false;
        }
    }


    private void chbOmitSpouse_MouseUp(object sender, MouseEventArgs e)
    {
        Listleer();
        if (ComboBox1.Text != "")
        {
            StartSearch();
        }
    }

    [RelayCommand]
    private void ReqHint()
    {
        List1.BringToFront();
        _ = Interaction.MsgBox(List1.Enabled.ToString());
        _ = List1.Focus();
        _ = Interaction.MsgBox(List1.Focused.ToString());
        _ = Interaction.MsgBox(List1.Visible.ToString());
        List1.Visible = false;
        List1.Visible = true;
    }

    [RelayCommand]
    private void RegisterSearch_Click()
    {
        Close();
    }

    public void Suchspeich()
    {
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Modul1.Verz + "SUCH.DAT", OpenMode.Output);
        var Modul1_MyList = new List<ListItem<int>>();
        ComboBox1_Visible = false;
        checked
        {
            int num = ComboBox1_Items.Count - 1;
            int num2 = 0;
            while (num2 <= num)
            {
                Modul1_MyList.Add(ComboBox1_Items[num2]);
                num2++;
            }
            ComboBox1_Items.Clear();
            var Modul1_oResult = Modul1.DeleteDoublicates<ListItem<int>>(Modul1_MyList);
            foreach (var item in Modul1_oResult)
            {
                ComboBox1_Items.Add(item);
            }
            int num5 = ComboBox1_Items.Count - 1;
            var M1_Iter = 0;
            while (true)
            {
                int i = M1_Iter;
                int num4 = num5;
                if (i > num4)
                {
                    break;
                }
                ComboBox1.SelectedIndex = M1_Iter;
                FileSystem.PrintLine(99, ComboBox1.SelectedItem);
                if (M1_Iter == 9)
                {
                    break;
                }
                M1_Iter++;
            }
            FileSystem.FileClose(99);
            ComboBox1.Visible = true;
        }
    }

    private void Zeigfamanl()
    {
        //Discarded unreachable code: IL_06a9
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string HT = default;
        int lErl = default;
        string DDatum = default;

        Listleer();
        //    goto IL_000b;
        //case 2492:
        //    {
        //        num2 = num;
        //        switch (num3 <= -2 ? 1 : num3)
        //        {
        //            case 2:
        //                break;
        //            case 1:
        //                goto IL_07fa;
        //            default:
        //                goto end_IL_0001;
        //        }
        //        int number = Information.Err().Number;
        //        if (number == 3021)
        //        {
        //            List1_Items.Add(new("------------ Ende der Liste-----------"));
        //            goto end_IL_0001_2;
        //        }
        //        else
        //        {
        //            if (number == 94)
        //            {
        //                DataModul.DSB_SearchTable.Edit();
        //                DataModul.DSB_SearchTable.Fields["iKenn"].Value = "9";
        //                DataModul.DSB_SearchTable.Update();
        //                ProjectData.ClearProjectError();
        //                if (num2 == 0)
        //                {
        //                    throw ProjectData.CreateProjectError(-2146828268);
        //                }
        //                goto IL_07f6;
        //            }
        //            else
        //            {
        //                if (number == 3421)
        //                {
        //                    View.Option1[1].Checked = true;
        //                    goto end_IL_0001_2;
        //                }
        //                else
        //                {
        //                    if (number == 3167)
        //                    {
        //                        goto end_IL_0001_2;
        //                    }
        //                    if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
        //                    {
        //                        ProjectData.EndApp();
        //                    }
        //                    ProjectData.ClearProjectError();
        //                    if (num2 == 0)
        //                    {
        //                        throw ProjectData.CreateProjectError(-2146828268);
        //                    }
        //                    goto IL_07f6;
        //                }
        //            }
        //        }
        //    }
        //end_IL_0001:
        //    break;
        //IL_000b:
        //ProjectData.ClearProjectError();
        num3 = 2;
        short num5 = 0;
        int num6 = $"{Text1_Text}00000000".Left(8).AsInt();
        int persInArb = 0;

        DataModul.DB_PersonTable.Index = nameof(PersonIndex.BeaDat);
        DataModul.DB_PersonTable.Seek(">=", num6);
        HT = "          ";
        while (!DataModul.DB_PersonTable.EOF
            && num5 < Modul1.Aus[(int)EOutCfg.o13].AsInt())
        {
            DateTime Person_EditDat = DataModul.DB_PersonTable.Fields[PersonFields.EditDat].AsDate();
            DDatum = Person_EditDat.AsString();
            HT = DDatum.Date2DotDateStr2();
            if (HT.Trim() == "")
            {
                HT = "          ";
            }

            persInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
            PersSp = persInArb;
            //                                    Perles1(persInArb);
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            if (Modul1.Person.Prefix.Trim() != "")
            {
                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
            }
            string text = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
            string text2 = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
            if ((!Male2_Checked
            || text2 != "F")
            && (!Female2_Checked
            || text2 != "M"))
            {
                IList<int> aiFam = null;
                if ((Male_Checked && text2 == "M") || (Females_Checked && text2 == "F") || !Male_Checked & !Females_Checked)
                {
                    aiFam = Modul1.Ehesuch(persInArb, text2);
                }

                ELinkKennz eLKennz = text2 == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;

                IListItem<int> LiText;
                int num11;
                if (aiFam == null)
                {
                    foreach (var Fam in aiFam)
                    {
                        LiText = Modul1.Famzeig(Fam, eLKennz);
                        num11 = Strings.InStr(text, ",");
                        if (num11 > 25f)
                        {
                            text = text.Left(25) + Strings.Mid(text, num11, text.Length);
                        }
                        List1_Items.Add(new($"{text,40} HT{PersSp,-10} {LiText,40} {Fam,-10}", (PersSp, LiText.ItemData, Fam)));
                        if (List1_Items.Count >= Modul1.Aus[(int)EOutCfg.o13].AsInt())
                        {
                            DataModul.DB_PersonTable.MoveLast();
                            break;
                        }

                    }
                }
                else
                {
                    num11 = Strings.InStr(text, ",");
                    if (num11 > 25f)
                    {
                        text = text.Left(25) + Strings.Mid(text, num11, text.Length);
                    }
                    if (!FamOnly_Checked)
                    {
                        List1_Items.Add(new($"{text,40} HT{PersSp,-10} {"",40}", (PersSp, 0, 0)));
                        //S += 1f;
                    }
                }
            }
            lErl = 2;
            DataModul.DB_PersonTable.MoveNext();

        }
        //                    goto end_IL_0001_2;
        //                IL_07f6:
        //                    num4 = num2;
        //                    goto IL_07fe;
        //                IL_07fa:
        //                    num4 = unchecked(num2 + 1);
        //                    goto IL_07fe;
        //                IL_07fe:
        //                    num2 = 0;
        //                    switch (num4)
        //                    {
        //                        case 1:
        //                            break;
        //                        case 62:
        //                        case 82:
        //                        case 84:
        //                        case 88:
        //                        case 94:
        //                        case 95:
        //                        case 98:
        //                        case 100:
        //                        case 106:
        //                        case 107:
        //                        case 108:
        //                            goto end_IL_0001_2;
        //                    }
        //                    goto default;
        //            }
        //        }
        //    }
        //    catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
        //    {
        //        ProjectData.SetProjectError(obj, lErl);
        //        try0001_dispatch = 2492;
        //        continue;
        //    }
        //    throw ProjectData.CreateProjectError(-2146828237);
        //end_IL_0001_2: // <========== 6
        //    break;
        //}
        //if (num2 != 0)
        //{
        //    ProjectData.ClearProjectError();
        //}
    }

    private void Zeigfamanl2()
    {
        //Discarded unreachable code: IL_0741
        int try0001_dispatch = -1;
        int num = default;
        string HT = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        int number = default;
        int lErl = default;
        float num5 = default;
        int num6 = default;
        string DDatum = default;
        float num8 = default;
        string text2 = default;
        /*    while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block* /
                    ;
                    int num4;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            HT = "";
                            goto IL_000b;
                        case 2587:
                            {
                                num2 = num;
                                switch (num3 <= -2 ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0865;
                                    default:
                                        goto end_IL_0001;
                                }
                                number = Information.Err().Number;
                                if (number == 3021)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0865;
                                }
                                else
                                {
                                    if (number == 94)
                                    {
                                        DataModul.DSB_SearchTable.Edit();
                                        DataModul.DSB_SearchTable.Fields["iKenn"].Value = "9";
                                        DataModul.DSB_SearchTable.Commit();
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        num4 = num2;
                                        goto IL_0869;
                                    }
                                    else
                                    {
                                        if (number == 3421)
                                        {
                                            Option1[1].Checked = true;
                                            goto end_IL_0001_2;
                                        }
                                        else
                                        {
                                            if (number != 3167)
                                            {
                                                goto end_IL_0001_2;
                                            }
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            num2 = 0;
                                        }
                                        goto IL_06e6;
                                    }
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_000b:
                            num = 2;*/
        Listleer();
        ProjectData.ClearProjectError();
        num3 = 2;
        num6 = Text1_Text.PadRight(8, '0').AsInt();
        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.BeaDat);
        DataModul.DB_FamilyTable.Seek(">=", num6);
        //iEventType = 0;
        string LiText = "          ";
        goto IL_0721;
    IL_06e6: // <========== 3
        num = 75;
        lErl = 2;
        if ((double)num5 >= Modul1.Aus[(int)EOutCfg.o13].AsInt())
        {
            goto end_IL_0001_2;
        }
        goto IL_0711;
    IL_0711: // <========== 3
        num = 81;
        DataModul.DB_FamilyTable.MoveNext();
        goto IL_0721;
    IL_0721: // <========== 3
        num = 10;
        if (DataModul.DB_FamilyTable.EOF)
        {
            goto end_IL_0001_2;
        }
        if (!DataModul.DB_FamilyTable.NoMatch)
        {
            Modul1.FamInArb = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
            DDatum = DataModul.DB_FamilyTable.Fields[FamilyFields.EditDat].AsString();
            HT = DDatum.Date2DotDateStr2();
            if (HT.Trim() == "")
            {
                HT = "          ";
            }
            FamzeigDat(Modul1.FamInArb, out LiText, default);
            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
            num8 = 0f;
            if (Text2_Text == "")
            {
                num8 = 1f;
            }
            Modul1.PersInArb = Modul1.Family.Frau;
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            if (Modul1.Person.SurName.ToUpper().Trim().Left(Text2_Text.Length) == Text2_Text.ToUpper())
            {
                num8 = 1f;
            }
            Modul1.PersInArb = Modul1.Family.Mann;
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            if (Modul1.Person.SurName.ToUpper().Trim().Left(Text2_Text.Length) == Text2_Text.ToUpper())
            {
                num8 = 1f;
            }

            if (num8 == 1f)
            {
                LiText = "";
                if (Modul1.Family.Mann == 0)
                {
                    LiText = " " + new string(' ', 40).Left(40) + "       " + "              ".Right(10) + " ";
                }
                else
                {
                    Modul1.PersInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    if (Modul1.Person.Prefix.Trim() != "")
                    {
                        Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                    }

                    text = Modul1.Person.FullSurName.Left(25).TrimEnd() + "," + Modul1.Person.Givennames;
                    DataModul.DSB_SearchTable.Index = "Nummer";
                    DataModul.DSB_SearchTable.Seek("=", Modul1.PersInArb);
                    text2 = (" " + DataModul.DSB_SearchTable.Fields["iKenn"].Value).AsString();
                    DDatum = text2 + "      " + DataModul.DSB_SearchTable.Fields["Datum"].AsString().Right(4);
                    if (null != DataModul.DSB_SearchTable.Fields["Sich"].Value)
                    {
                        DDatum = (DDatum + DataModul.DSB_SearchTable.Fields["Sich"].Value).AsString();
                    }
                    DDatum = DDatum + "   ".Left(7);
                    if (DDatum.Right(4).AsDouble() == 0.0)
                    {
                        DDatum = "       ";
                    }
                    LiText = text + new string(' ', 40).Left(37) + HT + "              " + Modul1.PersInArb.AsString().Right(10) + " " + LiText;
                }
                lErl = 4;
                LiText += " mit ";
                Modul1.PersInArb = Modul1.Family.Frau;
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                if (Modul1.Person.Prefix.Trim() != "")
                {
                    Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                }
                text = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
                LiText += text;
                LiText = (LiText + new string(' ', 135)).Left(135);
                List1_Items.Add(new(LiText + Modul1.FamInArb.AsString(),
                   (Modul1.Family.Mann, Modul1.Family.Frau, Modul1.FamInArb)));
                num5 += 1f;
                goto IL_06e6;
            }
        }
        goto IL_0711;
    /*  IL_0865:
          num4 = num2 + 1;
          goto IL_0869;
      IL_0869:
          num2 = 0;
          switch (num4)
          {
              case 1:
                  break;
              case 75:
              case 103:
                  goto IL_06e6;
              case 78:
              case 79:
              case 80:
              case 81:
                  goto IL_0711;
              case 9:
              case 10:
              case 82:
                  goto IL_0721;
              case 77:
              case 83:
              case 85:
              case 89:
              case 90:
              case 96:
              case 97:
              case 100:
              case 104:
              case 105:
                  goto end_IL_0001_2;
          }
          goto default;
  }
}
catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
{
  ProjectData.SetProjectError(obj, lErl);
  try0001_dispatch = 2587;
  continue;
}
throw ProjectData.CreateProjectError(-2146828237);*/
    end_IL_0001_2: // <========== 5
        return;
    }

    public void FamzeigDat(int Fam, out string LiText, ELinkKennz Kenn)
    {
        EEventArt[] aeConEvent = {
            EEventArt.eA_Marriage,  EEventArt.eA_MarrReligious, EEventArt.eA_504,  EEventArt.eA_505,
            EEventArt.eA_506,  EEventArt.eA_507, EEventArt.eA_500,  EEventArt.eA_501, EEventArt.eA_601
        };

        EEventArt num = default; // EEventArt
        string sDatu = "";
        foreach (var e in aeConEvent)
            if (GetEvtYear(Fam, num = e, out sDatu))
                break;


        string text = num switch
        {
            EEventArt.eA_500 => "Prok.",//Proklamation
            EEventArt.eA_501 => "Verl.",//Verlobung
            EEventArt.eA_Marriage => "Heir.",//Heirat
            EEventArt.eA_MarrReligious => "kirH.",//kirchliche Heirat
            EEventArt.eA_504 => "Scheid.",//Scheidung
            EEventArt.eA_505 => "Eheä.",//Eheähnliche Gemeinschaft
            EEventArt.eA_507 => "Dim.",//Dimissoriale
            EEventArt.eA_601 => "FiHr.",//Findbuch Heirat
            _ => "    ",
        };

        LiText = $"  {text,-6} {sDatu,4}";

        static bool GetEvtYear(int iLink, EEventArt num2, out string sDate)
        {
            DateTime dt;
            if ((dt = DataModul.Event.GetDate(num2, iLink)) != default)
            {
                sDate = $"{dt.Year,4}";
                return true;
            }
            else
            {
                sDate = "    ";
            }

            return false;
        }
    }

    public void Sterdat(byte St)
    {
        xDeathMark = false;
        Datles1(Modul1.PersInArb, Modul1.Person);
        if (Modul1.Person.Death.Trim() != "" | Modul1.Kont[18].Length > 0 | Modul1.Kont[23].Length > 0 | Modul1.Kont[33].Trim() != "" | Modul1.Kont[43].Trim() != "" | xDeathMark)
        {
            if (St == 0)
            {
                if (Option[EOutCfg.o14])
                {
                    _ = Document.AppendTextIfNd("\n");
                }
                else
                {
                    Document.AppendText(" ");
                }
            }
            else _ = Document.AppendTextIfNd("\n");

            if (Modul1.Person.Death != "")
            {
                Document.SetFont(new Font("Arial", 11.01f, FontStyle.Regular));
                Document.AppendText(Modul1.DTxt[3] + " " + Modul1.Person.Death.Trim() + ".");
            }
            if (Option[EOutCfg.o02] && Modul1.Kont[18].Length > 0)
            {
                string QuText = Modul1.Kont[18];
                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                Document.AppendText(" " + QuText);
            }
            if (Option[EOutCfg.o03] && Modul1.Kont[23].Length > 0)
            {
                string QuText = Modul1.Kont[23];
                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                Document.AppendText(" " + QuText);
            }
            if (Modul1.Kont[33].Trim() != "")
            {
                string QuText = Modul1.Kont[33];
                QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
                Document.AppendText(" " + QuText);
            }
            if (Document.AppendTextIfNd("."))
                Document.AppendText(" ");
            if (Modul1.Kont[43].Trim() != "")
            {
                Document.AppendText(" " + Modul1.Kont[43]);
            }
            if (xDeathMark)
            {
                Document.AppendText(Modul1.DTxt[3] + " verstorben.");
            }
        }
        if (!(Modul1.Person.Burial.Trim() != "" | Modul1.Kont[19].Length > 0 | Modul1.Kont[24].Length > 0 | Modul1.Kont[34].Trim() != "" | Modul1.Kont[44].Trim() != ""))
        {
            return;
        }
        if (St == 0)
        {
            if (Option[EOutCfg.o14])
            {
                _ = Document.AppendTextIfNd("\n");
            }
            else
            {
                Document.AppendText(" ");
            }
        }
        else _ = Document.AppendTextIfNd("\n");

        Document.AppendText(Modul1.DTxt[4] + " " + Modul1.Person.Burial.Trim() + ".");
        if (Option[EOutCfg.o02] && Modul1.Kont[19].Length > 0)
        {
            string QuText = Modul1.Kont[19];
            QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
            Document.AppendText(" " + QuText);
        }
        if (Option[EOutCfg.o03] && Modul1.Kont[24].Length > 0)
        {
            string QuText = Modul1.Kont[24];
            QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
            Document.AppendText(" " + QuText);
        }
        if (Modul1.Kont[34].Trim() != "")
        {
            string QuText = Modul1.Kont[34];
            QuText = Zeiweg(QuText, xStrip: !Option[EOutCfg.o07_KeepFormat]);
            Document.AppendText(" " + QuText);
        }
        if (Document.AppendTextIfNd("."))
            Document.AppendText(" ");
        _ = Document.AppendTextIfNd("\n");
        if (Modul1.Kont[44] != "")
        {
            Document.AppendText(" " + Modul1.Kont[44]);
        }
    }

    private void ListBox1_Click(object sender, EventArgs e)
    {
        ComboBox1.Text = Strings.Trim(ListBox1.Items[ListBox1.SelectedIndex].AsString().Left(240));
    }

    private void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        ComboBox1.Text = Strings.Trim(ListBox1.Items[ListBox1.SelectedIndex].AsString().Left(240));
        StartSearch();
    }

    public string Jobdreh(string Job, bool ereiRf)
    {
        var _jobSplit = Job.Split(' ');
        var _c = _jobSplit.Length - 1;
        if (ereiRf)
            (_jobSplit[_c - 1], _jobSplit[_c]) = (_jobSplit[_c], _jobSplit[_c - 1]);

        return string.Join(" ", _jobSplit);
    }

    [RelayCommand]
    private void ReqInfo()
    {
        string text = "Es ist  möglich mehrere Personen und Familienblätter auf einmal zu drucken.";
        text += "\nGeben Sie die Familien- oder Personennummern in dieses Feld in folgender Weise ein:";
        text += "\n1;6;45;9 druckt die Blätter 1 6 45 und 9";
        text += "\n5-67 druckt die Blätter 5 bis 67.";
        text += "\n";
        text += "\nDie Aufteilung auf einzelne Blätter müssen Sie anschließend in Ihrer Textverarbeitung manuell vornehmen, ";
        text += "\nda ich ja nicht wissen kann, welches Format Sie verwenden.";
        _ = Interaction.MsgBox(text);
    }

    private void ComboBox2_MouseClick(object sender, MouseEventArgs e)
    {
        Selection_Checked = false;
    }

    private void ComboBox2_TextChanged(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_060d
        if (!Selection_Checked)
        {
            Listleer();
        }
        Label4_Visible = false;
        Label10_Visible = false;
        Text2_Visible = false;
        ComboBox1.Text = "";

        Male_Visible = true;
        Females_Visible = true;
        FamOnly_Visible = true;
        Male2_Visible = true;
        Female2_Visible = true;

        OmitSpouse_Enabled = true;

        Male_Enabled = true;
        Female_Enabled = true;
        FamOnly_Enabled = true;

        var M1_Iter = 0;
        checked
        {
            Label1_Text[15] = new("");
            Label1_Text[16] = new("");

            Label1_Text[17] = new("");
            Label1_Text[18] = new("");

            Label1_Text[19] = new("");
            Label1_Text[20] = new("");

            Label1_Text[21] = new("");
            Label1_Text[22] = new("");

            Label1_Text[23] = new("");
            Label1_Text[24] = new("");

            Label1_Text[25] = new("");
            Label1_Text[26] = new("");

            Label1_Text[27] = new("");

            string left = ComboBox2.Text;
            if (left == Modul1.IText[EUserText.t314]
                || left == Modul1.IText[EUserText.t318]
                || left == Modul1.IText[EUserText.t320]
                || left == Modul1.IText[EUserText.t321]
                || left == Modul1.IText[EUserText.t339]
                || left == Modul1.IText[EUserText.t340])
            {
                if (!Selection_Checked)
                {
                    Male_Visible = false;
                    Females_Visible = false;
                    Male2_Visible = true;
                    Female2_Visible = true;
                }
                Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner                    Personennr.";
                FamOnly_Visible = true;
            }
            else if (left == Modul1.IText[EUserText.t315])
            {
                Label10_Visible = true;
                Text2_Visible = true;
                Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner                    Personennr.";
                OmitSpouse_Enabled = false;
                Male_Enabled = false;
                Female_Enabled = false;
                FamOnly_Enabled = false;

                Label4_Visible = true;
                Male2_Visible = false;
                Female2_Visible = false;
            }
            else if (left == Modul1.IText[EUserText.t313])
            {
                Male_Visible = false;
                Females_Visible = false;
                Male2_Visible = false;
                Female2_Visible = false;
                Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
            }
            else if (left == Modul1.IText[EUserText.t312] || left == Modul1.IText[EUserText.t311])
            {
                Label10_Visible = true;
                FamOnly_Checked = false;
                Text2_Visible = true;
                Label3_Text = "Name,Vorname                        Datum JJJJ-MM-TT Ort               Personennummer";
                Label4_Visible = true;
                OmitSpouse_Visible = false;
                Male_Visible = false;
                Females_Visible = false;
                Male2_Visible = true;
                Female2_Visible = true;
            }
            else
            {
                if (left == Modul1.IText[EUserText.t319])
                {
                    DataModul.DSB_SearchTable.Index = "Aliassuch";
                    DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
                    Male_Visible = false;
                    Females_Visible = false;
                    Male2_Visible = true;
                    Female2_Visible = true;
                    if (OmitSpouse_Checked)
                    {
                    }
                    return;
                }
                if (left == Modul1.IText[EUserText.t308])
                {
                    Male_Visible = false;
                    Females_Visible = false;
                    FamOnly_Visible = false;
                    Male2_Visible = false;
                    Female2_Visible = false;
                    Label3_Text = "Name,Vorname                               Personennummer Generation  Nachfahrennummer";
                }
                else if (left == Modul1.IText[EUserText.t309])
                {
                    Male_Visible = false;
                    Females_Visible = false;
                    FamOnly_Visible = false;
                    Male2_Visible = false;
                    Female2_Visible = false;
                    Label3_Text = "Name,Vorname                               Personennummer Generation  Ahnennummer";
                }
                else if (left == Modul1.IText[EUserText.t317] || left == Modul1.IText[EUserText.t316])
                {
                    Male_Visible = false;
                    Females_Visible = false;
                    Male2_Visible = true;
                    Female2_Visible = true;
                    M1_Iter = 0;
                    while (M1_Iter <= 15)
                    {
                        Label5_Text[(short)M1_Iter] = new("");
                        Label7_Text[(short)M1_Iter] = new("");
                        M1_Iter++;
                    }

                    Male_Enabled = true;
                    Female_Enabled = true;
                    FamOnly_Enabled = true;

                    List1_Items.Clear();
                    Male2_Visible = true;
                    Female2_Visible = true;
                    OmitSpouse_Enabled = true;
                    OmitSpouse_Visible = true;
                    FamOnly_Visible = false;
                    Label4_Visible = false;
                    Label10_Visible = false;
                    Text2_Visible = false;
                    Label3_Text = "Name,Vorname                        Bearb.Datum Personennummer                       Partner";
                    Label4_Visible = true;
                    OmitSpouse_Visible = false;
                    _ = ComboBox1.Focus();
                    ComboBox1.Text = "";
                    if (ComboBox2.Text == Modul1.IText[EUserText.t316])
                    {
                        FamPerschalt = 1;
                        Male2_Visible = false;
                        Female2_Visible = false;
                    }
                }
            }
            _ = ComboBox1.Focus();
        }
    }

    public string[] Datlf(int iFamNr)
    {
        short num = 500;
        string[] Datum = new string[2];
        Datum.Initialize();
        checked
        {
            short num2;
            short num3;
            do
            {
                int iArt = Modul1.Ubg;
                iArt = num;
                if (!!DataModul.Event.ReadData(((EEventArt)iArt, iFamNr, (short)0), out var evt))
                {
                    if (evt.dDatumV != default)
                    {
                        if (iArt == 500
                            || iArt == 501 & Datum[0] == ""
                            || iArt == 502 & Datum[0] == ""
                            || iArt == 503 & Datum[0] == ""
                            || iArt == 504 & Datum[0] == ""
                            || iArt == 505 & Datum[0] == "")
                        {
                            Datum[0] = evt.dDatumV.Year.ToString();
                        }
                    }
                    if (evt.iOrt > 0)
                    {
                        if (iArt == 500
                            || iArt == 501 & Datum[1] == ""
                            || iArt == 502 & Datum[1] == ""
                            || iArt == 503 & Datum[1] == ""
                            || iArt == 504 & Datum[1] == ""
                            || iArt == 505 & Datum[1] == "")
                        {
                            Datum[1] = Place_ReadData(evt.iOrt, 0, 1, Option[EOutCfg.o35]);
                        }
                    }
                }
                num = (short)unchecked(num + 1);
                num2 = num;
                num3 = 507;
            }
            while (num2 <= num3);
        }
        return Datum;
    }

    public void LPweg()
    {
        checked
        {
            while (true)
            {
                if (Document.TrimEnd(".")) continue;
                if (Document.TrimEnd()) continue;
                break;
            }
        }
    }


    public void Bildaus(string BiKe)
    {
        //Discarded unreachable code: IL_0780
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        string DateiName = default;
        int lErl = default;
        int num5 = default;
        Image image = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0009;
                    case 2543:
                        {
                            num2 = num;
                            switch (num3 <= -2 ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0841;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 5)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_071a;
                            }
                            else
                            {
                                if (Information.Err().Number == 53)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_05e6;
                                }
                                else
                                {
                                    if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_0845;
                                }
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_0009:
                        num = 2;
                        if (Option[EOutCfg.o13] | Option[EOutCfg.o41] | Option[EOutCfg.o42])
                        {
                            DataModul.DB_PictureTable.Index = "Perkenn  ";
                            num5 = BiKe == "P" ? Modul1.PersInArb : Modul1.FamInArb;
                            DataModul.DB_PictureTable.Seek("=", BiKe, num5);
                            goto IL_0757;
                        }
                        goto IL_0773;
                    IL_05e6: // <========== 4
                        num = 69;
                        lErl = 4;
                        Clipboard.Clear();
                        if (Option[EOutCfg.o13])
                        {
                            Document.AppendText("\n" + DateiName);
                        }
                        if (null != DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value)
                        {
                            Document.AppendText('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString());
                        }
                        if (null != DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value)
                        {
                            Document.AppendText('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString());
                        }
                        goto IL_071a;
                    IL_071a: // <========== 5
                        num = 83;
                        lErl = 2;
                        DataModul.DB_PictureTable.MoveNext();
                        Retweg3(Document);
                        Document.AppendText("\n");
                        goto IL_0757;
                    IL_0757: // <========== 3
                        num = 12;
                        if (!DataModul.DB_PictureTable.EOF)
                        {
                            if (!DataModul.DB_PictureTable.NoMatch)
                            {
                                Document.AppendText("\n");
                                if (!(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != num5))
                                {
                                    DateiName = Strings.Left(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(), 1) == "#"
                                        ? (Modul1.Verz + Strings.Mid(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(), 1, DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString())
                                        : DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();

                                    Bitmap bitmap;
                                    if (Option[EOutCfg.o42])
                                    {
                                        if (DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString() == "Personenbild")
                                        {
                                            DateiName = DateiName.Replace("#", "");
                                            FileStream fileStream = new FileStream(DateiName, FileMode.Open);
                                            bitmap = new Bitmap(fileStream);
                                            fileStream.Close();
                                            if (Option[EOutCfg.o44_PictOrginalSize])
                                            {
                                                Document.AppendImage(image);
                                                goto IL_05e6;
                                            }
                                            else
                                            {
                                                PictureBox pictureBox = PictureBox1;
                                                pictureBox.Image = AutoSizeImage(bitmap, pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height);
                                                pictureBox.Image = PicResizeByWidth(bitmap, 250);
                                                image = pictureBox.Image;
                                                pictureBox = null;
                                                Document.AppendImage(image);
                                                if (null != DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].Value)
                                                {
                                                    Document.AppendText('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString());
                                                }
                                                if (null != DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value)
                                                {
                                                    Document.AppendText('\n' + DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString());
                                                }
                                            }
                                        }
                                    }
                                    if (Option[EOutCfg.o41])
                                    {
                                        DateiName = DateiName.Replace("#", "");
                                        if (File.Exists(DateiName))
                                        {
                                            FileStream fileStream2 = new FileStream(DateiName, FileMode.Open);
                                            bitmap = new Bitmap(fileStream2);
                                            fileStream2.Close();
                                            if (Option[EOutCfg.o44_PictOrginalSize])
                                            {
                                                Document.AppendImage(image);
                                            }
                                            else
                                            {
                                                PictureBox pictureBox2 = PictureBox1;
                                                pictureBox2.Image = AutoSizeImage(bitmap, pictureBox2.ClientRectangle.Width, pictureBox2.ClientRectangle.Height);
                                                pictureBox2.Image = PicResizeByWidth(bitmap, 250);
                                                pictureBox2 = null;
                                                Image image2 = PictureBox1.Image;
                                                Document.AppendImage(image);
                                            }
                                            goto IL_05e6;
                                        }
                                    }
                                }
                                else goto IL_0773;
                            }
                            goto IL_071a;
                        }
                        goto IL_0773;
                    IL_0773: // <========== 4
                        num = 89;
                        lErl = 3;
                        goto end_IL_0001_2;
                    IL_0841:
                        num4 = num2 + 1;
                        goto IL_0845;
                    IL_0845:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 33:
                            case 60:
                            case 69:
                            case 97:
                                goto IL_05e6;
                            case 79:
                            case 80:
                            case 81:
                            case 82:
                            case 83:
                            case 93:
                                goto IL_071a;
                            case 11:
                            case 12:
                            case 87:
                                goto IL_0757;
                            case 16:
                            case 88:
                            case 89:
                                goto IL_0773;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 2543;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    public void ShowNamensuchDlg(string sTypeText, int iPersNr, int iFamNr)
    {
        Namensuch frmNamensuch = Namensuch.Default;
        frmNamensuch.Show();
        if (frmNamensuch.List1.SelectedIndex > 10)
        {
            frmNamensuch.List1.TopIndex = frmNamensuch.List1.SelectedIndex - 5;
        }
        frmNamensuch.Text = sTypeText;
        frmNamensuch.Close();
        frmNamensuch.Show();
        _ = frmNamensuch.ComboBox1.Focus();
        frmNamensuch.ComboBox1.SelectionStart = frmNamensuch.ComboBox1.Text.Length;
        frmNamensuch.Visible = false;
    }

    public void SetPerson(int persInArb, int iAn, short sShalt)
    {
        Namensuch frmNamensuch = Namensuch.Default;
        frmNamensuch.Show();
        PersNr = persInArb;

        frmNamensuch.btnPersonSheet.Visible = true;
        frmNamensuch.btnPersonSheet.PerformClick();
    }
}
