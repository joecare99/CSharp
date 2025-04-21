using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using Gedcomles.Model;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using GenFree.Data;
using GenFree.Interfaces.UI;
using System.Windows.Forms;
using GenFree.Interfaces.Model;

namespace GenFree.Interfaces.Sys;

public interface IModul1
{
    public enum MSOfficeComponent
    {
        MSAccess,
        MSExcel,
        MSOutlook,
        MSPowerPoint,
        MSWord
    }

    public enum MSOfficeVersion
    {
        Office95 = 7,
        Office97,
        Office2000,
        OfficeXP,
        Office2003,
        Office2007
    }

    public struct Geocode
    {
        public string Longitude;
        public string Latitude;
    }

    public struct Frauen
    {
        public int PERS;
        public Type Kek1;
        public int Kek2;
        public short Gen;
    }

    public struct Adresse
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        public char[] Teext;
    }

    public struct Ausgab
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public char[] F;
    }

    public struct Letzter
    {
        public int iPerson;
        public int iFamily;
    }

    float Aschalt { get; set; }
    /// <summary>Gets the background color.</summary>
    /// <value>The background color.</value>
    Color HintFarb { get; set; }
    Color Feld1Farb { get; set; }
    Color ErFarb { get; set; }
    Color Farb { get; set; }
    Color Farb1 { get; set; }
    Color Hintfarb1 { get; set; }

    short Feg { get; set; }
    /// <summary>Gets the font size.</summary>
    /// <value>The font size.</value>
    float Fs { get; set; }
    int PersInArb { get; set; }

    /// <summary>Gets the person texts.</summary>
    /// <value>The person texts.</value>
    IList<string> Kont1 { get; }
    IList<string> DAus { get; }
    IList<string> Aus { get; }
    IList<string> Quells { get; }
    IList<string> KontM { get; }
    IList<string> KontP { get; }
    IList<string> KontF { get; }
    IList<string> Kont { get; }
    IList<string> Absend { get; }
    IList<string> TxT { get; }

    bool EreiRf { get; set; }
    /// <summary>Gets the printing texts.</summary>
    /// <value>The printing texts.</value>
    IList<string> DTxt { get; set; }

    // Database
    IRecordset OrtSTable { get; set; }
    IRecordset DT_OrtIdxTable { get; set; }

    string AppName { get; }
    string Author { get; }
    string VendorName { get;}
    /// <summary>
    /// "Version DD.MM.YYYY {VersDat}"
    /// </summary>
    /// <value>The version.</value>
    string Version { get;  }
    /// <summary>
    /// "{AppName} {Description}"
    /// </summary>
    string VersionT { get; }
    /// <summary>
    /// "Special Version"
    /// </summary>    
    string Version2 { get; }
    /// <summary>
    /// "(c) YYYY {Author}"
    /// </summary>
    string Version1 { get; }
    /// <summary>
    /// "Stand DD.MM.YYYY"
    /// </summary>
    string VersDat { get; }
    /// <summary>
    /// "(c) YYYY-YYYY {Author} {Kontakt}"
    /// </summary>
    string Titel2 { get; }

    // FileSystem
    string InitDir { get; }
    string TempPath { get; }
    string GenFreeDir { get; }
    string ListDir { get; }
    string HelpDir { get; }
    string Verz { get; set; }
    string Verz1 { get; set; }
    string MainProg { get;  }
    DriveInfo cMandDrive { get; set; }
    DriveType Typ { get; }

    IPersistence Persistence { get; }

    // User
    int thisYear { get; }

    // =================
    byte Programtesttemp { get; set; }
    string AutoupD { get; set; }
    bool Demo { get; set; }
    bool FAendmerk { get; set; }
    bool PAendmerk { get; set; }

    string Lw { get; set; }
    float Aend { get; set; }
    float FontSize { get; set; }
    bool Aendf { get; set; }
    int FamInArb { get; set; }
    int PFSatz { get; set; }
    int Startpers { get; set; }

    IFamilyData Family { get; set; }
    IPersonData Person { get; set; }
    
    TGedLine Eing { get; set; }

    ELinkKennz eLKennz { get; set; }
    EEventArt Art { get; set; }
    ETextKennz eNKennz { get; set; }
    string sPKennz { get; set; }
    int eWKennz { get; set; }

    string UbgT { get; set; }
    string UbgT1 { get; set; }
    
    string AppHostName { get; }
    Enum WindowState { get; set; }
    IApplUserTexts IText { get; }
    int Uml { get; set; }
    int Histor { get; set; }
    int Quell { get; set; }
    [Obsolete]
    int Ubg { get; set; }
    int FeG { get; set; }

    IGedAus GedAus { get; }
    IEinles Einles { get; }
    string Zusatzquelle { get; set; }
    string Menue_Ziel { get; }
    string NamenSuch_Wort { get; set; }
    /// <summary>
    /// Druck: FaBu - Gets or sets the old name.
    /// </summary>
    /// <value>The name of the alt.</value>
    string AltName { get; set; }
    /// <summary>
    /// Druck: Gets or sets the old nr.
    /// </summary>
    /// <value>The alt nr.</value>
    int AltNr { get; set; }
    short LfNR { get; set; }
    byte Schalt { get; set; }
    string sDatu { get; set; }
    int Datklein { get; set; }

     short Les { get; set; }
    
    short Druck_Tast { get; set; }
    string cNoChangesOnCD { get; }
    Letzter Letzte { get; set; }
 
    
    byte Suchschalt { get; set; }
    int Suchfam { get; set; }
    int SuchPer { get; set; }
    short Trans { get; set; }
    string Inhaber { get; set; }
    
    short ErSchalt { get; set; }
    int FamPerschalt { get; set; }
    int Nr { get; set; }
    IList<string> Te { get; }

    int AendPruef(int PersInArb, int ubg2=0);
    void Ahnles(int PersInArb, out string[] asAhnData);
    DateTime AtomicTime(string sTimeServer);
    string Conform(string sText);
    string Datwand1(string Datu, string Ds);
    void Dateienopen();
    void Datles(int Ubg, int PersInArb);
    void Datles(int PersInArb, out IList<string> asPersDates);
    string Datwand(string Dat);
    void DezRechnen(ref string A4);
    void Diskvoll();
    IEnumerable<string> EnumerateMandants(string drive);
    void Erei(int PersInArb, EEventArt eArt, ref byte PerPos);
    IList<int> Famsuch(int PersInArb, ELinkKennz eLKennz);
    void Famdatles(int FamInArb, out string[] Kont);
    void Famdatles2();
    void FrmFamily_fameinlesen(int FamInArb, out short Rich);
     string GoogleInstallPath();
    void Info();
    [Obsolete("Use File.Exists")]
    bool IstDa(string DateiName);
    short IsFormloaded(Form Formtocheck);
    void Lerz(ref short A, ref int u);
    string Leerweg1(string sText);
    void OFBTextPruefenSpeichern(string UbgT, string Kennz, int LfNR);
    bool OfficeAppInstalled(MSOfficeComponent nComponent);
    string OfficeInstallPath(MSOfficeVersion nVersion);
    object OpenRecordSet();

    void ortles(int OrtNr, ref byte Schalt);
    void ortles(ref int Ortnr);
    string ortles(int OrtNr, byte Schalt);
    void Orttextspeichern();

    void Paten_O_Taufe();
    void Paten2(int PersInArb, ref string Pattext, long Ahne);
    void Perles(int PersInArb);
    int Persatzles(int PersInArb);
    void Person_ReadNames(int PersInArb, IPersonData person);
    int Person_TextSpeichern(int iPerson, string sText, ETextKennz eKennz1, int iLfNR = 0, short Ruf = 0);
    void Quellenaus(EEventArt L);
    string Rech(ref string datum1, ref string datum2);
    void Sichwand(string Dasich, string sDatumV_S, DateTime dDatumB, EEventArt eArt);
    void Sperrfehler();
    void STextles(string Formnam, ETextKennz Kennz, string UbgT, ListBox.ObjectCollection ocItems);
    int TextSpeich(string sText, string sLeitName, ETextKennz eTKennz, int PersInArb = 0, int LfNR = 0);
    void TextTeilen(string UbgT, string UbgT4, string Kennung);
    string Umlaute4(string Fld, int uml);
    void Vornam_Namles(int personNr);
    string Wortlesen(int Satz, string Kennz);

    void Zeugsu(EEventArt Art, short LfNR, short Listart, long Ahne);
    //--
    void Historie(string UbgT);
    [Obsolete("Auftrennen in 3 Funktionen")]




}
