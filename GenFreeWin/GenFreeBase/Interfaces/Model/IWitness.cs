//using DAO;
using GenFree.Interfaces;
using GenFree.Interfaces.Model;
using GenFree.Model;
using System.Collections.Generic;

namespace GenFree.Data
{
    public interface IWitness : 
        IHasIxDataItf<WitnessIndex, IWitnessData, (int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>,
        IHasRSIndex1<WitnessIndex, WitnessFields>,
        IUsesRecordset<(int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>, 
        IUsesID<(int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>
    {
        void DeleteAllE(int persInArb, int eWKennz);
        void DeleteAllF(int persInArb, int sWKennz);
        void DeleteAllZ(int persInArb, int sWKennz, EEventArt eArt, short iLfNr);
        bool ExistZeug(int persInArb, EEventArt eEvtArt, short lfNR, int eWKennz = 10);
        IEnumerable<IWitnessData> ReadAllFams(int iNr, int v);
    }
}