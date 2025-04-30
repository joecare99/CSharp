using GenFree.Data;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.Sys;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GenFree.Interfaces.Model;

public interface IEinles
{
    IPlaceData Place { get; }
    ICounter<short> A300_Zaehler { get; }
    ICounter<short> A301_Zaehler1 { get; }
    ICounter<short> A302_Zaehler2 { get; }
    ICounter<short> Zaehler3 { get; }
    ICounter<short> Zaehler4 { get; }
    ICounter<short> A603_Zaehler6 { get; }
    ICounter<short> Zaehler7 { get; }
    ICounter<short> RESI_Zaehler5 { get; }
    EGedSource eGedSource { get; set; }
    IList<string> Zeug { get; }
    string Pfad { get; set; }
    string Verz_Pictures { get; set; }
    string Urfile { get; }
    string Lesfile { get; set; }
    bool xPerson { get; set; }
    int Satz { get; set; }
    string Dat { get; set; }

    short LaufNr { get; set; }
    Encoding? Um { get; set; }

    void Datsatz(EEventArt Art);
    void NeuOrteinlesen(EEventArt Art, int Arryzaehl, string[] mararray);
    void Orteinlesen(EEventArt Art, StreamReader Sr);
    void Patschr(ELinkKennz eLKennz, int iFamInArb, int iPersInArb, int iNeuer);
    int IsNumspeich(ELinkKennz eLKennz, int iFamInArb, string sGedIndID);
    void Ortespeichern(IPlaceData place);
    void Repoeinles(StreamReader Sr);

}
