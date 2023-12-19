//using DAO;
using GenFree.Interfaces;
using GenFree.Interfaces.Model;
using System.Collections.Generic;

namespace GenFree.Data
{
    public interface IWitness : IHasDataItf<IWitnessData, (int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>, IUsesRecordset<(int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>, IUsesID<(int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>
    {
        IEnumerable<IWitnessData> ReadAllFams(int iNr, int v);
    }
}