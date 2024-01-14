//using DAO;
using GenFree.Data;
using GenFree.Model;
using System;

namespace GenFree.Interfaces.Model
{
    public interface INames :
        IHasDataItf<INamesData, (int, ETextKennz, int)>,
        IUsesRecordset<(int, ETextKennz, int)>, IUsesID<(int, ETextKennz, int)>,
        IHasRSIndex1<NameIndex, NameFields>
    {
        void DeleteAllP(int persInArb);
        bool DeleteNK(int persInArb, ETextKennz kennz);
        bool ExistsNK(int persInArb, ETextKennz eTKennz);
        bool ExistText(int textNr);
        bool ReadPersonNames(int PersonNr, out int[] aiName, out (int iName, bool xRuf, bool xNick)[] aiVorns);
        void Update(int nPersNr, int nText, ETextKennz kennz, int lfNR = 0, byte calln = 0, byte nickn = 0);
    }
}