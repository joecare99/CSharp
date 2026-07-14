using BaseLib.Helper;
using GenFreeWin;
using GenFreeWin.Views;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.UI;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace GenFree;

public static class Modul1Ext
{
    public static void AddContextMenu(this RichTextBox rtb)
    {
        ContextMenuStrip contextMenuStrip = new();
        contextMenuStrip.Opening += ContextMenuOpening;
        contextMenuStrip.Items.Add("Ausschneiden").Tag = "Cut";
        contextMenuStrip.Items.Add("Kopieren").Tag = "Copy";
        contextMenuStrip.Items.Add("Einfügen").Tag = "Paste";
        contextMenuStrip.Items.Add("Löschen").Tag = "Del";
        contextMenuStrip.ItemClicked += ContextMenuItemClicked;
        rtb.ContextMenuStrip = contextMenuStrip;
    }
    public static void ContextMenuOpening(object sender, CancelEventArgs e)
    {
        ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
        RichTextBox richTextBox = (RichTextBox)contextMenuStrip.SourceControl;
        contextMenuStrip.Items[0].Enabled = richTextBox.SelectionLength > 0;
        contextMenuStrip.Items[1].Enabled = richTextBox.SelectionLength > 0;
        contextMenuStrip.Items[2].Enabled = Clipboard.ContainsText();
        contextMenuStrip.Items[3].Enabled = richTextBox.SelectionLength > 0;
    }

    private static void ContextMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        RichTextBox richTextBox = (RichTextBox)NewLateBinding.LateGet(sender, null, "SourceControl", new object[0], null, null, null);
        string tag = e.ClickedItem.Tag.AsString();
        if (tag == "Cut")
        {
            richTextBox.Cut();
        }
        else if (tag == "Copy")
        {
            richTextBox.Copy();
        }
        else if (tag == "Paste")
        {
            richTextBox.Paste();
        }
        else if (tag == "Del")
        {
            richTextBox.SelectedText = "";
        }
    }
}

public partial class _Modul1 : IModul1
{
    public static IModul1 Instance => field ??= new _Modul1();

    [Obsolete]
    public IProjectData ProjectData => field;
    IInteraction Interaction { get; }
    [Obsolete]
    public IVBInformation Information => field;
    [Obsolete]
    public IOperators Operators => throw new NotImplementedException();
    [Obsolete]
    public IVBConversions Conversions => field;
    [Obsolete]
    public IStrings Strings => field;
    public ISystem System => field;

    public string AppName => "Gen_FreeWin";
    public string Author => "Joe Care";
    public string VendorName => "JC-Soft";

    public const string AKontact = "49082 Osnabrück Friedrich-Holthaus-Str. 18 Tel.: 0541-80 00 79 00";
    public const string cPYear = "2025";
    public const string cPDate = $"20.04.{cPYear}";
    public const string cPVersion = "24.09.01";

    public string Version => $"Version {cPVersion} Stand {cPDate}";
    public string VersionT => $"{AppName} das freie Genealogieprogramm";
    public string Version2 => "Eingeschränkte Sonderversion";
    public string Version1 => $"(c) 1994-{cPYear} {Author} {AKontact}";
    public string VersDat => $"Stand {cPDate}";
    public string Titel2 => $"(c) 1994-{cPYear} {Author} {AKontact}";

    public string AppHostName => "www.jc99.de";
    private const string RegKeyGoogleEarth = "SOFTWARE\\Google\\Google Earth Plus";
    private const string GoogleEarthExe = "googleearth.exe";
    public string Message_sNoChangesOnCD => "Auf der CD sind keine Änderungen möglich!";
    public string Message_sDemoVerNotPossibl => "In der Demoversion sind nur 2 Mandanten mit je 100 Personen möglich!\nEinen dieser Werte haben Sie erreicht.";

    private const int ErrUser14 = -2146828268;
    public string Inhaber { get; set; }

    // FileSystem
    public string InitDir => $"\\{AppName}\\Init\\";

    public string TempPath => Path.GetTempPath();
    public string GenFreeDir { get; private set; }
    public string PictureDir => Path.Combine(Verz, "Bilder");
    public string ListDir => Path.Combine(Verz, "Listen");
    public string HelpDir => Path.Combine(GenFreeDir, "Help");

    public string Verz1 { get; set; }
    public string Verz { get; set; }
    public DriveInfo cMandDrive { get; set; }
    public DriveType Typ => cMandDrive.DriveType;


    public IList<string> DAus => throw new NotImplementedException();
    public IList<string> Kont { get; } = new string[101];
    public IList<string> Kont1 { get; } = new string[101];
    public IList<string> Kont2 { get; } = new string[21];
    public IList<string> KontM { get; } = new string[8];
    public IList<string> KontP { get; } = new string[8];
    public IList<string> KontF { get; } = new string[8];
    public IList<string> DTxt { get; } = new string[21];
    public IList<string> TxT { get; } = new string[101];
    public IList<string> Aus { get; } = new string[61];
    public IList<string> Quells { get; } = new string[6];
    public IList<string> MyList { get; } = [];
    public IList<string> oResult { get; } = [];
    public IList<string> Absend { get; } = [];
    public IList<string> Te { get; } = new string[12];

    // Colors
    public Color ErFarb { get; set; }
    public Color Farb { get; set; }
    public Color HintFarb { get; set; }
    public Color Feld1Farb { get; set; }
    public Color Farb1 { get; set; }
    public Color Hintfarb1 { get; set; }

    public Enum eWindowState { get; set; }

    public string globOrt1;
    public string globOrt2;
    public int thisYear => DateTime.Today.Year;
    public string sGeocodeXMLAddress => "http://maps.google.com/maps/api/geocode/xml?address={0}&sensor=true";
    public bool SterbMerk = false;

    public bool Reli { get; set; } = false;
    public int priv;
    public int Pos1;
    public int Pos2;
    public bool reorga { get; set; } = false;
    //=========================================

    public byte Programtesttemp { get; set; }
    public string AutoupD { get; set; }
    public bool FAendmerk { get; set; } = false;
    public bool PAendmerk { get; set; } = false;

    public bool Ad { get; set; } = false;
    public string Lw { get; set; }
    public float Aend { get; set; }
    public float FontSize { get; set; }
    public bool Aendf { get; set; }
    public int FamInArb { get; set; }
    public int PersInArb { get; set; }
    public int PersInArbsp { get; }
    public int PFSatz { get; set; }
    public int Startpers { get; set; }
    public int Ubg { get; set; }
    public string UbgT { get; set; }
    public string UbgT1 { get; set; }
    // Missing members for Druck project
    public string Datum1 { get; set; } = "";
    public string Datum2 { get; set; } = "";
    public string Ausdat { get; set; } = "";
    public int iPriv_aus { get; set; }
    public string Font1 { get; set; } = "";
    public int PersSp { get; set; }
    public string Datschuname { get; set; } = "";
    public string Eltq { get; set; } = "";


    public string sql;
    public IList<ESearchSelection> Suchfeld { get; } = new ESearchSelection[3];
    public IPersonData Person { get; set; } = new CPersonData(DataModul.DB_PersonTable, true);
    public IFamilyData Family { get; set; }
    public IUserData User { get; }
    /*     public  int o04_Family.Mann;
        public  int o04_Family.Modul1.Family.Frau;
        public  int[] Kind = new int[100];
        public  string[] KiAText = new string[100];
    */

    public int iKenn { get; set; }
    public ELinkKennz eLKennz { get; set; }
    public ETextKennz eTKennz { get; set; } // Text Kennz
    public EEventArt Art { get; set; }
    public ETextKennz eNKennz { get; set; }
    public string sPKennz { get; set; } // Kennz für die Bilder
    public int eWKennz { get; set; }
    public short Qkenn { get; set; }

    public short LfNR { get; set; }

    public string Mandant { get; set; }

    public byte Schalt { get; set; }
    public short ErSchalt { get; set; }
    public float Aschalt { get; set; }
    public int FamPerschalt { get; set; }
    public byte Suchschalt { get; set; }
    public byte Namschalt { get; set; }
    public int Suchfam { get; set; }
    public int SuchPer { get; set; }


    public short Tast;
    public string[] TxtT { get; } = new string[101];
    public string Ds;
    public float Value;
    public string Mldg;
    public string Meld;
    public string V;
    public string LiText;
    public int I;
    public short Datfehler_Abbruch;
    public int[] PruefJahr = new int[11];
    public int OrtNr;

    public IApplUserTexts IText { get; set; } // Texts for the application
    //  public  int eArt;
    public ERahmenResult Got { get; }

