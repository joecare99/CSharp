using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Main;
using Gen_FreeWin.Views;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
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
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

public partial class Menu1ViewModel : BaseViewModelCT, IMenu1ViewModel
{
    public IProjectData ProjectData => field ??= IoC.GetRequiredService<IProjectData>();

    private Menue View => Menue.Default;

    public IModul1 Modul1 => field ??= IoC.GetRequiredService<IModul1>();

    private FormWindowState WindowState
    {
        get => ((FormWindowState?)GetWindowState?.Invoke()) ?? FormWindowState.Normal;
        set => SetWindowState?.Invoke(value);
    }

    private int lstList3Width;
    public IInteraction Interaction { get; set; }

    public Action<Enum> SetWindowState { get; set; }
    public Func<Enum> GetWindowState { get; set; }
    public Action<float> Grossaend { private get; set; }

    IRelayCommand IMenu1ViewModel.FormClosingCommand => FormClosingCommand;

    public IFraStatisticsViewModel Statistics { get; }

    public float FontSize => throw new NotImplementedException();

    [ObservableProperty]
    private Color _backColor;
    [ObservableProperty]
    private Color _ownerBackColor;
    [ObservableProperty]
    private Color _addressBackColor;
    [ObservableProperty]
    private Color _trackBar1BackColor;
    [ObservableProperty]
    private Color _mandantPathBackColor;
    [ObservableProperty]
    private Color _frmWindowSizeBackColor;

    [ObservableProperty]
    private bool _creationDateVisible;
    [ObservableProperty]
    private bool _markedVisible;
    [ObservableProperty]
    private bool _notesVisible;
    [ObservableProperty]
    private bool _ListBox2Visible;
    [ObservableProperty]
    private bool _list3Visible;
    [ObservableProperty]
    private bool _codeOfArmsVisible;
    [ObservableProperty]
    private bool _warningVisible;
    [ObservableProperty]
    private bool _enterLizenzVisible;
    [ObservableProperty]
    private bool _checkUpdateVisible;
    [ObservableProperty]
    private bool _updateVisible;
    [ObservableProperty]
    private bool _dateTimePicker1Visible;
    [ObservableProperty]
    private bool _setDateVisible;
    [ObservableProperty]
    private bool _frmWindowSizeVisible;
    [ObservableProperty]
    private bool _pbxLanguage1Visible;
    [ObservableProperty]
    private bool _pbxLanguage2Visible;

