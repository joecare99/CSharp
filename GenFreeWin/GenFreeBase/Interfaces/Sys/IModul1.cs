using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Interfaces.DB;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using GenFree.Data;
using GenFree.Interfaces.UI;
using GenFree.Interfaces.Model;
using GenFree.Interfaces.Data;
using GenFree.Helper;
using System.Collections;
using GenFree.Data.Models;
using GenFree.Interfaces.VB;

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
        public TypeCode Kek1;
        public int Kek2;
        public short Gen;

        public void SetKek1(int v)
        {
            throw new NotImplementedException();
        }
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
    IList<ESearchSelection> Suchfeld { get; }

    bool EreiRf { get; set; }
    /// <summary>Gets the printing texts.</summary>
    /// <value>The printing texts.</value>
    IList<string> DTxt { get; set; }

    // Database
    IRecordset DgbTable { get; set; }
    IRecordset DT_AhnTable { get; set; }

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
    /// <summary>
    /// Gets the initialization dir.
    /// </summary>
    /// <value>The initialization directory with default values.</value>
    string InitDir { get; }
    string InstPath { get; }
    string TempPath { get; }
    string GenFreeDir { get; }
    string ListDir { get; }
    string HelpDir { get; }
    /// <summary>
    /// Gets or sets the path to the mandant.
    /// </summary>
    /// <value>The (full) path to the mandant.</value>
    string Verz { get; set; }
    /// <summary>
    /// Gets the picture dir.
    /// </summary>
    /// <value>The path of picture .</value>
    string PictureDir { get;}

    string Verz1 { get; set; }
    string MainProg { get;  }
    /// <summary>
    /// Gets or sets the name of the mandant.
    /// </summary>
    /// <value>The name of the mandant.</value>
    string Mandant { get; set; }
    DriveInfo cMandDrive { get; set; }
    DriveType Typ { get; }

    IGenPersistence Persistence { get; }

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
    ETextKennz eTKennz { get; set; }
    short Qkenn { get; set; }

    string UbgT { get; set; }
    string UbgT1 { get; set; }
    
    string AppHostName { get; }
    Enum eWindowState { get; set; }
    EWindowSize eWindowSize { get; set; }
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
    /// <summary>
    /// Gets the Text-Substitution-List.
    /// </summary>
    /// <value>The Text-substitution-list. Contains the substitution for single keys.</value>
    IList<string> Te { get; }
    bool Reli { get; set; }
    int VerS { get; set; }
    int Frauenkek1 { get; set; }
    int Frauenkek2 { get; set; }
    string sBirthMark { get; }
    string sBaptismMark { get; }
    string sDeathMark { get; }
    string sBurialMark { get; }
    [Obsolete]
    bool Ad { get; set; }
    string Ind1 { get; set; }
    Frauen Frauen_Renamed { get; }
    IList<short> Posi { get; set; }
    string Job { get; set; }
    bool reorga { get; set; }
    string sDemoVerNotPossibl { get; }
    int PersInArbsp { get; }
    string sGeocodeXMLAddress { get; }
    bool xJudenfriedhofVersion { get; set; }
    byte Datschalt { get; set; }
    [Obsolete]
    IProjectData ProjectData { get; }

    [Obsolete]
    IVBInformation Information { get; }

    void Ahnles(int PersInArb, out string[] asAhnData);
    DateTime AtomicTime(string sTimeServer);
    Image AutoSizeImage(Image oBitmap, int maxWidth, int maxHeight, bool bStretch = false);
    string Conform(string sText);
    string Datwand1(string Datu, string Ds);
    void Dateienopen();
    float Datcheck(int eArt);
    void Datles(int Ubg, int PersInArb);
    void Datles(int PersInArb, out IList<string> asPersDates);
    (string sDat_Birth, string sDat_Death) Datles(int PersInArb, IPersonData person);
    void DatPruef(int Pschalt);
    string DezRechnen(string A4, string ubgT);
    void Diskvoll();
    IEnumerable<string> EnumerateMandants(string drive);
    void Erei(int PersInArb, EEventArt eArt, ref byte PerPos);
    IList<int> Link_Famsuch(int PersInArb, ELinkKennz eLKennz);
    void Famdatles(int FamInArb, out string[] Kont);
    void Famdatles2();
    string GoogleInstallPath();
    void Info();
    short IsFormloaded(object Formtocheck);
    void Lerz(ref short A, ref int u);
    string Leerweg1(string sText);
    void OFBTextPruefenSpeichern(string UbgT, string Kennz, int LfNR);
    bool OfficeAppInstalled(MSOfficeComponent nComponent);
    string OfficeInstallPath(MSOfficeVersion nVersion);
    string ortles1(int Ortnr, byte Schalt=0,Action<int,string>? action=null);
    string ortles(int OrtNr, byte Schalt);
    string[] Ortles(IPlaceData place, int Schalt = 0, Action<int, string>? export = null);
    void Orttextspeichern();

    void Paten_O_Taufe();
    void Paten2(int PersInArb, ref string Pattext, long Ahne);
    void Perles(int PersInArb);
    [Obsolete("Auftrennen in 3 Funktionen")]
    int Persatzles(int PersInArb);
    void Person_ReadNames(int PersInArb, IPersonData person);
    int Person_TextSpeichern(string sText, int iPerson, ETextKennz eKennz1, int iLfNR = 0, short Ruf = 0);
    void Quellenaus(EEventArt L);
    string Rech(ref string datum1, ref string datum2);
    void Sichwand(string Dasich, string sDatumV_S, DateTime dDatumB, EEventArt eArt);
    void Sperrfehler();
    void STextles(string Formnam, ETextKennz Kennz, string UbgT, IList ocItems);
    int TextSpeich(string sText, string sLeitName, ETextKennz eTKennz, int PersInArb = 0, int LfNR = 0, short ruf = 0);
    void TextTeilen(string UbgT, string UbgT4, string Kennung);
    string Umlaute4(string Fld, int uml);
    void Vornam_Namles(int personNr);
    string Wortlesen(int Satz, string Kennz);

    void Zeugsu(EEventArt Art, short LfNR, short Listart, long Ahne);
    //--
    void Historie(string UbgT);
    string[] F_GetAllFiles(string sPath, int funk);
    IList<int> Ehesuch(int personNr, string Persex);
    string BuildFullSurName(IPersonData person, bool xFamToUpper = true);
    void FrmPerson_EventUpd(int PersInArb);
    void Berufles(int PersInArb, EEventArt Beruf, object combo1);
    IList<T> DeleteDoublicates<T>(IList<T> oList);
    int Eltsuch(int persInArb);
    IEnumerable<IListItem<(int, DateTime, ELinkKennz)>> Family_Kindsuch(int iFamNr);
    string Ancesters_GetPersonData(int PersonNr, out int Ahnsp, out string Kont20);
    string Ancester_GetAncesterData(int iAnc);
    void Ausdruck(string Datnam);
    bool Bildzeig1(string biart, int PBW, int PBH, string Form, out string BiText1, out string Bitext2);
    void KTextles(string Formnam, ETextKennz eTKennz, IList oIIList, (string sText, ETextKennz eTKnz) Bezeichnu);
    void ExportPlace(int OrtNr, string sOrt, string ind1, string namen);
    bool RemoveWriteProtection(string sFile);
    string Wochtag(string Datu);
    string Event_PreDisplay(bool xCitation = false, bool xWitness = false, bool xAnnotation = false, bool xBC = false, bool xReg = false);
    IDictionary<EEventArt, ((EEventArt, int, short), string, DateTime)> FamPerDatles(int PersInArb, int schalt);
    void KTextlesTL5(ETextKennz txknz, IList items, (string, ETextKennz) m_Bezeichnu);
    IListItem<int> Event_ToShortLine(IEventData cEvent);
    void FamDatLes_int(int famInArb, Action disableIllg, Action<string, int, string> setEventText);
    bool EstDateLes(out string text);
    void Listbox3Clip(IList lList, short A);
    int? FamDatYear(int FamInArb, short schalt);
    bool GeolesPlace(IPlaceData cPlace, Action<(string, string)>? action = null, bool v = true);
    void GEExportPlace(string sName, (string sLongitude, string sLatiude) cKoords, bool xAppend = true);
    string Umlaute(string Fld);
    string Umlaute_UCase(string sText);
    short Famsatzles(int FamInArb, short Rich, IFamilyData cFamily);
    IListItem<int> Famzeig(int Fam, ELinkKennz Kenn);
    void DataModul_Texte_ListDistLeitname(ETextKennz eTKennz, string UbgT, IList items);
    void Datles3(short listart, long v, object value, ref bool neb);
    void Datles10(ref short listart, bool m1_Ki);
}