    public string KontU { get; set; }
    public string Gen1;
    public short Feg { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float Fs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool EreiRf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    public string MainProg => throw new NotImplementedException();

    public IEinles Einles => throw new NotImplementedException();
    public IGedAus GedAus => throw new NotImplementedException();
    public IPrintDat PrintDat => throw new NotImplementedException();

    public short Trans { get; set; }
    public int Nr { get; set; }
    public int AltNr { get; set; }
    public IModul1.Letzter Letzte { get; set; }
    public string AltName { get; set; }
    public short Druck_Tast { get; set; }


    public int Nr1;

    public string[] Kont_vorN { get; } = new string[16];
    public string[] arr_Folders { get; } = new string[201];
    public string[] arr_Files { get; } = new string[201];

    public string[] Temp { get; } = new string[21];

    public int[] aiVorn { get; } = new int[16];

    public int[] aiRuf { get; } = new int[16];

    public int[] aiSpitz { get; } = new int[16];

    public string LD;
    [Obsolete]
    public string Job { get; set; }
    public EWindowSize eWindowSize { get; set; }
    public int Prae;
    public int Ahnsp;
    public int Frauenkek1 { get; set; }
    public int Frauenkek2 { get; set; }

    public IList<short> Posi { get; set; } = new short[5];

    public bool Glob;
    public string InstPath => new FileInfo(Application.ExecutablePath).DirectoryName;
    public IModul1.Frauen Frauen_Renamed { get; }
    // public  Adresse Adr;

    public string sDeathMark => "†";
    public string sBirthMark => "~";
    public string sBaptismMark => "†";
    public string sBurialMark => "#";
    // Sorted and grouped properties by Interface, Type, and Name

    // Interface: IModul1

    // Interface: IPersonData
    public string FullSurName { get; }
    public string SurName { get; }

    // Interface: IPlaceData
    public string sPolName { get; }
    public int Histor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Quell { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int FeG { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Zusatzquelle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string Menue_Ziel => throw new NotImplementedException();

    public string NamenSuch_Wort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string sDatu { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Datklein { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public short Les { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Ind1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IRecordset DT_AhnTable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public byte Datschalt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public long Kek { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string OrtBem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    // General Properties
    public void Person_SetNames(int[] iName, (int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN)
        => throw new NotImplementedException();

    public string DezRechnen(string a4, string sKoor)
    {
        // Annahme: UbgT ist ein Feld/Property der Klasse (wie im Kontext)
        // Ziel: Dezimalberechnung auf Basis von UbgT, wie im Altcode, aber klarer und moderner
        // UbgT: z.B. "12345678" (mind. 4 Zeichen, sonst Fehler)
        // a4: wird mit dem Ergebnis überschrieben und zurückgegeben

        if (sKoor.Length != 4)
            return a4;

        // Extrahiere die ersten 2 und letzten 2 Zeichen
        string sMin = sKoor.Substring(0, 2);
        string sSec = sKoor.Substring(sKoor.Length - 2, 2);

        // Berechne num
        double fMinVal = double.TryParse(sMin, out var lval) ? lval / 0.00006d : 0.0;

        // Berechne num2
        double fSecVal = double.TryParse(sSec, out var lval2) ? lval2 / 0.0036d : 0.0;

        // Ergebnis als String
        string result = Math.Round(fMinVal + fSecVal).ToString();

        // Füge "00000" an, aber max. 6 Zeichen
        return result.PadRight(6, '0');
    }

    public DateTime AtomicTime(string sTimeServer)
    {
        DateTime result = default;
        try
        {
            TcpClient tcpClient = new();
            tcpClient.Connect(sTimeServer, 13);
            NetworkStream stream = tcpClient.GetStream();
            StringBuilder stringBuilder = new();
            int num;
            do
            {
                num = stream.ReadByte();
                if (num == -1)
                {
                    break;
                }
                _ = stringBuilder.Append(Convert.ToChar(num).ToString());
            }
            while (num != 13);
            tcpClient.Close();
            if (stringBuilder.Length > 0)
            {
                string text = stringBuilder.ToString();
                if (text.Contains(" "))
                {
                    string[] array = text.Split(' ');
                    if (array[1].Length == 8 && array[2].Length == 8)
                    {
                        string[] array2 = array[1].Split('-');
                        string[] array3 = array[2].Split(':');
                        result = new DateTime(2000 + array2[0].AsInt(), array2[1].AsInt(), array2[2].AsInt(), array3[0].AsInt(), array3[1].AsInt(), array3[2].AsInt());
                        int num2 = (int)Math.Round(DateTime.Now.ToString("zzzz").Substring(0, 3).AsDouble());
                        if (num2 != 0)
                        {
                            return result.AddHours(num2);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            result = default;
            ProjectData.ClearProjectError();
        }
        return result;

    }

    public void Info()
    {
        string text = "Entwicklungshistorie Hauptmodul";
        text += "\nBitte keine Anfragen hierzu";
        text += "\n";
        text += "\n01.10.                       Version 24";
        text += "\n30.10. Nachfahrenberechung (Obel).";
        text += "\n30.10. Fehlerhafte Bildbeschreibung aus Altanlage bei Ausdruck abfangen.";
        text += "\n17.11. Nachfahrenberechnung geändert wegen Family.Kind ohne Geb.-Dat.";
        text += "\n18.11. Merker entfernt.";
        text += "\n18.11. Datumspfrase einblenden.";
        text += "\n22.11. Partnersuche, Anzeige Urk.Nr. in Pers-Fam Maske, Nachfahrenberechnung.";
        text += "\n27.11. Partnersuche, Medienverwaltung in der Quellenmaske.";
        text += "\n01.12. Medienanzeigen.";
        text += "\n07.12. optisch Korrekturen, Zeugenanzeige.";
        text += "\n10.12. Schaltfäche Drucktest aktiviert";
        text += "\n10.12. Schaltfäche Drucktest deaktiviert";
        text += "\n22.12. Doppelte Familienbemerkung, Internetverbindung.";
        text += "\n07.01. Anzeige Datumspfrase, Anzeige Personenbild. ";
        text += "\n18.01. Prüfung Internetzugang, Vor- Rückblättern";
        text += "\n22.01. speichern nach Koordinaten ber., Schaltfläche Recherche, Kinderheiraten.";
        text += "\n28.01. Bilderanzeige.";
        text += "\n29.01. Problem bei Nachfahrenberechnung.";
        text += "\n07.02. ComboBox2.Items.Add('neue Eingabe/Bearbeiten')";
        text += "\n16.02. Abfrage der Datumszusätze eingebaut.";
        text += "\n24.02. nochmal Abfrage der Datumszusätze, Leitnamen.";
        text += "\n04.03. Dublettenbearbeitung, Speichern vor Familienaufruf.";
        text += "\n21.03. Suche nach Registernummer erweitert.";
        text += "\n25.03. <verstorben> bei Personen- und Familienblatt ausgeben, Lizenznummer.";
        text += "\n01.04. Nachfahrenberechnung ";
        text += "\n05.04. Familie nach Datum verschieben Coursor auf Label43.";
        text += "\n17.05. Dubletten Steuerelement n. gefunden abfangen.";
        text += "\n22.05. Pate bei in Datumsberechnung zugefügt.";
        text += "\n25.05. Quelle Standortmaske neu positioniert.";
        _ = Interaction.MsgBox(text);
    }

    /// <summary>
    /// Whether an office-suit application is installed.
    /// </summary>
    /// <param name="nComponent">The n component.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool OfficeAppInstalled(IModul1.MSOfficeComponent nComponent)
    {
        //Discarded unreachable code: IL_0060
        string name = Interaction.Choose((double)checked(nComponent + 1), "Access.Application", "Excel.Application", "Outlook.Application", "PowerPoint.Application", "Word.Application").AsString();
        RegistryKey classesRoot = Registry.ClassesRoot;
        return classesRoot.OpenSubKey(name) != null;
    }

    /// <summary>
    /// Ermittelt den Installationspfad von Google Earth, indem verschiedene Registry-Pfade geprüft werden.
    /// Gibt den vollständigen Pfad zur googleearth.exe zurück, falls gefunden, sonst einen leeren String.
    /// </summary>
    /// <returns>System.String.</returns>
    public string GoogleInstallPath()
    {
        ProjectData.ClearProjectError();
        string DateiName = "";
        string text = RegKeyGoogleEarth + "\\autoupdate";
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text);
        if (registryKey != null)
        {
            DateiName = ((registryKey.GetValue("AppPath", "")) + ($"\\{GoogleEarthExe}")).AsString();
            registryKey.Close();
        }
        if (DateiName == "")
        {
            text = RegKeyGoogleEarth;
            registryKey = Registry.CurrentUser.OpenSubKey(text);
            if (registryKey != null)
            {
                DateiName = ((registryKey.GetValue("InstallLocation", "")) + ($"client\\{GoogleEarthExe}")).AsString();
                registryKey.Close();
            }
            if (DateiName == "client\\googleearth.exe")
            {
                DateiName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + $"\\Google\\Google Earth\\client\\{GoogleEarthExe}";
            }
        }
        return DateiName;
    }

    /// <summary>
    /// Offices the install path.
    /// </summary>
    /// <param name="nVersion">The n version.</param>
    /// <returns>System.String.</returns>
    public string OfficeInstallPath(IModul1.MSOfficeVersion nVersion)
    {
        string name = "SOFTWARE\\Microsoft\\Office\\" + ((int)nVersion).AsString() + ".0\\Common\\InstallRoot";
        string text = "";
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
        if (registryKey != null)
        {
            text = registryKey.GetValue("Path", "").AsString();
            if (text.Length == 0)
            {
                text = registryKey.GetValue("OfficeBin", "").AsString();
            }
        }
        return text;
    }

    /// <summary>
    /// Enumerates the mandants.
    /// </summary>
    /// <param name="drive">The drive.</param>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    public IEnumerable<string> EnumerateMandants(string drive)
    {
        string[] asIgnore = new[] { "INIT", "TEMP", "LIST", "TEMPOSB", "HILFE", "INTERAHN", "TESTDAT1" };
        foreach (var di in new DirectoryInfo(Path.Combine(drive, AppName)).EnumerateDirectories())
        {
            if (!asIgnore.Contains(di.Name.ToUpper()))
            {
                yield return di.Name;
            }
        }
    }

    private _Modul1()
    {

        // The global init
        GenFreeDir = $"C:\\{AppName}";
        IText = IoC.GetRequiredService<IApplUserTexts>();
        ProjectData = IoC.GetRequiredService<IProjectData>();
        Interaction = IoC.GetRequiredService<IInteraction>();
        Information = IoC.GetRequiredService<IVBInformation>();
        Strings = IoC.GetRequiredService<IStrings>();
        Conversions = IoC.GetRequiredService<IVBConversions>();
        User = IoC.GetRequiredService<IUserData>();
        System = IoC.GetRequiredService<ISystem>();
        Family = new CFamilyPersons();
        Person = new CPersonData();
    }
    /// <summary>
    /// Diese Methode öffnet und initialisiert die Mandantendateien, prüft Lizenzen, behandelt verschiedene Fehlerfälle
    /// und bereitet die Datenbanktabellen für die weitere Arbeit vor. Sie enthält umfangreiche Fehlerbehandlung und
    /// stellt sicher, dass alle benötigten Ressourcen und Einstellungen korrekt geladen werden.
    /// </summary>
    public void Dateienopen()
    {
        ProjectData.ClearProjectError();
        System.xDemo = false;

        var menue = Menue.Default;
        var mand = MainProject.Forms.Mand;

        CloseAllDatabases();
        Persistence.ReadStringsInit("Text.Dat", Te, false);

        string value = string.Empty;
        if (cMandDrive.DriveType == DriveType.CDRom)
        {
            HandleCdMandant(menue, ref value);
        }
        else
        {
            HandleWritableMandant(menue, mand, ref value);
        }

        PrepareMandantPath();
        Mandant = Path.Combine(Verz, "Gen_plusdaten.mdb");
        if (System.xDemo)
        {
            menue.btnEnterLizenz.Visible = true;
            User.Owner = "Demoversion";
        }

        Ubg = 0;
        Rech2();
        FileSystem.FileClose(6);

        if (Typ == DriveType.CDRom)
        {
            DataModul.DataOpenRO(Mandant);
            AfterMandantDatabaseOpened(false);
        }
        else
        {
            EnsurePictureDirectory();
            DataModul.DataOpen(Mandant);
            AfterMandantDatabaseOpened(true);
            // Check for religi field and handle schema migrations
            if (DBHelper.TableExists(DataModul.MandDB, nameof(dbTables.Ahnen)))
                DataModul.MandDB.TryExecute($"DROP Table {dbTables.Ahnen} ");
            if (DBHelper.TableExists(DataModul.MandDB, nameof(dbTables.Leer)))
                DataModul.MandDB.TryExecute($"DROP Table {dbTables.Leer}");
            if (!DBHelper.DbFieldExists(DataModul.MandDB, dbTables.Personen, "religi"))
            {
                Reli = true;
            }
        }
    }

    private static void CloseAllDatabases()
    {
        DataModul.MandDB?.Close();
        DataModul.TempDB?.Close();
        DataModul.DOSB?.Close();
        DataModul.DSB?.Close();
    }

    private void HandleCdMandant(Menue menue, ref string value)
    {
        value = Persistence.ReadStringProg("SDVIDF.Dat");
        if (!CheckLicenceCD(value))
        {
            _ = Interaction.MsgBox("Lizenz wurde manipuliert\nProgramm wird beendet");
            ProjectData.EndApp();
        }
        System.xDemo = false;
        menue.btnEnterLizenz.Visible = false;
        menue.btnCheckUpdate.Visible = false;
        menue.btnRemoteDiag.Visible = false;
        User.Owner = value.Substring(16);
    }

    private void HandleWritableMandant(Menue menue, Mand mand, ref string value)
    {
        EnsureMandantDirectoryExists(menue, mand);
        if (Verz.Length < 16)
        {
            Information.Err().Raise(54);
        }

        Verz1 = Verz.Left(System.VerSpecial == 1 ? 20 : 15);
        string mandantDrive = Verz.Left(2);

        value = string.Empty;
        Mandant = Path.Combine(Verz, "Gen_plusdaten.mdb");
        value = Persistence.ReadStringProg("IDF.Dat");
        ValidateHardDiskLicence(value);
        PrepareAddressFile(mandantDrive);
        InitializeOwnerFromAddress(menue, ref value);
    }

    private void EnsureMandantDirectoryExists(Menue menue, Mand mand)
    {
        if (Verz.Length <= 16)
        {
            Verz = string.Empty;
        }

        if (!Persistence.ExistFileMand("Gen_Plusdaten.mdb"))
        {
            Verz = GenFreeDir + "\\Testdat1";
            mand.BezMAND.Text = string.Empty;
            _ = Interaction.MsgBox("Der aktuelle Mandant ist nicht vorhanden.\nEs muss ein andere Mandant gewählt oder neu angelegt werden!");
            Persistence.WriteStringInit("gen-verz.ini", Verz);
            if (Verz.Right(1) != "\\")
            {
                Verz += "\\";
            }
            Mandant = Verz + "Gen_plusdaten.mdb";
            menue.btnMandants.PerformClick();
        }
    }

    private void ValidateHardDiskLicence(string value)
    {
        if (value.Substring(2, 1).ToUpper() != "Q")
        {
            FileSystem.FileClose(99);
            FileSystem.Kill(Verz1 + "IDF.Dat");
        }

        if (!CheckLicenceHD(value))
        {
            _ = Interaction.MsgBox("Lizenz wurde manipuliert\nProgramm wird beendet");
            ProjectData.EndApp();
        }

        FileSystem.FileClose(99);
    }

    private void PrepareAddressFile(string mandantDrive)
    {
        FileSystem.FileClose(99);
        if (cMandDrive.DriveType != DriveType.CDRom)
        {
            FileSystem.FileOpen(99, Path.Combine(GenFreeDir, "Adresse"), OpenMode.Append);
        }
        FileSystem.FileClose(99);
    }

    private void InitializeOwnerFromAddress(Menue menue, ref string value)
    {
        var address = new string[10];
        Persistence.ReadStringsProg("Adresse", address);

        value = string.Join(" ", address.AsSpan(0, 2).ToArray());
        string flags = address[8];

        if (value.Trim() == string.Empty)
        {
            value = "Adresse eingeben";
            menue.lblOwner.BackColor = Color.Red;
            menue.btnAddress.BackColor = Color.Pink;
            _ = Interaction.MsgBox("Bitte Adresse eingeben. Ohne Adresse ist eine Personalisierug von Ausdrucken und Gedcom-Ausgaben nicht möglich.");
        }

        User.Owner = value.Trim();
        menue.lblOwner.Text = value.Trim();
        menue.btnSendData.Visible = true;

        Programtesttemp = 0;
        if (flags.Contains("Test"))
        {
            Programtesttemp = 1;
        }
    }

    private void PrepareMandantPath()
    {
        if (Verz.Right(1) != "\\")
        {
            Verz += "\\";
        }
    }

    private void EnsurePictureDirectory()
    {
        if (!Directory.Exists(Verz + "Bilder"))
        {
            FileSystem.MkDir(Verz + "Bilder");
        }
    }

    private void AfterMandantDatabaseOpened(bool mandantWritable)
    {
        DataModul_TestAndChangeNamenLfdNrFieldsize();
        DataModul.DB_OFBTable = DataModul.MandDB.OpenRecordset(dbTables.IndNam, RecordsetTypeEnum.dbOpenTable);

        try
        {
            _ = DataModul.DT_DescendentTable.Fields["Gen"].AsString();
        }
        catch
        {
            // ignored – keep compatibility with original behaviour
        }

        DataModul.Link.DeleteInvalidPerson();

        if (mandantWritable)
        {
            DataModul.Person.AllSetEditDate();
            DataModul.Family.AllSetEditDate();
        }

        InitializeQuellenRepository();
    }

    private void InitializeQuellenRepository()
    {
        DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
        IRecordset dB_QuTable = DataModul.DB_QuTable;
        if (dB_QuTable.RecordCount <= 0)
        {
            return;
        }

        dB_QuTable.Index = "NR";
        dB_QuTable.MoveFirst();
        while (!dB_QuTable.EOF)
        {
            if (!dB_QuTable.NoMatch)
            {
                string quS6 = dB_QuTable.Fields[QuFields._6].AsString();
                if (quS6 != string.Empty)
                {
                    Nr = DataModul_Repository_ReadValue(quS6);
                    int quNr = dB_QuTable.Fields[QuFields._1].AsInt();
                    DataModul_Repository_AddRaw(quNr, Nr);
                }
            }
            dB_QuTable.MoveNext();
        }
        dB_QuTable.Close();
        DataModul.MandDB.TryExecute($"ALTER Table {dbTables.Quellen} DROP COLUMN 6;");
        DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
    }

    public void Dateienopen_old()
    {
        ProjectData.ClearProjectError();
        int num = default;
        int num2 = default;
        int num3 = default;
        bool flag = default;
        IEnumerator enumerator2 = default;
        object objectValue2 = default;
        IEnumerator enumerator = default;
        object objectValue = default;
        short num5 = default;
        int lErl = default;
        string text;
        int number;

        int num4;
        string text3;
        string text4;
        string Value;
        Value = "";
        ProjectData.ClearProjectError();
        num3 = 2;
        System.xDemo = false;

        Menue _Menue = Menue.Default;
        Mand _Mand = MainProject.Forms.Mand;

        DataModul.MandDB?.Close();
        DataModul.TempDB?.Close();
        DataModul.DOSB?.Close();
        DataModul.DSB?.Close();

        Persistence.ReadStringsInit("Text.Dat", Te, false);

        goto IL_0129;
    IL_0129: // <========== 3
             // <=================
        num = 20;
        lErl = 13;

        if (cMandDrive.DriveType == DriveType.CDRom)
        {
            Value = Persistence.ReadStringProg("SDVIDF.Dat");
            if (!CheckLicenceCD(Value))
            {
                _ = Interaction.MsgBox("Lizenz wurde manipuliert\nProgramm wird beendet");
                ProjectData.EndApp();
            }
            System.xDemo = false;
            _Menue.btnEnterLizenz.Visible = false;
            _Menue.btnCheckUpdate.Visible = false;
            _Menue.btnRemoteDiag.Visible = false;
            User.Owner = Value.Substring(16);
        }
        else
        {
            if (Verz.Length <= 16)
            {
                Verz = "";
            }
            if (!Persistence.ExistFileMand("Gen_Plusdaten.mdb"))
            {
                Verz = GenFreeDir + "\\Testdat1";
                _Mand.BezMAND.Text = "";
                _ = Interaction.MsgBox("Der aktuelle Mandant ist nicht vorhanden.\nEs muss ein andere Mandant gewählt oder neu angelegt werden!");
                Persistence.WriteStringInit("gen-verz.ini", Verz);
                if (Verz.Right(1) != "\\")
                {
                    Verz += "\\";
                }
                Mandant = Verz + "Gen_plusdaten.mdb";
                _Menue.btnMandants.PerformClick();
            }
            if (Verz.Length < 16)
            {
                Information.Err().Raise(54);
            }
            Verz1 = Verz.Left(15);
            if (System.VerSpecial == 1)
            {
                Verz1 = Verz.Left(20);
            }
            text3 = Verz.Left(2);
            ProjectData.ClearProjectError();
            num3 = 5;
            Value = "";
            lErl = 18;
            Mandant = Path.Combine(Verz, "Gen_plusdaten.mdb");
            Value = Persistence.ReadStringProg("IDF.Dat");
            text4 = Value.Substring(20, 4);
            if (Value.Substring(2, 1).ToUpper() != "Q")
            {
                FileSystem.FileClose(99);
                FileSystem.Kill(Verz1 + "IDF.Dat");
            }
            if (!CheckLicenceHD(Value))
            {
                _ = Interaction.MsgBox("Lizenz wurde manipuliert\nProgramm wird beendet");
                ProjectData.EndApp();
            }
            FileSystem.FileClose(99);
            ProjectData.ClearProjectError();
            num3 = 6;
            lErl = 89;
            FileSystem.FileClose(99);
            if (cMandDrive.DriveType != DriveType.CDRom)
            {
                FileSystem.FileOpen(99, Path.Combine(GenFreeDir, "Adresse"), OpenMode.Append);
            }
            FileSystem.FileClose(99);
            var address = new string[10];
            Persistence.ReadStringsProg("Adresse", address);

            Value = string.Join(" ", address.AsSpan(0, 2).ToArray());

            string @string = address[8];

            if (Value.Trim() == "")
            {
                Value = "Adresse eingeben";
                _Menue.lblOwner.BackColor = Color.Red;
                _Menue.btnAddress.BackColor = Color.Pink;
                _ = Interaction.MsgBox("Bitte Adresse eingeben. Ohne Adresse ist eine Personalisierug von Ausdrucken und Gedcom-Ausgaben nicht möglich.");
            }
            User.Owner = Value.Trim();
            _Menue.lblOwner.Text = Value.Trim();
            _Menue.btnSendData.Visible = true;

            if (@string.Contains("OE"))
            {
            }
            if (@string.Contains("JFR"))
            {
            }
            Programtesttemp = 0;
            if (@string.Contains("Test"))
            {
                Programtesttemp = 1;
            }
        }
        goto IL_L151;
    IL_L151: // <========== 5
        num = 151;
        lErl = 1;
        if (Verz.Right(1) != "\\")
        {
            Verz += "\\";
        }
        Mandant = Path.Combine(Verz, "Gen_plusdaten.mdb");
        if (System.xDemo)
        {
            _Menue.btnEnterLizenz.Visible = true;
            User.Owner = "Demoversion";
        }
        Ubg = 0;
        Rech2();
        ProjectData.ClearProjectError();
        num3 = 7;
        FileSystem.FileClose(6);
        if (Typ == DriveType.CDRom)
        {
            DataModul.DataOpenRO(Mandant);
            goto IL_2ade;
        }
        else
        {
            if (!Directory.Exists(Verz + "Bilder"))
                FileSystem.MkDir(Verz + "Bilder");
            DataModul.DataOpen(Mandant);
            /*
                                        DataModul.DOSB.Execute("Create Table "+nameof(dbTables.OrtSuch)+" (Name Text(100));");
                                        DataModul.DOSB.Execute("Alter table "+nameof(dbTables.OrtSuch)+" add Column PerNr Long;");
                                        DataModul.DOSB.Execute("CREATE  INDEX Ortnr ON "+nameof(dbTables.OrtSuch)+" (  [PerNr]);");
                                        DataModul.DOSB.Execute("CREATE  INDEX Ortsu ON "+nameof(dbTables.OrtSuch)+" (  [Name]);");
            */
            //DataModul.DSB = DataModul.DAODBEngine_definst.Workspaces[0].CreateDatabase(MandDir + "Such.mdb", ";LANGID=0x0409;CP=1252;COUNTRY=0");
            //DataModul.DSB = DataModul.DAODBEngine_definst.OpenDatabase(MandDir + "Such.mdb", false, false, "");
            /*
                                        DataModul.DSB.Execute("Create Table "+nameof(dbTables.Such)+" (Name Text(50));");
                                        DataModul.DSB.Execute("Alter Table "+nameof(dbTables.Such)+" add column K_Phon Text(60);");
                                        DataModul.DSB.Execute("Alter Table "+nameof(dbTables.Such)+" add column Leit Text(60);");
                                        DataModul.DSB.Execute("Alter Table "+nameof(dbTables.Such)+" add column Alias Text(60);");
                                        DataModul.DSB.Execute("Alter Table "+nameof(dbTables.Such)+" add column Sound Text(60);");
                                        DataModul.DSB.Execute("Alter table "+nameof(dbTables.Such)+" add Column Datum Integer;");
                                        DataModul.DSB.Execute("Alter table "+nameof(dbTables.Such)+" add Column Nummer long;");
                                        DataModul.DSB.Execute("Alter table "+nameof(dbTables.Such)+" add Column iKenn Text (1);");
                                        DataModul.DSB.Execute("Alter table "+nameof(dbTables.Such)+" add Column Sich Text(1);");
                                        DataModul.DSB.Execute("CREATE UNIQUE INDEX Nummer ON "+nameof(dbTables.Such)+" ([Nummer]);");
                                        DataModul.DSB.Execute("CREATE  INDEX Namen ON "+nameof(dbTables.Such)+" ([Name]);");
                                        DataModul.DSB.Execute("CREATE  INDEX Persuch ON "+nameof(dbTables.Such)+" ([Name],[Datum]);");
                                        DataModul.DSB.Execute("CREATE  INDEX Soundsuch ON "+nameof(dbTables.Such)+" ([Sound],[Name],[Datum]);");
                                        DataModul.DSB.Execute("CREATE  INDEX K_Phonsuch ON "+nameof(dbTables.Such)+" ([K_Phon],[Name],[Datum]);");
                                        DataModul.DSB.Execute("CREATE  INDEX Aliassuch ON "+nameof(dbTables.Such)+" ([Alias],[Datum]);");
                                        DataModul.DSB.Execute("CREATE  INDEX Leitsuch ON "+nameof(dbTables.Such)+" ([Leit],[Datum]);");
            ///
                                        DataModul.TempDB.Execute("CREATE Table "+nameof(dbTables.Ahnen1)+" (Gene long)");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnen1)+" ADD COLUMN PerNr long;");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnen1)+" ADD COLUMN Weiter Text(1);");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnen1)+" ADD COLUMN EHE long;");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnen1)+" ADD COLUMN Ahn Text(40);");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnen1)+" ADD COLUMN Name Text(255);");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnen1)+" ADD COLUMN axSpitz Text (1);");
                                        DataModul.TempDB.Execute("CREATE INDEX axSpitz ON "+nameof(dbTables.Ahnen1)+" ([axSpitz],[Ahn]);");
                                        DataModul.TempDB.Execute("CREATE INDEX Namen ON "+nameof(dbTables.Ahnen1)+" ([Name],[Ahn]);");
                                        DataModul.TempDB.Execute("CREATE INDEX PerNr ON "+nameof(dbTables.Ahnen1)+" ([PerNr]);");
                                        DataModul.TempDB.Execute("CREATE INDEX Gene ON "+nameof(dbTables.Ahnen1)+" ([Gene]);");
                                        DataModul.TempDB.Execute("CREATE INDEX Ahnen ON "+nameof(dbTables.Ahnen1)+" ([Ahn]);");
                                        DataModul.TempDB.Execute("CREATE INDEX Implex ON "+nameof(dbTables.Ahnen1)+" ([PerNr],[Ahn]);");

                                        DataModul.TempDB.Execute("CREATE Table "+nameof(dbTables.Ahnew)+" (Ahn Text(40))");
                                        DataModul.TempDB.Execute("ALTER TABLE "+nameof(dbTables.Ahnew)+" ADD COLUMN PerNr long;");
                                        DataModul.TempDB.Execute("CREATE INDEX o04_Family.Kind ON "+nameof(dbTables.Ahnew)+" ([PerNr]);");

                                        DataModul.TempDB.Execute("DROP Table Konf");
                                        DataModul.TempDB.Execute("Create Table "+nameof(dbTables.Konf)+" (PerNr long)");
                                        DataModul.TempDB.Execute("Alter table "+nameof(dbTables.Konf)+" Add column Textnr long;");
                                        DataModul.TempDB.Execute("create Index T on "+nameof(dbTables.Konf)+" ([TextNr]);");

                                        DataModul.TempDB.Execute($"CREATE Table {dbTables.Nachk} (Gene integer,PerNr TEXT(240)Not NULL,Pr long,Weiter long,kia Text(2));");
                                        DataModul.TempDB.Execute("CREATE UNIQUE INDEX PerNr ON "+nameof(dbTables.Nachk)+" ([Pr]);");
                                        DataModul.TempDB.Execute("CREATE INDEX PerNr ON "+nameof(dbTables.Nachk)+" ([PerNr]);");
                                        DataModul.TempDB.Execute("CREATE INDEX GNr ON "+nameof(dbTables.Nachk)+" ([Gene],[PerNr]);");
                                        DataModul.TempDB.Execute("CREATE INDEX LNr ON "+nameof(dbTables.Nachk)+" ([Gene],[Weiter]);");
                                        lErl = 44;

                                        DataModul.DT_DescendentTable = DataModul.TempDB.OpenRecordset(dbTables.Nachk, RecordsetTypeEnum.dbOpenTable);
                                        DataModul.DSB_SearchTable = DataModul.DSB.OpenRecordset(dbTables.Such, RecordsetTypeEnum.dbOpenTable);
                                        DataModul.DataModul.DOSB_OrtSTable = DataModul.DOSB.OpenRecordset(dbTables.OrtSuch, RecordsetTypeEnum.dbOpenTable);
                                        DataModul.DT_RelgionTable = DataModul.TempDB.OpenRecordset(dbTables.Konf, RecordsetTypeEnum.dbOpenTable);
                                        DataModul.DT_AncesterTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnen1, RecordsetTypeEnum.dbOpenTable);
                                        DataModul.DT_KindAhnTable = DataModul.TempDB.OpenRecordset(dbTables.Ahnew, RecordsetTypeEnum.dbOpenTable);
                                        DataModul.DT_AncesterTable.Index = "PerNr";
                                        */
            num = 200;
            if (DBHelper.TableExists(DataModul.MandDB, nameof(dbTables.Ahnen)))
                DataModul.MandDB.TryExecute($"DROP Table {dbTables.Ahnen} ");
            ProjectData.ClearProjectError();
            num3 = 8;
            /*
                         DataModul.MandDB.Execute("Create Table "+nameof(dbTables.Leer1)+" (PerNr long)");
                         DataModul.MandDB.Execute("Alter table "+nameof(dbTables.Leer1)+"  add Column Modul1.Art Text(1);");
                         DataModul.MandDB.Execute("create Index Leer on "+nameof(dbTables.Leer1)+" (Modul1.Art)");
              */
            if (DBHelper.TableExists(DataModul.MandDB, nameof(dbTables.Leer)))
                DataModul.MandDB.TryExecute($"DROP Table {dbTables.Leer}");
            if (!DBHelper.DbFieldExists(DataModul.MandDB, dbTables.Personen, "religi"))
            {
                ProjectData.ClearProjectError();
                num3 = 9;
                Reli = true;
                lErl = 34;
            }
        }
        goto end_IL_0001_2;
        //=================

    IL_2ade: // <========== 3
             //<=============
        DataModul_TestAndChangeNamenLfdNrFieldsize();

        DataModul.DB_OFBTable = DataModul.MandDB.OpenRecordset(dbTables.IndNam, RecordsetTypeEnum.dbOpenTable);
        num = 600;
        try
        {
            Value = DataModul.DT_DescendentTable.Fields["Gen"].AsString();
        }
        catch
        {
        }
        DataModul.Link.DeleteInvalidPerson();
        num = 620;
        if (flag)
        {
            DataModul.Person.AllSetEditDate();
            DataModul.Family.AllSetEditDate();
        }
        ProjectData.ClearProjectError();
        num3 = 20;

        DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
        IRecordset dB_QuTable = DataModul.DB_QuTable;
        if (dB_QuTable.RecordCount > 0)
        {
            dB_QuTable.Index = "NR";
            dB_QuTable.MoveFirst();
            while (!dB_QuTable.EOF)
            {
                if (!dB_QuTable.NoMatch)
                {
                    string Qu_s6 = dB_QuTable.Fields[QuFields._6].AsString();
                    if (Qu_s6 != "")
                    {
                        Nr = DataModul_Repository_ReadValue(Qu_s6);
                        int num7 = dB_QuTable.Fields[QuFields._1].AsInt();
                        DataModul_Repository_AddRaw(num7, Nr);
                    }
                }
                dB_QuTable.MoveNext();
            }
            dB_QuTable.Close();
            DataModul.MandDB.TryExecute($"ALTER Table {dbTables.Quellen} DROP COLUMN 6;");
            DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
        }
    end_IL_0001_2: // <========== 5
        return;
    }

    private static bool CheckLicenceHD(string Value, int offs = 14)
    {
        int num8 = 0;
        for (int num5 = 0; num5 <= 11; num5++)
        {
            num8 += Value[offs + num5].AsInt() - ('0'.AsInt());
        }
        int num12 = (num8 - 1) / (Value[offs + 15].AsInt() - ('0'.AsInt()));
        return num12 == Value.Substring(offs + 13, 2).AsInt();
    }

    private static bool CheckLicenceCD(string Value)
    {
        // Prüfsumme über kompletten string
        int num8 = 0;
        int num5 = 1;
        while (num5 <= (Value.Length - 1))
        {
            num8 += (byte)Value[num5 - 1];
            num5++;
        }
        num8 += Value.Right(1).AsInt();
        if (num8 / 10.0 != Math.Floor(num8 / 10.0))
        {
            return false;
        }
        // Prüfsumme über die ersten 12 Zeichen
        num8 = 0;
        for (num5 = 0;
        num5 <= 11;
        num5++)
        {
            num8 += (int)Value[num5];
        }
        int num12 = (num8 - 1) / Value[15].AsInt();
        if (num12 != Value[14].AsInt())
        {
            return false;
        }
        return true;
    }

    private void DataModul_Repository_AddRaw(int num7, int Mr)
    {
        DataModul.DB_RepoTab.AddNew();
        DataModul.DB_RepoTab.Fields["Repo"].Value = Mr;
        DataModul.DB_RepoTab.Fields["Quelle"].Value = num7;
        DataModul.DB_RepoTab.Update();
    }

    private int DataModul_Repository_ReadValue(string Qu_s6)
    {
        IRecordset dB_RepoTable = DataModul.DB_RepoTable;
        dB_RepoTable.Index = "Name";
        dB_RepoTable.Seek("=", Qu_s6.Trim());
        var Nr = 0;
        if (!dB_RepoTable.NoMatch)
        {
            Nr = dB_RepoTable.Fields[RepoFields.Nr].AsInt();
        }
        else
        {
            Nr = DataModul_RepoTable_MaxID() + 1;
            DataModul.Repository_AppendRaw(Nr, [Qu_s6, "", "", "", "", "", "", ""]);
        }
        return Nr;
    }

    private static int DataModul_RepoTable_MaxID()
    {
        IRecordset dB_RepoTable = DataModul.DB_RepoTable;
        if (dB_RepoTable.RecordCount < 1)
            return 0;
        dB_RepoTable.Index = "Nr";
        dB_RepoTable.MoveLast();
        return dB_RepoTable.Fields[RepoFields.Nr].AsInt();
    }

    /// <summary>
    /// Erstellt einen eindeutigen Index "Zitat" auf das Feld 4 der Tabelle "Quellen".
    /// Falls das Feld leer ist, werden Platzhalterwerte eingefügt, um die Indexerstellung zu ermöglichen.
    /// </summary>
    private static void DataModul_Quellen_CreateZitatIndex()
    {
        DataModul.DB_QuTable?.Close();
        DataModul.MandDB.TryExecute($"CREATE INDEX {SourceIndex.Zitat} on {dbTables.Quellen}([{SourceFields._4.AsFld}]);");
        IRecordset dB_QuTable = DataModul.MandDB.OpenRecordset(dbTables.Quellen, RecordsetTypeEnum.dbOpenTable);
        dB_QuTable.Index = SourceIndex.Zitat.AsFld();
        dB_QuTable.Seek("=", "");
        var Value = 0;
        while (!dB_QuTable.EOF
            && !dB_QuTable.NoMatch
            && dB_QuTable.Fields[SourceFields._4].AsString().Trim() == "")
        {
            dB_QuTable.Edit();
            dB_QuTable.Fields[SourceFields._4].Value = $"Eingefügt wegen Indexerstellung {Value++}";
            dB_QuTable.Update();
            dB_QuTable.MoveNext();
        }
        dB_QuTable.Close();
    }

    /// <summary>
    /// Prüft, ob das Feld "LfNr" in der Tabelle "INamen" nur 1 Zeichen lang ist und erweitert es ggf. auf 2 Zeichen.
    /// Dazu wird ein temporäres Feld angelegt, die Werte übertragen, das alte Feld entfernt und durch ein neues Feld mit größerer Feldlänge ersetzt.
    /// Anschließend werden die Werte zurückkopiert und der temporäre Name entfernt. Der Index "Vollname" wird ebenfalls neu angelegt.
    /// </summary
    private static void DataModul_TestAndChangeNamenLfdNrFieldsize()
    {
        // Testen ob Weiter Feld in INamen Tabelle 1 Zeichen lang ist, dann auf 2 Zeichen ändern
        const string copyFld = "LFNR1"; // Name des neuen Feldes

        if (DataModul.DB_NameTable.Fields[NameFields.LfNr].Size == 1)
        {
            DataModul.DB_NameTable.Close();
            DataModul.wrkDefault.BeginTrans();
            DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.INamen} ADD COLUMN {copyFld} Text(2)NULL;");

            IRecordset dB_NameTable = DataModul.MandDB.OpenRecordset(dbTables.INamen, RecordsetTypeEnum.dbOpenTable);
            dB_NameTable.ForEachDo((f) => f[copyFld].Value = f[nameof(NameFields.LfNr)].AsInt());
            dB_NameTable.Close();

            DataModul.MandDB.TryExecute($"Drop INDEX {NameIndex.Vollname} on {dbTables.INamen}");
            DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.INamen} DROP COLUMN {NameFields.LfNr};");
            DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.INamen} ADD COLUMN {NameFields.LfNr} Text(2)NULL;");
            dB_NameTable = DataModul.MandDB.OpenRecordset(dbTables.INamen, RecordsetTypeEnum.dbOpenTable);
            dB_NameTable.ForEachDo((f) => f[nameof(NameFields.LfNr)].Value = f[copyFld].AsInt());
            dB_NameTable.Close();

            DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.INamen} DROP COLUMN {copyFld};");
            DataModul.MandDB.TryExecute($"CREATE UNIQUE INDEX {NameIndex.Vollname} ON {dbTables.INamen} (  [{NameFields.PersNr}],[{NameFields.Kennz}],[{NameFields.LfNr}],[{NameFields.Ruf}]);");
            DataModul.wrkDefault.CommitTrans();
            DataModul.DB_NameTable = DataModul.MandDB.OpenRecordset(dbTables.INamen, RecordsetTypeEnum.dbOpenTable);
        }
    }

    private void DataModul_TryRepairDB()
    {
        // Personen
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.religi} integer;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Such1} Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.OFB} Text(1)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Such2} Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Such3} Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Such4} Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Such5} Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Such6} Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.PUid} Text(40)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Bem2} Memo;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Personen} ADD COLUMN {PersonFields.Bem3} Memo;");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.reli} ON {dbTables.Personen} ([{PersonFields.religi}])WITH  IGNORE NULL;");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Such1} ON {dbTables.Personen} ([{PersonFields.Such1}],[{PersonFields.PersNr}])WITH  IGNORE NULL;");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Such2} ON {dbTables.Personen} ([{PersonFields.Such2}],[{PersonFields.PersNr}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Such3} ON {dbTables.Personen} ([{PersonFields.Such3}],[{PersonFields.PersNr}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Such4} ON {dbTables.Personen} ([{PersonFields.Such4}],[{PersonFields.PersNr}])WITH  IGNORE NULL;");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Such5} ON {dbTables.Personen} ([{PersonFields.Such5}],[{PersonFields.PersNr}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Such6} ON {dbTables.Personen} ([{PersonFields.Such6}],[{PersonFields.PersNr}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.Puid} ON {dbTables.Personen} ({PersonFields.PUid});");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.BeaDat} ON {dbTables.Personen} ([{PersonFields.EditDat}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {PersonIndex.PerNr} ON {dbTables.Personen} ([{PersonFields.PersNr}]);");
        // Familie
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.Eltern} single;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.Fuid} Text(40)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.Bem2} Memo;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.Bem3} Memo;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.Prae}  Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.Suf}  Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Familie} ADD COLUMN {FamilyFields.ggv}  Text(1);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {FamilyIndex.Fuid} ON {dbTables.Familie} ([{FamilyFields.Fuid}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {FamilyIndex.BeaDat} ON {dbTables.Familie} ([{FamilyFields.EditDat}]);");
        // Tab
        DataModul.MandDB.TryExecute($"CREATE INDEX {LinkIndex.ElSu} ON {dbTables.Tab} (  [{LinkFields.PerNr}],[{LinkFields.Kennz}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {LinkIndex.FamPruef} ON {dbTables.Tab} (  [{LinkFields.FamNr}],[{LinkFields.PerNr}],[{LinkFields.Kennz}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {LinkIndex.FamSu1} ON {dbTables.Tab} (  [{LinkFields.FamNr}],[{LinkFields.Kennz}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {LinkIndex.PAFI} ON {dbTables.Tab} (  [{LinkFields.Kennz}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {LinkIndex.Per} on {dbTables.Tab} ([{LinkFields.PerNr}]);");
        // Ereigniss
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.Bem3} Memo;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.Bem4} Memo;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.ArtText} Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.Zusatz} Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.DatumText} integer;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.priv} Text(1);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.Hausnr} integer;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.GrabNr} Integer;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.tot} Text(1)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.Causal} integer;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Ereignis} ADD COLUMN {EventFields.an} integer;");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.DatInd} ON {dbTables.Ereignis} ([{EventFields.DatumV}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.Datvs} ON {dbTables.Ereignis} ([{EventFields.DatumV_S}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.Datbs} ON {dbTables.Ereignis} ([{EventFields.DatumB_S}]);");
        if (DBHelper.DbFieldExists(DataModul.MandDB, dbTables.Ereignis, $"{EventFields.Hausnr1}"))
        {
            DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.HaNu} ON {dbTables.Ereignis} ([{EventFields.Hausnr1}]);");
        }
        else
        {
            DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.HaNu} ON {dbTables.Ereignis} ([{EventFields.Hausnr}]);");
        }
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.PText} ON {dbTables.Ereignis} ([{EventFields.Platz}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.NText} ON {dbTables.Ereignis} ([{EventFields.ArtText}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.CText} ON {dbTables.Ereignis} ([{EventFields.Causal}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.Reg1} ON {dbTables.Ereignis} ([{EventFields.Art}],[{EventFields.Reg}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.JaTa} ON {dbTables.Ereignis} ([{EventFields.Art}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {EventIndex.Reg} ON {dbTables.Ereignis} ([{EventFields.Reg}]);");

        // Bilder
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Bilder} ADD COLUMN {PictureFields.Format} Text(1)NULL;");
        DataModul.MandDB.TryExecute($"CREATE INDEX PerKenn ON {dbTables.Bilder} ([{PictureFields.Kennz}],[{PictureFields.ZuNr}]);");
        DataModul.MandDB.TryExecute($"CREATE UNIQUE INDEX Nr ON {dbTables.Bilder} ([{PictureFields.LfNr}]);");
        // Texte
        DataModul.MandDB.TryExecute($"CREATE INDEX {TexteIndex.STexte} ON {dbTables.Texte} (  [{TexteFields.Kennz}],[{TexteFields.Txt}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {TexteIndex.SSTexte} ON {dbTables.Texte} (  [{TexteFields.Txt}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {TexteIndex.LTexte} ON {dbTables.Texte} (  [{TexteFields.Leitname}],[{TexteFields.Kennz}]);");
        DataModul.MandDB.TryExecute($"CREATE UNIQUE INDEX {TexteIndex.TxNr1} ON {dbTables.Texte} ([{TexteFields.TxNr}]);");
        // Orte
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Orte} ADD COLUMN Zusatz Text(240)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Orte} ADD COLUMN L Text(10)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Orte} ADD COLUMN B Text(10)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Orte} ADD COLUMN GOV Text(20)NULL;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Orte} ADD COLUMN g single;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Orte} ADD COLUMN PolName long NULL;");
        DataModul.MandDB.TryExecute($"CREATE INDEX Pol ON {dbTables.Orte} ([Polname]);");
        // Quellen
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.Quellen} (1 Long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 2 Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 3 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 4 text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 5 text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 7 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 8 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 9 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 10 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 11 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 12 Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Quellen} ADD COLUMN 13 Memo");
        DataModul.MandDB.TryExecute($"CREATE INDEX Nr on {dbTables.Quellen}([1]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Nam on {dbTables.Quellen}([2]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Dopp on {dbTables.Quellen}([2],[4]);");
        DataModul.MandDB.TryExecute("DROP INDEX Zitat1 ON " + nameof(dbTables.Quellen) + ";");
        DataModul.MandDB.TryExecute($"CREATE unique INDEX ZITAT on {dbTables.Quellen}([4]) WITH DISALLOW NULL;");
        DataModul.MandDB.TryExecute($"CREATE  INDEX Autor on {dbTables.Quellen}([5]) ;");
        // Repo
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.Repo} (Nr Long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Name Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Strasse Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Ort text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN PLZ text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Fon text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Mail Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Http Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Bem Memo");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Repo} ADD COLUMN Suchname Text(240)");
        DataModul.MandDB.TryExecute($"CREATE INDEX Name on {dbTables.Repo}([SuchName]);");
        DataModul.MandDB.TryExecute($"CREATE unique INDEX Such on {dbTables.Repo}([SuchName],[Ort]) WITH DISALLOW NULL;");
        DataModul.MandDB.TryExecute($"CREATE INDEX Nr on {dbTables.Repo}([Nr]);");
        // RepoTab
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.RepoTab} (Quelle Long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.RepoTab} ADD COLUMN Repo Long");
        DataModul.MandDB.TryExecute($"CREATE INDEX Nr on {dbTables.RepoTab}([Quelle]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Leer on {dbTables.RepoTab}([Repo]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Dop on {dbTables.RepoTab}([Quelle],[Repo]);");
        // HGA
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.HGA} (Nr Long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Akte Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Kirchspiel Text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Beschr text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Flur text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Parzelle text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Hof text(240)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Brandkasse Memo");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.HGA} ADD COLUMN Bem Memo");
        DataModul.MandDB.TryExecute($"CREATE unique INDEX Akte on {dbTables.HGA}([Akte]) WITH DISALLOW NULL;");
        DataModul.MandDB.TryExecute($"CREATE unique INDEX Nr on {dbTables.HGA}([nr]);");
        // GBE
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.GBE} (Nr Long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.GBE} ADD COLUMN Akte Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.GBE} ADD COLUMN Jahr Text(24))");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.GBE} ADD COLUMN Name Memo");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.GBE} ADD COLUMN Geb Memo");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.GBE} ADD COLUMN Erb Text(24)");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.GBE} ADD COLUMN Abg Text(24)");
        DataModul.MandDB.TryExecute($"CREATE INDEX Akte on {dbTables.GBE}([Akte]);");
        DataModul.MandDB.TryExecute($"CREATE unique INDEX Nr on {dbTables.GBE}([nr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX AkteJa on {dbTables.GBE}([Akte],[Jahr]);");
        DataModul.DB_GbeTable = DataModul.MandDB.OpenRecordset(dbTables.GBE, RecordsetTypeEnum.dbOpenTable);
        /*
        DataModul.MandDB.Execute("CREATE Table "+nameof(dbTables.BesitzTab)+" (PerNr Long )");
        DataModul.MandDB.Execute("ALTER TABLE "+nameof(dbTables.Besitztab)+" ADD COLUMN Akte Text(240);");
        DataModul.MandDB.Execute("ALTER TABLE "+nameof(dbTables.Besitztab)+" ADD COLUMN PerNR long;");
        DataModul.MandDB.Execute("CREATE unique INDEX NuAkPer on "+nameof(dbTables.Besitztab)+"([PerNr],[Akte],[PerNR]);");
        DataModul.MandDB.Execute("CREATE INDEX Per on "+nameof(dbTables.Besitztab)+"([PerNR]);");
        DataModul.MandDB.Execute("CREATE INDEX Akt on "+nameof(dbTables.Besitztab)+"([Akte]);");

        DataModul.DB_PropertyTable = DataModul.MandDB.OpenRecordset(dbTables.Besitztab, RecordsetTypeEnum.dbOpenTable);
        */
        // Tab1
        DataModul.MandDB.TryExecute($"Drop INDEX {SourceLinkIndex.Tab1} on {dbTables.Tab1}");
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.Tab1} ({SourceLinkFields._1.AsFld()} single )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields._2.AsFld()} long;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields._3.AsFld()} long;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields._4.AsFld()} Text(240);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields.LfNr} single;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields.Art} single;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields.Aus} Text(240)Null;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields.Orig} Memo Null");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab1} ADD COLUMN {SourceLinkFields.Kom} Memo Null");
        DataModul.MandDB.TryExecute($"CREATE INDEX {SourceLinkIndex.Tab} ON {dbTables.Tab1} ([{SourceLinkFields._1.AsFld()}],[{SourceLinkFields._2.AsFld()}]);");
        DataModul.MandDB.TryExecute($"CREATE  INDEX {SourceLinkIndex.Tab21} ON {dbTables.Tab1} ([{SourceLinkFields._1.AsFld()}],[{SourceLinkFields._2.AsFld()}],[{SourceLinkFields._3.AsFld()}]);");
        DataModul.MandDB.TryExecute($"CREATE  INDEX {SourceLinkIndex.Tab22} ON {dbTables.Tab1} ([{SourceLinkFields._1.AsFld()}],[{SourceLinkFields._2.AsFld()}],[Art],[LfNr]);");
        DataModul.MandDB.TryExecute($"CREATE  INDEX {SourceLinkIndex.Tab23} ON {dbTables.Tab1} ([{SourceLinkFields._1.AsFld()}],[{SourceLinkFields._2.AsFld()}],[{SourceLinkFields._3.AsFld()}],[Art],[LfNr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX {SourceLinkIndex.Verw} ON {dbTables.Tab1} ([{SourceLinkFields._3.AsFld()}],[{SourceLinkFields._1.AsFld()}]);");
        // IndNam
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.IndNam} ([PerNr] Long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.IndNam} ADD COLUMN [Kennz] Text(2);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.IndNam} ADD COLUMN [Textnr] long;");
        DataModul.MandDB.TryExecute($"CREATE INDEX Indnum ON {dbTables.IndNam} ([TextNr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Indn ON {dbTables.IndNam} ([PerNr],[Kennz],[TextNr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Indnr ON {dbTables.IndNam} ([PerNr],[Kennz]);");
        // INamen
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.INamen} ADD COLUMN [Spitz] Byte Null;");
        DataModul.MandDB.TryExecute($"CREATE INDEX NamKenn ON {dbTables.INamen} ([PersNr],[Kennz]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Vollname ON {dbTables.INamen} ([PersNr],[Kennz],[LfNr],[axRuf]);");
        // Tab2
        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.Tab2} ({WitnessFields.FamNr} long )");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab2} ADD COLUMN {WitnessFields.PerNr} long;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab2} ADD COLUMN {WitnessFields.Kennz} Text(2);");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab2} ADD COLUMN {WitnessFields.Art} Single;");
        DataModul.MandDB.TryExecute($"ALTER TABLE {dbTables.Tab2} ADD COLUMN {WitnessFields.LfNr} Single;");
        DataModul.MandDB.TryExecute($"CREATE INDEX FamPruef ON {dbTables.Tab2} ([{WitnessFields.FamNr}],[{WitnessFields.PerNr}],[Kennz],[Art],[LfNr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX Zeug ON {dbTables.Tab2} ([{WitnessFields.FamNr}],[{WitnessFields.PerNr}],[Kennz],[Art],[LfNr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX ZeugSu ON {dbTables.Tab2} ([{WitnessFields.FamNr}],[{WitnessFields.Kennz}],[Art],[LfNr]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX FamSu ON {dbTables.Tab2} ([{WitnessFields.FamNr}],[{WitnessFields.Kennz}]);");
        DataModul.MandDB.TryExecute($"CREATE INDEX ElSu ON {dbTables.Tab2} ([{WitnessFields.PerNr}],[{WitnessFields.Kennz}]);");

        DataModul.MandDB.TryExecute($"CREATE Table {dbTables.WDBL}(Nr Byte)");

        DataModul.DB_WDTable = DataModul.MandDB.OpenRecordset(dbTables.WDBL, RecordsetTypeEnum.dbOpenTable);
        if (DataModul.DB_WDTable.RecordCount == 0)
        {
            DataModul.DB_WDTable.AddNew();
            DataModul.DB_WDTable.Fields[WDFields.Nr].Value = 0;
            DataModul.DB_WDTable.Update();
        }

    }


    public IRecordset DataModul_OpenRecordSet(dbTables dbTables)
    {
        IRecordset instance;
        DataModul.DB_QuTable = DataModul.MandDB.OpenRecordset(dbTables);
        instance = DataModul.DB_QuTable;
        return instance;
    }


    /// <summary>
    /// Stores a Text, And conditionally puts it to a Person .
    /// </summary>
    /// <param name="sText">The s text.</param>
    /// <param name="sLeitName">Name of the s leit.</param>
    /// <param name="eTKennz">The e t kennz.</param>
    /// <param name="PersInArb">The pers in arb.</param>
    /// <param name="LfNR">The lf Mr.</param>
    /// <returns>System.Int32.</returns>
    public int TextSpeich(string sText, string sLeitName, ETextKennz eTKennz, int PersInArb = 0, int LfNR = 0, bool xCalln = false, bool xNickn = false)
    {
        ETextKennz[] aePersonTexts = [
            ETextKennz.A_,
            ETextKennz.B_,
            ETextKennz.C_,
            ETextKennz.D_,
            ETextKennz.tkName,
            ETextKennz.F_,
            ETextKennz.U_,
            ETextKennz.V_,
            ETextKennz.tk2_ ];

        ETextKennz[] aeXXTexts =
        [
            ETextKennz.E_,
            ETextKennz.G_,
            ETextKennz.H_,
            ETextKennz.I_,
            ETextKennz.J_,
            ETextKennz.K_,
            ETextKennz.L_,
            ETextKennz.M_,
            ETextKennz.O_,
            ETextKennz.Q_,
            ETextKennz.R_,
            ETextKennz.S_,
            ETextKennz.T_,
            ETextKennz.Z_,
            ETextKennz.tk1_,
            ETextKennz.tk3_,
            ETextKennz.tk4_,
            ETextKennz.tk5_,
            ETextKennz.tk6_
        ];

        var Satz = DataModul.Texte_Schreib(sText, sLeitName, eTKennz);


        if (aeXXTexts.Contains(eTKennz)
            || (aePersonTexts.Contains(eTKennz) && (PersInArb == 0)))
            return Satz;
        else
            if (!aeXXTexts.Contains(eTKennz) && !aePersonTexts.Contains(eTKennz))
            {
                _ = Interaction.MsgBox("F3");
                Debugger.Break();
            }

        DataModul_Namen_Update(sText, eTKennz, PersInArb, LfNR, Satz, xCalln, xNickn);
        return Satz;
    }

    private static void DataModul_Namen_Update(string sText, ETextKennz eTKennz, int PersInArb, int LfNR, int Satz, bool xCalln, bool xNickn)
    {
        if (sText != "")
        {
            DataModul.Names.Update(PersInArb, Satz, eTKennz, LfNR % 15, xCalln, xNickn);
        }
        else
        {
            DataModul.Names.DeleteNK(PersInArb, eTKennz);
        }
    }

    public void STextles(string Formnam, ETextKennz Kennz, string UbgT, IList ocIItems)
    {

        //_ = Interaction.MsgBox("!" + UbgT, title: "STextles" + Formnam, mb: MessageBoxButtons.OKCancel);
        //if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
        //{
        //    ProjectData.EndApp();
        //}
        //ProjectData.ClearProjectError();
        //if (num2 == 0)
        //{
        //    throw ProjectData.CreateProjectError(ErrUser14);
        //}
        //num7 = num2;
        var num9 = 0;
        IRecordset dB_TexteTable = DataModul.DB_TexteTable;
        ETextKennz eTextKennz;
        dB_TexteTable.Index = nameof(TexteIndex.STexte);
        dB_TexteTable.Seek(">=", (char)Kennz, UbgT);
        while (!dB_TexteTable.EOF
               && !dB_TexteTable.NoMatch
               && (eTextKennz = dB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>()) == Kennz
               && num9++ < 200)
        {
            string Texte_sTxt = dB_TexteTable.Fields[TexteFields.Txt].AsString();
            int Texte_iTxNr = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
            string text2 = Texte_sTxt.Replace("ssss", "ß");
            int num10;
            var ocItems = ocIItems as IList<ListItem<int>>;

            int num8;
            switch (Formnam)
            {
                case "aiVorn":
                    ocItems.Add(new($"{text2.Trim(),240}W{Texte_iTxNr,10}", Texte_iTxNr));
                    break;
                case "Ortsverw.List1":
                    ocItems.Add(new($"{text2.Trim(),240}{Texte_iTxNr,10}", Texte_iTxNr));
                    break;
                case "Personen.List3":
                    ocItems.Add(new($"{text2.Trim(),240}{Texte_iTxNr,10}", Texte_iTxNr));
                    break;
                case $"{nameof(Familie)}.{nameof(Familie.ListBox3)}":

                    ocItems.Add(new($"{text2.Trim(),240}{Texte_iTxNr,10}", Texte_iTxNr));
                    break;
                case "Ereignis.Liste2":
                    ocItems.Add(new($"{text2.Trim(),240}{Texte_iTxNr,10}", Texte_iTxNr));
                    break;
                case "OFBL1":

                    ocItems.Add(new(text2.Trim(), Texte_iTxNr));
                    break;
                case "OFBL2":

                    ocItems.Add(new(text2.Trim(), Texte_iTxNr));
                    break;
                case "OFBL3":

                    ocItems.Add(new(text2.Trim(), Texte_iTxNr));
                    break;
                case "TL5":

                    if (Texte_sTxt.ToUpper().Left(1) != UbgT.ToUpper().Left(1))
                    {
                        goto end_IL_0001_2;
                    }

                    LiText = "";
                    if (MainProject.Forms.Textlesen.Text4.Text.Trim() == "")
                    {
                        LiText = $"{text2.Trim(),240}{Texte_sTxt.TrimStart()}";

                    }
                    else if (Texte_sTxt.ToUpper().Contains(MainProject.Forms.Textlesen.Text4.Text.ToUpper()))
                    {
                        LiText = $"{Texte_sTxt.TrimStart(),240}{Texte_iTxNr}";
                    }

                    if (LiText.Trim() != "")
                    {
                        ocItems.Add(new(LiText, Texte_iTxNr));
                        LiText = "";
                    }
                    //     goto IL_1402;

                    break;
                case "Partnerrecherche.List2":

                    ocItems.Add(new($"{text2.TrimStart(),240}{Texte_iTxNr}", Texte_iTxNr));
                    //   goto IL_1402;

                    break;
                case "Ortsteil":

                    int[] array7 = new int[122];
                    num10 = 1;
                    while (num10 <= 120
                        && !(eTextKennz != ETextKennz.I_))
                    {
                        array7[num10] = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                        dB_TexteTable.MoveNext();
                        num10++;
                    }

                    int num13 = num10 - 1;
                    num8 = 1;
                    while (num8 <= num13)
                    {
                        foreach (var cPlace in DataModul.Place.ReadAll(PlaceIndex.OT, array7[num8]))
                        {
                            if ((cPlace.sOrtsteil + cPlace.sOrt).Trim().Length > 0)
                            {
                                LiText = $"{cPlace.sOrtsteil}-{cPlace.sOrt}                                          {cPlace.ID.AsString()}";
                                ocItems.Add(new ListItem<int>(LiText, cPlace.ID));
                                LiText = "";
                            }
                        }
                        num8++;
                    }
                    goto end_IL_0001_2;

                //    break;
                case "Ausland":
                    int[] array8 = new int[122];
                    num10 = 1;
                    while (num10 <= 120
                        && !(eTextKennz != ETextKennz.S_))
                    {
                        array8[num10] = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
                        dB_TexteTable.MoveNext();
                        num10++;
                    }

                    int num18 = num10 - 1;
                    num8 = 1;
                    while (num8 <= num18)
                    {
                        foreach (var cPlace in DataModul.Place.ReadAll(PlaceIndex.Pol, array8[num8]))
                        {
                            dB_TexteTable.Index = nameof(TexteIndex.TxNr1);
                            if ((cPlace.sPolName + cPlace.sOrt).Trim().Length > 0)
                            {
                                LiText = $"{cPlace.sPolName}-{cPlace.sOrt}                                          {cPlace.ID.AsString()}";
                                ocItems.Add(new ListItem<int>(LiText, cPlace.ID));
                                LiText = "";
                                //=================
                            }
                        }
                        num8++;
                        //=================
                    }

                    goto end_IL_0001_2;

                //        break;
                case "Rechtext.Liste1":

                    if (eTextKennz != Kennz || text2.ToUpper().Left(1) != UbgT.ToUpper().Left(1))
                    {
                        goto end_IL_0001_2;
                    }
                    var text = dB_TexteTable.Fields[TexteFields.Txt].AsString();
                    LiText = text2.TrimStart() + new string(' ', 240).Left(240) + Strings.LTrim(dB_TexteTable.Fields[TexteFields.TxNr].AsString());
                    ocItems.Add(new(LiText, Texte_iTxNr));
                    //=================


                    break;
                case nameof(Namensuch):
                    ocItems.Add(new($"{text2.Trim(),240}{Texte_iTxNr,10}", Texte_iTxNr));
                    //=================

                    break;
            }
            dB_TexteTable.MoveNext();
        }
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 11
        return;
    }

    public string Ancester_GetAncesterData(int iAnc)
    {
        var (iPerson, iAhn, iWeiter, iGen) = DataModul.Ancester_GetAncData(iAnc);
        string Kont10 = Kont[10];
        Kont10 = "";
        if (iPerson != 0)
        {
            if (iWeiter != 0)
            {
                Kont[20] = ">>";
            }
            Kont[11] = $"{iAhn}";
            Kont10 = $"{IText[EUserText.t123]} {iGen} {IText[EUserText.t124]} {iAhn}";
        }
        return Kont10;
    }


    public void Person_ReadNames(int PersonNr, IPersonData Person)
    {
        if (PersonNr == 0 && Person.ID > 0)
        {
            PersonNr = Person.ID;
        }
        else
        {
            Person.SetPersonNr(PersonNr);
        }

        _ = DataModul.Names.ReadPersonNames(PersonNr, out var iName, out var aiVorns);

        Person.SetPersonNames(iName, aiVorns, Aus[(int)EOutCfg.o20] == "Y");
    }


    public string Ancesters_GetPersonData(int PersonNr, out int Ahnsp, out string Kont20)
    {
        Kont20 = default;
        string Kont10 = default;
        Ahnsp = 0;

        foreach (var Anc in DataModul.Ancester_EmitPersonData(PersonNr))
        {
            if (Anc.iAhn > 0)
            {
                if (Anc.iWeiter != 0)
                {
                    Kont20 = ">>";
                }

                Kont10 = $"{IText[EUserText.t123]} {Anc.iGen} {IText[EUserText.t124]} {Anc.iAhn}";
            }

            if (Ahnsp == 0
                || Anc.iAhn < Ahnsp
                || Ahnsp > 0
                && Anc.iAhn < Ahnsp)
            {
                Ahnsp = Anc.iAhn;
            }
        }

        Kont[20] = Kont20;
        return Kont10;
    }

    //public  void Famberufles_()
    //{
    //    //Discarded unreachable code: IL_06ae, IL_081b
    //    int try0001_dispatch = -1;
    //    int num = _Menue;
    //    int num2 = _Menue;
    //    int num3 = _Menue;
    //    int number = _Menue;
    //    int lErl = _Menue;
    //    string prompt = _Menue;
    //    int iMaxFamNr = _Menue;
    //    float num8 = _Menue;
    //    while (true)
    //    {
    //        try
    //        {
    //            /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
    //            ;

    //            checked
    //            {
    //                int num4;
    //                int num6;
    //                int num7;
    //                float num9;
    //                float num10;
    //                string LD;
    //                int AAA;
    //                short Schalt;
    //                switch (try0001_dispatch)
    //                {
    //                    _Menue:
    //                        num = 1;
    //                        int eArt = Ubg;
    //                        goto IL_L002;

    //                    case 2481:
    //                        {
    //                            num2 = num;
    //                            switch ((num3 <= -2) ? 1 : num3)
    //                            {
    //                                case 2:
    //                                    break;
    //                                case 1:
    //                                    goto IL_081f;
    //                                //=================
    //                                _Menue:
    //                                    goto end_IL_0001;
    //                            }

    //                            goto IL_06b0;
    //                        }
    //                    IL_0621:
    //                        num = 62;
    //                        o01_Person.Stat = Kont1[7];
    //                        goto IL_0634;
    //                    IL_0634:
    //                        num = 63;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
    //                        goto end_IL_0001_2;
    //                    //=================
    //                    IL_05f3:
    //                        num = 61;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.Reg].Value !=  " "))
    //                        {
    //                            goto IL_0621;
    //                        }
    //                        else
    //                            goto IL_064e;
    //                        //=================
    //                        IL_06b0:
    //                        num = 73;
    //                        number = Information.Err().Number;
    //                        goto IL_06c1;
    //                    IL_06c1:
    //                        num = 76;
    //                        if (number == 94)
    //                        {
    //                            goto IL_06d2;
    //                        }
    //                        else
    //                            goto IL_06f4;
    //                        IL_06f4:
    //                        num = 80;
    //                        if (number == 3021)
    //                        {
    //                            goto IL_0708;
    //                        }
    //                        else
    //                            goto IL_0728;
    //                        IL_0728:
    //                        num = 85;
    //                        if (Information.Err().Number != 0)
    //                        {
    //                            goto IL_0742;
    //                        }
    //                        else
    //                            goto IL_07bd;
    //                        IL_07bd:
    //                        num = 89;
    //                        Interaction.MsgBox("F13");
    //                        goto IL_07ce;
    //                    IL_07ce:
    //                        num = 90;
    //                        if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, (Information.Err().Number).AsString()) == DialogResult.Cancel)
    //                        {
    //                            ProjectData.EndApp();
    //                        }
    //                        goto IL_07fb;
    //                    //=================
    //                    IL_0663:
    //                        num = 68;
    //                        lErl = 2;
    //                        goto IL_066a;
    //                    //=================
    //                    IL_07fb:
    //                        num = 93;
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(ErrUser14);
    //                        }
    //                        goto IL_081f;
    //                    //=================
    //                    IL_066a:
    //                        num = 69;
    //                        DataModul.DB_EventTable.MoveNext();
    //                        goto IL_067a;
    //                    //=================
    //                    IL_081f:
    //                        num4 = unchecked(num2 + 1);
    //                        num2 = 0;
    //                        switch (num4)
    //                        {
    //                            case 1:
    //                                break;
    //                            case 2:
    //                                goto IL_L002;
    //                            case 3:
    //                                goto IL_0013;
    //                            case 4:
    //                                goto IL_0023;
    //                            case 5:
    //                                goto IL_0034;
    //                            case 6:
    //                                goto IL_003c;
    //                            case 7:
    //                                goto IL_004f;
    //                            case 10:
    //                                goto IL_00bc;
    //                            case 12:
    //                            case 13:
    //                                goto IL_00d6;
    //                            case 14:
    //                                goto IL_00e4;
    //                            case 15:
    //                                goto IL_00ee;
    //                            case 16:
    //                                goto IL_0106;
    //                            case 17:
    //                                goto IL_011f;
    //                            case 18:
    //                                goto IL_0129;
    //                            case 19:
    //                                goto IL_0137;
    //                            case 21:
    //                            case 22:
    //                                goto IL_0170;
    //                            case 23:
    //                                goto IL_01bc;
    //                            case 25:
    //                            case 26:
    //                                goto IL_01d6;
    //                            case 27:
    //                                goto IL_0206;
    //                            case 28:
    //                                goto IL_0249;
    //                            case 29:
    //                                goto IL_026a;
    //                            case 30:
    //                            case 31:
    //                                goto IL_0279;
    //                            case 32:
    //                                goto IL_02a8;
    //                            case 33:
    //                                goto IL_02eb;
    //                            case 34:
    //                                goto IL_0310;
    //                            case 35:
    //                            case 36:
    //                                goto IL_031f;
    //                            case 37:
    //                                goto IL_0352;
    //                            case 38:
    //                                goto IL_0390;
    //                            case 39:
    //                                goto IL_03d3;
    //                            case 40:
    //                                goto IL_03ff;
    //                            case 41:
    //                            case 42:
    //                            case 43:
    //                                goto IL_040f;
    //                            case 44:
    //                                goto IL_044a;
    //                            case 45:
    //                                goto IL_045b;
    //                            case 46:
    //                                goto IL_0496;
    //                            case 47:
    //                                goto IL_04d4;
    //                            case 48:
    //                            case 49:
    //                                goto IL_0514;
    //                            case 50:
    //                                goto IL_0534;
    //                            case 51:
    //                            case 52:
    //                                goto IL_0543;
    //                            case 53:
    //                                goto IL_055c;
    //                            case 54:
    //                                goto IL_058a;
    //                            case 55:
    //                                goto IL_05ae;
    //                            case 57:
    //                            case 58:
    //                                goto IL_05c8;
    //                            case 60:
    //                                goto IL_05ee;
    //                            case 61:
    //                                goto IL_05f3;
    //                            case 62:
    //                                goto IL_0621;
    //                            case 63:
    //                                goto IL_0634;
    //                            case 65:
    //                            case 66:
    //                                goto IL_064e;
    //                            case 59:
    //                            case 67:
    //                            case 68:
    //                                goto IL_0663;
    //                            case 69:
    //                                goto IL_066a;
    //                            case 8:
    //                            case 9:
    //                            case 70:
    //                                goto IL_067a;
    //                            case 20:
    //                            case 71:
    //                                goto IL_0694;
    //                            case 73:
    //                                goto IL_06b0;
    //                            case 75:
    //                            case 76:
    //                                goto IL_06c1;
    //                            case 77:
    //                                goto IL_06d2;
    //                            case 80:
    //                                goto IL_06f4;
    //                            case 81:
    //                                goto IL_0708;
    //                            case 84:
    //                            case 85:
    //                                goto IL_0728;
    //                            case 86:
    //                                goto IL_0742;
    //                            case 87:
    //                                goto IL_07ab;
    //                            case 88:
    //                            case 89:
    //                                goto IL_07bd;
    //                            case 90:
    //                                goto IL_07ce;
    //                            case 91:
    //                            case 93:
    //                                goto IL_07fb;
    //                            //=================
    //                            _Menue:
    //                                goto end_IL_0001;
    //                            case 11:
    //                            case 24:
    //                            case 56:
    //                            case 64:
    //                            case 72:
    //                            case 74:
    //                            case 78:
    //                            case 79:
    //                            case 82:
    //                            case 83:
    //                            case 94:
    //                            case 95:
    //                            case 96:
    //                                goto end_IL_0001_2;
    //                        }

    //                        goto _Menue;
    //                    //=================
    //                    IL_0742:
    //                        num = 86;
    //                        prompt = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
    //                        goto IL_07ab;
    //                    IL_07ab:
    //                        num = 87;
    //                        Interaction.MsgBox(prompt, MessageBoxButtons.OK, "Fehler");
    //                        goto IL_07bd;
    //                    //=================
    //                    IL_0708:
    //                        num = 81;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
    //                        goto end_IL_0001_2;
    //                    //=================
    //                    IL_06d2:
    //                        num = 77;
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(ErrUser14);
    //                        }
    //                        goto IL_081f;
    //                    //=================
    //                    IL_064e:
    //                        num = 66;
    //                        o01_Person.Stat = Kont1[7];
    //                        goto IL_0663;
    //                    //=================
    //                    IL_L002:
    //                        num = 2;
    //                        int eArt = Ubg;
    //                        goto IL_L002;

    //                    IL_0013:
    //                        num = 3;
    //                        Kont[iMaxFamNr] = "";
    //                        goto IL_0023;
    //                    IL_0023:
    //                        num = 4;
    //                        iMaxFamNr++;
    //                        num6 = iMaxFamNr;
    //                        num7 = 10;
    //                        if (num6 <= num7)
    //                        {
    //                            goto IL_0013;
    //                        }
    //                        else
    //                            goto IL_0034;
    //                        IL_0034:
    //                        ProjectData.ClearProjectError();
    //                        num3 = 2;
    //                        goto IL_003c;
    //                    IL_003c:
    //                        num = 6;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.BeSu);
    //                        goto IL_004f;
    //                    IL_004f:
    //                        num = 7;
    //                        DataModul.DB_EventTable.Seek("=", eArt.AsString(), PersInArb.AsString());
    //                        goto IL_067a;
    //                    IL_067a:
    //                        num = 9;
    //                        if (!DataModul.DB_EventTable.EOF)
    //                        {
    //                            goto IL_00bc;
    //                        }
    //                        else
    //                            goto IL_0694;
    //                        //=================
    //                        IL_00bc:
    //                        num = 10;
    //                        if (DataModul.DB_EventTable.NoMatch)
    //                        {
    //                            goto end_IL_0001_2;
    //                        }
    //                        else
    //                            goto IL_00d6;
    //                        IL_00d6:
    //                        num = 13;
    //                        sPlace = "";
    //                        goto IL_00e4;
    //                    IL_00e4:
    //                        num = 14;
    //                        num8 = 0f;
    //                        goto IL_00ee;
    //                    IL_00ee:
    //                        num = 15;
    //                        Kont1[(int)Math.Round(num8)] = "";
    //                        goto IL_0106;
    //                    IL_0106:
    //                        num = 16;
    //                        num8 += 1f;
    //                        num9 = num8;
    //                        num10 = 15f;
    //                        if (num9 <= num10)
    //                        {
    //                            goto IL_00ee;
    //                        }
    //                        else
    //                            goto IL_011f;
    //                        IL_011f:
    //                        num = 17;
    //                        Ubg = iMaxFamNr;
    //                        goto IL_0129;
    //                    IL_0129:
    //                        num = 18;
    //                        Datu = "";
    //                        goto IL_0137;
    //                    IL_0137:
    //                        num = 19;
    //                        if (!(DataModul.DB_EventTable.Fields[EventFields.Modul1.Art].AsInt() !=  eArt))
    //                        {
    //                            goto IL_0170;
    //                        }
    //                        else
    //                            goto IL_0694;
    //                        IL_0694:
    //                        num = 71;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
    //                        goto end_IL_0001_2;
    //                    //=================
    //                    IL_0170:
    //                        num = 22;
    //                        if (Conversions.ToBoolean(Operators.OrObject(DataModul.DB_EventTable.NoMatch, (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() !=  PersInArb))))
    //                        {
    //                            goto IL_01bc;
    //                        }
    //                        else
    //                            goto IL_01d6;
    //                        //=================
    //                        IL_01bc:
    //                        num = 23;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
    //                        goto end_IL_0001_2;
    //                    //=================
    //                    IL_01d6:
    //                        num = 26;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.DatumText].Value)
    //                        {
    //                            goto IL_0206;
    //                        }
    //                        else
    //                            goto IL_0279;
    //                        //=================
    //                        IL_0206:
    //                        num = 27;
    //                        AAA = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt();
    //                        LD = "";
    //                        KontU = DataModul.TextLese1(AAA);
    //                        goto IL_0249;
    //                    IL_0249:
    //                        num = 28;
    //                        Kont1[14] = " " + KontU + " ";
    //                        goto IL_026a;
    //                    IL_026a:
    //                        num = 29;
    //                        KontU = "";
    //                        goto IL_0279;
    //                    IL_0279:
    //                        num = 31;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt() >  0))
    //                        {
    //                            goto IL_02a8;
    //                        }
    //                        else
    //                            goto IL_031f;
    //                        //=================
    //                        IL_02a8:
    //                        num = 32;
    //                        AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
    //                        LD = "";
    //                        KontU = DataModul.TextLese1(AAA);
    //                        goto IL_02eb;
    //                    IL_02eb:
    //                        num = 33;
    //                        Kont1[7] = " " + KontU.Trim() + " ";
    //                        goto IL_0310;
    //                    IL_0310:
    //                        num = 34;
    //                        KontU = "";
    //                        goto IL_031f;
    //                    IL_031f:
    //                        num = 36;
    //                        if (null != DataModul.DB_EventTable.Fields[EventFields.GedAus.Hausnr].Value)
    //                        {
    //                            goto IL_0352;
    //                        }
    //                        else
    //                            goto IL_040f;
    //                        //=================
    //                        IL_0352:
    //                        num = 37;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.GedAus.Hausnr].AsString()) != "")
    //                        {
    //                            goto IL_0390;
    //                        }
    //                        else
    //                            goto IL_040f;
    //                        //=================
    //                        IL_0390:
    //                        num = 38;
    //                        AAA = DataModul.DB_EventTable.Fields[EventFields.GedAus.Hausnr].AsInt();
    //                        LD = "";
    //                        KontU = DataModul.TextLese1(AAA);
    //                        goto IL_03d3;
    //                    IL_03d3:
    //                        num = 39;
    //                        Kont1[7] = Kont1[7] + " " + KontU.Trim() + " ";
    //                        goto IL_03ff;
    //                    IL_03ff:
    //                        num = 40;
    //                        KontU = "";
    //                        goto IL_040f;
    //                    IL_040f:
    //                        num = 43;
    //                        if (Conversions.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()) > 0.0)
    //                        {
    //                            goto IL_044a;
    //                        }
    //                        else
    //                            goto IL_0543;
    //                        //=================
    //                        IL_044a:
    //                        num = 44;
    //                        Kont2[6] = "";
    //                        goto IL_045b;
    //                    IL_045b:
    //                        num = 45;
    //                        AAA = (int)Math.Round(Conversions.Val(DataModul.DB_EventTable.Fields[EventFields.Ort].AsInt()));
    //                        Schalt = 1;
    //                        Ortles(ref AAA, ref Schalt);
    //                        goto IL_0496;
    //                    IL_0496:
    //                        num = 46;
    //                        if (Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString()) != "")
    //                        {
    //                            goto IL_04d4;
    //                        }
    //                        else
    //                            goto IL_0514;
    //                        //=================
    //                        IL_04d4:
    //                        num = 47;
    //                        sPlace = sPlace.TrimEnd() + " " + Strings.Trim(DataModul.DB_EventTable.Fields[EventFields.Ort_S].AsString());
    //                        goto IL_0514;
    //                    IL_0514:
    //                        num = 49;
    //                        Kont1[5] = " " + sPlace.Trim();
    //                        goto IL_0534;
    //                    IL_0534:
    //                        num = 50;
    //                        sPlace = "";
    //                        goto IL_0543;
    //                    IL_0543:
    //                        num = 52;
    //                        if (eArt == 302)
    //                        {
    //                            goto IL_055c;
    //                        }
    //                        else
    //                            goto IL_05ee;
    //                        //=================
    //                        IL_055c:
    //                        num = 53;
    //                        if ((DataModul.DB_EventTable.Fields[EventFields.Reg].Value !=  " "))
    //                        {
    //                            goto IL_058a;
    //                        }
    //                        else
    //                            goto IL_05c8;
    //                        //=================
    //                        IL_058a:
    //                        num = 54;
    //                        o01_Person.Stat = Kont1[7] + " " + Kont1[5];
    //                        goto IL_05ae;
    //                    IL_05ae:
    //                        num = 55;
    //                        DataModul.DB_EventTable.Index = nameof(EventIndex.ArtNr);
    //                        goto end_IL_0001_2;
    //                    //=================
    //                    IL_05c8:
    //                        num = 58;
    //                        o01_Person.Stat = Kont1[7] + " " + Kont1[5];
    //                        goto IL_0663;
    //                    //=================
    //                    IL_05ee:
    //                        num = 60;
    //                        goto IL_05f3;
    //                    //=================
    //                    end_IL_0001:
    //                        break;
    //                }
    //            }
    //        }
    //        catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
    //        {
    //            ProjectData.SetProjectError(obj, lErl);
    //            try0001_dispatch = 2481;
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
    public short Famsatzles(int FamInArb, short Rich, IFamilyData cFamily)
    {
        if (!DataModul.Family.Exists(FamInArb))
        {
            (bool flowControl, short value) = Handle_NonExists(ref FamInArb, Rich);
            if (!flowControl)
            {
                return value;
            }
        }

        cFamily.FillData(DataModul.DB_FamilyTable);
        cFamily.CheckSetAnlDatum(DataModul.DB_FamilyTable);
        return Rich;
    }

    public IListItem<int> Famzeig(int Fam, ELinkKennz Kenn)
    {
        var Modul1 = this;
        EEventArt[] eArts = new[] { EEventArt.eA_Marriage , EEventArt.eA_MarrReligious, EEventArt.eA_504,
        EEventArt.eA_505,EEventArt.eA_506,EEventArt.eA_507,EEventArt.eA_500,EEventArt.eA_501,EEventArt.eA_601};
        EEventArt num2 = default;
        string sDatu;
        DateTime Datu = default;
        foreach (var eArt in eArts)
        {
            string sDate_S;
            Datu = DataModul.Event.GetDate(eArt, Fam, out sDate_S);
            if (Datu != default)
            {
                num2 = eArt;
                sDatu = $"{Datu.Year} {sDate_S}";
                break;
            }
        }

        string text = num2 switch
        {
            EEventArt.eA_500 => "Prok.",
            EEventArt.eA_501 => "Verl.",
            EEventArt.eA_Marriage => "Heir.",
            EEventArt.eA_MarrReligious => "kirH.",
            EEventArt.eA_504 => "Scheid.",
            EEventArt.eA_505 => "Eheä.",
            EEventArt.eA_507 => "Dim.",
            EEventArt.eA_601 => "FiHr.",
            _ => "    ",
        };

        var LiText = new string(' ', 80);
        int perInArb = 0;

        if (DataModul.Link.GetFamPerson(Fam, Kenn, out perInArb))
        {
            Modul1.Person_ReadNames(perInArb, Modul1.Person);
            LiText = $"{$"{text}{Datu}",-12} mit {Modul1.Person.SurName.ToUpper()}, {Modul1.Person.Givennames}";
        }
        else
        {
            LiText = $"{$"{text}{Datu}",-12} mit Unknown";
        }
        if (Datu == default)
        {
            if (DataModul.Family.Get_Aeb(Fam))
            {
                // Ersetze Strings.MidStmtStr(ref LiText, 2, 13, "Ausserehel. ");
                // C#: Ersetze von Index 1 (2. Zeichen, 0-basiert) 13 Zeichen durch "Ausserehel. "
                if (LiText.Length > 1)
                {
                    LiText = LiText.Substring(0, 1) + "Ausserehel. " + LiText.Substring(Math.Min(14, LiText.Length));
                }
                else
                {
                    LiText = "Ausserehel. ";
                }
            }
        }
        return new ListItem<int>(LiText, perInArb);
    }


    /// <summary>
    /// Handles the family not exists.
    /// </summary>
    /// <param name="FamInArb">The fam in arb.</param>
    /// <param name="Rich">The rich.</param>
    /// <returns>System.ValueTuple&lt;System.Boolean, System.Int16&gt;.</returns>
    private (bool flowControl, short value) Handle_NonExists(ref int FamInArb, short Rich)
    {
        if (FamInArb < 1)
        {
            FamInArb = 1;
        }
        if (DataModul.Family.Count == 0)
        {
            return (flowControl: false, value: 1);//break;
                                                  //=================
        }

        var dB_FamilyTable = DataModul.DB_FamilyTable;
        dB_FamilyTable.Index = nameof(FamilyIndex.Fam);
        dB_FamilyTable.MoveFirst();
        if (FamInArb < dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt())
        {
            FamInArb = dB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
        }

        int iMaxFamNr = DataModul.Family.MaxID;

        do
        {
            dB_FamilyTable.Seek("=", FamInArb);
            if (dB_FamilyTable.NoMatch)
            {
                if (FamInArb > iMaxFamNr)
                {
                    FamInArb = iMaxFamNr;
                }
                else
                {
                    if (Rich == 1)
                    {
                        if (FamInArb > iMaxFamNr)
                        {
                            Rich = 0;
                        }
                        FamInArb++;
                    }
                    else
                    {
                        FamInArb--;
                    }
                }
            }
        }
        while (dB_FamilyTable.NoMatch);
        return (flowControl: true, value: Rich);
    }
    /// <summary>
    /// Gibt den Wochentag eines Datums zurück, falls das Datum gültig ist und im erwarteten Format vorliegt.
    /// Das Datum wird geprüft und nur wenn Tag, Monat und Jahr sinnvoll erscheinen, wird der Wochentag als String (z.B. "Montag") zurückgegeben.
    /// Andernfalls wird ein leerer String zurückgegeben.
    /// </summary>
    public string Wochtag(string Datu)
    {
        if (Datu.Trim() == "" || Datu.Left(2).AsInt() > 31)
        {
            return "";
        }
        if (Datu.Right(4).AsInt() > Aus[(int)EOutCfg.o15].AsInt()
            && (Datu.Left(2).AsInt() > 0)
            && (Datu.Substring(3, 2).AsInt() > 0))
        {
            return DateTime.Parse(Datu).DayOfWeekStr(CultureInfo.CurrentUICulture);
        }
        return "";
    }

    /// <summary>
    /// Liest für eine Person (PersInArb) alle relevanten Ereignisdaten (Geburt bis Beerdigung) aus und gibt sie als Dictionary zurück.
    /// Der Schlüssel ist der Ereignistyp (EEventArt), der Wert ist ein Tupel mit:
    /// - der Ereignis-ID (Ereignisart, Link, LfdNr),
    /// - einer formatierten Zeichenkette mit Zusatzinfos (z.B. Zitat, Annotation, Datum, Ort),
    /// - dem zweiten Datum (z.B. Beerdigungsdatum) als DateTime.
    /// Der Parameter 'schalt' steuert, ob das zweite Datum und der Ort mit ausgegeben werden (schalt != 2).
    /// </summary>
    /// <param name="PersInArb">The pers in arb.</param>
    /// <param name="schalt">The schalt.</param>
    /// <returns>IDictionary&lt;EEventArt, System.ValueTuple&lt;System.ValueTuple&lt;EEventArt, System.Int32, System.Int16&gt;, System.String, DateTime&gt;&gt;.</returns>
    public IDictionary<EEventArt, ((EEventArt, int, short), string, DateTime)> FamPerDatles(int PersInArb, int schalt)
    {
        var result = new Dictionary<EEventArt, ((EEventArt, int, short), string, DateTime)>();

        EEventArt num4 = EEventArt.eA_Birth;

        while (num4 <= EEventArt.eA_Burial)
        {
            var sPlace = "";

            if (DataModul.Event.ReadData(num4, PersInArb, out var cEvt, 0))
            {
                var sDatum = "";
                if (cEvt!.dDatumV != default)
                {
                    sDatum = cEvt.dDatumV.AsString();
                }

                var sDatumB = "";


                if (schalt != 2)
                {
                    // include 2. Date
                    if (cEvt.dDatumB != default)
                    {
                        sDatumB = cEvt.dDatumB.AsString();
                    }
                    // Include Place
                    if (cEvt.iOrt > 0 && DataModul.Place.ReadData(cEvt.iOrt, out var cPlace))
                    {
                        sPlace = DataModul.Place.FullName(cPlace, true, true);
                        if (cEvt.sOrt_S.AsString().Trim() != "")
                        {
                            sPlace = sPlace.TrimEnd() + " " + cEvt.sOrt_S.Trim();
                        }
                    }
                }
                bool xCitation = DataModul.SourceLink_Exists(3, PersInArb, (EEventArt)num4, LfNR) || ("" != cEvt.sBem[3]);
                bool xAnnotation = (cEvt.sBem[1].TrimEnd() != "")
                    || (cEvt.sBem[2].TrimEnd() != "");
                bool xBC = cEvt.sVChr != "0";
                bool xReg = cEvt.sReg.TrimEnd() != "";
                bool xWitness = false;
                string text = Event_PreDisplay(xCitation, xAnnotation: xAnnotation, xBC: xBC, xReg: xReg);

                result[num4] = (cEvt.ID,
                    text + " " + sDatum + " " + cEvt.sDatumV_S + " " + sDatumB + " " + cEvt.sDatumB_S + " " + sPlace,
                 cEvt.dDatumB);
            }
            num4++;
        }
        return result;
    }

    /// <summary>
    /// Gibt eine Zeichenkette zurück, die verschiedene Statuskennzeichen für ein Ereignis darstellt.
    /// Die Parameter steuern, ob bestimmte Symbole für Zitat (§), Zeugen (Z), Anmerkung (B), BC (<) oder Register (U) ausgegeben werden.
    /// Die Symbole werden in der Reihenfolge §, Z, B, <, U aneinandergehängt, jeweils mit einem Leerzeichen.
    /// </summary>
    /// <param name="xCitation">true, wenn ein Zitat vorhanden ist (gibt "§ " aus).</param>
    /// <param name="xWitness">true, wenn ein Zeuge vorhanden ist (gibt "Z " aus).</param>
    /// <param name="xAnnotation">true, wenn eine Anmerkung vorhanden ist (gibt "B " aus).</param>
    /// <param name="xBC">true, wenn ein BC-Kennzeichen vorhanden ist (gibt "&lt; " aus).</param>
    /// <param name="xReg">true, wenn ein Registereintrag vorhanden ist (gibt "U " aus).</param>
    /// <returns>Eine Zeichenkette mit den entsprechenden Statussymbolen.</returns>
    public string Event_PreDisplay(bool xCitation = false, bool xWitness = false, bool xAnnotation = false, bool xBC = false, bool xReg = false)
    {
        string text = "";
        if (xCitation)
            text = "§ ";
        if (xWitness)
            text += "Z ";
        if (xAnnotation)
            text += "B ";
        if (xBC)
            text += "< ";
        if (xReg)
            text += "U ";
        return text;
    }

    /// <summary>
    /// Sucht alle Familien-IDs, in denen eine bestimmte Person mit einer bestimmten Verknüpfungsart (ELinkKennz) beteiligt ist.
    /// Bei mehreren Kind-Einträgen wird eine Fehlermeldung angezeigt, da eine Person nur in einer Familie als Kind vorkommen darf.
    /// Das Ergebnis wird als Liste von Familiennummern zurückgegeben und in UbgT als kommaseparierte Zeichenkette gespeichert.
    /// </summary>
    /// <param name="iPerFamNr">The pers in arb.</param>
    /// <param name="eLKennz1">The e l kennz1.</param>
    /// <returns>IList&lt;System.Int32&gt;.</returns>
    public IList<int> Link_Famsuch(int iPerFamNr, ELinkKennz eLKennz1)
    {
        var aiFam = DataModul.Link.GetPersonFams(iPerFamNr, eLKennz1);
        if (eLKennz1 == ELinkKennz.lkChild & (aiFam.Count > 1))
        {
            string text2 = $"Person {iPerFamNr} ist in den Familien {string.Join(", ", aiFam)} als Family.Kind eingebunden. Eine Person kann aber nur in einer Familie als Family.Kind sein.";
            text2 += "\nBitte diesen Fehler zuerst korrigieren.";
            _ = Interaction.MsgBox(text2, title: "Schwerer Datenfehler", icon: MessageBoxIcon.Exclamation);
        }
        return aiFam;
    }

    public int Eltsuch(int persInArb)
    {
        var result = DataModul.FindParentialFamilies(persInArb).ToList();
        if (result.Count > 1)
        {
            string text2 = $"Person {PersInArb} ist in den Familien {string.Join(", ", result)} als Family.Kind eingebunden. Eine Person kann aber nur in einer Familie als Family.Kind sein.";
            text2 += "\nBitte diesen Fehler zuerst korrigieren.";
            _ = Interaction.MsgBox(text2, title: "Schwerer Datenfehler", icon: MessageBoxIcon.Exclamation);
        }
        else if (result.Count == 1)
        {
            return result[0];
        }
        return 0;
    }

    public void FamDatLes_int(int FamInArb, Action _DisableIllg, Action<string, int, string> SetEventText)
    {
        EEventArt eArt = EEventArt.eA_500;
        while (eArt <= EEventArt.eA_507)
        {
            UbgT = "";
            string[] Kont1 = new string[15];
            Kont1.Initialize();

            string text = "";


            string sK1_1 = Kont1[1];
            string sK1_2 = Kont1[2];
            string sK1_3 = Kont1[3];
            if (DataModul.Event.ReadData((EEventArt)eArt, FamInArb, out var Event, 0))
            {
                bool xZeugExist = DataModul.Witness.ExistZeug(FamInArb, eArt, LfNR);
                bool xCitExist = DataModul.SourceLink.Exists(3, FamInArb, eArt, LfNR);
                xCitExist |= !string.IsNullOrWhiteSpace(Event.sBem[3]);
                xZeugExist |= !string.IsNullOrWhiteSpace(Event.sBem[4]);
                text = Event_PreDisplay(xCitExist, xZeugExist, !string.IsNullOrWhiteSpace(Event.sBem[1])
                    || !string.IsNullOrWhiteSpace(Event.sBem[2]),
                    Event.sVChr != "0",
                    !string.IsNullOrWhiteSpace(Event.sReg));

                if (Event.dDatumV != default)
                {
                    var Datu = Event.dDatumV.AsString();
                    Datu = Datu.Date2DotDateStr();
                    Kont1[1] = (Wochtag(Datu) + " " + Datu).Trim();
                }

                Kont1[2] = Event.sDatumV_S;
                if (Event.dDatumB != default)
                {
                    var Datu = ("00000000" + Event.dDatumB.AsString().Trim()).Right(8);
                    Datu = Datu.Date2DotDateStr();
                    if (Kont1[1] != "")
                    {
                        Datu = "/ " + Datu;
                    }
                    Kont1[3] = Datu;
                }

                if (0 != Event.iDatumText)
                {
                    Kont1[14] = " " + DataModul.TextLese1(Event.iDatumText) + " ";
                }

                if (Event.iKBem > 0)
                {
                    Kont1[7] = " " + DataModul.TextLese1(Event.iKBem).Trim() + " ";
                }
                UbgT = "";
                if (Event.iOrt > 0 && DataModul.Place.ReadData(Event.iOrt, out var cPlace))
                {
                    _ = Ortles(cPlace, 0).IntoString(Kont2);
                    Kont2[6] = Event.sZusatz.AsString();
                    UbgT = Kont2[6] + " " + Kont2[0];
                    if (Event.sOrt_S.AsString().Trim() != "")
                    {
                        UbgT = UbgT.TrimEnd() + " " + Event.sOrt_S.Trim();
                    }
                }
                Kont1[4] = Event.sDatumB_S;
                if (Event.iPlatz > 0)
                {
                    Kont1[8] = " " + DataModul.TextLese1(Event.iPlatz).Trim() + " ";
                }

                if (Event.iOrt > 0 && DataModul.Place.ReadData(Event.iOrt, out var cPlace2))
                {
                    Kont2[6] = "";
                    if (null != Event.sZusatz)
                    {
                        Kont2[6] = Event.sZusatz;
                    }
                    _Modul1.Instance.UbgT = DataModul.Place.FullName(cPlace2, true, true);
                    if (Event.sOrt_S.Trim() != "")
                    {
                        UbgT = UbgT.TrimEnd() + " " + Event.sOrt_S.Trim();
                    }
                    Kont1[5] = " " + UbgT.Trim();
                    UbgT = "";
                }
                Job = Kont1[9] + Kont1[1] + Kont1[3] + Kont1[14] + Kont1[7] + Kont1[5] + Kont1[8].Replace("  ", " ");
                if ((Event.sBem[1].TrimEnd() != "")
                    || (Event.sBem[2].TrimEnd() != ""))
                    if (Job.Trim().Length < 2)
                    {
                        {
                            Kont1[1] = "Bemerkung";
                            Job = Kont1[1] + Kont1[3] + Kont1[7] + Kont1[5] + Kont1[8];
                        }
                    }

                SetEventText(text, (int)eArt, $"{sK1_1} {sK1_2}{sK1_3} {Kont1[4]}{Kont1[14]}{Kont1[7]}{Kont1[5]}{Kont1[6]} {UbgT}{Kont1[8]}");
            }
            eArt++;
        }
    }

    public bool EstDateLes(out string text)
    {
        text = "";
        if (DataModul.Event.ReadData(EEventArt.eA_601, FamInArb, out var cEvt, 0))
        {
            int num5 = 0;
            while (num5 <= 15f)
            {
                Kont1[num5] = "";
                num5 += 1;
            }

            if (cEvt.dDatumV != default)
            {
                Kont1[1] = cEvt.dDatumV.ToString();
            }
            //=================
            UbgT = "";
            if (cEvt.iOrt > 0 && DataModul.Place.ReadData(cEvt.iOrt, out var cPlace))
            {
                UbgT = DataModul.Place.FullName(cPlace, true, true);
                if (cEvt.sOrt_S != "")
                {
                    UbgT = UbgT.TrimEnd() + " " + cEvt.sOrt_S.Trim();
                }
            }

            text = Kont1[1] + " " + UbgT;
            return true;
        }
        return false;
    }

    public IListItem<int> Event_ToShortLine(IEventData cEvent)
    {
        string sLine = "";
        if (cEvent.eArt == EEventArt.eA_603 && 0 != cEvent.iArtText)
        {
            sLine += cEvent.sArtText.Trim() + " ";
        }

        if (cEvent.dDatumV != default)
        {
            sLine += $"{cEvent.dDatumV.Year:####0000}";
        }

        if (cEvent.dDatumB != default)
        {
            sLine += $"/{cEvent.dDatumB.Year:0000####}";
        }
        if (cEvent.iDatumText > 0)
        {
            sLine += " " + cEvent.sDatumText;
        }

        if (cEvent.iKBem > 0)
        {
            sLine += " " + cEvent.sKBem.Trim();
        }

        if (0 != cEvent.iHausNr)
        {
            sLine += " " + cEvent.sHausNr.Trim();
        }

        if (cEvent.iOrt > 0 && DataModul.Place.ReadData(cEvent.iOrt, out var cPlace))
        {
            if ("" != cEvent.sZusatz)
            {
                sLine += " " + cEvent.sZusatz;
            }
            sLine += " " + DataModul.Place.FullName(cPlace).Trim();
        }

        if (cEvent.iPlatz >= 0)
        {
            sLine += " " + cEvent.sPlatz.Trim();
        }

        if (sLine.Trim().Length < 2)
        {
            if ((cEvent.sBem[1].TrimEnd() != "")
                || (cEvent.sBem[2].TrimEnd() != ""))
            {
                sLine = "Bemerkung " + sLine;
            }
        }

        return new ListItem<int>(sLine, cEvent.iLfNr);
    }

    public int? FamDatYear(int FamInArb, short schalt)
    {
        int? result = null;
        EEventArt eArt = EEventArt.eA_500;
        while (eArt <= EEventArt.eA_507)
        {
            if (DataModul.Event.ReadData(eArt, FamInArb, out var cEvt, 0))
            {
                if (schalt != 1)
                {
                    Familie.Default.cbxIllegitRel.CheckState = CheckState.Unchecked;
                }
                if (eArt != EEventArt.eA_506 && schalt != 1)
                {
                    Familie.Default.cbxIllegitRel.Enabled = false;
                }

                if (cEvt.dDatumV != default && FamInArb != Familie.Default.iFamNr)
                {
                    result = cEvt.dDatumV.Year;
                    if (eArt >= EEventArt.eA_MarrReligious)
                        break;
                }
            }
            eArt++;
        }
        return result;
    }

    public short IsFormloaded(object Formtocheck)
    {
        int num2 = 0;
        while (num2 <= MainProject.Application.OpenForms.Count - 1)
        {
            if (MainProject.Application.OpenForms[num2] == Formtocheck)
            {
                return -1;
            }
            num2++;
        }
        return 0;
    }


    public void Berufles(int PersInArb, EEventArt eArt, object _combo1)
    {
        var combo1 = (ComboBox)_combo1;
        string[] Kont1 = new string[101];
        Kont1.Initialize();

        combo1.Text = "";
        combo1.Items.Clear();

        foreach (var Event in DataModul.Event.ReadEventsBeSu(PersInArb, eArt))
        {
            if (Event.iLfNr >= 0)
            {
                int num5 = 0;
                while (num5 <= 15)
                {
                    Kont1[num5] = "";
                    num5 += 1;
                }
                //eArt = I;
                var Datu = "";
                if (eArt == EEventArt.eA_105
                    && 0 != Event.iArtText && Aus[(int)EOutCfg.o47] == "Y")
                {
                    Kont1[9] = " " + DataModul.TextLese1(Event.iArtText).Trim() + " ";
                }

                if (Event.dDatumV != default)
                {
                    Datu = Event.dDatumV.AsString();
                    Ds = Event.sDatumV_S.AsString();

                    Datu = Datu.Date2DotDateStr();
                    Kont1[1] = Datu;
                }
                //=================

                if (Event.dDatumB != default)
                {
                    Datu = ("00000000" + Event.dDatumB.AsString()).Right(8);
                    Datu = Datu.Date2DotDateStr();
                    if (Datu != "")
                    {
                        Datu = " / " + Datu;
                    }
                    Kont1[3] = Datu;

                }
                string UbgT = "";
                if (Event.iDatumText > 0)
                {
                    Kont1[14] = " " + DataModul.TextLese1(Event.iDatumText) + " ";
                }

                if (Event.iKBem > 0)
                {
                    Kont1[7] = " " + DataModul.TextLese1(Event.iKBem).Trim() + " ";
                }


                if (0 != Event.iHausNr)
                {
                    Kont1[7] = Kont1[7] + " " + DataModul.TextLese1(Event.iHausNr).Trim() + " ";
                }

                if (Event.iOrt.AsInt() > 0.0)
                {
                    Kont2[6] = "";
                    if (null != Event.sZusatz)
                    {
                        Kont2[6] = Event.sZusatz;
                    }
                    if (DataModul.Place.ReadData(Event.iOrt, out var cPlace2))
                    {
                        GeolesPlace(cPlace2!);
                    }

                    UbgT = Kont2[6] + " " + UbgT;
                    globOrt2 = Kont2[1];
                    if (Event.sOrt_S.AsString().Trim() != "")
                    {
                        UbgT = UbgT.TrimEnd() + " " + Event.sOrt_S.Trim();
                    }
                }
                Kont1[4] = Event.sDatumB_S;
                if (Event.iPlatz != 0)
                {
                    Kont1[8] = " " + DataModul.TextLese1(Event.iPlatz).Trim() + " ";
                }

                if (Event.iOrt > 0 && DataModul.Place.ReadData(Event.iOrt, out var cPlace))
                {
                    Kont2[6] = "";
                    if (null != Event.sZusatz)
                    {
                        Kont2[6] = Event.sZusatz;
                    }
                    _Modul1.Instance.UbgT = DataModul.Place.FullName(cPlace, true, true);
                    if (Event.sOrt_S.Trim() != "")
                    {
                        UbgT = UbgT.TrimEnd() + " " + Event.sOrt_S.Trim();
                    }
                    Kont1[5] = " " + UbgT.Trim();
                    UbgT = "";
                }
                Job = Kont1[9] + Kont1[1] + Kont1[3] + Kont1[14] + Kont1[7] + Kont1[5] + Kont1[8].Replace("  ", " ");
                if ((Event.sBem[1].TrimEnd() != "")
                    | (Event.sBem[2].TrimEnd() != ""))
                    if (Job.Trim().Length < 2)
                    {
                        {
                            Kont1[1] = "Bemerkung";
                            Job = Kont1[1] + Kont1[3] + Kont1[7] + Kont1[5] + Kont1[8];
                        }
                    }

                SetComboText(combo1, Event.iLfNr, Event.sReg, Job);

                //=================

            }
        }
    }

    private static void SetComboText(ComboBox combo1, int Event_iLfNr, string Event_sReg, string job)
    {
        if (Event_sReg != " ")
        {
            combo1.Text = job;
            combo1.Tag = Event_iLfNr;
        }
        _ = combo1.Items.Add(new ListItem<int>(job, Event_iLfNr));
    }

    public void DatPruef(int Pschalt)
    {
        var num = 0;
        var num2 = num;
        //switch ((num3 <= -2) ? 1 : num3)
        //{
        //    case 2:
        //        lErl = 1;
        //        if (Information.Err().Number == 13)
        //        {
        //            if (Pschalt == 1f)
        //            {
        //                MainProject.Forms.Hinter.RTB.SelectedText = IText[EUserText.t207] + ":" + PersInArb.AsString() + "\n";
        //                _ = MainProject.Forms.Hinter.List1.List6_Items.Add(new ListItem(IText[EUserText.t207] + ":" + PersInArb.AsString(), PersInArb));
        //                Modul1.LfNR++;
        //                MainProject.Forms.Hinter.Label16.Text = IText[EUserText.t207] + ": " + Modul1.LfNR.AsString();
        //                MainProject.Forms.Hinter.Label16.Refresh();
        //            }
        //            goto IL_0e53;
        //            //=================
        //        }
        //        else
        //        {
        //            if (Interaction.MsgBox(Conversions.ErrorToString(), MessageBoxButtons.OKCancel, Information.Err().Number.AsString()) == DialogResult.Cancel)
        //            {
        //                ProjectData.EndApp();
        //            }
        //            Modul1.LfNR++;
        //            MainProject.Forms.Hinter.btnMoveToChurchCemet.Visible = true;
        //            MainProject.Forms.Hinter.btnMoveToEntityAnot.Visible = true;
        //            MainProject.Forms.Hinter.RTB.SelectedText = IText[EUserText.t207] + ":" + PersInArb.AsString() + "\n";
        //            _ = MainProject.Forms.Hinter.List1.List6_Items.Add(new ListItem(IText[EUserText.t207] + ":" + PersInArb.AsString(), PersInArb));
        //            Modul1.LfNR++;
        //            MainProject.Forms.Hinter.Label16.Text = "Fehler: " + Modul1.LfNR.AsString();
        //            MainProject.Forms.Hinter.Label16.Refresh();
        //            ProjectData.ClearProjectError();
        //            if (num2 == 0)
        //            {
        //                throw ProjectData.CreateProjectError(ErrUser14);
        //            }
        //            //=================
        //        }
        //        break;
        //    case 1:
        //        break;
        //    //=================
        //    _Menue:
        //        goto end_IL_0001;
        //}
        //goto IL_1011;

        num = 2;
        var num4 = 1;
        var array = new string[5];
        while (num4 <= 4)
        {
            PruefJahr[num4] = 0;
            array[num4] = "";
            num4 += 1;
        }
        ErSchalt = 0;
        num4 = 101;
        var num3 = 0;
        while (num4 <= 104)
        {
            PruefJahr[(int)Math.Round(num4 - 100f)] = 0;
            if (DataModul.Event.ReadData((EEventArt)num4, PersInArb, out var cEvt, 0))
            {
                if (cEvt.sVChr != "0")
                {
                    goto end_IL_0001_2;
                }
                else if (cEvt.dDatumV != default)
                {
                    var Datu = cEvt.dDatumV.ToString();
                    if (Datu.Left(2).AsDouble() > 0.0)
                    {
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        _ = Datu.AsDate().DayOfWeek;
                    }
                }
                else if (cEvt.dDatumV != default)
                {
                    PruefJahr[num4 - 100] = cEvt.dDatumV.Year;
                }
            }
            else
            {
                PruefJahr[num4 - 100] = 0;
            }

            num4++;
        }
        num4 = 2;
        while (num4 <= 4)
        {
            if (PruefJahr[num4] == 0)
            {
                PruefJahr[num4] = PruefJahr[num4 - 1];
            }
            num4++;
        }
        num4 = 1;
        while (num4 <= 3)
        {
            if (PruefJahr[num4 + 1] == 0)
            {
                PruefJahr[num4 + 1] = PruefJahr[num4];
            }
            array[num4] = PruefJahr[num4].AsString().PadLeft(8, '0');
            array[num4 + 1] = PruefJahr[num4 + 1].AsString().PadLeft(8, '0');
            float num11;
            if (PruefJahr[num4] > PruefJahr[num4 + 1])
            {
                if (Strings.Mid(array[num4 + 1], 5, 2) == "00")
                {
                    array[num4 + 1] = array[num4 + 1].Left(4) + Strings.Mid(array[num4], 5, 2) + array[num4 + 1].Right(2);
                }
                if (Strings.Mid(array[num4 + 1], 7, 2) == "00")
                {
                    array[num4 + 1] = array[num4 + 1].Left(6) + Strings.Mid(array[num4], 7, 2);
                }
                PruefJahr[num4 + 1] = array[num4 + 1].AsInt();
                array[num4 + 1] = array[num4 + 1].AsInt().AsString();
                num4 += 2;
                continue;
            }
            else
            {
                num11 = 0f;
            }
            while (PruefJahr[num4] > 0)
            {
                if (Strings.Mid(array[num4], 5, 2) == "00")
                {
                    if (num11 != 0f || array[num4 - 1].AsInt() <= array[num4].AsInt())
                    {
                        array[num4] = array[num4].Left(4) + Strings.Mid(array[num4 + 1], 5, 2) + array[num4].Right(2);
                        break;
                    }
                    array[num4] = array[num4].Left(4) + Strings.Mid(array[num4 - 1], 5, 2) + array[num4].Right(2);
                    PruefJahr[num4] = array[num4].AsInt();
                    num11 = 1f;

                    //=================
                }

                num11 = 0f;
                while (num11 == 0f
                    && Strings.Mid(array[num4], 7, 2) == "00")
                {
                    if (array[num4 - 1].AsInt() <= array[num4].AsInt())
                    {
                        array[num4] = array[num4].Left(6) + Strings.Mid(array[num4 + 1], 7, 2);
                        break;
                    }
                    array[num4] = array[num4].Left(6) + Strings.Mid(array[num4 - 1], 7, 2);
                    PruefJahr[num4] = array[num4].AsInt();
                    num11 = 1;

                }
                PruefJahr[num4] = array[num4].AsInt();
                break;
            }
            num4 += 2;

        }
        num4 = 1;
        while (num4 <= 4)
        {
            if (PruefJahr[num4] > 39000000)
            {
                if (Pschalt == 0)
                {
                    Datfehler_Abbruch = (short)Interaction.MsgBox(IText[EUserText.t207] + ":" + PersInArb.AsString(), "", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    break;
                    //=================
                }
                LfNR++;
                MainProject.Forms.Hinter.Button7.Visible = true;
                MainProject.Forms.Hinter.Button8.Visible = true;
                MainProject.Forms.Hinter.RTB.SelectedText = IText[EUserText.t207] + ":" + PersInArb.AsString() + "\n";
                _ = MainProject.Forms.Hinter.List1.Items.Add(new ListItem(IText[EUserText.t207] + ":" + PersInArb.AsString(), PersInArb));
                MainProject.Forms.Hinter.Label16.Text = "Fehler: " + LfNR.AsString();
                MainProject.Forms.Hinter.Label16.Refresh();
            }
            num4 += 1;
        }
        if (PruefJahr[3] > 0)
        {
            if (Strings.Mid(array[3], 5, 4) == "0000")
            {
                PruefJahr[3] = PruefJahr[3] + 1231;
            }
        }
        if (PruefJahr[4] > 0)
        {
            if (Strings.Mid(array[4], 5, 4) == "0000")
            {
                PruefJahr[4] = PruefJahr[4] + 1231;
            }
        }
        num4 = 1;
        goto IL_0b1c;
    IL_0b1c: // <========== 3
        num = 115;
        while (num4 <= 3
            && PruefJahr[num4 + 1] <= 0
            || PruefJahr[num4] <= PruefJahr[num4 + 1])
        {
            num4 += 1;
        }
        if (num4 > 3)
        {
            if (Pschalt == 0)
            {
                Datfehler_Abbruch = (short)Interaction.MsgBox(IText[EUserText.t207] + ":" + PersInArb.AsString(), title: "", mb: MessageBoxButtons.OKCancel, icon: MessageBoxIcon.Error);
                //=================
            }
            else
            {
                LfNR++;
                MainProject.Forms.Hinter.Button7.Visible = true;
                MainProject.Forms.Hinter.Button8.Visible = true;
                MainProject.Forms.Hinter.RTB.SelectedText = IText[EUserText.t207] + ":" + PersInArb.AsString() + "\n";
                _ = MainProject.Forms.Hinter.List1.Items.Add(new ListItem(IText[EUserText.t207] + ":" + PersInArb.AsString(), PersInArb));
                MainProject.Forms.Hinter.Label16.Text = "Fehler: " + LfNR.AsString();
                MainProject.Forms.Hinter.Label16.Refresh();
                //=================
            }
        }
        goto end_IL_0001_2;
    end_IL_0001_2:
        return;
    }



    public IList<int> Ehesuch(int personNr, string Persex)
    {
        var aiMarr = new List<int>();
        switch (Persex)
        {
            case "M":
                eLKennz = ELinkKennz.lkFather;
                break;
            case "F":
                eLKennz = ELinkKennz.lkMother;
                break;
            default:
                UbgT = "";
                return aiMarr;
        }
        foreach (var link in DataModul.Link.ReadAllPers(personNr, eLKennz))
        {
            aiMarr.Add(link.iFamNr);
        }

        UbgT = string.Join("", aiMarr.Select((i) => $"{i,10}"));
        return aiMarr;
    }

    public IEnumerable<IListItem<(int, DateTime, ELinkKennz)>> Family_Kindsuch(int iFamNr)
    {

        foreach (var eLKennz in new[] { ELinkKennz.lkChild, ELinkKennz.lkAdoptedChild })
            foreach (var link in DataModul.Link.ReadAllFams(iFamNr, eLKennz))
            {
                var dBirt = DataModul.Event.GetPersonBirthOrBapt(link.iPersNr);

                string sDest = $"{dBirt,10}{link.iPersNr,10}{(eLKennz == ELinkKennz.lkAdoptedChild ? "A" : "")}";
                yield return new ListItem<(int, DateTime, ELinkKennz)>(sDest, (link.iPersNr, dBirt, eLKennz));
            }

    }

    public void Rech2()
    {
        ProjectData.ClearProjectError();
        var num3 = 2;
        var num4 = 0;
        var num5 = 0;
        var num6 = 0;
        var text = "";
        var sDat_Birth = "08.11.1827";
        var sDat_Death = "26.03.1900";
        if (sDat_Death.Left(2).AsDouble() != 0.0)
        {
            if (Strings.Mid(sDat_Death, 4, 2).AsDouble() != 0.0)
            {
                num6 = ((sDat_Birth.AsDate() - sDat_Death.AsDate()).TotalDays / 365.25d).AsInt();
                if ((num6 > 100) & (DateTime.Compare(Microsoft.VisualBasic.CompilerServices.Conversions.ToDate(sDat_Death), DateTime.Now) == 0))
                {
                    text = "";
                    //=================
                }
                else
                {
                    if (Strings.Mid(sDat_Birth, 4, 2).CompareTo(Strings.Mid(sDat_Death, 4, 2)) > 0)
                    {
                        num6--;
                    }

                    //=================


                    string interval = "yyyy";
                    sDat_Death = (sDat_Death.AsDate() - TimeSpan.FromDays(num6 * 365.25)).AsString();
                    num4 = ((sDat_Birth.AsDate() - sDat_Death.AsDate()).Days * 12 / 365.25).AsInt();
                    if (sDat_Birth.Left(2).CompareTo(sDat_Death.Left(2)) > 0)
                    {
                        num4--;
                    }
                    if (num4 < 0)
                    {
                        num6--;
                        num4 += 12;
                        sDat_Birth = (sDat_Birth.AsDate() + TimeSpan.FromDays((num4 - 12) * 30.25)).AsString();
                        //=================
                    }
                    else
                    {
                        if (num4 > 12)
                        {
                            num6++;
                            num4 -= 12;
                            //=================
                        }
                        else
                        {
                            sDat_Death = (sDat_Death.AsDate() - TimeSpan.FromDays(num4 * 30.25)).AsString();
                        }
                        //=================
                    }
                    num5 = (sDat_Birth.AsDate() - sDat_Death.AsDate()).TotalDays.AsInt();
                    text = IText[EUserText.t117] + num6.AsString() + IText[EUserText.t119] + num4.AsString() + IText[EUserText.t120] + num5.AsString() + IText[EUserText.t121];
                }
                if (unchecked(num4 > 12 || num5 > 31))
                {
                    _ = Interaction.MsgBox("Mit dem eingestelltem Datumsformat ist eine Altersberechnung nicht möglich. Das Datumsformat (kurzes Datum) ist in der Windows-Systemsteuerung auf >TT.MM.JJJJ< einzustellen.");
                }
                goto end_IL_0001;
            }
        }
        text = $"{IText[EUserText.t117]} {IText[EUserText.t118]} {Strings.Mid(sDat_Death, 7, 4).AsInt() - Strings.Mid(sDat_Birth, 7, 4).AsInt()} {IText[EUserText.t216]}";
        if (sDat_Death == "")
        {
            text = "";
        }
        Personen.Default.lblSearch2.Text = text;
    end_IL_0001: // <========== 12
        return;
    }

    public void DataModul_Texte_ListDistLeitname(ETextKennz eTKennz, string UbgT, IList items)
    {
        string right = "";
        items.Clear();
        IRecordset dB_TexteTable = DataModul.DB_TexteTable;
        dB_TexteTable.Index = nameof(TexteIndex.LTexte);
        dB_TexteTable.Seek(">=", UbgT, (char)eTKennz);
        int num = 0;
        while (num < 200
            && !dB_TexteTable.EOF
            && !dB_TexteTable.NoMatch)
        {
            string Texte_sLeitname = dB_TexteTable.Fields[TexteFields.Leitname].AsString();
            int Texte_iTxtNr = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
            ETextKennz Texte_eTKennz = dB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>();
            if (Texte_eTKennz == eTKennz)
            {
                if (Texte_sLeitname != right)
                {
                    num++;
                    _ = items.Add(new ListItem(Texte_sLeitname, Texte_iTxtNr));
                    right = Texte_sLeitname;
                }
            }
            dB_TexteTable.MoveNext();
        }
    }

    public void Vornam_Namles(int personNr)
    {

        var vornam = MainProject.Forms.Vornam;
        IRecordset dB_NameTable = DataModul.DB_NameTable;

        var num7 = 0f;
        var num4 = 1;

        if (DataModul.Names.ReadPersonNames(personNr, out var aiVorn, out var aiNick))
        {
            var text = "";
            num4 = 1;
            while (num4 <= 15 && aiVorn[num4] != 0)
            {
                var (V, LD) = DataModul.TextLese2(aiVorn[num4]);
                if (aiNick[num4].xRuf)
                {
                    (num4 switch
                    {
                        1 => vornam.CheckBox1,
                        2 => vornam.CheckBox2,
                        3 => vornam.CheckBox3,
                        4 => vornam.CheckBox4,
                        5 => vornam.CheckBox5,
                        6 => vornam.CheckBox6,
                        7 => vornam.CheckBox7,
                        8 => vornam.CheckBox8,
                        9 => vornam.CheckBox9,
                        10 => vornam.CheckBox10,
                        11 => vornam.CheckBox11,
                        12 => vornam.CheckBox12,
                        13 => vornam.CheckBox13,
                        14 => vornam.CheckBox14,
                        15 => vornam.CheckBox15,
                        //=================
                        _ => new CheckBox(),
                    }).CheckState = CheckState.Checked;
                }

                if (aiNick[num4].xNick)
                {
                    (num4 switch
                    {
                        1 => vornam.CheckBox16,
                        2 => vornam.CheckBox17,
                        3 => vornam.CheckBox18,
                        4 => vornam.CheckBox19,
                        5 => vornam.CheckBox20,
                        6 => vornam.CheckBox21,
                        7 => vornam.CheckBox22,
                        8 => vornam.CheckBox23,
                        9 => vornam.CheckBox24,
                        10 => vornam.CheckBox25,
                        11 => vornam.CheckBox26,
                        12 => vornam.CheckBox27,
                        13 => vornam.CheckBox28,
                        14 => vornam.CheckBox29,
                        15 => vornam.CheckBox30,
                        //=================
                        _ => new CheckBox(),
                    }).CheckState = CheckState.Checked;
                }
                text = text + V.TrimEnd() + " ";
                vornam.Text_Renamed[(short)num4].Text = V;
                vornam.Text_Renamed[(short)(num4 + 50)].Text = LD;
                if (LD.Length > 0 && Aus[(int)EOutCfg.o20] == "Y")
                {
                    text = text + ">" + LD.Trim() + "< ";
                }
                num4++;
            }
        }
        if (num7 == 0f)
        {
            num7 = num4;
        }
        if (Tast > 13)
        {
            vornam.Text_Renamed[(short)Math.Round(num7)].Text = Strings.Chr(Tast).AsString().ToUpper();
        }
        vornam.Text_Renamed[(short)Math.Round(num7)].SelectionStart = vornam.Text_Renamed[(short)Math.Round(num7)].Text.Length;
        _ = vornam.Text_Renamed[(short)Math.Round(num7)].Focus();
    }

    public float Datcheck(int eArt)
    {
        Nr = eArt > 499 ? Familie.Default.iFamNr : Personen.Default.PersonNr;
        return DataModul.Event.Exists((EEventArt)eArt, Nr, LfNR) ? 5f : 1f;
    }
    public bool RemoveWriteProtection(string sFile)
    {
        bool result = true;
        if (File.Exists(sFile))
        {
            FileInfo fileInfo = new(sFile);
            FileInfo fileInfo2 = fileInfo;
            try
            {
                if ((fileInfo2.Attributes & FileAttributes.ReadOnly) != 0)
                {
                    fileInfo2.Attributes ^= FileAttributes.ReadOnly;
                }
            }
            catch (Exception projectError)
            {
                ProjectData.SetProjectError(projectError);
                result = false;
                ProjectData.ClearProjectError();
            }
        }
        return result;
    }

    public string[] F_GetAllFiles(string sPath, int funk)
    {
        int num4 = default;
        string[] array = default;
        string[] files = default;
        string[] directories = default;
        int num6 = default;
        int num7 = default;
        directories = Directory.GetDirectories(sPath);
        num7 = directories.Length - 1;
        while (num6 <= num7)
        {
            files = Directory.GetFiles(directories[num6]);
            if (array == null)
            {
                array = files;
                //=================
            }
            else
            {
                num4 = array.Length;
                array = (string[])Utils.CopyArray(array, new string[num4 + files.Length - 1 + 1]);
                files.CopyTo(array, num4);
                //=================
            }
            //            _ = F_GetAllFiles(directories[num6], funk);
            num6++;
        }


        if (funk == 1)
        {
            return array;
            //=================
        }
        else
        {
            return directories;
            //=================
        }
    }

    #region Text Methods
    public string Strngs_Umlaute4(string Fld, int uml)
    {
        bool IsLowerCase(string s) => !string.IsNullOrWhiteSpace(s) && char.IsLower(s[0]);

        switch (uml)
        {
            default:
                return Fld;
            case 2:
                IList<string[]> replacements2 = [
                        [ "Ä", "AE", "Ae" ],
                            [ "ä", "ae" ],
                            [ "Ö" , "OE", "Oe"],
                            [ "ö" , "oe" ],
                            [ "Ü" , "UE", "Ue"],
                            [ "ü" , "ue" ],
                            [ "ß" , "ss" ]
                ];

                foreach (var replacement in replacements2)
                {
                    if (replacement.Length > 2)
                        while (Fld.Contains(replacement[0]))
                        {
                            var iIdx = Fld.IndexOf(replacement[0]);
                            if ((Fld.Length > iIdx + replacement[0].Length)
                                && IsLowerCase(Fld.Substring(iIdx + replacement[0].Length, 1)))
                                Fld = Fld.Substring(0, iIdx) + replacement[2] + Fld.Substring(iIdx + replacement[0].Length);
                            else
                                Fld = Fld.Substring(0, iIdx) + replacement[1] + Fld.Substring(iIdx + replacement[0].Length);
                        }
                    else
                        Fld.Replace(replacement[0], replacement[1]);
                }

                return Fld;
            case 1:
                IList<string[]> replacements = [
                    ["ü", $"{(char)129}"],
                    ["é", $"{(char)130}"],
                    ["â", $"{(char)131}"],
                    ["ä", $"{(char)132}"],
                    ["à", $"{(char)133}"],
                    ["å", $"{(char)134}"],
                    ["ç", $"{(char)135}"],
                    ["ê", $"{(char)136}"],
                    ["ë", $"{(char)137}"],
                    ["è", $"{(char)138}"],
                    ["ï", $"{(char)139}"],
                    ["î", $"{(char)140}"],
                    ["ì", $"{(char)141}"],
                    ["Ä", $"{(char)142}" ],
                    ["Å", $"{(char)143}"],
                    ["É", $"{(char)144}"],

                    ["ô", $"{(char)147}"],
                    ["ö", $"{(char)148}"],
                    ["ò", $"{(char)149}"],
                    ["û", $"{(char)150}"],
                    ["ù", $"{(char)151}"],
                    ["ÿ", $"{(char)152}"],
                    [ "Ö", $"{(char)153}"],

                    ["á", $"{(char)160}"],
                    ["í", $"{(char)161}"],
                    [ "ó", $"{(char)162}" ],
                    [ "ú", $"{(char)163}" ],
                    [ "ñ", $"{(char)164}" ],
                    [ "Ñ", $"{(char)165}" ],

                    [ "ß", $"{(char)225}" ],
               ];

                int index;
                while ((index = Fld.IndexOf('Ü')) != -1)
                {
                    string replacement = (index + 1 < Fld.Length && char.IsLower(Fld[index + 1])) ? "Ue" : "UE";
                    Fld = string.Concat(Fld.Substring(0, index), replacement, Fld.Substring(index + 1));
                }

                foreach (string[] replacement in replacements)
                {
                    Fld = Fld.Replace(replacement[0], replacement[1]);
                }
                return Fld;

        }
    }

    /// <summary>
    /// Ersetze alle relevanten Kleinbuchstaben durch Großbuchstaben (Akzentzeichen etc.)
    /// Die Reihenfolge ist wichtig, damit keine doppelten Ersetzungen passieren.
    /// </summary>
    /// <param name="sText">The s text.</param>
    /// <returns>string.</returns>
    public string Umlaute_UCase(string sText)
    {
        var replacements = new (string small, string capital)[]
        {
            ("ü", "Ü"), ("é", "É"), ("â", "Â"), ("ä", "Ä"), ("à", "À"), ("ç", "Ç"),
            ("ê", "Ê"), ("ë", "Ë"), ("è", "È"), ("ï", "Ï"), ("î", "Î"), ("ì", "Ì"),
            ("ô", "Ô"), ("ö", "Ö"), ("ò", "Ò"), ("û", "Û"), ("ù", "Ù"), ("á", "Á"),
            ("í", "Í"), ("ó", "Ó"), ("ú", "Ú"), ("ñ", "Ñ"), ("ß", "SS")
        };

        foreach (var (small, capital) in replacements)
        {
            int idx;
            while ((idx = sText.IndexOf(small, StringComparison.Ordinal)) != -1)
            {
                sText = sText.Substring(0, idx) + capital + sText.Substring(idx + small.Length);
            }
        }
        return sText;
    }
    public string Umlaute(string Fld)
    {

        (string, string)[] UmlDef = [
            ("Ä", "A"), ("ä", "a"), ("Ö", "O"), ("ö", "o"), ("Ü", "U"),
            ("ü", "u"), ("é", "e"), ("â", "a"), ("à", "a"), ("å", "a"),
            ("á", "a"), ("ç", "c"), ("ê", "e"), ("ë", "e"), ("è", "e"),
            ("ï", "l"), ("î", "l"), ("ì", "l"), ("í", "l"), ("Å", "A"),
            ("Á", "A"), ("É", "E"), ("ô", "o"), ("ò", "o"), ("ó", "o"),
            ("Ò", "O"), ("Ó", "O"), ("û", "u"), ("ù", "u"), ("ú", "u"),
            ("ÿ", "y"), ("ñ", "n"), ("Ñ", "N"), ("ß", "ss") ];

        foreach (var ud in UmlDef)
            Fld = Fld.Replace(ud.Item1, ud.Item2);

        return Fld;

    }

    #endregion
    public void DataModul_Texte_ForEachIdx(TexteIndex eTIdx, ETextKennz eTKennz, Action<int, string> action)
    {
        IRecordset dB_TexteTable = DataModul.DB_TexteTable;
        dB_TexteTable.Index = nameof(eTIdx);
        dB_TexteTable.Seek("=", eTKennz);
        while (!dB_TexteTable.EOF
           && !dB_TexteTable.NoMatch
           && dB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() == eTKennz)
        {
            int Texte_iTxNr = dB_TexteTable.Fields[TexteFields.TxNr].AsInt();
            string Texte_sText = dB_TexteTable.Fields[TexteFields.Txt].AsString().Replace("ssss", "ß");
            action(Texte_iTxNr, Texte_sText);
            dB_TexteTable.MoveNext();
        }
    }

    public void KTextlesTL5(ETextKennz eTKennz, IList oIIList, (string, ETextKennz) Bezeichnu)
    {
        DataModul_Texte_ForEachIdx(TexteIndex.KText, eTKennz, Handle_Text);

        static void AppendList((string sText, ETextKennz eTKnz) Bezeichnu, int Texte_iTxNr, string text, IList<ListItem<int>> oIList, ITextLesenViewModel textlesen)
        {
            var LiText = $"{text.Trim(),-240}{Texte_iTxNr}";
            if (LiText.Trim() != "")
            {
                oIList.Add(new(LiText, Texte_iTxNr));
            }

            textlesen.tTextBez = Bezeichnu;
        }

        void Handle_Text(int Texte_iTxNr, string Texte_sText)
        {
            IList<ListItem<int>> oIList = oIIList as IList<ListItem<int>>;
            ITextLesenViewModel textlesen = MainProject.Forms.Textlesen.ViewModel;

            if (textlesen.Text4_Text.Trim() == "")
            {
                AppendList(Bezeichnu, Texte_iTxNr, Texte_sText, oIList, textlesen);
            }
            else

                if (textlesen.Check3_Checked)
                {
                    if (Texte_sText.Contains(textlesen.Text4_Text))
                    {
                        AppendList(Bezeichnu, Texte_iTxNr, Texte_sText, oIList, textlesen);
                    }
                }
                else
                {
                    if (Texte_sText.ToUpper().Contains(textlesen.Text4_Text.ToUpper()))
                    {
                        AppendList(Bezeichnu, Texte_iTxNr, Texte_sText, oIList, textlesen);
                    }
                }
        }
    }

    public void KTextles(string Formnam, ETextKennz eTKennz, IList oIIList, (string sText, ETextKennz eTKnz) Bezeichnu)
    {
        //if (Information.Err().Number != 0)
        //{
        //    Mldg = "Fehler # " + Information.Err().Number.AsString() + " wurde ausgelöst von " + Information.Err().Source + "\r" + Information.Err().Description;
        //    _ = Interaction.MsgBox(Mldg, title: "Fehler", mb: MessageBoxButtons.OK);
        //}
        //goto IL_0e1d;
        DataModul_Texte_ForEachIdx(TexteIndex.KText, eTKennz, (i, s) => Handle_TextNr(Formnam, eTKennz, DataModul.DB_TexteTable, i, s));
        return;

        static void AppendList(int Texte_iTxNr, string text, IList oIList)
        {
            var LiText = $"{text.Trim(),-240}{Texte_iTxNr}";
            if (LiText.Trim() != "")
            {
                _ = oIList.Add(new ListItem<int>(LiText, Texte_iTxNr));
            }
        }

        void Handle_TextNr(string Formnam, ETextKennz eTKennz, IRecordset dB_TexteTable, int Texte_iTxNr, string text)
        {
            if (Formnam == "Ortsverw.List1")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.Ortsver.ListBox1.Items);
            }
            else if (Formnam == "Personen.List3")
            {
                AppendList(Texte_iTxNr, text, Personen.Default.List3.Items);
            }
            else if (Formnam == "Familie.List3")
            {
                AppendList(Texte_iTxNr, text, Familie.Default.ListBox3.Items);
            }
            else if (Formnam == "Ereignis.Liste2")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.Ereignis.ListBox2.Items);
            }
            else if (Formnam == "OFBL1")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.OFB.List1.Items);
            }
            else if (Formnam == "OFBL2")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.OFB.List4.Items);
            }
            else if (Formnam == "OFBL3")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.OFB.List3.Items);
            }
            else if (Formnam == "Partnerrecherche.List2")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.Partnerrecherche.List2.Items);
            }
            else if (Formnam == "Ortsteil")
            {
                var array = new string[22];
                var num6 = 1;
                while (!(dB_TexteTable.Fields[TexteFields.Kennz].AsEnum<ETextKennz>() != ETextKennz.I_)
                    && num6 <= 20)
                {
                    array[num6] = dB_TexteTable.Fields[TexteFields.TxNr].AsString();
                    dB_TexteTable.MoveNext();
                    num6++;
                }
                var num10 = 1;
                while (num10 <= num6)
                {
                    num10++;
                    if (DataModul.Place.ReadIdxData(PlaceIndex.OT, array[num10], out var cPlace))
                    {
                        Kont2[2] = cPlace!.sOrtsteil;
                        Kont2[1] = cPlace.sOrt;
                        LiText = Kont2[2] + "-" + Kont2[1];
                        _ = Interaction.MsgBox("Melden H5");
                    }
                }
            }
            else if (Formnam == "Rechtext.Liste1")
            {
                if (text.ToUpper().StartsWith(UbgT.ToUpper()))
                {
                    AppendList(Texte_iTxNr, text, MainProject.Forms.RechText.Liste1.Items);
                }

            }
            else if (Formnam == "RechText.Liste1")
            {
                AppendList(Texte_iTxNr, text, MainProject.Forms.RechText.Liste1.Items);
            }

        }
    }
    public (string sDat_Birth, string sDat_Death) Datles(int PersInArb, IPersonData person, bool xPlace = false)
    {
        var sDat_Death = "";
        var sDat_Birth = "";
        var Kont = new string[20];

        for (var _Iter2 = EEventArt.eA_Birth; _Iter2 <= EEventArt.eA_Burial; _Iter2++)
        { // <-- here
            var cEvt = DataModul.Event.ReadDataPl(_Iter2, PersInArb, out var xBreak);
            if (!xBreak)
            {
                var Datu = "";
                if (cEvt!.dDatumV != default)
                {
                    Datu = cEvt.dDatumV.AsString();
                    Ds = cEvt.sDatumV_S.AsString();

                    Datu = Datu.Date2DotDateStr();
                    Kont1[1] = Datu;
                    switch (_Iter2)
                    {
                        case EEventArt.eA_Birth:
                        case EEventArt.eA_Baptism when sDat_Birth == "":
                            sDat_Birth = $"{Datu,10}";
                            break;
                        case EEventArt.eA_Death:
                        case EEventArt.eA_Burial when sDat_Death == "":
                            sDat_Death = $"{Datu,10}";
                            break;
                    }

                    Kont1[1] += " " + cEvt.sDatumV_S;
                }


                if (cEvt.dDatumB != default)
                {
                    Datu = cEvt.dDatumB.AsString();
                    Datu = Datu.Date2DotDateStr();
                    Kont1[3] = Datu;
                }

                var sPlace = "";
                if (cEvt.iOrt > 0 && DataModul.Place.ReadData(cEvt.iOrt, out var cPlace))
                {
                    Kont2[6] = "";
                    if ("" != cEvt.sZusatz)
                    {
                        Kont2[6] = cEvt.sZusatz;
                    }

                    sPlace = DataModul.Place.FullName(cPlace, true, true);
                }
                if (!xPlace)
                {
                    Kont[(int)_Iter2 - 90] = Kont1[1] != "" ? IText[EUserText.t175] : " ";

                    Kont[(int)_Iter2 - 80] = sPlace != "" ? IText[EUserText.t175] : " ";

                }
                else
                {
                    Kont1[4] = cEvt.sDatumB_S;
                    Kont[(int)_Iter2 - 90] = Event_PreDisplay(xCitation: DataModul.SourceLink.Exists(3, _Modul1.Instance.PersInArb, _Iter2, LfNR)
                        || !string.IsNullOrWhiteSpace(cEvt.sBem[3]));
                    if ((cEvt.sBem[1].TrimEnd() != "")
                        || (cEvt.sBem[2].TrimEnd() != ""))
                    {
                        Kont[(int)_Iter2 - 85] = cEvt.sBem[1].TrimEnd();
                    }
                    Kont[(int)_Iter2 - 90] += Kont1[1] + " " + Kont1[2] + Kont1[3] + Kont1[4] + Kont1[5] + Kont1[6] + " " + sPlace;
                }
            }
        }
        person.SetDates(Kont, (sB, sD) => DateHelper2.CalcAge(sD, sB, _Modul1.Instance.IText));
        return (sDat_Birth, sDat_Death);
    }

    public void Listbox3Clip(IList lList, short A)
    {
        Clipboard.Clear();
        string text = "";
        if (A == 1)
        {
            text = "Bitte Seiteneinstellung A4 Querformat einstellen!\r\n\r\n";
        }
        short num = (short)(lList.Count - 1);
        short num2 = 0;
        while (num2 <= num)
        {

            text += lList[num2].ToString() + "\r\n";
            num2++;
        }

        Persistence.WriteStringTemp("Text4.Txt", text);
        if (Typ != DriveType.CDRom)
        {
            Persistence.WriteStringProg("Text4.Txt", text);
        }

        Ausdruck("Text4.Txt");
    }

    public string BuildFullSurName(IPersonData person, bool xFamToUpper = true)
    {
        string FullSurName = person.SurName.TrimEnd();
        FullSurName = xFamToUpper ? FullSurName.ToUpper() : FullSurName;

        if (person.Prefix != "")
        {
            FullSurName = person.Prefix.TrimEnd() + " " + FullSurName;
        }
        if (person.Suffix != "")
        {
            FullSurName = FullSurName.TrimEnd() + " " + person.Suffix.TrimEnd();
        }
        return FullSurName;
    }

    public Bitmap PicResizeByWidth(Image SourceImage, int Newheigth)
    {
        decimal d = new(Newheigth / (double)SourceImage.Height);
        int width = Convert.ToInt32(decimal.Multiply(d, new decimal(SourceImage.Width)));
        Bitmap bitmap = new(width, Newheigth);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle rect = new(0, 0, width, Newheigth);
            graphics.DrawImage(SourceImage, rect);
        }
        return bitmap;
    }

    public Bitmap PicResizeByHeigth(Image SourceImage, int Newwidth)
    {
        decimal d = new(Newwidth / (double)SourceImage.Width);
        int height = Convert.ToInt32(decimal.Multiply(d, new decimal(SourceImage.Height)));
        Bitmap bitmap = new(Newwidth, height);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Rectangle rect = new(0, 0, Newwidth, height);
            graphics.DrawImage(SourceImage, rect);
        }
        return bitmap;
    }

    public Image AutoSizeImage(Image oBitmap, int maxWidth, int maxHeight, bool bStretch = false)
    {
        //Discarded unreachable code: IL_00ba, IL_00c1
        float num = (float)(maxWidth / (double)maxHeight);
        int width = oBitmap.Width;
        int height = oBitmap.Height;
        float num2 = (float)(width / (double)height);
        if (width > maxWidth || height > maxHeight || bStretch)
        {
            checked
            {
                if (num2 <= num)
                {
                    width = (int)Math.Round(width / (height / (double)maxHeight));
                    height = maxHeight;
                }
                else
                {
                    height = (int)Math.Round(height / (width / (double)maxWidth));
                    width = maxWidth;
                }
                Bitmap bitmap = new(width, height);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Rectangle rect = new(0, 0, width, height);
                    graphics.DrawImage(oBitmap, rect);
                }
                return bitmap;
            }
        }
        return oBitmap;
    }


    public IList<T> DeleteDoublicates<T>(IList<T> oList)
    {
        var oResult = new List<T>();
        foreach (T o in oList)
        {
            if (!oResult.Contains(o))
            {
                oResult.Add(o);
            }
        }

        return oResult;
    }

    public void RKSchutz()
    {
        if (cMandDrive.DriveType != DriveType.CDRom)
        {
            FileSystem.FileClose(99);
            if (Typ != DriveType.CDRom)
            {
                FileSystem.FileOpen(99, Verz1 + "\\RK", OpenMode.Append);
                FileSystem.FileClose(99);
            }
            var num4 = Persistence.ReadIntMand("RK") + 1;
            Persistence.WriteIntMand("RK", num4);
            if (num4 > 5)
            {
                FileSystem.Kill(Verz1 + "IDF.Dat");
                FileSystem.Kill(Verz1 + "OFBIDF.Dat");
            }
            if (num4 > 50)
            {
                ProjectData.EndApp();
            }
            FileSystem.FileClose(99);
            //=================
        }
    }

    public void Ausdruck(string Datnam)
    {
        if (Typ == DriveType.CDRom)
        {
            _ = Process.Start(TempPath + Datnam, 0.AsString());
            return;
        }
        else
            _ = Interaction.Shell(Aus[(int)EOutCfg.o07_KeepFormat] + " " + GenFreeDir + "\\Temp" + Datnam, (int)AppWinStyle.NormalFocus);
    }

    public bool GeolesPlace(IPlaceData cPlace, Action<(string, string)>? action = null, bool xAppend = true)
    {
        string sLongitude = "";
        string sLatiude = "";
        string Place_sLat = cPlace.sB;
        string Place_sLong = cPlace.sL;
        if (Place_sLat.AsString().Trim() != "")
        {
            sLatiude = ConvKoordinate(Place_sLat, "B", action);

            if (Place_sLong.Trim() != "")
            {
                var num3 = 0;
                if (Place_sLong.IndexOf(',') == 0f)
                {
                    _ = Interaction.MsgBox("Fehler bei der Längenangabe");
                }
                else
                {
                    sLongitude = ConvKoordinate(Place_sLong, "L", action);
                }
            }
        }
        Kont2[1] = cPlace.sOrt;
        Kont2[2] = cPlace.sOrtsteil;
        string sName = Kont2[1];
        globOrt2 += Kont2[1];
        if ((sLongitude.AsInt() > 0.0) || (sLatiude.AsInt() != 0.0))
        {
            if (Kont2[2].Trim() != "")
            {
                sName = sName + "-" + Kont2[2].Trim();
            }
            GEExportPlace(sName, (sLongitude, sLatiude), xAppend);
            return true;
        }
        return false;
    }

    private string ConvKoordinate(string Place_sKoor, string v, Action<(string, string)>? action)
    {
        string sKoordinate = "";
        int num3 = Place_sKoor.IndexOf(',');
        if (num3 != 0)
        {
            sKoordinate = Place_sKoor.Left(num3 - 1) + ".";
            action?.Invoke((v + ".h", sKoordinate));
            var num4 = Place_sKoor.Substring(num3 + 1, 2).AsDouble() / 60.0 * 100.0;
            action?.Invoke((v + ".m", Place_sKoor.Substring(num3 + 1, 2)));
            var num5 = Place_sKoor.Substring(num3 + 3, 2).AsDouble() / 3600.0 * 100.0;
            action?.Invoke((v + ".s", Place_sKoor.Substring(num3 + 3, 2)));
            num4 += num5;
            num4 *= 100f;
            num3 = (int)Math.Round(num4);
            string text3 = $"{num3:D4}";
            sKoordinate += text3;
        }
        else
            action?.Invoke((v + ".h", Place_sKoor));
        return sKoordinate;
    }

    public void GEExportPlace(string sName, (string sLongitude, string sLatiude) cKoords, bool xAppend = true)
    {
        var lines = new List<string>();
        if (!Persistence.ExistFileTemp("GenPluswin.kml") || !xAppend)
        {
            lines.AddRange([ "<?xml version='1.0' encoding='Ansi'?>",
                "<kml xmlns='http://earth.google.com/kml/2.1'>",
                "<Document>",
                "<name>GenPluswin.kml</name>"]);
        }
        lines.AddRange(
        [
            "<Placemark>",
                $"<name>{sName}</name>",
                "<LookAt>",
                    $"<longitude>{cKoords.sLongitude}</longitude>",
                    $"<latitude>{cKoords.sLatiude}</latitude>",
                    "<range>1000</range>",
                "</LookAt>",
                    "<Point>",
                    $"<coordinates>{cKoords.sLongitude},{cKoords.sLatiude},0</coordinates>",
                "</Point>",
            "</Placemark>"
        ]);
        if (!xAppend)
        {
            lines.AddRange(["</Document>", "</kml>"]);
            Persistence.WriteStringsTemp("GenPluswin.kml", lines);
        }
        else
            Persistence.AppendStringsTemp("GenPluswin.kml", lines);
    }

    //    public bool Bildzeig1(string biart, int PBW, int PBH, string Form, out string BiText1, out string Bitext2)
    // Schritt 1: Pseudocode
    // 1. Extrahiere die Bildlade- und Bildprüflogik in eine eigene Methode, die nur die Bilddaten liefert (ohne UI-Logik).
    // 2. Die UI-Logik (Zuweisung zu PictureBox/Image) wird in eine eigene Methode ausgelagert, die das Ergebnis der Bilddaten-Methode verwendet.
    // 3. Die Hauptmethode ruft die Datenmethode auf und gibt die Bilddaten zurück, die UI-Methode kann separat aufgerufen werden.

    // Hilfsstruktur für Bilddaten
    public class PictureData
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public Bitmap Bitmap { get; set; }
        public bool IsValid { get; set; }
    }

    // Daten-orientierte Methode: Liefert Bilddaten für einen Datensatz
    public PictureData? GetPictureData(string biart, int persOrQuId, string kennz, string pfad, string datei, string beschreibung, string bem, DriveType typ, string verz)
    {
        var fileName = Path.Combine(pfad, datei);
        if (fileName.StartsWith("#"))
            fileName = verz + fileName.Substring(1);

        if (typ != DriveType.CDRom && !Persistence.ExistFile(fileName))
            return null;
        if (!CheckFileExt(fileName))
            return null;

        Bitmap bitmap;
        try
        {
            if (typ != DriveType.CDRom)
            {
                using (var fs = new FileStream(fileName, FileMode.Open))
                    bitmap = new Bitmap(fs);
            }
            else
            {
                bitmap = new Bitmap(fileName);
            }
        }
        catch
        {
            return null;
        }

        return new PictureData
        {
            FileName = fileName,
            Description = beschreibung,
            Text = bem,
            Bitmap = bitmap,
            IsValid = true
        };
    }

    // UI-orientierte Methode: Zeigt das Bild in einer PictureBox an
    public void ShowPictureInUI(PictureData picData, PictureBox pictureBox, int pbw, int pbh)
    {
        if (picData == null || picData.Bitmap == null)
            return;
        float ratio = (float)picData.Bitmap.Width / picData.Bitmap.Height;
        if (ratio > 1)
            pictureBox.Image = PicResizeByHeigth(picData.Bitmap, pbw);
        else
            pictureBox.Image = PicResizeByWidth(picData.Bitmap, pbh);
    }

    // Hauptmethode: Liefert Bilddaten, UI muss separat aufgerufen werden
    public bool Bildzeig1(string biart, int PBW, int PBH, string Form, out string BiText1, out string Bitext2)
    {
        BiText1 = "";
        Bitext2 = "";
        IRecordset dB_PictureTable = DataModul.DB_PictureTable;
        dB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        int num5 = 0;
        string kennz = "";
        if (Form == "Quellverw")
        {
            if (DataModul.DB_QuTable.Fields[QuFields._1].AsInt() > 0)
            {
                dB_PictureTable.Seek("=", "Q", DataModul.DB_QuTable.Fields[QuFields._1].Value);
                num5 = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
                kennz = "Q";
            }
        }
        else if (Form == "Personen")
        {
            dB_PictureTable.Seek("=", sPKennz, PersInArb);
            num5 = PersInArb;
            kennz = sPKennz;
        }
        if (dB_PictureTable.NoMatch)
            return false;

        while (!dB_PictureTable.EOF)
        {
            string beschreibung = dB_PictureTable.Fields[PictureFields.Beschreibung].AsString();
            int zuNr = dB_PictureTable.Fields[PictureFields.ZuNr].AsInt();
            if (beschreibung != biart || zuNr != num5)
            {
                dB_PictureTable.MoveNext();
                continue;
            }

            var picData = GetPictureData(
                biart,
                num5,
                kennz,
                dB_PictureTable.Fields[PictureFields.Pfad].AsString(),
                dB_PictureTable.Fields[PictureFields.Datei].AsString(),
                beschreibung,
                dB_PictureTable.Fields[PictureFields.Bem].AsString(),
                Typ,
                Verz
            );

            if (picData != null && picData.IsValid)
            {
                BiText1 = !string.IsNullOrEmpty(picData.Description) ? $"Name: {picData.Description}" : "";
                if (!string.IsNullOrEmpty(picData.Text))
                    BiText1 += $"\nText: {picData.Text}";
                Bitext2 = picData.FileName;

                // UI-Aufruf muss separat erfolgen, z.B.:
                // ShowPictureInUI(picData, <PictureBox>, PBW, PBH);

                return true;
            }
            dB_PictureTable.MoveNext();
        }
        return false;
    }

    public bool Bildzeig2(string biart, int PBW, int PBH, string Form, out string BiText1, out string Bitext2)
    {
        var flag2 = false;
        var num5 = 0;
        var ja = false;
        BiText1 = "";
        Bitext2 = "";
        IRecordset dB_PictureTable = DataModul.DB_PictureTable;
        dB_PictureTable.Index = nameof(PictureIndex.PerKenn);
        string left = Form;
        if (left == "Quellverw")
        {
            if (DataModul.DB_QuTable.Fields[QuFields._1].AsInt() > 0)
            {
                dB_PictureTable.Seek("=", "Q", DataModul.DB_QuTable.Fields[QuFields._1].Value);
                num5 = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
            }
        }
        else if (left == "Personen")
        {
            dB_PictureTable.Seek("=", sPKennz, PersInArb);
            num5 = PersInArb;
        }
        if (dB_PictureTable.NoMatch)
        {
            goto end_IL_0001_2;
        }

        ja = true;

        while (!dB_PictureTable.EOF)
        {
            string Picture_sDesctiption = dB_PictureTable.Fields[PictureFields.Beschreibung].AsString();
            int Picture_iZuNr = dB_PictureTable.Fields[PictureFields.ZuNr].AsInt();

            if (Picture_sDesctiption != biart)
            {
                dB_PictureTable.MoveNext();
                continue;
            }
            if (!(Picture_iZuNr != num5))
            {
                var DateiName = Path.Combine(
                     dB_PictureTable.Fields[PictureFields.Pfad].AsString(),
                     dB_PictureTable.Fields[PictureFields.Datei].AsString());
                if (DateiName.Left(1) == "#")
                {
                    DateiName = Verz + Strings.Mid(DateiName, 2, DateiName.Length);
                }

                Bitmap bitmap2;
                if (Typ != DriveType.CDRom)
                {
                    if (!Persistence.ExistFile(DateiName))
                    {
                        _ = Interaction.MsgBox("Das Medium " + DateiName + " ist nicht vorhanden\nDie Verbindung kann über >Medien >Bilder ansehen gelöscht werden");
                        dB_PictureTable.MoveNext();
                        continue;
                    }
                    else if (!CheckFileExt(DateiName))
                    {
                        dB_PictureTable.MoveNext();
                        continue;
                    }

                    FileStream fileStream = new FileStream(DateiName, FileMode.Open);
                    bitmap2 = new Bitmap(fileStream);
                    fileStream.Close();
                }
                else
                {
                    bitmap2 = new Bitmap(DateiName);
                }

                int width2 = bitmap2.Width;
                int height2 = bitmap2.Height;
                float num6 = (float)(width2 / (double)height2);
                string left2 = Form;
                string text = "";
                if (left2 == "Quellverw")
                {
                    if (num6 > 1)
                    {
                        text = PBW.AsString();
                        PictureBox pictureBox2 = MainProject.Forms.Quellverw.PictureBox1;
                        pictureBox2.Image = PicResizeByHeigth(bitmap2, PBW);
                        pictureBox2 = null;
                        //=================
                    }
                    else
                    {
                        text = PBH.AsString();
                        PictureBox pictureBox3 = MainProject.Forms.Quellverw.PictureBox1;
                        pictureBox3.Image = PicResizeByWidth(bitmap2, PBH);
                        pictureBox3 = null;
                        //=================
                    }
                    //=================
                }
                else if (left2 == "Personen")
                {
                    if (num6 > 1)
                    {
                        text = PBW.AsString();
                        PictureBox pictureBox4 = Personen.Default.Picture1;
                        pictureBox4.Image = PicResizeByHeigth(bitmap2, PBW);
                        pictureBox4 = null;
                        //=================
                    }
                    else
                    {
                        text = PBH.AsString();
                        PictureBox pictureBox5 = Personen.Default.Picture1;
                        pictureBox5.Image = PicResizeByWidth(bitmap2, PBH);
                        pictureBox5 = null;
                    }
                    //=================
                }
                Bitext2 = DateiName;
                if (null != Picture_sDesctiption)
                {
                    BiText1 = ((("Name: ") + (
                        Picture_sDesctiption))).AsString();
                }
                if (null != dB_PictureTable.Fields[PictureFields.Bem].Value)
                {
                    BiText1 = ((string.Concat(BiText1 + "\n", "Text: ")) + (dB_PictureTable.Fields[PictureFields.Bem].Value)).AsString();
                }
                flag2 = true;
            }
        }
        if (Aus[(int)EOutCfg.o48] == "Y"
            || flag2)
        {
            goto end_IL_0001_2;
        }

        string left3 = Form;
        if (left3 == "Quellverw")
        {
            dB_PictureTable.Seek("=", "Q", DataModul.DB_QuTable.Fields[QuFields._1].Value);
            num5 = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();
            //=================
        }
        else if (left3 == "Personen")
        {
            dB_PictureTable.Seek("=", sPKennz, PersInArb);
            num5 = PersInArb;
        }

        if (dB_PictureTable.NoMatch)
            goto end_IL_0001_2;
        while (!dB_PictureTable.EOF
            && dB_PictureTable.Fields[PictureFields.ZuNr].AsInt() == num5)
        {
            var DateiName = Path.Combine(
                dB_PictureTable.Fields[PictureFields.Pfad].AsString(), dB_PictureTable.Fields[PictureFields.Datei].AsString());
            if (DateiName.Left(1) == "#")
            {
                DateiName = Verz + Strings.Mid(DateiName, 2, DateiName.Length);
            }

            Bitmap bitmap;
            //=================
            if (Typ != DriveType.CDRom)
            {
                if (!Persistence.ExistFile(DateiName))
                {
                    _ = Interaction.MsgBox("Das Medium " + DateiName + " ist nicht vorhanden\nDie Verbindung kann über >Medien >Bilder ansehen gelöscht werden");
                    dB_PictureTable.MoveNext();
                    continue;
                }
                else if (!CheckFileExt(DateiName))
                {
                    dB_PictureTable.MoveNext();
                    continue;
                }

                FileStream fileStream2 = new FileStream(DateiName, FileMode.Open);
                bitmap = new Bitmap(fileStream2);
                fileStream2.Close();
                //=================
            }
            else
            {
                bitmap = new Bitmap(DateiName);
            }

            int width2 = bitmap.Width;
            int height2 = bitmap.Height;
            float num7 = (float)(width2 / (double)height2);
            string left4 = Form;
            string text = "";
            if (left4 == "Quellverw")
            {
                if (num7 > 1f)
                {
                    text = PBW.AsString();
                    PictureBox pictureBox6 = MainProject.Forms.Quellverw.PictureBox1;
                    pictureBox6.Image = PicResizeByHeigth(bitmap, PBW);
                    pictureBox6 = null;
                    //=================
                }
                else
                {
                    text = PBH.AsString();
                    PictureBox pictureBox7 = MainProject.Forms.Quellverw.PictureBox1;
                    pictureBox7.Image = PicResizeByWidth(bitmap, PBH);
                    pictureBox7 = null;
                    //=================
                }
                //=================
            }
            else if (left4 == "Personen")
            {
                if (num7 > 1f)
                {
                    text = PBW.AsString();
                    PictureBox pictureBox8 = Personen.Default.Picture1;
                    pictureBox8.Image = PicResizeByHeigth(bitmap, PBW);
                    pictureBox8 = null;
                }
                else
                {
                    text = PBH.AsString();
                    PictureBox pictureBox = Personen.Default.Picture1;
                    pictureBox.Image = PicResizeByWidth(bitmap, PBH);
                    pictureBox = null;
                }
            }

            Bitext2 = DateiName;
            if (null != dB_PictureTable.Fields[PictureFields.Beschreibung].AsString())
            {
                BiText1 = Microsoft.VisualBasic.CompilerServices.Conversions.ToString((("Name: ") + (dB_PictureTable.Fields[PictureFields.Beschreibung].AsString())));
            }
            if (null != dB_PictureTable.Fields[PictureFields.Bem].AsString())
            {
                BiText1 = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(((string.Concat(BiText1 + "\n", "Text: ")) + (dB_PictureTable.Fields[PictureFields.Bem].AsString())));
                //=================
            }
            break;
        }
        goto end_IL_0001_2;
        //=================

    end_IL_0001_2: // <========== 11
        return ja;
    }

    private bool CheckFileExt(string DateiName)
        => DateiName.ToUpper().Right(4) switch
        {
            ".JPG" or "JPEG" or ".GIF" or ".TIF" or ".BMP" or ".PNG" => true,
            _ => false,
        };


    public string Rech(ref string datum1, ref string datum2)
    {
        throw new NotImplementedException();
    }

    public void Famdatles(int FamInArb, out string[] Kont)
    {
        throw new NotImplementedException();
    }

    public void Paten2(int PersInArb, ref string Pattext, long Ahne)
    {
        throw new NotImplementedException();
    }

    public int Person_TextSpeichern(string sText, int iPerson, ETextKennz eKennz1, int iLfNR = 0, short Ruf = 0)
    {
        return TextSpeich(sText, "", eKennz1, iPerson, iLfNR, Ruf != 0);
    }

    public void Perles(int PersInArb)
    {
        throw new NotImplementedException();
    }

    public void Datles(int Ubg, int PersInArb)
    {
        throw new NotImplementedException();
    }

    public string ortles1(int OrtNr, byte Schalt = 0, Action<int, string> action = null)
    {
        DataModul.Place.ReadData(OrtNr, out var cPlace);
        return Ortles(cPlace, Schalt, action)[0];
    }

    public string[] Ortles(IPlaceData cPlace, int Schalt = 0, Action<int, string> export = null)
    {
        var Kont2 = new string[7];
        Kont2.Initialize();

        var cFields = (Schalt == 0) ? new[] {
            EPlaceProp.sOrt,
            EPlaceProp.sOrtsteil,
            EPlaceProp.sKreis,
            EPlaceProp.sLand,
            EPlaceProp.sStaat } :
            [ EPlaceProp.sOrt,
            EPlaceProp.sOrtsteil ];

        foreach (var f in cFields)
            Kont2[(int)f + 1 - (int)EPlaceProp.sOrt] = cPlace.GetPropValue<string>(f);

        Kont2[(int)PlaceFields.Ortsteil + 1] = Kont2[(int)PlaceFields.Ortsteil + 1].FrameIfNEoW("-", "");

        Kont2[0] = string.Join(", ", Kont2.Where((s) => !string.IsNullOrWhiteSpace(s))).Replace(", -", "-");
        export?.Invoke(cPlace.iOrt, Kont2[1] + Kont2[2]);
        return Kont2;
    }

    public void Orttextspeichern()
    {
        throw new NotImplementedException();
    }

    public void Ahnles(int PersInArb, out string[] asAhnData)
    {
        throw new NotImplementedException();
    }

    public void Erei(int PersInArb, EEventArt eArt, ref byte PerPos)
    {
        throw new NotImplementedException();
    }

    public void Famdatles2()
    {
        throw new NotImplementedException();
    }

    public void OFBTextPruefenSpeichern(string UbgT, string Kennz, int LfNR)
    {
        throw new NotImplementedException();
    }


    public void GedAus_Diskvoll()
    {
        throw new NotImplementedException();
    }

    public int Persatzles(int PersInArb)
    {
        throw new NotImplementedException();
    }


    public string Conform(string sText)
    {
        throw new NotImplementedException();
    }

    public void Sperrfehler()
    {
        throw new NotImplementedException();
    }

    public void Quellenaus(EEventArt L)
    {
        throw new NotImplementedException();
    }

    public string Strings_Leerweg1(string sText)
    {
        throw new NotImplementedException();
    }

    public void Sichwand(string Dasich, string sDatumV_S, DateTime dDatumB, EEventArt eArt)
    {
        throw new NotImplementedException();
    }

    public void Lerz(ref short A, ref int u)
    {
        throw new NotImplementedException();
    }

    public void Paten_O_Taufe()
    {
        throw new NotImplementedException();
    }

    public string Datwand1(string Datu, string Ds)
    {
        throw new NotImplementedException();
    }

    public void Datles(int PersInArb, out IList<string> asPersDates)
    {
        throw new NotImplementedException();
    }

    public string ortles(int OrtNr, byte Schalt)
    {
        throw new NotImplementedException();
    }

    public void Zeugsu(EEventArt Art, short LfNR, short Listart, long Ahne)
    {
        throw new NotImplementedException();
    }

    public void FrmPerson_EventUpd(int PersInArb)
    {
        throw new NotImplementedException();
    }

    public void ExportPlace(int OrtNr, string sOrt, string ind1, string namen)
    {
        throw new NotImplementedException();
    }

    public void Datles3(short listart, long v, object value, ref bool neb)
    {
        throw new NotImplementedException();
    }

    public void Datles10(ref short listart, bool m1_Ki)
    {
        throw new NotImplementedException();
    }

    public int System_TestForm_Height() => MainProject.Forms.Test1.Height;

    public short Strings_Lerz(string s)
    {
        throw new NotImplementedException();
    }

    public int DataModul_PeekMandant_RecordCount()
    {
        throw new NotImplementedException();
    }

    public void Family_Les(int famInArb, IFamilyData family)
    {
        DataModul.Link.ReadFamily(famInArb, family);
    }

    // Methods required by Druck project
    public void Datles2()
    {
        // TODO: Implement Datles2
        throw new NotImplementedException();
    }

    public string Person_FullSurname(IPersonData person, bool xFamToUpper)
    {
        return BuildFullSurName(person, xFamToUpper);
    }

    public void Person_FullSurname(IList<string> kont, bool v)
    {
        kont[0] = BuildFullSurName(Person, v);
    }

    public void Famdatles1(int schalt, out string[] asFamDate)
    {
        // TODO: Implement Famdatles1
        asFamDate = new string[20];
        throw new NotImplementedException();
    }

    public void PerSatzLes(int persInArb)
    {
        // TODO: Implement PerSatzLes
        throw new NotImplementedException();
    }

    public void Famles()
    {
        // TODO: Implement Famles
        throw new NotImplementedException();
    }
}
