using GenFree.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.Model
{
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
        string Pfad { get; set; }
        bool xPerson { get; }

        void NeuOrteinlesen(EEventArt Art, int Arryzaehl, string[] mararray);
        void Orteinlesen(EEventArt Art, StreamReader Sr);
    }
}