    [ObservableProperty]
    private string _notes;
    [ObservableProperty]
    private string _address;
    [ObservableProperty]
    private string _mandant;
    [ObservableProperty]
    private string _mandantPath;
    [ObservableProperty]
    private string _hdrOwner;
    [ObservableProperty]
    private string _owner;
    [ObservableProperty]
    private string _menue18;
    [ObservableProperty]
    private string _hdrProgName;
    [ObservableProperty]
    private string _hdrAdt;
    [ObservableProperty]
    private string _hdrCopyright;
    [ObservableProperty]
    private string _warningText;
    [ObservableProperty]
    private string _autoUpdState;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenMandantsCommand))]
    private bool _mandantsEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenPrintCommand))]
    private bool _printEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenImportExportCommand))]
    private bool _importExportEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenAddressCommand))]
    private bool _addressEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCalculationsCommand))]
    private bool _calculationsEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenFunctionKeysCommand))]
    private bool _functionKeysEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenConfigCommand))]
    private bool _configEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenReorgCommand))]
    private bool _reorgEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenBackupReadCommand))]
    private bool _backupReadEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenBackupWriteCommand))]
    private bool _backupWriteEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenSendDataCommand))]
    private bool _sendDataEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCheckUpdateCommand))]
    private bool _checkUpdateEnabled;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenRemoteDiagCommand))]
    private bool _remoteDiagEnabled;

    [ObservableProperty]
    private string _creationDate;
    [ObservableProperty]
    private int _trackBar1Maximum;
    [ObservableProperty]
    private int _trackBar1Value;
    [ObservableProperty]
    private string _frmWindowSizeText;
    [ObservableProperty]
    private DateTime _dateTimePicker1Value;
    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<ListItem<(EEventArt, int)>> _listBox2Items;
    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<ListItem<(bool, int)>> _lstList3Items;
    [ObservableProperty]
    private string _lstList3Text;
    [ObservableProperty]
    private string _dateLastCheckText;
    [ObservableProperty]
    private ListItem<(bool, int)> _lstList3SelectedItem;
    private bool Families_Enabled;
    private Color ImportExportBackColor;

    public Menu1ViewModel(IFraStatisticsViewModel fraStatisticsVm)
    {
        Statistics = fraStatisticsVm; 
    }

    public bool PbxLanguage3Visible { get; private set; }
    public bool RemoteDiagVisible { get; private set; }

    public bool DateTimePickerVisible { get; private set; }

    public Type AdresseType { get; set; }
    public bool DisableMsgChecked { get; private set; }

    [RelayCommand]
    private void FormClosing(FormClosingEventArgs e)
    {
        if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
        {
            Modul1.Persistence.WriteEnumInit("Windowstate", WindowState);
        }
        Modul1.Persistence.WriteEnumInit("state", Modul1.eWindowSize);
    }

    [RelayCommand]
    private void FormLoad()
    {
        //Discarded unreachable code: IL_1720, IL_3bc9, IL_3bee, IL_3dfa, IL_3eb9, IL_3f29, IL_4b73, IL_4c67, IL_4c8c, IL_4ed2
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int Value = default;
        string Value2 = default;
        string text3 = default;
        string text4 = default;
        int lErl = default;
        int Value3 = default;
        short num9 = default;
        Bitmap oBitmap = default;
        int Satz2 = default;
        var M1_Iter = 0;
        int num4 = 0;
        string documentText = "";
        Font font;
        //Field field;
        string DateiName2;
        string DateiName;
        int farb2 = 0;
        string text2 = "";

        DoFormLoad(out num, out num3, Value, out Value2, ref text3, ref text4, out lErl, ref Value3, num9, ref oBitmap, ref Satz2, ref M1_Iter, ref documentText, out DateiName2, out DateiName, ref farb2, ref text2);
        return;
    // =============== Begin der Fehlerbehandlung ===============
    IL_4c8e:
        num = 961;
        int number = Information.Err().Number;
        if (number == 62)
        {
            Modul1.Verz = Modul1.GenFreeDir + "\\Testdat1";
            //Todo
            //Modul1.GenFreeDir = FileSystem.CurDir().Left( 2) + Modul1.AppName;
            Modul1.Verz = Modul1.Persistence.ReadStringInit("gen-verz.ini");
            ProjectData.EndApp();
            ProjectData.ClearProjectError();
            if (num2 == 0)
            {
                throw ProjectData.CreateProjectError(-2146828268);
            }
//            goto IL_4efc;
        }
        else if (number == 53)
        {
            Modul1.Verz = Modul1.GenFreeDir + "\\Testdat1";
            Modul1.Persistence.WriteStringInit("gen-verz.ini", Modul1.Verz);
            ProjectData.EndApp();
            ProjectData.ClearProjectError();
            if (num2 == 0)
            {
                throw ProjectData.CreateProjectError(-2146828268);
            }
  //          goto IL_4efc;
        }
  
    }

    private void DoFormLoad(
        out int num,
        out int num3,
        int Value,
        out string
        Value2,
        ref string text3,
        ref string text4,
        out int lErl,
        ref int Value3,
        short num9,
        ref Bitmap oBitmap,
        ref int Satz2,
        ref int M1_Iter,
        ref string documentText,
        out string DateiName2,
        out string DateiName,
        ref int farb2,
        ref string text2)
    {
        checked
        {
            DateiName2 = "";
            DateiName = "";
            DataModul.DAODBEngine_definst = IoC.GetRequiredService<IDBEngine>();

            CPersonData.SetGetText(DataModul.TextLese1);
            CFamilyPersons.SetGetText(DataModul.TextLese1);
            CEventData.SetGetText(DataModul.TextLese1);

            string text = DateTime.Now.AsString();
            bool flag = false;
            Value2 = "";
            ProjectData.ClearProjectError();
            num3 = 2;
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                // Phone home 1
                flag = NetworkInterface.GetIsNetworkAvailable()
                    && (new Ping().Send(Modul1.AppHostName, 1500).Status == IPStatus.Success);
                if (!Modul1.System.xDemo)
                {                                // !!!
                    Value2 = File.ReadAllText(Modul1.GenFreeDir + "\\IDF.Dat");
                    string startupPath = Application.StartupPath;
                    //    Modul1.GenFreeDir = startupPath.Left( 3) + "Gen_Pluswin";
                    text3 = Strings.Mid(Value2, 20, 5);
                    text4 = Value2;
                }
                if (flag && !Modul1.System.xDemo)
                {
                    MainProject.Forms.Hinter.WebBrowser1.DocumentText = "";
                    //===============================
                    ProjectData.ClearProjectError();
                    num3 = 3;
                    Value2 = Modul1.Persistence.ReadStringMLProg("Adresse", 8).Replace(Environment.NewLine, " ").Trim();

                    lErl = 23;
                    if (Value2 == "")
                    {
                        Value2 = "XXXXXXXXX";
                    }
                    //====================================
                    ProjectData.ClearProjectError();
                    num3 = 4;
                    Modul1.Verz = Modul1.Persistence.ReadStringInit("gen-verz.ini");
                    Modul1.AutoupD = Modul1.Persistence.ReadStringInit("Update_ini.dat");
                    Value3 = Modul1.Persistence.ReadIntInit("GPRK");
                    text2 = "RKKKRRRR";
                    Value2 = Value2 + " !! " + Environment.UserName;

                    if (DateTime.Today.ToOADate() - Value3 > 0.0)
                    {
                        /// Phone home 2
                        MainProject.Forms.Hinter.WebBrowser1.DocumentText = "";
                        string urlString = $"https://{Modul1.AppHostName}/Up24/rkmelden.php?name=" + text2 + text3 + " " + Environment.MachineName + "   " + text4 + "  " + Modul1.Verz + "! " + Value2;
                        MainProject.Forms.Hinter.WebBrowser1.Navigate((string)default);
                        lErl = 10;
                        //=====================
                        Application.DoEvents();
                        documentText = MainProject.Forms.Hinter.WebBrowser1.DocumentText;
                        Value3 = (int)Math.Round(DateTime.Today.ToOADate());
                        Modul1.Persistence.WriteIntInit("GPRK", Value3);
                    }
                }
            }
            ProjectData.ClearProjectError();
            num3 = 6;
            FileSystem.FileClose(99);

            FileSystem.FileOpen(99, Modul1.TempPath + "\\GenPluswin.kml", OpenMode.Output);
            FileSystem.FileClose(99);

            ProjectData.ClearProjectError();
            num3 = 7;
            Modul1.cMandDrive = new DriveInfo(FileSystem.CurDir());
            if (Modul1.System_TestForm_Height() >= 89)
                // Todo: Besser machen.
                if (Interaction.MsgBox("In den Windows-Einstellungen für die Bildschirmanzeige ist die Schriftgröße nicht auf 'normale Schriftarten' eingestellt. Dies führt in einigen Bildschirmmasken zu Darstellungsproblemen. ", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Information) == DialogResult.Cancel)
                {
                    ProjectData.EndApp();
                }
            FileSystem.FileClose(99);

            if (Modul1.cMandDrive.DriveType == DriveType.CDRom)
            {
                string path = Modul1.cMandDrive.Name + Modul1.AppName;
                FileSystemInfo[] fileSystemInfos = new DirectoryInfo(path).GetFileSystemInfos();
                FileSystemInfo[] array = fileSystemInfos;
                int num6 = 0;
                while (num6 < array.Length)
                {
                    FileSystemInfo fileSystemInfo = array[num6];
                    string text5 = fileSystemInfo.Name.ToUpper();
                    switch (text5)
                    {
                        case "INIT":
                        case "TEMP":
                        case "LIST":
                        case "TEMPOSB":
                        case "HILFE":
                        case "INTERAHN":
                        case "TESTDAT1":
                            num6++;
                            break;
                        default:
                            Modul1.Verz = fileSystemInfo.FullName;
                            break;
                    }
                }
            }
            else
            {
                //!!
                Modul1.Verz = Modul1.Persistence.ReadStringInit("gen-verz.ini").Trim();
            }
            num = 120;
            Modul1.Verz1 = Modul1.Verz.Left(15);
            MandantPath = Modul1.Verz;
            FileSystem.FileClose(99);
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Append);
                FileSystem.FileClose(99);
            }
            FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Input);
            FileSystem.FileClose(99);

            num3 = 8;
            //Modul1.GenFreeDir = Modul1.cMandDrive.Name + "Gen_Pluswin";
            Lingua();
            lErl = 45;
            FrmWindowSizeVisible = true;
            if (Modul1.Typ != DriveType.CDRom
                    && Directory.Exists(Modul1.Verz)
                    && Modul1.Verz.Trim().Length >= 16)
            {
                Modul1.Persistence.ReadSuchDatMand("suchfeld.dat", Modul1.Suchfeld);
            }
            lErl = 47;
            var cls = Modul1.Persistence.ReadFarbenInit("Farb.dat", 3);
            Modul1.HintFarb = cls[1];
            Modul1.Feld1Farb = cls[2];
            Modul1.ErFarb = cls[3];

            TrackBar1Maximum = 10;
            if (!FrmWindowSizeVisible)
            {
                Modul1.FontSize = 0f;
            }

            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            int num7 = bounds.Width;
            int num8 = bounds.Height;
            if (num7 >= 800)
            {
                TrackBar1Maximum = 1;
                TrackBar1BackColor = Color.Red;
            }
            if (num7 >= 900)
            {
                TrackBar1Maximum = 3;
                TrackBar1BackColor = Color.Red;
            }
            if (num7 >= 1024)
            {
                TrackBar1Maximum = 5;
                TrackBar1BackColor = Modul1.HintFarb;
            }
            if (num7 >= 1280)
            {
                TrackBar1Maximum = 7;
                TrackBar1BackColor = Modul1.HintFarb;
            }
            if (num7 >= 1400)
            {
                TrackBar1Maximum = 8;
                TrackBar1BackColor = Modul1.HintFarb;
            }
            if (num7 > 1400)
            {
                TrackBar1Maximum = 9;
                TrackBar1BackColor = Modul1.HintFarb;
            }
            //    Modul1.GenFreeDir = Modul1.cMandDrive.Name + "Gen_Pluswin";
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                ProjectData.ClearProjectError();
                num3 = 9;
                FileSystem.FileOpen(6, Modul1.InitDir + "Windowstate", OpenMode.Append);
                FileSystem.FileClose(6);
            }
            //<====================
            ProjectData.ClearProjectError();
            num3 = 10;
            Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS); Modul1.eWindowState = WiS;
            if (Modul1.eWindowState == (Enum)FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Maximized;
                lstList3Width = 493;
            }
            else
            {
                WindowState = FormWindowState.Normal;
                lstList3Width = 306;
            }
            //============================
            ProjectData.ClearProjectError();
            num3 = 11;
            Modul1.eWindowSize = Modul1.Persistence.ReadEnumsInit<EWindowSize>("state")[0];
            System_Windowsize(num9, num8);
            Grossaend(Modul1.FontSize);
            FileSystem.FileClose(99);
            lErl = 46;
            RemoveWriteProtFromFiles(Modul1.GenFreeDir);
            num3 = 12;
            if (!Directory.Exists(Modul1.GenFreeDir + "\\Temp"))
                Directory.CreateDirectory(Modul1.GenFreeDir + "\\Temp");
            if (!Directory.Exists(Modul1.GenFreeDir + "\\list"))
                Directory.CreateDirectory(Modul1.GenFreeDir + "\\list");
            PictureBox pictureBox;

            Modul1.HintFarb = Color.FromArgb(0xC0C0C9);
            Modul1.ErFarb = Color.FromArgb(0xC0C0C9);
            Modul1.Feld1Farb = Color.White;
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                cls = Modul1.Persistence.ReadFarbenInit("Farb.dat", 3);
                Modul1.HintFarb = cls[1];
                Modul1.Feld1Farb = cls[2];
                Modul1.ErFarb = cls[3];

            }
            FrmWindowSizeBackColor = Modul1.HintFarb;
            BackColor = Modul1.HintFarb;
            OwnerBackColor = BackColor;
            MandantPathBackColor = BackColor;

            HdrProgName = Modul1.VersionT;
            HdrCopyright = Modul1.Version1;
            HdrAdt = Modul1.Version;

            if (Modul1.cMandDrive.DriveType == DriveType.CDRom)
            {
                HdrAdt = "Anwenderspezifische Sonderversion";
            }
            string text6;
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                //!!!
                DateiName2 = Modul1.GenFreeDir + "\\Testdat1\\Gen_plusdaten.mdb";
                if (!File.Exists(DateiName2))
                {
                    //!!!!
                    Modul1.Verz = Modul1.GenFreeDir + "\\Testdat1\\";
                    Directory.CreateDirectory(Modul1.Verz);
                    text6 = Modul1.InitDir + "Gen_plusdaten1.mdb";
                    string destination = Modul1.Verz + "Gen_plusdaten.mdb";
                    FileSystem.FileCopy(text6, destination);
                    FileSystem.Kill(Modul1.Verz + "Such.mdb");
                    FileSystem.Kill(Modul1.Verz + "Ort1.mdb");
                    FileSystem.Kill(Modul1.Verz + "Tempo.mdb");
                    text6 = Modul1.GenFreeDir + "\\Such.mdb";
                    destination = Modul1.Verz + "Such.mdb";
                    FileSystem.FileCopy(text6, destination);
                    text6 = Modul1.GenFreeDir + "\\Ort1.mdb";
                    destination = Modul1.Verz + "Ort1.mdb";
                    FileSystem.FileCopy(text6, destination);
                    text6 = Modul1.GenFreeDir + "\\Tempo.mdb";
                    destination = Modul1.Verz + "Tempo.mdb";
                    FileSystem.FileCopy(text6, destination);
                    FileSystem.FileClose();
                }
            }
            text6 = "";
            if (File.Exists(DateiName2 = Modul1.Verz + "Wappen.bmp")
                || File.Exists(DateiName2 = Modul1.Verz + "Wappen.TIF")
                || File.Exists(DateiName2 = Modul1.Verz + "Wappen.GIF")
                || File.Exists(DateiName2 = Modul1.Verz + "Wappen.PNG")
                || File.Exists(DateiName2 = Modul1.Verz + "Wappen.JPG")
                || File.Exists(DateiName2 = Modul1.Verz1 + "Wappen.bmp")
                || File.Exists(DateiName2 = Modul1.Verz1 + "Wappen.TIF")
                || File.Exists(DateiName2 = Modul1.Verz1 + "Wappen.PNG")
                || File.Exists(DateiName2 = Modul1.Verz1 + "Wappen.GIF")
                || File.Exists(DateiName2 = Modul1.Verz1 + "Wappen.JPG"))
            {
                text6 = DateiName2;
            }
            if (Modul1.cMandDrive.DriveType == DriveType.CDRom)
            {
                oBitmap = new Bitmap(text6);
                pictureBox = View.pbxCodeOfArms;
                pictureBox.Image = Modul1.AutoSizeImage(oBitmap, pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height);
            }
            else if (text6 != "")
            {
                FileStream fileStream = new FileStream(text6, FileMode.Open);
                try
                {
                    oBitmap = new Bitmap(fileStream);
                    pictureBox = View.pbxCodeOfArms;
                    pictureBox.Image = Modul1.AutoSizeImage(oBitmap, pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height);
                }
                finally
                {
                    fileStream?.Close();
                }
            }
            pictureBox = null;
            if (Modul1.cMandDrive.DriveType == DriveType.CDRom)
            {
                string path2 = Modul1.cMandDrive.Name + Modul1.AppName;
                FileSystemInfo[] fileSystemInfos2 = new DirectoryInfo(path2).GetFileSystemInfos();
                FileSystemInfo[] array3 = fileSystemInfos2;
                int num14 = 0;
                while (num14 < array3.Length)
                {
                    FileSystemInfo fileSystemInfo2 = array3[num14];
                    if (fileSystemInfo2 is DirectoryInfo)
                    {
                        string text7 = fileSystemInfo2.Name.ToUpper();
                        switch (text7)
                        {
                            case "INIT":
                            case "TEMP":
                            case "LIST":
                            case "TEMPOSB":
                            case "HILFE":
                            case "INTERAHN":
                            case "TESTDAT1":
                                break;
                            default:
                                Modul1.Verz = fileSystemInfo2.FullName;
                                num14 = array3.Length;
                                break;
                        }
                    }
                    num14++;
                }
            }
            else
            {
                Modul1.Verz = Modul1.Persistence.ReadStringInit("gen-verz.ini").Trim();
            }
            if (string.IsNullOrEmpty(Modul1.Verz))
            {
                Modul1.Verz = Modul1.GenFreeDir + "\\Testdat1";
            }
            DateiName2 = Modul1.Verz.Left(1) + ":\\";
            if (Modul1.Typ == DriveType.Network)
            {
                if (Modul1.Verz.Right(1) != "\\")
                {
                    Modul1.Verz += "\\";
                }
                if (!Module2.IsDriveReady(Modul1.Verz1.Left(1) + ":", Modul1.Verz + "Gen_plusdaten.mdb", bCheckWriteAccess: true))
                {
                    //CDRom ???
                }
            }
            Modul1.Dateienopen();
            if (DBHelper.DbFieldExists(DataModul.MandDB, dbTables.Ereignis, "Hausnr1"))
            {
                Event_DropField(EventFields.Hausnr1);
                Modul1.Dateienopen();
            }
            lErl = 3;
            Statistics.UpdateStat();
            MandantPath = Modul1.Verz;
            Show();
            if ((Modul1.Typ != DriveType.CDRom) & (Strings.InStr(Modul1.Mandant, "Testdat1") == 0))
            {
                if (Modul1.Reli)
                {
                    _ = Interaction.MsgBox("Die Religionseinträge werden jetzt bearbeitet. Dieser einmalig erforderliche Vorgang kann einige Minuten dauern.");
                    
                    int num17 = DataModul.Person.MaxID;
                    DataModul.DB_PersonTable.MoveFirst();
                    int num18 = DataModul.Person.MinID;
                    M1_Iter = 1;
                    while (M1_Iter <= num18)
                    {
                        WarningVisible = true;
                        WarningText = $"Person: {M1_Iter}";

                        DataModul.DB_PersonTable.Edit();
                        if (Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Konv].AsString()) != "")
                        {
                            var field2 = DataModul.DB_PersonTable.Fields[PersonFields.Konv];
                            DateiName = field2.Value.AsString();
                            Satz2 = DataModul.Texte_Schreib(DateiName, Modul1.UbgT1, ETextKennz.tk7_);
                            field2.Value = DateiName;
                            DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz2;
                            Satz2 = 0;
                        }
                        else
                        {
                            DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz2;
                            Satz2 = 0;
                        }
                        DataModul.DB_PersonTable.Update();
                        DataModul.DB_PersonTable.MoveNext();
                        M1_Iter++;
                    }
                    WarningVisible = false;
                    _ = Interaction.MsgBox("Fertig");
                    Modul1.Reli = false;
                }
                ProjectData.ClearProjectError();
                num3 = 13;
                DataModul.DB_TexteTable.Index = nameof(TexteIndex.SSTexte);
                DataModul.DB_TexteTable.MoveFirst();
                while (!DataModul.DB_TexteTable.EOF)
                {
                    if (Strings.InStr(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString(), "ß") != 0)
                    {
                        DataModul.DB_TexteTable.Edit();
                        DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value = Strings.Replace(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString(), "ß", "ssss");
                        DataModul.DB_TexteTable.Update();
                        if (DataModul.DSB_SearchTable.RecordCount > 0)
                        {
                            DataModul.DSB_SearchTable.MoveFirst();
                            DataModul.DSB_SearchTable.Delete();
                        }
                    }
                    DataModul.DB_TexteTable.MoveNext();
                }
                MandantPath = Modul1.Verz;
                if (DataModul.DB_PersonTable.RecordCount != DataModul.DSB_SearchTable.RecordCount)
                {
                    byte b = (byte)Interaction.MsgBox("Datenreorganisation ist erforderlich!\nJetzt reorganisieren?", title: "", mb: MessageBoxButtons.YesNo);
                    if (b != 7)
                    {
                        OpenReorg();
                    }
                }
            }
            ProjectData.ClearProjectError();
            num3 = 14;
            if (Modul1.Ubg == 999)
            {
                Debugger.Break();
                DateiName = "";
                return;
            }
            else
            {
                if (!Modul1.System.xDemo)
                {
                    EnterLizenzVisible = false;
                }
                ProjectData.ClearProjectError();
                num3 = 15;
                FileSystem.FileClose(99);
                Modul1.Lw = Modul1.Verz.Left(3);
                Modul1.Lw = Modul1.Verz.Left(3);
                Owner = Modul1.User.Owner;
                Modul1.Persistence.ReadStringsInit("Texte1.dat", 24, Modul1.TxT);
            }
            num3 = 16;
            Modul1.Persistence.ReadStringsInit("Druck_ini.dat", Modul1.Aus, true);
            num3 = 17;
            Aus_SetDefaults();
            num3 = 18;
            M1_Iter = 0;
            if (Modul1.Aus[(int)EOutCfg.o14] == "")
            {
                Modul1.Aus[(int)EOutCfg.o14] = 0.AsString();
            }
            M1_Iter = 0;
            Modul1.Persistence.ReadStringsInit("DruckTexte.dat", Modul1.DTxt, false);

            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                if (!File.Exists(Modul1.Aus[(int)EOutCfg.o07_KeepFormat]))
                {
                    _ = Interaction.MsgBox("Die eingestellte Textverarbeitung ist nicht vorhanden. Es wird versucht eine andere Textverarbeitung zu finden.");
                    Modul1.Aus[(int)EOutCfg.o07_KeepFormat] = "";
                }
                if (Modul1.Aus[(int)EOutCfg.o07_KeepFormat] == "")
                {
                    if ((Modul1.Aus[(int)EOutCfg.o07_KeepFormat] = SearchForOfficeWord()) == null)
                    {
                        _ = Interaction.MsgBox("Die eingestellte Textverarbeitung ist nicht vorhanden. Es wird versucht eine andere Textverarbeitung zu finden.");
                        Modul1.Aus[(int)EOutCfg.o07_KeepFormat] = "";
                    }
                }
            }
            else if (!File.Exists(Modul1.Aus[(int)EOutCfg.o07_KeepFormat]))
            {
                if (Modul1.Typ != DriveType.CDRom)
                {
                    _ = Interaction.MsgBox("Die eingestellte Textverarbeitung ist nicht vorhanden. Es wird versucht eine andere Textverarbeitung zu finden.");
                }
                Modul1.Aus[(int)EOutCfg.o07_KeepFormat] = "";
            }

            lErl = 22;
            Modul1.Aus[(int)EOutCfg.o27] = "";
            if (Modul1.Aus[(int)EOutCfg.o27] == "")
            {
                string text9 = Modul1.GoogleInstallPath();
                if (text9.Length > 0)
                {
                    text9 = text9.Replace("/", "\\");
                    Modul1.Aus[(int)EOutCfg.o27] = text9;
                }
            }
            FileSystem.FileClose(99);
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
                FileSystem.FileOpen(99, Modul1.InitDir + "Druck_ini.dat", OpenMode.Output);
                M1_Iter = 1;
                while (M1_Iter <= 50)
                {
                    FileSystem.PrintLine(99, Modul1.Aus[M1_Iter]);
                    M1_Iter++;
                }
            }
            FileSystem.FileClose(99);
            Prog_CheckActuality((i) =>
            {
                UpdateVisible = true;
                DateLastCheckText = Value > 0
                    ? DateTime.Today.ToOADate() - Value == 1.0
                        ? $"vor {DateTime.Today.ToOADate() - Value} Tag!"
                        : $"vor {DateTime.Today.ToOADate() - Value} Tagen!"
                    : "nicht erkennbar!";
            });
            if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
            {
            }
            else
            {
                MandantsEnabled = false;
                PrintEnabled = false;
                ImportExportEnabled = false;
                CalculationsEnabled = false;
                FunctionKeysEnabled = false;
                ConfigEnabled = false;
                ReorgEnabled = false;
                BackupReadEnabled = false;
                BackupWriteEnabled = false;
                SendDataEnabled = false;
                CheckUpdateVisible = false;
                RemoteDiagVisible = false;
            }
        }
    }

    private void Prog_CheckActuality(Action<int> actUpdate)
    {
        Modul1.AutoupD = Modul1.Persistence.ReadStringInit("Update_ini.dat");
        if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
        {
            var Value = Modul1.Persistence.ReadIntInit("GPRK");
            if (Modul1.AutoupD == "1")
            {
                AutoUpdState = "Automatische Aktualitäts- kontrolle ist 'AUS'\nEinschalten unter >Einstellungen<";
            }
            else
            {
                AutoUpdState = "Automatische Aktualitäts- kontrolle ist 'EIN'";
            }
            if (DateTime.Today.ToOADate() - Value > 0.0)
            {
                if (Modul1.AutoupD == "1")
                {
                    // DoUpdate
                    UpdateYes();
                }
                else if (DateTime.Today.ToOADate() - Value > 4.0
                    && Value < DateTime.Today.ToOADate())
                {
                    actUpdate?.Invoke(Value);
                }
            }

        }
    }

    private void System_Windowsize(short num9, int num8)
    {
        checked
        {
            var num_9 = num8.GetWindowSize();
            if (num9 < (short)Modul1.eWindowSize)
            {
                Modul1.eWindowSize = (EWindowSize)num9;
                TrackBar1Maximum = num9;
            }
            TrackBar1Value = (short)Modul1.eWindowSize;
            if (Modul1.eWindowSize == 0)
            {
                TrackBar1Value = num9;
                Modul1.eWindowSize = (EWindowSize)num9;
            }
            switch (Modul1.eWindowSize)
            {
                case EWindowSize.ws800x600:
                    Modul1.FontSize = 7.8f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 800 X 600 ";
                    break;
                case EWindowSize.ws900x670:
                    Modul1.FontSize = 8.7f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 900 X 670 ";
                    break;
                case EWindowSize.ws900x710:
                    Modul1.FontSize = 9.5f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 900 X 710 ";
                    break;
                case EWindowSize.ws1024x710:
                    Modul1.FontSize = 10.3f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1024 X 710 ";
                    break;
                case EWindowSize.ws1024x768:
                    Modul1.FontSize = 11f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1024 X 768 ";
                    break;
                case EWindowSize.ws1150x800:
                    Modul1.FontSize = 11.7f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1150 X 800 ";
                    break;
                case EWindowSize.ws1150x835:
                    Modul1.FontSize = 12.4f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1150 X 835 ";
                    break;
                case EWindowSize.ws1280x920:
                    Modul1.FontSize = 13.2f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1280 X 920 ";
                    break;
                case EWindowSize.ws1400x1050:
                    Modul1.FontSize = 14.9f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1400 X 1050 ";
                    break;
                case EWindowSize.ws1600x1200:
                    Modul1.FontSize = 16.5f;
                    FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1600 X 1200 ";
                    break;
                default:
                    break;
            }
        }
    }

    private void Aus_SetDefaults()
    {
        if (Modul1.Aus[(int)EOutCfg.o08].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o08] = "20";
        }
        if (Modul1.Aus[(int)EOutCfg.o09].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o09] = "40";
        }
        if (Modul1.Aus[(int)EOutCfg.o10_EmitIDs].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o10_EmitIDs] = "18";
        }
        if (Modul1.Aus[(int)EOutCfg.o11].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o11] = "50";
        }
        if (Modul1.Aus[(int)EOutCfg.o12].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o12] = "400";
        }
        if (Modul1.Aus[(int)EOutCfg.o13].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o13] = "400";
        }
        if (Modul1.Aus[(int)EOutCfg.o15].AsInt() == 0.0)
        {
            Modul1.Aus[(int)EOutCfg.o15] = "1800";
        }
        if (Modul1.Aus[(int)EOutCfg.o24] != "1")
        {
            Modul1.Aus[(int)EOutCfg.o24] = "0";
        }
        if (Modul1.Aus[(int)EOutCfg.o26] == "")
        {
            Modul1.Aus[(int)EOutCfg.o26] = true.AsString();
        }
    }

    private static void Event_DropField(EventFields eDropField)
    {
        IRecordset dB_EventTable = DataModul.DB_EventTable;
        dB_EventTable.Index = nameof(EventIndex.HaNu);
        dB_EventTable.Seek(">", "");
        int num15 = dB_EventTable.RecordCount - 1;
        var M1_Iter = 1;
        while (M1_Iter <= num15
            && !dB_EventTable.EOF
            && !dB_EventTable.NoMatch
            && "" != dB_EventTable.Fields[$"{eDropField}"].AsString().Trim())
        {
            var Event_sHausNr1 = dB_EventTable.Fields[$"{eDropField}"].AsString();
            int Satz = DataModul.Texte_Schreib(Event_sHausNr1, "", ETextKennz.tk5_);

            dB_EventTable.Edit();
            dB_EventTable.Fields[EventFields.Hausnr].Value = Satz;
            dB_EventTable.Fields[$"{eDropField}"].Value = "";
            dB_EventTable.Update();
            dB_EventTable.MoveNext();
            M1_Iter++;
        }
        dB_EventTable.Close();
        DataModul.MandDB.TryExecute($"Drop INDEX {EventIndex.HaNu} on {dbTables.Ereignis}");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} DROP COLUMN {eDropField};");
    }

    private string? SearchForOfficeWord()
    {
        string? DateiName = null;
        const string cWinWordExe = "Winword.exe";
        const string WordPad0 = "wordpad.exe";
        const string WordPad1 = "zubehör";
        const string Wordpad2 = "Windows NT\\zubehör";
        const string WordPad3 = "Windows NT\\Accessories";
        if (Modul1.OfficeAppInstalled(IModul1.MSOfficeComponent.MSWord))
        {
            string text8;
            if ((text8 = Modul1.OfficeInstallPath(IModul1.MSOfficeVersion.Office2000)).Length > 0
                || (text8 = Modul1.OfficeInstallPath(IModul1.MSOfficeVersion.Office2007)).Length > 0
                || (text8 = Modul1.OfficeInstallPath(IModul1.MSOfficeVersion.Office2003)).Length > 0
                || (text8 = Modul1.OfficeInstallPath(IModul1.MSOfficeVersion.Office95)).Length > 0
                || (text8 = Modul1.OfficeInstallPath(IModul1.MSOfficeVersion.Office97)).Length > 0
                || (text8 = Modul1.OfficeInstallPath(IModul1.MSOfficeVersion.OfficeXP)).Length > 0)
            {
                return Path.Combine(text8, cWinWordExe);
            }
            return null;
        }
        else if (File.Exists(DateiName = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), WordPad1, WordPad0))
            || File.Exists(DateiName = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), Wordpad2, WordPad0))
            || File.Exists(DateiName = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), WordPad3, WordPad0)))
        {
            return DateiName;
        }
        else
            return null;

    }

    private void RemoveWriteProtFromFiles(string genFreeDir)
    {
        var array2 = Modul1.F_GetAllFiles(genFreeDir, 1);
        foreach (var sFile in array2)
            try
            {
                if (!Modul1.RemoveWriteProtection(sFile))
                {
                    //Todo: MessageBox.Show("Fehler beim Entfernen des Schreibschutzes von " + sFile);
                }
            }
            catch
            { }
        ProjectData.ClearProjectError();
    }

    //private void _Button1_Click(object sender, EventArgs e)
    //{
    //    //Discarded unreachable code: IL_1c43, IL_1d36, IL_1f18
    //    int try0001_dispatch = -1;
    //    int num = default;
    //    int num2 = default;
    //    int num3 = default;
    //    int lErl = default;
    //    while (true)
    //    {
    //        try
    //        {
    //            /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
    //            ;

    //            int num4;
    //            switch (try0001_dispatch)
    //            {
    //                default:
    //                    num = 1;
    //                    FileSystem.FileClose(99);
    //                    try0001_dispatch = 9890;
    //                    break;

    //                case 9890:
    //                    {
    //                        num2 = num;
    //                        switch ((num3 <= -2) ? 1 : num3)
    //                        {
    //                            case 5:
    //                                break;
    //                            case 2:
    //                            case 4:
    //                                goto IL_1d38;
    //                            case 3:
    //                                goto IL_L387;
    //                            case 1:
    //                                goto IL_205c;
    //                            default:
    //                                goto end_IL_0001;
    //                        }
    //                        goto IL_L351;
    //                    }

    //                IL_2058:
    //                    num4 = num2;
    //                    goto IL_2060;

    //                IL_2060:
    //                    num2 = 0;
    //                    switch (num4)
    //                    {
    //                        case 1:
    //                            break;
    //                        case 2:
    //                            goto IL_L002;
    //                        case 351:
    //                            goto IL_L351;
    //                        case 386:
    //                        case 387:
    //                            goto IL_L387;
    //                        default:
    //                            goto end_IL_0001;
    //                        case 29:
    //                        case 33:
    //                        case 51:
    //                        case 57:
    //                        case 58:
    //                        case 79:
    //                        case 80:
    //                        case 85:
    //                        case 204:
    //                        case 207:
    //                        case 214:
    //                        case 217:
    //                        case 218:
    //                        case 225:
    //                        case 229:
    //                        case 230:
    //                        case 233:
    //                        case 235:
    //                        case 236:
    //                        case 255:
    //                        case 259:
    //                        case 283:
    //                        case 296:
    //                        case 306:
    //                        case 313:
    //                        case 326:
    //                        case 336:
    //                        case 337:
    //                        case 340:
    //                        case 343:
    //                        case 346:
    //                        case 349:
    //                        case 350:
    //                        case 397:
    //                            goto end_IL_0001_2;
    //                    }
    //                    goto default;

    //                //=======================================================
    //                IL_L002:
    //                    num = 2;
    //                    HandleWindowstate();
    //                    if (CheckTestDat(sender))
    //                        goto end_IL_0001_2;
    //                    switch (((Button)sender).Name)
    //                    {
    //                        case nameof(btnFamilies):
    //                            OpenFamilies();
    //                            break;
    //                        case nameof(btnSources):
    //                            OpenSources();
    //                            break;
    //                        case nameof(btnPersons):
    //                            OpenPersons();
    //                            break;
    //                        case nameof(btnPlaces):
    //                            OpenPlaces();
    //                            break;
    //                        case nameof(btnMandants):
    //                            OpenMandants();
    //                            break;
    //                        case nameof(btnManageTexts):
    //                            OpenTexts(); 
    //                            break;
    //                        case nameof(btnPrint):
    //                            OpenPrint();
    //                            break;
    //                        case nameof(btnImportExport):
    //                            OpenImportExport();
    //                            break;
    //                        case nameof(btnAddress):
    //                            OpenAddress();
    //                            break;
    //                        case nameof(btnEndProgram):
    //                            EndProgram();
    //                            break;
    //                        case nameof(btnCalculations):
    //                            OpenCalculations();
    //                            break;
    //                        case nameof(btnFunctionKeys):
    //                            OpenFunctionKeys();
    //                            break;
    //                        case nameof(btnConfig):
    //                            OpenConfig();
    //                            break;
    //                        case nameof(btnCheckFamilies):
    //                            OpenCheckFamilies();
    //                            break;
    //                        case nameof(btnCheckMissing):
    //                            OpenCheckMissing();
    //                            break;
    //                        case nameof(btnCheckPersons):
    //                            OpenCheckPersons();
    //                            break;
    //                        case nameof(btnDuplettes):
    //                            OpenDuplettes();
    //                            break;
    //                        case nameof(btnNotes):
    //                            OpenNotes();
    //                            break;
    //                        case nameof(btnCardMode):
    //                            Button19_Click();
    //                            break;
    //                        case nameof(btnEnterLizenz):
    //                            OpenEnterLizenz();
    //                            break;
    //                        default:
    //                            Debugger.Break();
    //                            break;
    //                    }
    //                    goto end_IL_0001_2;


    //                IL_L351:
    //                    num = 351;
    //                    if (Information.Err().Number == 5)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                        else
    //                            goto IL_205c;
    //                    }
    //                    var Mldg =  "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
    //                    if (Interaction.MsgBox(Mldg,  "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
    //                    {
    //                        ProjectData.EndApp();
    //                    }
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_2058;

    //                IL_1d38:
    //                    num = 361;
    //                    if (Information.Err().Number == 5)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                    }
    //                    else if (Information.Err().Number == 53)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                    }
    //                    else if (Information.Err().Number == 55)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                    }
    //                    else if (Information.Err().Number == 91)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                    }
    //                    else if (Information.Err().Number == 3420)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        var Mldg =  "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
    //                        if (Interaction.MsgBox(Mldg,  "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
    //                        {
    //                            ProjectData.EndApp();
    //                        }
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                        goto IL_2058;
    //                    }

    //                    goto IL_205c;

    //                IL_205c:
    //                    num4 = num2 + 1;
    //                    goto IL_2060;

    //                IL_L387:
    //                    num = 387;
    //                    if (Information.Err().Number == 3021)
    //                    {
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                        num2 = 0;
    //                        lErl = 3;
    //                        WarningVisible = false;
    //                        _ = Interaction.MsgBox("Fertig");
    //                        Modul1.Reli = false;
    //                        MandantPath = Modul1.MandDir;
    //                        if (DataModul.DB_PersonTable.RecordCount != DataModul.DSB_SearchTable.RecordCount)
    //                        {
    //                            byte b = checked((byte)Interaction.MsgBox("Datenreorganisation ist erforderlich!\nJetzt reorganisieren?", "", MessageBoxButtons.YesNo));
    //                            if (b != 7)
    //                            {
    //                                btnReorg.PerformClick();
    //                            }
    //                        }
    //                        Dates = DataModul.Event.Count;
    //                        frmStatictics1.Texts = DataModul.DB_TexteTable.RecordCount;
    //                        FileSystem.FileClose(99);
    //                        //====================================
    //                    }
    //                    var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
    //                    if (Interaction.MsgBox(Mldg, "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
    //                    {
    //                        ProjectData.EndApp();
    //                    }
    //                    DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value = 
    //                            DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString()+ ".";
    //                    ProjectData.ClearProjectError();
    //                    if (num2 == 0)
    //                    {
    //                        throw ProjectData.CreateProjectError(-2146828268);
    //                    }
    //                    goto IL_2058;


    //                end_IL_0001:
    //                    break;
    //            }
    //        }
    //        catch (Exception obj) when (obj is Exception exception && num3 != 0 && num2 == 0)
    //        {
    //            ProjectData.SetProjectError(exception, lErl);
    //            try0001_dispatch = 9890;
    //            continue;
    //        }
    //        throw ProjectData.CreateProjectError(-2146828237);
    //    end_IL_0001_2:
    //        break;
    //    }
    //    if (num2 != 0)
    //    {
    //        ProjectData.ClearProjectError();
    //    }
    //}

    [RelayCommand]
    private void OpenTexts()
    {
        _ = MainProject.Forms.Textlesen.ShowDialog();
    }

    [RelayCommand]
    private void OpenPersons()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Hide();
        if (Modul1.Typ != DriveType.CDRom)
        {
            int Value = Modul1.Persistence.GetIntMand("Letzter.dat", 1L);
            Modul1.Letzte = new() { iPerson = (Value != 0) ? Value : Modul1.Letzte.iPerson, iFamily= Modul1.Letzte.iFamily };
        }
        if (Modul1.PersInArb == 0)
        {
            Modul1.PersInArb = 1;
        }
        Modul1.PersInArb = Modul1.Letzte.iPerson;
        Personen.Default.WindowState = WindowState;
        Personen.Default.Show(Modul1.PersInArb, EUserText.t158);
    }

    private bool CheckTestDat(bool xSenderIsMandantvEndPrg)
    {
        var flag = false;
        if (Modul1.Verz.ToUpper().Contains("TESTDAT1"))
        {
            if (!xSenderIsMandantvEndPrg)
            {
                _ = Interaction.MsgBox("Der Mandant ''Testdat1'' ist nur als Rückfallebene gedacht, Eingaben sind hier nicht möglich.\n\nBitte wählen Sie einen anderen Mandanten, oder legen Sie einen neuen Mandanten an.");
                flag = true;
            }
        }

        return flag;
    }

    [RelayCommand]
    private void OpenPlaces()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Modul1.Ubg = 1;
        Modul1.Schalt = 1;

        MainProject.Forms.Ortsver.Show();
    }

    [RelayCommand]
    private void OpenSources()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Modul1.Ubg = 0;
        MainProject.Forms.Quellverw.Close();
        MainProject.Forms.Quellverw.Show();
        Hide();
    }

    [RelayCommand]
    private void OpenFamilies()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        int Value;
        Hide();
        if (Modul1.Typ != DriveType.CDRom)
        {
            Value = Modul1.Persistence.GetIntMand("Letzter.dat", 2L);
            Modul1.Letzte = new() { iPerson = Modul1.Letzte.iPerson, iFamily = (Value != 0) ? Value : default };
            Modul1.FamInArb = Modul1.Letzte.iFamily;
        }
        Familie.Default.Family_ShowFamilyDlg(Modul1.FamInArb);
    }

    [RelayCommand]
    private void OpenMandants()
    {
        HandleWindowstate();
        CreationDateVisible = false;
        MarkedVisible = false;
        NotesVisible = false;
        ListBox2Visible = false;
        List3Visible = false;
        Notes = "";
        CodeOfArmsVisible = true;
        Main_CloseAllForms();

        if (Modul1.Typ != DriveType.CDRom)
        {
            Modul1.Persistence.ReadSuchDatMand("suchfeld.dat", Modul1.Suchfeld);
        }
        Modul1.Dateienopen();
        Statistics.UpdateStat();
        MandantPath = Modul1.Verz;
        if (Modul1.Typ != DriveType.CDRom)
        {
            Mand_CheckReorg();
        }
        Statistics.SetDates( DataModul.Event.Count);
        Statistics.SetTexts(DataModul.DB_TexteTable.RecordCount);
        FileSystem.FileClose(99);
    }

    private void Mand_CheckReorg()
    {
        ProjectData.ClearProjectError();
        Texte_SearchTab_Check();
        if (Modul1.Reli)
        {
            _ = Interaction.MsgBox("Die Religionseinträge werden jetzt bearbeitet. Dieser einmalig erforderliche Vorgang kann einige Minuten dauern.");
            Person_UpdateReliEntry();
            _ = Interaction.MsgBox("Fertig");
            Modul1.Reli = false;
        }
        if (DataModul.DB_PersonTable.RecordCount != DataModul.DSB_SearchTable.RecordCount)
        {
            var mbr = Interaction.MsgBox("Datenreorganisation ist erforderlich!\nJetzt reorganisieren?", title: "", mb: MessageBoxButtons.YesNo);
            if (mbr != DialogResult.No)
            {
                // Todo: direct call to btnReorg_Click
                PerformReorg();
            }
        }
    }

    private void PerformReorg()
    {
        throw new NotImplementedException();
    }

    private void Person_UpdateReliEntry()
    {
        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DataModul.DB_PersonTable.MoveLast();
        int Person_iPersNr() => DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
        int iMaxPerson = Person_iPersNr();
        DataModul.DB_PersonTable.MoveFirst();
        while (!DataModul.DB_PersonTable.EOF
            && Person_iPersNr() <= iMaxPerson)
        {
            WarningVisible = true;
            WarningText = "Person: " + Person_iPersNr().AsString();
            if (DataModul.DB_PersonTable.NoMatch)
            {
                Debugger.Break();
            }
            DataModul.DB_PersonTable.Edit();
            int Satz = 0;
            string Person_sKonv = DataModul.DB_PersonTable.Fields[PersonFields.Konv].AsString();
            if (Person_sKonv.Trim() != "")
            {
                var field = DataModul.DB_PersonTable.Fields[PersonFields.Konv];
                string DateiName2 = field.AsString();
                Satz = DataModul.Texte_Schreib(DateiName2, Modul1.UbgT1, ETextKennz.tk7_);
                field.Value = DateiName2;
                DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz;
            }
            else
            {
                DataModul.DB_PersonTable.Fields[PersonFields.religi].Value = Satz;
            }
            DataModul.DB_PersonTable.Update();
            DataModul.DB_PersonTable.MoveNext();

        }
        WarningVisible = false;
    }

    private static void Texte_SearchTab_Check()
    {
        DataModul.DB_TexteTable.MoveFirst();
        while (!DataModul.DB_TexteTable.EOF)
        {
            if (Strings.InStr(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString(), "ß") != 0)
            {
                DataModul.DB_TexteTable.Edit();
                DataModul.DB_TexteTable.Fields[TexteFields.Txt].Value = Strings.Replace(DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString(), "ß", "ssss");
                DataModul.DB_TexteTable.Update();
                if (DataModul.DSB_SearchTable.RecordCount > 0)
                {
                    DataModul.DSB_SearchTable.MoveFirst();
                    DataModul.DSB_SearchTable.Delete();
                }
            }
            DataModul.DB_TexteTable.MoveNext();
        }
    }

    private static void Main_CloseAllForms()
    {
        Personen.Default.Close();
        Familie.Default.Close();
        MainProject.Forms.Quellverw.Close();
        MainProject.Forms.Ortsver.Close();
        MainProject.Forms.Textlesen.Close();
        MainProject.Forms.RechText.Close();
        MainProject.Forms.Namensuch.Close();
        MainProject.Forms.Regsuch.Close();
        MainProject.Forms.Partnerrecherche.Close();
        MainProject.Forms.Mand.Close();
        MainProject.Forms.Dub.Close();
        _ = MainProject.Forms.Mand.ShowDialog();
        MainProject.Forms.Mand.Close();
    }

    [RelayCommand(CanExecute = nameof(PrintEnabled))]
    private void OpenPrint()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        const string modul = "Druck.exe";
        string Kennz = Modul1.GenFreeDir + $"\\{modul}";
        if (!File.Exists(Kennz))
        {
            string text = $"Die Datei **{modul}** fehlt. vermutlich wurde Sie von Ihrem falsch eingestellem Virenprogramm gelöscht. \n";
            text += "Bitte kopieren Sie die Datei von der Installations-CD und weisen Sie Ihr Virenprogramm an, Dateien nicht zu löschen, nur weil sie neu sind\n";
            text += "\nSehen Sie hierzu in der Anleitung Ihres Virenprogramms nach, da ich nicht die Bedienung aller Virenprogramme kennen kann.";
            _ = Interaction.MsgBox(text);
        }
        else
        {
            _ = Interaction.Shell(Modul1.GenFreeDir + $"\\{modul}");
            ProjectData.EndApp();
        }
    }

    [RelayCommand(CanExecute = nameof(ImportExportEnabled))]
    private void OpenImportExport()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        const string modul = "Gedles.exe";
        string Kennz = Modul1.GenFreeDir + $"\\{modul}";
        if (!File.Exists(Kennz))
        {
            string text = $"Die Datei **{modul}** fehlt. vermutlich wurde Sie von Ihrem falsch eingestellem Virenprogramm gelöscht. \n";
            text += "Bitte kopieren Sie die Datei von der Installations-CD und weisen Sie Ihr Virenprogramm an, Dateien nicht zu löschen, nur weil sie neu sind\n";
            text += "\nSehen Sie hierzu in der Anleitung Ihres Virenprogramms nach, da ich nicht die Bedienung aller Virenprogramme kennen kann.";
            _ = Interaction.MsgBox(text);
        }
        else
        {
            _ = Interaction.Shell(Modul1.GenFreeDir + $"\\{modul}");
            Application.Exit();
            ProjectData.EndApp();
        }
    }

    [RelayCommand(CanExecute = nameof(AddressEnabled))]
    private void OpenAddress()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        _ = MainProject.Forms.Adresse.ShowDialog();
    }

    [RelayCommand()]
    private void EndProgram()
    {
        HandleWindowstate();
        ProjectData.EndApp();
    }

    [RelayCommand()]
    private void OpenDuplettes()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        _ = MainProject.Forms.Dub.ShowDialog();
    }

    private void HandleWindowstate()
    {
        if (Modul1.cMandDrive?.DriveType != DriveType.CDRom)
        {
            Modul1.Persistence.WriteIntsProg("maspos.dat", new[] { View.Left, View.Top });
            Modul1.Persistence.WriteEnumInit("Windowstate", WindowState);
            Modul1.Persistence.WriteIntInit("state", TrackBar1Value);
        }
    }

    [RelayCommand()]
    private void OpenNotes()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        _ = MainProject.Forms.Bemsuch.ShowDialog();
    }

    [RelayCommand()]
    private void Button19_Click()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Debugger.Break();
    }

    [RelayCommand()]
    private void OpenEnterLizenz()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        _ = MainProject.Forms.Lizenz.ShowDialog();
    }

    [RelayCommand()]
    private void OpenCalculations()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        string destination = Path.Combine(Modul1.TempPath, "NumTemp.mdb");
        string source = Path.Combine(Modul1.InitDir, "NUMTEMP.mdb");

        Hide();
        DataModul.ReplaceNBDatafile(destination, source, () => MainProject.Forms.Ahnen.ShowDialog());
        MainProject.Forms.Ahnen.Close();
        Show();
    }

    private void Show()
    {
        if (!View.Visible)
            View.Visible = true; 
    }

    private void Hide()
    {
        View.Hide();
    }

    [RelayCommand()]
    private void OpenFunctionKeys()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Hide();
        _ = MainProject.Forms.FunkT.ShowDialog();
    }

    [RelayCommand]
    private void OpenCheckPersons()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Hide();
        MainProject.Forms.Hinter.Show0(" Logische Personenprüfung");
        Modul1.Dateienopen();
        if (DataModul.DB_PersonTable.RecordCount == 0)
        {
            _ = Interaction.MsgBox("Keine Personen vorhanden");
            Show();
        }
        else
        {
            DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
            DataModul.DB_PersonTable.MoveLast();
            MainProject.Forms.Hinter.Show1(0);
        }
    }
    [RelayCommand]
    private void OpenCheckMissing()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Hide();
        MainProject.Forms.Fehlerli.Close();
        _ = MainProject.Forms.Fehlerli.ShowDialog();
        Show();
    }

    [RelayCommand]
    private void OpenCheckFamilies()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        Hide();
        MainProject.Forms.Hinter.Show0(" Logische Familienprüfung");
        Modul1.Dateienopen();
        if (DataModul.DB_FamilyTable.RecordCount == 0)
        {
            _ = Interaction.MsgBox("Keine Familien vorhanden");
            Show();
        }
        else
        {
            DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
            DataModul.DB_FamilyTable.MoveLast();
            MainProject.Forms.Hinter.Show1(1);
        }
    }

    [RelayCommand]
    private void OpenConfig()
    {
        HandleWindowstate();
        if (CheckTestDat(false))
            return;
        MainProject.Forms.Hinter.reorga = false;
        MainProject.Forms.Hinter.Frame4.Top = 65;
        MainProject.Forms.Hinter.Frame4.Left = 0;
        MainProject.Forms.Hinter.Frame4.Visible = true;
        _ = MainProject.Forms.Hinter.ShowDialog();
        if (MainProject.Forms.Hinter.reorga)
        {
            if (Modul1.Typ != DriveType.CDRom)
            {
                Modul1.Persistence.PutEnumsMand("suchfeld.dat", Modul1.Suchfeld);
                OpenReorg();
            }
        }
        int farb = Modul1.Persistence.GetIntInit("Farb.dat", 1L);
        Modul1.Farb = Color.FromArgb(farb);
        Modul1.HintFarb = Modul1.Farb;
        BackColor = Modul1.HintFarb;
    }

    private void _Timer1_Tick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0281
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        short num5 = default;
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
                    case 1130:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_039c;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number == 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_039c;
                            }
                            else
                            {
                                if (Information.Err().Number == 91)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_039c;
                                }
                                else
                                {
                                    var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                    if (Interaction.MsgBox(Mldg, "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_03a0;
                                }
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        if (!View.Visible)
                        {
                            goto end_IL_0001_2;
                        }
                        Menue18 = DateTime.Today.AsString();
                        num5++;
                        if (num5 >= 10)
                        {
                            num5 = 0;
                            if (Owner == "Adresse eingeben")
                            {
                                if (OwnerBackColor == Color.Red)
                                {
                                    OwnerBackColor = Color.Blue;
                                    Address = "Adresse";
                                    goto end_IL_0001_2;
                                }
                                else
                                {
                                    if (OwnerBackColor == Color.Blue)
                                    {
                                        OwnerBackColor = Color.Red;
                                        Address = "eingeben";
                                    }
                                }
                            }
                        }
                        Statistics.UpdateStat();

                        goto end_IL_0001_2;
                    IL_039c: // <========== 3
                        num4 = num2 + 1;
                        goto IL_03a0;
                    IL_03a0:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 12:
                            case 32:
                            case 33:
                            case 47:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is Exception exception && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(exception);
                try0001_dispatch = 1130;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    [RelayCommand]
    private void OpenReorg()
    {
        //Discarded unreachable code: IL_2331, IL_45c7, IL_48ea, IL_49e1
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        int num18 = default;
        int num19 = default;
        float Value = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    string DateiName;
                    string text3;
                    switch (try0001_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0009;
                        case 22335:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 3:
                                    case 5:
                                    case 6:
                                        break;
                                    case 2:
                                        goto IL_48ec;
                                    case 4:
                                        goto IL_49e3;
                                    case 1:
                                        goto IL_4ae1;
                                    default:
                                        goto end_IL_0001;
                                }
                                switch (Information.Err().Number)
                                {
                                    case 53:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_4ae1;
                                    case 55:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        num2 = 0;
                                        goto IL_1c85;
                                    case 3022:
                                        DataModul.DOSB_OrtSTable.Fields["Name"].Value =
                                            "&" + DataModul.DOSB_OrtSTable.Fields["Name"].AsString();
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        break;
                                    case 3167:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        num2 = 0;
                                        goto IL_1475;
                                    case 3250:
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_4ae1;
                                    case 3420:
                                        var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                        if (Interaction.MsgBox(Mldg, "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                                        {
                                            ProjectData.EndApp();
                                        }
                                        Modul1.Dateienopen();
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        break;
                                    default:
                                        var Mldg2 = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                        if (Interaction.MsgBox(Mldg2, title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                        {
                                            ProjectData.EndApp();
                                        }
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        break;
                                }
                                goto IL_4add;
                            }
                        end_IL_0001:
                            break;
                        IL_0009:
                            num = 2;
                            bool value;
                            int num8;
                            string text;
                            if (Modul1.Typ == DriveType.CDRom)
                            {
                                _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
                                goto end_IL_0001_2;
                            }
                            Value = (float)Interaction.MsgBox("Die Datei wird jetzt komprimiert, dieser Vorgang kann, je nach Grösse der Datei, einige Zeit dauern. Warten Sie ab, schalten Sie den Computer nicht aus!!!!! Anschliessend werden die Suchindizes neu aufgebaut.", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                            if (Value == 2f)
                            {
                                goto end_IL_0001_2;
                            }
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            value = false;
                            Menue_ButtonsSetEnable(value);
                            Application.DoEvents();
                            lErl = 12;
                            num8 = 1;
                            while (num8 == 1)
                            {
                                DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                if (DataModul.DB_FamilyTable.RecordCount > 0)
                                {
                                    DataModul.DB_FamilyTable.MoveLast();
                                    int iMaxFamNr = (int)Math.Round(Conversion.Val(DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt()));
                                    num8 = 0;
                                    byte b = 5;
                                    while (b <= 7)
                                    {
                                        int num10;
                                        switch (b)
                                        {
                                            case 5:
                                                int num15 = iMaxFamNr;
                                                num10 = 1;
                                                while (num10 <= num15)
                                                {
                                                    DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.Fam);
                                                    DataModul.DB_FamilyTable.Seek("=", num10);
                                                    if (!DataModul.DB_FamilyTable.NoMatch)
                                                    {
                                                        string Family_sBem2 = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem2].AsString();
                                                        if ("" != Family_sBem2)
                                                        {
                                                            text = Family_sBem2.Replace("\n", "");
                                                            if (text.Trim() != "")
                                                            {
                                                                num8 = 1;
                                                                text = Strings.Trim(Family_sBem2.Replace("\n", ""));
                                                                text = text.Replace("\r", "").Trim();
                                                                DataModul.Event.SetValAppend((EEventArt.eA_Marriage, num10, 0), EventFields.Bem4, Family_sBem2);

                                                                DataModul.DB_FamilyTable.Edit();
                                                                Family_sBem2 = "";
                                                                DataModul.DB_FamilyTable.Update();
                                                            }
                                                        }
                                                    }
                                                    var link = DataModul.Link.ReadAllFams(num10, ELinkKennz.lkMarrWitness).FirstOrDefault();
                                                    if (link != null)
                                                    {
                                                        num8 = 1;
                                                        DataModul.Witness.Add(link.iFamNr, link.iPersNr, EEventArt.eA_Marriage, 0, 10);
                                                        link.Delete();
                                                    }
                                                    lErl = 11;
                                                    num10++;
                                                }
                                                break;
                                            case 6:
                                                int num13 = iMaxFamNr;
                                                num10 = 1;
                                                while (num10 <= num13)
                                                {
                                                    var link = DataModul.Link.ReadAllFams(num10, ELinkKennz.lkWitnOfEngage).FirstOrDefault();
                                                    if (link != null)
                                                        num8 = 1;
                                                    DataModul.Witness.Add(link.iFamNr, link.iPersNr, EEventArt.eA_501, 0, 10);
                                                    link.Delete();
                                                    lErl = 21;
                                                    num10++;
                                                }
                                                break;
                                            case 7:
                                                int num9 = iMaxFamNr;
                                                num10 = 1;
                                                while (num10 <= num9)
                                                {
                                                    var link = DataModul.Link.ReadAllFams(num10, ELinkKennz.lkWitnOfMarr).FirstOrDefault();
                                                    if (link != null)
                                                    {
                                                        num8 = 1;
                                                        DataModul.Witness.Add(link.iFamNr, link.iPersNr, EEventArt.eA_MarrReligious, 0, 10);
                                                        link.Delete();
                                                    }
                                                    lErl = 31;
                                                    num10++;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        if (num8 == 1)
                                        {
                                            break;
                                        }
                                        b++;
                                    }
                                }
                                else
                                    break;
                            }
                            if (DataModul.Link.Count > 0)
                            {
                                foreach (var link in DataModul.Link.ReadAllKennzs(ELinkKennz.lkGodparent))
                                {
                                    if (DataModul.Person.Exists(link.iPersNr))
                                    {
                                        link.Delete();
                                    }
                                }
                            }
                            goto IL_1475;
                        //============

                        IL_1475: // <========== 3
                                 // <========== 3
                            num = 235;
                            SourceLink_Check();
                            lErl = 41;
                            if (Value == 2f)
                            {
                                goto end_IL_0001_2;
                            }
                            if ((DataModul.Person.Count > 0)
                                | (DataModul.Family.Count < 0))
                            {
                                DataModul.Person_Leertest();
                                if (DataModul.Link.Count > 0)
                                {
                                    if (DataModul.Family.Count > 0)
                                    {
                                        Link_DeleteAllEmptyFam();
                                    }

                                    if (DataModul.Person.Count > 0)
                                    {
                                        DataModul.Link.DeleteInvalidPerson();
                                    }
                                }
                            }
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            if (DataModul.Witness.Count > 0)
                            {
                                DataModul.Witness.DeleteAllFamPred((i) => !DataModul.Family.Exists(i));
                            }
                            goto IL_1b4b;
                        IL_1b4b: // <========== 5
                                 // <========== 5
                            ProjectData.ClearProjectError();
                            num3 = 5;
                            WarningVisible = true;
                            WarningText = "Datei komprimieren";
                            while (!DataModul.DB_NameTable.EOF)
                            {
                                DataModul.DB_NameTable.Index = nameof(NameIndex.PNamen);
                                DataModul.DB_NameTable.Seek("=", 0);
                                if (!DataModul.DB_NameTable.NoMatch)
                                {
                                    if (DataModul.DB_NameTable.Fields[NameFields.PersNr].AsInt() == 0)
                                    {
                                        DataModul.DB_NameTable.Delete();
                                    }
                                    continue;
                                }
                                else
                                    break;
                            }
                            goto IL_1c85;
                        IL_1c85: // <========== 3
                                 // <========== 4
                            num = 332;
                            lErl = 3;
                            Filesystem_Scrubb(out DateiName, out text);
                            Modul1.Dateienopen();
                            WarningText = "Personenindex löschen";
                            DataModul.DSB.TryExecute("DELETE * FROM Such");
                            if (DataModul.DB_PersonTable.RecordCount > 1)
                            {
                                num18 = 0;
                                DataModul.DB_PersonTable.MoveFirst();
                                WarningText = "bearbeite Person";
                                while (!DataModul.DB_PersonTable.EOF)
                                {
                                    if (DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt() == 0)
                                    {
                                        DataModul.DB_PersonTable.Delete();
                                        num18 = 0;
                                        DataModul.DB_PersonTable.MoveFirst();
                                        WarningText = "bearbeite Person";
                                        continue;
                                    }
                                    num18++;
                                    Application.DoEvents();
                                    Modul1.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                                    while (num18 < Modul1.PersInArb)
                                    {
                                        DataModul.Names.DeleteAllPers(num18);

                                        DataModul.Event.DeleteAllVitalE(num18);

                                        DataModul.Event.DeleteAllNonVitalE(num18);
                                        num18++;

                                    }
                                    lErl = 6;
                                    WarningText = "Person:" + Modul1.PersInArb.AsString();
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    text3 = Modul1.Person.Givennames;
                                    var sGivennames = Modul1.Person.Givennames.Replace("\"", "");
                                    string text4 = Modul1.Person.SurName + "," + sGivennames.Trim();
                                    text4 = text4.Replace("Ț", "T");
                                    DataModul.DSB_SearchTable.AddNew();
                                    DataModul.DB_PersonTable.Edit();
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = "";
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = "";
                                    DataModul.DB_PersonTable.Fields[PersonFields.Such6].Value = "";
                                    DataModul.DB_PersonTable.Update();
                                    DataModul.DSB_SearchTable.Fields["Name"].Value = text4.Left(50).Trim();
                                    if (Modul1.Suchfeld[0] == ESearchSelection.e2)
                                    {
                                        DataModul.DB_PersonTable.Edit();
                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = text4.Left(50).Trim();
                                        DataModul.DB_PersonTable.Update();
                                    }
                                    if (Modul1.Suchfeld[1] == ESearchSelection.e2)
                                    {
                                        DataModul.DB_PersonTable.Edit();
                                        DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = text4.Left(50).Trim();
                                        DataModul.DB_PersonTable.Update();
                                    }
                                    if (Modul1.Suchfeld[2] == ESearchSelection.e2)
                                    {
                                        DataModul.DB_PersonTable.Edit();
                                        DataModul.DB_PersonTable.Fields[PersonFields.Such6].Value = text4.Left(50).Trim();
                                        DataModul.DB_PersonTable.Update();
                                    }
                                    DataModul.DSB_SearchTable.Fields["Nummer"].Value = Modul1.PersInArb;
                                    DataModul.DSB_SearchTable.Fields["Leit"].Value = Modul1.Person.Burial.Trim() == ""
                                        ? Strings.Trim(Strings.Left(Modul1.Person.SurName + "," + Modul1.Person.Givennames.Trim(), 50))
                                        : (object)Strings.Trim(Strings.Left(Modul1.Person.Burial + "," + Modul1.Person.Givennames.Trim(), 50));
                                    DataModul.DSB_SearchTable.Fields["Alias"].Value = Modul1.Person.Alias.Trim() == ""
                                        ? Strings.Trim(Strings.Left(Modul1.Person.SurName + "," + Modul1.Person.Givennames.Trim(), 50))
                                        : (object)Strings.Trim(Strings.Left(Modul1.Person.Alias + "," + Modul1.Person.Givennames.Trim(), 50));

                                    DataModul.DSB_SearchTable.Fields["K_Phon"].Value = Module2.Koelner_Phonetic(Modul1.Person.SurName);
                                    DataModul.DSB_SearchTable.Fields["Sound"].Value = Module2.GetSoundEx(Modul1.Person.SurName);
                                    DataModul.DSB_SearchTable.Fields["iKenn"].Value = " ";
                                    DataModul.DSB_SearchTable.Fields["Datum"].Value = 0;
                                    DateTime Datu;
                                    M1_Iter = 101;
                                    while (M1_Iter <= 104)
                                    {
                                        if ((Datu = DataModul.Event.GetDate((EEventArt)M1_Iter, Modul1.PersInArb, out var ds)) != default)
                                        {
                                            Modul1.sDatu = Strings.Right("00000000" + Strings.Trim(Datu.AsString()), 8);
                                            DataModul.DSB_SearchTable.Fields["Datum"].Value = Modul1.sDatu.Left(4).AsInt();
                                            DataModul.DSB_SearchTable.Fields["Sich"].Value = ds;
                                            string sReg = DataModul.Event.GetValue(((EEventArt)M1_Iter, Modul1.PersInArb, 0), EventFields.Reg, "");
                                            int i2 = M1_Iter;
                                            if (i2 is 101 or 102)
                                            {
                                                DataModul.DSB_SearchTable.Fields["iKenn"].Value = "*";
                                                if (Modul1.sDatu.Trim() != "")
                                                {
                                                    if ((Modul1.Suchfeld[0] == ESearchSelection.e6) & (M1_Iter == 101))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if ((Modul1.Suchfeld[0] == ESearchSelection.e7) & (M1_Iter == 102))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if (Modul1.Suchfeld[0] == ESearchSelection.e3)
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = text4.Left(50).Trim() + " " + Modul1.sDatu.AsString();
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if (Modul1.Suchfeld[0] == ESearchSelection.e4)
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Modul1.sDatu.AsString().Trim() + " " + text4.Left(50).Trim();
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if (Modul1.Suchfeld[1] == ESearchSelection.e3)
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = text4.Left(50).Trim() + " " + Modul1.sDatu.AsString().Trim();
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if (Modul1.Suchfeld[1] == ESearchSelection.e4)
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = Modul1.sDatu.AsString().Trim() + " " + text4.Left(50).Trim();
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if (Modul1.Suchfeld[2] == ESearchSelection.e3)
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such6].Value = text4.Left(50).Trim() + " " + Modul1.sDatu.AsString();
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if (Modul1.Suchfeld[2] == ESearchSelection.e4)
                                                    {
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such6].Value = Modul1.sDatu.AsString().Trim() + " " + text4.Left(50).Trim();
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                if (i2 is 103 or 104)
                                                {
                                                    DataModul.DSB_SearchTable.Fields["iKenn"].Value = "+";
                                                    if ((Modul1.Suchfeld[0] == ESearchSelection.e8) & (M1_Iter == 103))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if ((Modul1.Suchfeld[0] == ESearchSelection.e9) & (M1_Iter == 104))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if ((Modul1.Suchfeld[1] == ESearchSelection.e8) & (M1_Iter == 103))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such5].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if ((Modul1.Suchfeld[1] == ESearchSelection.e9) & (M1_Iter == 104))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if ((Modul1.Suchfeld[2] == ESearchSelection.e8) & (M1_Iter == 103))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                    if ((Modul1.Suchfeld[2] == ESearchSelection.e9) & (M1_Iter == 104))
                                                    {
                                                        DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                                                        DataModul.DB_PersonTable.Edit();
                                                        DataModul.DB_PersonTable.Fields[PersonFields.Such4].Value = Strings.Trim(sReg);
                                                        DataModul.DB_PersonTable.Update();
                                                    }
                                                }
                                                else
                                                {
                                                    DataModul.DSB_SearchTable.Fields["iKenn"].Value = "S";
                                                }
                                            }
                                            break;
                                        }
                                        lErl = 2;
                                        M1_Iter++;
                                    }
                                    DataModul.DSB_SearchTable.Update();
                                    DataModul.DB_PersonTable.MoveNext();
                                }
                            }
                            ProjectData.ClearProjectError();
                            num3 = 6;
                            if (DataModul.Place.Count > 0)
                            {
                                WarningText = "Ortsindex löschen";
                                DataModul.DOSB.TryExecute("DELETE * FROM ortSuch");
                                WarningText = "bearbeite Ort";
                                DataModul.Place.ForeEachTextDo(DataModul.TextLese1,
                                (i, s) =>
                                {
                                    if (s[0] != "")
                                    {
                                        DataModul.DOSB_OrtSTable.AddNew();
                                        DataModul.DOSB_OrtSTable.Fields["Name"].Value = s[0].Left(100);
                                        DataModul.DOSB_OrtSTable.Fields["Nr"].Value = i;
                                        DataModul.DOSB_OrtSTable.Update();
                                    }
                                }, (f, i) => { WarningText = $"Bearbeite Ort {i}"; }
                                );
                            }
                            if ((DataModul.DB_PersonTable.RecordCount > 0)
                                | (DataModul.DB_FamilyTable.RecordCount > 0))
                            {
                                Person_SetGUIDsIN();
                                Family_SetGUIDsIN();
                            }
                            value = true;
                            Menue_ButtonsSetEnable(value);
                            _ = Interaction.MsgBox("Fertig", title: "Dateireorganisation", mb: MessageBoxButtons.OK);
                            WarningText = "";
                            WarningVisible = false;
                            goto end_IL_0001_2;

                        IL_48ec:
                            num = 768;
                            if (Information.Err().Number == 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_4ae1;
                            }
                            else
                            {
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                            }
                            goto IL_4add;
                        IL_49e3:
                            num = 778;
                            if (Information.Err().Number == 3021)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num2 = 0;
                                goto IL_1b4b;
                            }
                            else
                            {
                                var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                if (Interaction.MsgBox(Mldg, "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                            }
                            goto IL_4add;
                        IL_4add: // <========== 3
                                 // <========== 5
                            num4 = num2;
                            goto IL_4ae5;
                        IL_4ae1: // <========== 4
                                 // <========== 4
                            num4 = unchecked(num2 + 1);
                            goto IL_4ae5;
                        IL_4ae5:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 234:
                                case 235:
                                    goto IL_1475;
                                case 305:
                                case 316:
                                case 317:
                                case 780:
                                    goto IL_1b4b;
                                case 326:
                                case 332:
                                case 738:
                                    goto IL_1c85;

                                case 4:
                                case 8:
                                case 270:
                                case 731:
                                case 787:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is Exception exception && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(exception, lErl);
                try0001_dispatch = 22335;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 5
            // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private static void Family_SetGUIDsIN()
    {
        checked
        {
            DataModul.Family.ForEachDo((r) =>
            {
                if (null == r.Fields[FamilyFields.Fuid].Value
                || Strings.Trim(r.Fields[FamilyFields.Fuid].AsString()) == "")
                {
                    r.Edit();
                    r.Fields[FamilyFields.Fuid].Value = Guid.NewGuid();
                    r.Update();
                }
            });
        }
    }

    private static void Person_SetGUIDsIN()
    {
        checked
        {
            DataModul.Person.ForEachDo((r) =>
            {
                if (default == r.Fields[PersonFields.PUid].AsGUID()
                || Strings.Trim(r.Fields[PersonFields.PUid].AsString()) == "")
                {
                    r.Edit();
                    r.Fields[PersonFields.PUid].Value = Guid.NewGuid();
                    r.Update();
                }
            });
        }
    }

    private void Filesystem_Scrubb(out string DateiName, out string text)
    {
        DataModul.MandDB.Close();
        DataModul.TempDB.Close();
        DataModul.DOSB.Close();
        DataModul.DSB.Close();
        if (Modul1.Verz.Right(1) != "\\")
        {
            Modul1.Verz += "\\";
        }
        FileSystem.Kill(Modul1.Verz + "Such.mdb");
        FileSystem.Kill(Modul1.Verz + "Ort1.mdb");
        FileSystem.Kill(Modul1.Verz + "Tempo.mdb");
        DateiName = Modul1.GenFreeDir + "\\Such2.mdb";
        text = File.Exists(DateiName) ? Modul1.GenFreeDir + "\\Such2.mdb" : Modul1.GenFreeDir + "\\Such.mdb";

        string destination = Modul1.Verz + "Such.mdb";
        FileSystem.FileCopy(text, destination);
        DateiName = Modul1.GenFreeDir + "\\Ort2.mdb";
        text = File.Exists(DateiName) ? Modul1.GenFreeDir + "\\Ort2.mdb" : Modul1.GenFreeDir + "\\Ort1.mdb";
        destination = Modul1.Verz + "Ort1.mdb";
        FileSystem.FileCopy(text, destination);
        DateiName = Modul1.GenFreeDir + "\\Tempo2.mdb";
        text = File.Exists(DateiName) ? Modul1.GenFreeDir + "\\Tempo2.mdb" : Modul1.GenFreeDir + "\\Tempo.mdb";
        destination = Modul1.Verz + "Tempo.mdb";
        FileSystem.FileCopy(text, destination);
        FileSystem.Kill(Modul1.Verz + "Gen_Plusdaten1.mdb");
        DataModul.CompactDatabase(Modul1.Verz + "Gen_Plusdaten.mdb", Modul1.Verz + "Gen_Plusdaten1.mdb");
        var text2 = Filesystem_HandleBackup("Gen_Plusdaten");
        FileSystem.Rename(Modul1.Verz + "Gen_Plusdaten.mdb", Modul1.Verz + text2 + ".mdb");
        FileSystem.Rename(Modul1.Verz + "Gen_Plusdaten1.mdb", Modul1.Verz + "Gen_Plusdaten.mdb");
    }

    private string Filesystem_HandleBackup(string sFileName)
    {
        string text2;
        if (File.Exists($"{Modul1.Verz}{sFileName}{2}.mdb"))
        {
            if (File.Exists($"{Modul1.Verz}{sFileName}{3}.mdb"))
            {
                if (File.Exists($"{Modul1.Verz}{sFileName}{4}.mdb"))
                {
                    text2 = $"Gen_Plusdaten{2.AsString().Trim()}";
                    FileSystem.Kill($"{Modul1.Verz}{sFileName}{2}.mdb");
                    FileSystem.Kill($"{Modul1.Verz}{sFileName}{3}.mdb");
                }
                else
                {
                    text2 = sFileName + 4.AsString().Trim();
                    FileSystem.Kill($"{Modul1.Verz}{sFileName}{2}.mdb");
                }
            }
            else
            {
                text2 = sFileName + 3.AsString().Trim();
                FileSystem.Kill($"{Modul1.Verz}{sFileName}{4}.mdb");
            }
        }
        else
        {
            text2 = sFileName + 2.AsString().Trim();
            FileSystem.Kill($"{Modul1.Verz}{sFileName}{3}.mdb");
        }
        return text2;
    }

    private static void Link_DeleteAllEmptyFam()
    {
        DataModul.Link.DeleteFamWhere(0, (l) => true);
    }

    private static void SourceLink_Check()
    {
        DataModul.SourceLink_DeleteAllWhere((SourceLink) => !(SourceLink.iLinkType switch
        {
            1 => DataModul.Person.Exists(SourceLink.iPerFamNr),
            2 => DataModul.Family.Exists(SourceLink.iPerFamNr),
            3 => DataModul.Event.Exists(SourceLink.eArt, SourceLink.iPerFamNr, SourceLink.iLfdNr),
            _ => false,
        }));
    }

    private void Menue_ButtonsSetEnable(bool value)
    {
        MandantsEnabled = value;
        PrintEnabled = value;
        ImportExportEnabled = value;
        AddressEnabled = value;
        CalculationsEnabled = value;
        FunctionKeysEnabled = value;
        ConfigEnabled = value;
        ReorgEnabled = value;
        BackupWriteEnabled = value;
        CheckUpdateEnabled = value;
        SendDataEnabled = value;
        RemoteDiagEnabled = value;

        /*        
                FamiliesEnabled = value;
                SourcesEnabled = value;
                PersonsEnabled = value;
                PlacesEnabled = value;
                ManageTextsEnabled = value;

                CheckFamiliesEnabled = value;
                CheckMissingEnabled = value;
                CheckPersonsEnabled = value;
                DuplettesEnabled = value;
                NotesEnabled = value;
                CardModeEnabled = value;
                EnterLizenzEnabled = value;

                BachupReadEnabled = value;

                UpdNoEnabled = value;
                UpdYesEnabled = value;


                MergingEnabled = value;
                HelpMainEnabled = value;


                TodayBirthEnabled = value;
                TodayDeathEnabled = value;
                TodayMarriageEnabled = value;
                TodayMarrRelEnabled = value;
                PropertyEnabled = value;
          */
    }


    [RelayCommand]
    private void OpenBackupWrite()
    {
        try
        {
            string drive = Interaction.InputBox("Auf welches Laufwerk soll gesichert werden?", "Datensicherung des aktuellen Mandanten").Trim().Left(1);
            if (string.IsNullOrEmpty(drive) || !Module2.IsDriveReady(drive + ":", true.AsString()))
            {
                Interaction.MsgBox("Das Laufwerk " + drive + ": ist nicht vorhanden oder nicht beschreibbar\nBitte wählen Sie ein anderes Laufwerk für Ihre Sicherung.");
                return;
            }

            string backupDir = drive + ":\\Genplussich\\" + Strings.Mid(Modul1.Verz, 16, Modul1.Verz.Length - 1);
            if (Directory.Exists(backupDir))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                Directory.Move(backupDir, backupDir + timestamp);
            }
            Directory.CreateDirectory(backupDir);

            WarningVisible = true;
            WarningText = "Daten kopieren";

            string source = Modul1.Verz + "Gen_plusdaten.Mdb";
            string destination = backupDir + "Gen_plusdaten.Mdb";
            File.Copy(source, destination, true);

            Modul1.Persistence.CopyDirectory(Modul1.Verz + "Bilder", backupDir);

            WarningVisible = false;
            Interaction.MsgBox("Datensicherung auf Laufwerk " + drive + ": erfolgreich beendet");
        }
        catch (Exception ex)
        {
            WarningVisible = false;
            Interaction.MsgBox("Während der Datensicherung sind Fehler aufgetreten: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Modul1.Dateienopen();
            //            this.Cursor = Cursors.Default;
        }
    }

    [RelayCommand]
    private void OpenBackupRead()
    {
        try
        {
            var Value = (float)Interaction.MsgBox("Alle Daten des aktuellen Mandanten werden überschrieben", "Achtung", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            if (Value == 2f) return;

            CloseDatabases();

            string drive = Interaction.InputBox("Auf welchem Laufwerk befinden sich die gesicherten Daten?").Trim().Left(1);
            if (string.IsNullOrEmpty(drive)) return;

            string backupPath = $"{drive}:\\Genplussich\\{Strings.Mid(Modul1.Verz, 16, Modul1.Verz.Length - 1)}";
            //     Dir1.Path = backupPath;
            //     File2.Path = backupPath;

            FileSystem.MkDir(Modul1.Verz + "Bilder");

            WarningVisible = true;
            WarningText = "Daten kopieren";

            string currentDataPath = Modul1.Verz + "Gen_plusdaten.Mdb";
            string backupDataPath = backupPath + "Gen_plusdaten.Mdb";

            string message = $"Soll der Mandant mit letzter Änderung am \n\n{FileSystem.FileDateTime(currentDataPath)}\n\nmit Daten vom \n\n{FileSystem.FileDateTime(backupDataPath)}\n\nüberschrieben werden?";
            if (Interaction.MsgBox(message, title: "Warnung", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Exclamation) == DialogResult.No) return;

            FileSystem.FileCopy(backupDataPath, currentDataPath);

            CopyBackupFiles(drive, backupPath);

            WarningVisible = false;
            Interaction.MsgBox("Rücksichern der Daten abgeschlossen");
            Modul1.Dateienopen();
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private void CloseDatabases()
    {
        DataModul.TempDB.Close();
        DataModul.DOSB.Close();
        DataModul.DSB.Close();
        DataModul.MandDB.Close();
    }

    private void CopyBackupFiles(string drive, string backupPath)
    {
        try
        {
            Modul1.Persistence.CopyDirectory($"{drive}:\\Genplussich\\{Strings.Mid(Modul1.Verz, 16, Modul1.Verz.Length - 1)}Bilder", Modul1.Verz1);
        }
        catch (Exception ex)
        {
            Interaction.MsgBox("Fehler beim Kopieren der Dateien: " + ex.Message, icon: MessageBoxIcon.Exclamation);
        }
    }

    private void HandleException(Exception ex)
    {
        if (Information.Err().Number == 75 || Information.Err().Number == 76)
        {
            Interaction.MsgBox("Auf dem angegebenen Laufwerk wurde keine Datensicherung für den aktuellen Mandaten gefunden.");
        }
        else
        {
            Interaction.MsgBox("Fehler: " + ex.Message, icon: MessageBoxIcon.Exclamation);
        }
        WarningVisible = false;
        Modul1.Dateienopen();
    }
    private void _RadioButton3_Click(object sender, EventArgs e)
    {
        switch (((RadioButton)sender).Name)
        {
            case "RadioButton1":
                Modul1.FontSize = 8.7f;
                break;
            case "RadioButton2":
                Modul1.FontSize = 9.5f;
                break;
            case "RadioButton3":
                Modul1.FontSize = 10.3f;
                break;
            case "RadioButton4":
                Modul1.FontSize = 10.9f;
                break;
            case "RadioButton5":
                Modul1.FontSize = 11.7f;
                break;
            case "RadioButton6":
                Modul1.FontSize = 12.4f;
                break;
            case "RadioButton7":
                Modul1.FontSize = 13.2f;
                break;
            case "RadioButton8":
                Modul1.FontSize = 14.9f;
                break;
            case "RadioButton9":
                Modul1.FontSize = 7.8f;
                break;
            case "RadioButton10":
                Modul1.FontSize = 16.8f;
                break;
        }
        Grossaend(Modul1.FontSize);
    }

    [RelayCommand]
    private void UpdateYes()
    {
        //Discarded unreachable code: IL_07df
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        int num6 = default;
        int lErl = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int M1_Iter = default;
                    int num4;
                    string DateiName;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            if (Modul1.cMandDrive.DriveType == DriveType.CDRom)
                            {
                                goto end_IL_0001;
                            }
                            goto IL_001d;
                        case 2789:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_08e7;
                                    default:
                                        goto end_IL_0001_2;
                                }
                                if (Information.Err().Number == 62)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_08e7;
                                }
                                else
                                {
                                    if (Information.Err().Number == 5)
                                    {
                                        goto end_IL_0001;
                                    }
                                    var Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
                                    if (Interaction.MsgBox(Mldg, "Fehler", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                                    {
                                        ProjectData.EndApp();
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_08eb;
                                }
                            }
                        end_IL_0001_2:
                            break;
                        IL_001d:
                            num = 4;
                            string Value = "";
                            CheckUpdateEnabled = false;
                            UpdateVisible = false;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            if (Modul1.System.xDemo)
                            {
                                Value = "===================Demov";
                            }
                            else
                            {
                                Value = Modul1.Persistence.ReadStringProg("IDF.Dat");
                            }

                            string text = Value;
                            string text2 = Strings.Mid(Value, 20, 5);
                            string text3 = Strings.Mid(Modul1.Version, 9, 8);
                            MainProject.Forms.Hinter.WebBrowser1.DocumentText = "";
                            bool flag = false;
                            if (NetworkInterface.GetIsNetworkAvailable())
                            {
                                flag = new Ping().Send(Modul1.AppHostName, 1500).Status == IPStatus.Success;
                            }
                            else if (Modul1.AutoupD != "0")
                            {
                                _ = Interaction.MsgBox("Es besteht keine Internetverbindung!");
                            }

                            if (!flag)
                                goto IL_07c8;
                            MainProject.Forms.Hinter.WebBrowser1.DocumentText = "";
                            DateiName = Modul1.GenFreeDir + "\\GPUpd1.exe";
                            if (!File.Exists(DateiName))
                            {
                                _ = Interaction.MsgBox("Updater laden");
                            }
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Append);
                            FileSystem.FileClose(99);
                            FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Input);
                            Value = "";
                            M1_Iter = 1;
                            while (M1_Iter <= 8)
                            {
                                Value = Value + " " + FileSystem.LineInput(99);
                                M1_Iter++;
                            }
                            if (Modul1.AutoupD == "1")
                            {
                                text += "A";
                            }
                            FileSystem.FileClose(99);
                            num6 = 0;
                            Value = Value + " !! " + Environment.UserName;
                            MainProject.Forms.Hinter.WebBrowser1.DocumentText = "";
                            string urlString = "https://www.Genpluswin.de/Up24/Versionmelden.php?name=" + text3 + text2 + " " + Environment.MachineName + "   " + text + "  " + Modul1.Verz + " " + Value;
                            MainProject.Forms.Hinter.WebBrowser1.Navigate(urlString);
                            goto IL_03ba;

                        IL_03ba: // <========== 3
                            num = 52;
                            lErl = 10;
                            Application.DoEvents();
                            string documentText = MainProject.Forms.Hinter.WebBrowser1.DocumentText;
                            if (Strings.InStr(documentText, "nicht aktuell") > 0)
                            {
                                _ = Interaction.MsgBox("Ihre Programmversion ist nicht aktuell.\r\nEs wird eine Aktualisierung empfohlen!", title: "Aktualitätskontrolle", icon: MessageBoxIcon.Exclamation);
                                byte b = (byte)Interaction.MsgBox("Aktualisierung jetzt durchführen?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                int num7 = (int)Math.Round(DateTime.Today.ToOADate());
                                Modul1.Persistence.WriteIntInit("GPAK", num7);
                                if (b == 6)
                                {
                                    _ = Interaction.Shell(Modul1.InstPath + "\\GPUpd1.exe GEN_PLUS", (int)AppWinStyle.NormalFocus);
                                }
                            }
                            else if (Strings.InStr(documentText, "ist aktuell") > 0)
                            {
                                _ = Interaction.MsgBox("Ihr Hauptmodul ist aktuell!\nEs sind keine Maßnahmen erforderlich!", title: "Aktualitätskontrolle", icon: MessageBoxIcon.Information);
                                int num8 = (int)Math.Round(DateTime.Today.ToOADate());
                                Modul1.Persistence.WriteIntInit("GPAK", num8);
                            }
                            else if (Strings.InStr(documentText, "Version eingestellt") > 0)
                            {
                                _ = Interaction.MsgBox("Die Updates für diese Version sind eingestellt!\nEs gibt eine neue Version!", title: "Aktualitätskontrolle", icon: MessageBoxIcon.Information);
                            }
                            else if (Strings.InStr(documentText, "nicht gefunden") > 0)
                            {
                                _ = Interaction.MsgBox("Verbindung fehlgeschlagen!\nVersuchen Sie es später noch einmal!", title: "Aktualitätskontrolle", icon: MessageBoxIcon.Information);
                            }
                            else
                            {
                                num6++;
                                if (num6 < 100000)
                                {
                                    goto IL_03ba;
                                }
                                else
                                {
                                    var Mldg = "Internetverbindung zu langsam oder blockiert";
                                    Mldg += "\nUrsache ist meist die Blockierung des Internetzugangs eines Programms ";
                                    Mldg += "\noder Programmteils durch Schutzprogramme oder PC-Einstellungen.";
                                    Mldg += "\nÄndern Sie diese Einstellungen oder laden Sie die Updates direkt von ";
                                    Mldg += "\nhttps://www.genpluswin.de/index.php?id=update";
                                    _ = Interaction.MsgBox(Mldg);
                                }



                            }
                            goto IL_07c8;
                        IL_07c8: // <========== 3
                            num = 109;
                            CheckUpdateEnabled = true;
                            goto end_IL_0001;
                        IL_08e7:
                            num4 = unchecked(num2 + 1);
                            goto IL_08eb;
                        IL_08eb:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 52:
                                case 104:
                                case 105:
                                    goto IL_03ba;
                                case 70:
                                case 71:
                                case 84:
                                case 85:
                                case 88:
                                case 89:
                                case 92:
                                case 93:
                                case 103:
                                case 106:
                                case 107:
                                case 108:
                                case 109:
                                    goto IL_07c8;
                                case 2:
                                case 110:
                                case 116:
                                case 123:
                                    goto end_IL_0001;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is Exception exception && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(exception, lErl);
                try0001_dispatch = 2789;
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

    [RelayCommand(CanExecute = nameof(CheckUpdateEnabled))]
    private void OpenShowUpdate()
    {
        UpdateVisible = false;
        OpenCheckUpdate();
    }


    [RelayCommand]
    private void UpdateNo()
    {
        if (DisableMsgChecked)
        {
            int num = 400000;
            Modul1.Persistence.WriteIntInit("GPAK", num);
        }
        UpdateVisible = false;
    }

    [RelayCommand(CanExecute = nameof(SendDataEnabled))]
    private void OpenSendData()
    {
        _ = MainProject.Forms.Datenversandt.ShowDialog();
    }

    [RelayCommand]
    private void OpenInfoMessage()
    {
        if (Families_Enabled)
        {
            Modul1.Info();
        }
    }

    [RelayCommand]
    private void TrackBar1_MouseUp()
    {
        if (Modul1.FontSize > 0f)
        {
            Grossaend(Modul1.FontSize);
        }
    }


    partial void OnTrackBar1ValueChanged(int newval)
    {
        //Discarded unreachable code: IL_0448
        Rectangle bounds = Screen.PrimaryScreen.Bounds;
        int num = bounds.Width;
        int num2 = bounds.Height;
        if (Modul1.IText[EUserText.t271] == "")
        {
            Modul1.IText[EUserText.t271] = "Bildschirmauflösung";
        }
        if (Modul1.IText[EUserText.t270] == "")
        {
            Modul1.IText[EUserText.t270] = "Fenstergröße";
        }
        switch (newval)
        {
            case 0:
                {
                    short num3 = 600;
                    short num4 = 800;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 7.8f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 800 X 600 ";
                    }
                    break;
                }
            case 1:
                {
                    short num3 = 670;
                    short num4 = 900;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 8.7f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 900 X 670 ";
                    }
                    break;
                }
            case 2:
                {
                    Modul1.FontSize = 9.5f;
                    short num3 = 710;
                    short num4 = 900;
                    if (num2 >= num3 && num >= num4)
                    {
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 900 X 710 ";
                    }
                    break;
                }
            case 3:
                {
                    short num3 = 710;
                    short num4 = 1024;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 10.3f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1024 X 710 ";
                    }
                    break;
                }
            case 4:
                {
                    short num3 = 768;
                    short num4 = 1024;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 11f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1024 X 768 ";
                    }
                    break;
                }
            case 5:
                {
                    short num3 = 800;
                    short num4 = 1150;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 11.7f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1150 X 800 ";
                    }
                    break;
                }
            case 6:
                {
                    short num3 = 835;
                    short num4 = 1150;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 12.4f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1150 X 835 ";
                    }
                    break;
                }
            case 7:
                {
                    short num3 = 920;
                    short num4 = 1280;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 13.2f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1280 X 920 ";
                    }
                    break;
                }
            case 8:
                {
                    short num3 = 1050;
                    short num4 = 1400;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 14.9f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1400 X 1050 ";
                    }
                    break;
                }
            case 9:
                {
                    short num3 = 1200;
                    short num4 = 1600;
                    if (num2 >= num3 && num >= num4)
                    {
                        Modul1.FontSize = 16.5f;
                        FrmWindowSizeText = Modul1.IText[EUserText.t270] + ": 1600 X 1200 ";
                    }
                    break;
                }
        }
    }

    [RelayCommand]
    private void OpenShowHelp()
    {
        _ = Process.Start(Modul1.GenFreeDir + "\\Hilfe\\TeilA.PDF");
    }

    public void Person_LeerPerLoesch()
    {
        EEventArt[] aeVital = new[] { EEventArt.eA_Birth, EEventArt.eA_Baptism, EEventArt.eA_Death, EEventArt.eA_Burial,
            EEventArt.eA_105, EEventArt.eA_106, EEventArt.eA_107 };

        if (DataModul.DB_PersonTable.RecordCount <= 1)
        {
            return;
        }
        int num = 0;
        DataModul.DB_PersonTable.MoveFirst();
        WarningText = "bearbeite Person";
        while (!DataModul.DB_PersonTable.EOF)
        {
            Modul1.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
            if (Strings.Trim(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString()) == "")
            {
                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                bool xIsEmpty;
                if (xIsEmpty = Modul1.Person.isEmpty)
                {
                    foreach (var num3 in aeVital)
                    {
                        if (DataModul.Event.Exists(num3, Modul1.PersInArb, 0))
                        {
                            xIsEmpty = false;
                            break;
                        }
                    }
                    if (xIsEmpty)
                    {
                        EEventArt num5 = EEventArt.eA_300;
                        while (num5 <= EEventArt.eA_302)
                        {
                            DataModul.Event.DeleteAll(num5, Modul1.PersInArb);
                            num5++;
                        }
                        DataModul.DB_PersonTable.Delete();
                    }
                }
            }
            DataModul.DB_PersonTable.MoveNext();
        }

    }

    [RelayCommand]
    private void RemoteSupport()
    {
        _ = Interaction.Shell(Modul1.Verz1 + "Fernwartung.exe");
    }

    private void _TrackBar1_Scroll(object sender, EventArgs e)
    {
        int value = TrackBar1Value;
        TrackBar1BackColor = value < 4 ? Color.Red : Modul1.HintFarb;
    }

    [RelayCommand]
    private void SelectLanguage(object sender)
    {
        var M1_Iter = 0;
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Modul1.InitDir + "Multiling.dat", OpenMode.Output);
        var recordset = DataModul.OpenLinguaRecordSet(Modul1.InitDir + "Text1.mdb");
        checked
        {
            while (!recordset.EOF)
            {
                switch (((PictureBox)sender).Name)
                {
                    case "PictureBox2":
                        FileSystem.PrintLine(99, recordset.Fields["Text_Germ"].Value);
                        break;
                    case "PictureBox3":
                        FileSystem.PrintLine(99, recordset.Fields["Text_Franz"].Value);
                        break;
                    case "PictureBox4":
                        FileSystem.PrintLine(99, recordset.Fields["Text_Engl"].Value);
                        break;
                }
                recordset.MoveNext();
                M1_Iter++;
            }
            FileSystem.FileClose(99);
            Modul1.IText[EUserText.t262] = "Das Programm muss neu gestartet werde, um die Änderungen zu übernehmen";
            Modul1.IText[EUserText.t262] = Modul1.IText[EUserText.t262] + "\n\nLe programme doit être redémarré pour appliquer les changements";
            Modul1.IText[EUserText.t262] = Modul1.IText[EUserText.t262] + "\n\nProgram must be restarted to apply changes";
            _ = Interaction.MsgBox(Modul1.IText[EUserText.t262]);
            ProjectData.EndApp();
        }
    }


    public void Lingua()
    {
        string DateiName = Modul1.InitDir + "Text1.mdb";
        if (File.Exists(DateiName))
        {
            PbxLanguage1Visible = true;
            PbxLanguage2Visible = true;
            PbxLanguage3Visible = true;
        }
        DateiName = Modul1.InitDir + "Multiling.dat";
        checked
        {
            var M1_Iter = 0;
            if (!File.Exists(DateiName))
            {
                Modul1.Persistence.ReadStringsInit("Textegerm.dat", Modul1.IText, false);

                Modul1.IText[EUserText.tDeath_PNo_FNo_ANo] = "Gestorben  Pers.-Nr.  Fam.-Nr. Hei.Jahr     Ahn-Nr.";
                Modul1.IText[EUserText.tChild_AS] = "&" + Modul1.IText[EUserText.tChild_AS];
                Modul1.IText[EUserText.t44] = "&" + Modul1.IText[EUserText.t44];
                Modul1.IText[EUserText.tGodparents] = "Pa&ten";
                Modul1.IText[EUserText.t129] = "Medien";
                Modul1.IText[EUserText.t244] = "&" + Modul1.IText[EUserText.t244];
                Modul1.IText[EUserText.t246] = "Quellenverwaltung";
                Modul1.IText[EUserText.t247] = "Fernwartung";
                Modul1.IText[EUserText.t248] = "Dubletten";
                Modul1.IText[EUserText.t249_Property] = "Bemerkungen duchsuch.";
                Modul1.IText[EUserText.t250] = "Aktualitätskontrolle";
                Modul1.IText[EUserText.t251] = "Datei reorganisieren";
                Modul1.IText[EUserText.t252] = "Datensicherung";
                Modul1.IText[EUserText.t253] = "Sicherung zurücklesen";
                Modul1.IText[EUserText.t254] = "Daten verschicken";
                Modul1.IText[EUserText.t255] = "Hilfetext";
                Modul1.IText[EUserText.t256] = "Mandant";
                Modul1.IText[EUserText.t257] = "Geb./Get.:";
                Modul1.IText[EUserText.t258] = "Gest./Begr.:";
                Modul1.IText[EUserText.t259] = "Präfix";
                Modul1.IText[EUserText.t260] = "fiktiv.Heiratsdatum";
                Modul1.IText[EUserText.t261] = "Dimissiorale";
                Modul1.IText[EUserText.t263] = "&Zeugen";
                Modul1.IText[EUserText.t264] = "Geburt";
                Modul1.IText[EUserText.t267] = "Sex";
                Modul1.IText[EUserText.t272] = "suchen Nu&mmer";
                Modul1.IText[EUserText.t273] = "su&chen Name";
                Modul1.IText[EUserText.t274] = "Partnersuche";
                Modul1.IText[EUserText.t275] = "Registersuche";
                Modul1.IText[EUserText.t276] = "Recherche";
                Modul1.IText[EUserText.t277] = "&Familienblatt";
                Modul1.IText[EUserText.t278] = "Lizenznummer eingeben";
                Modul1.IText[EUserText.t279] = "gesperrt";
                Modul1.IText[EUserText.t280] = "privat";
                Modul1.IText[EUserText.t281] = "frei";
                Modul1.IText[EUserText.t282] = "Datum vor Chr.";
                Modul1.IText[EUserText.t283] = "Ausgabezusatz";
                Modul1.IText[EUserText.t284] = "Geburt berechnen";
                Modul1.IText[EUserText.t283] = "Ausgabezusatz";
                Modul1.IText[EUserText.t284] = "Geburt berechnen";
                Modul1.IText[EUserText.t285] = "obere Ereignisbemerkungen";
                Modul1.IText[EUserText.t286] = "untere Ereignisbemerkungen";
                Modul1.IText[EUserText.t288] = "Sicherheit des Datums FUid=vor N=nach U=um R=errechnet ?=fraglich  Z=Zwischen **.**.****/ und **.**.****";
                Modul1.IText[EUserText.t289] = "Zeitraum des Datums  A= und (>And<) B=Bis";
                Modul1.IText[EUserText.t290] = "Vertraulichkeit der Information";
                Modul1.IText[EUserText.t291] = "&Kinddatum verwenden";
                Modul1.IText[EUserText.t292] = "&Letzten Ort wiederholen";
                Modul1.IText[EUserText.t293] = "Geburtsdatum errechen";
                Modul1.IText[EUserText.t294] = "bekanntes Alter";
                Modul1.IText[EUserText.t295] = "Jahre   Monate (Wochen) Tage";
                Modul1.IText[EUserText.t296] = "Rechnen";
                Modul1.IText[EUserText.t297] = "übernehmen";
                Modul1.IText[EUserText.t298] = "Suche nach";
                Modul1.IText[EUserText.t299] = "kopieren";
                Modul1.IText[EUserText.t300] = "neue &Familie anlegen";
                Modul1.IText[EUserText.t301] = "Zeugen";
                Modul1.IText[EUserText.t302] = "Zeuge bei";
                Modul1.IText[EUserText.t303] = "verb.Personen";
                Modul1.IText[EUserText.t304] = "verb.mit";
                Modul1.IText[EUserText.t305] = "Bemerkungen zur Person";
                Modul1.IText[EUserText.t306] = "neu E&ingeben";
                Modul1.IText[EUserText.t307] = "ohne Rückfrage löschen";
                Modul1.IText[EUserText.t308] = "Suche nach Nachfahrennummern";
                Modul1.IText[EUserText.t309] = "Suche nach Ahnennummern";
                Modul1.IText[EUserText.t310] = "Liste drucken";
                Modul1.IText[EUserText.t311] = "Suche nach (*)Geburts/(~)Taufdatum";
                Modul1.IText[EUserText.t312] = "Suche nach (+)Sterbe/(/)Begräbnisd.";
                Modul1.IText[EUserText.t313] = "Suche nach Personennummern";
                Modul1.IText[EUserText.t314] = "Detaillierte Suche nach Namen";
                Modul1.IText[EUserText.t315] = "Suche nach Familiendatum";
                Modul1.IText[EUserText.t316] = "Suche nach Änderungsdatum Personen";
                Modul1.IText[EUserText.t317] = "Suche nach Änderungsdatum Familien";
                Modul1.IText[EUserText.t318] = "Phonetic-Suche";
                Modul1.IText[EUserText.t319] = "nach Aliasnamen";
                Modul1.IText[EUserText.t320] = "Soundex-Suche";
                Modul1.IText[EUserText.t321] = "nachLeitnamen";
                Modul1.IText[EUserText.t322] = "Einschränkung auf Namen";
                Modul1.IText[EUserText.t323] = "Auswahl beibehalten";
                Modul1.IText[EUserText.t324] = "Ehepartner nicht anzeigen";
                Modul1.IText[EUserText.t325] = "Nur Familien";
                Modul1.IText[EUserText.t326] = "Frauen";
                Modul1.IText[EUserText.t327] = "Männer";
                Modul1.IText[EUserText.t328] = "Familien und Personen-Nr. ausgeben";
                Modul1.IText[EUserText.t329] = "Ahnen-Nr.ausgeben";
                Modul1.IText[EUserText.t330] = "Nachfahren-Nr.ausgeben";
                Modul1.IText[EUserText.t331] = "Bilder-Pfad ausgeben";
                Modul1.IText[EUserText.t332] = "Quellen ausgeben";
                Modul1.IText[EUserText.t333] = "Urkundennummer";
                Modul1.IText[EUserText.t334] = "Strukturierte Ausgabe";
                Modul1.IText[EUserText.t335] = "Ortsbezeichnung verkürzt";
                Modul1.IText[EUserText.t336] = "Nebenpersonen nur Grunddaten";
                Modul1.IText[EUserText.t337] = "Todesursache nicht ausgeben";
                Modul1.IText[EUserText.t338] = "ohne(Daten)";
                Modul1.IText[EUserText.t339] = "Geburt, Tod, Beruf / sonst.Daten";
                Modul1.IText[EUserText.t340] = "Geburt, Beruf / sonst.Daten, Tod";
                Modul1.IText[EUserText.t341] = "Personen nur Grunddaten";
                Modul1.IText[EUserText.t342] = "Reihenfolge der Daten";
                Modul1.IText[EUserText.t343] = "Suche";
                Modul1.IText[EUserText.t344] = "weiblich";
                Modul1.IText[EUserText.t345] = "männlich";
                Modul1.IText[EUserText.t347] = "Formatierung bei Quellen und Bemerkungen beibehalten";
                Modul1.IText[EUserText.t348] = "Pate";
                Modul1.IText[EUserText.t349] = "Nummerneingabe für Mehrfachdruck";
                Modul1.IText[EUserText.t350] = "GOV-Ortskennung";
                Modul1.IText[EUserText.t351] = "Heutiger Ortsname in ehemals deutschen Gebieten";
                Modul1.IText[EUserText.t352] = "Angaben für Forscherkontakte der DAGV, können Entfallen wenn die GOV-Kennung vorhanden ist";
                Modul1.IText[EUserText.t353] = "Ortsname?";
                Modul1.IText[EUserText.t354] = "Abbruch ohne speichern";
                Modul1.IText[EUserText.t355] = "&Koordinaten ermitteln";
                Modul1.IText[EUserText.t356] = "Ort anzeigen mit Google-&Earth";
                Modul1.IText[EUserText.t357] = "&Ort anzeigen mit Google-Maps";
                Modul1.IText[EUserText.t358] = "&Koordinaten umrechnen";
                Modul1.IText[EUserText.t359] = "Verbindung zum & GOV";
                Modul1.IText[EUserText.t360] = "Or&tssuche im GOV";
                Modul1.IText[EUserText.t361] = "&Zurück zur Datumsmaske";
                Modul1.IText[EUserText.t362] = "Nichtverwendete Orte";
                Modul1.IText[EUserText.t363] = "Ort löschen";
                Modul1.IText[EUserText.t364] = "Koordinatenrechner";
                Modul1.IText[EUserText.t365] = "Dezimal        Grad Minuten  Sekunden";
                Modul1.IText[EUserText.t366] = "Standardtextverarbeitung einstellen";
                Modul1.IText[EUserText.t367] = "automatische Aktualitätskontrolle ein";
                Modul1.IText[EUserText.t368] = "Farbeinstellungen";
                Modul1.IText[EUserText.t369] = "Prüfkriterien Familienprüfung";
                Modul1.IText[EUserText.t370] = "Hintergrund";
                Modul1.IText[EUserText.t371] = "Ereignismaske";
                Modul1.IText[EUserText.t372] = "Eingabe- und Anzeigefenster";
                Modul1.IText[EUserText.t373] = "Standard wieder herstellen";
                Modul1.IText[EUserText.t374] = "Word als Textverarbeitung einstellen";
                Modul1.IText[EUserText.t375] = "Interne Liste wiederbelegbarer Datensätze schreiben.Ist erforderlich, um gelöschte Personen- und Familiennummern neu zu belegen.";
                Modul1.IText[EUserText.t376] = "Mindestalter bei der Heirat";
                Modul1.IText[EUserText.t377] = "Jahre Mann";
                Modul1.IText[EUserText.t378] = "Jahre Frau";
                Modul1.IText[EUserText.t379] = "Jahre max. Altersdifferenz Mann / Frau";
                Modul1.IText[EUserText.t380] = "Jahre max. Alter der Frau beim letzten Kind";
                Modul1.IText[EUserText.t381] = "Kind max.";
                Modul1.IText[EUserText.t382] = "Jahre vor der Heirat";
                Modul1.IText[EUserText.t383] = "Dublettenkontrolle";
                Modul1.IText[EUserText.t384] = "Leitnamen ausgeben";
                Modul1.IText[EUserText.t385] = "Alter ausdrucken bei fehlendem Sterbedatum (bis 120 Jahre)";
                Modul1.IText[EUserText.t386] = "Bildschirmanzeige Suchliste max.";
                Modul1.IText[EUserText.t387] = "Einträge";
                Modul1.IText[EUserText.t388] = "Bei Suchliste Namenauswahlliste einblenden";
                Modul1.IText[EUserText.t389] = "Wohnorte starten mit Ort";
                Modul1.IText[EUserText.t390] = "mit Datum";
                Modul1.IText[EUserText.t391] = "Personen und Familiennummern wieder neu belegen";
                Modul1.IText[EUserText.t392] = "Beenden Schnellausstieg";
                Modul1.IText[EUserText.t393] = "Proband hat Generationenziffer 0";
                Modul1.IText[EUserText.t394] = "Proband hat Generationenziffer 1";
                Modul1.IText[EUserText.t395] = "Weiter";
                Modul1.IText[EUserText.t396] = "Suffix";
                Modul1.IText[EUserText.t397] = "Prädikat";
                Modul1.IText[EUserText.t398] = "Heimatort/-recht";
                Modul1.IText[EUserText.t399] = "Anzeige max";
                Modul1.IText[EUserText.t400] = "Angezeigt";
                Modul1.IText[EUserText.t401] = "Sonderfelder OFB";
                Modul1.IText[EUserText.t402] = "Person für OFB sperren";
                Modul1.IText[EUserText.t403] = "Sortiername";
                Modul1.IText[EUserText.t404] = "Namen für Index";
                Modul1.IText[EUserText.t405] = "Berufe für Index";
                Modul1.IText[EUserText.t406] = "Orte für Index";
                Modul1.IText[EUserText.t407] = "Einträge entfernen mit Doppelklick auf den Eintrag";
                Modul1.IText[EUserText.t408] = "Belegung der Funktionstasten mit festen Texten";
                Modul1.IText[EUserText.t409] = "&Schließen";
                Modul1.IText[EUserText.t410] = "S&uche starten";
                Modul1.IText[EUserText.t411] = "Wochentage anzeigen ab:";
                Modul1.IText[EUserText.t412] = "Keine Berechnung vorhanden";
                Modul1.IText[EUserText.t413] = "Namenspräfix";
                Modul1.IText[EUserText.t414] = "Namenssuffix";
                Modul1.IText[EUserText.t415] = "Status";
                Modul1.IText[EUserText.t416] = "Ereignisnamen";
                Modul1.IText[EUserText.t417] = "Haus-Nr.";
                Modul1.IText[EUserText.t418] = "Religionen";
                Modul1.IText[EUserText.t419] = "Todesursache";
                Modul1.IText[EUserText.t420] = "Groß- und Kleinschreibung beachten";
                Modul1.IText[EUserText.t421] = "Text enthält:";
                Modul1.IText[EUserText.t422] = "Start mit:";
                Modul1.IText[EUserText.t423] = "Vorauswahl beibehalten";
                Modul1.IText[EUserText.t424] = "Leitname";
                Modul1.IText[EUserText.t425] = "&Alten Namen zum Alias verschieben";
                Modul1.IText[EUserText.t426] = "Zur Todesursache verschieben";
                Modul1.IText[EUserText.t427] = "Zu Kirche/Friedhof  verschieben";
                Modul1.IText[EUserText.t428] = "Nichtverwendete Texte löschen";
                Modul1.IText[EUserText.t429] = "Diese Liste drucken";
                Modul1.IText[EUserText.t430] = "Auswahl umkehren";
                Modul1.IText[EUserText.t431] = "kein Hinweis auf Familien mit ausgeschalteter Prüfung";
                Modul1.IText[EUserText.t432] = "Zugehörige Personen anzeigen";
            }
            else
            {
                FileSystem.FileClose(99);
                M1_Iter = Modul1.Persistence.ReadStringsInit("Multiling.dat", Modul1.IText, true);
            }
            if (Modul1.IText[EUserText.t249_Property] == "Bemerkungen duchsuch.")
            {
                Modul1.IText[EUserText.t249_Property] = "Bemerkungen durchsuch.";
            }
            if (M1_Iter < 455)
            {
                Modul1.IText[EUserText.t435] = "Heute Geburtstag";
                Modul1.IText[EUserText.t436] = "Heute Todestag";
                Modul1.IText[EUserText.t437] = "Heute Hochzeitstag";
                Modul1.IText[EUserText.t438] = "Heute kirchl. Hochzeitstag";
                Modul1.IText[EUserText.t439] = "Heute haben Geburtstag:";
                Modul1.IText[EUserText.t440] = "Heute haben Todestag:";
                Modul1.IText[EUserText.t441] = "Heute haben Hochzeitstag:";
                Modul1.IText[EUserText.t442] = "Heute haben kirchl. Hochzeitstag:";
                Modul1.IText[EUserText.t443] = "Familienblatt für";
                Modul1.IText[EUserText.t444] = "Wohnung:";
                Modul1.IText[EUserText.t445] = "Wohnungen:";
                Modul1.IText[EUserText.t446] = "und";
                Modul1.IText[EUserText.t447] = $"mit {Modul1.AppName} aus Mandant:";
                Modul1.IText[EUserText.t448] = "Erstellt am";
                Modul1.IText[EUserText.t449] = "Seiten:";
                Modul1.IText[EUserText.t450] = "Quellen:";
                Modul1.IText[EUserText.t451] = "Kinder:";
                Modul1.IText[EUserText.t452] = "Adoptivkinder:";
                Modul1.IText[EUserText.t453] = "Geschwister und Halbgeschwister:";
                Modul1.IText[EUserText.t454] = "Eigene Verbindung:";
                Modul1.IText[EUserText.t455] = "Eigene Verbindungen:";
            }
            Modul1.IText[EUserText.t129] = "Medien";
        }
        Modul1.IText[EUserText.t456] = "Heute haben Tauftag";
        Modul1.IText[EUserText.t457] = "Heute haben Begräbnistag";
        Modul1.IText[EUserText.t458] = "Tauftag";
        Modul1.IText[EUserText.t459] = "Begräbnistag";
    }


    private void _Button28_Click(object sender, EventArgs e)
    {
        Stream stream = null;
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = "c:\\",
            Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 2,
            RestoreDirectory = true
        };
        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }
        try
        {
            stream = openFileDialog.OpenFile();
            if (stream == null)
            {
            }
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            _ = MessageBox.Show("Cannot read file from disk. Original error: " + ex2.Message);
            ProjectData.ClearProjectError();
        }
        finally
        {
            stream?.Close();
        }
    }


    private void _Button31_Click(object sender, EventArgs e)
    {
        CreationDateVisible = true;
        MarkedVisible = true;
        NotesVisible = true;
        List3Visible = true;
        Notes = "";
        CodeOfArmsVisible = false;
        DateTimePicker1Visible = true;
        SetDateVisible = true;
        switch (((Button)sender).Name)
        {
            case nameof(Menue.btnTodayBirth):
                CreationDate = Modul1.IText[EUserText.t439];
                Modul1.Art = EEventArt.eA_Birth;
                break;
            case nameof(Menue.btnTodayDeath):
                CreationDate = Modul1.IText[EUserText.t440];
                Modul1.Art = EEventArt.eA_Death;
                break;
            case nameof(Menue.btnTodayBapt):
                CreationDate = Modul1.IText[EUserText.t456];
                Modul1.Art = EEventArt.eA_Baptism;
                break;
            case nameof(Menue.btnTodayBurial):
                CreationDate = Modul1.IText[EUserText.t457];
                Modul1.Art = EEventArt.eA_Burial;
                break;
            case nameof(Menue.btnTodayMarriage):
                CreationDate = Modul1.IText[EUserText.t441];
                Modul1.Art = EEventArt.eA_Marriage;// 502;
                break;
            case nameof(Menue.btnTodayMarrRel):
                CreationDate = Modul1.IText[EUserText.t442];
                Modul1.Art = EEventArt.eA_MarrReligious; //503;
                break;
        }
        string right = "00" + DateTimePicker1Value.Month.AsString().Trim().Right(2) + "00" + DateTimePicker1Value.Day.AsString().Trim().Right(2);
        ListBox2Items.Clear();
        LstList3Items.Clear();
        foreach (var cEv in DataModul.Event.ReadAll(EventIndex.JaTa, Modul1.Art))
        {
            if (Strings.Len(Strings.Trim(cEv.dDatumV.AsString())) == 8
                && Strings.Mid(cEv.dDatumV.AsString(), 5, 4) == right)
            {
                if (Modul1.Art < EEventArt.eA_500)
                {
                    Modul1.PersInArb = cEv.iPerFamNr;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1.UbgT = Strings.Mid(Strings.Mid(cEv.dDatumV.AsString(), 1, 4) + " " + Modul1.Person.SurName.ToUpper() + ", " + Modul1.Person.Givennames + new string(' ', 240), 1, 200) + cEv.iPerFamNr.AsString();
                    ListBox2Items.Add(new(Modul1.UbgT, (Modul1.Art, cEv.iPerFamNr)));
                    Modul1.UbgT = "";
                }
                else
                {
                    Modul1.FamInArb = cEv.iPerFamNr;
                    DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                    Modul1.PersInArb = Modul1.Family.Mann;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1.UbgT = Strings.Mid(cEv.dDatumV.AsString(), 1, 4) + " " + Modul1.Person.SurName.ToUpper() + ", " + Modul1.Person.Givennames;
                    Modul1.PersInArb = Modul1.Family.Frau;
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Modul1.UbgT = Strings.Mid(Modul1.UbgT + " und " + Modul1.Person.SurName.ToUpper() + ", " + Modul1.Person.Givennames + new string(' ', 240), 1, 200) + cEv.iPerFamNr.AsString();
                    ListBox2Items.Add(new(Modul1.UbgT, (Modul1.Art, cEv.iPerFamNr)));
                    Modul1.UbgT = "";
                }
            }
        }
        checked
        {
            var M1_Iter = ListBox2Items.Count;
            while (M1_Iter-- > 0)
            {
                // Todo: Data ???
                LstList3Items.Add(new (ListBox2Items[M1_Iter].ToString()));
            }
        }
    }

    private void _ListBox3_Click(object sender, EventArgs e)
    {
        int num = checked((int)Math.Round(DateTime.Today.Year.ToString().AsDouble() - LstList3Text.Left(4).AsInt()));
        string left = CreationDate;
        if (left == Modul1.IText[EUserText.t439])
        {
            Notes = Strings.Mid(LstList3Text, 5, 180).Trim() + "\n hat heute den " + num.AsString() + ". \n" + Strings.Mid(Modul1.IText[EUserText.t435], 6, Modul1.IText[EUserText.t435].Length);
        }
        else if (left == Modul1.IText[EUserText.t440])
        {
            Notes = Strings.Mid(LstList3Text, 5, 180).Trim() + "\n hat heute den " + num.AsString() + ". \n" + Strings.Mid(Modul1.IText[EUserText.t436], 6, Modul1.IText[EUserText.t436].Length);
        }
        else if (left == Modul1.IText[EUserText.t456])
        {
            Notes = Strings.Mid(LstList3Text, 5, 180).Trim() + "\n hat heute den " + num.AsString() + ". \n" + Modul1.IText[EUserText.t458];
        }
        else if (left == Modul1.IText[EUserText.t457])
        {
            Notes = Strings.Mid(LstList3Text, 5, 180).Trim() + "\n hat heute den " + num.AsString() + ". \n" + Modul1.IText[EUserText.t459];
        }
        else if (left == Modul1.IText[EUserText.t441])
        {
            Notes = Strings.Mid(LstList3Text, 5, 180).Trim() + "\n haben heute ihren " + num.AsString() + ". \n" + Strings.Mid(Modul1.IText[EUserText.t437], 6, Modul1.IText[EUserText.t437].Length);
        }
        else if (left == Modul1.IText[EUserText.t442])
        {
            Notes = Strings.Mid(LstList3Text, 5, 180).Trim() + "\n haben heute ihren " + num.AsString() + ". \n" + Strings.Mid(Modul1.IText[EUserText.t438], 6, Modul1.IText[EUserText.t438].Length);
        }
    }

    private void _ListBox3_DoubleClick(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_02bb
        int try0001_dispatch = -1;
        int num = default;
        string left = default;
        int num2 = default;
        int num3 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                short Rich;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        goto IL_0011;
                    case 849:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 1:
                                    break;
                                default:
                                    goto end_IL_0001;
                            }
                            int num4 = num2 + 1;
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 30:
                                    goto end_IL_0001_2;
                                case 2:
                                case 19:
                                case 20:
                                case 31:
                                case 32:
                                    goto end_IL_0001_3;
                            }
                            goto default;
                        }
                    end_IL_0001_2:
                        break;
                    IL_0011:
                        num = 4;
                        var sel = LstList3SelectedItem.ItemData<(bool xPer, int iPerFam)>();
                        if (sel.xPer)
                        {
                            Modul1.PersInArb = sel.iPerFam;
                            Hide();
                            FileSystem.FileClose(99);
                            FileSystem.FileClose();

                            if (Modul1.PersInArb == 0)
                            {
                                Modul1.PersInArb = 1;
                            }
                            Personen.Default.WindowState = WindowState;
                            Personen.Default.Show(Modul1.PersInArb, EUserText.t158);

                        }
                        else if ((left == Modul1.IText[EUserText.t441] || left == Modul1.IText[EUserText.t442]) && 1 != 0)
                        {
                            Modul1.FamInArb = sel.iPerFam;
                            ProjectData.ClearProjectError();
                            num3 = -2;
                            Hide();
                            Familie.Default.iFamNr = Modul1.FamInArb;
                            Familie.Default.btnMainmenue.Text = Modul1.IText[EUserText.t158];
                            Familie.Default.Show();
                            Familie.Default.Hide();
                            Rich = 3;
                            Familie.Default.Fameinlesen(Modul1.FamInArb, out Rich);
                            num = 30;
                            _ = Familie.Default.ShowDialog();
                        }
                        goto end_IL_0001_2;
                }
                break;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is Exception exception && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(exception);
                try0001_dispatch = 849;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_3: // <========== 3
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void _Label27_Click(object sender, EventArgs e)
    {
        CreationDateVisible = false;
        MarkedVisible = false;
        NotesVisible = false;
        ListBox2Visible = false;
        List3Visible = false;
        Notes = "";
        DateTimePicker1Visible = false;
        SetDateVisible = false;
        CodeOfArmsVisible = true;
    }
    
    [RelayCommand]
    private void OpenProperty()
    {
        Modul1.Ubg = 0;
        MainProject.Forms.HGakte.Close();
        MainProject.Forms.HGakte.Show();
        Hide();
    }

    [RelayCommand]
    private void BackupWrite()
    {
    }


    [RelayCommand]
    private void OpenRemoteDiag()
    {
        //
    }

    [RelayCommand]
    private void OpenCheckUpdate()
    {
        // Check for updates logic here
    }


    private void _DateTimePicker1_ValueChanged(object sender, EventArgs e)
    {
        LstList3Items.Clear();
    }

    private void _Button38_Click(object sender, EventArgs e)
    {
        WebClient webClient = new();
        string uriString = "https://www.Genpluswin.de/Testfile/Druck.exe";
        string fileName = Modul1.Verz1 + "Druck.exe";
        try
        {
            webClient.DownloadFile(new Uri(uriString), fileName);
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            _ = Interaction.MsgBox("Fehler!\r\n" + ex2.Message, icon: MessageBoxIcon.Exclamation);
            ProjectData.ClearProjectError();
        }
        finally
        {
            _ = Interaction.MsgBox("Testversion Druckmodul geladen. Sie können jetzt mit der Testversion arbeiten.\nZum beenden des Testmodus starten Sie im Druckmodul die Aktualitätskontrolle.");
        }
    }

    public void SetAdress(string sFullname)
    {
        Owner = sFullname;
        OwnerBackColor = BackColor;
        if (sFullname.Trim() == "")
        {
            Owner = "Adresse eingeben";
            AddressBackColor = Color.Pink;
            _ = Interaction.MsgBox("Bitte Adresse eingeben. Ohne Adresse ist eine Personalisierug von Ausdrucken und Gedcom-Ausgaben nicht möglich.");
        }
        else
        {
            AddressBackColor = ImportExportBackColor;
            Address = "&Adresse";
        }
    }
}
