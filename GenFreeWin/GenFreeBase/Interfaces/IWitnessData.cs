//using DAO;
using GenFree.Data;

namespace GenFree.Interfaces
{
    public interface IWitnessData : IHasID<(int iLink, int iPers, int iWKennz, EEventArt eArt, short iLfNr)>, IHasPropEnum<EWitnessProp>
    {
        int iLink { get; }
        int iPers { get; }
        int iWKennz { get; }
        EEventArt eArt { get; }
        short iLfNr { get; }
        object iPerNr { get; }
    }
}