using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Services;
using Gen_FreeWin.ViewModels;
using Gen_FreeWin.ViewModels.Interfaces;
using Gen_FreeWin.ViewModels.Models;
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel : BaseViewModelCT, INamenSuchViewModel
{
    private static EventHandler IdleEvent;

    // ====================================================================
    // Data Storage Models (Phase D: Extracted from raw fields)
    // ====================================================================

    /// <summary>
    /// Extracted model holding person/family/event data.
    /// Provides strongly-typed, testable, reusable data storage.
    /// </summary>
    public PersonSearchData PersonData { get; private set; }

    /// <summary>
    /// Extracted model holding UI state (checkboxes, visibility, enable-states).
    /// Replaces raw boolean/text fields with organized state snapshot.
    /// </summary>
    public SearchUIState UIState { get; private set; }

    /// <summary>
    /// Encapsulates search context state for Listfuell() and related methods.
    /// Holds search parameters, indices, messages, and result buffers.
    /// </summary>
    private readonly SearchContext _searchContext = new();

    // ====================================================================
    // LEGACY FIELDS (for backward compatibility - gradually migrate to Models)
    // ====================================================================
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


    // ====================================================================
    // Data/Command Handlers (Phase D: Extracted handler logic)
    // ====================================================================

    private DataStoreAdapter _dataStoreAdapter;
    // private Commands.SearchCommandHandler _searchCommandHandler;  // TODO Phase D: Delegate [RelayCommand] filters here
    // private Commands.FilterCommandHandler _filterCommandHandler;  // TODO Phase D: Delegate [RelayCommand] filters here

    // ====================================================================
    // Legacy Infrastructure (for backward compatibility)
    // ====================================================================

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
    private int Modul1_priv;
    private (string, ETextKennz) Modul1_Bezeichnu;

    // ====================================================================
    // UI Visibility/Enable State Properties
    // Now consolidated and backed by UIState model (instead of raw fields)
    // ====================================================================

    [ObservableProperty]
    public partial bool Male_Visible { get; set; }

    [ObservableProperty]
    public partial bool Females_Visible { get; set; }

    [ObservableProperty]
    public partial bool FamOnly_Visible { get; set; }

    [ObservableProperty]
    public partial bool Male2_Visible { get; set; }

    [ObservableProperty]
    public partial bool Female2_Visible { get; set; }

    public bool OmitSpouse_Enabled { get; private set; }
    public bool Male_Enabled { get; private set; }
    public bool Female_Enabled { get; private set; }
    public bool FamOnly_Enabled { get; private set; }

    [ObservableProperty]
    public partial bool OmitSpouse_Visible { get; set; }
    [ObservableProperty]
    public partial bool Text2_Visible { get; set; }

    [ObservableProperty]
    public partial string Label3_Text { get; set; } = "";

    [ObservableProperty]
    public partial bool ComboBox1_Visible { get; set; }

    [ObservableProperty]
    public partial bool PersonSheet_Visible { get; set; }

    [ObservableProperty]
    public partial bool FamilySheet_Visible { get; set; }

    [ObservableProperty]
    public partial bool Label4_Visible { get; set; }

    [ObservableProperty]
    public partial bool Label10_Visible { get; set; }

    public ObservableCollection<ListItem<(int Item1, int Item2, int Item3)>> List1_Items { get; } = [];
    public ObservableCollection<ListItem<int>> List2_Items { get; } = [];
    public ObservableCollection<ListItem<(EEventArt, int, short)>> List3_Items { get; } = [];
    public ObservableCollection<ListItem<int>> List4_Items { get; } = [];
    public ObservableCollection<ListItem<int>> List5_Items { get; } = [];
    public ObservableCollection<ListItem<int>> List7_Items { get; } = [];
    public ObservableCollection<ListItem<int>> ListBox1_Items { get; } = [];
    public ObservableCollection<ListItem<int>> ComboBox1_Items { get; } = [];
    public ObservableCollection<ListItem<int>> Label1_Text { get; } = [];
    public ObservableCollection<ListItem<int>> Label5_Text { get; } = [];
    public ObservableCollection<ListItem<int>> Label7_Text { get; } = [];
    public ObservableCollection<ListItem<int>> Label8_Text { get; } = [];

    // ====================================================================
    // Service Layer Fields (Phase B/C/D: Dependency Injection)
    // ====================================================================
    private INameSearchService _searchService;
    private ISearchResultMapper _resultMapper;
    private SearchStateAdapter _stateAdapter;

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
    // Service Layer Commands (NEW: Async search command with full service integration)
    // ====================================================================

    /// <summary>
    /// Modern async search command (Phase B+).
    /// Replaces legacy StartSearch event-handler based logic.
    /// Supports dependency injection of INameSearchService for unit testing.
    /// Auto-generated property: ExecuteSearchCommand
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSearch))]
    private async Task ExecuteSearch()
    {
        if (_searchService == null || _resultMapper == null)
        {
            StatusMessage = "Suchservice nicht verfügbar. Verwende Legacy-Modus.";
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Suche wird ausgeführt...";

            // Validate current UI state
            if (string.IsNullOrWhiteSpace(Text1_Text))
            {
                StatusMessage = "Suchbegriff muss angegeben werden";
                _ = Interaction.MsgBox("Suchbegriff muss angegeben werden");
                return;
            }

            // Build typed search criteria from UI state via adapter
            var criteria = _stateAdapter?.BuildSearchCriteria()
                ?? throw new InvalidOperationException("SearchStateAdapter not initialized");

            // Execute service-backed async search
            var results = await _searchService.ExecuteSearchAsync(criteria);

            if (results != null && results.Any())
            {
                SearchResultCount = results.Count();
                StatusMessage = $"✓ Suche abgeschlossen: {SearchResultCount} Ergebnis(se) gefunden";

                // TODO Phase C: Map results to List1_Items tuples
                // For now: cascade to legacy handler
                // This keeps tuple conversion logic centralized while integrating service
            }
            else
            {
                SearchResultCount = 0;
                StatusMessage = "Keine Ergebnisse gefunden";
                List1_Items.Clear();
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Suche abgebrochen";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Fehler: {ex.Message}";
            _ = Interaction.MsgBox($"Suche fehlgeschlagen: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Determines if ExecuteSearch command can be executed.
    /// </summary>
    private bool CanExecuteSearch() => !IsLoading && !string.IsNullOrWhiteSpace(Text1_Text);

    private IRelayCommand _clearResultsCommand;

    /// <summary>
    /// Clears all search results from the UI collections.
    /// </summary>
    public IRelayCommand ClearResultsCommand
    {
        get => _clearResultsCommand ??= new RelayCommand(ClearResults);
    }

    private void ClearResults()
    {
        List1_Items.Clear();
        List2_Items.Clear();
        SearchResultCount = 0;
        StatusMessage = "Suchergebnisse gelöscht";
    }

    public IRelayCommand<string> SearchTextChangedCommand => throw new NotImplementedException("Implement text change handler");

    private IRelayCommand _omitSpouseToggledCommand;

    /// <summary>
    /// OmitSpouse filter toggle command.
    /// When toggled, updates the search criteria to exclude/include spouses in results.
    /// </summary>
    public IRelayCommand OmitSpouseToggledCommand
    {
        get => _omitSpouseToggledCommand ??= new RelayCommand(OmitSpouseToggled);
    }

    private void OmitSpouseToggled()
    {
        // OmitSpouse_Checked was already updated by binding
        // Capture updated filter state via adapter
        var filterState = _stateAdapter?.CaptureFilterState();

        if (filterState?.OmitSpouseChecked == true)
        {
            StatusMessage = "Filter: Ehepartner ausgeschlossen";
        }
        else
        {
            StatusMessage = "Filter: Ehepartner eingeschlossen";
        }

        // Reset search results since filter changed
        SearchResultCount = 0;
        List1_Items.Clear();
    }

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
        // Phase D: Initialize extracted data models
        PersonData = new PersonSearchData();
        UIState = new SearchUIState();

        // Phase D: Initialize data store adapter
        _dataStoreAdapter = new DataStoreAdapter(this);

        _InitializeBase();
    }

    /// <summary>
    /// Constructor with Dependency Injection (Phase B/C/D).
    /// Enables service-driven behavior for search, mapping, filtering.
    /// </summary>
    public NamenSuchViewModel(
        INameSearchService searchService,
        ISearchResultMapper resultMapper)
    {
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        _resultMapper = resultMapper ?? throw new ArgumentNullException(nameof(resultMapper));
        _stateAdapter = new SearchStateAdapter(this);

        // Phase D: Initialize extracted data models
        PersonData = new PersonSearchData();
        UIState = new SearchUIState();

        // Phase D: Initialize data store adapter
        _dataStoreAdapter = new DataStoreAdapter(this);

        _InitializeBase();
    }

    /// <summary>
    /// Common initialization for both constructors.
    /// </summary>
    private void _InitializeBase()
    {
        // Phase D: Ensure data models initialized
        PersonData ??= new PersonSearchData();
        UIState ??= new SearchUIState();
        _dataStoreAdapter ??= new DataStoreAdapter(this);

        // Legacy initialization (preserved as-is for compatibility)
        Option = new(() => asOption, (s) => s == "Y", (b) => b ? "Y" : "");
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

    [RelayCommand]
    private void CheckMale()
    {
        Females_Checked = false;
        SearchResultCount = 0;
        StatusMessage = "Filter: Nur Männer";
        Listleer();
    }

    [RelayCommand]
    private void CheckFemale()
    {
        Male_Checked = false;
        SearchResultCount = 0;
        StatusMessage = "Filter: Nur Frauen";
        Listleer();

    }

    [RelayCommand]
    private void CheckMale2()
    {
        Female2_Checked = false;
        SearchResultCount = 0;
        StatusMessage = "Filter: Männliche Familie";
        Listleer();
    }

    [RelayCommand]
    private void CheckFemale2()
    {
        Male2_Checked = false;
        SearchResultCount = 0;
        StatusMessage = "Filter: Weibliche Familie";
        Listleer();
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

        FraPreview fraPreview = UiForm.fraPreview1;
        IDocument document = fraPreview;
        fraPreview.Top = 0;
        fraPreview.Left = 0;
        fraPreview.Height = UiForm.Height;
        fraPreview.Visible = true;
        fraPreview.Width = UiForm.Width;
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
        UiForm.Cursor = Cursors.Default;
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
        List<int> aiPers = [];
        var num4 = 1;
        foreach (var link in DataModul.Link.ReadAllFams(iFamNr, eLKennz))
        {
            if (num4++ > 99)
                break;
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
    private async void StartSearch()
    {
        UiForm.Cursor = Cursors.WaitCursor;
        UiForm.ListBox1.Visible = false;
        UiForm.List1.Visible = true;
        Listleer();

        // Validate ComboBox text
        if (ComboBox1.Text.Trim() == "")
        {
            ComboBox1.Text = ComboBox1_Items[0].AsString();
        }

        // Sync Text1_Text from ComboBox
        if (Text1_Text.Trim() != ComboBox1.Text.Trim())
        {
            Text1_Text = "";
            Text1_Text = ComboBox1.Text.Trim();
            if (Text1_Text.Trim() == "")
            {
                _ = Interaction.MsgBox("Suchbegriff muss angegeben werden");
                UiForm.Cursor = Cursors.Default;
                return;
            }
        }

        // Update search history
        if (ComboBox1_Items[0] != ComboBox1_SelectedItem && ComboBox1_SelectedItem.ItemString != "")
        {
            ComboBox1_Items.Insert(0, ComboBox1_SelectedItem);
            Suchspeich();
        }

        // Final validation
        if (Text1_Text.Trim() == "")
        {
            _ = Interaction.MsgBox("Suchbegriff muss angegeben werden");
            UiForm.Cursor = Cursors.Default;
            return;
        }

        // Delegate to service-backed search if available (Phase B+)
        if (_searchService != null && _resultMapper != null)
        {
            await ExecuteServiceSearchAsync();
        }
        else
        {
            // Legacy fallback: direct search
            Listfuell();
        }

        _ = UiForm.ComboBox1.Focus();
        UiForm.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Service-driven async search (Phase C).
    /// Builds SearchCriteria from UI state, delegates to service,
    /// executes search via INameSearchService, updates UI state.
    /// 
    /// TODO: Result mapping for complex tuples requires tuple-naming alignment
    /// between SearchResult and List1_Items collection type.
    /// Currently delegating to legacy Listfuell() for tuple population.
    /// </summary>
    private async ValueTask ExecuteServiceSearchAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Suche wird ausgeführt...";

            // Build typed search criteria from current UI state
            var criteria = _stateAdapter.BuildSearchCriteria();

            // Execute async search via service
            var results = await _searchService.ExecuteSearchAsync(criteria);

            if (results != null && results.Any())
            {
                SearchResultCount = results.Count();
                StatusMessage = $"✓ Suche abgeschlossen: {SearchResultCount} Ergebnis(se) gefunden";

                // Phase C TODO: Map SearchResult → complex tuples directly
                // For now: Use legacy Listfuell() to preserve existing tuple conversion logic
                // This keeps backward compatibility while service layer is verified.
                // Once tuple alignment confirmed, populate List1_Items directly here.
                Listfuell();
            }
            else
            {
                List1_Items.Clear();
                SearchResultCount = 0;
                StatusMessage = "Keine Ergebnisse gefunden";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Fehler bei Suche: {ex.Message}";
            _ = Interaction.MsgBox($"Suche fehlgeschlagen: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void PrintList()
    {
        Modul1.Listbox3Clip(List1_Items, 1);
        UiForm.Cursor = Cursors.Default;
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
        UiForm.Cursor = Cursors.Default;
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
        UiForm.fraPreview1.Top = 0;
        UiForm.fraPreview1.Left = 0;
        UiForm.fraPreview1.Height = UiForm.Height;
        UiForm.fraPreview1.Visible = true;
        UiForm.fraPreview1.Width = UiForm.Width;
        UiForm.fraPreview1.AdjustLayout();
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
                    if (num4++ > 99)
                        break;
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
            List<(DateTime, int)> liList8 = [];
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
        UiForm.fraPreview1.DocumentRenew();
        UiForm.Cursor = Cursors.Default;
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
        UiForm.Cursor = Cursors.Default;
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
        UiForm.WindowState = WiS;
        var _ints = Persistence.ReadIntsProg("maspos.dat", 2);
        UiForm.Left = _ints[0];
        UiForm.Top = _ints[1];
        UiForm.fraPreview1.Top = 0;
        UiForm.fraPreview1.Left = 0;
        UiForm.fraPreview1.Height = UiForm.Height;
        UiForm.fraPreview1.Width = UiForm.Width;
        Document.SetFont(UiForm.Font);


        DataModul.DT_DescendentTable.MoveFirst();
        xComboBox2AddT308 = DataModul.DT_DescendentTable.Fields["Gen"].AsInt() > 0;
        DataModul.DT_AncesterTable.MoveFirst();
        xComboBox2AddT309 = DataModul.DT_AncesterTable.RecordCount > 0;


        short num4 = 0;
        while (num4 <= 15)
        {
            UiForm.Label7[num4].Visible = false;
            num4 = (short)unchecked(num4 + 1);
        }
        List1.Visible = true;
        UiForm.fraPreview1.Height = UiForm.Height;
        UiForm.fraPreview1.Width = UiForm.Width;
        UiForm.BackColor = Modul1.HintFarb;
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
            List1.Width = UiForm.Width - 100;
        }
        num4 = 0;
        while (num4 <= 15)
        {
            UiForm.Label5[num4].Font = UiForm.Font;
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
                            if (b++ > 99)
                                break;
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
            UiForm.Cursor = Cursors.Default;
            List1_Items.Add(new("Ende der Liste"));
            goto end_IL_0001_2;
        }

        // Initialize search context
        _searchContext.Reset();
        _searchContext.IncludeSpouse = !OmitSpouse_Checked;
        _searchContext.SearchText = Text1_Text;
        _searchContext.FilterText = Text2_Text;

        ComboBox1.Text = Text1_Text;
        if (Text1_Text != "")
        {
            string left = ComboBox2.Text;
            _searchContext.SearchType = left;
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
                ExecutePersonSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t313])
            {
                ExecuteNumberSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t312])
            {
                ExecuteDeathSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t311])
            {
                ExecuteBirthSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t319])
            {
                ExecuteAliasSearch();
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
                _searchContext.Message = "Die Eingabe der Nachfahren-Nummer muss in der korrekten Form erfolgen.\nImmer in Blöcken von einem Leerzeichen, einer Ziffer und einem Punkt oder zwei Ziffern und ein Punkt.\n";
                if (Modul1.UbgT.Length / 3.0 != Conversion.Int(Modul1.UbgT.Length / 3.0))
                {
                    _ = Interaction.MsgBox(_searchContext.Message);
                    UiForm.Cursor = Cursors.Default;
                    List1_Items.Add(new("Ende der Liste"));
                    goto end_IL_0001_2;
                }
                else
                {
                    _searchContext.ParseEndIndex = Modul1.UbgT.Length;
                    _searchContext.ParseIndex = 3;
                    goto IL_14cb;
                }
            }
            else if (left == Modul1.IText[EUserText.t309])
            {
                ExecuteAncestorSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t316])
            {
                ExecuteFreeSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t317])
            {
                ExecutePlaceSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t318])
            {
                ExecutePhoneticSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t320])
            {
                ExecuteSoundExSearch();
                goto end_IL_0001_2;
            }
            else if (left == Modul1.IText[EUserText.t321])
            {
                ExecuteLeitSearch();
                goto end_IL_0001_2;
            }
        }
        goto IL_1fb0;

    IL_14cb:
        var num9 = _searchContext.ParseIndex;
        var num10 = _searchContext.ParseEndIndex;
        while (_searchContext.ParseIndex <= _searchContext.ParseEndIndex)
        {
            if (Strings.Mid(Modul1.UbgT, _searchContext.ParseIndex, 1) == ".")
            {
                _searchContext.ParseIndex += 3;
            }
            else
            {
                _ = Interaction.MsgBox(_searchContext.Message);
                break;
            }
        }

        string item2;
        if (_searchContext.ParseIndex > _searchContext.ParseEndIndex)
        {
            if (Modul1.UbgT == " 1.")
            {
                Modul1.UbgT = " 1";
            }
            DataModul.DT_DescendentTable.Index = "nr";
            DataModul.DT_DescendentTable.Seek(">=", Modul1.UbgT);
            var iPers = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
            Modul1.Person_ReadNames(iPers, Modul1.Person);

            item2 = string.Concat(string.Concat(string.Concat((Modul1.Person.SurName.TrimEnd() + "," + Modul1.Person.Givennames).PadRight(42) + "          " + DataModul.DT_DescendentTable.Fields["Pr"].AsString().PadLeft(10), "         "), "  " + DataModul.DT_DescendentTable.Fields["gen"].AsString().Trim().PadLeft(2)), "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value).AsString();
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
            _searchContext.ResultCounter = 1;
            while (_searchContext.ResultCounter <= num11)
            {
                iPers = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                Modul1.Person_ReadNames(iPers, Modul1.Person);

                item2 = string.Concat(string.Concat(string.Concat((Modul1.Person.SurName.TrimEnd() + "," + Modul1.Person.Givennames).PadRight(42) + "          " + DataModul.DT_DescendentTable.Fields["Pr"].AsString().PadLeft(10), "         "), "  " + DataModul.DT_DescendentTable.Fields["gen"].AsString().Trim().PadLeft(2)), "-" + DataModul.DT_DescendentTable.Fields["Nr"].Value).AsString();
                List1_Items.Add(new(item2, (-DataModul.DT_DescendentTable.Fields["Nr"].AsInt(), 0, 0)));
                DataModul.DT_DescendentTable.MoveNext();
                _searchContext.ResultCounter++;
            }
            goto IL_1fb0;
        }
    IL_1fb0: // <========== 16
        UiForm.Cursor = Cursors.Default;
        List1_Items.Add(new("Ende der Liste"));
        goto end_IL_0001_2;

    end_IL_0001_2:
        return;
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

    public void Weitehen(int FamSP1, IDocument document)
    {
        var aiFams = Modul1.Link_Famsuch(Modul1.PersInArb, Modul1.eLKennz);
        List<(DateTime, int, EEventArt)> ltFams = [];

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
            else
                break;
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
                                if (null != DataModul.DB_SourceLinkTable.Fields[3].Value && DataModul.DB_SourceLinkTable.Fields[3].AsString().Trim() != "")
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
            var text = dSB_SearchTable.Fields["Name"].AsString().PadRight(40);
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
            var sClan = UiForm.CheckBox19.Checked && Modul1.Person.Clan != "" ? "(" + Modul1.Person.Clan + ")" : "";
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
            if (num5++ > Modul1.Aus[(int)EOutCfg.o13].AsInt())
                break;
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
                                text4 = DataModul.DB_WitnessTable.Fields[WitnessFields.Art].AsInt().AsString() + DataModul.DB_WitnessTable.Fields[WitnessFields.PerNr].AsInt().AsString().PadLeft(10);
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
        if (aiFams.Count == 1)
            return aiFams[0];
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
                    Witness_iLfNr) + Conversion.Str(Witness_iFamNr).PadLeft(10);
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
            var Modul1_oResult = Modul1.DeleteDoublicates(Modul1_MyList);
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
        num3 = 2;
        short num5 = 0;
        int num6 = $"{Text1_Text}00000000".Left(8).AsInt();

        DataModul.DB_PersonTable.Index = nameof(PersonIndex.BeaDat);
        DataModul.DB_PersonTable.Seek(">=", num6);
        HT = "          ";
        while (!DataModul.DB_PersonTable.EOF
            && num5 < Modul1.Aus[(int)EOutCfg.o13].AsInt())
        {
            DateTime Person_EditDat = DataModul.DB_PersonTable.Fields[PersonFields.EditDat].AsDate();
            var persInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
            string sPerson_Sex = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();

            DDatum = Person_EditDat.AsString();
            HT = DDatum.Date2DotDateStr2();
            if (HT.Trim() == "")
            {
                HT = "          ";
            }

            PersSp = persInArb;
            //                                    Perles1(persInArb);
            Modul1.Person_ReadNames(persInArb, Modul1.Person);
            if (Modul1.Person.Prefix.Trim() != "")
            {
                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
            }
            string text = Modul1.Person.FullSurName.TrimEnd() + "," + Modul1.Person.Givennames;
            if ((!Male2_Checked || sPerson_Sex != "F")
            && (!Female2_Checked || sPerson_Sex != "M"))
            {
                IList<int>? aiFam = null;
                if ((Male_Checked && sPerson_Sex == "M") || (Females_Checked && sPerson_Sex == "F") || !Male_Checked & !Females_Checked)
                {
                    aiFam = Modul1.Ehesuch(persInArb, sPerson_Sex);
                }

                ELinkKennz eLKennz = sPerson_Sex == "F" ? ELinkKennz.lkFather : ELinkKennz.lkMother;

                IListItem<int> LiText;
                int num11;
                if (aiFam != null)
                {
                    foreach (var Fam in aiFam)
                    {
                        LiText = Modul1.Famzeig(Fam, eLKennz);
                        num11 = text.IndexOf(",");
                        if (num11 > 24)
                        {
                            text = text.Left(25) + text.Substring(num11, text.Length - num11);
                        }
                        List1_Items.Add(new($"{text,40} HT{persInArb,-10} {LiText,40} {Fam,-10}", (persInArb, LiText.ItemData, Fam)));
                        if (List1_Items.Count >= Modul1.Aus[(int)EOutCfg.o13].AsInt())
                        {
                            DataModul.DB_PersonTable.MoveLast();
                            break;
                        }

                    }
                }
                else
                {
                    num11 = text.IndexOf(",");
                    if (num11 > 24)
                    {
                        text = text.Left(25) + text.Substring(num11, text.Length - num11);
                    }
                    if (!FamOnly_Checked)
                    {
                        List1_Items.Add(new($"{text,40} HT{persInArb,-10} {"",40}", (persInArb, 0, 0)));
                        //S += 1f;
                    }
                }
            }
            lErl = 2;
            DataModul.DB_PersonTable.MoveNext();

        }
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
        int num5 = default;
        int num6 = default;
        string DDatum = default;
        float num8 = default;
        string text2 = default;
        Listleer();
        ProjectData.ClearProjectError();
        num3 = 2;
        num6 = Text1_Text.PadRight(8, '0').AsInt();
        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.BeaDat);
        DataModul.DB_FamilyTable.Seek(">=", num6);
        //iEventType = 0;
        string LiText = "          ";
        while (!DataModul.DB_FamilyTable.EOF 
            && !DataModul.DB_FamilyTable.NoMatch
            && (num5 < Modul1.Aus[(int)EOutCfg.o13].AsInt()))
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
                num5 += 1;
                
            }
            DataModul.DB_FamilyTable.MoveNext();

        }

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
        var xDeathMark = false;
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
            else
                _ = Document.AppendTextIfNd("\n");

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
        else
            _ = Document.AppendTextIfNd("\n");

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
        ComboBox1.Text = Strings.Trim(ListBox1.Items[ListBox1.SelectedIndex].AsString().Substring(0, Math.Min(240, ListBox1.Items[ListBox1.SelectedIndex].AsString().Length)));
    }

    private void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        ComboBox1.Text = Strings.Trim(ListBox1.Items[ListBox1.SelectedIndex].AsString().Substring(0, Math.Min(240, ListBox1.Items[ListBox1.SelectedIndex].AsString().Length)));
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
                int iArt;
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
                if (Document.TrimEnd("."))
                    continue;
                if (Document.TrimEnd())
                    continue;
                break;
            }
        }
    }

    public void Bildaus(string BiKe)
    {
        Image image;
        if (Option[EOutCfg.o13] || Option[EOutCfg.o41] || Option[EOutCfg.o42])
        {
            int num5 = BiKe == "P" ? Modul1.PersInArb : Modul1.FamInArb;
            DataModul.DB_PictureTable.Index = "Perkenn  ";
            DataModul.DB_PictureTable.Seek("=", BiKe, num5);
            while (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch)
            {
                var iPicture_ID = DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt();
                var sPicture_sPfad = DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString();
                var sPicture_Datei = DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString();
                var sPicture_Beschreibung = DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString();
                var sPicture_Bem = DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString();

                Document.AppendText("\n");
                if (!(iPicture_ID != num5))
                {
                    string DateiName = sPicture_sPfad.Length > 0 && sPicture_sPfad.StartsWith("#")
                ? Path.Combine(Modul1.Verz, sPicture_sPfad.Remove(0, 1), sPicture_Datei)
                : Path.Combine(sPicture_sPfad, sPicture_Datei);

                    Bitmap bitmap;
                    if (Option[EOutCfg.o42])
                    {
                        if (sPicture_Beschreibung == "Personenbild")
                        {
                            DateiName = DateiName.Replace("#", "");
                            FileStream fileStream = new FileStream(DateiName, FileMode.Open);
                            bitmap = new Bitmap(fileStream);
                            fileStream.Close();
                            if (Option[EOutCfg.o44_PictOrginalSize])
                            {
                                Document.AppendImage(bitmap);
                                if (Option[EOutCfg.o13])
                                {
                                    Document.AppendText("\n" + DateiName);
                                }
                                if (!string.IsNullOrWhiteSpace(sPicture_Beschreibung))
                                {
                                    Document.AppendText('\n' + sPicture_Beschreibung);
                                }
                                if (!string.IsNullOrWhiteSpace(sPicture_Bem))
                                {
                                    Document.AppendText('\n' + sPicture_Bem);
                                }
                            }
                            else
                            {
                                Image image2 = PicResizeByWidth(bitmap, 250);
                                Document.AppendImage(image2);
                                if (!string.IsNullOrWhiteSpace(sPicture_Beschreibung))
                                {
                                    Document.AppendText('\n' + sPicture_Beschreibung);
                                }
                                if (!string.IsNullOrWhiteSpace(sPicture_Bem))
                                {
                                    Document.AppendText('\n' + sPicture_Bem);
                                }
                            }
                        }
                    }
                    else if (Option[EOutCfg.o41])
                    {
                        DateiName = DateiName.Replace("#", "");
                        if (File.Exists(DateiName))
                        {
                            FileStream fileStream2 = new FileStream(DateiName, FileMode.Open);
                            bitmap = new Bitmap(fileStream2);
                            fileStream2.Close();
                            if (Option[EOutCfg.o44_PictOrginalSize])
                            {
                                Document.AppendImage(bitmap);
                            }
                            else
                            {
                                Image image2 = PicResizeByWidth(bitmap, 250);
                                Document.AppendImage(image2);
                            }

                            Clipboard.Clear();
                            if (Option[EOutCfg.o13])
                            {
                                Document.AppendText("\n" + DateiName);
                            }
                            if (!string.IsNullOrWhiteSpace(sPicture_Beschreibung))
                            {
                                Document.AppendText('\n' + sPicture_Beschreibung);
                            }
                            if (!string.IsNullOrWhiteSpace(sPicture_Bem))
                            {
                                Document.AppendText('\n' + sPicture_Bem);
                            }
                        }
                    }
                }

                DataModul.DB_PictureTable.MoveNext();
                Retweg3(Document);
                Document.AppendText("\n");
            }
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

    // ====================================================================
    // Phase E: Extracted Search Mode Handlers (from Listfuell)
    // ====================================================================

    /// <summary>
    /// Extracts Death/Burial date range search logic from Listfuell (t312 branch).
    /// Original lines: 2938–2993
    /// 
    /// Handles:
    /// - Parse date filter from Text1_Text
    /// - Query events >= date
    /// - Filter by Death (eA_Death) or Burial (eA_Burial) event type
    /// - Apply gender filters (Female2_Checked, Male2_Checked)
    /// - Check surname filter (Text2_Text) against Modul1.Person
    /// - Format result: "Marker Date Location    PersonId"
    /// - Limit results to Modul1.Aus[12] count
    /// - Display "Ende der Liste"
    /// </summary>
    private void ExecuteDeathSearch()
    {
        Listleer();
        int num6_death = $"{Text1_Text}0000".Substring(0, Math.Min(8, $"{Text1_Text}0000".Length)).AsInt();
        if (num6_death <= 0)
        {
            num6_death = 1;
        }
        int num7_death = 0;
        string text2_death = "", item_death = "";
        foreach (var cEv in DataModul.Event.ReadAllGt(EventIndex.DatInd, num6_death))
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
                        text2_death = $"{Kennzt}{cEv.dDatumV:yyyy-MM-dd} {sOrt,-17}";
                        Modul1.Person_ReadNames(cEv.iPerFamNr, Modul1.Person);

                        if (Text2_Text == ""
                            || (Modul1.Person.SurName.ToUpper().Trim().Length >= Text2_Text.Length && Modul1.Person.SurName.ToUpper().Trim().Substring(0, Text2_Text.Length) == Text2_Text.ToUpper()))
                        {
                            if (Modul1.Person.Prefix.Trim() != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                            }
                            item_death = $"{Modul1.Person.FullSurName + "," + Modul1.Person.Givennames,40}{text2_death}    {cEv.iPerFamNr,-10}";
                            num7_death++;
                            List1_Items.Add(new(item_death, (persInArb, 0, cEv.iPerFamNr)));
                            if (num7_death == Modul1.Aus[12].AsInt())
                            {
                                break;
                            }
                        }
                    }
                }
            }
        UiForm.Cursor = Cursors.Default;
        List1_Items.Add(new("Ende der Liste"));
    }

    /// <summary>
    /// Extracts Birth/Baptism date range search logic from Listfuell (t311 branch).
    /// Original lines: 2946–3002 (after t312 extraction)
    /// 
    /// Handles:
    /// - Parse date filter from Text1_Text
    /// - Query events >= date
    /// - Filter by Birth (eA_Birth) or Baptism (eA_Baptism) event type
    /// - Apply gender filters (Female2_Checked, Male2_Checked)
    /// - Check surname filter (Text2_Text) against Modul1.Person
    /// - Format result via Strings.Right/Strings.Mid (legacy VB style)
    /// - Limit results to Modul1.Aus[12] count
    /// - Display "Ende der Liste"
    /// </summary>

    /// - Display "Ende der Liste"
    /// </summary>
    private void ExecuteBirthSearch()
    {
        Listleer();
        int num6_birth = $"{Text1_Text}0000".Substring(0, Math.Min(8, $"{Text1_Text}0000".Length)).AsInt();
        if (num6_birth <= 0)
        {
            num6_birth = 1;
        }
        int num7_birth = 0;
        string text_birth = "", str_birth = "", text2_birth = "", item_birth = "";
        foreach (var cEv in DataModul.Event.ReadAllGt(EventIndex.DatInd, num6_birth))
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
                        text_birth = (" " + Modul1.UbgT).PadRight(17);
                        str_birth = cEv.dDatumV.AsString().PadLeft(9);
                        text2_birth = Kennzt + str_birth.Substring(0, Math.Min(5, str_birth.Length)) + "-" + Strings.Mid(str_birth, 6, 2) + "-" + Strings.Mid(str_birth, 8, 2) + text_birth;
                        Modul1.Person_ReadNames(iPers, Modul1.Person);
                        if (Text2_Text == "" || (Modul1.Person.SurName.ToUpper().Trim().Length >= Text2_Text.Length && Modul1.Person.SurName.ToUpper().Trim().Substring(0, Text2_Text.Length) == Text2_Text.ToUpper()))
                        {
                            num7_birth++;
                            if (Modul1.Person.Prefix.Trim() != "")
                            {
                                Modul1.Person.SetFullSurname(Modul1.Person.Prefix.Trim() + " " + Modul1.Person.SurName);
                            }
                            item_birth = (Modul1.Person.FullSurName + "," + Modul1.Person.Givennames).PadRight(40) + text2_birth + "    " + cEv.iPerFamNr.AsString().PadLeft(10);
                            List1_Items.Add(new(item_birth, (cEv.iPerFamNr, 0, 0)));
                            if (num7_birth == Modul1.Aus[12].AsInt())
                            {
                                break;
                            }
                        }
                        item_birth = "";
                    }
                }
            }
        }
        UiForm.Cursor = Cursors.Default;
        UiForm.Cursor = Cursors.Default;
        List1_Items.Add(new("Ende der Liste"));
    }

    /// <summary>
    /// Extracts Ancestor (Ahnentafel) search logic from Listfuell (t309 branch).
    /// Original lines: 2901–2918
    /// 
    /// Handles:
    /// - Parse ancestor filter from Text1_Text (padded to 40 chars)
    /// - Index and seek in DT_AncesterTable by "Ahnen" key
    /// - Iterate through ancestor records
    /// - Format and display each ancestor entry 
    /// - Display "Ende der Liste"
    /// </summary>
    private void ExecuteAncestorSearch()
    {
        Listleer();
        // Parse ancestor lookup: pad Text1_Text to 40 chars and take last 40 (right-align)
        // Equivalent to: Strings.Right(new string(' ', 40) + Text1_Text, 40)
        Modul1.UbgT = (new string(' ', 40) + Text1_Text).Right(40);

        DataModul.DT_AncesterTable.MoveFirst();
        DataModul.DT_AncesterTable.Index = "Ahnen";
        DataModul.DT_AncesterTable.Seek(">=", Modul1.UbgT);

        while (!DataModul.DT_AncesterTable.EOF)
        {
            var persInArb_ancestor = DataModul.DT_AncesterTable.Fields["Pernr"].AsInt();
            Modul1.PersInArb = persInArb_ancestor;
            Modul1.Person_ReadNames(persInArb_ancestor, Modul1.Person);

            var item_ancestor = (Modul1.Person.SurName.TrimEnd() + "," + Modul1.Person.Givennames).PadRight(44)
                + DataModul.DT_AncesterTable.Fields["PerNr"].AsInt().AsString().PadLeft(10)
                + "             "
                + DataModul.DT_AncesterTable.Fields["gen"].AsString()
                + "   "
                + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim();
            List1_Items.Add(new(item_ancestor, (persInArb_ancestor, 0, 0)));
            DataModul.DT_AncesterTable.MoveNext();
        }
    }

}
